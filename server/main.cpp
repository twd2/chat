#include <cstdlib>
#include <cstring>

#include <unistd.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netinet/tcp.h>
#include <arpa/inet.h>
#include <signal.h>

#include <iostream>
#include <fstream>
#include <thread>
#include <list>
#include <memory>

#include "config.h"
#include "handle.h"
#include "chat.pb.h"
#include "global.h"

void error_and_exit(const char *msg)
{
    perror(msg);
    exit(1);
}

void ctrl_c_handler(int)
{
    std::cout << "Exiting..." << std::endl;
    exit(0);
}

int main()
{
    GOOGLE_PROTOBUF_VERIFY_VERSION;
    signal(SIGINT, ctrl_c_handler);
    signal(SIGPIPE, SIG_IGN);
    
    global::init_ssl();
    
    global::load_users();

    int server_sock = socket(AF_INET, SOCK_STREAM, 0);
    if (server_sock < 0)
    {
        error_and_exit("create server socket");
    }

    const int on = 1;
    setsockopt(server_sock, SOL_SOCKET, SO_REUSEADDR, &on, sizeof(on));

    sockaddr_in listen_sin;
    listen_sin.sin_family = AF_INET;
    listen_sin.sin_addr.s_addr = inet_addr(LISTEN_ADDR);
    listen_sin.sin_port = htons(LISTEN_PORT);
    if (::bind(server_sock, (sockaddr *)&listen_sin, sizeof(listen_sin)) < 0)
    {
        error_and_exit("bind");
    }

    if (listen(server_sock, 100) < 0)
    {
        error_and_exit("listen");
    }

    std::list<std::shared_ptr<std::thread> > threads;
    while (true)
    {
        sockaddr_in client_sin;
        socklen_t sin_len = sizeof(client_sin);
        int client_sock = accept(server_sock, (sockaddr *)&client_sin, &sin_len);
        if (client_sock < 0)
        {
            error_and_exit("accept");
        }

        // set keepalive
        int keepIdle = 5;
        int keepInterval = 5;
        int keepCount = 9;
        setsockopt(client_sock, SOL_TCP, TCP_KEEPIDLE, &keepIdle, sizeof(keepIdle));
        setsockopt(client_sock, SOL_TCP, TCP_KEEPINTVL, &keepInterval, sizeof(keepInterval));
        setsockopt(client_sock, SOL_TCP, TCP_KEEPCNT, &keepCount, sizeof(keepCount));
        setsockopt(client_sock, SOL_SOCKET, SO_KEEPALIVE, &on, sizeof(on));
        // no SIGPIPE
        // setsockopt(client_sock, SOL_SOCKET, SO_NOSIGPIPE, &on, sizeof(on));

        char remote_addr[INET6_ADDRSTRLEN];
        inet_ntop(AF_INET, &(client_sin.sin_addr), remote_addr, sizeof(remote_addr));
        unsigned short remote_port = ntohs(client_sin.sin_port);
        std::cout << std::dec << "[main] Accept " << remote_addr << ":" << remote_port << std::endl;
        
        // TODO: clean dummy threads
        threads.push_back(std::make_shared<std::thread>(handle, client_sock, client_sin));
    }
    global::release_ssl();
    return 0;
}
