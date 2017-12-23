#!/bin/bash

openssl genrsa -out key.pem 2048
openssl req -new -key key.pem -out csr.pem \
  -subj "/C=CN/O=twd2/CN=chat.server"
openssl x509 -req -in csr.pem -extfile v3.ext -out cert.pem -signkey key.pem -days 3652 -sha256
