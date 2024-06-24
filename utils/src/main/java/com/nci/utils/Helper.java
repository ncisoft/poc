package com.nci.utils;

import java.lang.reflect.*;
//import sun.misc.Unsafe;
import java.util.Scanner;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.lang.management.ManagementFactory;
import java.lang.reflect.Field;
import java.net.URL;
import java.nio.file.Files;
import java.nio.charset.StandardCharsets;
import java.text.MessageFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Stack;

import javax.management.MBeanServer;
import com.sun.management.HotSpotDiagnosticMXBean;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.nci.utils.pojo.StackTraceElementBean;

import lombok.extern.slf4j.Slf4j;


@Slf4j
public class Helper {
    private static final Helper mHelper = new Helper();
    //private static Unsafe unsafe = getUnsafe();

    private Helper() {
    }

    public static String getPid() throws IOException {
        String path = new File("/proc/self/stat").getCanonicalPath();
        // ! Format: /proc/173256/stat
        log.debug("\tpid = {}", path);

        int endPos = path.lastIndexOf("/");
        int beginPos = path.indexOf("/", 2);
        log.debug("\tbeginPos={}, lastPos={}", beginPos, endPos);
        path = path.substring(beginPos+1, endPos);
        log.debug("\tpid = {}", path);
        return path;
    }

    public static void pressEnterToContinue() {
        System.out.println("Press Enter key to continue...");
        try {
            Scanner scanner = new Scanner(System.in);
            System.in.read();
            //scanner.nextLine();
        } 
        catch (Exception e) {
        }
    }

    public Helper newInstance() {
        return mHelper;
    }

    public static void  loadJNI() {
//        System.load("/usr/java/packages/lib/helper.so");
        System.loadLibrary("helper");
    }

    public static int  getMaxTenuringThreshold() {
        try {
            MBeanServer server = ManagementFactory.getPlatformMBeanServer();
            HotSpotDiagnosticMXBean bean = ManagementFactory.newPlatformMXBeanProxy(
                    server,
                    "com.sun.management:type=HotSpotDiagnostic",
                    HotSpotDiagnosticMXBean.class);
            int threshold = Integer.valueOf(bean.getVMOption("MaxTenuringThreshold").getValue());
            return threshold;
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
    }

    public static void sleep(long millis) {
        try {
            Thread.currentThread().sleep(millis);
        } catch (InterruptedException e) {
            // TODO: handle exception
        }
    }
    public static boolean getBoolean(boolean b) {
        return b;
    }

    public static String getResource(String resourceName) {
        StringBuilder sb = new StringBuilder(1000);

        try {
        URL url = Helper.class.getClassLoader().getResource(resourceName);
        File file = new File(url.toURI());
        List<String> lines;
        lines = Files.readAllLines(file.toPath(), StandardCharsets.UTF_8);
        for (String line : lines) {
            sb.append(line);
            sb.append("\n");
            
        }

        } catch (Exception e) {
             e.printStackTrace();
        }
        return sb.toString();


    }

    /*
     * 
    public static Unsafe getUnsafe() {
        try {
            Field theUnsafe = Unsafe.class.getDeclaredField("theUnsafe");
            theUnsafe.setAccessible(true);
            Unsafe unsafe = (Unsafe) theUnsafe.get(null);
            return unsafe;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
     */

    public static int  getTenuredGenerationByUnsafe(Object o) {
        if (getBoolean(false)) {
            ;
            // 获取对象的年龄
            //int objectAge = unsafe.getByte(o, 0L) & 0x78;
            //return objectAge;
            return 20;
        } else {
            return 20; 
        }
    }

    public static boolean  isTenuredGenerationByUnsafe(Object o) {
        // 获取对象的年龄
        int objectAge = getTenuredGenerationByUnsafe(o);

        // 获取应用程序的年龄阈值
        int threshold =15;// Integer.valueOf(bean.getVMOption("MaxTenuringThreshold").getValue());
        threshold = getMaxTenuringThreshold();

        // 判断对象是否在老年代
        log.debug("objAge = {}", objectAge);
        log.debug("MaxTenuringThreshol= {}", threshold);
        if (objectAge >= threshold) {
            // 对象在老年代
            log.debug("Object is in the Old Generation");
            return true;
        } else {
            // 对象在新生代
            log.debug("Object is in the Young Generation");
            return false;
        }
    }

    public static native void getAge(Object o);
    public static native void getAgeByAddress(long address);

    public static void nop(Object o) {
        if (o != null) {
        }
    }
    public static void nop(int  _int) {
        _int = 0;
    }


    public static void nop() {
        nop(Helper.class);
    }

    public static void println(String format, Object... arguments) {
        String content = MessageFormat.format(format, arguments);
        System.out.println(content);
    }

    public static void println(String message) {
        System.out.println(message);
    }

    public static void println() {
        System.out.println("");
    }

    public static StackTraceElementBean getCallerStackTraceElement() {
        return getCallerStackTraceElement(1);
    }

    public static StackTraceElementBean getCallerStackTraceElement(int level) {
        RuntimeException ex = new RuntimeException();
        StackTraceElement[] stackElements = ex.getStackTrace();
        StackTraceElement _callerStackTraceElement = stackElements[level];
        StackTraceElementBean callerStackTraceElement = StackTraceElementBean.transform(_callerStackTraceElement);
//        Helper.dump_var("call stack element", callerStackTraceElement);

        return callerStackTraceElement;
    }

    public static StackTraceElementBean[] getCallerStackTraceElementList() {
        RuntimeException ex = new RuntimeException();
        StackTraceElement[] _stackElements = ex.getStackTrace();
        StackTraceElementBean[] stackElements = StackTraceElementBean.transform(_stackElements);

        return stackElements;
    }

    public static List<String> elimateEmptyString(List<String> list) {
        Stack<Integer> indexStack = new Stack<Integer>();
        list = new ArrayList<String>(list);

        for (int i = 0; i < list.size(); i++) {
            String e = list.get(i);
            if (e == null || e.length() == 0) {
                indexStack.push(i);
                log.trace("found empty element: {}", i);
            }
        }
        while (!indexStack.empty()) {
            Integer index = indexStack.pop();
            list.remove(index.intValue());
        }
        return list;
    }

    public static void dump_var(String tag, Object o) {
        Gson gson = new GsonBuilder()
                .setPrettyPrinting()
                .create();
        String jsonContent = gson.toJson(o);
        StackTraceElementBean callerStack = getCallerStackTraceElement(2);
        log.debug("{} dump obj {} <{}>", callerStack.toLogInfo(), tag, o.getClass().getCanonicalName());
        System.out.printf("%s\n", jsonContent);
    }
}
