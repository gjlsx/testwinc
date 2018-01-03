using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace testwinc
{
    /**
   * <p>Title:  UseStatic </p>
   * <p>Description: 静态工具类</p>
   * <p>Copyright: 2011</p>
   * @author wind
   * @version 1.0
   */
    class UseStatic
    {
        public static int id = 0;

        public static string strName = "";

        private static WindForm wf;

        private static ArrayList listData;

        public static ArrayList ListData
        {

            get { return listData; }

        }


        public static ArrayList GetListData()
        {
            return listData;
        }
        
        //todo:工具类还要持续添加 ，by wind ,11.11.05
        private static void addDataToList()
        {
            if (listData == null)
            {
                listData = new ArrayList();
            }

            //listData.Add("DotNet");
        }


        public static void setWindForm(WindForm tt) 
        {
            wf = tt;
        }

        public static WindForm getWindForm()
        {
            return wf;
        }

        /// <summary>
        /// 提供静态方法供输出用
        /// </summary>
        /// <param name="str"></param>
        public static void sout(Object str)
        {
            if (wf != null)
            {
                wf.sout(str);
            }
        }

        /// <summary>
        /// 提供静态方法供输出用,线程安全
        /// </summary>
        /// <param name="stext"></param>
        public static void soutTd(Object stext)
        {
            if (wf != null)
            {
                wf.addTxtForThread(stext);
            } 
        }

        /// <summary>
        /// 输出richtextbox2 文字,线程安全
        /// </summary>
        /// <param name="stext"></param>
        public static void soutTdRtb2(Object stext)
        {
           wf.addRtb2ForThread(stext);
        }


    }//end UseStatic

}
