using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace testwinc.tools
{

    class Charpinyin
    {
        //法2:也可使用正则表达式：判断字符串是否为连续的中文字符(不包含英文及其他任何符号和数字)的正则表达式为："^[\u4e00-\u9fa5]+$".
        //法1：int chfrom = Convert.ToInt32("4e00", 16); int chend = Convert.ToInt32("9fa5", 16);
        //code = Char.ConvertToUtf32(input, index);    //获得字符串input中指定索引index处字符unicode编码  
        //if (code >= chfrom && code <= chend)    // unicode不是用拼音排序的，所以不行

       public static char convertChinese(int n)
        {
            if (In(0xB0A1, 0xB0C4, n)) return 'a';
            if (In(0XB0C5, 0XB2C0, n)) return 'b';
            if (In(0xB2C1, 0xB4ED, n)) return 'c';
            if (In(0xB4EE, 0xB6E9, n)) return 'd';
            if (In(0xB6EA, 0xB7A1, n)) return 'e';
            if (In(0xB7A2, 0xB8c0, n)) return 'f';
            if (In(0xB8C1, 0xB9FD, n)) return 'g';
            if (In(0xB9FE, 0xBBF6, n)) return 'h';
            if (In(0xBBF7, 0xBFA5, n)) return 'j';
            if (In(0xBFA6, 0xC0AB, n)) return 'k';
            if (In(0xC0AC, 0xC2E7, n)) return 'l';
            if (In(0xC2E8, 0xC4C2, n)) return 'm';
            if (In(0xC4C3, 0xC5B5, n)) return 'n';
            if (In(0xC5B6, 0xC5BD, n)) return 'o';
            if (In(0xC5BE, 0xC6D9, n)) return 'p';
            if (In(0xC6DA, 0xC8BA, n)) return 'q';
            if (In(0xC8BB, 0xC8F5, n)) return 'r';
            if (In(0xC8F6, 0xCBF0, n)) return 's';
            if (In(0xCBFA, 0xCDD9, n)) return 't';
            if (In(0xCDDA, 0xCEF3, n)) return 'w';
            if (In(0xCEF4, 0xD188, n)) return 'x';
            if (In(0xD1B9, 0xD4D0, n)) return 'y';
            if (In(0xD4D1, 0xD7F9, n)) return 'z';
            return '*';
        }

        private static bool In(int start, int end, int code)
        {
            if (code >= start && code <= end)
            {
                return true;
            }
            return false;
        }

        public static void testpinyin()
        {
            string sc = "炫旋"; // 输入的字符串

            for (int i = 0; i < sc.Length; i++)
            {
                String sgs = Charpinyin.getSpell(sc[i].ToString());
                UseStatic.sout(sc[i] + " 拼音是  [" + sgs + "]");

            }

            //String s = "\u4e00";
            int chfrom = Convert.ToInt32("4e00", 16); 
            int chend = Convert.ToInt32("9fa5", 16);
            int code = Char.ConvertToUtf32("美炫旋", 2);    //获得字符串input中指定索引index处字符unicode编码 
            UseStatic.sout(chfrom + " " + chend + " " + code);

            getPinyinUnicode();


            //Encoding gb = System.Text.Encoding.GetEncoding("GB18030");
            //byte[] arrCN = gb.GetBytes(cnChar);
            //string lowCode = System.Convert.ToString(bytes[0], 16);   //取元素1编码内容（两位16进制）,low在前c3  
            //string hightCode = System.Convert.ToString(bytes[1], 16);//取元素2编码内容（两位16进制）,high在后c0(0xC3C0,'m'
            return;
        }

        //unicode字符不是按照拼音排的,并且，特殊字符也不是俺拼音排序的，所以没法做
        private static void getPinyinUnicode() 
        {
            Encoding gb = System.Text.Encoding.GetEncoding("GB2312");
            String[] str = new String[]{
                  "八",// =   'A '
                  "嚓",//   =   'B '
                  "咑",//   =   'C '
                  "妸",//   =   'D '
                  "发",// '   =   'E '
                  "旮",// '   =   'F '
                  "铪",// '   =   'G '
                  "丌",// '   =   'H '
                  "丌",// '   =   'I '
                  "咔",// '   =   'J '
                  "垃",// '   =   'K '
                  "嘸",// '   =   'L '
                  "拏",// '   =   'M '
                  "噢",// '   =   'N '
                  "妑",// '   =   'O '
                  "七",// '   =   'P '
                  "呥",// '   =   'Q '
                  "仨",// '   =   'R '
                  "他",// '   =   'S '
                  "屲",// '   =   'T '
                  "屲",// '   =   'U '
                  "屲",// '   =   'V '
                  "夕",// '   =   'W '
                  "丫",// '   =   'X '
                  "帀",// '   =   'Y '
                  // 'Z ' 
                };

            for (int i = 0; i < str.Length; i++)
            {
                byte[] arrCN = gb.GetBytes(str[i]);
                if (arrCN.Length > 1)
                {
                    int area = (short)arrCN[0];
                    int pos = (short)arrCN[1];
                    int code = (area << 8) + pos;
                    UseStatic.sout(code + " is " + str[i]);

                }
            }
          
                 
        }

        //返回汉字首拼音，有问题，字符集太小
        public static string getSpell(string cnChar)
        {
            byte[] arrCN = Encoding.Default.GetBytes(cnChar);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324,
                      49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25) max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new byte[] { (byte)(65 + i) });//大写
                    }
                }
                return "*";
            }
            else return cnChar;//英文字符或其他
        }


       public static int selectComboBoxByPinyin(LinkedList<LinkedList<String>> lls, LinkedList<int> li)
       {
           //IEnumerator<int> myEnmu = llcs.GetEnumerator();
           //while(myEnmu.MoveNext()){
           //    int tcs = myEnmu.Current();
           //}
           int cscount = li.Count;
           int islct = -1, maxFind = 0, ttime = 0;

           foreach (LinkedList<String> istr in lls)
           {
               int strCount = istr.Count;
               int tmpc = (strCount < cscount) ? strCount : cscount;
               int tmpfind = 0;
               for (int tm = 0; tm < tmpc; tm++)
               {
                   String pp = istr.ElementAt(tm);
                   int tcs = li.ElementAt(tm);
                   String st = ((char)tcs).ToString().ToUpper();
                   if (pp.Equals(st))
                   {
                       tmpfind = tm + 1;
                   }
                   else
                       tm = tmpc;//中断
               }
               if (tmpfind > maxFind)
               {
                   maxFind = tmpfind;
                   islct = ttime;
               }
               ttime++;
           }
           return islct;
       }

       //取得ComboBox中元素汉字字母拼音
       public static LinkedList<LinkedList<String>> getPinyinForCombox(ComboBox cbb)
       {
           LinkedList<LinkedList<string>> llc = new LinkedList<LinkedList<string>>();

           foreach (Object obj in cbb.Items)
           {
               LinkedList<string> llt = new LinkedList<string>();
               String st = obj.ToString().Trim().ToUpper();
               for (int tt = 0; tt < st.Length; tt++)
               {
                   String sg = st.Substring(tt, 1).Trim();
                   if (sg != "")
                   {
                       String sgs = Charpinyin.getSpell(sg.ToString());
                       llt.AddLast(sgs);
                   }
               }
               llc.AddLast(llt);
           }
           return llc;
       }

      
   
    }
}
