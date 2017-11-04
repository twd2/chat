#ifndef _CHAT_PACKET_H_
#define _CHAT_PACKET_H_

#include <cstddef>
#include <stdint.h>
#include <cstring>

#include <sys/types.h>

#define PACKET_MAGIC "chatserv"
#define PACKET_TYPE_HELLO 0
#define PACKET_TYPE_LOGIN_REQ 1
#define PACKET_TYPE_LOGIN_RES 2

#define PACKET_LOGIN_SUCCEEDED 0
#define PACKET_LOGIN_USER_NOT_FOUNT 1
#define PACKET_LOGIN_BAD_PASSWORD 2

namespace packet
{
#pragma pack(push, 1)
    struct head
    {
        uint16_t size; // whole packet size
        uint16_t type;

        head(uint16_t size, uint16_t type)
            : size(size), type(type)
        {
        }
    };

    struct hello
    {
        head h;
        char magic[8];
        char message[0];

        hello(uint16_t message_size)
            : h(sizeof(hello) + message_size, PACKET_TYPE_HELLO)
        {
            memcpy(magic, PACKET_MAGIC, sizeof(PACKET_MAGIC));
        }
    };

    struct login_req
    {
        head h;
        char username[64];
        char password[64];
    };

    struct login_res
    {
        head h;
        uint16_t code;
        
        login_res(uint16_t code)
            : h(sizeof(login_res), PACKET_TYPE_LOGIN_RES), code(code)
        {
        }
    };
#pragma pack(pop)

    void ntoh(head *p);
    void ntoh(hello *p);
    void ntoh(login_req *p);
    void ntoh(login_res *p);

    void hton(head *p);
    void hton(hello *p);
    void hton(login_req *p);
    void hton(login_res *p);

    void *recv(int sock, ssize_t *err);
    ssize_t send(int sock, void *p);
}

using packet::ntoh;
using packet::hton;

#endif // _CHAT_PACKET_H_
