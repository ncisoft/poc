package com.nci.apps;

import java.util.List;

import org.slf4j.helpers.MessageFormatter;

import com.nci.utils.pojo.StackTraceElementBean;

import java.text.MessageFormat;
import java.util.Arrays;


import static com.nci.utils.Helper.*;

public class HelloWorld {


    public static void dumpClassList() {
        List<String> classList = Arrays.asList(null
             ,com.nci.utils.IdentifyJvmArgs.class.getName()
             ,StackTraceElementBean.class.getName()
             ,com.nci.apps.mock.Caller.class.getCanonicalName()
             ,com.nci.utils.Helper.class.getName()
             ,null
             );

             println("-- dump class list --");
             classList = elimateEmptyString(classList);
             for (String className : classList) {
                 println("{0}", className);
             }
             println("--content dump class list end--");
             println();

    }
    public static void main( String[] args ) {
        
        dumpClassList();
        System.out.printf("%s\n", "Hello World");
    }
}
