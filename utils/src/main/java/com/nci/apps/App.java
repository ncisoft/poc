package com.nci.apps;

import com.nci.utils.GCChecker;
import com.nci.utils.Helper;
import com.nci.utils.IdentifyJvmArgs;

import static com.nci.utils.Helper.*;

import java.util.List;

import com.nci.utils.FakeRAII;

import lombok.extern.slf4j.Slf4j;

@Slf4j
public class App 
{
    static class Foo {
        int id=0;
        String name="leeyg";
        String[] names = {"leeyg", "doom"};

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

    public static void main( String[] args )
    {
        GCChecker holder = new GCChecker();
        FakeRAII raii = new FakeRAII(() -> {
            log.info("outer resources was closed");
        });


        try (raii) {
            Helper.nop();
        } catch (Exception e) {
            e.printStackTrace();
        }
        nop(holder);
        App.Foo foo = new App.Foo();
        dump_var("foo", foo);
        getCallerStackTraceElement(); 
        log.debug( "Hello World!" );
        List<String> list = IdentifyJvmArgs.getArgsList();
        dump_var("vm_args", list);
        String classpath = System.getProperty("java.class.path");
//        log.debug(classpath);

    }
}
