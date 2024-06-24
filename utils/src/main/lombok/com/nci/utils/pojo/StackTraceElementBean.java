package com.nci.utils.pojo;

import java.util.ArrayList;
import java.util.HashMap;
import java.io.*; 
import java.util.List;
import java.util.Map;

import lombok.Builder;
import lombok.Data;
import lombok.extern.slf4j.Slf4j;

import com.nci.utils.*;

@Data
@Builder
@Slf4j
public class StackTraceElementBean implements Serializable{
    // For Throwables and StackWalker, the VM initially sets this field to a
    // reference to the declaring Class.  The Class reference is used to
    // construct the 'format' bitmap, and then is cleared.
    //
    // For STEs constructed using the public constructors, this field is not used.
    private String classLoaderName;
    /**
     * The declaring class.
     */
    private String className;
    /**
     * The source file name.
     */
    private String fileName;
    /**
     * The method name.
     */
    private String methodName;
    /**
     * The source line number.
     */
    private int    lineNumber;

    public static StackTraceElementBean transform(StackTraceElement e) {
        return StackTraceElementBean
                .builder()
                .className(e.getClassName())
                .fileName(e.getFileName())
                .lineNumber(e.getLineNumber())
                .methodName(e.getMethodName())
                .build();
    }

    public static StackTraceElementBean[] transform(StackTraceElement[] es) {
        List<StackTraceElementBean> list = new ArrayList<StackTraceElementBean>();
        for (StackTraceElement e : es) {
            list.add(transform(e));
        }
        StackTraceElementBean[] array = new StackTraceElementBean[list.size()];
        list.toArray(array);

        return array;
    }

    private String getSimpleClassName() {
        String name = getClassName();
        int i = name.lastIndexOf(".");
        if (i >=0) {
            name = name.substring(i+1);
        }
        return  name;
    }
     private static StringTemplate mLogInfoTpl = StringTemplate.newInstance("{{fileName}}:{{{methodName}}}():{{lineNumber}}");
    public String toLogInfo() {
        Map<String, String> params = new HashMap<String, String>() {{
            put("className", getSimpleClassName());
            put("fileName", getFileName());
            put("methodName", getMethodName());
            put("lineNumber", "" + getLineNumber());
        }};

        String out = mLogInfoTpl.render(params);
        return out;
    }

    private static StringTemplate mShortTpl = StringTemplate.newInstance("{{className}}:{{{methodName}}}():{{lineNumber}}");
    public String toShortInfo() {
        Map<String, String> params = new HashMap<String, String>() {{
            put("className", getSimpleClassName());
            put("methodName", getMethodName());
            put("lineNumber", "" + getLineNumber());
        }};

        String out =mShortTpl.render(params);
        return out;
    }
}

