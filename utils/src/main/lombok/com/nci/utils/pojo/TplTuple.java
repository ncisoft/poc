package com.nci.utils.pojo;

import lombok.Builder;
import lombok.Data;

@Data
@Builder(toBuilder = true)
public class TplTuple {
    private int lastIndex;
    private String content;
    private String partName;

     void nop_all() {
        lastIndex = 0;
        content = null;
        partName = null;
    }
}
