#include "packet.h"
#include "util.h"

#include <cstdlib>
#include <string>

#include <sys/socket.h>
#include <arpa/inet.h>

std::string recv_packet(SSL *sock, packet_type_t &type, ssize_t *err)
{
    uint32_t size = 0;

    // read and check size
    ssize_t result = ssl_safe_recv(sock, &size, sizeof(size));
    if (result <= 0)
    {
        if (err)
        {
            *err = result;
        }
        return "";
    }
    size = ntohl(size);
    // TODO: check if size is too big
    
    // read type
    // assert sizeof(type) == 1
    result = ssl_safe_recv(sock, &type, sizeof(type));
    if (result <= 0)
    {
        if (err)
        {
            *err = result;
        }
        return "";
    }
    
    log() << "received a packet sized " << std::dec << size << "bytes, type=" << (int)type << ", reading payload..." << std::endl;
    
    // FIXME: ugly!
    if (type != PACKET_LOGIN && type != PACKET_REGISTER && type != PACKET_LIST_USER &&
        type != PACKET_LIST_BUDDY && type != PACKET_ADD_BUDDY && type != PACKET_REMOVE_BUDDY &&
        type != PACKET_MESSAGE && type != PACKET_RAW && type != PACKET_RESET)
    {
        return "";
    }
    
    if (size == 0)
    {
        return "";
    }

    // read payload
    std::string buffer;
    buffer.resize(size);
    result = ssl_safe_recv(sock, &buffer[0u], size);
    if (result <= 0)
    {
        if (err)
        {
            *err = result;
        }
        return "";
    }
    return buffer;
}

ssize_t send_packet(SSL *sock, const std::string &buffer, packet_type_t type)
{
    log() << "sending" << std::endl;
    // send size
    uint32_t size = buffer.size();
    size = htonl(size);
    ssize_t result = ssl_safe_send(sock, &size, sizeof(size));
    if (result <= 0)
    {
        return result;
    }
    
    // send type
    // assert sizeof(type) == 1
    result = ssl_safe_send(sock, &type, sizeof(type));
    if (result <= 0)
    {
        return result;
    }

    // send payload
    result = ssl_safe_send(sock, buffer.data(), buffer.size());
    if (result <= 0)
    {
        return result;
    }
    return sizeof(size) + sizeof(type) + buffer.size();
}

