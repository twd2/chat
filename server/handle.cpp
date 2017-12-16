#include "handle.h"
#include "session.h"

void handle(int sock, sockaddr_in sin)
{
    session sess(sock, sin);
    sess.handle();
}


