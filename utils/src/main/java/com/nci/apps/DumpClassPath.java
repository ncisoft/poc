package com.nci.apps;

import lombok.extern.slf4j.Slf4j;
import java.util.Arrays;
import java.io.*;
import java.net.URL;
import java.net.URLClassLoader;
import java.lang.management.ManagementFactory;
import java.lang.management.RuntimeMXBean;


@Slf4j
public class DumpClassPath {
    public static void main( String[] args )
    {
        String classpathStr = System.getProperty("java.class.path");
        log.debug(classpathStr);
        System.out.println(classpathStr);

        System.out.println();

        System.out.println("----");
         
    }
}
