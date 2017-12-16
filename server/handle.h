#ifndef _CHAT_SERVER_HANDLE_H_
#define _CHAT_SERVER_HANDLE_H_

#include <netinet/in.h>

void handle(int sock, sockaddr_in sin);

#endif // _CHAT_SERVER_HANDLE_H_
