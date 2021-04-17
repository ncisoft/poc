.PHONY : help build clean rebuild

help:
	@echo "Usage: make help | clean | build | rebuild"
	@echo ""

clean:
	scons -c
	rm -rf build

build:
	scons -Q

rebuild:
	scons -c
	scons
