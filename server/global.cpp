#include "global.h"
#include "config.h"

#include <fstream>

std::list<std::shared_ptr<session> > global::sessions;
std::mutex global::sessions_mtx;
std::map<uint32_t, std::shared_ptr<session> > global::user_sessions;
std::mutex global::user_sessions_mtx;
std::map<uint32_t, std::queue<Message> > global::pending_messages;
std::mutex global::pending_messages_mtx;

UserDatabase global::users;
std::mutex global::users_mtx;

void global::load_users()
{
    std::fstream in_file(USER_DATABASE_FILE, std::ios::in | std::ios::binary);
    if (!users.ParseFromIstream(&in_file))
    {
        std::cerr << "Failed to parse user database, initializing..." << std::endl;
        users.set_maxuid(0);
        users.clear_users();
        save_users();
    }
}

void global::save_users()
{
    std::fstream out_file(USER_DATABASE_FILE, std::ios::out | std::ios::trunc | std::ios::binary);
    users.SerializeToOstream(&out_file);
}

bool global::has_session(uint32_t uid)
{
    auto iter = global::user_sessions.find(uid);
    return iter != global::user_sessions.end();
}
