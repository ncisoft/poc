package com.nci.utils;

import java.util.Map;
import java.util.HashMap;
import com.nci.utils.pojo.*;


public class CodeGenHelper {
    static boolean step_commemt(String msg, int stepId) {
        return true;
    }
    
    final static String pattern = "/* block */";

    private static TplTuple extract(String tpl, int _index) {
            int index= tpl.indexOf(pattern, _index);
            if (index < 0) {
                throw new RuntimeException("---");
            }
            int indexNext = tpl.indexOf(pattern, index + pattern.length());
            if (indexNext < 0) {
                throw new RuntimeException("---");
            }
            String part = tpl.substring(index + pattern.length() + 1, indexNext-1);
            return TplTuple
                    .builder()
                    .lastIndex(indexNext - pattern.length())
                    .content(part)
                    .build();

    }
    public static String[] parseTimingTemplate(Class clz) {
        String tplContent = "";

        if (step_commemt("get template and render", 0x01)) {
            String _tpl = Helper.getResource(Constants.TIMING_TPL);
            Map<String, String> params = new HashMap<String, String>();
            params.put("class_name", clz.getCanonicalName());
            StringTemplate tpl = StringTemplate.newInstance(_tpl);
            tplContent = tpl.render(params);
        }

        if (step_commemt("split template to two parts", 0x02)) {
           TplTuple partFirst = extract(tplContent, 0) ;
           TplTuple part = partFirst;
           partFirst= partFirst
                   .toBuilder()
                   .partName("first part")
                   .build();
           System.out.println("---- " + partFirst.getPartName());
           System.out.println(partFirst.getContent());
           System.out.println("---- ");
           
           TplTuple partTwo = extract(tplContent, partFirst.getLastIndex()) ;
           part = partTwo;
           partTwo = partTwo.toBuilder().partName("last part").build();
           System.out.println("---- "+ partTwo.getPartName());
           System.out.println(partTwo.getContent());
           System.out.println("---- ");
           System.out.println("---- eoc");

        }
        return null;
    }

}
