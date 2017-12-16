#include "session.h"
#include "packet.h"
#include "util.h"

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

    std::string msg = "hello, ";
    msg = msg + remote_addr + "!";
    send_packet(sock, msg, PACKET_RAW);

    while (true)
    {
        packet_type_t type;
        std::string buffer = recv_packet(sock, type);
        if (buffer.empty())
        {
            log() << "broken" << std::endl;
            break;
        }
        send_packet(sock, buffer, type);
    }
    shutdown(sock, SHUT_RDWR);
    close(sock);
    log() << "shutdown and close" << std::endl;
}
