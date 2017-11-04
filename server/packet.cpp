#include "packet.h"
#include "util.h"

#include <cstdlib>

#include <sys/socket.h>
#include <arpa/inet.h>

namespace packet
{
    void ntoh(head *p)
    {
        p->size = ntohs(p->size);
        p->type = ntohs(p->type);
    }

    void ntoh(hello *p)
    {
        ntoh(&(p->h));
    }

    void ntoh(login_req *p)
    {
        ntoh(&(p->h));
    }

    void ntoh(login_res *p)
    {
        ntoh(&(p->h));
        p->code = ntohs(p->code);
    }

    void hton(head *p)
    {
        p->size = htons(p->size);
        p->type = htons(p->type);
    }

    void hton(hello *p)
    {
        hton(&(p->h));
    }

    void hton(login_req *p)
    {
        hton(&(p->h));
    }

    void hton(login_res *p)
    {
        hton(&(p->h));
        p->code = htons(p->code);
    }

    void *recv(int sock, ssize_t *err)
    {
        uint16_t size = 0;
        int result = safe_recv(sock, &size, sizeof(size));
        if (result <= 0)
        {
            if (err)
            {
                *err = result;
            }
            return nullptr;
        }
        size = ntohs(size);
        void *p = malloc(size);
        ((head *)p)->size = htons(size);
        result = safe_recv(sock, (uint8_t *)p + sizeof(size), size - sizeof(size));
        if (result <= 0)
        {
            if (err)
            {
                *err = result;
            }
            free(p);
            return nullptr;
        }
        return p;
    }

    ssize_t send(int sock, void *p)
    {
        return safe_send(sock, p, ntohs(((head *)p)->size));
    }
}

