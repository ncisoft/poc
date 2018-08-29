.PHONY: all run r build clean


include ~/conf/make/rules.mk

ifdef RT
msbuild := $(shell which ${RT})
else
msbuild := $(shell which dotnet || which msbuild || which xbuild)
endif

xdotnet := $(shell which dotnet || echo "...dotnet")
xmsbuild := $(shell which msbuild || echo "...msbuild")
xxbuild := $(shell which xbuild || echo "...xbuild")

ifeq ($(msbuild), $(xdotnet))
  exe=./cs-etcd/bin/Debug/cs-etcd.exe
else
  exe := ./src/bin/Debug/asyncdotnet.exe
endif

all: .build

.build: .phony_target
	@echo $(msbuild)
ifeq ($(msbuild), $(xdotnet))
	find . -name "*.dll" -exec rm {} \;
#	dotnet build
	dotnet build asyncdotnet.sln /p:Configuration=Debug;TargetFrameworkVersion=v4.6.1
	find . -name "*.dll" -exec cp {} $@ \;
	test -f $@ && truncate -s 0 $@
else

	test -f ${exe} && rm ${exe} || echo ""
	${msbuild} asyncdotnet-mono.sln
       #	/maxcpucount /p:Configuration=Debug;TargetFrameworkVersion=v4.5.1

	test -f ${exe}
	#mono  --debug=casts  -O=all --aot ${exe}
	chmod +x ${exe}

	touch $@
endif

run: .build r

r: 
ifeq ($(msbuild), $(xdotnet))
	dotnet run -p src/async.csproj 
else
	mono  --debug=casts ${exe} major=marksweep-par-fixed,major-heap-size=2g
endif

clean:
ifeq ($(msbuild), $(xdotnet))
	dotnet msbuild asyncdotnet.sln   /t:clean
	rm  -rf .build .phony_target src/obj src/bin
else
	${msbuild} asyncdotnet-mono.sln /t:clean
	rm  -rf .build .phony_target src/obj src/bin
endif
