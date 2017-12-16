#ifndef _CHAT_PACKET_H_
#define _CHAT_PACKET_H_

#include <cstddef>
#include <stdint.h>
#include <cstring>
#include <string>

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

#endif // _CHAT_PACKET_H_
