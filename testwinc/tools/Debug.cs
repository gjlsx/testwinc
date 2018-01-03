using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace testwinc.tools
{
    class Debug
    {
        public static Boolean DEBUG_KEY = true;//系统debug标志,可通过配置文件读取
	    public static  String TOOLS_DEBUG = "TOOLS";//tools包debug标签
	    public static  String TEST_DEBUG = "TEST";//test包debug标签
	    public static  String[] DEBUG_KEYS = new String[]{TOOLS_DEBUG,TEST_DEBUG};
    	
	    private static Boolean isToolsDebug = true;
	    private static Boolean isTestDebug = true;

        //add code here ,添加通过配置文件读取读取debug_key
        public static void setDebug(Boolean isdebug)
        {
           DEBUG_KEY = isdebug;
        }
    	
	    //和DEBUG_KEYS值 1,1对应
	    private static Boolean[] DEBUG_KEYS_VALUE = new Boolean[]{isToolsDebug,isTestDebug};
    	
        
        //获得系统调试标志
        public static Boolean getDebug()
        {
    	    return DEBUG_KEY;
        }
    
        //获得模块调试标志
        public static Boolean getDebugByName(String name)
        {
    	    return (DEBUG_KEY && toBoolean(name));
        }
        
        //返回key名字对应调试状态
        private static Boolean toBoolean(String name)
        {
    	    if(name == null) {
    		    return false;
    	    }
        	
    	    int iSize = DEBUG_KEYS.Count();
    	    for(int i = 0; i< iSize; i++)
    	    {
    		    if(name.ToUpper().Equals(DEBUG_KEYS[i]))
    		    {
    			    return DEBUG_KEYS_VALUE[i];
    		    }
    	    }
            //无该模块默认返回true
    	    return true;
        }

    }
}
