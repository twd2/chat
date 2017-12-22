include config.make

ifdef DEBUG
DBG = -g
else
DBG = -O2
endif

ifeq ($(shell uname),Linux)
	# compiler
	CC = g++
	INC_DIR = 
	CC_FLAGS = $(DBG) -Wall -fno-strict-aliasing -std=gnu++11 $(INC_DIR)

	# linker
	LD = g++
	LIB_DIR = 
	LD_LIBS = -lstdc++ -lc -lm -ldl -lpthread -lprotobuf -lssl -lcrypto
	LD_FLAGS = $(LIB_DIR)
else

ifeq ($(shell uname),Darwin)
	# compiler
	INC_DIR = 
	CC_FLAGS = $(DBG) -Wall -fno-strict-aliasing -std=gnu++11 $(INC_DIR)

	# linker
	LD = cc
	LIB_DIR = 
	LD_LIBS = -lc++ -lm -lSystem -lpthread -lprotobuf -lssl -lcrypto
	LD_FLAGS = $(LIB_DIR)
else
	# Unsupported operating system.
	CC = echo && echo "******** Unsupported operating system! ********" && echo && exit 1 ||
endif

endif

.PHONY: all
all: main
	

main: main.o chat.pb.o $(OBJECTS)
	$(LD) $(LD_FLAGS) -o $@ $^ $(LD_LIBS)

main.o: main.cpp $(HEADERS)
	$(CC) $(CC_FLAGS) -o $@ -c $<

%.o: %.cpp %.h
	$(CC) $(CC_FLAGS) -o $@ -c $<

.PHONY: clean
clean:
	-$(RM) *.o main
