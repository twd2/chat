#include <cstdlib>
#include <cstring>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <thread>
#include <vector>
#include <memory>

void error_and_exit(const char *msg)
{
    perror(msg);
    exit(1);
}

void handle(int sock, sockaddr_in sin)
{
    const char *buffer = "hello, world\n";
    send(sock, buffer, strlen(buffer), 0);
    // shutdown(sock, SHUT_RDWR);
    char buff[1024];
    recv(sock, buff, 1024, 0);
    buff[1023] = '\0';
}

int main()
{
    const int on = 1;
    int server_sock = socket(AF_INET, SOCK_STREAM, 0);
    if (server_sock < 0)
    {
        error_and_exit("create server socket");
    }
    setsockopt(server_sock, SOL_SOCKET, SO_REUSEADDR, &on, sizeof(on));
    sockaddr_in sin;
    sin.sin_family = AF_INET;
    sin.sin_port = htons(1025);
    sin.sin_addr.s_addr = inet_addr("0.0.0.0");
    if (::bind(server_sock, (sockaddr *)&sin, sizeof(sin)) < 0)
    {
        error_and_exit("bind");
    }
    if (listen(server_sock, 100) < 0)
    {
        error_and_exit("listen");
    }

    std::vector<std::shared_ptr<std::thread> > threads;
    while (true)
    {
        sockaddr_in client_sin;
        socklen_t sin_len = sizeof(client_sin);
        int client_sock = accept(server_sock, (sockaddr *)&client_sin, &sin_len);
        if (client_sock < 0)
        {
            error_and_exit("accept");
        }
        threads.push_back(std::make_shared<std::thread>(handle, client_sock, client_sin));
    }
    return 0;
}