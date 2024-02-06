package com.nci.utils;

import java.lang.ref.WeakReference;
import lombok.extern.slf4j.Slf4j;

@Slf4j
public class GCChecker {
    private WeakReference<GCOwer> rf = new WeakReference<>(new GCOwer());

    private class GCOwer {

        @Override
        protected void finalize() throws Throwable {
            //super.finalize();
            log.info("GCChecker: {}", "finalize: app gc occur");

            rf = new WeakReference<>(new GCOwer());
        }
    }

}
