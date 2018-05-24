include ~/conf/make/android.mk
.PHONY:  all clean help


all: fork-cost thread-cost

fork-cost: fork-cost.c
	${CC} -o $@ ${CFLAGS} -std=c99 -O2 -Wall $?

thread-cost: thread-cost.c
	${CC} -o $@ ${CFLAGS} -std=c99 -O2 -Wall $? $(libpthread)

clean:
	rm -rf *.o fork-cost thread-cost

help:
	@echo "make CROSS=[ arm | x86 ]"
	@echo "make clean && make CROSS=[ arm | x86 ]"
	@echo "xcross=$(xcross) libpthread=$(libpthread)"

