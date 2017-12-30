# chat
Yet Another Instant Messaging.

## Features

* Instant Messaging
* File Transportation
* Shared Canvas
* Inputting Hint
* Buddy Discovery
* TLS support
* Fantastic GUI

## Prerequisites

* [protobuf 3.5.0+](https://github.com/google/protobuf/releases)
* `libssl-dev` (Ubuntu)
* `openssl-dev` (CentOS)
* C++ 11

## Building

### Server

1. Install prerequisites.
2. `make` protobuf files.
3. `cd server && make` server on Linux.

### Client

1. `make` protobuf files.
2. Build client just using Visual Studio 2015+ on Windows.

## Note

For design details, see: `report.md` or `report.pdf`.