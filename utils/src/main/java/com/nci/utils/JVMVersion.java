package com.nci.utils;

import lombok.extern.slf4j.Slf4j;

@Slf4j
public class JVMVersion {
    public static int parseJDKVersion() {
        String major_version_String = "";
        int major = 0;
        // fmt: 17.0.5  | 1.8.0_352
        String java_version = System.getProperty("java.version");
        if (java_version.startsWith("1.")) {
            // fmt:  1.8.0_352
            String[] jversion_splits = java_version.split("\\.");
            if (jversion_splits.length >= 1) {
                major_version_String = jversion_splits[1];
            }
        }
        else {
            // fmt: 17.0.5
            String[] jversion_splits = java_version.split("\\.");
            if (jversion_splits.length >= 2) {
                major_version_String = jversion_splits[0];
            }

        }
        if (major_version_String !=null && major_version_String.length() >= 0) {
            major = Integer.parseInt(major_version_String);
        }

        log.info("found jvm_major_version={}", major);
        return major;
    }
    
}
