package com.nci.utils;

import java.lang.ref.WeakReference;

import com.nci.utils.pojo.StackTraceElementBean;

import lombok.extern.slf4j.Slf4j;

@Slf4j
public class GCChecker {
    private WeakReference<GCOwer> rf = new WeakReference<>(new GCOwer());
    private StackTraceElementBean mCallerLocation;

    public GCChecker() {
        StackTraceElementBean stackElement = Helper.getCallerStackTraceElement(2);
        Helper.dump_var("GCChecker", stackElement);
        Helper.nop(stackElement);
        Helper.println("---------");
        Helper.println();
        Helper.nop(rf);
    }

    private class GCOwer {

        StackTraceElementBean stackElement = Helper.getCallerStackTraceElement(2);

        @Override
        protected void finalize() throws Throwable {
            Helper.nop(stackElement);
            //super.finalize();
            log.info("GCChecker: {}", "finalize: app gc occur");
            Helper.dump_var("mCallerLocation", mCallerLocation);

            rf = new WeakReference<>(new GCOwer());
        }
    }

}
