#ifndef _CHAT_PACKET_H_
#define _CHAT_PACKET_H_

#include "chat.pb.h"

#include <cstddef>
#include <stdint.h>
#include <cstring>
#include <string>
#include <iostream>

#include <sys/types.h>

#include <openssl/ssl.h>

#define PACKET_LOGIN 0
#define PACKET_REGISTER 1
#define PACKET_LIST_USER 2
#define PACKET_LIST_BUDDY 3
#define PACKET_ADD_BUDDY 4
#define PACKET_REMOVE_BUDDY 5
#define PACKET_MESSAGE 6
#define PACKET_RAW 254
#define PACKET_RESET 255

typedef uint8_t packet_type_t;

std::string recv_packet(SSL *sock, packet_type_t &type, ssize_t *err = nullptr);
ssize_t send_packet(SSL *sock, const std::string &buffer, packet_type_t type);

template <typename T> ssize_t  __send_packet(SSL *sock, const T &p, packet_type_t type)
{
    std::string buffer;
    if (!p.SerializeToString(&buffer))
    {
        std::cerr << "???" << std::endl;
        exit(1);
    }
    return send_packet(sock, buffer, type);
}

inline ssize_t send_packet(SSL *sock, const LoginResponse &p)
{
    return __send_packet(sock, p, PACKET_LOGIN);
}

inline ssize_t send_packet(SSL *sock, const RegisterResponse &p)
{
    return __send_packet(sock, p, PACKET_REGISTER);
}

inline ssize_t send_packet(SSL *sock, const ListUserResponse &p)
{
    return __send_packet(sock, p, PACKET_LIST_USER);
}

inline ssize_t send_packet(SSL *sock, const ListBuddyResponse &p)
{
    return __send_packet(sock, p, PACKET_LIST_BUDDY);
}

inline ssize_t send_packet(SSL *sock, const AddBuddyResponse &p)
{
    return __send_packet(sock, p, PACKET_ADD_BUDDY);
}

inline ssize_t send_packet(SSL *sock, const RemoveBuddyResponse &p)
{
    return __send_packet(sock, p, PACKET_REMOVE_BUDDY);
}

inline ssize_t send_packet(SSL *sock, const Message &p)
{
    return __send_packet(sock, p, PACKET_MESSAGE);
}

inline ssize_t send_packet(SSL *sock, const Reset &p)
{
    return __send_packet(sock, p, PACKET_RESET);
}

#endif // _CHAT_PACKET_H_
