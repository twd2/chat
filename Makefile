.PHONY: all
all: client/ChatClient/Chat.cs server/chat.pb.cc

client/ChatClient/Chat.cs: chat.proto
	protoc chat.proto --csharp_out=client/ChatClient

server/chat.pb.cc: chat.proto
	protoc chat.proto --cpp_out=server

.PHONY: clean
clean:
	-$(RM) -f client/ChatClient/Chat.cs
	-$(RM) -f server/chat.pb.cc
