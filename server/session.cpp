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
    Reset rst;
    rst.set_code(Reset::UNKNOWN_ERROR);
    rst.set_msg(msg);
    send_packet(sock, rst);

    while (is_alive)
    {
        packet_type_t type;
        std::string buffer = recv_packet(sock, type);
        if (buffer.empty())
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
            send_packet(sock, "invalid type or bad data", PACKET_RAW);
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
    if (q.username() == "twd2" && q.password() == "123456")
    {
        r.set_code(LoginResponse::SUCCESS);
        r.set_uid(1);
    }
    else
    {
        r.set_code(LoginResponse::PASSWORD_ERROR);
        r.set_uid(0);
    }
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
    r.set_code(RegisterResponse::USER_EXISTS);
    r.set_uid(0);
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
    send_packet(sock, r);
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
        send_packet(sock, "kicked!", PACKET_RAW);
    }
    global::user_sessions.erase(uid);
    is_alive = false;
    shutdown(sock, SHUT_RDWR);
    close(sock);
    log() << "shutdown and close" << std::endl;
}
