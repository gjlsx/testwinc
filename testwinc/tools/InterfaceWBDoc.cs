using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace testwinc.tools
{
    //浏览器完成响应后执行事件委托的接口类
    public interface InterfaceWBDoc
    {
        void doAfterDocumentCompleted();
    }
}
