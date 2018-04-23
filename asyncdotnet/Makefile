.PHONY: all run r build


include ~/conf/make/rules.mk

exe=./cs-etcd/bin/Debug/cs-etcd.exe
msbuild := $(shell which msbuild || which xbuild)

all: .build

.build: .phony_target

	test -f ${exe} && rm ${exe} || echo ""
	${msbuild} cs-etcd.sln /maxcpucount /p:Configuration=Debug;TargetFrameworkVersion=v4.5.1

	test -f ${exe}
	mono  --debug=casts  -O=all --aot ${exe}
	chmod +x ${exe}

	touch $@

run: .build r

r: 
	mono  --debug=casts ./cs-etcd/bin/Debug/cs-etcd.exe major=marksweep-par-fixed,major-heap-size=2g
