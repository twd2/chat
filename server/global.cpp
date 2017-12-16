#include "global.h"

std::list<std::shared_ptr<session> > global::sessions;
std::mutex global::sessions_mtx;
std::map<uint32_t, std::shared_ptr<session> > global::user_sessions;
std::mutex global::user_sessions_mtx;
std::map<uint32_t, std::queue<Message> > global::pending_messages;
std::mutex global::pending_messages_mtx;
