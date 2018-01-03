using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using testwinc.tools;

namespace testwinc.capvideo
{
    //根据以往号码生成当期号码的策略类
    class ForNaiStrategy
    {
        
        public static int STRATE_BIG_SMALL = 1;//单双
        public static int STRATE_BIG_SMALL2 = 2;//单双2
        public static int STRATE_LONG_HU = 3; //龙虎
        public static int STRATE_LONG_HU2 = 4;//龙虎2

        public static int iNowUseStrate = 0; // 当前所用策略，只能有一个

        private static String[] sDan = { "1", "3", "5", "7", "9" };
        private static String[] sTwo = { "2", "4", "6", "8", "10" };

        private static String[] strdan = { "1", "2" };//表示前5为单，后5为双
        private static String[] strtwo = { "2", "1" };

       
        public static int innf11 = -1;//加减值
        public static int innf12 = -2;
        public static int innf21 = -3;
        public static int innf22 = -4;
        public static int innf31 = -5;
        public static int innfw1 = 1;//错的加减值
        public static int innfw2 = 2;
        public static int innfw3 = 3;
        public static int innfw4 = 4;
        public static int innfw5 = 5;


        public static int innft11 = -1;//加减值
        public static int innft12 = -2;
        public static int innft21 = -3;
        public static int innft22 = -4;
        public static int innft31 = -5;

        public static int innftw1 = 1;//加减值
        public static int innftw2 = 2;
        public static int innftw3 = 3;
        public static int innftw4 = 4;
        public static int innftw5 = 5;

        public static int istartnumdan = 5;//起初倍数
        public static int istartnumtwo = 5;

        public static int istartLaterTime = 2;//延迟时间
        
      
        public static String[] iprevNum = null;//上期开号码

        public static int iDanCpu = -1;//上次计算的单号下注数//初始值为起始倍率，最小值>1才有效投，
        public static int iTwoCpu = -1;//上次计算的双号下注数

        public static int iDanCput = -1;//上次计算的单号下注数,模拟计算用
        public static int iTwoCput = -1;//上次计算的双号下注数,模拟计算用
        
        public static int iDanMax = -1;//当日方案a最大值
        public static int iTwoMax = -1;//当日方案b最大值

        public static int[] iCpuBak = new int[] { -1, -1, -1, -1 };//为投注不成功回退用
        
        public static int iDanMaxHistory = -1;//历史方案a最大值
        public static int iTwoMaxHistory = -1;//历史方案b最大值
        

        public static LinkedList<String> llKaiJiang = new LinkedList<string>();//已开的期数列表，期数
        public static Hashtable htKai = new Hashtable();//已开的历史号码列表
        public static LinkedList<ForNaiTouzhu> llmy = new LinkedList<ForNaiTouzhu>();//已投

        //以后当用sqllite来实现
        public static LinkedList<String> llnfls = new LinkedList<String>();//所有历史开奖记录
        public static Hashtable htKails = new Hashtable();//所有历史号码列表




        /*
                       *冠軍
              大 jeuM_52_548
              小 jeuM_52_549
              单 jeuM_53_550
              双 jeuM_53_551
              龙 jeuM_54_552
              虎 jeuM_54_553

              亞軍
              大 jeuM_52_554
              小 jeuM_52_555
              单 jeuM_53_556
              双 jeuM_53_557
              龙 jeuM_54_558
              虎 jeuM_54_559

              第三名
              大 jeuM_52_560
              小 jeuM_52_561
              单 jeuM_53_562
              双 jeuM_53_563
              龙 jeuM_54_564
              虎 jeuM_54_565
              4。
              大 jeuM_52_566
              小 jeuM_52_567
              单 jeuM_53_568
              双 jeuM_53_569
              龙 jeuM_54_570
              虎 jeuM_54_571
              5.
              大 jeuM_52_572
              小 jeuM_52_573
              单 jeuM_53_574
              双 jeuM_53_575
              龙 jeuM_54_576
              虎 jeuM_54_577
              6.
              大 jeuM_52_578
              小 jeuM_52_579
              单 jeuM_53_580
              双 jeuM_53_581

              第七名
              大 jeuM_52_582
              小 jeuM_52_583
              单 jeuM_53_584
              双 jeuM_53_585
              第八名
              大 jeuM_52_586
              小 jeuM_52_587
              单 jeuM_53_588
              双 jeuM_53_589
              第9名
              大 jeuM_52_590
              小 jeuM_52_591
              单 jeuM_53_592
              双 jeuM_53_593
              第十名
              大 jeuM_52_594
              小 jeuM_52_595
              单 jeuM_53_596
              双 jeuM_53_597 
       */

       // public static String[] sFirst5dan = { "jeuM_53_550", "jeuM_53_556", "jeuM_53_562", "jeuM_53_568", "jeuM_53_574" };
        //public static String[] sFirst5two = { "jeuM_53_551", "jeuM_53_557", "jeuM_53_563", "jeuM_53_569", "jeuM_53_575" };
        //public static String[] sLast5dan = { "jeuM_53_580", "jeuM_53_584", "jeuM_53_588", "jeuM_53_592", "jeuM_53_596" };
        //public static String[] sLast5two = { "jeuM_53_581", "jeuM_53_585", "jeuM_53_589", "jeuM_53_593", "jeuM_53_597" };
       
        //<INPUT style="DISPLAY: inline" class="amount-input hide" maxLength=9 jQuery171018996606361941748="9"> 
        //<INPUT class=amount-input maxLength=9 jQuery171040092058461596597="8">

        public static String[] sFirst5dan = { "\"3\"", "\"9\"", "\"15\"", "\"21\"", "\"27\"" }; //8,14,20,26,32
        public static String[] sFirst5two = { "\"4\"", "\"10\"", "\"16\"", "\"22\"", "\"28\"" };//9,15,21,27,33

        public static String[] sFirst5dan2 = { "\"8\"", "\"14\"", "\"20\"", "\"26\"", "\"32\"" };
        public static String[] sFirst5two2 = { "\"9\"", "\"15\"", "\"21\"", "\"27\"", "\"33\"" };

        
        
        //根据生成策略不同，对应原号码生成新号码数组
        /*
         * //大小方案1：若上期第一是单，则前5都压单，后5压双。若是双，则前5压双，后5压单
         * //大小方案2：若上期第一是单，则前5都压双，后5压单。若是双，则前5压单，后5压双
         * 
         * 
         */  
        //public static String[] getCodeByBefore(String sold, int type)        
        //{
        //    if (sold == null)
        //        return null;        
        //    String[] st = null;

        //    iNowUseStrate = type;

        //    if (type == STRATE_BIG_SMALL)
        //    {
        //        String[] ss = sold.Split('.');
        //        //第一位是否是1，3,5,7,9,//第一位有个空字符串数组
        //        foreach(String sd in sDan)
        //        {
        //            if(ss[1].Equals(sd))
        //            {
        //               st = strdan;
        //               return st;
        //            }                   
        //        }
        //        st = strtwo;
        //        return st;
        //    }
        //    else if (type == STRATE_BIG_SMALL2)
        //    {
        //        String[] ss = sold.Split('.');
        //        //第一位是否是1，3,5,7,9
        //        foreach (String sd in sDan)
        //        {
        //            if (ss[1].Equals(sd))
        //            {
        //                st = strtwo;
        //                return st;
        //            }
        //        }
        //        st = strdan;
        //        return st;
                
        //    }
        //    return st;
        //}//end getCodeByBefore


        public static String getTitle2By1(String sti)
        {
            for (int ik = 0; ik < 5; ik++)
            {
                if (sti.Equals(sFirst5dan[ik]))
                {
                    return sFirst5dan2[ik];
                }

                if (sti.Equals(sFirst5two[ik]))
                {
                    return sFirst5two2[ik];
                }
            }
            return "123456789";
        }


        //根据策略返回值，对应各个标签，设置对应值
        //
        public static LinkedList<String[]> getvalueForEachCode(int svalue)
        {
            int inow = ForNaiStrategy.iNowUseStrate;
            LinkedList<String[]> ll = null;

            if (inow == ForNaiStrategy.STRATE_BIG_SMALL2)
            {
                if (svalue>=0)
                {
                    ll = new LinkedList<String[]>();
                    foreach (String stmp in sFirst5dan)
                    {
                        String[] s1 = { stmp, svalue+""};
                        ll.AddFirst(s1);
                    }
                  
                   //只用下单边5个即可
                    //foreach (String stmp in sLast5two)
                    //{
                    //    String[] s1 = { stmp, svalue };
                    //    ll.AddFirst(s1);
                    //}
                    
                }
                else
                {
                	svalue = 0 - svalue;
                        ll = new LinkedList<String[]>();
                        foreach (String stmp in sFirst5two)
                        {
                            String[] s1 = { stmp, svalue+""};
                            ll.AddFirst(s1);
                        }
                       
                        //只用下单边5个即可
                        //foreach (String stmp in sLast5dan)
                        //{
                        //    String[] s1 = { stmp, svalue };
                        //    ll.AddFirst(s1);
                        //}                        
                    }
            }
            return ll;
        }

        
      /*
                     *1：)单双策略 描述
             描述如下  起始：单20， 双 25：  根据上期开出的是单还是双，对的减，错的加  	
             
            轮数      买单     买双       实际下单
            开单(双)

            1双      20+1=21   25-1 = 24：  24>21,双3  
            2双      21+1=22   24-1=23    23>22,双1
            3双      22+1=23   23-1=22    23>22,单1
            4双      23+1=24   22-1=21    单3
            5双      25        20         单5un
            6单      24        21         单3
            7单      23        22         单1
            8单      22       若也是22？   不下单
            9单      21        23         双2 
         * 
        */
        /// <summary>
        /// 要下注时判断上次号码对几个得出加减值，和已有倍数根据策略来取得要下的倍数
        /// 返回数大于0说明下单，小于0下双
        ///</summary>
        ///<returns>int</returns>

        public static int getNBeiNum2(int type, String qihao, String kaijiang, String old2kjiang)
        {
            //取得上期号数
            String spre = qihao;
            String sht = kaijiang;
            String[] sh = sht.Split('.');
            if (sh.Length < 5)
            {
                return 0;//开奖号码不对
            }

            int ifirst = isDan(sh[0]);//判断第一个数是否是单
            String[] shtmp = new String[5];
            for (int ip = 0; ip < 5; ip++)
            {
                shtmp[ip] = sh[ip];
            }
            int dannum = findDanInStr(shtmp);//有几个单//只判断前5个

            if (type != STRATE_BIG_SMALL2) return 0;//先做单双1策略
            iNowUseStrate = type;

            //如果是第一次投注，直接投预设号码
            Boolean isfirstTouzhu = false;//old2kjiang == "";
            if (old2kjiang.Equals(""))
            {
                isfirstTouzhu = true;
            }
            if (isfirstTouzhu)
            {
                iDanCpu = UseStatic.getWindForm().getTbnfqsbs1value();
                iTwoCpu = UseStatic.getWindForm().getTbnfqsbs2value();
            }
            else
            {
                String[] sh2 = old2kjiang.Split('.');
                if (sh2.Length < 5)
                {
                    return 0;//开奖号码不对
                }
                int ifirst2 = isDan(sh2[0]);//判断上2期第一个数是否是单

                if (ifirst == 1)
                {
                    //根据上1期开单则本期左边投单
                    //根据上2期开的单双判断对或错了几个
                    //单对了就加对的相应数量设置里填的数字
                    //双错了就加错的相应数字
                    if (ifirst2 == 1)
                    {
                        if (dannum == 1)//单对1双错1，左边对1，右边错1
                        {
                            iDanCpu = iDanCpu + innf11;
                            iTwoCpu = iTwoCpu + innftw1;
                        }
                        else if (dannum == 2)
                        {
                            iDanCpu = iDanCpu + innf12;
                            iTwoCpu = iTwoCpu + innftw2;
                        }
                        else if (dannum == 3)
                        {
                            iDanCpu = iDanCpu + innf21;
                            iTwoCpu = iTwoCpu + innftw3;
                        }
                        else if (dannum == 4)
                        {
                            iDanCpu = iDanCpu + innf22;
                            iTwoCpu = iTwoCpu + innftw4;
                        }
                        else if (dannum == 5)
                        {
                            iDanCpu = iDanCpu + innf31;
                            iTwoCpu = iTwoCpu + innftw5;
                        }
                    }
                    else if (ifirst2 == 0)
                    {
                        if (dannum == 1)//左双错1，右单对1，左边是错1，右边是对1
                        {
                            iDanCpu = iDanCpu + innfw1;
                            iTwoCpu = iTwoCpu + innft11;
                        }
                        else if (dannum == 2)
                        {
                            iDanCpu = iDanCpu + innfw2;
                            iTwoCpu = iTwoCpu + innft12;
                        }
                        else if (dannum == 3)
                        {
                            iDanCpu = iDanCpu + innfw3;
                            iTwoCpu = iTwoCpu + innft21;
                        }
                        else if (dannum == 4)
                        {
                            iDanCpu = iDanCpu + innfw4;
                            iTwoCpu = iTwoCpu + innft22;
                        }
                        else if (dannum == 5)  //上期投注双，开出左边错5，右边对5
                        {
                            iDanCpu = iDanCpu + innfw5;
                            iTwoCpu = iTwoCpu + innft31;
                        }
                    }
                }
                else if (ifirst == 0)
                {
                    if (ifirst2 == 0)//上两期也开双
                    {
                        //根据上1期开双则左边为双
                        if (dannum == 0) //左边对5个,右边错5个
                        {
                            iDanCpu = iDanCpu + innf31;
                            iTwoCpu = iTwoCpu + innftw5;
                        }
                        else if (dannum == 1)//左边对4个,右边错4个
                        {
                            iDanCpu = iDanCpu + innf22;
                            iTwoCpu = iTwoCpu + innftw4;
                        }
                        else if (dannum == 2)
                        {
                            iDanCpu = iDanCpu + innf21;
                            iTwoCpu = iTwoCpu + innftw3;
                        }
                        else if (dannum == 3)
                        {
                            iDanCpu = iDanCpu + innf12;
                            iTwoCpu = iTwoCpu + innftw2;
                        }
                        else if (dannum == 4)//双对1个
                        {
                            iDanCpu = iDanCpu + innf11;
                            iTwoCpu = iTwoCpu + innftw1;
                        }
                    }
                    else if (ifirst2 == 1)//上两期开单
                    {
                        //根据上1期开双则左边为双
                        if (dannum == 0)
                        {
                            iDanCpu = iDanCpu + innfw5;
                            iTwoCpu = iTwoCpu + innft31;

                        }
                        else if (dannum == 1)//4个双右对了4个,左单错4个
                        {
                            iDanCpu = iDanCpu + innfw4;
                            iTwoCpu = iTwoCpu + innft22;
                        }
                        else if (dannum == 2)
                        {
                            iDanCpu = iDanCpu + innfw3;
                            iTwoCpu = iTwoCpu + innft21;
                        }
                        else if (dannum == 3)
                        {
                            iDanCpu = iDanCpu + innfw2;
                            iTwoCpu = iTwoCpu + innft12;
                        }
                        else if (dannum == 4)//双对1个
                        {
                            iDanCpu = iDanCpu + innfw1;
                            iTwoCpu = iTwoCpu + innft11;
                        }
                    }
                }
            }
            if (iDanCpu < 0)//小于0时候不减
            {
                iDanCpu = 0;
            }
            if (iTwoCpu < 0)
            {
                iTwoCpu = 0;
            }
            iDanMax = Math.Max(iDanMax, iDanCpu);
            iTwoMax = Math.Max(iTwoMax, iTwoCpu);

            int it = iDanCpu - iTwoCpu;
            return it;     	
        }

        //左右单双策略2
        public static int getNBeiNum2Bak(int type,String qihao,String kaijiang,String old2kjiang)
        {
            //取得上期号数
            String spre = qihao;
            String sht = kaijiang;
            String[] sh = sht.Split('.');
            if (sh.Length < 5)
            {
                return 0;//开奖号码不对
            }

            int ifirst = isDan(sh[0]);//判断第一个数是否是单
              String[] shtmp = new String[5];
            for(int ip = 0; ip < 5;ip++)
            {
            	shtmp[ip] = sh[ip];
            }            
        	int dannum = findDanInStr(shtmp);//有几个单//只判断前5个

            if (type != STRATE_BIG_SMALL2) return 0;//先做单双1策略
            iNowUseStrate = type;

           //如果是第一次投注，直接投预设号码
           //add code here
           Boolean isfirstTouzhu = false;//old2kjiang == "";
           if(old2kjiang.Equals(""))
           {
           	  isfirstTouzhu = true;
           }
           if(isfirstTouzhu)
           {
               iDanCpu = UseStatic.getWindForm().getTbnfqsbs1value();
               iTwoCpu = UseStatic.getWindForm().getTbnfqsbs2value();
           }
           else
           {           	
           	 	String[] sh2 = old2kjiang.Split('.');
	            if (sh2.Length < 5)
	            {
	                return 0;//开奖号码不对
	            } 
           		int ifirst2 = isDan(sh2[0]);//判断上2期第一个数是否是单
           		
	            if (ifirst == 1)
	            {
	            	//根据上1期开单则本期左边投单
	            	//根据上2期开的单双判断对或错了几个
	            	//单对了就加对的相应数量设置里填的数字
	                //双错了就加错的相应数字
	                if(ifirst2 == 1)
	                {
		                if (dannum == 1)//单对1双错1
		                {
		                    iDanCpu = iDanCpu + innf11;
		                    iTwoCpu = iTwoCpu + innftw1;
		                }
		                else if (dannum == 2)
		                {
		                    iDanCpu = iDanCpu + innf12;
		                    iTwoCpu = iTwoCpu + innftw2;
		                }
		                else if (dannum == 3)
		                {
		                    iDanCpu = iDanCpu + innf21;
		                    iTwoCpu = iTwoCpu + innftw3;
		                }
		                else if (dannum == 4)
		                {
		                    iDanCpu = iDanCpu + innf22;
		                    iTwoCpu = iTwoCpu + innftw4;
		                }
		                else if (dannum == 5)
		                {
		                    iDanCpu = iDanCpu + innf31;
		                    iTwoCpu = iTwoCpu + innftw5;
		                }
	                }
	                 else if(ifirst2 == 0)
	                {
		                if (dannum == 1)//左边是双错1，右边是单对1
		                {
		                	int tv =  iTwoCpu + innftw1;
		                    iTwoCpu = iDanCpu + innf11;
		                    iDanCpu = tv;
		                }
		                else if (dannum == 2)
		                {
		                	int tv =  iTwoCpu + innftw2;
		                    iTwoCpu = iDanCpu + innf12;
		                    iDanCpu = tv;
		                }
		                else if (dannum == 3)
		                {
		                	int tv =  iTwoCpu + innftw3;
		                    iTwoCpu = iDanCpu + innf21;
		                    iDanCpu = tv;
		                }
		                else if (dannum == 4)
		                {
		                	int tv = iTwoCpu + innftw4;
		                    iTwoCpu = iDanCpu + innf22;
		                    iDanCpu = tv;
		                }
		                else if (dannum == 5)
		                {
		                	int tv = iTwoCpu + innftw5;
		                    iTwoCpu = iDanCpu + innf31;
		                    iDanCpu = tv;
		                }
	                }
	            }
	            else if (ifirst == 0)
	            {
	               if(ifirst2 == 0)//上两期也开双
	                {
		            	//根据上1期开双则左边为双
		                if (dannum == 0)
		                {
		                    iDanCpu = iDanCpu + innfw5;
		                    iTwoCpu = iTwoCpu + innft31;
		                }
		                else if (dannum == 1)//4个双对了4个,单错4个
		                {
		                    iDanCpu = iDanCpu + innfw4;
		                    iTwoCpu = iTwoCpu + innft22;
		                }
		                else if (dannum == 2)
		                {
		                    iDanCpu = iDanCpu + innfw3;
		                    iTwoCpu = iTwoCpu + innft21;
		                }
		                else if (dannum == 3)
		                {
		                    iDanCpu = iDanCpu + innfw2;
		                    iTwoCpu = iTwoCpu + innft12;
		                }
		                else if (dannum == 4)//双对1个
		                {
		                    iDanCpu = iDanCpu + innfw1;
		                    iTwoCpu = iTwoCpu + innft11;
		                }
	                }
	               else if(ifirst2 == 1)//上两期开单
	                {
	               	  //根据上1期开双则左边为双
		                if (dannum == 0)
		                {
		                    int tv =  iDanCpu + innfw5;
		                    iDanCpu = iTwoCpu + innft31;
		                    iTwoCpu = tv;
		                    
		                }
		                else if (dannum == 1)//4个双对了4个,单错4个
		                {
		                     int tv  = iDanCpu + innfw4;
		                    iDanCpu = iTwoCpu + innft22;
		                     iTwoCpu = tv;
		                }
		                else if (dannum == 2)
		                {
		                     int tv =  iDanCpu + innfw3;
		                     iDanCpu = iTwoCpu + innft21;
		                     iTwoCpu = tv;
		                }
		                else if (dannum == 3)
		                {
		                      int tv = iDanCpu + innfw2;
		                    iDanCpu = iTwoCpu + innft12;
		                     iTwoCpu = tv;
		                }
		                else if (dannum == 4)//双对1个
		                {
		                    int tv = iDanCpu + innfw1;
		                    iDanCpu = iTwoCpu + innft11;
		                    iTwoCpu = tv;
		                }                	
	                }	               
	            }
           }
            if (iDanCpu < 0)//小于0时候不减
            {
                iDanCpu = 0;
            }
            if (iTwoCpu < 0)
            {
                iTwoCpu = 0;
            }
            iDanMax = Math.Max(iDanMax, iDanCpu);
            iTwoMax = Math.Max(iTwoMax, iTwoCpu);

            int it = iDanCpu - iTwoCpu;
            return it;
        }

		//模拟策略
        public static int getNBeiNum2TestOld(int type, String qihao, String kaijiang, String old2kjiang)
        {
             //取得上期号数
            String spre = qihao;
            String sht = kaijiang;
            String[] sh = sht.Split('.');
            if (sh.Length < 5)
            {
                return 0;//开奖号码不对
            }

            int ifirst = isDan(sh[0]);//判断第一个数是否是单
            String[] shtmp = new String[5];
            for(int ip = 0; ip < 5;ip++)
            {
            	shtmp[ip] = sh[ip];
            }            
        	int dannum = findDanInStr(shtmp);//有几个单//只判断前5个

            if (type != STRATE_BIG_SMALL2) return 0;//先做单双1策略
            iNowUseStrate = type;

           //如果是第一次投注，直接投预设号码
           //add code here
           Boolean isfirstTouzhu = false;//old2kjiang == "";
           if(old2kjiang.Equals(""))
           {
           	  isfirstTouzhu = true;
           }
           if(isfirstTouzhu)
           {
               iDanCput = UseStatic.getWindForm().getTbnfqsbs1value();
               iTwoCput = UseStatic.getWindForm().getTbnfqsbs2value();
           }
           else
           {           	
           	 	String[] sh2 = old2kjiang.Split('.');
	            if (sh2.Length < 5)
	            {
	                return 0;//开奖号码不对
	            } 
           		int ifirst2 = isDan(sh2[0]);//判断上2期第一个数是否是单
           		
	            if (ifirst == 1)
	            {
	            	//根据上1期开单则本期左边投单
	            	//根据上2期开的单双判断对或错了几个
	            	//单对了就加对的相应数量设置里填的数字
	                //双错了就加错的相应数字
	                if(ifirst2 == 1)
	                {
		                if (dannum == 1)//单对1双错1
		                {
		                    iDanCput = iDanCput + innf11;
		                    iTwoCput = iTwoCput + innftw1;
		                }
		                else if (dannum == 2)
		                {
		                    iDanCput = iDanCput + innf12;
		                    iTwoCput = iTwoCput + innftw2;
		                }
		                else if (dannum == 3)
		                {
		                    iDanCput = iDanCput + innf21;
		                    iTwoCput = iTwoCput + innftw3;
		                }
		                else if (dannum == 4)
		                {
		                    iDanCput = iDanCput + innf22;
		                    iTwoCput = iTwoCput + innftw4;
		                }
		                else if (dannum == 5)
		                {
		                    iDanCput = iDanCput + innf31;
		                    iTwoCput = iTwoCput + innftw5;
		                }
	                }
	                 else if(ifirst2 == 0)
	                {
		                if (dannum == 1)//左边是双错1，右边是单对1
		                {
		                	int tv =  iTwoCput + innftw1;
		                    iTwoCput = iDanCput + innf11;
		                    iDanCput = tv;
		                }
		                else if (dannum == 2)
		                {
		                	int tv =  iTwoCput + innftw2;
		                    iTwoCput = iDanCput + innf12;
		                    iDanCput =tv;
		                }
		                else if (dannum == 3)
		                {
		                	int tv =  iTwoCput + innftw3;
		                    iTwoCput = iDanCput + innf21;
		                    iDanCput = tv;
		                }
		                else if (dannum == 4)
		                {
		                	int tv = iTwoCput + innftw4;
		                    iTwoCput = iDanCput + innf22;
		                    iDanCput = tv;
		                }
		                else if (dannum == 5)
		                {
		                	int tv = iTwoCput + innftw5;
		                    iTwoCput = iDanCput + innf31;
		                    iDanCput = tv;
		                }
	                }
	            }
	            else if (ifirst == 0)
	            {
	               if(ifirst2 == 0)//上两期也开双
	                {
		            	//根据上1期开双则左边为双
		                if (dannum == 0)
		                {
		                    iDanCput = iDanCput + innfw5;
		                    iTwoCput = iTwoCput + innft31;
		                }
		                else if (dannum == 1)//4个双对了4个,单错4个
		                {
		                    iDanCput = iDanCput + innfw4;
		                    iTwoCput = iTwoCput + innft22;
		                }
		                else if (dannum == 2)
		                {
		                    iDanCput = iDanCput + innfw3;
		                    iTwoCput = iTwoCput + innft21;
		                }
		                else if (dannum == 3)
		                {
		                    iDanCput = iDanCput + innfw2;
		                    iTwoCput = iTwoCput + innft12;
		                }
		                else if (dannum == 4)//双对1个
		                {
		                    iDanCput = iDanCput + innfw1;
		                    iTwoCput = iTwoCput + innft11;
		                }
	                }
	               else if(ifirst2 == 1)//上两期开单
	                {
	               	  //根据上1期开双则左边为双
		                if (dannum == 0)
		                {
		                    int tv =  iDanCput + innfw5;
		                    iDanCput = iTwoCput + innft31;
		                    iTwoCput = tv;
		                    
		                }
		                else if (dannum == 1)//4个双对了4个,单错4个
		                {
		                     int tv  = iDanCput + innfw4;
		                    iDanCput = iTwoCput + innft22;
		                     iTwoCput = tv;
		                }
		                else if (dannum == 2)
		                {
		                     int tv =  iDanCput + innfw3;
		                     iDanCput = iTwoCput + innft21;
		                     iTwoCput = tv;
		                }
		                else if (dannum == 3)
		                {
		                      int tv = iDanCput + innfw2;
		                    iDanCput = iTwoCput + innft12;
		                     iTwoCput = tv;
		                }
		                else if (dannum == 4)//双对1个
		                {
		                    int tv = iDanCput + innfw1;
		                    iDanCput = iTwoCput + innft11;
		                    iTwoCput = tv;
		                }                	
	                }	               
	            }
           }
            if (iDanCput < 0)//小于0时候不减
            {
                iDanCput = 0;
            }
            if (iTwoCput < 0)
            {
                iTwoCput = 0;
            }
          //  iDanMax = Math.Max(iDanMax, iDanCpu);
          //  iTwoMax = Math.Max(iTwoMax, iTwoCpu);

            int it = iDanCput - iTwoCput;
            return it;
        }
        
        //显示为左右的方案
        public static int getNBeiNum2Test(int type, String qihao, String kaijiang, String old2kjiang)
        {
            //取得上期号数
            String spre = qihao;
            String sht = kaijiang;
            String[] sh = sht.Split('.');
            if (sh.Length < 5)
            {
                return 0;//开奖号码不对
            }

            int ifirst = isDan(sh[0]);//判断第一个数是否是单
            String[] shtmp = new String[5];
            for (int ip = 0; ip < 5; ip++)
            {
                shtmp[ip] = sh[ip];
            }
            int dannum = findDanInStr(shtmp);//有几个单//只判断前5个

            if (type != STRATE_BIG_SMALL2) return 0;//先做单双1策略
            iNowUseStrate = type;

            //如果是第一次投注，直接投预设号码
            //add code here
            Boolean isfirstTouzhu = false;//old2kjiang == "";
            if (old2kjiang.Equals(""))
            {
                isfirstTouzhu = true;
            }
            if (isfirstTouzhu)
            {
                iDanCput = UseStatic.getWindForm().getTbnfqsbs1value();
                iTwoCput = UseStatic.getWindForm().getTbnfqsbs2value();
            }
            else
            {
                String[] sh2 = old2kjiang.Split('.');
                if (sh2.Length < 5)
                {
                    return 0;//开奖号码不对
                }
                int ifirst2 = isDan(sh2[0]);//判断上2期第一个数是否是单

                if (ifirst == 1)
                {
                    //根据上1期开单则本期左边投单
                    //根据上2期开的单双判断对或错了几个
                    //单对了就加对的相应数量设置里填的数字
                    //双错了就加错的相应数字
                    if (ifirst2 == 1)
                    {
                        if (dannum == 1)//单对1双错1，左边对1，右边错1
                        {
                            iDanCput = iDanCput + innf11;
                            iTwoCput = iTwoCput + innftw1;
                        }
                        else if (dannum == 2)
                        {
                            iDanCput = iDanCput + innf12;
                            iTwoCput = iTwoCput + innftw2;
                        }
                        else if (dannum == 3)
                        {
                            iDanCput = iDanCput + innf21;
                            iTwoCput = iTwoCput + innftw3;
                        }
                        else if (dannum == 4)
                        {
                            iDanCput = iDanCput + innf22;
                            iTwoCput = iTwoCput + innftw4;
                        }
                        else if (dannum == 5)
                        {
                            iDanCput = iDanCput + innf31;
                            iTwoCput = iTwoCput + innftw5;
                        }
                    }
                    else if (ifirst2 == 0)
                    {
                        if (dannum == 1)//左双错1，右单对1，左边是错1，右边是对1
                        {
                            iDanCput = iDanCput + innfw1;
                            iTwoCput = iTwoCput + innft11;
                        }
                        else if (dannum == 2)
                        {
                            iDanCput = iDanCput + innfw2;
                            iTwoCput = iTwoCput + innft12;
                        }
                        else if (dannum == 3)
                        {
                            iDanCput = iDanCput + innfw3;
                            iTwoCput = iTwoCput + innft21;
                        }
                        else if (dannum == 4)
                        {
                            iDanCput = iDanCput + innfw4;
                            iTwoCput = iTwoCput + innft22;
                        }
                        else if (dannum == 5)  //上期投注双，开出左边错5，右边对5
                        {
                            iDanCput = iDanCput + innfw5;
                            iTwoCput = iTwoCput + innft31;
                        }
                    }
                }
                else if (ifirst == 0)
                {
                    if (ifirst2 == 0)//上两期也开双
                    {
                        //根据上1期开双则左边为双
                        if (dannum == 0) //左边对5个,右边错5个
                        {   
                            iDanCput = iDanCput + innf31;
                            iTwoCput = iTwoCput + innftw5;
                        }
                        else if (dannum == 1)//左边对4个,右边错4个
                        {
                            iDanCput = iDanCput + innf22;
                            iTwoCput = iTwoCput + innftw4;
                        }
                        else if (dannum == 2)
                        {
                            iDanCput = iDanCput + innf21;
                            iTwoCput = iTwoCput + innftw3;
                        }
                        else if (dannum == 3)
                        {
                            iDanCput = iDanCput + innf12;
                            iTwoCput = iTwoCput + innftw2;
                        }
                        else if (dannum == 4)//双对1个
                        {
                            iDanCput = iDanCput + innf11;
                            iTwoCput = iTwoCput + innftw1;
                        }
                    }
                    else if (ifirst2 == 1)//上两期开单
                    {
                        //根据上1期开双则左边为双
                        if (dannum == 0)
                        {
                            iDanCput = iDanCput + innfw5;
                            iTwoCput = iTwoCput + innft31;

                        }
                        else if (dannum == 1)//4个双右对了4个,左单错4个
                        {
                            iDanCput = iDanCput + innfw4;
                            iTwoCput = iTwoCput + innft22;
                        }
                        else if (dannum == 2)
                        {
                            iDanCput = iDanCput + innfw3;
                            iTwoCput = iTwoCput + innft21;
                        }
                        else if (dannum == 3)
                        {
                            iDanCput = iDanCput + innfw2;
                            iTwoCput = iTwoCput + innft12;
                        }
                        else if (dannum == 4)//双对1个
                        {
                            iDanCput = iDanCput + innfw1;
                            iTwoCput = iTwoCput + innft11;
                        }
                    }
                }
            }
            if (iDanCput < 0)//小于0时候不减
            {
                iDanCput = 0;
            }
            if (iTwoCput < 0)
            {
                iTwoCput = 0;
            }
            //  iDanMax = Math.Max(iDanMax, iDanCpu);
            //  iTwoMax = Math.Max(iTwoMax, iTwoCpu);

            int it = iDanCput - iTwoCput;
            return it;
        }

        //有时投注不成功需要回退这次改变参数
        public static void backCpuValue() 
        {
            iCpuBak[0] = iDanCpu;
            iCpuBak[1] = iTwoCpu;
            iCpuBak[2] = iDanMax;
            iCpuBak[3] = iTwoMax;
        }

        //有时投注不成功需要回退这次改变参数
        public static void loadCpuValue()
        {
            iDanCpu = iCpuBak[0];
            iTwoCpu = iCpuBak[1];
            iDanMax = iCpuBak[2];
            iTwoMax = iCpuBak[3];
        }



        /// <summary>
        ///判断对了几个号
        /// </summary>
        /// <param name="sk">开的号码</param>
        /// <param name="smy">上期投的号码</param>
        /// <returns></returns>
        private int getRightNum(String[]sk, String[]smy) 
        {
            if (smy == null || sk == null)
                return -1;
            int it = sk.Count();
            int right = 0;
            for (int ik = 0; ik < it; ik++)
            {
                foreach(String st in smy)
                {
                    if(st.Trim().Equals(sk[ik].Trim()))
                    {
                        right++;
                    }
                }
            }
            return right;
        }

        //存储上期开奖号,return false已经有号码或者sk为空， true:已经存储
        public static Boolean setKaiJiang(String sk) 
        {
            if (llKaiJiang == null)
                llKaiJiang = new LinkedList<string>();
            String[] ss = sk.Split('.');
            if (ss == null || ss.Count() < 3)
            {
                return false;
            }
            if (llKaiJiang.Contains(ss[0]))
            {
                return false;
            }
            else
            {
                llKaiJiang.AddLast(ss[0]);
                if (htKai == null)
                {
                    htKai = new Hashtable();
                }
                else if (htKai.Contains(ss[0]))
                {
                    htKai.Remove(ss[0]);
                }
                htKai.Add(ss[0], sk.Substring(ss[0].Length + 1));               
                return true;
            }
        }

        //看号码里有几个单 
        public static int findDanInStr(String[] sh)
        {
            int idan = 0;
            foreach (String st in sh)
            {
            	if(isDan(st)==1)
            	{
            		 idan++;                      
            	}              
            } 
            return idan;
        }               
        
        
        /// <summary>
        /// 判断输入字符是否是单数, 单数返回1,else return 0.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int isDan(String input)
        {
                foreach (String sd in sDan)
                {
                    if (input.Equals(sd))
                    {
                        return 1;
                    }
                }
                return 0;
        }

        public static void clearHistory()
        {
            if (llKaiJiang!= null)
                llKaiJiang.Clear();
            if (htKai!=null)
                htKai.Clear();
            if (llmy!= null)
                llmy.Clear(); 
        }

        public static void setTbValue(System.Windows.Forms.TextBox[] tb)
        {
               innf11 =  Apitool.getIntValue(tb[0].Text);//加减值
               innf12 = Apitool.getIntValue(tb[1].Text);
               innf21 = Apitool.getIntValue(tb[2].Text);
               innf22 = Apitool.getIntValue(tb[3].Text);
               innf31 = Apitool.getIntValue(tb[4].Text);
               innfw1 = Apitool.getIntValue(tb[5].Text);//错的加减值
               innfw2 = Apitool.getIntValue(tb[6].Text);
               innfw3 = Apitool.getIntValue(tb[7].Text);
               innfw4 = Apitool.getIntValue(tb[8].Text);
               innfw5 = Apitool.getIntValue(tb[9].Text);


               innft11 = Apitool.getIntValue(tb[10].Text);//加减值
               innft12 = Apitool.getIntValue(tb[11].Text);
               innft21 = Apitool.getIntValue(tb[12].Text);
               innft22 = Apitool.getIntValue(tb[13].Text);
               innft31 = Apitool.getIntValue(tb[14].Text);

               innftw1 = Apitool.getIntValue(tb[15].Text);//加减值
               innftw2 = Apitool.getIntValue(tb[16].Text);
               innftw3 = Apitool.getIntValue(tb[17].Text);
               innftw4 = Apitool.getIntValue(tb[18].Text);
               innftw5 = Apitool.getIntValue(tb[19].Text);

        }


    }
}
