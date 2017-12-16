#include "handle.h"
#include "global.h"
#include "session.h"

#include <list>
#include <iterator>

void handle(int sock, sockaddr_in sin)
{
    std::shared_ptr<session> sess = std::make_shared<session>(sock, sin);
    std::list<std::shared_ptr<session> >::iterator iter;

    {
        std::unique_lock<std::mutex> lock(global::sessions_mtx);
        global::sessions.push_back(sess);
        iter = global::sessions.end();
    }

    --iter;
    sess->set_iter(iter);

    sess->handle();

    {
        std::unique_lock<std::mutex> lock(global::sessions_mtx);
        global::sessions.erase(iter);
    }
}

