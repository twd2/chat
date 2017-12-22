#!/bin/bash

openssl genrsa -out key.pem 2048
openssl req -new -key key.pem -out csr.pem \
  -subj "/C=CN/ST=N-A/O=twd2/CN=chat.server"
openssl x509 -req -in csr.pem -out cert.pem -signkey key.pem -days 3650 -sha256
