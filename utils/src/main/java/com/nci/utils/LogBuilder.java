package com.nci.utils;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class LogBuilder {
    public static Logger getLogger(Class<?> clazz) {
        Logger logger = LoggerFactory.getLogger(clazz);
        return logger;
    }
    
    
}
