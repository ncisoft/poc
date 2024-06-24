package com.nci.utils;

import java.lang.ref.WeakReference;

import com.nci.utils.pojo.StackTraceElementBean;

import lombok.extern.slf4j.Slf4j;

@Slf4j
public class GCChecker {
    private WeakReference<GCOwer> mWeakReference = new WeakReference<>(new GCOwer());
    private StackTraceElementBean mCallerLocation;

    private static final int mMaxTenuringThreshold = Helper.getMaxTenuringThreshold()/2;
    private int mCounter=0;

    private void init(int stackId) {
        StackTraceElementBean stackElement = Helper.getCallerStackTraceElement(stackId+1);
        mCallerLocation = stackElement;
        Helper.nop(stackElement);
        Helper.println("---------");
        Helper.println();
        Helper.nop(mWeakReference);
        Helper.nop(mMaxTenuringThreshold);
    }
        
    public GCChecker() {
        this.init(2);
    }

    public GCChecker(int stackId) {
        this.init(stackId);
    }

    public int getMinorGC_Counter() {

        return mCounter;
    }
    private class GCOwer {

        StackTraceElementBean stackElement = Helper.getCallerStackTraceElement(2);

        @Override
        protected void finalize() throws Throwable {
            if (mCounter < mMaxTenuringThreshold) {
                mCounter++;
                mWeakReference = new WeakReference<>(new GCOwer());

                //super.finalize();
                if (Helper.getBoolean(false)) {
                    log.info("GCChecker: {}:{} {}{}",
                            "finalize: app gc occur",
                            mCounter,
                            mCallerLocation.toShortInfo(),
                            "");

                }

            }
        }
    }

}
