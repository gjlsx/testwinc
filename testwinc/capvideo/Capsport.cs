using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using testwinc.tools;
using System.Windows.Forms;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml;


namespace testwinc.capvideo
{
    class Capsport
    {
        private static WindForm wf = null;
        public static String strdbUrl = "Server=218.85.133.206;database=tvsport;uid=sa;pwd=FJwz%@)50";
        public static readonly String STR_DB_PWD = "FJwz%@)50";
        public static readonly String STR_DB_NAME = "sa";
        public static readonly String StrProjname = "capsport";
        private static VideoMember vmb = null;
        private static String strReg = @"[^\s>]*vs[^<]*";
        public static readonly String[] SPlayType = { "足球", "篮球" };
        private static readonly String[][] SVideoType = new String[][] { new String[] { "CNTV", "cntv." }, new String[] { "新浪","sina." }, new String[] { "优酷","youku." }, 
            new String[]{ "PPTV","pptv." },new String[] { "SOHU","sohu." },new String[] { "腾讯","qq." },new String[] { "网易","netease." },
            new String[] { "土豆","tudou." } ,new String[]{"zhibo8","zhibo8."}, new String[] { "迅雷","xunlei." } ,new String[]{"奇艺","qiyi."}
          };

        private static readonly String[] SContentStartText = new String[] { "新浪体育", "网易体育" };
        private static readonly String StrResult = "select top 1 SCOPE_IDENTITY() from video";

        static Capsport()
        {
            String st = Apis.getDbstr(StrProjname, STR_DB_PWD, STR_DB_NAME);
            if (st != "")
                strdbUrl = st;
            if (wf == null)
            {
                wf = UseStatic.getWindForm();
            }
        }  

        //抓节目视屏并入库
        public static void getSportVideo()
        {
            SqlDataAdapter sda = null;
            //DataTable Dt = null;
            SqlConnection cn = null;
            try
            {
                //cn = Apis.GetCon(strdbUrl);//连接数据库
                //SqlCommand msc = cn.CreateCommand();

                ////这里要判断是否数据已经入库，add code here

                //msc.CommandText = "select top 200 * from video order by id";
                //sda = new SqlDataAdapter(msc);  //adapter可用于写和更新
                //DataSet ds = new DataSet();

                //sda.Fill(ds, "find");
                //Dt = ds.Tables["find"];
                ////if (Dt.Rows.Count == 0) { }//没找到。

                //foreach (DataRow row in Dt.Rows)
                //{
                //    wf.addRtb2ForThread("id: " + row["id"] + "  title:  " + row["title"] + " videoType: "
                //        + row["videoType"] + " time: " + row["creatTime"].ToString() + "  url: " + row["surl"]);
                //}
                //test 批量增加数据
                //for (int i = 0; i < 3; i++)
                //{
                //    DataRow DR = Dt.NewRow();
                //    DR["title"] = "测试批量录入" + i.ToString();
                //    DR["surl"] = "http://www.tvsou.com/test.flv";
                //    DR["videoType"] = "fortest";
                //    DR["creatTime"] = DateTime.Now.ToString();
                //    Dt.Rows.Add(DR);
                //}
                //SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                //sda.Update(Dt);

            }
            catch (Exception e)
            {
                wf.addRtb2ForThread(e.StackTrace);
            }
               finally{
                 WindForm.isCappptv = false;
                 wf.getbtnpptv().Invoke((MethodInvoker)delegate { wf.getbtnpptv().Enabled = true; });
                //if (Dt != null)
                //{
                //    Dt.Dispose();
                //}
                if (sda != null)
                {
                    sda.Dispose();
                }
                if (cn != null)
                {
                    cn.Close();
                    cn.Dispose();
                }
            }
        }

        //抓直播8数据
        public static void getSportZhibo8()
        {
            try
            {
                String url = "http://www.zhibo8.com";
                //String url = "http://www.zhibo8.com/zuqiu/2011/";
                //String url = "http://www.zhibo8.com/nba/2011/";
                if (wf == null)
                {
                    wf = UseStatic.getWindForm();
                }
                String page = "", regex = "";

                page = Apis.GetPageCode(url, "GB2312");

                String[] datenow = getDatenow();
                page = Apitool.FindStrByName(page, datenow[0], datenow[1]);
                if (page == null)
                {
                    wf.addRtb2ForThread(url + " 网址打不开!");
                    return;
                }
                else
                {
                    wf.addRtb2ForThread("开始抓 " +url + " "+ datenow[0] + " 数据  ===============");
                }
                String[] strsplit = { "日 星期" };
                String[] pages = page.Split(strsplit, StringSplitOptions.RemoveEmptyEntries);
                regex = @"[^|]*vs[^|]*";
                Boolean isbreak = false;
                foreach (String stmp in pages)
                {
                    Match m = Apitool.GetResultOfReg(stmp, regex);
                    if (m != null)
                    {
                        while (m.Success)
                        {
                            if (WindForm.isCapyuku == false)
                            {
                                isbreak = true;
                                wf.addRtb2ForThread("抓取已被人工终止!");
                                break;
                            }
                            //int it = m.Index;
                            //int len = m.Length;
                            getTv8OneInfo(m.Value);
                            m = m.NextMatch();
                        }
                    }
                    if (isbreak) break;
                }

                wf.addRtb2ForThread(url + " " + datenow[0] + " 数据已被抓取");
            }
            catch (Exception e)
            {
                wf.addRtb2ForThread(e.StackTrace);
            }
            finally
            {
                WindForm.isCapyuku = false;
                wf.getbtnpptv().Invoke((MethodInvoker)delegate { wf.setbtnyuku(true); });
                WindForm.oneCatchEnd("isCapyuku");
            }
        }

        //获得一条比赛信息，string形如
        //切尔西vs利物浦 <a href="/zuqiu/2011/1120-qieerxi-jijin.htm" target="_blank">集锦</a> 
        //<a href="/zuqiu/2011/1120-qieerxi-luxiang.htm" target="_blank">录像</a> 
        private static void getTv8OneInfo(String value) 
        {
           
            String sitea = "", siteb = "" ;
            Match m = Apitool.GetResultOfReg(value, strReg);

            if (m != null)
            {
                if (m.Success)
                {
                    String st = m.Value.ToLower(); ;
                    int t = st.IndexOf("vs");
                    sitea = st.Substring(0, t).Trim();
                    siteb = st.Substring(t + "vs".Length).Trim(); 
                }
            }
            if (sitea == "" || siteb == "") return;
            int tta = sitea.IndexOf("-");
            if (tta != -1) 
            {
                sitea = sitea.Substring(tta + "-".Length);
            }
            tta = siteb.IndexOf("-");
            if (tta != -1)
            {
                siteb = siteb.Substring(tta + "-".Length);
            }
            Capsport.vmb = new VideoMember();
            vmb.sSitea = sitea;
            vmb.sSiteB = siteb;

            String[] rs = Apitool.getValueByName(value, "<a", "</a", "href");
            if (rs != null) //集锦或者直接单条链接
            {
                getSportYKDetail(rs);
            }
            value = value.Substring(value.IndexOf("</a",0));
            String[] rs2 = Apitool.getValueByName(value, "<a", "</a", "href");//录像或者网页tr，td类代码
            if (rs2 != null)
            {
                getSportYKDetail(rs2);
            }
        }

        //获得详细数据：播放地址和新闻
        private static void getSportYKDetail(String[] rs)
        {
            if (rs[1] == null) return;
            if ((rs[0].IndexOf("集锦", 0) == -1) && (rs[0].IndexOf("录像", 0) == -1)) return;
            String page = "", regex = "";
            //行如 /zuqiu/2011/1103-mu.htm 或http://www.xxx.com/zuqiu
            String[] stmp = rs[1].Replace("http://", "").Split('/');
            String stype = "",sdate = "";
            if (stmp != null && stmp.Length > 3)
            {
                if(stmp[1].IndexOf("zuqiu")!= -1)
                {
                    stype = SPlayType[0];
                }
                else if (stmp[1].IndexOf("nba") != -1)
                {
                    stype = SPlayType[1];
                }

                if(stmp[2].IndexOf("20")!= -1)
                {
                   sdate = stmp[2];
                    int day = stmp[3].IndexOf("-");
                   if(day != -1)
                    {
                        sdate = sdate + "-"+stmp[3].Substring(0,2) + "-" + stmp[3].Substring(2,2);
                    }
                }
            }
            String path = "";
            if (rs[1].IndexOf("www") == -1)
            {
                path = "http://www.zhibo8.com" + rs[1];
            }
            else 
            {
                path = rs[1];
            }
            page = Apis.GetPageCode(path, "GB2312");
            int index = page.IndexOf("<div class=\"title\">", 0);
            if (index == -1) return;

            vmb.sUrl = path;
            vmb.sProblemName = stype;
            vmb.playtime = sdate;
            vmb.sVideoType = "直播吧";
            page = page.Substring(index);
            String spagetmp = page;
            String stitle = Apitool.FindStrByName(page, "<h1>", "</h1>");
            if (stitle != null) vmb.sTitle = stitle;

            String idnow = "";
            LinkedList<string[]> llt = new LinkedList<string[]>();


            if (rs[0].IndexOf("集锦", 0) != -1)
            {
                if (stype == SPlayType[0])
                {//集锦有详细介绍
                    int isar = getStartTextPosit(page);

                    if (isar != -1)
                    {
                        int iend = spagetmp.IndexOf("<!-- JiaThis", isar);
                        if (iend != -1)
                        {
                            vmb.content = Apis.shtmlQuick(spagetmp.Substring(isar, iend - isar));
                        }
                    }
                    idnow = inserDBAll(vmb);//入库
                }
                else if (stype == SPlayType[1])
                {//篮球集锦
                    int iend = page.IndexOf("<table");
                    if (iend != -1)
                    {
                        page = page.Substring(0, iend + "<table".Length);
                        page = Apis.shtmlQuick(page);

                        //取得详细介绍
                        int iposit = 0;
                        int tmp = page.IndexOf("</a>", iposit);
                        while (tmp != -1)
                        {
                            if ((tmp - iposit) <= 477)
                            {
                                iposit = tmp;
                            }
                            else
                            {
                                break;
                            }
                            tmp = page.IndexOf("</a>", iposit + 4);
                        }
                        int iendpos = page.IndexOf("<table", iposit+4);
                        if (iendpos == -1)
                        {
                            iendpos = page.Length - iposit - "</a>".Length;
                        }
                        vmb.content = page.Substring(iposit + 4, iendpos - iposit - 4); ;

                        idnow = inserDBAll(vmb);//入库

                        regex = "<a [^<]*";
                        Match m = Apitool.GetResultOfReg(page.Substring(0, iposit + 4), regex);
                        if (m != null)
                        {
                            while (m.Success)
                            {
                                String surl = "", sname = "";
                                String[] rs2 = Apitool.getValueByName(m.Value + "<", "<a", "<", "href");
                                if (rs2 != null)
                                {
                                    if (rs2[0].IndexOf("更多与", 0) != -1) break;//最后一项去掉
                                    if (rs2[0].IndexOf("直播吧篮球公园", 0) != -1) break;//最后一项去掉
                                    if (rs2[0].IndexOf("观看]", 0) == -1 && rs2[0].IndexOf("[讨论]", 0) == -1)   //去掉[窗口-手机/Pad观看]
                                    {
                                        surl = rs2[1].Trim().Replace("http://", "");
                                        sname = rs2[0].Trim();

                                        String[] strtmp = { sname, surl, idnow };
                                        llt.AddLast(strtmp);
                                        //insertDBAddress(sname, surl, "zhibo8", idnow, "");
                                    }
                                }
                                m = m.NextMatch();
                            }
                        }
                    }
                }
            }

            if (!(rs[0].IndexOf("集锦", 0) != -1 && stype == SPlayType[1]))//录像和集锦足球
            {
                if (idnow == "")
                {
                    idnow = isInDb(vmb.sSitea, vmb.sSiteB, vmb.playtime,vmb.sProblemName);
                }
                if (idnow == ""||idnow == null) return;//没有比赛队伍信息，不入库返回
                index = getStartTextPosit(page);
               

                page = page.Substring(0, index);
                page = Apis.shtmlQuick(page);
                regex = "<a [^<]*";
                Match m = Apitool.GetResultOfReg(page, regex);
                if (m != null)
                {
                    while (m.Success)
                    {
                        String surl = "", sname = "";
                        //形如 href="http://sports.cntv.cn/20111123/101367.shtml" target="_blank">[录像]11月23日 欧冠小组赛 那不勒斯vs曼城 上半场录像
                        String[] rs2 = Apitool.getValueByName(m.Value + "<", "<a", "<", "href");
                        if (rs2 != null)
                        {
                            if (rs2[0].IndexOf("更多与", 0) != -1) break;//最后一项去掉
                            else if (rs2[0].IndexOf("观看]", 0) == -1 && rs2[0].IndexOf("[讨论]", 0) == -1)   //去掉[窗口-手机/Pad观看]
                            {
                                if (rs2[1].IndexOf("bbs.zhibo8.com") == -1)
                                {
                                    surl = rs2[1].Trim().Replace("http://", "");
                                    sname = rs2[0].Trim();
                                    String[] strtmp = { sname, surl, idnow };
                                    llt.AddLast(strtmp);
                                }
                            }
                        }
                        m = m.NextMatch();
                    }
                }
            }

            if (llt.Count != 0)
            {
                LinkedList<string[]> ltmp = isInDbAddress(llt, idnow);
                if(ltmp.Count != 0){
                    insertDBAddress(ltmp);
                }
            }
            
        }// end getSportYKDetail

        /// <summary>
        /// 根据队名和日期判断是否数据已经入库,已存在返回该id,否则返回空字符
        /// </summary>
        /// <param name="sa"></param>
        /// <param name="sb"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static String isInDb(String sa,String sb,String date,String sporttype) 
        {
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
            SqlConnection cn = null;
            try
            {
                String strfind = "select sitea,siteb,id from video where playTime = '" + date + "'" + " and probName = '" + sporttype + "'";
                cn = Apis.GetCon(strdbUrl);//连接数据库
                SqlCommand msc = cn.CreateCommand();
                msc.CommandText = strfind;
               
                dr1 = Apis.searchInDB(msc, strfind);
                while (dr1.Read())
                {
                    String at = dr1["sitea"].ToString().Trim();
                    String bt = dr1["siteb"].ToString().Trim();
                    sb = sb.Trim();
                    sa = sa.Trim();
                    if (at == sa && bt == sb) return dr1["id"].ToString();
                    if (at == sb && bt == sa) return dr1["id"].ToString();
                }
            }
            catch (Exception ex)
            {
                wf.addRtb2ForThread(ex.StackTrace);
            }
            finally
            {
                if (dr1 != null)
                {
                    dr1.Close();
                    dr1.Dispose();
                    dr1 = null;
                }
                if (cn != null)
                {
                    cn.Close();
                    cn.Dispose();
                    cn = null;
                }
            }
            return "";
        }

        /// <summary>
        /// 根据url和videoid判断是否数据已经在address表中,已存在返回该id,否则返回空字符
        /// </summary>
        /// <param name="videoid"></param>
        /// <param name="surl"></param>
        /// <returns></returns>
        public static String isInDbAddress(String surl,String videoid)
        {
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
            SqlConnection cn = null;
            try
            {
                String strfind = "select surl,id from address where videoid = " + videoid;
                cn = Apis.GetCon(strdbUrl);//连接数据库
                SqlCommand msc = cn.CreateCommand();

                msc.CommandText = strfind;
                dr1 = Apis.searchInDB(msc, strfind);
                while (dr1.Read())
                {
                    if (dr1["surl"].ToString().Trim().ToLower().Replace("http://", "") == surl.Trim().ToLower().Replace("http://", ""))
                        return dr1["id"].ToString();
                }
            }
            catch (Exception ex)
            {
                wf.addRtb2ForThread(ex.StackTrace);
            }
            finally
            {
                if (dr1 != null)
                {
                    dr1.Close();
                    dr1.Dispose();
                    dr1 = null;
                }
                if (cn != null)
                {
                    cn.Close();
                    cn.Dispose();
                    cn = null;
                }
            }
            return "";
        }

        //把已经存在项给移除掉
        private static LinkedList<string[]> isInDbAddress(LinkedList<string[]> llt,String videoid)
        {
            SqlConnection cn = null;
            SqlDataReader dr1 = null;
            LinkedList<string[]> ltmp = new LinkedList<string[]>();
            try
            {
                foreach (String[] istr in llt)
                {
                    ltmp.AddLast(istr);
                }

                String strfind = "select surl from address where videoid = " + videoid;
                cn = Apis.GetCon(strdbUrl);//连接数据库
                SqlCommand msc = cn.CreateCommand();

                msc.CommandText = strfind;
                dr1 = Apis.searchInDB(msc, strfind);
                while (dr1.Read())
                {
                    foreach (String[] istr in llt)
                    {
                        if (dr1["surl"].ToString().Trim().ToLower().Replace("http://", "") == istr[1].Trim().ToLower().Replace("http://", ""))
                        {
                            ltmp.Remove(istr);
                        }
                    }
                }
                return ltmp;
            }
            catch (Exception ex)
            {
                wf.addRtb2ForThread(ex.StackTrace);
                return ltmp;
            }
            finally
            {
                dr1.Close();
                cn.Close();
                cn.Dispose();
            }
        }

        private static String[] getDatenow() 
        {
            String sDate = "11月30日";
            String sDate2 = "12月01日";
            sDate = DateTime.Now.ToString("MMdd");//.Month + "月" + DateTime.Now.Day + "日";
            sDate = sDate.Substring(0, 2) + "月" + sDate.Substring(2) + "日"; 
            DateTime dt2 = DateTime.Now.AddDays(1);
            sDate2 = dt2.ToString("MMdd");
            sDate2 = sDate2.Substring(0, 2) + "月" + sDate2.Substring(2) + "日"; 
            // dt2.Month + "月" + dt2.Day + "日";

            String[] sdatenow = wf.gettextBox2Input().Split('|');

            //如1128:1129
             if (sdatenow.Length == 2)
             {
                 int ts = Int16.Parse(sdatenow[0]);
                 int te = Int16.Parse(sdatenow[1]);
                 if(ts < 1232 && ts > 10 && te < 1232 && te >10)
                 {
                     sDate = sdatenow[0].Substring(0, 2) + "月" + sdatenow[0].Substring(2) + "日";
                     sDate2 = sdatenow[1].Substring(0, 2) + "月" + sdatenow[1].Substring(2) + "日";
                 }
             }
            return new String[]{sDate,sDate2};
        }

        //插入一条数据，1.title,2.surl,3.videoType,均为列名
        public static void inserDB(String title, String surl, String videoType)
        {
            String strInser = "insert into video(title,surl,videoType,creatTime) values('" + title + "','"
                + surl + "','" + videoType + "','" + DateTime.Now.ToString() + "')";
            Apis.runSql(strInser,strdbUrl);
        }



        //入库address表
        public static void insertDBAddress(String title, String surl,String videoType,String videoID,String content,String imageurl,String timelong)
        {
               String strInser = "insert into address(title,surl,videoType,creatTime,videoid,content,imageurl,timelong) values('" + title + "','"
                + surl + "','" + videoType + "','" + DateTime.Now.ToString() + "','" + videoID + "','" + content + "','" + imageurl + "','" + timelong + "')";
               Apis.runSql(strInser, strdbUrl);
        }

        //批量入库address表
        private static void insertDBAddress(LinkedList<string[]> ltmp)
        {
            if (ltmp == null || ltmp.Count == 0) return;
            SqlConnection cnst = null;
            SqlCommand Comm = null;
            try
            {
                cnst = Apis.GetCon(strdbUrl);//连接数据库
                Comm = cnst.CreateCommand();

                foreach (String[] strt in ltmp)
                {
                    //insertDBAddress(sname, surl, "zhibo8", idnow, "");
                    if (strt[0].Length > 6) //标题小于6个字符不入库
                    {
                        String svideotype = getVideoTypeFromURL(strt[1]);
                        String strInser = "insert into address(title,surl,videoType,creatTime,videoid,content) values('" + strt[0] + "','"
                         + strt[1] + "','" + svideotype + "','" + DateTime.Now.ToString() + "','" + strt[2] + "','" + "" + "')";
                        Apis.UpdateDB(Comm, strInser);

                        wf.addRtb2ForThread(strt[0] + " 已入库");
                    }
                }
            }
            catch (Exception ex) 
            {
                wf.addRtb2ForThread(ex.StackTrace);
            }
            finally
            {
                Comm.Dispose();
                cnst.Close();
                cnst.Dispose();
            }
        }
  
        private static void inserDBAll(String title, String surl, String videoType, String probname, String playtime, int forenoticid, String sitea,
            String siteb,String content)
        {
            String strInser = "insert into video(title,surl,videoType,probname,creatTime,playtime,forenoticid,sitea,siteb,content) values('" + title 
                + "','" + surl + "','" + videoType + "','" + probname + "','" + DateTime.Now.ToString() + "','" + playtime + "','" 
                + forenoticid + "','" + sitea + "','" + siteb + "','" + content + "')";
            Apis.runSql(strInser, strdbUrl);
        }

        //
        /// <summary>
        /// 插入一条数据，1.title,2.surl,3.videoType,4 problemname 5.creattime 6 palytime 7 forenoticed 8 site a  9 site b 10 .content 
        /// </summary>
        public static String inserDBAll(VideoMember vm)
        {
            if (vm.sTitle == "全场集锦稍候更新" || vm.sTitle == "全场集锦") return "";
            String isexist = isInDb(vm.sSitea,vm.sSiteB,vm.playtime,vm.sProblemName);
            if (isexist != "")
            {
                wf.addRtb2ForThread(vm.sTitle + " " + vm.playtime + " 已经存在");
                return isexist;
            }

            String strInser = "insert into video(title,surl,videoType,probname,creatTime,playtime,forenoticid,sitea,siteb,content) values('" + vm.sTitle
                + "','" + vm.sUrl + "','" + vm.sVideoType + "','" + vm.sProblemName + "','" + DateTime.Now.ToString() + "','" + vm.playtime + "','"
                + vm.forenoticid + "','" + vm.sSitea + "','" + vm.sSiteB + "','" + vm.content + "')";

            //String strResult = "select top 1 SCOPE_IDENTITY() from video";//取得该范围内插入某表的最后一条数据id

            wf.addRtb2ForThread(vm.sTitle + " " + vm.playtime + " 正在入库");
            return Apis.runSqlOneResult(strInser, StrResult,strdbUrl)+"";
        }

        //删除数据，以title为主键
        private static void deleteDB(String title)
        {
            String strDel = "delete from video where title = '" + title + "'";
            Apis.runSql(strDel, strdbUrl);
        }

        //private static Object runSqlAllInOne(String[] strsql, String strResultTable)
        //{
        //    SqlConnection cn = Apis.GetCon(strdbUrl);//连接数据库
        //    Object obj = Apis.upDBOneResult(cn, strsql, strResultTable);

        //    cn.Close();
        //    return obj;
        //}

        //查找段落文本位置，未找到返回 -1
        private static int getStartTextPosit(String page)
        {
           int tt = -1;
           foreach (String str in SContentStartText)
            {
                tt = page.IndexOf(str);
                if (tt != -1)
                {
                    if(tt > 27){
                        if (page.Substring(tt - 27, 27).IndexOf("本文来源") == -1)
                            return tt;
                        else
                            break;
                    }
                }
            }
            tt = page.IndexOf("更多与");
            if (tt == -1) 
                {
                    //取得开始地址,假设文本长度为577个字符
                    int iposit = 0;
                    int tmp = page.IndexOf("</a>", iposit);
                    while (tmp != -1)
                    {
                        if ((tmp - iposit) <= 577)
                        {
                            iposit = tmp;
                        }
                        else
                        {
                            break;
                        }
                        tmp = page.IndexOf("</a>", iposit + 4);
                    }
                    tt = iposit + 4;
                }
            else
            {
                int tmp = page.IndexOf("</a>", tt);
                if (tmp != -1)
                    return tmp + 4;
            }
            return tt;
        }

        /// <summary>
        /// 根据url取得对应播放站点名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static String getVideoTypeFromURL(String url) 
        {
            String st = url.ToLower();
            foreach (String[] str in SVideoType)
            {
                if (url.IndexOf(str[1]) != -1)
                    return str[0];
            }
            return "unknow";
        }

        /// <summary>
        /// 释放该类所占资源
        /// </summary>
        public static void dispose()
        {
            if (vmb != null)
            {
                vmb = null;
            }
        }


        public Boolean isCapturedTV(String sDate, String sitea, String siteb)
        {
            //SqlDataAdapter sda = null;
            //DataTable Dt = null;
            //SqlConnection cn = null;
         
            //    if (wf == null) {
            //        wf = UseStatic.getWindForm();
            //    }
            //    cn = Apis.GetCon(strdbUrl);//连接数据库
            //    SqlCommand msc = cn.CreateCommand();

            //    msc.CommandText = "select top 100 * from video order by id";
                
            //    sda = new SqlDataAdapter(msc);  //adapter可用于写和更新
            //    DataSet ds = new DataSet();

            //    sda.Fill(ds, "find");
            //    Dt = ds.Tables["find"];
            
            //SqlDataReader sdr1;
            //sqlc1.CommandText = "select status,souce from catchLog where channelID=" + ChannelID + " and catchdate='" + sDate + "'";
            //sdr1 = sqlc1.ExecuteReader();

            //if (sdr1.Read())
            //{
            //    if ((bool)sdr1.GetValue(0) || sdr1.GetValue(1).ToString() == "4")
            //    {
            //        if (sOutTime == "")
            //        {
            //            sOutTime = "06:00";
            //        }
            //        sdr1.Close();

            //        txt1.Invoke((MethodInvoker)delegate { txt1.AppendText(sDate + " " + ChannelName + "的节目上次抓取出错了!现在重新抓取" + "\r\n"); });
            //        ///删除当天的数据和标记
            //        apia.UpdateDB(cnsc, "delete catchLog where catchDate='" + sDate + "' and channelID=" + ChannelID);
            //        //	txt1.AppendText("delete program where  playtime >='"+sDate+" "+sOutTime+"' and playtime <DATEADD(d, 1, '"+sDate+" "+sOutTime+"') and channelid = "+ChannelID+" \r\n");
            //        apia.UpdateDB(cnsc, "delete program where  playtime >='" + sDate + " " + sOutTime + "' and playtime <DATEADD(d, 1, '" + sDate + " " + sOutTime + "') and channelid =" + ChannelID + " ");
            //        cnsc.Close();
            //        return false;
            //    }
            //    else
            //    {
            //        //如果已经进行过该日期的节目抓取，则不再重复抓取
            //        txt1.Invoke((MethodInvoker)delegate { txt1.AppendText(sDate + " " + ChannelName + "的节目抓取过了!" + "\r\n"); });
            //        sdr1.Close();
            //        cnsc.Close();
            //        return true;
            //    }

            //}
            //sdr1.Close();
            //cnsc.Close();
            return false;
        }
    }//end Capsport
}
