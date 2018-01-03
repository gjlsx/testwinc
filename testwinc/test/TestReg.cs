using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using testwinc.tools;
using System.Windows.Forms;


namespace testwinc.test
{
    /// <summary>
    /// 正则表达式测试类
    /// by wind 
    /// 11.10.25
    /// </summary>
    class TestReg
    {
        public static String SnotCDW = @"[^a-zA-Z\d ]";   //检查非字母数字空格的正则
        public static String SnotDig = @"[\d.]";   //检查非数字小数点
        public static String SIP = @"\d+\.\d+\.\d+\.\d+"; //检查IP地址
        public static String Shtma = @"<[a-z]+[^>]*>"; //检查html 以a-z开头
        public static String Shtmenda = @"</[a-z\d]+>"; //检查html 以a-z结尾
        public static String Simg = @"<img[^>]*>"; //检查html头 以img开头

        public String strinput = "";
        public String strinputReg = "";
        public String strResult = "";


        //查询输入字符串中是否有查找字符串find,若有返回true
        public static Boolean IsMatchStr(String strinput, String find)
        {
            return Regex.IsMatch(strinput, find, RegexOptions.IgnoreCase);
        }

        public static String findReg(String input, String reg)
        {
            StringBuilder sb = null;
            RichTextBox rtbx = UseStatic.getWindForm().getRtbxSource();
            String strtem = rtbx.Text;
            rtbx.Clear();
            rtbx.Text = strtem;
            try
            {
                Match m = Apitool.GetResultOfReg(input, reg);
                if (m != null)
                {
                    while (m.Success)
                    {
                        int it = m.Index;
                        int len = m.Length;
                        setColor(it, len, rtbx);
                        sb = addString(sb, m.Value);
                        m = m.NextMatch();
                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            if (sb == null) return "";
            return sb.ToString();
        }

        public static String findRegNoshow(String input, String reg)
        {
            StringBuilder sb = null;
            //RichTextBox rtbx = UseStatic.getWindForm().getRtbxSource();
            //String strtem = rtbx.Text;
           // rtbx.Clear();
            //rtbx.Text = strtem;
            try
            {
                Match m = Apitool.GetResultOfReg(input, reg);
                if (m != null)
                {
                    while (m.Success)
                    {
                       // int it = m.Index;
                        //int len = m.Length;
                        //setColor(it, len, rtbx);
                        sb = addString(sb, m.Value);
                        m = m.NextMatch();
                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            if (sb == null) return "";
            return sb.ToString();
        }

        public static StringBuilder addString(StringBuilder old, String add)
        {
            if (old == null)
                old = new StringBuilder();
            if (add == "") return old;
            old.Append(add);
            old.Append("\r\n");
            return old;
        }

        public static String replaceReg(String input, String reg, String replace)
        {
            if (reg == "" || replace == "") return input;
            String st = "";
            try
            {
                st = Apitool.GetReplaceOfReg(input, reg, replace);
                return st;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }


        /// <summary>
        /// 给关键字上色
        /// </summary>
        /// <param name="p"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int setColor(int index, int len, RichTextBox tSql)
        {
                     int cnt = 0;
                    tSql.Select(index, len);
                    tSql.SelectionBackColor = System.Drawing.Color.Yellow;
                    tSql.SelectionColor = System.Drawing.Color.DarkRed;
                    cnt += (index+len);
                    return cnt;
            }
            

    }//end class TestReg
}
