#ifndef _CHAT_SERVER_HANDLE_H_
#define _CHAT_SERVER_HANDLE_H_

#include <netinet/in.h>

#include <iostream>

void handle(int sock, sockaddr_in sin);
std::ostream &log();

#endif // _CHAT_SERVER_HANDLE_H_
