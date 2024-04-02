package com.nci.utils;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.Stack;
import java.util.stream.Collectors;

import org.apache.commons.lang3.StringUtils;
import org.slf4j.Logger;
import com.google.common.collect.ImmutableMap;
import oshi.SystemInfo;
import oshi.hardware.HardwareAbstractionLayer;
import oshi.hardware.CentralProcessor;

public class IdentifyJvmArgs {
    public static final Logger logger = LogBuilder.getLogger((IdentifyJvmArgs.class));

    private static int getPhysicalProcessorCount() {
        SystemInfo systemInfo = new SystemInfo();
        HardwareAbstractionLayer hardwareAbstractionLayer = systemInfo.getHardware();
        CentralProcessor centralProcessor = hardwareAbstractionLayer.getProcessor();
        return centralProcessor.getPhysicalProcessorCount();
    }

    private static String geParallelGCThreadsArgs() {
        String out;

        StringTemplate tpl = StringTemplate.newInstance("-XX:ParallelGCThreads={{core_count}}");
        Map<String, Integer> tplParams =
                ImmutableMap.of("core_count", getPhysicalProcessorCount());
        out = tpl.render(tplParams);
        logger.info("out=" + out);
        return "";
    }

    private static List<String> elimateEmptyString(List<String> list) {

        list = list.stream()
                .filter(s -> s instanceof String && s.length() > 0)
                .collect(Collectors.toList());
        return list;
    }

    public static  List<String> getArgsList() {
       List<String> jvmArgsList;



        if (JVMVersion.parseJDKVersion() <= 8) {
            // 流行的并发标记清除 (CMS) GC 算法在 JDK 9 中已弃用
            // https://medium.com/tier1app-com/cms-deprecated-in-jdk-9-5d2918f36257
            jvmArgsList = Arrays.asList(
                    "-Xms2G", "-Xmx2G",
                    "-XX:+UseConcMarkSweepGC",
                    //"-XX:NewRatio=1",
                    //"-XX:SurvivorRatio=8",
                    "-XX:ConcGCThreads=2",

                    "-XX:+PrintGCDetails",
                    "-XX:+PrintGCTimeStamps",
                    "-XX:+PrintTenuringDistribution",
                    "-Xloggc:/tmp/gc_memory_logs.log",
                    "-XX:+UseStringDeduplication",
                    "-XX:MaxTenuringThreshold=15",
                    "-XX:+PrintTenuringDistribution",
                    geParallelGCThreadsArgs(),
                    ""
                    );
        }
        else {
            // JVM >= 1.9, prefer G1Gc
            jvmArgsList = Arrays.asList(null,
                    "-server",
                    "-Xms1024m",
                    "-Xmx1024m",
                    //"-XX:NewRatio=1",
                    //"-XX:SurvivorRatio=8",
                    "-XX:+UseG1GC",
                    "-Xlog:gc:file=/tmp/gc_memory_logs.log",
                    "-XX:+UseStringDeduplication",
                    "-XX:ConcGCThreads=2",
                    geParallelGCThreadsArgs(),
                    //"-Xlog:gc*,gc+ref=debug,gc+heap=debug,gc+age=trace:file=/tmp/gc_memory_logs.log",
                    //"-Xlog:gc*,gc+ref=info,gc+heap=info,gc+age=info:file=/tmp/gc_memory_logs.log",
                    ""
                    );

        }
        jvmArgsList = elimateEmptyString(jvmArgsList);
        return jvmArgsList;
    }

    public static String[] getArgsArray() {
        List<String> jvmArgsList = getArgsList();
        String[] jvmArgsArray = new String[jvmArgsList.size()];
        jvmArgsList.toArray(jvmArgsArray);
        return jvmArgsArray;
    }

    public static String getVMArgs() {
        List<String> jvmArgsList = getArgsList();
        String line = StringUtils.join(jvmArgsList, " ");
        return line;
    }

    public static void main(String[] args) {
        String vmArgs = getVMArgs();
        String[] xargs = getArgsArray();
        vmArgs = String.format("--jvmArgs '%s'\n", vmArgs);
        System.out.println(xargs);
        System.out.println(vmArgs);
        final Logger logger = LogBuilder.getLogger(IdentifyJvmArgs.class);
        logger.info("0000");

    }
}
