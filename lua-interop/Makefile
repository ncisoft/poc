lua_incdir=$(shell luarocks config --lua-incdir | tr -d '\n')
lua_libdir=$(shell luarocks config --lua-libdir | tr -d '\n')
init_0 := $(shell test -f .init0 || touch .init0)

CFLAGS=-g -O2 -std=c99 -Wall  -I${lua_incdir}
LDFLAGS=-L${lua_libdir} -llua -lm -Wl,-E -ldl -lreadline

.PHONY: build xbuild clean env run xrun

build: .init1
	make -C src \
		CFLAGS="${CFLAGS}" \
		LDFLAGS="${LDFLAGS}" \
		build

xbuild: .init1
	touch src/log.h && \
	make -C src \
		CFLAGS="${CFLAGS}" \
		LDFLAGS="${LDFLAGS}" \
		build

.init1: .init0
	touch $@

clean:
	make -C src clean
	rm -rf .init?

env:
	@echo  "incdir=${lua_incdir}"
	@echo  "libdir=${lua_libdir}"

run: build
	@echo " "
	./src/t

xrun: xbuild
	@echo " "
	./src/t



