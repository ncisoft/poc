.PHONY : build help build clean app vmargs touch jdkflags

git-dir := $(shell git rev-parse --show-toplevel | tr -d '\n')

help:
	@echo "make [ build | classpath | touch | app | clean | jmh | vmargs | jdkflags"
	@echo ". ./etc/vscode/activate"

build:
	@./bin/cmp-src.sh target/javautils.jar ./src
	@free -h


touch:
	@find ./src -name "*" -type f -exec touch {} \;

clean:
	@mvn clean

test:
	@mvn test

jdkflags:
	@java -XX:+PrintFlagsFinal -version | grep Threads

	#java -cp ./target/javautils.jar com.nci.apps.App
app:
	@java -cp $(shell cat cp.txt) com.nci.apps.App

classpath:
	@mvn exec:exec -Dexec.executable=echo -Dexec.args="%classpath"
vmargs:
	java -cp ./target/benchmarks.jar com.bettercodebytes.Utils.IdentifyJvmArgs

