package com.nci.utils;

public class Helper {
    private static final Helper mHelper = new Helper();

    private Helper() {
    }

    public Helper newInstance() {
        return mHelper;
    }
}
