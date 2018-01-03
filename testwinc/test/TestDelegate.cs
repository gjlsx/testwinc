using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace testwinc.test
{
    public class TestDelegate
    {
        public delegate void windHandler(object obj, WindEventargs wEventargs);//声明委派
        public windHandler wevent;//声明事件
        //public event windHandler wevent;
        private String strmsg = "";

        public void setString(String mesg) 
        {
            this.strmsg = mesg;
            if (strmsg == "i love u")//某字符串（状态）被改变
            {
                if (wevent != null && wevent.GetInvocationList().Length > 0)
                {
                    WindEventargs wea = new WindEventargs("make nage love");
                    wevent(this, wea);//发送事件
                }
                return ;
            }
        }

        public void justTest()
        {
            TestDelegate td = new TestDelegate();
            WindMonitor wm2 = new WindMonitor(td);
            td.setString("abc");
            td.setString("i love u");
            wm2.removeListener(td);
            td.setString("i love u");
        }

    }//end TestDelegate

    public class WindMonitor 
    {
        private TestDelegate.windHandler we = null;
        public WindMonitor(TestDelegate td)
        {
            we = new TestDelegate.windHandler(doSthWhenEventfire);
            td.wevent += we;
        }

        public void doSthWhenEventfire(object obj,WindEventargs agrs) 
        {
            UseStatic.sout(agrs.Message);
        }

        public void removeListener(TestDelegate td) 
        {
            if (td == null || td.wevent == null)
            {
                return;
            }
            foreach (Delegate dl in td.wevent.GetInvocationList())
            {
                if (we.Equals(dl))
                {
                    td.wevent -= we;
                    break;
                }
            }
            we = null;
        }

    }

    public class WindEventargs : EventArgs
    {
        private String message;

        public WindEventargs(string msg)
        {
            this.message = msg;
        }

        public String Message 
        {
            get
            {
                return message;
            }
        }
    }

}
