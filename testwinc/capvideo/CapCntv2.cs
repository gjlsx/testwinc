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
    /// <summary>
    /// 抓央视xml数据并入库类  by wind 12.06.25
    /// </summary>
    class CapCntv2
    {
        public static String strdbUrl = "Server=218.85.133.206;database=tvsport;uid=sa;pwd=FJwz%@)50";
        private static WindForm wf = null;
        private static MovieMember vmb = null;
        private static VideoMember vb = null;

        public static readonly String[] STagname = { "名栏目", "电视剧" };

        static CapCntv2()
        {
            String st = Apis.getDbstr(Capsport.StrProjname, Capsport.STR_DB_PWD, Capsport.STR_DB_NAME);
            if (st != "")
                strdbUrl = st;
            if (wf == null)
            {
                wf = UseStatic.getWindForm();
            }
        }

        //抓cntvxml
        public static void getCntvXML()
        {
            try
            {
                //该xml 每日更新
                String url = "http://sitemap.cntv.cn/xml/tv/hao360/hao360_tv_5.xml";

                //String page = "", regex = "";

                wf.addRtb2ForThread("开始抓 " + url + " " + " 数据 \r\n");

                XmlDocument xdom = new XmlDocument();
                xdom.Load(url);
                XmlNodeList nl = xdom.GetElementsByTagName("item");
                if (nl != null)
                {
                    foreach (XmlNode xnode in nl)
                    {
                        getInfoxml(xnode);
                        if (WindForm.isCapCntv == false)
                        {
                            wf.addRtb2ForThread("抓取已被人工终止!");
                            break;
                        }
                    }
                }

                wf.addRtb2ForThread(url + " " + " 数据已被抓取");
            }
            catch (Exception e)
            {
                wf.addRtb2ForThread(e.StackTrace);
            }
            finally
            {
                WindForm.isCapCntv = false;
                wf.Invoke((MethodInvoker)delegate { wf.setbtncntv(true); });
                WindForm.oneCatchEnd("isCapCntv");
            }
        }


        /**
         * <op>add</op>
         * <title>《多多的婚事》 第1集</title>
         * <playLink>http://baidu.cntv.cn/act/schedule.jsp?scheduleId=SCHE1323605401715647&videoId=8c9656befc3c4b4c95f6a04bfb1fb9be</playLink>
         * <imageLink>http://p3.img.cctvpic.com/fmspic/2011/12/10/8c9656befc3c4b4c95f6a04bfb1fb9be-180.jpg</imageLink>
         * <category>电视剧</category>
         * <tag>多多的婚事</tag>
         * <comment>钱多多，如今面临着一个大难题，要过春节了，年近三十，在感情上仍一无所获的她，一想到回杭州老家面对父母的责难就胆战心惊...</comment>
         * <duration>2486</duration>
         * <pubDate>2011-12-11 20:10:03.0</pubDate> 
         * 
         */
        private static void getInfoxml(XmlNode nl)
        {
            if (nl != null)
            {
                String title = "", surl = "", imageurl = "", category = "", tag = "", comment = "", duration = "0", pubDate = "", js = "";
                //通过SelectSingleNode方法获得当前节点下的courses子节点
                XmlNode nd = nl.SelectSingleNode("title");
                if (nd != null) title = nd.InnerText;

                nd = nl.SelectSingleNode("playLink");
                if (nd != null) surl = nd.InnerText;

                nd = nl.SelectSingleNode("imageLink");
                if (nd != null) imageurl = nd.InnerText;

                nd = nl.SelectSingleNode("category");
                if (nd != null) category = nd.InnerText;


                nd = nl.SelectSingleNode("tag");
                if (nd != null) tag = nd.InnerText;

                nd = nl.SelectSingleNode("comment");
                if (nd != null) comment = nd.InnerText;

                nd = nl.SelectSingleNode("duration");
                if (nd != null) duration = nd.InnerText;

                nd = nl.SelectSingleNode("pubDate");
                if (nd != null) pubDate = nd.InnerText;

                vmb = new MovieMember();
                if (STagname[1] == category) //电视剧
                {
                    vmb.sTitle = tag;
                    vmb.tvnum = 1;
                    //还要判断集数,add code here// 第2集<
                    int itmp = title.IndexOf(" 第");
                    int iend = title.LastIndexOf("集");
                    if (itmp != -1 && iend != -1 && itmp < iend)
                    {
                        js = title.Substring(itmp + " 第".Length, iend - itmp - " 第".Length);
                    }
                }
                else
                {
                    //if (title.IndexOf("勇敢游戏") != -1) 
                    //{
                    //    int abcdefg = 1; 
                    //}
                    String st = Apitool.FindStrByName(title, "《", "》");
                    if (st != null)
                        vmb.sTitle = st;
                    else
                        vmb.sTitle = title;
                    if (tag.IndexOf("影视同期声") != -1)
                        vmb.sTitle = "影视同期声";
                    else if
                        (tag.IndexOf("朝闻天下") != -1) vmb.sTitle = "朝闻天下";
                    else if (tag.IndexOf("新闻30分") != -1) vmb.sTitle = "新闻30分";
                    else if (tag.IndexOf("春晚") != -1) vmb.sTitle = tag;
                    else if (title.IndexOf("[NBA]") != -1) vmb.sTitle = tag;
                    vmb.tvnum = 3;//央视名栏目
                    vmb.sType = tag;
                }
                if (title.IndexOf("庭审现场") != -1)
                {

                }

                vmb.sUrl = surl;
                vmb.simageUrl = imageurl;
                vmb.time = duration;//以秒为单位
                vmb.syear = DateTime.Now.Year + "";
                vmb.sContent = comment;
                vmb.sPlayTime = pubDate;
                if (js == "") js = "0";

                vmb.shtmlString();

                if (vmb.tvnum == 3)
                {
                    if (getSportVideo(vmb)) return;//体育节目入体育库
                }

                String sid = inserDBAll(vmb);
                if (sid != "" && vmb.sUrl != "")
                {
                    if (Capmovie.isInMovieUrl(vmb.sUrl, sid) == "")
                    {
                        Capmovie.insertDBAddress(sid, title, Capmovie.SMovieType[2], vmb.sUrl, js, vmb.sPlayTime, vmb.time);
                        wf.addRtb2ForThread(vmb.sTitle + " ID: " + sid + " : " + vmb.sUrl + " 已入库");
                    }
                }
            }
        }


        /// <summary>
        /// //把体育节目独立出来
        /// </summary>
        /// <param name="vmb"></param>
        private static Boolean getSportVideo(MovieMember vmb)
        {
            //(CBA)常规赛12月10日：东莞VS广厦 第2节
            int itmp = vmb.sTitle.IndexOf("(CBA)");
            int ik = 0;
            String st = "";
            vb = null;
            if (itmp != -1)
            {
                vb = new VideoMember();
                vb.sProblemName = Capsport.SPlayType[1];
                vb.sVideoType = Capmovie.SMovieType[2];
                vb.sTitle = vmb.sTitle;

                itmp = vmb.sTitle.IndexOf("日：");
                if (itmp != -1)
                {
                    ik = vmb.sTitle.Substring(0, itmp).LastIndexOf("月");
                    if (ik != -1)
                    {
                        st = vmb.sTitle.Substring(ik - 2, 1);
                        int ilen = 1;
                        if (st == "1" || st == "0")
                        {
                            ilen = 2;
                        }
                        vb.playtime = DateTime.Today.Year + "-" +
                                vmb.sTitle.Substring(ik - ilen, ilen) + "-" + vmb.sTitle.Substring(ik + 1, itmp - ik - 1);
                    }
                    ik = vmb.sTitle.IndexOf("VS", itmp);
                    if (ik != -1)
                    {
                        vb.sSitea = vmb.sTitle.Substring(itmp + "日：".Length, ik - itmp - "日：".Length);
                        itmp = vmb.sTitle.IndexOf(" ", ik);
                        if (itmp != -1)
                        {
                            vb.sSiteB = vmb.sTitle.Substring(ik + 2, itmp - ik - 2);
                        }
                        else
                        {
                            vb.sSiteB = vmb.sTitle.Substring(ik + 2);
                        }
                    }
                }

            }
            else
            {
                //2011/2012赛季美国男子篮球职业联赛 雄鹿-快船 第1节 20120108
                //2011/2012赛季美国职业篮球联赛 勇士-湖人 第4节 20120107
                //2011/2012赛季中国男子篮球职业联赛 江苏中天钢铁-新疆沃尔沃 第4节 20120108
                itmp = vmb.sTitle.IndexOf("美国男子篮球");
                if (itmp == -1)
                    itmp = vmb.sTitle.IndexOf("美国职业篮球");
                if (itmp == -1)
                    itmp = vmb.sTitle.IndexOf("中国男子篮球");
                if (itmp == -1)
                    itmp = vmb.sTitle.IndexOf("中国职业篮球");
                if (itmp != -1)
                {
                    vb = new VideoMember();
                    vb.sProblemName = Capsport.SPlayType[1];
                    vb.sVideoType = Capmovie.SMovieType[2];
                    vb.sTitle = vmb.sTitle;

                    itmp = vmb.sTitle.IndexOf("-");
                    if (itmp != -1)
                    {
                        ik = vmb.sTitle.LastIndexOf(" ");
                        if (ik != -1)
                        {
                            st = vmb.sTitle.Substring(ik + 1).Trim();
                            if (st.Length == 8 && int.TryParse(st, out ik))
                            {
                                if (ik > 20000101 && ik < 20220101)
                                    vb.playtime = st.Substring(0, 4) + "-" +
                                     st.Substring(4, 2) + "-" + st.Substring(6, 2);
                            }
                        }
                        ik = vmb.sTitle.Substring(0, itmp).LastIndexOf(" ");
                        if (ik != -1)
                        {
                            vb.sSitea = vmb.sTitle.Substring(ik + 1, itmp - ik - 1);
                        }
                        ik = vmb.sTitle.IndexOf(" ", itmp);
                        if (ik != -1)
                        {
                            vb.sSiteB = vmb.sTitle.Substring(itmp + 1, ik - itmp - 1);
                        }
                    }

                }
                else
                {
                    //(意甲)第15轮：莱切VS拉齐奥 上半场
                    itmp = vmb.sTitle.IndexOf("(意甲)");
                    if (itmp == -1)
                        itmp = vmb.sTitle.IndexOf("(德甲)");
                    if (itmp == -1)
                        itmp = vmb.sTitle.IndexOf("(西甲)");
                    if (itmp != -1)
                    {
                        vb = new VideoMember();
                        vb.sProblemName = Capsport.SPlayType[0];
                        vb.sVideoType = Capmovie.SMovieType[2];
                        vb.sTitle = vmb.sTitle;
                        vb.playtime = vmb.sPlayTime;

                        itmp = vmb.sTitle.IndexOf("：");

                        if (itmp != -1)
                        {
                            ik = vmb.sTitle.IndexOf("VS", itmp);
                            if (ik != -1)
                            {
                                vb.sSitea = vmb.sTitle.Substring(itmp + 1, ik - itmp - 1);
                                itmp = vmb.sTitle.IndexOf(" ", ik);
                                if (itmp != -1)
                                {
                                    vb.sSiteB = vmb.sTitle.Substring(ik + 2, itmp - ik - 2);
                                }
                                else
                                {
                                    vb.sSiteB = vmb.sTitle.Substring(ik + 2);
                                }
                            }
                        }
                    }
                    else
                    {
                        //2011/2012赛季意大利足球甲级联赛第17轮 锡耶纳-拉齐奥 下半场 20120108
                        //2011/2012赛季西班牙足球甲级联赛第18轮 皇家马德里-格拉纳达 下半场 20120108
                        itmp = vmb.sTitle.IndexOf("意大利足球甲级");
                        if (itmp == -1)
                            itmp = vmb.sTitle.IndexOf("西班牙足球甲级");
                        if (itmp != -1)
                        {
                            vb = new VideoMember();
                            vb.sProblemName = Capsport.SPlayType[0];
                            vb.sVideoType = Capmovie.SMovieType[2];
                            vb.sTitle = vmb.sTitle;
                            vb.playtime = vmb.sPlayTime;
                            itmp = vmb.sTitle.IndexOf("-");
                            if (itmp != -1)
                            {
                                ik = vmb.sTitle.LastIndexOf(" ");
                                if (ik != -1)
                                {
                                    st = vmb.sTitle.Substring(ik + 1).Trim();
                                    if (st.Length == 8 && int.TryParse(st, out ik))
                                    {
                                        if (ik > 20000101 && ik < 20220101)
                                            vb.playtime = st.Substring(0, 4) + "-" +
                                             st.Substring(4, 2) + "-" + st.Substring(6, 2);
                                    }
                                }
                                ik = vmb.sTitle.Substring(0, itmp).LastIndexOf(" ");
                                if (ik != -1)
                                {
                                    vb.sSitea = vmb.sTitle.Substring(ik + 1, itmp - ik - 1);
                                }
                                ik = vmb.sTitle.IndexOf(" ", itmp);
                                if (ik != -1)
                                {
                                    vb.sSiteB = vmb.sTitle.Substring(itmp + 1, ik - itmp - 1);
                                }
                            }
                        }

                    }
                }
            }
            if (vb != null)
            {
                if (vb.sSitea != "" && vb.sSiteB != "" && vb.playtime != "")
                {
                    String sid = Capsport.inserDBAll(vb);
                    if (sid != null && sid != "")
                    {
                        if (Capsport.isInDbAddress(vmb.sUrl, sid) != "")//已存在
                            return true;
                        Capsport.insertDBAddress(vb.sTitle, vmb.sUrl, vb.sVideoType, sid, vmb.sContent, vmb.simageUrl, vmb.time);
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 插入一条数据，1.title,2.years,3 type,4 froeid 5.time 6 creattime 7 director 8 presenter 9 url 10 .content 
        /// 返回已插入或已存在数据的movie id
        /// </summary>
        private static String inserDBAll(MovieMember vm)
        {
            String isexist = isInDb(vm);
            if (isexist != "")
            {
                wf.addRtb2ForThread(vm.sTitle + " 已经存在, movie ID = " + isexist);
                return isexist;
            }
            String strInser = "insert into movie(title,years,type,time,creattime,edittime,foreid,imageurl,content,tvnum) values('" + vm.sTitle
                + "','" + vm.syear + "','" + vm.sType + "','" + vm.time + "','" + DateTime.Now.ToString() + "','" + DateTime.Now.ToString()
                + "','" + vm.forenoticid + "','" + vm.simageUrl + "','" + vm.sContent + "','" + vm.tvnum + "')";

            return Apis.runSqlOneResult(strInser, Capmovie.StrResult, strdbUrl) + "";
        }

        /// <summary>
        /// 根据movie名和年份，导演等判断是否数据已经入库,已存在返回该id,否则返回空字符
        /// </summary>
        /// <param name="vm">MovieMember</param>
        /// <returns>id或""</returns>
        private static String isInDb(MovieMember vm)
        {
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
            SqlConnection cn = null;
            try
            {
                String strfind = "";
                if (vm.tvnum == 1)
                {
                    strfind = "select id,title,years,zjs,tvnum from movie where title like '%" + vm.sTitle + "%' and tvnum > 0 and tvnum < 3";
                }
                else if (vm.tvnum == 3)
                {
                    strfind = "select id,title,years,tvnum from movie where title like '%" + vm.sTitle + "%' and tvnum = 3";
                }
                cn = Apis.GetCon(strdbUrl);//连接数据库
                SqlCommand msc = cn.CreateCommand();
                //这里要判断是否数据已经入库，add code here
                msc.CommandText = strfind;

                dr1 = Apis.searchInDB(msc, strfind);
                while (dr1.Read())
                {
                    String title = dr1["title"].ToString().Trim();
                    //String dire = dr1["tvnum"].ToString().Trim();
                    // String year = dr1["years"].ToString().Trim();
                    if (title == vm.sTitle)//数据不全
                    {
                        // if (vm.tvnum == 1)
                        return dr1["id"].ToString();
                    }
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

        public static void dispose()
        {
            if (vmb != null)
            {
                vmb = null;
            }
            if (vb != null)
                vb = null;
        }

    }//end class CapCntvXML 
}
