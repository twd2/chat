# DEBUG = 1
LS = ls
RM = rm -fr
CP = cp
SLASH = /

ifeq ($(shell uname),Linux)
	DSO_POSTFIX = so
else

ifeq ($(shell uname),Darwin)
	DSO_POSTFIX = dylib
else
	# May be Windows
	LS = dir /b
	RM = del /f
	CP = copy
	SLASH = \\
	DSO_POSTFIX = dll
endif

endif

SOURCES := $(shell $(LS) *.cpp)
HEADERS := $(shell $(LS) *.h *.hpp)
OBJECTS := $(patsubst %.cpp, %.o, $(SOURCES))