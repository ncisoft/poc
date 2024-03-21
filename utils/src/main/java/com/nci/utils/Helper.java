package com.nci.utils;

import java.text.MessageFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Stack;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.nci.utils.pojo.StackTraceElementPOJO;

import lombok.extern.slf4j.Slf4j;


@Slf4j
public class Helper {
    private static final Helper mHelper = new Helper();

    private Helper() {
    }

    public Helper newInstance() {
        return mHelper;
    }

    public static void nop(Object o) {
        if (o != null) {

        }
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

    public static StackTraceElementPOJO getCallerStackTraceElement() {
        return getCallerStackTraceElement(1);
    }

    public static StackTraceElementPOJO getCallerStackTraceElement(int level) {
        RuntimeException ex = new RuntimeException();
        StackTraceElement[] stackElements = ex.getStackTrace();
        StackTraceElement _callerStackTraceElement = stackElements[level];
        StackTraceElementPOJO callerStackTraceElement = StackTraceElementPOJO.transform(_callerStackTraceElement);
//        Helper.dump_var("call stack element", callerStackTraceElement);

        return callerStackTraceElement;
    }

    public static StackTraceElementPOJO[] getCallerStackTraceElementList() {
        RuntimeException ex = new RuntimeException();
        StackTraceElement[] _stackElements = ex.getStackTrace();
        StackTraceElementPOJO[] stackElements = StackTraceElementPOJO.transform(_stackElements);

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
        StackTraceElementPOJO callerStack = getCallerStackTraceElement(2);
        log.debug("{} dump obj {} <{}>", callerStack.toLogInfo(), tag, o.getClass().getCanonicalName());
        System.out.printf("%s\n", jsonContent);
    }
}
