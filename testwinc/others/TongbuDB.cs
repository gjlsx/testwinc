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


namespace testwinc.others
{
    class TongbuDB
    {
        private static WindForm wf = null;
        public static Boolean isAutoRun = false;  // 线程是不是已经在运行
        public static int iTimeStep = 0;


        static TongbuDB()
        {
            if (wf == null)
            {
                wf = UseStatic.getWindForm();
            }
        }

        #region 同步全部
        public static void TongbuDBNowALL()
        {
            // TongbuDBNow(false, false);
            TongbuDBNowAll(false, false);
        }
        #endregion

        #region 快速复制
        public static void TongbuDBNow()
        {
           // TongbuDBNow(false, false);
            TongbuDBNowTwo(false, false);
        }
        #endregion

        #region 单条sql同步数据库
        public static void TongbuDBNowTwo()
        {
           // TongbuDBNow(false, true);
            TongbuDBNowTwo(false, true);
        }
        #endregion

        #region 同步全部
        // 生成节目表
        private static void TongbuDBNowAll(bool isrun, bool isTwo)
        {
            wf.Invoke((MethodInvoker)delegate { wf.setbtnTongbuDB(false); });

            SqlConnection conn = null;
            DataTable dt1 = null;
            SqlConnection conn2 = null;
            DataTable dt2 = null;
            SqlBulkCopy sqlBC = null;
            SqlBulkCopy sqlBC2 = null;

            try
            {
                wf.addTbjmForThread("\r\n 同步开始！" + System.DateTime.Now.ToString() + "\r\n");

                conn = Apis.GetCon(Apis.sDBUrl247);

                SqlCommand msc = conn.CreateCommand();
                SqlCommand msc3 = conn.CreateCommand();


                int jk = int.Parse(System.DateTime.Now.DayOfWeek.ToString("d"));

                if (jk == 0)
                {
                    jk = 7;
                }
                String BeDT = System.DateTime.Now.AddDays(1 - jk).ToString("yyyy-MM-dd"); //周一  
                String beDt2 = Convert.ToDateTime(BeDT).ToString("yyyy-M-d");

                //获得需要同步的数据
                //先从catchlog获得需要更新的频道列表

                String spingdao = wf.getTbPingdao();
                LinkedList<String> lls = new LinkedList<string>();
                LinkedList<String> llp = new LinkedList<string>();
                String sChannelid = "1";
                String sChannelAll = "1";
                StringBuilder sb = new StringBuilder();
                if (spingdao == "")
                {

                    String sql2 = "";
                    sql2 = "select channelid from catchlog where catchdate>='" + BeDT + "'  group by channelid order by channelid asc";
                 
                    dt1 = Apis.GetDateTable(sql2, conn);
                    if (dt1 == null) return;

                    if (dt1.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            String sclid = dt1.Rows[i][0] + "";
                            sb.Append(sclid);
                            lls.AddFirst(sclid);
                        }

                        sChannelAll = sb.ToString();
                        sChannelAll = sChannelAll.Substring(0, sChannelAll.Length - 1);

                        dt1.Clear();
                        dt1.Dispose();
                    }
                }
                else
                {
                    String[] s3 = spingdao.Split(new String[] { "," }, 9999, StringSplitOptions.RemoveEmptyEntries);
                    foreach (String stmp4 in s3)
                    {
                        lls.AddFirst(stmp4);
                    }
                }

                if (lls.Count == 0)
                {
                    wf.addTbjmForThread("没有需更新的频道。");
                }
                else
                {
                    //根据频道分开来复制，避免数据过大不能同步
                    int icount = lls.Count;
                    wf.addTbjmForThread("需更新频道： " + icount + " 个.\r\n");
                    conn2 = Apis.GetCon(Apis.sDBUrl38);


                    foreach (String strchan in lls)
                    {

                        wf.addTbjmForThread("目前更新 " + strchan);

                        sChannelid = strchan;

                        String sperte = "select id,programName,playTime,endTime,channelID,TVID,matchID,playType,properties,ForenoticeID,ForenoticeID2,content,dt,js,jsUserName,username ";
                        //因为合作伙伴需要R 所以同步*
                        sperte = "select * ";  
                        
                        String sql = sperte + "from program where ChannelID in(" + sChannelid + ")" + " and playTime>='" + BeDT + "' order by id";

                        dt1 = Apis.GetDateTable(sql, conn);
                        if (dt1 != null && dt1.Rows.Count > 0)
                        {
                            wf.addTbjmForThread("\r\n 需要更新 ：" + dt1.Rows.Count + "条数据！ \r\n");

                            // 删除原有
                            String sqlone = "", sqltwo = "", sqlthree = "";
                            sqlone = "delete from catchlog where catchdate>='" + BeDT + "' and channelid in(" + sChannelid + ")";
                            sqltwo = "delete from catchlog where catchdate>='" + beDt2 + "' and channelid in(" + sChannelid + ")";
                            sqlthree = "delete from program where channelid in(" + sChannelid + ") and  playtime >='" + BeDT + " 0:00 '";

                            SqlCommand sc4 = conn2.CreateCommand();
                            Apis.UpdateDB(sc4, sqlone);//删除 抓取日志yyyy-MM-dd
                            Apis.UpdateDB(sc4, sqltwo);//删除抓取日志 yyyy-m-d
                            Apis.UpdateDB(sc4, sqlthree);//删除节目表

                            if (isTwo) //判断是否用单条sql加入
                            {
                                for (int j = 0; j < dt1.Rows.Count; j++)
                                {
                                    String sqlTwo = "";
                                    sqlTwo = "insert into program(programName,playTime,endTime,channelID,TVID,matchID,playType,properties,ForenoticeID,ForenoticeID2,content,dt,js,jsUserName,username)" +
                                        "values('" + dt1.Rows[j]["programname"] + "','" + dt1.Rows[j]["playtime"] + "','" + dt1.Rows[j]["endTime"] + "','" + dt1.Rows[j]["channelID"] + "','" + dt1.Rows[j]["TVID"] + "','" + dt1.Rows[j]["matchID"] + "','" + dt1.Rows[j]["playType"] + "','" + dt1.Rows[j][""] + "',)" +
                                        ",'" + dt1.Rows[j]["properties"] + "','" + dt1.Rows[j]["ForenoticeID"] + "','" + dt1.Rows[j]["ForenoticeID2"] + "','" + dt1.Rows[j]["content"] + "','" + dt1.Rows[j]["dt"] + "','" + dt1.Rows[j]["js"] + "','"
                                        + dt1.Rows[j]["jsUserName"] + "','" + dt1.Rows[j]["username"] + "'";
                                    Apis.upDB(conn2, sqlTwo);
                                }
                            }
                            else
                            {
                                using (sqlBC = new SqlBulkCopy(conn2))
                                {
                                    sqlBC.BatchSize = 10000; //每次执行的行数
                                    sqlBC.BulkCopyTimeout = 60; //超时时间设置单位秒
                                    sqlBC.NotifyAfter = 10000;
                                    sqlBC.DestinationTableName = "program"; //表
                                    sqlBC.WriteToServer(dt1);
                                    sqlBC.Close();
                                    sqlBC = null;

                                }

                            }

                            int irowlen = dt1.Rows.Count;
                            dt1.Clear();
                            dt1.Dispose();

                            wf.addTbjmForThread("节目表同步完毕！");


                            String sql3 = "select * from catchlog where catchdate>='" + BeDT + "' order by channelid asc";
                            if (spingdao != "")
                            {
                                sql3 = "select * from catchlog where catchdate>='" + BeDT + "' and channelid in (" + spingdao + ")  order by channelid asc";
                            }

                            dt2 = Apis.GetDateTable(sql3, conn);

                            if (dt2 != null && dt2.Rows.Count > 0)
                            {
                                using (sqlBC2 = new SqlBulkCopy(conn2))
                                {
                                    sqlBC2.BatchSize = 10000; //每次执行的行数
                                    sqlBC2.BulkCopyTimeout = 60; //超时时间设置单位秒
                                    sqlBC2.NotifyAfter = 10000;
                                    sqlBC2.DestinationTableName = "catchlog"; //表
                                    sqlBC2.WriteToServer(dt2);                //执行写入数据库表
                                    sqlBC2.Close();
                                    sqlBC2 = null;

                                    dt2.Clear();
                                    dt2.Dispose();
                                    dt2 = null;
                                    sql3 =
                                        "update catchlog set iscommon = 1 where catchdate>='" + BeDT + "' and iscommon = 0 and channelid in (" + sChannelid + ")";
                                    Apis.upDB(conn, sql3);
                                }
                            }


                        }
                    }
                }

                wf.addTbjmForThread("\r\n 全部同步完成！" + System.DateTime.Now.ToString() + "\r\n");
            }
            catch (Exception ex)
            {
                wf.addTbjmForThread(ex.ToString());
            }
            finally
            {

                if (sqlBC != null)
                {
                    sqlBC.Close();
                    sqlBC = null;
                }
                if (sqlBC2 != null)
                {
                    sqlBC2.Close();
                    sqlBC2 = null;
                }
                if (dt1 != null)
                {
                    dt1.Clear();
                    dt1.Dispose();
                    dt1 = null;
                }
                if (dt2 != null)
                {
                    dt2.Clear();
                    dt2.Dispose();
                    dt2 = null;
                }
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
                if (conn2 != null)
                {
                    conn2.Close();
                    conn2.Dispose();
                }

                   wf.Invoke((MethodInvoker)delegate { wf.setbtnTongbuDBAll(true); });
            }


        }//end TongbuDBNow

        #endregion
        #region 使用datatable同步数据库
        // 生成节目表
        private static void TongbuDBNowTwo(bool isrun, bool isTwo)
        {
            if (isTwo)
            {
                wf.Invoke((MethodInvoker)delegate { wf.setbtnTongbuDBTwo(false); });
            }
            else
            {
                wf.Invoke((MethodInvoker)delegate { wf.setbtnTongbuDB(false); });
            }
            SqlConnection conn = null;
            DataTable dt1 = null;
            SqlConnection conn2 = null;
            DataTable dt2 = null;
            SqlBulkCopy sqlBC = null;
            SqlBulkCopy sqlBC2 = null;

            try
            {
                wf.addTbjmForThread("\r\n 同步开始！" + System.DateTime.Now.ToString() + "\r\n");

                conn = Apis.GetCon(Apis.sDBUrl247);

                SqlCommand msc = conn.CreateCommand();
                SqlCommand msc3 = conn.CreateCommand();


                int jk = int.Parse(System.DateTime.Now.DayOfWeek.ToString("d"));

                if (jk == 0)
                {
                    jk = 7;
                }
                String BeDT = System.DateTime.Now.AddDays(1 - jk).ToString("yyyy-MM-dd"); //周一  
                String beDt2 = Convert.ToDateTime(BeDT).ToString("yyyy-M-d");



                //获得需要同步的数据
                //先从catchlog获得需要更新的频道列表

                String spingdao = wf.getTbPingdao();
                LinkedList<String> lls = new LinkedList<string>();
                LinkedList<String> llp = new LinkedList<string>();
                String sChannelid = "1";
                String sChannelAll = "1";
                StringBuilder sb = new StringBuilder();
                if (spingdao == "")
                {

                    String sql2 = "";
                    sql2 = "select channelid from catchlog where catchdate>='" + BeDT + "' and iscommon = 0 group by channelid order by channelid asc";
                    //sql2 = "select channelid from catchlog where catchdate>='" + BeDT + "' and iscommon = 1 group by channelid order by channelid asc";//测试
                    dt1 = Apis.GetDateTable(sql2,conn);
                    if (dt1==null) return;

                    if (dt1.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            String sclid = dt1.Rows[i][0] + "";
                            sb.Append(sclid);
                            lls.AddFirst(sclid);
                        }

                        sChannelAll = sb.ToString();

                        if (sChannelAll.Length > 0)
                        {
                            sChannelAll = sChannelAll.Substring(0, sChannelAll.Length - 1);
                        }
                        dt1.Clear();
                        dt1.Dispose();
                    }
                }
                else
                {
                    if (spingdao.Length > 0&&spingdao!=""&&spingdao!=null)
                    {
                        String[] s3 = spingdao.Split(new String[] { "," }, 9999, StringSplitOptions.RemoveEmptyEntries);
                        foreach (String stmp4 in s3)
                        {
                            lls.AddFirst(stmp4);
                        }
                    }
                }

                if (lls.Count == 0)
                {
                    wf.addTbjmForThread("没有需更新的频道。");
                }
                else
                {
                    //根据频道分开来复制，避免数据过大不能同步
                    int icount = lls.Count;
                    wf.addTbjmForThread("需更新频道： " + icount + " 个.\r\n");
                    conn2 = Apis.GetCon(Apis.sDBUrl38);

                    foreach (String strchan in lls)
                    {

                        wf.addTbjmForThread("目前更新 " + strchan);

                        try
                        {
                            

                            sChannelid = strchan;

                            String sperte = "select id,programName,playTime,endTime,channelID,TVID,matchID,playType,properties,ForenoticeID,ForenoticeID2,content,dt,js,jsUserName,username ";
                            //因为合作伙伴需要R 所以同步*
                            sperte = "select * ";
                            String sql = sperte + "from program where ChannelID in(" + sChannelid + ")" + " and playTime>='" + BeDT + "' order by id";

                            dt1 = Apis.GetDateTable(sql, conn);
                            if (dt1 != null && dt1.Rows.Count > 0)
                            {
                                wf.addTbjmForThread("\r\n 需要更新 ：" + dt1.Rows.Count + "条数据！ \r\n");

                                // 删除原有
                                String sqlone = "", sqltwo = "", sqlthree = "";
                                sqlone = "delete from catchlog where catchdate>='" + BeDT + "' and channelid in(" + sChannelid + ")";
                                sqltwo = "delete from catchlog where catchdate>='" + beDt2 + "' and channelid in(" + sChannelid + ")";
                                sqlthree = "delete from program where channelid in(" + sChannelid + ") and  playtime >='" + BeDT + " 0:00 '";

                                SqlCommand sc4 = conn2.CreateCommand();
                                Apis.UpdateDB(sc4, sqlone);//删除 抓取日志yyyy-MM-dd
                                Apis.UpdateDB(sc4, sqltwo);//删除抓取日志 yyyy-m-d
                                Apis.UpdateDB(sc4, sqlthree);//删除节目表

                                if (isTwo) //判断是否用单条sql加入
                                {
                                    for (int j = 0; j < dt1.Rows.Count; j++)
                                    {
                                        String sqlTwo = "";
                                        sqlTwo = "insert into program(programName,playTime,endTime,channelID,TVID,matchID,playType,properties,ForenoticeID,ForenoticeID2,content,dt,js,jsUserName,username)" +
                                            " values('" + dt1.Rows[j]["programname"].ToString().Replace("'", "") + "','" + dt1.Rows[j]["playtime"] + "','" + dt1.Rows[j]["endTime"] + "','" + dt1.Rows[j]["channelID"] + "','" + dt1.Rows[j]["TVID"] + "','" + dt1.Rows[j]["matchID"] + "','" + dt1.Rows[j]["playType"] + "'" +
                                            ",'" + dt1.Rows[j]["properties"] + "','" + dt1.Rows[j]["ForenoticeID"] + "','" + dt1.Rows[j]["ForenoticeID2"] + "','" + dt1.Rows[j]["content"].ToString().Replace("'", "") + "','" + dt1.Rows[j]["dt"] + "','" + dt1.Rows[j]["js"] + "','"
                                            + dt1.Rows[j]["jsUserName"] + "','" + dt1.Rows[j]["username"] + "') ";
                                        Apis.upDB(conn2, sqlTwo);
                                    }
                                }
                                else
                                {
                                    using (sqlBC = new SqlBulkCopy(conn2))
                                    {
                                        sqlBC.BatchSize = 10000; //每次执行的行数
                                        sqlBC.BulkCopyTimeout = 60; //超时时间设置单位秒
                                        sqlBC.NotifyAfter = 10000;
                                        sqlBC.DestinationTableName = "program"; //表
                                        sqlBC.WriteToServer(dt1);
                                        sqlBC.Close();
                                        sqlBC = null;

                                    }

                                }

                                int irowlen = dt1.Rows.Count;
                                dt1.Clear();
                                dt1.Dispose();

                                wf.addTbjmForThread("节目表同步完毕！");


                                String sql3 = "select * from catchlog where catchdate>='" + BeDT + "' and iscommon = 0 and channelid in (" + sChannelid + ") order by channelid asc";
                                if (spingdao != "")
                                {
                                    sql3 = "select * from catchlog where catchdate>='" + BeDT + "' and channelid in (" + spingdao + ")  order by channelid asc";
                                }

                                dt2 = Apis.GetDateTable(sql3, conn);

                                if (dt2 != null && dt2.Rows.Count > 0)
                                {
                                    using (sqlBC2 = new SqlBulkCopy(conn2))
                                    {
                                        sqlBC2.BatchSize = 10000; //每次执行的行数
                                        sqlBC2.BulkCopyTimeout = 60; //超时时间设置单位秒
                                        sqlBC2.NotifyAfter = 10000;
                                        sqlBC2.DestinationTableName = "catchlog"; //表
                                        sqlBC2.WriteToServer(dt2);                //执行写入数据库表
                                        sqlBC2.Close();
                                        sqlBC2 = null;

                                        dt2.Clear();
                                        dt2.Dispose();
                                        dt2 = null;
                                        sql3 =
                                            "update catchlog set iscommon = 1 where catchdate>='" + BeDT + "' and iscommon = 0 and channelid in (" + sChannelid + ")";
                                        Apis.upDB(conn, sql3);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            wf.addRTbjm("出错！频道ID: " + sChannelid);
                            wf.addRTbjm(ex.ToString());
                        }

                    }
                }

                wf.addTbjmForThread("\r\n 同步结束！" + System.DateTime.Now.ToString() + "\r\n");
            }
            catch (Exception ex)
            {
                wf.addRTbjm("节目表同步出错：" + ex.ToString());
            }
            finally
            {

                if (sqlBC != null)
                {
                    sqlBC.Close();
                    sqlBC = null;
                }
                if (sqlBC2 != null)
                {
                    sqlBC2.Close();
                    sqlBC2 = null;
                }
                if (dt1 != null)
                {
                    dt1.Clear();
                    dt1.Dispose();
                    dt1 = null;
                }
                if (dt2 != null)
                {
                    dt2.Clear();
                    dt2.Dispose();
                    dt2 = null;
                }
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
                if (conn2 != null)
                {
                    conn2.Close();
                    conn2.Dispose();
                }

                if (isTwo)
                {
                    wf.Invoke((MethodInvoker)delegate { wf.setbtnTongbuDBTwo(true); });
                }
                else
                {
                    wf.Invoke((MethodInvoker)delegate { wf.setbtnTongbuDB(true); });
                }
            }

            if (iTimeStep != 0)
            {
                int ihour = -1;

                switch (iTimeStep)
                {
                    case 1:
                        ihour = 15;
                        break;
                    case 2:
                        ihour = 20;
                        break;
                    case 3:
                        ihour = 30;
                        break;
                    case 4:
                        ihour = 45;
                        break;
                    case 5:
                        ihour = 60;
                        break;
                    case 6:
                        ihour = 120;
                        break;
                }

                if ((!isAutoRun) || isrun) //auto run 已经运行 ，无需再次运行
                {
                    isAutoRun = true;
                    wf.addTbjmForThread(" 下一次同步节目表将在 " + ihour + " 分钟后。\r\n");
                    Thread.Sleep(60000 * ihour);//1000*60*60*12 = 60000*720
                    if (isTwo)
                    {
                        TongbuDBNowTwo(true, true);
                    }
                    else
                    {
                        TongbuDBNowTwo(true, false);
                    }
                }
            }

        }//end TongbuDBNow

        #endregion
        #region 同步数据库
        // 生成节目表
        private static void TongbuDBNow(bool isrun,bool isTwo)
        {
            wf.Invoke((MethodInvoker)delegate { wf.setbtnTongbuDB(false); });

            SqlConnection conn = null;
            SqlDataReader dr1 = null;
            SqlConnection conn2 = null;
            SqlDataReader dr2 = null;
            SqlBulkCopy sqlBC = null;
            SqlBulkCopy sqlBC2 = null;
           
            try
            {
                wf.addTbjmForThread("\r\n 同步开始！" + System.DateTime.Now.ToString() + "\r\n");

                conn = Apis.GetCon(Apis.sDBUrl247);
               
                
                SqlCommand msc = conn.CreateCommand();
                SqlCommand msc3 = conn.CreateCommand();


                int jk = int.Parse(System.DateTime.Now.DayOfWeek.ToString("d"));

                if (jk == 0)
                {
                    jk = 7;
                }
                String BeDT = System.DateTime.Now.AddDays(1 - jk).ToString("yyyy-MM-dd"); //周一  
                //StringEnDT = System.DateTime.Now.AddDays(7 - jk + iWeek).ToString("yyyy-MM-dd");//下周一
                String beDt2 = Convert.ToDateTime(BeDT).ToString("yyyy-M-d");


               
                //获得需要同步的数据
                //先从catchlog获得需要更新的频道列表
           
                String spingdao = wf.getTbPingdao();
                LinkedList<String> lls = new LinkedList<string>();
                LinkedList<String> llp = new LinkedList<string>();
                String sChannelid = "1";
                String sChannelAll = "1";
                StringBuilder sb = new StringBuilder();
                if (spingdao == "")
                {

                    String sql2 = "";
                    sql2 = "select channelid from catchlog where catchdate>='" + BeDT + "' and iscommon = 0 group by channelid order by channelid asc";
                 
                    dr1 = Apis.searchInDB(msc, sql2);
                   
                    if (dr1 == null) return;

                    while (dr1.Read())
                    {
                        String sclid = dr1.GetValue(0) + "";
                        sb.Append(sclid);
                        lls.AddFirst(sclid);
                    }

                    sChannelAll = sb.ToString();
                    sChannelAll = sChannelAll.Substring(0, sChannelAll.Length - 1);

                    dr1.Close();
                }
                else
                {
                    String[] s3 = spingdao.Split(new String[]{","},9999,StringSplitOptions.RemoveEmptyEntries);
                    foreach (String stmp4 in s3)
                    {
                        lls.AddFirst(stmp4);
                    }
                }

                //fortest
                //lls.Clear();
                //sb = new StringBuilder();
                //String stmp5 = "select id from tvchannel where ah_id=4";
                //dr1 = Apis.searchInDB(msc, stmp5);
                //if (dr1 != null)
                //{
                //    while (dr1.Read())
                //    {
                //        String sclid = dr1.GetValue(0) + "";
                //        sb.Append(sclid);
                //        lls.AddFirst(sclid);
                //    }
                //    sChannelAll = sb.ToString();
                //    sChannelAll = sChannelAll.Substring(0, sChannelAll.Length - 1);

                //    dr1.Close();
                //}

                if (lls.Count == 0)
                {
                    wf.addTbjmForThread("没有需更新的频道。");
                }
                else
                {
                    //根据频道分开来复制，避免数据过大不能同步
                    int icount = lls.Count;
                    wf.addTbjmForThread("需更新频道： " + icount + " 个.\r\n");
                    conn2 = Apis.GetCon(Apis.sDBUrl38);

                    foreach (String strchan in lls)
                    {

                        wf.addTbjmForThread("目前更新 " + strchan);

                        sChannelid = strchan;
                        
                        String sperte = "select id,programName,playTime,endTime,channelID,TVID,matchID,playType,properties,ForenoticeID,ForenoticeID2,content,dt,js,jsUserName,username ";
                        String sql = sperte + "from program where ChannelID in(" + sChannelid + ")" + " and playTime>='" + BeDT + "' order by id";

                        dr1 = Apis.searchInDB(msc, sql);
                       
                        if (dr1 != null)
                        {
                            //wf.addTbjmForThread("\r\n 需要更新 ：" + "条数据！ \r\n");

                            //删除原有
                            String sqlone = "", sqltwo = "", sqlthree = "";
                            sqlone = "delete from catchlog where catchdate>='" + BeDT + "' and channelid in(" + sChannelid + ")";
                            sqltwo = "delete from catchlog where catchdate>='" + beDt2 + "' and channelid in(" + sChannelid + ")";
                            sqlthree = "delete from program where channelid in(" + sChannelid + ") and  playtime >='" + BeDT + " 0:00 '";

                            SqlCommand sc4 = conn2.CreateCommand();
                            Apis.UpdateDB(sc4, sqlone);//删除 抓取日志yyyy-MM-dd
                            Apis.UpdateDB(sc4, sqltwo);//删除抓取日志 yyyy-m-d
                            Apis.UpdateDB(sc4, sqlthree);//删除节目表
                           
                            if (isTwo) //判断是否用单条sql加入
                            {
                                //单条加入
                                while (dr1.Read())
                                {
                                    String sqlOne = "";
                                    sql = "insert into program(programName,playTime,endTime,channelID,TVID,matchID,playType,properties,ForenoticeID,ForenoticeID2,content,dt,js,jsUserName,username)" +
                                        "values('" + dr1["programname"] + "','" + dr1["playtime"] + "','" + dr1["endTime"] + "','" + dr1["channelID"] + "','" + dr1["TVID"] + "','" + dr1["matchID"] + "','" + dr1["playType"] + "','" + dr1[""] + "',)" +
                                        ",'" + dr1["properties"] + "','" + dr1["ForenoticeID"] + "','" + dr1["ForenoticeID2"] + "','" + dr1["content"] + "','" + dr1["dt"] + "','" + dr1["js"] + "','"
                                        + dr1["jsUserName"] + "','" + dr1["username"] + "'";
                                    Apis.upDB(conn2, sqlOne);
                                }

                            }
                            else
                            {


                                using (sqlBC = new SqlBulkCopy(conn2))
                                {
                                    sqlBC.BatchSize = 10000; //每次执行的行数
                                    sqlBC.BulkCopyTimeout = 60; //超时时间设置单位秒
                                    sqlBC.NotifyAfter = 10000;
                                    sqlBC.DestinationTableName = "program"; //表
                                    sqlBC.WriteToServer(dr1);                //执行写入数据库表
                                    sqlBC.Close();
                                    sqlBC = null;

                                }

                            }


                            int irowlen = dr1.Depth;
                            dr1.Close();
                            dr1 = null;

                            wf.addTbjmForThread("节目表同步完毕！");


                            String sql3 = "select * from catchlog where catchdate>='" + BeDT + "' and iscommon = 0  order by channelid asc";
                            if (spingdao != "")
                            {
                                sql3 = "select * from catchlog where catchdate>='" + BeDT + "' and channelid in ("+spingdao+")  order by channelid asc";
                            }

                            dr2 = Apis.searchInDB(msc3, sql3);
                            if (dr2 != null)
                            {
                                using (sqlBC2 = new SqlBulkCopy(conn2))
                                {
                                    sqlBC2.BatchSize = 10000; //每次执行的行数
                                    sqlBC2.BulkCopyTimeout = 60; //超时时间设置单位秒
                                    sqlBC2.NotifyAfter = 10000;
                                    sqlBC2.DestinationTableName = "catchlog"; //表
                                    sqlBC2.WriteToServer(dr2);                //执行写入数据库表
                                    sqlBC2.Close();
                                    sqlBC2 = null;

                                    dr2.Close();
                                    dr2 = null;

                                    sql3 =
                                        "update catchlog set iscommon = 1 where catchdate>='" + BeDT + "' and iscommon = 0 and channelid in (" + sChannelid + ")";
                                    Apis.upDB(conn, sql3);
                                }
                            }


                        }
                    }
                }

                wf.addTbjmForThread("\r\n 同步结束！" + System.DateTime.Now.ToString() + "\r\n");
            }
            catch (Exception ex)
            {
                wf.addTbjmForThread(ex.ToString());
            }
            finally 
            {

                if (sqlBC != null)
                {
                    sqlBC.Close();
                    sqlBC = null;
                }
                if (sqlBC2 != null)
                {
                    sqlBC2.Close();
                    sqlBC2 = null;
                }
                if (dr1 != null)
                {
                    dr1.Close();
                    dr1.Dispose();
                    dr1 = null;
                }
                if (dr2 != null)
                {
                    dr2.Close();
                    dr2.Dispose();
                    dr2 = null;
                }
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
                if (conn2 != null)
                {
                    conn2.Close();
                    conn2.Dispose();
                }

                if (isTwo)
                {
                    wf.Invoke((MethodInvoker)delegate { wf.setbtnTongbuDBTwo(true); });
                }
                else
                {
                    wf.Invoke((MethodInvoker)delegate { wf.setbtnTongbuDB(true); });
                }
            }

            if (iTimeStep != 0)
            {
                int ihour = -1;

                switch (iTimeStep)
                {
                    case 1: 
                        ihour = 15;
                        break;
                    case 2:
                        ihour = 20;
                        break;
                    case 3:
                        ihour = 30;
                        break;
                    case 4:
                        ihour = 45;
                        break;
                    case 5:
                        ihour = 60;
                        break;
                    case 6:
                        ihour = 120;
                        break;
                }
              
                if ((!isAutoRun) || isrun) //auto run 已经运行 ，无需再次运行
                {
                    isAutoRun = true;
                    wf.addTbjmForThread(" 下一次同步将在 " + ihour + " 分钟后。\r\n");
                    Thread.Sleep(60000 * ihour);//1000*60*60*12 = 60000*720
                    if (isTwo)
                    {
                        TongbuDBNow(true,true);
                    }
                    else
                    {
                        TongbuDBNow(true,false);
                    }
                }
            }

        }//end TongbuDBNow

        #endregion
        public static void TongbuDBOther()
        {
            TongbuDBOther(false);
        }

        //同步其余表 
            //images
            //forenotice
            //program_preivew
            //program
            //CatchLog
            //TV
            //Channel
            //Compere
            //topic 
        public static void TongbuDBOther(Boolean isrun)
        {
           //根据表的edittime，同步最新的edittime
            SqlConnection conn = null;
            SqlDataReader dr1 = null;
            SqlConnection conn2 = null;
            //SqlDataReader dr2 = null;
            SqlBulkCopy sqlBC = null;
            //SqlBulkCopy sqlBC2 = null;
            SqlDataAdapter cmd = null;
            DataSet ds = null;
            SqlCommandBuilder builder = null;
            StringBuilder sb = new StringBuilder();
            LinkedList<String> lls = new LinkedList<string>();
            DataTable dt2 = null;
            try
            {
                wf.addTbjmForThread("同步其余表开始！" + System.DateTime.Now.ToString());

                conn = Apis.GetCon(Apis.sDBUrl247);
                conn2 = Apis.GetCon(Apis.sDBUrl38);
                SqlCommand msc = conn.CreateCommand();
                SqlCommand msc2 = conn2.CreateCommand();


                //获得需要同步的数据,同步forenotice表
                String sql = "", stime2="";
                sql = "select top 1 id,editTime from forenotice order by editTime desc";
                dr1 = Apis.searchInDB(msc2, sql);
                if (dr1 == null) return;
                if (dr1.Read())
                {
                    stime2 = dr1.GetValue(1) + "";
                }
                dr1.Close();

                sql = "select * from forenotice where edittime > '" + stime2 + "' order by edittime desc";

                //fortesthere 
                //sql = "select * from forenotice where id > 83471";

                msc.CommandText = sql;
                cmd = new SqlDataAdapter(msc);  //adapter可用于写和更新
                builder = new SqlCommandBuilder(cmd);
                ds = new DataSet();
                cmd.Fill(ds, "forenotice");
                DataTable myDataTable = ds.Tables["forenotice"];
                foreach (DataRow row in myDataTable.Rows)
                {
                    String strid = row["id"] + "";
                    lls.AddFirst(strid);
                    sb.Append(strid);
                    sb.Append(",");
                }


                if (lls.Count > 0)
                {
                    wf.addTbjmForThread("开始同步forenotice 表,需同步 " + lls.Count + " 条数据！");
                    //删除原有数据
                    String sidlist = sb.ToString();
                    sidlist = sidlist.Substring(0, sidlist.Length - 1);
                    String sql3 = "delete from forenotice where id in (" + sidlist + ")";
                    Apis.UpdateDB(msc2, sql3);

                    using (sqlBC = new SqlBulkCopy(conn2))
                    {
                        sqlBC.BatchSize = 10000; //每次执行的行数
                        sqlBC.BulkCopyTimeout = 60; //超时时间设置单位秒
                        sqlBC.NotifyAfter = 10000;
                        sqlBC.DestinationTableName = "forenotice"; //表
                        sqlBC.WriteToServer(myDataTable);                //执行写入数据库表
                        sqlBC.Close();
                        sqlBC = null;

                    }
                }

                lls.Clear();
                lls = null;
                if (myDataTable != null)
                {
                    myDataTable.Dispose();
                    myDataTable = null;
                }
                if (ds != null)
                {
                    ds.Dispose();
                    ds = null;
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }


                wf.addTbjmForThread("forenotice表同步完毕！");

                //images表
                sql = "select top 1 id from images order by id desc";
                dr1 = Apis.searchInDB(msc2, sql);
                if (dr1 == null) return;

                String imageid = "";
                if (dr1.Read())
                {
                    imageid = dr1.GetValue(0) + "";
                }
                dr1.Close();

                if (imageid != "")
                {
                    sql = "select * from images where id > " + imageid + " order by id desc";
                    dr1 = Apis.searchInDB(msc, sql);
                    if (dr1 != null && dr1.Read() == true)
                    {
                        using (sqlBC = new SqlBulkCopy(conn2))
                        {
                            sqlBC.BatchSize = 10000; //每次执行的行数
                            sqlBC.BulkCopyTimeout = 60; //超时时间设置单位秒
                            sqlBC.NotifyAfter = 10000;
                            sqlBC.DestinationTableName = "images"; //表
                            sqlBC.WriteToServer(dr1);                //执行写入数据库表
                            sqlBC.Close();
                            sqlBC = null;

                        }
                    }
                    dr1.Close();
                }

                wf.addTbjmForThread("images表同步完毕！");

                //Compere表
                sql = "select top 1 id,editTime from Compere order by editTime desc";
                dr1 = Apis.searchInDB(msc2, sql);
                if (dr1 == null) return;

                stime2 = "";
                if (dr1.Read())
                {
                    stime2 = dr1.GetValue(1) + "";
                }
                dr1.Close();

                if (stime2 != "")
                {
                    sql = "select * from Compere where edittime > '" + stime2 + "' order by edittime desc";
                    //fortesthere 
                    //sql = "select * from Compere where id > 8948";

                    msc.CommandText = sql;
                    cmd = new SqlDataAdapter(msc);  //adapter可用于写和更新
                    builder = new SqlCommandBuilder(cmd);
                    ds = new DataSet();
                    cmd.Fill(ds, "Compere");
                    dt2 = ds.Tables["Compere"];
                    lls = new LinkedList<string>();
                    sb = new StringBuilder();
                    foreach (DataRow row in dt2.Rows)
                    {
                        String strid = row["id"] + "";
                        lls.AddFirst(strid);
                        sb.Append(strid);
                        sb.Append(",");
                    }


                    if (lls.Count > 0)
                    {
                        wf.addTbjmForThread("开始同步Compere 表,需同步 " + lls.Count + " 条数据！");
                        //删除原有数据
                        String sidlist = sb.ToString();
                        sidlist = sidlist.Substring(0, sidlist.Length - 1);
                        String sql3 = "delete from Compere where id in (" + sidlist + ")";
                        Apis.UpdateDB(msc2, sql3);

                        using (sqlBC = new SqlBulkCopy(conn2))
                        {
                            sqlBC.BatchSize = 10000; //每次执行的行数
                            sqlBC.BulkCopyTimeout = 60; //超时时间设置单位秒
                            sqlBC.NotifyAfter = 10000;
                            sqlBC.DestinationTableName = "Compere"; //表
                            sqlBC.WriteToServer(dt2);                //执行写入数据库表
                            sqlBC.Close();
                            sqlBC = null;

                        }
                    }

                    lls.Clear();
                    lls = null;
                    if (dt2 != null)
                    {
                        dt2.Dispose();
                        dt2 = null;
                    }
                    if (ds != null)
                    {
                        ds.Dispose();
                        ds = null;
                    }
                    if (cmd != null)
                    {
                        cmd.Dispose();
                        cmd = null;
                    }

                    wf.addTbjmForThread("Compere表同步完毕！");
                }
                //program_preivew表
                sql = "select top 1 id,editTime from program_preview order by editTime desc";
                dr1 = Apis.searchInDB(msc2, sql);
                if (dr1 == null) return;

                stime2 = "";
                if (dr1.Read())
                {
                    stime2 = dr1.GetValue(1) + "";
                }
                dr1.Close();

                if (stime2 != "")
                {
                    sql = "select * from program_preview where edittime > '" + stime2 + "' order by edittime desc";
                    //fortesthere 
                    //sql = "select * from program_preview where id > 207264";

                    msc.CommandText = sql;
                    cmd = new SqlDataAdapter(msc);  //adapter可用于写和更新
                    builder = new SqlCommandBuilder(cmd);
                    ds = new DataSet();
                    cmd.Fill(ds, "program_preview");
                    dt2 = ds.Tables["program_preview"];
                    lls = new LinkedList<string>();
                    sb = new StringBuilder();
                    foreach (DataRow row in dt2.Rows)
                    {
                        String strid = row["id"] + "";
                        lls.AddFirst(strid);
                        sb.Append(strid);
                        sb.Append(",");
                    }


                    if (lls.Count > 0)
                    {
                        wf.addTbjmForThread("开始同步program_preview 表,需同步 " + lls.Count + " 条数据！");
                        //删除原有数据
                        String sidlist = sb.ToString();
                        sidlist = sidlist.Substring(0, sidlist.Length - 1);
                        String sql3 = "delete from program_preview where id in (" + sidlist + ")";
                        Apis.UpdateDB(msc2, sql3);

                        using (sqlBC = new SqlBulkCopy(conn2))
                        {
                            sqlBC.BatchSize = 10000; //每次执行的行数
                            sqlBC.BulkCopyTimeout = 60; //超时时间设置单位秒
                            sqlBC.NotifyAfter = 10000;
                            sqlBC.DestinationTableName = "program_preview"; //表
                            sqlBC.WriteToServer(dt2);                //执行写入数据库表
                            sqlBC.Close();
                            sqlBC = null;

                        }
                    }
                    if (lls != null)
                    {
                        lls.Clear();
                        lls = null;
                    }

                    if (dt2 != null)
                    {
                        dt2.Dispose();
                        dt2 = null;
                    }
                    if (ds != null)
                    {
                        ds.Dispose();
                        ds = null;
                    }
                    if (cmd != null)
                    {
                        cmd.Dispose();
                        cmd = null;
                    }
                }
                wf.addTbjmForThread("program_preview表同步完毕！");



                    ////tv
                    ////流程-->先查询94的最后一个电视台ID,在查询247的    update 2012.05.23 by zrw
                    //sql="select top 1 id from tv order by id desc";int tvid;
                    //tvid =Convert.ToInt32(Apis.upDBOneResult(conn2, sql, sql).ToString());

                    //if (tvid > 0)
                    //{

                    //    // sql = "select *  from TV where id > 402";  // old sql
                    //    sql = "select * from tv where id>" + tvid;
                    //    dr1 = Apis.searchInDB(msc, sql);//247
                    //    if (dr1 != null && dr1.Read() == true)
                    //    {
                    //        using (sqlBC = new SqlBulkCopy(conn2))
                    //        {
                    //            sqlBC.BatchSize = 10000; //每次执行的行数
                    //            sqlBC.BulkCopyTimeout = 60; //超时时间设置单位秒
                    //            sqlBC.NotifyAfter = 10000;
                    //            sqlBC.DestinationTableName = "TV"; //表
                    //            sqlBC.WriteToServer(dr1);                //执行写入数据库表
                    //            sqlBC.Close();
                    //            sqlBC = null;
                    //            dr1.Close();
                    //        }
                    //    }

                    //    if (dr1 != null)
                    //    {
                    //        dr1.Close();
                    //    }
                    //}
                   
                    ////tvchannel 流程：先查询94最后更新的时间--》根据时间查询247,--》同步对应频道 

                    //sql = "select top 1 id,editTime from TVChannel order by editTime desc";
                    //dr1 = Apis.searchInDB(msc2, sql);
                    //if (dr1 == null&&dr1.Read()==false) return;

                    //stime2 = "";
                    //if (dr1.Read())
                    //{
                    //    stime2 = dr1.GetValue(1) + "";
                    //}
                    //dr1.Close();

                    //if (stime2 != "")
                    //{
                    //    sql = "select * from TVChannel where edittime > '" + stime2 + "' order by edittime desc";
                    //    msc.CommandText = sql;
                    //    cmd = new SqlDataAdapter(msc);  //adapter可用于写和更新
                    //    builder = new SqlCommandBuilder(cmd);
                    //    ds = new DataSet();
                    //    cmd.Fill(ds, "TVChannel");
                    //    dt2 = ds.Tables["TVChannel"];
                    //    lls = new LinkedList<string>();
                    //    sb = new StringBuilder();
                    //    foreach (DataRow row in dt2.Rows)
                    //    {
                    //        String strid = row["id"] + "";
                    //        lls.AddFirst(strid);
                    //        sb.Append(strid);
                    //        sb.Append(",");
                    //    }


                    //    if (lls.Count > 0)
                    //    {
                    //        wf.addTbjmForThread("开始同步TVChannel 表,需同步 " + lls.Count + " 条数据！");
                    //        //删除原有数据
                    //        String sidlist = sb.ToString();
                    //        sidlist = sidlist.Substring(0, sidlist.Length - 1);
                    //        String sql3 = "delete from TVChannel where id in (" + sidlist + ")";
                    //        Apis.UpdateDB(msc2, sql3);

                    //        using (sqlBC = new SqlBulkCopy(conn2))
                    //        {
                    //            sqlBC.BatchSize = 10000; //每次执行的行数
                    //            sqlBC.BulkCopyTimeout = 60; //超时时间设置单位秒
                    //            sqlBC.NotifyAfter = 10000;
                    //            sqlBC.DestinationTableName = "TVChannel"; //表
                    //            sqlBC.WriteToServer(dt2);                //执行写入数据库表
                    //            sqlBC.Close();
                    //            sqlBC = null;

                    //        }
                      // }
                        //if (lls != null)
                        //{
                        //    lls.Clear();
                        //    lls = null;
                        //}

                        //if (dt2 != null)
                        //{
                        //    dt2.Dispose();
                        //    dt2 = null;
                        //}
                        //if (ds != null)
                        //{
                        //    ds.Dispose();
                        //    ds = null;
                        //}
                        //if (cmd != null)
                        //{
                        //    cmd.Dispose();
                        //    cmd = null;
                        //}
                   // }

                   // wf.addTbjmForThread("Tvchannel表同步完毕！");


                    //对删除的数据进行操作，compere ,forenotice表
                    //获得所有数据
                    sql = "select * from dic_delforTongbu order by id desc ";
                    dr1 = Apis.searchInDB(msc, sql);
                    if (dr1 == null) return;
                    StringBuilder sb2 = new StringBuilder();
                    sb = new StringBuilder();
                    while (dr1.Read())
                    {
                        if ((dr1.GetValue(1) + "") == "forenotice")
                        {
                            sb.Append(dr1.GetValue(2) + ",");
                        }
                        else if ((dr1.GetValue(1) + "") == "compere")
                        {
                            sb2.Append(dr1.GetValue(2) + ",");
                        }
                    }
                    dr1.Close();

                    String sdtmp = sb.ToString();
                    String sdtmp2 = sb2.ToString();
                    sql = "";

                    if (sdtmp != "")
                    {
                        sql = "delete forenotice where id in(" + sdtmp.Substring(0, sdtmp.Length - 1) + ")";
                        Apis.UpdateDB(msc2, sql);

                    }

                    if (sdtmp2 != "")
                    {
                        sql = "delete compere where id in(" + sdtmp2.Substring(0, sdtmp2.Length - 1) + ")";
                        Apis.UpdateDB(msc2, sql);

                    }

                    wf.addTbjmForThread("forenotice,compere表同步完毕！" + System.DateTime.Now.ToString() + "\r\n");


                    wf.addTbjmForThread("同步其余表结束！" + System.DateTime.Now.ToString() + "\r\n");
               
        
            }
            catch (Exception ex)
            {
                wf.addTbjmForThread(ex.ToString());
            }
            finally
            {

                if (sqlBC != null)
                {
                    sqlBC.Close();
                    sqlBC = null;
                }
                
                if (dr1 != null)
                {
                    dr1.Close();
                    dr1.Dispose();
                    dr1 = null;
                }
                if (ds != null)
                {
                    ds.Dispose();
                    ds = null;
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
               
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
                if (conn2 != null)
                {
                    conn2.Close();
                    conn2.Dispose();
                }


               //wf.Invoke((MethodInvoker)delegate { wf.btnTbDB(true); });
            }

            wf.addTbjmForThread(" 下一次同步其余表将在 4小时后");
            Thread.Sleep(3600000 * 4);//1000*60*60*12 = 60000*720
            TongbuDBOther(true);
        }

    }
}
