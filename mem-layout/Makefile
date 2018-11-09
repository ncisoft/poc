.PHONY: build  clean run r  help

build: mem-layout raii

mem-layout: mem-layout.c
	$(CC) -o $@ -Wall -std=c99 $<

raii: raii.c
	$(CC) -o $@ -Wall -std=c99 $<

clean:
	@true && \
		test -f mem-layout && rm -f mem-layout && \
		test -f raii && rm -f raii && \
		echo "is clean" || echo "is clean.."

run: build
	./mem-layout

r: run

help:
	@echo "make build | clean | run | r | help"
