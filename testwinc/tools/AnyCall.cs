using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

//屏蔽webbrowser浏览器声音工具类
namespace testwinc.tools
{

    ////首先感谢一下 CSDN网友 pathletboy 、chys3584；还要重点感谢一下同学K`one，给予的无私直到；
    ////使用方法：
    ////1：将DSounds.dll放到应用程序的bin\Debug目录；
    ////2：在项目中新建一个类，比如 AnCall，并把上面的代码复制到类里面，压缩包中已经附带的有了；
    ////3：程序开始的地方调用AnyCall方法；比如：new JavaScript.AnyCall().CallDSoundsCode();
    ////4：按下F6看看效果如何；//win7下，.net3.5无效！
   /// <summary>
    /// 调用外部DLL DELPHI
    /// </summary>
    public class AnyCall
    {
        private const string _fileDll = @"DSounds";
       
        [DllImport(_fileDll, EntryPoint = "DSoundsCode", CharSet = CharSet.Ansi,CallingConvention = CallingConvention.StdCall)]       
        public static extern int DSoundsCode();

        //public int CallDSoundsCode()
        public int CallDSoundsCode()
        {
            return DSoundsCode();
        }
    }
}


