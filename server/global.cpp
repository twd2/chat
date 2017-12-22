#include "global.h"
#include "config.h"

#include <fstream>

#include <openssl/ssl.h>
#include <openssl/err.h>

std::list<std::shared_ptr<session> > global::sessions;
std::mutex global::sessions_mtx;
std::map<uint32_t, std::shared_ptr<session> > global::user_sessions;
std::mutex global::user_sessions_mtx;
std::map<uint32_t, std::queue<Message> > global::pending_messages;
std::mutex global::pending_messages_mtx;

UserDatabase global::users;
std::mutex global::users_mtx;

void global::load_users()
{
    std::fstream in_file(USER_DATABASE_FILE, std::ios::in | std::ios::binary);
    if (!users.ParseFromIstream(&in_file))
    {
        std::cerr << "Failed to parse user database, initializing..." << std::endl;
        users.set_maxuid(0);
        users.clear_users();
        save_users();
    }
}

void global::save_users()
{
    std::fstream out_file(USER_DATABASE_FILE, std::ios::out | std::ios::trunc | std::ios::binary);
    users.SerializeToOstream(&out_file);
}

bool global::has_session(uint32_t uid)
{
    auto iter = global::user_sessions.find(uid);
    return iter != global::user_sessions.end();
}

std::mutex global::ssl_ctx_mtx;
SSL_CTX *global::ssl_ctx = nullptr;
void global::init_ssl()
{
    SSL_load_error_strings();
    SSL_library_init();
    OpenSSL_add_all_algorithms();

    ssl_ctx = SSL_CTX_new(TLSv1_2_server_method());
    if (!ssl_ctx) {
        ERR_print_errors_fp(stderr);
        exit(1);
    }

    if (SSL_CTX_use_certificate_file(ssl_ctx, CERT_FILE, SSL_FILETYPE_PEM) <= 0) {
        ERR_print_errors_fp(stderr);
        exit(1);
    }

    if (SSL_CTX_use_PrivateKey_file(ssl_ctx, KEY_FILE, SSL_FILETYPE_PEM) <= 0) {
        ERR_print_errors_fp(stderr);
        exit(1);
    }

    if (!SSL_CTX_check_private_key(ssl_ctx)) {
        fprintf(stderr, "Private key does not match the certificate public key\n");
        exit(1);
    }
}

void global::release_ssl()
{
    SSL_CTX_free(ssl_ctx);
    ssl_ctx = nullptr;

    ERR_free_strings();
    EVP_cleanup();
}
