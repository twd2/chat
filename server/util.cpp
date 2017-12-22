#include "util.h"

#include <sys/socket.h>

#include <iostream>
#include <algorithm>
#include <thread>

ssize_t safe_recv(int sock, void *buf, std::size_t len)
{
    std::size_t read = 0;
    while (read < len)
    {
        ssize_t result = recv(sock, (uint8_t *)buf + read, std::min(CHUNK_SIZE, len - read), 0);
        if (result <= 0)
        {
            return result;
        }
        read += result;
    }
    return read;
}

ssize_t safe_send(int sock, const void *buf, std::size_t len)
{
    std::size_t sent = 0;
    while (sent < len)
    {
        ssize_t result = send(sock, (uint8_t *)buf + sent, std::min(CHUNK_SIZE, len - sent), 0);
        if (result <= 0)
        {
            return result;
        }
        sent += result;
    }
    return sent;
}

ssize_t ssl_safe_recv(SSL *sock, void *buf, std::size_t len)
{
    std::size_t read = 0;
    while (read < len)
    {
        ssize_t result = SSL_read(sock, (uint8_t *)buf + read, std::min(CHUNK_SIZE, len - read));
        if (result <= 0)
        {
            return result;
        }
        read += result;
    }
    return read;
}

ssize_t ssl_safe_send(SSL *sock, const void *buf, std::size_t len)
{
    std::size_t sent = 0;
    while (sent < len)
    {
        ssize_t result = SSL_write(sock, (uint8_t *)buf + sent, std::min(CHUNK_SIZE, len - sent));
        if (result <= 0)
        {
            return result;
        }
        sent += result;
    }
    return sent;
}

std::ostream &log()
{
    return std::cout << "[thread 0x" << std::hex << std::this_thread::get_id() << "] ";
}
