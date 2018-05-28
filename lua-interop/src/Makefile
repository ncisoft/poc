
.PHONY: build clean 

OBJS = t.o
build: t

t: ${OBJS}
	${CC} -o $@ $? ${LDFLAGS}

t.o: t.c log.h

.c.o: log.h
	${CC} ${CFLAGS} -o $@ -c $<

clean: 
	rm -rf t *.o


