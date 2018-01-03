using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Collections;
using testwinc.tools;
using System.Windows.Forms;
using System.Data;
using System.Xml;

namespace testwinc.others
{
    class BuildJiemu
    {
        private static WindForm wf = null;

        private static IList<String> channelList = null;
        private static Hashtable ht = null;   //存的是引用，基础数据类型不能更改，对象数据可以更改

        public static String saveUrl = @"D:\work\compwork\testwinc\testwinc\bin\Debug\epgfile\txt\";
        public static String saveUrl2 = @"D:\work\compwork\testwinc\testwinc\bin\Debug\epgfile\txt\";


        public static String Str_epgtxt = "epgtxt";
        public static String Str_CatchTime = "catchtime";
        public static String Str_OutputType = "outputtype";

        public static String STRListFile = @"\epgfile\epglist20120409.txt";
        public static String STRListFile2 = @"\epgfile\epglist20120409.xml";
        public static String STRUpdateFile = @"\epgfile\epgupdate20120409.txt";
        public static String STRUpdateFile2 = @"\epgfile\epgupdate20120409.xml";
        public static String STRConfigFile = @"\epgconfig.txt";

        public static int iWeek = 0; //0: 本周 ,7: 下一周
        public static int iOutType = 0;//0输出txt 格式，1：输出xml格式
        public static int iOutDay = 0;//0 输出按每天格式，1：输出按每周格式

        private static String BeDT, EnDT;

        private static String Str_channellist = "";

        public static Boolean isForceAll = false;//是否强制更新所有

        private static Boolean isFor206Only = true;//为206服务器上使用，一般为false,放上去时为true
        private static String Str_For206 = "usefor206";

        public static int iTimeStep = 0;//自动运行时间间隔
        public static Boolean isAutoRun = false;  // 线程是不是已经在运行

        private static String filetxt = "";
        private static String filexml = "";
        private static String isListinDBID = "";


        static BuildJiemu()
        {
            if (wf == null)
            {
                wf = UseStatic.getWindForm();
            }
        }

        private static void SetChannelId(SqlCommand myCommand1, SqlDataReader dr1)
        {
            
            int jk = int.Parse(System.DateTime.Now.DayOfWeek.ToString("d"));

            if (jk == 0)
            {
                jk = 7;
            }
            BeDT = System.DateTime.Now.AddDays(1 - jk + iWeek).ToString("yyyy-MM-dd"); //周一  
            EnDT = System.DateTime.Now.AddDays(7 - jk + iWeek).ToString("yyyy-MM-dd");//下周一

            string pathnow = System.Environment.CurrentDirectory;

            

            STRConfigFile = pathnow + @"\epgconfig.txt";

            String sfor206 = Apis.getValueByStrName(STRConfigFile, Str_For206);
            //默认txt,xml全部输出
            //String souType = Apis.getValueByStrName(STRConfigFile, Str_OutputType);
            //String stxtxml = ".txt";
            //iOutType = 0;
            //if (souType != "" && souType == "1")
            //{
            //    stxtxml = ".xml";
            //    iOutType = 1;
            //}
            if (sfor206 == "true")
            {
                pathnow = "e:\\webvideo";  //for test 206服务器
                isFor206Only = true;
            }
            else
            {
                isFor206Only = false;
            }

            //加入随机文件名,记入数据库中
            //获得list列表
            myCommand1.CommandText =
                       "select top 1 id,filetxt,filexml from epgupdatelist where cdate = '"+ BeDT.Replace("-", "") + "'";

            dr1 = myCommand1.ExecuteReader();
            filetxt = "";
            filexml = "";
            isListinDBID = "";
            if (dr1.Read())
            {
                filetxt = dr1.GetValue(1) + "";
                filexml = dr1.GetValue(2) + "";
                isListinDBID = dr1.GetValue(0) + "";
            }
            dr1.Close();

            if (filetxt == "")
            {
                filetxt = "epglist" + BeDT + Apitool.getRandomString(4); 
            }
            if (filexml == "")
            {
                filexml = "epglist" + BeDT + Apitool.getRandomString(4); 
            }


            String path3 = Apis.getValueByStrName(STRConfigFile, Str_epgtxt);
            if (path3 == "")
            {
                path3 = pathnow + @"\epgfile\";
            }

            STRListFile = path3 + filetxt + ".txt";
            STRListFile2 = path3 + filexml + ".xml";
            STRUpdateFile = path3 +"epgupdate" + BeDT + ".txt";
            STRUpdateFile2 = path3 + "epgupdate" + BeDT + ".xml";

            if (channelList == null||channelList.Count==0)
            //if (channelList == null) by20120521
            {
                channelList = new List<String>();

                getChannelListSlct(path3+"epgselect.dat");

                //channelList.Add("1");
                //channelList.Add("7");
                //channelList.Add("2063");
                //channelList.Add("1977");
                //channelList.Add("4");
            }

            if (ht == null)
            {
                ht = new Hashtable();

                getChannelLiastAll(path3+"channellist.dat");
                //ht.Add("43", "浙江卫视");
                //ht.Add("1", "CCTV-1");
                //ht.Add("2063", "江门电视台公共频道");
                //ht.Add("1977", "三立财经台");
               // ht.Add("4", "CCTV-3");
               //ht.Add("5", "CCTV-4亚洲");
            }
           

            if (path3 != "")
            {
                    saveUrl = path3 + @"txt\";
                    saveUrl2 = path3 + @"xml\";
            }
            else
            {
                    saveUrl = pathnow + @"\epgfile\txt\";
                    saveUrl2 = pathnow + @"\epgfile\xml\";
            }
           

        }

        public static void makeJiemuNow()
        {
            makeJiemuNow(false);
        }

          // 生成节目表
        private static void makeJiemuNow(bool isrun)
        {
            wf.Invoke((MethodInvoker)delegate { wf.setbtnJiemu(false); });

            SqlConnection Mycon = null;
            SqlDataReader dr1 = null;

            FileStream fs = null;
            StreamWriter sw = null;
            FileStream fs2 = null;
            StreamWriter sw2 = null;
          
            try
            {
                    wf.addTbjmForThread("开始生成节目表！" + System.DateTime.Now.ToString());

                    string insertTXT = "", BS_DT = "", BS_DT2 = ""; ;
                    int i = 0;
                    

                    Mycon = Apis.GetCon(Apis.sDBUrl247);
                    SqlCommand myCommand1 = Mycon.CreateCommand();


                    SetChannelId(myCommand1,dr1);

                    String ChannelName = "", sChannelID = "", newFileName = "", newFileName2 = "";

                        if (!Directory.Exists(saveUrl))
                        {
                            Directory.CreateDirectory(saveUrl);
                        }
                        if (!Directory.Exists(saveUrl2))
                        {
                            Directory.CreateDirectory(saveUrl2);
                        }
                       
                        try
                        {
                            if (channelList.Count > 0)
                            {

                                String sLastCatchTime = Apis.getValueByStrName(STRListFile, Str_CatchTime);
                                if (isForceAll)
                                {
                                    sLastCatchTime = "";
                                }
                                if (sLastCatchTime == "")
                                {
                                    sLastCatchTime = "2012-02-14";
                                }
                                DateTime dtnew = System.DateTime.Parse(sLastCatchTime);


                                List<String> llneed = new List<String>();


                   
                                //根据当前数据判断是否已经存在数据，是否需要更新,7天日期
                                //myCommand1.CommandText =
                               // "select top 4700 channelid from catchLog  where status = 0 and souce != 4 and catchDate >= '"
                              //  + BeDT + "'  and catchDate < DATEADD(d,7,'" + BeDT
                              //  + "') and createtime > '" + sLastCatchTime + "' and channelid in (" + Str_channellist + ") group by channelid order by channelid";

                                myCommand1.CommandText =
                          "select top 4700 channelid from catchLog  where status = 0 and catchDate >= '"
                          + BeDT + "'  and catchDate < DATEADD(d,7,'" + BeDT
                          + "') and createtime > '" + sLastCatchTime + "' and channelid in (" + Str_channellist + ") group by channelid order by channelid";

                                dr1 = myCommand1.ExecuteReader();

                                while (dr1.Read())
                                {
                                    int itktmp = -1; ;
                                    String sidt = dr1.GetValue(0) + "";
                                    if (int.TryParse(sidt, out itktmp))
                                    {
                                        llneed.Add(itktmp + "");
                                    }
                                }
                                dr1.Close();

                                
                                if (llneed.Count > 0)
                                {
                                    for (int k = 0; k < channelList.Count; k++)
                                    {
                                        if (!(sChannelID.Equals(channelList[k].ToString())))
                                        {
                                            i = 0;
                                        }
                                        sChannelID = channelList[k].ToString();

                                        if (!llneed.Contains(sChannelID))
                                        {
                                            continue;
                                        }

                                        ChannelName = ht[sChannelID].ToString();


                                        //根据当前数据判断是否已经存在数据，是否需要更新,7天日期
                                        myCommand1.CommandText =
                                        "select top 1 id,catchDate,createtime from catchLog  where status = 0 and souce != 4 and catchDate >= '"
                                        + BeDT + "'  and channelid = '" + sChannelID + "'  and catchDate < DATEADD(d,7,'" + BeDT
                                        + "') and createtime > '" + sLastCatchTime + "' order by createtime desc";

                                        dr1 = myCommand1.ExecuteReader();
                                        DateTime dtget = DateTime.Now;
                                        if (dr1.Read())
                                        {
                                            //有数据需要更新;
                                            String scatchid = dr1.GetValue(0) + "";
                                            String scid = dr1.GetValue(2) + "";
                                            dtget = System.DateTime.Parse(scid);
                                            if (dtnew == null) dtnew = dtget;
                                            if (dtget > dtnew)
                                            {
                                                dtnew = dtget;
                                            }
                                            dr1.Close();
                                        }
                                        else
                                        {
                                            dr1.Close();
                                            continue;
                                        }


                                        {
                                            myCommand1.CommandText =
                                                "select ProgramName,PlayTime,ForenoticeID,ForenoticeID2,js from program where ChannelID ="
                                                + sChannelID + " and  playtime > '" + BeDT + "' and playtime < '"
                                                + Convert.ToDateTime(EnDT).AddDays(1).ToString("yyyy-MM-dd") + "' order by ChannelID,playtime";

                                            dr1 = myCommand1.ExecuteReader();

                                            //生成文件
                                            //String srandom = getRandomString(3);
                                            String srandom = dtget.ToString().Replace(":", "'");
                                            String newFileOnly = sChannelID + "." + ChannelName + "." + srandom + ".txt";
                                            String newFileOnly2 = sChannelID + "." + ChannelName + "." + srandom + ".xml";

                                            //每200个频道生成一个目录
                                            String ichannel = saveUrl + BeDT + "/" + getChaneldir(sChannelID) + "/";
                                            String ichannel2 = saveUrl2 + BeDT + "/" + getChaneldir(sChannelID) + "/";
                                            if (!Directory.Exists(ichannel))
                                            {
                                                Directory.CreateDirectory(ichannel);
                                            }
                                            if (!Directory.Exists(ichannel2))
                                            {
                                                Directory.CreateDirectory(ichannel2);
                                            }

                                            newFileName = ichannel + newFileOnly;
                                            newFileName2 = ichannel2 + newFileOnly2;
                                            fs = new FileStream(newFileName, FileMode.Create, FileAccess.Write);
                                            sw = new StreamWriter(fs, System.Text.Encoding.Default);

                                            fs2 = new FileStream(newFileName2, FileMode.Create, FileAccess.Write);
                                            sw2 = new StreamWriter(fs2, System.Text.Encoding.Default);
                                            sw2.WriteLine("<?xml version=\"1.0\" encoding=\"gb2312\"?>");
                                            sw2.WriteLine("<epg>");


                                            insertTXT = "频道名称 " + " " + ChannelName;
                                           // sw.WriteLine("");  //天柏需求，在频道名称和时间之间要隔行 2012.05.22 by  zrw
                                            sw.WriteLine(insertTXT);
                                            sw.WriteLine("");
                                            insertTXT = "<channelname>" + ChannelName + "</channelname>";
                                            sw2.WriteLine(insertTXT);

                                            while (dr1.Read())
                                            {
                                                System.DateTime sdtmp = System.DateTime.Parse(dr1.GetValue(1).ToString());
                                                BS_DT2 = sdtmp.ToString("yyyy") + "/" + sdtmp.ToString("MM") + "/" + sdtmp.ToString("dd");
                                                if (BS_DT != BS_DT2)
                                                {
                                                    if (i != 0)
                                                    {
                                                        sw.WriteLine("23:59  结束 ");
                                                        sw.WriteLine("");
                                                        sw2.WriteLine("</date>");
                                                    }
                                                    i++;
                                                    BS_DT = BS_DT2;
                                                    insertTXT = "播出日期 " + " " + BS_DT;
                                                    sw.WriteLine(insertTXT);
                                                    sw.WriteLine("");
                                                    sw2.WriteLine("<date>");
                                                    sw2.WriteLine("<day>" + BS_DT + "</day>");

                                                }
                                                String sdtmphm = sdtmp.ToString("HH:mm");
                                                String dr1valu0 = dr1.GetValue(0).ToString();
                                                String dr1valu2 = dr1.GetValue(2).ToString();
                                                String dr1valu3 = dr1.GetValue(3).ToString();

    
                                                insertTXT = sdtmphm + " " + dr1valu0;
                                                if (insertTXT.Length > 97)
                                                {
                                                    insertTXT = insertTXT.Substring(0, 97);
                                                }
                                                if (dr1valu0.Length > 97)
                                                {
                                                    dr1valu0 = dr1valu0.Substring(0, 97);
                                                    }

                                                sw.WriteLine(insertTXT);
                                                sw2.WriteLine("<C>");
                                                sw2.WriteLine("<pt>" + sdtmphm + "</pt>");
                                                sw2.WriteLine("<pn><![CDATA[" + dr1valu0 + "]]></pn>");

                                                sw2.WriteLine("<fid>" + dr1valu2 + "</fid>");
                                                sw2.WriteLine("<fid2>" + dr1valu3 + "</fid2>");

                                                sw2.WriteLine("</C>");

                                                //获得影视数据信息
                                                //SqlCommand sc3 = null;
                                                //int sid = 0;
                                                //int ijs = 0;
                                                //String sql3 = "select top 1 forenotice.title,forenotice.presenter,program_preview.program_preview from forenotice  inner join program_preview on forenotice.id = program_preview.forenoticeID where forenotice.id = "
                                                //+ sid + " and program_preview.ordernum = " + num;

                                            }
                                            sw.WriteLine("23:59  结束");
                                            sw2.WriteLine("</date>");
                                            sw2.WriteLine("</epg>");
                                            sw.Close();
                                            fs.Close();
                                            sw2.Close();
                                            fs2.Close();

                                            sw = null;
                                            fs = null;
                                            sw2 = null;
                                            fs2 = null;

                                            dr1.Close();

                                            String oldFile = Apis.getValueByStrName(STRListFile, sChannelID);
                                            String oldFile2 = Apis.getValueByStrName(STRListFile2, sChannelID);

                                            if (newFileName != oldFile)
                                            {
                                                Apis.setValueByStrName(STRListFile, sChannelID, newFileName);
                                                //删除老文件
                                                if (oldFile != "")
                                                {
                                                    if (File.Exists(oldFile))
                                                        File.Delete(oldFile);
                                                }
                                            }
                                            if (newFileName2 != oldFile2)
                                            {
                                                Apis.setValueByStrName(STRListFile2, sChannelID, newFileName2);
                                                //删除老文件
                                                if (oldFile2 != "")
                                                {
                                                    if (File.Exists(oldFile2))
                                                        File.Delete(oldFile2);
                                                }
                                            }
                                            wf.addTbjmForThread(sChannelID + "   " + ChannelName + "   节目表创建成功. ");

                                        }

                                    }
                                    //数据库中有毫秒，所以
                                    Apis.setValueByStrName(STRListFile, Str_CatchTime, dtnew.AddSeconds(1.0).ToString());
                                    Apis.setValueByStrName(STRListFile2, Str_CatchTime, dtnew.AddSeconds(1.0).ToString());
                                    Apis.setValueByStrName(STRUpdateFile, Str_CatchTime, dtnew.AddSeconds(1.0).ToString());
                                    Apis.setValueByStrName(STRUpdateFile2, Str_CatchTime, dtnew.AddSeconds(1.0).ToString());
                                    //把该list文件名存入数据库中
                                    if (isListinDBID != "")
                                    {
                                        //myCommand1.CommandText = "update epgupdatelist set filetxt='"+ filetxt +"',filexml='"+ filexml+ 
                                        //    "' where id = '" + isListinDBID + "'";

                                        //myCommand1.ExecuteNonQuery();
                                    }
                                    else if (isFor206Only)
                                    {
                                        myCommand1.CommandText =
                                               "insert into epgupdatelist(cdate,filetxt,filexml) values ('" + BeDT.Replace("-", "") + "','"
                                               +filetxt+"','"+filexml+"') ";
                                       myCommand1.ExecuteNonQuery();
                                    }

                                    myCommand1.Dispose();
                                }
                            }
                        }
                        catch (Exception ei)
                        {
                            wf.addTbjmForThread(ei.ToString());
                        }

                        wf.addTbjmForThread("\r\n 生成结束！" + System.DateTime.Now.ToString() + "\r\n");
                   
            }
            catch (Exception ex2)
            {
                wf.addTbjmForThread(ex2.ToString());
               
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
                if (fs2 != null)
                {
                    fs2.Close();
                    fs2 = null;
                }
                if (sw2 != null)
                {
                    sw2.Close();
                    sw2 = null;
                }

                if (dr1 != null)
                {
                    dr1.Close();
                    dr1.Dispose();
                    dr1 = null;
                }
                if (Mycon != null)
                {
                    Mycon.Close();
                    Mycon.Dispose();
                    Mycon = null;
                }
               
            }

            //iTimeStep = wf.getCmbTimeStepSlct();
            wf.getCmbTimeStepSlct();
            if (iTimeStep != 0)//一旦开始，根据时间确定
            {
                int ihour = iTimeStep;
                if (iTimeStep == 3)
                {
                    ihour = 4;
                }
                if (iTimeStep == 4)
                {
                    ihour = 8;
                }
                if (iTimeStep == 5)
                {
                    ihour = 12;
                }
                if (iTimeStep == 6)
                {
                    ihour = 24;
                }

                if ((!isAutoRun) || isrun) //auto run 已经运行 ，无需再次运行
                {
                    isAutoRun = true;
                    wf.addTbjmForThread(" 下一次运行将在 " + ihour + " 小时后。\r\n");
                    Thread.Sleep(3600000 * ihour);//1000*60*60*12 = 60000*720
                    makeJiemuNow(true);
                }
            }
            else
            {
                wf.Invoke((MethodInvoker)delegate { wf.setbtnJiemu(true); });
            }

        }

      

        //获得频道所属目录
        private static int getChaneldir(String channel)
        {
            int it = 0;
            if (int.TryParse(channel, out it))
            {
                return (it / 200 + 1);
            }
            return 0;
        }

        //获得所有频道列表，成功返回0，不成功返回-1
        private static int getChannelLiastAll(String file)
        {
            //channelList.Clear();
            ht.Clear();
            if (File.Exists(file))
            {
                //FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                String[] sr = File.ReadAllLines(file,Encoding.GetEncoding("gb2312"));
                foreach(String s in sr)
               {
                    int it =  s.IndexOf("\t");
                    if(it != -1)
                    {
                      int ik = 0;
                      if(int.TryParse(s.Substring(0,it),out ik))
                      {
                            //channelList.Add(ik + "");
                            ht.Add(ik+"",s.Substring(it+1).Trim());
                      }
                    }    
               }
                return 0;
            }
            return -1;
        }

        private static int getChannelListSlct(String file)
        {
            channelList.Clear();
            StringBuilder sb = new StringBuilder();
            if (File.Exists(file))
            {
                //FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                String[] sr = File.ReadAllLines(file, Encoding.GetEncoding("gb2312"));
                foreach (String s in sr)
                {
                    
                        int ik = 0;
                        if (int.TryParse(s, out ik))
                        {
                            channelList.Add(ik + "");
                            sb.Append(ik + ",");
                        }
                    
                }
                Str_channellist = sb.ToString()+"2";
                return 0;
            }
            return -1;
        }

        public static void setTimeStep(int time) 
        {
            iTimeStep = time;
        }

    }
}
