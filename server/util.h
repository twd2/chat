#ifndef _CHAT_UTIL_H_
#define _CHAT_UTIL_H_

#include <cstddef>
#include <stdint.h>
#include <sys/types.h>
#include <iostream>

#include <openssl/ssl.h>

#define CHUNK_SIZE ((std::size_t)4096)

ssize_t safe_recv(int sock, void *buf, std::size_t len);
ssize_t safe_send(int sock, const void *buf, std::size_t len);
ssize_t ssl_safe_recv(SSL *sock, void *buf, std::size_t len);
ssize_t ssl_safe_send(SSL *sock, const void *buf, std::size_t len);

std::ostream &log();

#endif // _CHAT_UTIL_H_
