using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace testwinc.test
{
    class TestString
    {
        private static String strBR = "< br >";


        /// <summary>
        /// 建立大String
        /// </summary>
        /// <param name="len">字符串大小</param>
        /// <returns>某个大字符串</returns>
        public void buildTestString() 
        {
            testStr(2);
            testBuilder(2);

       
        }

        //
        /// <summary>
        /// 测试String+的时间
        /// </summary>
        public String testStr(int filecopytime)
        {

            string s = "";
            string pathnow = System.Environment.CurrentDirectory;
            UseStatic.sout(pathnow);

            pathnow = pathnow + @"\test\" + "teststr.txt";

            DateTime dtn = DateTime.Now;
            UseStatic.sout("now String+ start: " + dtn.ToString() + ": " + dtn.Millisecond + "ms");// DateTime.Now.ToString("h:mm:ss.fff"));

            StreamReader objReader = new StreamReader(pathnow, System.Text.Encoding.Default);
            string strLine = "";
            //读取
            strLine = objReader.ReadLine();
            while (strLine != null)
            {
              for (int i = 0; i < filecopytime; i++)
               {
                        s += strLine.Trim() + i + "<br>";
               }

                strLine = objReader.ReadLine();
            }

            //long lend = DateTime.Now.Ticks;
            UseStatic.sout("now String+  end : " + DateTime.Now.ToString() + ": " + DateTime.Now.Millisecond + " ms");
            objReader.Close();
            objReader.Dispose();
       
            return s;
        }


        /// <summary>
        /// 测试Stringbuilder的时间
        /// </summary>
        public String testBuilder(int filecopytime)
        {
            string s = "";
            string pathnow = System.Environment.CurrentDirectory;
            UseStatic.sout(pathnow);

            pathnow = pathnow + @"\test\" + "teststr.txt";

            DateTime dtn = DateTime.Now;
            UseStatic.sout("now Stringbuilder start: " + dtn.ToString() + ": " + dtn.Millisecond + "ms");// DateTime.Now.ToString("h:mm:ss.fff"));

            StreamReader objReader = new StreamReader(pathnow, System.Text.Encoding.Default);
            string strLine = "";
            //读取
            strLine = objReader.ReadLine();
            StringBuilder sb = new StringBuilder(100);
            while (strLine != null)
            {
               for (int i = 0; i < filecopytime; i++)
                {
                    sb.Append(strLine.Trim() + i);
                    sb.Append(strBR);
                }
                strLine = objReader.ReadLine();
            }
            s = sb.ToString();
            //long lend = DateTime.Now.Ticks;
            UseStatic.sout("now Stringbuilder  end : " + DateTime.Now.ToString() + ": " + DateTime.Now.Millisecond + " ms");
            objReader.Close();
            objReader.Dispose();
            return s;
        }

    }
}
