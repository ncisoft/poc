.PHONY: build  clean run r  help

build: mem-layout raii backtrace

mem-layout: mem-layout.c
	$(CC) -o $@ -Wall -std=c99 $<

raii: raii.c
	$(CC) -o $@ -Wall -std=c99 $<

backtrace: backtrace.c
	$(CC) -o $@ -Wall -std=c99 -rdynamic $<

clean:
	@true && \
		test -f mem-layout && rm -f mem-layout && \
		test -f raii && rm -f raii && \
		test -f backtrace && rm -f backtrace && \
		echo "is clean" || echo "is clean.."

run: build
	./mem-layout

r: run

help:
	@echo "make build | clean | run | r | help"
