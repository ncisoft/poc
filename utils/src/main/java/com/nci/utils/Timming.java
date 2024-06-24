package com.nci.utils;

import java.lang.ref.WeakReference;
import com.nci.utils.pojo.StackTraceElementBean;
import org.fusesource.jansi.Ansi;
import static org.fusesource.jansi.Ansi.*;
import static org.fusesource.jansi.Ansi.Attribute.*;
import static org.fusesource.jansi.Ansi.Color.*;

import lombok.extern.slf4j.Slf4j;

@Slf4j
public class Timming implements AutoCloseable {

    private StackTraceElementBean mCallerLocation;
    private GCChecker mChecker;
    private String token = null;
    private long start_t;
    private Runnable callback_task = null;

    private Ansi ansiHL() {
        return ansi()
                .a(INTENSITY_BOLD)
                .fg(YELLOW);
    }

    private Ansi ansiHL2() {
        return ansi()
                .fg(YELLOW);
    }

    private Ansi ansiReset() {
        return ansi().reset();
    }

    void init(String _token) {
        mChecker = new GCChecker(4);
        this.token = _token;
        StackTraceElementBean stackElement = Helper.getCallerStackTraceElement(2+1);
        mCallerLocation = stackElement;
        token = token + stackElement.toShortInfo();
        stackElement = Helper.getCallerStackTraceElement(1);
        log.debug("{}{} new instance", _token, stackElement.toShortInfo());

        start_t = System.currentTimeMillis();
        log.debug("{}{} timing start{}",
                ansiHL(),
                token,
                ansiReset());

    }
    public Timming(String _token) {
        init(_token);
    }

    public Timming(Runnable callback_task) {
        init("");
        this.callback_task = callback_task;
        log.debug("init");

    }

    public void close() throws FakeException {
        Helper.nop();
        if (this.callback_task != null) {
            this.callback_task.run();
            log.debug("{} resources was closed, Timing age={}",
                    token,
                    Helper.getTenuredGenerationByUnsafe(this));
        } else {
            log.debug("{}{} elapsed = {}ms, Timing obj age = {}:{} age-counted-by-minorGC={}{}",
                    ansiHL2(),
                    token,
                    System.currentTimeMillis() - start_t,
                    Helper.getTenuredGenerationByUnsafe(this),
                    Helper.isTenuredGenerationByUnsafe(this),
                    mChecker.getMinorGC_Counter(),
                    ansiReset()
                    );
            log.debug("{} closed", token);

        }
    }

}
