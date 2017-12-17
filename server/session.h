#ifndef _CHAT_SERVER_SESSION_H_
#define _CHAT_SERVER_SESSION_H_

#include "chat.pb.h"

#include <list>
#include <memory>
#include <mutex>

#include <netinet/in.h>

class session
{
private:
    std::list<std::shared_ptr<session> >::iterator self_iter;
    int sock;
    sockaddr_in sin;
    uint32_t uid = 0;
    std::mutex mtx;
    
    void set_uid(uint32_t new_uid);

public:
    bool is_alive = true;
    session(int sock, sockaddr_in sin)
        : sock(sock), sin(sin)
    {
    }
    
    ~session();
    
    void set_iter(std::list<std::shared_ptr<session> >::iterator iter)
    {
        self_iter = iter;
    }

    void handle();
    void handle_login(LoginRequest &q);
    void handle_register(RegisterRequest &q);
    void handle_list_user(ListUserRequest &q);
    void handle_list_buddy(ListBuddyRequest &q);
    void handle_add_buddy(AddBuddyRequest &q);
    void handle_remove_buddy(RemoveBuddyRequest &q);
    void handle_message(Message &q);
    void handle_reset(Reset &q);
    void kick(bool send_msg = false);
};

#endif // _CHAT_SERVER_SESSION_H_
