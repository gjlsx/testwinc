using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using testwinc.tools;

namespace testwinc.capvideo
{
    //根据以往号码生成当期号码的策略类
    class ForNaiStra2
    {

        public static int STRATE_All = 9;//全部
        public static int STRATE_BIG_SMALL = 1;//大小
        public static int STRATE_LONG_HU = 3; //龙虎
        public static int STRATE_DAN_TWO = 5; //单双


        public static int[] ic1, ic2, ic3, ic4, ic5, ic6, ic7, ic8, ic9, ic10;//当前所有投注参数值

        public static int[][] icAll;


        //给所有参数设置初始值
        public static void setStartNum()
        {
               if (ic1 == null)
                {
                    ic1 = new int[10];
                    ic2 = new int[10];
                    ic3 = new int[10];
                    ic4 = new int[10];
                    ic5 = new int[10];
                    ic6 = new int[10];
                    ic7 = new int[10];
                    ic8 = new int[10];
                    ic9 = new int[10];
                    ic10 = new int[10];
                    icAll = new int[][] { ic1, ic2, ic3, ic4, ic5, ic6, ic7, ic8, ic9, ic10 };
                }

            for (int it = 0; it < 10; it++)
            {

                for (int ik = 0; ik < 10; ik++)
                {
                    icAll[it][ik] = Apitool.getIntValue( UseStatic.getWindForm().tbNall[it][ik].Text);

                }              
            }
        }



        //根据上期开奖号码和当前加减值计算当期倍数
        public static void setIcNumValue(int type, String qihao, String kaijiang)
        {

            String[] sh = kaijiang.Split('.');
            if (sh.Length < 10)
            {
                return;//开奖号码不对
            }

            for (int ik = 0; ik < 10; ik++)
            {
                //判断单双
                int ids = ForNaiStrategy.isDan(sh[ik]);//判断该数是否是单
                int ibs = isBigsmall(sh, ik); //判断大小
                int ilh = isLonghu(sh, ik);   //判断龙虎

                if (ids == 1)
                {

                }
                else
                {
 
                } 


            }

        }

        /// <summary>
        /// 判断该数大小，大返回1，小返回0
        /// </summary>
        /// <param name="sh"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int isBigsmall(String[] sh, int pos)  
        {
            return 0;
        }

        /// <summary>
        /// 判断该数龙虎，龙返回1，虎返回0
        /// </summary>
        /// <param name="sh"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int isLonghu(String[] sh, int pos)
        {
            return 0;
        }





    }
}// end class ForNaiStra2