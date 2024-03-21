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
public class StackTraceElementPOJO implements Serializable{
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

    public static StackTraceElementPOJO transform(StackTraceElement e) {
        return StackTraceElementPOJO
                .builder()
                .classLoaderName(e.getClassLoaderName())
                .className(e.getClassName())
                .fileName(e.getFileName())
                .lineNumber(e.getLineNumber())
                .methodName(e.getMethodName())
                .build();
    }

    public static StackTraceElementPOJO[] transform(StackTraceElement[] es) {
        List<StackTraceElementPOJO> list = new ArrayList<StackTraceElementPOJO>();
        for (StackTraceElement e : es) {
            list.add(transform(e));
        }
        StackTraceElementPOJO[] array = new StackTraceElementPOJO[list.size()];
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
    public String toLogInfo() {
        Map<String, String> params = new HashMap<String, String>() {{
            put("className", getSimpleClassName());
            put("fileName", getFileName());
            put("methodName", getMethodName());
            put("lineNumber", "" + getLineNumber());
        }};
        StringTemplate tpl = StringTemplate.newInstance("{{fileName}}:{{{methodName}}}():{{lineNumber}}");

        String out =tpl.render(params);
        return out;
    }
}
