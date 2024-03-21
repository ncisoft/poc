package com.nci.apps;

import com.nci.utils.GCChecker;
import com.nci.utils.Helper;
import static com.nci.utils.Helper.*;

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

        nop(holder);
        App.Foo foo = new App.Foo();
        dump_var("foo", foo);
        getCallerStackTraceElement(); 
        log.debug( "Hello World!" );
    }
}
