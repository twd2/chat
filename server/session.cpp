#include "session.h"
#include "packet.h"
#include "util.h"
#include "chat.pb.h"
#include "global.h"

#include <cstdlib>
#include <cstring>

#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>

#include <iostream>
#include <string>

void session::handle()
{
    char remote_addr[INET6_ADDRSTRLEN];
    inet_ntop(AF_INET, &(sin.sin_addr), remote_addr, sizeof(remote_addr));
    unsigned short remote_port = ntohs(sin.sin_port);
    log() << "Started handle for " << std::dec << remote_addr << ":" << remote_port << std::endl;

    std::string msg = "This is a chat server. Hello, ";
    msg = msg + remote_addr + "!";
    send_packet(sock, msg, PACKET_RAW);

    while (is_alive)
    {
        ssize_t result = 1;
        packet_type_t type;
        std::string buffer = recv_packet(sock, type, &result);
        if (result <= 0)
        {
            log() << "broken" << std::endl;
            break;
        }
        bool invalid = false;
        
        // dispatch
        switch (type)
        {
#define CASE(packet_type, type, handler) \
            case packet_type: \
            { \
                type q; \
                if (!q.ParseFromString(buffer)) \
                { \
                    invalid = true; \
                    break; \
                } \
                handle_##handler(q); \
                break; \
            }
            CASE(PACKET_LOGIN, LoginRequest, login)
            CASE(PACKET_REGISTER, RegisterRequest, register)
            CASE(PACKET_LIST_USER, ListUserRequest, list_user)
            CASE(PACKET_LIST_BUDDY, ListBuddyRequest, list_buddy)
            CASE(PACKET_ADD_BUDDY, AddBuddyRequest, add_buddy)
            CASE(PACKET_REMOVE_BUDDY, RemoveBuddyRequest, remove_buddy)
            CASE(PACKET_MESSAGE, Message, message)
            CASE(PACKET_RESET, Reset, reset)
#undef CASE
            case PACKET_RAW:
                // TODO
                break;
            default:
            {
                invalid = true;
                break;
            }
        }
        if (invalid)
        {
            log() << "invalid type or bad data" << std::endl;
            Reset r;
            r.set_code(Reset::PROTOCOL_MISMATCH);
            r.set_msg("invalid type or bad data");
            send_packet(sock, r);
            break;
        }
    }

    kick();
}

void session::handle_login(LoginRequest &q)
{
    log() << "handling login!" << std::endl;
    if (uid)
    {
        kick(true);
        return;
    }

    LoginResponse r;
    
    uint32_t new_uid = 0;
    
    {
        std::unique_lock<std::mutex> lock(global::users_mtx);
        bool found = false;
        for (const UserDatabase::User &u : global::users.users())
        {
            if (q.username() == u.username())
            {
                if (q.password() == u.password())
                {
                    r.set_code(LoginResponse::SUCCESS);
                    r.set_uid(u.uid());
                    new_uid = u.uid();
                }
                else
                {
                    r.set_code(LoginResponse::PASSWORD_ERROR);
                    r.set_uid(0);
                }
                found = true;
                break;
            }
        }
        if (!found)
        {
            r.set_code(LoginResponse::USER_NOT_FOUND);
            r.set_uid(0);
        }
    }
    
    set_uid(new_uid);

    send_packet(sock, r);
}

void session::handle_register(RegisterRequest &q)
{
    log() << "handling register!" << std::endl;
    if (uid)
    {
        kick(true);
        return;
    }
    
    RegisterResponse r;

    {
        std::unique_lock<std::mutex> lock(global::users_mtx);
        bool found = false;
        for (const UserDatabase::User &u : global::users.users())
        {
            if (q.username() == u.username())
            {
                r.set_code(RegisterResponse::USER_EXISTS);
                r.set_uid(0);
                found = true;
                break;
            }
        }
        if (!found)
        {
            uint32_t new_uid = global::users.maxuid() + 1;
            global::users.set_maxuid(new_uid);
            UserDatabase::User &new_user = *global::users.add_users();
            new_user.set_uid(new_uid);
            new_user.set_username(q.username());
            new_user.set_password(q.password());
            global::save_users();
            r.set_code(RegisterResponse::SUCCESS);
            r.set_uid(new_uid);
        }
    }

    send_packet(sock, r);
}

void session::handle_list_user(ListUserRequest &q)
{
    log() << "handling list user!" << std::endl;
    if (!uid)
    {
        kick(true);
        return;
    }

    ListUserResponse r;
    
    {
        std::unique_lock<std::mutex> lock(global::users_mtx);
        std::unique_lock<std::mutex> lock2(global::user_sessions_mtx);
        for (const UserDatabase::User &u : global::users.users())
        {
            ListUserResponse::User &ru = *r.add_users();
            ru.set_uid(u.uid());
            ru.set_username(u.username());
            ru.set_online(global::has_session(u.uid()));
        }
    }
    
    send_packet(sock, r);
}


void session::handle_list_buddy(ListBuddyRequest &q)
{
    log() << "handling list buddy!" << std::endl;
}

void session::handle_add_buddy(AddBuddyRequest &q)
{
    log() << "handling add buddy!" << std::endl;
}

void session::handle_remove_buddy(RemoveBuddyRequest &q)
{
    log() << "handling remove buddy!" << std::endl;
}

void session::handle_message(Message &q)
{
    log() << "handling message!" << std::endl;
    if (!uid)
    {
        kick(true);
        return;
    }

    uint32_t dest_uid = q.uid();
    q.set_uid(uid);
    // TODO: do forward
}

void session::handle_reset(Reset &q)
{
    log() << "handling reset!" << std::endl;
    kick();
}

void session::set_uid(uint32_t new_uid)
{
    if (uid == new_uid)
    {
        return;
    }

    std::unique_lock<std::mutex> lock(global::user_sessions_mtx);
    auto iter = global::user_sessions.find(new_uid);
    if (iter != global::user_sessions.end())
    {
        std::unique_lock<std::mutex> lock(iter->second->mtx);
        iter->second->kick(true);
    }
    global::user_sessions.erase(uid);
    uid = new_uid;
    global::user_sessions[new_uid] = *self_iter;
}

session::~session()
{
    log() << "~session::session()" << std::endl;
}

void session::kick(bool send_msg)
{
    if (!is_alive)
    {
        return;
    }

    if (send_msg)
    {
        Reset r;
        r.set_code(Reset::KICKED);
        send_packet(sock, r);
    }
    global::user_sessions.erase(uid);
    is_alive = false;
    shutdown(sock, SHUT_RDWR);
    close(sock);
    log() << "shutdown and close" << std::endl;
}
