.PHONY : build help build clean app vmargs touch jdkflags

git-dir := $(shell git rev-parse --show-toplevel | tr -d '\n')
JAVA_OPTS_INTERPRETED_MODE = -Xint #interpreted mode
JAVA_OPTS = -XX:+TieredCompilation -XX:TieredStopAtLevel=1
JAVA_OPTS = -Xint  -Xshare:on -Xms256m -Xmx256m
JAVA_OPTS =  -Xms256m -Xmx256m

help:
	@echo "make [ build | classpath | touch | app | helloworld | clean | jmh | vmargs | jdkflags"
	@echo ". ./etc/vscode/activate"

build:
	@./bin/cmp-src.sh target/javautils.jar ./src
	@free -h


.PHONY: touch
touch:
	@find ./src -name "*" -type f -exec touch {} \;

.PHONY: clean
clean:
	@mvn clean

.PHONY: test
test:
	@mvn test

.PHONY: jdkflags
jdkflags:
	@java $(JAVA_OPTS) -XX:+PrintFlagsFinal -version | grep Threads

	#java -cp ./target/javautils.jar com.nci.apps.App

.PHONY: HelloWorld
HelloWorld: build
	@echo
	@time java $(JAVA_OPTS_INTERPRETED_MODE) -cp $(shell cat cp.txt)  com.nci.apps.HelloWorld

.PHONY: app
app: build
	@echo '................ mvn exec:java -Dexec.mainClass="com.nci.apps.App" -Dexec.cleanupDaemonThreads=false'
	@echo '>>> ................ drip ..............................' | ~/conf/bash/hl_words.lua java drip END
	@time drip $(JAVA_OPTS) -cp $(shell cat cp.txt) com.nci.apps.App
	@echo '>>> ................ END ..............................' | ~/conf/bash/hl_words.lua java drip END

.PHONY: classpath
classpath: build
	@mvn exec:exec -Dexec.executable=echo -Dexec.args="%classpath"

.PHONY: vmargs
vmargs: build
	@java $(JAVA_OPTS) -cp $(shell cat cp.txt) com.nci.utils.IdentifyJvmArgs

