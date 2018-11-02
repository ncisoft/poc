.PHONY: build  clean run r  help

build: mem-layout raii

mem-layout: mem-layout.c
	gcc -o $@ $<

raii: raii.c
	gcc -o $@ $<

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
