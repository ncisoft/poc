.PHONY: all run r build clean


include ~/conf/make/rules.mk

exe=./cs-etcd/bin/Debug/cs-etcd.exe
ifdef RT
msbuild := $(shell which ${RT})
else
msbuild := $(shell which dotnet || which msbuild || which xbuild)
endif

xdotnet := $(shell which dotnet || echo "...dotnet")
xmsbuild := $(shell which msbuild || echo "...msbuild")
xxbuild := $(shell which xbuild || echo "...xbuild")

all: .build

.build: .phony_target
	@echo $(msbuild)
ifeq ($(msbuild), $(xdotnet))
	find . -name "*.dll" -exec rm {} \;
#	dotnet build
	dotnet build /p:Configuration=Debug;TargetFrameworkVersion=v4.6.1
	find . -name "*.dll" -exec cp {} $@ \;
	test -f $@ && truncate -s 0 $@
else

	test -f ${exe} && rm ${exe} || echo ""
	${msbuild} cs-etcd.sln /maxcpucount /p:Configuration=Debug;TargetFrameworkVersion=v4.5.1

	test -f ${exe}
	mono  --debug=casts  -O=all --aot ${exe}
	chmod +x ${exe}

	touch $@
endif

run: .build r

r: 
ifeq ($(msbuild), $(xdotnet))
	dotnet run -p src/main.csproj 
else
	mono  --debug=casts ./cs-etcd/bin/Debug/cs-etcd.exe major=marksweep-par-fixed,major-heap-size=2g
endif

clean:
	dotnet msbuild ./src/async.csproj  /t:clean
	rm  -rf .build .phony_target src/obj src/bin
