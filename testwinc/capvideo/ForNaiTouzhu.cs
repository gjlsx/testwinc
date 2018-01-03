using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace testwinc.capvideo
{
	/// <summary>
	/// 投注对象类，by wind,14.12.12
	/// </summary>
    class ForNaiTouzhu
    {
        public String sQihao = "0";
        public String sOnetwo = "";//单，双
        public String sBeilv = "0";
        public String sTime = "";
        public int isKaijiang = 0;//0未开奖，1已开奖 (未中，中奖)
        public String sKaijiang = "";//"1.2.4.6.7";//开奖号码//for test here
        public int idannum = 6;
        public int itwonum = 10;//计算的双号下注数

        private static String[] Str_Minci = new String[] { "冠军", "亚军", "第三名", "第四名", "第五名"};
        private static String[] Str_Kaijiang = new String[] { "未开奖", "未中","中奖"};
        private static String Str_Dantwo = "单双";
        private static String Str_Space = "      ";
        public int[] isRightNum = null;

        public String sKjPre = "";//上期开奖号码

        public ForNaiTouzhu()
        {
        }

        public ForNaiTouzhu(String qihao,String onetwo,String beilv,String time)
        {
        	this.sQihao = qihao;
        	this.sOnetwo = onetwo;
        	this.sBeilv = beilv;
        	this.sTime = time;
        }

        public String[] testOut() 
        {
            String[] stp = new String[5];
          
            if (sKaijiang.Equals(""))
            {
                String tspace = Str_Space + "  ";
                for (int it = 0; it < 5; it++)
                {
                    
                     if(it > 1)
                   {
                       tspace = Str_Space;
                   }
                    StringBuilder sb = new StringBuilder();
                   sb.Append("    " + Str_Dantwo + Str_Space + Str_Minci[it]
                        + tspace + sOnetwo +"  "+ Str_Space + sBeilv + Str_Space + Str_Kaijiang[0]);
                   stp[it] = sb.ToString();
                }

            }
            else 
            {//已经开奖
                String tspace = Str_Space + "  ";
                int[] irc = isRightCode();
                isRightNum = irc;
                for (int it = 0; it < 5; it++)
                {
                   if(it > 1)
                   {
                       tspace = Str_Space;
                   }
                   StringBuilder sb = new StringBuilder();
                   sb.Append("    " + Str_Dantwo + Str_Space + Str_Minci[it]
                        + tspace + sOnetwo + "  " + Str_Space + sBeilv + Str_Space + Str_Kaijiang[irc[it]]);
                   stp[it] = sb.ToString();
                }
            }
            return stp;
        }

        //判断5个号码是否分别中奖。 -----  0:未开奖，1未中奖，2已经中奖
        public int[] isRightCode() 
        {
            int[] itmp = new int[] { 0, 0, 0, 0, 0 };
            if (this.sKaijiang.Equals("")){
                return itmp;
            }

            String[] sh = sKaijiang.Split('.');
            if (sh.Count() < 5){
                return itmp;}
                
            for(int ik =0 ;ik< 5;ik++)
            {
                 int ifirst = ForNaiStrategy.isDan(sh[ik]);//判断第n个数是否是单
                 if (ifirst == 1)
                 {
                     if (sOnetwo.Equals("单"))
                     {
                         itmp[ik] = 2;
                     }
                     else
                         itmp[ik] = 1;
                 }
                 else
                 {
                     if (sOnetwo.Equals("单"))
                     {
                         itmp[ik] = 1;
                     }
                     else
                         itmp[ik] = 2;
                 }           
            }
            return itmp;
        } 


        public override String ToString() 
        {
            StringBuilder sb = new StringBuilder();

            String sbl = "";
            if (!sKjPre.Equals(""))
            {
                String[] skjtemp = sKjPre.Split('.');
                int isfirstdan = ForNaiStrategy.isDan(skjtemp[0]);

                if (isfirstdan == 1)
                {
                    sbl = "(单: " + idannum + "  双: " + itwonum + ")";
                }
                else
                {
                    sbl = "(双: " + idannum + "  单: " + itwonum + ")";
                }
            }
            else
            {
                sbl = "(单: " + idannum + "  双: " + itwonum + ")";
            }

            sb.Append("  第" + sQihao + "期(" + sTime + ") :   " + sKaijiang + "---");
            sb.Append(sbl);
           // sb.Append(testOut());

            return sb.ToString();
        }
        
    }
}
