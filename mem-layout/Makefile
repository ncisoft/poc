.PHONY: build  clean run r  help

build: mem-layout

mem-layout: mem-layout.c
	gcc -o $@ $<

clean:
	@test -f mem-layout && rm -f mem-layout && echo "is clean" || echo "is clean.."

run: build
	./mem-layout

r: run

help:
	@echo "make build | clean | run | r | help"
