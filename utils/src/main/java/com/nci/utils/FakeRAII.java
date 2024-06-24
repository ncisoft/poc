package com.nci.utils;
import lombok.extern.slf4j.Slf4j;

@Slf4j
public class FakeRAII implements AutoCloseable {

    public FakeRAII() {
        log.debug("init");
    }
    private Runnable callback_task = null;
    public FakeRAII(Runnable callback_task) {
        this.callback_task = callback_task;
        log.debug("init");

    }
     
    public void close() throws Exception {
        Helper.nop();
        if (this.callback_task != null) {
            this.callback_task.run();
            log.debug("resources was closed");
        }
        else {
            log.debug("closed");

        }
    }
    
}
