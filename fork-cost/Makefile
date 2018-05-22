include ~/conf/make/android.mk
.PHONY:  all clean help

all: fork-cost

fork-cost: fork-cost.c
	${CC} -o $@ ${CFLAGS} -std=c99 -O2 -Wall $?

clean:
	rm -rf *.o fork-cost

help:
	@echo "make CROSS=[ arm | x86 ]"
	@echo "make clean && make CROSS=[ arm | x86 ]"

