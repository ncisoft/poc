package com.nci.apps;

import com.nci.utils.GCChecker;
import com.nci.utils.Helper;
import com.nci.utils.IdentifyJvmArgs;
import com.nci.utils.Timming;

import static com.nci.utils.Helper.*;

import java.io.IOException;
import java.util.List;

// CodeGenHelper
import com.nci.utils.CodeGenHelper;
import com.nci.utils.FakeException;
import com.nci.utils.FakeRAII;

import lombok.extern.slf4j.Slf4j;

@Slf4j
public class App {
    static class Foo {
        int id = 0;
        String name = "leeyg";
        String[] names = { "leeyg", "doom" };

        public String getName() {
            return name;
        }

        public int getId() {
            return id;
        }

        public String[] getNames() {
            return names;
        }
    }

    public static void test02() {
        CodeGenHelper.parseTimingTemplate(com.nci.utils.IdentifyJvmArgs.class);
    }
    public static void test01(Object o) {

        try (Timming t = new Timming("[allocate objects]")) {
            boolean doLoop = true;
            App.Foo foo = new App.Foo();
            log.debug("--allocate objects");
            log.debug("--hashCode = {}", o.hashCode());
            for (long i = 0; doLoop && i < 10000; i++) {
                int[] intList = new int[100 * 100 * 5];
                Helper.sleep(0);
            }
            // System.out.println(VM.current().details());
        }
        catch (FakeException e) {}


        /*
        String xx = ClassLayout.parseInstance(o).toPrintable();
        println(xx);
        println("0x3d = 0b{0}", Integer.toBinaryString(0x3d));
        println("0x7d = 0b{0}", Integer.toBinaryString(0x7d));
        long end_t = System.currentTimeMillis();
        log.debug("elapsed jol ={}",end_t - start_t);
         * 
         */
        
        //isTenuredGenerationByUnsafe(o);
        //isTenuredGenerationByUnsafe(foo);
        // pressEnterToContinue();


    }
    public static void main(String[] args) {
//        GCChecker holder = new GCChecker();
        FakeRAII raii = new FakeRAII(() -> {
            log.info("outer resources was closed");
        });


        //try (raii) {
        //    Helper.nop();
        //} catch (Exception e) {
        //    e.printStackTrace();
        //}
        App.Foo foo = new App.Foo();
        App.Foo foo2 = new App.Foo();
        Integer foo_int = 30;
        /*
        dump_var("foo", foo);
        getCallerStackTraceElement();
        log.debug("Hello World!");
        List<String> list = IdentifyJvmArgs.getArgsList();
        dump_var("vm_args", list);
        String classpath = System.getProperty("java.class.path");
//        pressEnterToContinue();
        // log.debug(classpath);
        System.out.println("-----");
        try {
            String pid = getPid();
            log.debug("\tpid = {} !!!", pid);
            println();
        } catch (IOException e) {
            e.printStackTrace();

        }
        */
        log.debug("-----");
        test01(foo2);
        log.debug("-----");
        test02();
        println();

    }
}
