#ifndef _CHAT_SERVER_GLOBAL_H_
#define _CHAT_SERVER_GLOBAL_H_

#include "chat.pb.h"

#include <list>
#include <memory>
#include <mutex>
#include <map>
#include <queue>

#include <openssl/ssl.h>

class session;

class global
{
public:
    static std::list<std::shared_ptr<session> > sessions;
    static std::mutex sessions_mtx;
    static std::map<uint32_t, std::shared_ptr<session> > user_sessions;
    static std::mutex user_sessions_mtx;
    static std::map<uint32_t, std::queue<Message> > pending_messages;
    static std::mutex pending_messages_mtx;
    static UserDatabase users;
    static std::mutex users_mtx;
    static void load_users();
    static void save_users();
    static bool has_session(uint32_t uid);
    
    static std::mutex ssl_ctx_mtx;
    static SSL_CTX *ssl_ctx;
    static void init_ssl();
    static void release_ssl();
};

#endif // _CHAT_SERVER_GLOBAL_H_
