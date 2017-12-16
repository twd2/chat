#ifndef _CHAT_SERVER_SESSION_H_
#define _CHAT_SERVER_SESSION_H_

#include <netinet/in.h>

class session
{
private:
    int sock;
    sockaddr_in sin;

public:
    session(int sock, sockaddr_in sin)
        : sock(sock), sin(sin)
    {
    }

    void handle();
};

#endif // _CHAT_SERVER_CONFIG_H_
