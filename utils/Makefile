
CFLAGS = -fPIC  -shared \
	-I ${JAVA_HOME}/include/linux \
	-I ${JAVA_HOME}/include \

JNI_NAME=helper
git-dir := $(shell git rev-parse --show-toplevel | tr -d '\n')
JAVA_OPTS_INTERPRETED_MODE = -Xint #interpreted mode
JAVA_OPTS = -XX:+TieredCompilation -XX:TieredStopAtLevel=1
JAVA_OPTS = -Xint  -Xshare:on -Xms256m -Xmx256m
JAVA_OPTS = -Xint  -Xms256m -Xmx256m -Djdk.attach.allowAttachSelf -XX:-UseCompressedOops
JAVA_OPTS = -Xint  -Xms256m -Xmx256m  -XX:-UseCompressedOops

.PHONY: help
help:
	@echo "make [ build | classpath | touch | app | helloworld | clean | jmh | vmargs | jdkflags"
	@echo ". ./etc/vscode/activate"

.PHONY: build
build:
	@./bin/cmp-src.sh target/javautils.jar ./src
	@free -h


.PHONY: touch
touch:
	@find ./src/main/java/com/nci/apps/ -name "*.java" -type f -exec touch {} \;

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

# TODO: @drip $(JAVA_OPTS) -cp $(shell cat ./etc/cp-buster-lxd.txt) com.nci.apps.App
.PHONY: app
app: build
	@echo '................ mvn exec:java -Dexec.mainClass="com.nci.apps.App" -Dexec.cleanupDaemonThreads=false'
	@echo '>>> ................ drip ..............................' | ~/conf/bash/hl_words.lua java drip END
	@time  java $(JAVA_OPTS) -cp $(shell cat ./etc/cp-buster-lxd.txt) com.nci.apps.App
	@echo
	@echo '>>> ................ END ..............................' | ~/conf/bash/hl_words.lua java drip END
	@echo '................ mvn exec:java -Dexec.mainClass="com.nci.apps.App" -Dexec.cleanupDaemonThreads=false'
	@echo "................ JAVA_OPTS = ${JAVA_OPTS}"
# @echo sudo -H  ${JAVA_HOME}/bin/java $(JAVA_OPTS) -cp $(shell cat ./etc/cp-buster-lxd.txt) com.nci.apps.App

.PHONY: classpath
classpath: build
	@mvn exec:exec -Dexec.executable=echo -Dexec.args="%classpath"

.PHONY: vmargs
vmargs: build
	@java $(JAVA_OPTS) -cp $(shell cat cp.txt) com.nci.utils.IdentifyJvmArgs
	@echo
	@java -XX:+PrintFlagsFinal -version|grep UseCompressedOops


build/lib$(JNI_NAME).so: src/main/jni/com_nci_utils_Helper.c
	cd build && make
# $(CC) $(CFLAGS) -o $(JNI_NAME).so com_nci_utils_Helper.c 

.PHONY: jni
jni: build  build/lib$(JNI_NAME).so
	test -d /usr/java/packages/lib || sudo mkdir -p /usr/java/packages/lib
	sudo cp -uv build/libhelper.so /usr/java/packages/lib/lib$(JNI_NAME).so
