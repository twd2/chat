#ifndef _CHAT_PACKET_H_
#define _CHAT_PACKET_H_

#include "chat.pb.h"

#include <cstddef>
#include <stdint.h>
#include <cstring>
#include <string>
#include <iostream>

#include <sys/types.h>

#define PACKET_LOGIN 0
#define PACKET_REGISTER 1
#define PACKET_LIST_USER 2
#define PACKET_MESSAGE 3
#define PACKET_RAW 254
#define PACKET_RESET 255

typedef uint8_t packet_type_t;

std::string recv_packet(int sock, packet_type_t &type, ssize_t *err = nullptr);
ssize_t send_packet(int sock, const std::string &buffer, packet_type_t type);

template <typename T> ssize_t  __send_packet(int sock, const T &p, packet_type_t type)
{
    std::string buffer;
    if (!p.SerializeToString(&buffer))
    {
        std::cerr << "???" << std::endl;
        exit(1);
    }
    return send_packet(sock, buffer, type);
}

inline ssize_t send_packet(int sock, const LoginResponse &p)
{
    return __send_packet(sock, p, PACKET_LOGIN);
}

inline ssize_t send_packet(int sock, const RegisterResponse &p)
{
    return __send_packet(sock, p, PACKET_REGISTER);
}

inline ssize_t send_packet(int sock, const ListUserResponse &p)
{
    return __send_packet(sock, p, PACKET_LIST_USER);
}

inline ssize_t send_packet(int sock, const Message &p)
{
    return __send_packet(sock, p, PACKET_MESSAGE);
}

inline ssize_t send_packet(int sock, const Reset &p)
{
    return __send_packet(sock, p, PACKET_RESET);
}

#endif // _CHAT_PACKET_H_
