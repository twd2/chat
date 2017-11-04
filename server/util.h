#ifndef _CHAT_UTIL_H_
#define _CHAT_UTIL_H_

#include <cstddef>
#include <stdint.h>
#include <sys/types.h>

#define CHUNK_SIZE ((std::size_t)4096)

ssize_t safe_recv(int sock, void *buf, std::size_t len);
ssize_t safe_send(int sock, const void *buf, std::size_t len);

#endif // _CHAT_UTIL_H_
