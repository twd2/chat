#include "handle.h"
#include "packet.h"

#include <cstdlib>
#include <cstring>

#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>

#include <iostream>
#include <thread>
#include <string>

void handle(int sock, sockaddr_in sin)
{
    char remote_addr[INET6_ADDRSTRLEN];
    inet_ntop(AF_INET, &(sin.sin_addr), remote_addr, sizeof(remote_addr));
    unsigned short remote_port = ntohs(sin.sin_port);
    log() << "Started handle for " << std::dec << remote_addr << ":" << remote_port << std::endl;

    std::string msg = "hello, ";
    msg = msg + remote_addr + "!";
    packet::hello *p = (packet::hello *)malloc(sizeof(packet::hello) + msg.length() + 1);
    new (p) packet::hello(msg.length() + 1);
    strcpy(p->message, msg.c_str());
    hton(p);
    packet::send(sock, p);
    free(p);
    
    while (true)
    {
        void *pp = packet::recv(sock, nullptr);
        if (!pp)
        {
            log() << "broken" << std::endl;
            break;
        }
        packet::send(sock, pp);
        free(pp);
    }
    shutdown(sock, SHUT_RDWR);
    close(sock);
    log() << "shutdown and close" << std::endl;
}

std::ostream &log()
{
    return std::cout << "[thread 0x" << std::hex << std::this_thread::get_id() << "] ";
}


