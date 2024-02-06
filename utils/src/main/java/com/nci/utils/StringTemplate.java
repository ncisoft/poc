package com.nci.utils;

import java.io.IOException;
import java.text.MessageFormat;
import com.github.jknack.handlebars.Handlebars;
import com.github.jknack.handlebars.Template;
import lombok.extern.slf4j.Slf4j;

@Slf4j
public class StringTemplate {
    private static final Handlebars handlebars = new Handlebars();
    private Template tpl = null;

    public static StringTemplate newInstance(String source) {
        StringTemplate o = new StringTemplate(source);
        return o;
    }

    public static String messageFormat(String pattern,  Object... arguments) {
        return MessageFormat.format(pattern, arguments);
    }

    public String render(Object context) {
        try {
            return tpl.apply(context);
        } catch (IOException e) {
            log.warn(e.getMessage());
            throw new RuntimeException(e);
        }
    }

    public StringTemplate(String input) {
        try {
            tpl = handlebars.compileInline(input);
        } catch (IOException e) {
            tpl = null;
            log.warn(e.getMessage());
            throw new RuntimeException(e);
        }
    }
}
