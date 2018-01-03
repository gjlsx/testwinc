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
    class CapYouku
    {
        public static String strdbUrl = "Server=218.85.133.206;database=tvsport;uid=sa;pwd=FJwz%@)50";
        private static WindForm wf = null;
        private static MovieMember vmb = null;
        private static readonly String start = "通栏图片列表";//数据页面开始
        private static readonly String start2 = "class=\"items\">";
        private static readonly String strpage = "class=\"next\"";//下一页的div 
        private static readonly String strend = "class=\"qPager\">";//最后一页数据结束标志
        private static String sUrl = ""; //www.youku.com/v_olist/c_97_a__s__g__r_2011_lg__im__st__mt__d_1_et_0_fv_0_fl__fc__fe__o_7_p_1.html
        private static String sYear = "";
        private static int Itvnum = -1;
        private static readonly String StrYoukuAddress = "www.youku.com";


        private static readonly String[] years = new String[] { 
                    "2012"
                    //,"2011", 
                   // "2010", "2009", "2008", "2007", "2006", "2005", "2004", "2003", "2002", "2001", "2000","1990", "1980","1970"
        };

        private static readonly String[] Stype = new String[] { 
                    "/v_olist/c_96", "/v_olist/c_97", "/v_olist/c_85", "/v_olist/c_100"};

        private static readonly int[] StypeNum = new int[] { 
                    Capmovie.ITvNum[0], Capmovie.ITvNum[2],Capmovie.ITvNum[4] //,Capmovie.ITvNum[1]
        };

        static CapYouku()
        {
            String st = Apis.getDbstr(Capsport.StrProjname, Capsport.STR_DB_PWD, Capsport.STR_DB_NAME);
            if (st != "")
                strdbUrl = st;
            if (wf == null)
            {
                wf = UseStatic.getWindForm();
            }
        }

        //抓视频数据
        public static void getYoukuVideo()
        {

          //getYoukuZongyi();

            try
            {
                sUrl = wf.getTbxAddress().ToLower();
                Boolean isCapByBrowserweb = true;
                //如果浏览器栏是youku页面，就从浏览器栏的页面抓取
                if (sUrl.IndexOf("youku.com") != -1)
                {
                    wf.addRtb2ForThread("开始抓 " + sUrl + " " + " 数据");
                }
                else
                {//2011 youku 电影
                    wf.addRtb2ForThread("开始抓取优酷 电影,动画，综艺，要抓其他请在浏览器中输入优酷视频页面地址！  \r\n");
                    sUrl = "";
                    isCapByBrowserweb = false;
                }
                //String su1 = "http://movie.youku.com/search?ccat40486[r]=";
                //String su2 = "&m40487[cc-ms-q]=a|releaseyear%3A";
                String su1 = "http://www.youku.com";
                String su2 = "_a__s__g__r_";


                //http://comic.youku.com/search?ccat41458[r]=2011&m41459[cc-ms-q]=a|releaseyear%3A2011  动画
                //http://zy.youku.com/search?ccat40640[r]=2010&m40641[cc-ms-q]=a|releaseyear%3A2010 综艺
                //http://movie.youku.com/search?ccat40486[r]=2009&m40487[cc-ms-q]=a|releaseyear%3A2009 电影

                Itvnum = getYoukuTvnum(); // 是否手动输入tvnum状态控制

                sYear = getYear();

                if (isCapByBrowserweb)
                {
                    if (Itvnum == -1 || sYear == "")
                    {
                        wf.addRtb2ForThread("请输入tvnum! year, example:  y-2012|n-0");
                        return;
                    }
                }

                foreach (int tnum in StypeNum)
                {
                    if (WindForm.isCapyouku == false)
                    {
                        break;
                    }
                    if (!isCapByBrowserweb)
                    {
                       
                        if (tnum == Capmovie.ITvNum[2])//优酷动画
                        {
                            su1 = "http://comic.youku.com/search?ccat41458[r]=";
                            su2 = "&m41459[cc-ms-q]=a|releaseyear%3A";
                        }
                        else if (tnum == Capmovie.ITvNum[4])
                        {//http://zy.youku.com/search?ccat40640[r]=2010&m40641[cc-ms-q]=a|releaseyear%3A2010 //综艺
                            su1 = "http://zy.youku.com/search?ccat40640[r]=";
                            su2 = "&m40641[cc-ms-q]=a|releaseyear%3A";
                        }
                        else if (tnum == Capmovie.ITvNum[0])
                        {//http://movie.youku.com/search?ccat40486[r]=2011&m40487[cc-ms-q]=a|releaseyear%3A2011 //电影
                            su1 = "http://movie.youku.com/search?ccat40486[r]=";
                            su2 = "&m40487[cc-ms-q]=a|releaseyear%3A";
                        }
                        else if (tnum == Capmovie.ITvNum[1])//！电视剧节目手动输入网址抓
                        {
                            continue;
                        }
                        Itvnum = tnum;
                    }
                    else
                    {
                        if (Itvnum != -1 && Itvnum != tnum)
                        {
                            continue;
                        }
                    }
                   
                    foreach (String sye in years)
                    {
                        if (WindForm.isCapyouku == false)
                        {
                            break;
                        }
                        String page = "";
                        if (!isCapByBrowserweb)
                        {
                            //sUrl = su1 + stp + su2 + sye + su3;
                            sUrl = su1 + sye + su2 + sye;
                            page = Apis.GetPageCode(sUrl, "utf-8");
                        }
                        else
                        {
                            page = Apis.GetPageCode(sUrl, "utf-8");

                        }
                        if (page == null)
                        {
                            wf.addRtb2ForThread(" 网址打不开!");
                            continue;
                        }
                        if (sYear == "") sYear = sye;

                         String snext = getOneInfo(page);

                        while (snext != null && snext != "") 
                        {
                            if(snext.IndexOf("http://") == -1)
                                snext = su1 + snext;
                            page = Apis.GetPageCode(snext, "utf-8");

                            snext = getOneInfo(page);
                            if (WindForm.isCapyouku == false)
                            {
                                break;
                            }
                        }
                        sUrl = "";
                        sYear = "";
                        if (isCapByBrowserweb) break;
                    }
                    if (isCapByBrowserweb) break;
                }
                wf.addRtb2ForThread("所有数据已被抓取");
            }
            catch (Exception e)
            {
                wf.addRtb2ForThread(e.StackTrace);
            }
            finally
            {
                WindForm.isCapyouku = false;
                wf.Invoke((MethodInvoker)delegate { wf.setbtnUku(true); });
                WindForm.oneCatchEnd("isCapyouku");
            }
        }

        private static String getYear() 
        {
            String sye = wf.gettextBox2Input();
            if (sye != null && sye != "")
            {
                sye = sye.ToUpper();
                int it = sye.IndexOf("Y-");
                if (it != -1)
                {
                    int ik = sye.IndexOf("|", it);
                    if (ik == -1) ik = sye.Length;
                    sye = sye.Substring(it + 2,ik - it -2);
                    if (int.TryParse(sye, out it))
                        return sye;
                }
            }
            return "";
        }

        private static String getOneInfo(String page)
        {
            if (page == null) return "";
            int ist = page.IndexOf(start2);
            if (ist == -1)
            {
                ist = page.IndexOf(start);
                if (ist == -1)
                    return "";
            }

            String snext = "";
            int ise = page.IndexOf(strpage, ist);
            //Boolean isendPage = false;
            if (ise == -1)
            {
                //isendPage = true;
                ise = page.IndexOf(strend, ist);
                if (ise == -1)
                    ise = page.Length;
            }
            else
            {
                int isk = page.IndexOf("<", ise);
                if (isk != -1)
                {
                    String[] srs = Apitool.getValueByName(page.Substring(isk), "<", ">", "href");
                    if(srs != null)
                       snext = srs[1];
                }
            }
                page = page.Substring(ist, ise - ist);

                String[] strsplit = { "<img src=\"" };
                String[] pages = page.Split(strsplit, StringSplitOptions.RemoveEmptyEntries);

                foreach (String stmp in pages)
                {
                    if (WindForm.isCapyouku == false)
                    {
                        wf.addRtb2ForThread("抓取已被人工终止!");
                        break;
                    }
                    if (stmp.Length > 4 && stmp.Substring(0, 4) == "http")
                        getMovieOneInfo(stmp);
                }
            return snext;
        }

        private static void getMovieOneInfo(String page)
        {
            try
            {
                vmb = new MovieMember();
                int it = 0, ik = 0; ;
                String reg = "", stmp = "";
                String[] rs = new string[2];

                it = page.IndexOf("\"");
                if (it != -1)
                {
                    vmb.simageUrl = page.Substring(0, it);
                }

                rs = Apitool.getValueByName(page, "<a title=", "</a>", "href");
                if (rs != null)
                {
                    vmb.sUrl = rs[1];
                    it = vmb.sUrl.IndexOf("http://");
                    if (it != -1)
                    {
                        vmb.sUrl = vmb.sUrl.Substring(it + "http://".Length);
                    }
                    vmb.sTitle = rs[0];
                }
                String szjs = "";//class=\"status\">46集全</span>";<span class="status">更新至10</span>
                rs = Apitool.getValueByName(page, "class=\"status\">", "</span>", "");
                if (rs != null)
                {
                    szjs = rs[0];
                    it = szjs.IndexOf("预告");
                    if (it != -1)
                    {
                        return;
                    }
                    it = szjs.IndexOf("资料");
                    if (it != -1)
                    {
                        return;
                    }
                    it = szjs.IndexOf("集全");
                    if (it != -1)
                    {
                        if (int.TryParse(szjs.Substring(0, it), out ik))
                            vmb.zjs = ik;
                    }
                    it = szjs.IndexOf("更新至");
                    if (it != -1)
                    {
                        if (int.TryParse(szjs.Substring(it + 3), out ik))
                            vmb.jsnew = ik;
                    }
                }

                if (Itvnum != -1)
                {
                    vmb.tvnum = Itvnum;
                }
                else
                {
                    vmb.tvnum = getYoukuTvnum(sUrl);
                }

                reg = "=\"show_intro\">";
                it = page.IndexOf(reg);
                if (it != -1)
                {
                    int itz = page.IndexOf("</li>", it);
                    if (itz != -1)
                    {
                        vmb.sContent = page.Substring(it + reg.Length, itz - it - reg.Length);
                    }
                    page = page.Substring(it);
                }

                reg = ">导演";
                it = page.IndexOf(reg);
                if (it != -1)
                {
                    int itz = page.IndexOf("</a>", it);
                    ik = page.IndexOf(">主演");
                    if (itz != -1 && ik != -1 && itz < ik)
                    {
                        itz = page.IndexOf("\">", it);
                        ik = page.IndexOf("</", itz);
                        if (ik != -1)
                        {
                            vmb.director = page.Substring(itz + 2, ik - itz - 2);
                        }
                    }
                }

                reg = ">主演";
                it = page.IndexOf(reg);
                if (it != -1)
                {
                    ik = page.IndexOf("<li", it);
                    if (ik != -1)
                    {
                        int itz = page.IndexOf("</a>", it);
                        if (itz != -1 && itz < ik)
                        {
                            stmp = page.Substring(it + 3, ik - it - 3);
                            page = page.Substring(ik);
                            reg = "\">[^<]*";
                            Match m = Apitool.GetResultOfReg(stmp, reg);
                            if (m != null)
                            {
                                StringBuilder sb = new StringBuilder();
                                while (m.Success)
                                {
                                    sb.Append(m.Value.Substring(2) + "/");
                                    m = m.NextMatch();
                                }
                                vmb.presenter = sb.ToString();
                            }
                        }
                    }
                }

                String subTitle = "";
                if (vmb.tvnum == Capmovie.ITvNum[4])
                {//综艺节目，>主持人:</span> 
                    //<a href="http://www.youku.com/star_page/uid_UOTEwNzY=.html" target="_blank">韩庚</a>/
                    // <a href="http://www.youku.com/star_page/uid_UMTQxMjc0MA==.html" target="_blank">/
                    // 东海</a>/ <a href="http://www.youku.com/star_page/uid_UMTAxMTMyMA==.html" target="_blank">厉旭</a></li>
                    reg = ">主持";
                    it = page.IndexOf(reg);
                    if (it != -1)
                    {
                        ik = page.IndexOf("<li", it);
                        if (ik != -1)
                        {
                            int itz = page.IndexOf("</a>", it);
                            if (itz != -1 && itz < ik)
                            {
                                stmp = page.Substring(it + 3, ik - it - 3);
                                page = page.Substring(ik);
                                reg = "\">[^<]*";
                                Match m = Apitool.GetResultOfReg(stmp, reg);
                                if (m != null)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    while (m.Success)
                                    {
                                        sb.Append(m.Value.Substring(2) + "/");
                                        m = m.NextMatch();
                                    }
                                    vmb.presenter = sb.ToString();
                                }
                            }
                        }
                    }
                    //综艺节目的副标题用sub特殊录入
                    if (vmb.sUrl != null && vmb.sUrl != "")
                    {

                        String url = vmb.sUrl;
                        if (url.IndexOf("http://") == -1)
                        {
                            url = "http://" + url;
                        }
                        String spa = null;
                        try
                        {
                            spa = Apis.GetPageCode(url, "utf-8");
                            if (spa != null)
                            {
                                // <span class="subtitle" id="subtitle">证券情报站 120312</span>
                                int ily = spa.IndexOf("id=\"subtitle\"");
                                if (ily != -1)
                                {
                                    String sylm = Apitool.FindStrByName(spa.Substring(ily), ">", "<");
                                    if (sylm != null)
                                        subTitle = sylm;
                                }
                            }

                        }
                        catch
                        {

                        }
                    }
                }

                reg = ">地区";
                it = page.IndexOf(reg);
                if (it != -1)
                {
                    ik = page.IndexOf(">分类", it);
                    if (ik != -1)
                    {
                        int itz = page.IndexOf("</a>", it);
                        if (itz != -1 && itz < ik)
                        {
                            page = page.Substring(it);
                            rs = Apitool.getValueByName(page, "<a", "</a>", "");
                            if (rs != null)
                            {
                                vmb.slang = rs[0];
                            }
                        }
                    }
                }
                reg = ">分类";
                it = page.IndexOf(reg);
                if (it != -1)
                {
                    page = page.Substring(it);
                    ik = page.IndexOf("</li");
                    if (ik != -1)
                    {
                        stmp = page.Substring(3, ik - 3);
                        page = page.Substring(ik);
                        reg = "<a[^>]*";
                        stmp = Apitool.GetReplaceOfReg(stmp, reg, "</a").Replace("</a>", " ");
                        reg = "\\w[^\\W]*";
                        Match m = Apitool.GetResultOfReg(stmp, reg);
                        if (m != null)
                        {
                            StringBuilder sb = new StringBuilder();
                            while (m.Success)
                            {
                                sb.Append(m.Value + "/");
                                m = m.NextMatch();
                            }
                            vmb.sType = sb.ToString().Replace("span/", "");
                        }
                    }
                }

                //reg = ">年份";
                if (sYear != "" && int.TryParse(sYear, out ik))
                {
                    vmb.syear = sYear;
                }
                //if (vmb.sTitle.IndexOf("月亮坪的秘密") != -1)
                //{
                //    int aaa = 0;
                //}
                vmb.shtmlString();

                //vmb.tvnum = Capmovie.ITvNum[0];//电影
                //vmb.tvnum = Capmovie.ITvNum[1];//电视剧
                //vmb.tvnum = ITvNum[2];//动漫
                //vmb.tvnum = ITvNum[4];//综艺


                //getLsjfile("www.youku.com/show_page/id_zcc16f73a962411de83b1.html", "16382", "", 1);

                String sid = Capmovie.inserDBAll(vmb);
                if (sid != "" && vmb.sUrl != "")
                {
                    if (Capmovie.isInMovieUrl(vmb.sUrl, sid) == "")
                    {
                        if (vmb.tvnum == Capmovie.ITvNum[4])
                        {
                            vmb.sTitle = subTitle;
                        }
                        Capmovie.insertDBAddress(sid, vmb.sTitle, Capmovie.SMovieType[3], vmb.sUrl, "0", vmb.sPlayTime, vmb.time, vmb.jsnew);
                        if (vmb.tvnum == Capmovie.ITvNum[1] || vmb.tvnum == Capmovie.ITvNum[2])
                        {
                            getLsjfile(vmb.sUrl, sid, "", vmb.tvnum);
                        }
                        wf.addRtb2ForThread(vmb.sTitle + " ID: " + sid + " : " + vmb.sUrl + " 已入库");
                    }
                }
            }
            catch 
            {
            }
        }

        private static void getYoukuZongyi() 
        {
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
            SqlConnection cn = null;
            try
            {
                String strfind =
                    "select id,title,surl from movieurl where movieid in (select id from movie where tvnum = 4 and years = '2012') and videotype = '优酷视频' and id < 349947 order by id desc";
                    //= 38193";
                //
                cn = Apis.GetCon(strdbUrl);//连接数据库
                SqlCommand msc = cn.CreateCommand();
                //这里要判断是否数据已经入库，add code here
                msc.CommandText = strfind;

                dr1 = Apis.searchInDB(msc, strfind);
                while (dr1.Read())
                {
                    String id = dr1["id"].ToString().Trim();
                    String url = dr1["surl"].ToString().Trim();
                    String title = dr1["title"].ToString().Trim();
                    if (url.IndexOf("http://") == -1)
                    {
                        url = "http://" + url;
                    }
                    String spa = null;
                    String subTitle = "";
                    try
                    {
                        spa = Apis.GetPageCode(url, "utf-8");
                        if (spa != null)
                        {
                            // <span class="subtitle" id="subtitle">证券情报站 120312</span>
                            int ily = spa.IndexOf("id=\"subtitle\"");
                            if (ily != -1)
                            {
                                String sylm = Apitool.FindStrByName(spa.Substring(ily), ">", "<");
                                if (sylm != null)
                                    subTitle = sylm;
                            }
                        }

                    }
                    catch(Exception ex)
                    {
                        wf.addRtb2ForThread(title + "\r\n" + ex.ToString());
                    }
                    if (subTitle != "" && title != subTitle)
                    {
                        subTitle = Apitool.filterDBStr(subTitle);
                        String strInser = "update movieurl set title = '" + subTitle + "' where id = " + id;
                        Apis.runSql(strInser, strdbUrl);
                        wf.addRtb2ForThread("update movieurl id = " + id + " subTitle= " + subTitle);
                    }
                }
            }
            catch (Exception ex)
            {
                wf.addRtb2ForThread(ex.ToString());
            }
            finally
            {
                if (dr1 != null)
                {
                    dr1.Dispose();
                    dr1 = null;
                }
                if (cn != null)
                {
                    cn.Dispose();
                    cn = null;
                }
            }
        }

        private static int getYoukuTvnum()
        {
            //Y-2006|N-4|
            String sye = wf.gettextBox2Input();
            if (sye != null && sye != "")
            {
                sye = sye.ToUpper();
                int it = sye.IndexOf("N-");
                if (it != -1)
                {
                    int ik = sye.IndexOf("|", it);
                    if (ik == -1) ik = sye.Length;
                    sye = sye.Substring(it + 2, ik - it - 2);
                    if (int.TryParse(sye, out it))
                        return it;
                }
            }
            return -1;
        }

        /// <summary>
        /// 根据文本框输入获得参数，输入例子//Y-2006|N-4|P-15,无输入返回-1
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static int getParaByInput(String para)
        {
            //Y-2006|N-4|P-15
            String sye = wf.gettextBox2Input();
            if (sye != null && sye != "")
            {
                sye = sye.ToUpper();
                int it = sye.IndexOf(para.ToUpper() + "-");
                if (it != -1)
                {
                    int ik = sye.IndexOf("|", it);
                    if (ik == -1) ik = sye.Length;
                    sye = sye.Substring(it + 2, ik - it - 2);
                    if (int.TryParse(sye, out it))
                        return it;
                }
            }
            return -1;
        }

        //根据路径地址判断tvnum属性
        private static int getYoukuTvnum(String url)
        {
           if (url != "")
            {
                for (int it = 0; it < Stype.Length; it++)
                {
                    if (url.IndexOf(Stype[it]) != -1)
                        return StypeNum[it];
                }
            }
            return 999;
        }

        public static void capYoukudsj()
        {
            SqlConnection cn = null;
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接

            try
            {
                UseStatic.soutTdRtb2("开始抓优酷电视剧分集");

                if (cn == null)
                    cn = Apis.GetCon(Apis.sDBUrl206);
                SqlCommand msc = cn.CreateCommand();

                int[] tvNums = {1,2};
                foreach (int tvnum in tvNums)
                {
                    if (WindForm.isCapyoukudsj == false)
                    {
                        return;
                    }
                    msc.CommandText =
                        "select id,movieid,surl from movieurl where movieid in (select id from movie where years > 2011 and tvnum = " + tvnum +
                        ") and videotype = '优酷视频' and js = 0 order by movieid desc";
                    if (dr1 != null)
                    {
                        dr1.Close();
                    }
                    dr1 = msc.ExecuteReader();
                    while (dr1.Read())
                    {
                        if (WindForm.isCapyoukudsj == false)
                        {
                            return;
                        }
                        String id = dr1["movieid"].ToString().Trim();
                        String url = dr1["surl"].ToString().Trim();
                        String urlid = dr1["id"].ToString().Trim();
                        getLsjfile(url, id, urlid, tvnum);
                    }
                    UseStatic.soutTdRtb2("所有优酷 tvnum= " + tvnum + " 频道节目已抓取!");
                }
                //getDsjfile("http://v.qq.com/cover/v/vwfr2scuby3xbgm.html", "77777");
                UseStatic.soutTdRtb2("所有优酷电视剧已抓取!");
            }
            catch (Exception ex)
            {
                UseStatic.soutTdRtb2(ex);
            }
            finally
            {
                if (WindForm.isCapyoukudsj == false)
                {
                    UseStatic.soutTdRtb2("抓取已经由人工中断!");
                }
                else
                    WindForm.isCapyoukudsj = false;
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
                WindForm.oneCatchEnd("isCapyoukudsj");
            }
        }

        //http://218.85.133.206:81/
        private static void getLsjfile(String url, String id, String urlid, int tvnum)
        {
            if (url == null || url == "") return;
            if (url.IndexOf("http://") == -1)
            {
                url = "http://" + url;
            }
            String page = null;
            try
            {
                page = Apis.GetPageCode(url, "utf-8");
                if (page == null)
                  {
                        return;
                  }
               //>集数:</span>
				//		40集,更新至32 </li>
                int zjs = 0;
                int jsnew = 0;
                if (tvnum == 1 || tvnum == 2)
                {
                    int its = page.IndexOf("集数:<");
                    if (its != -1)
                    {
                        int ite = page.IndexOf("集", its+4);
                        int itl = page.IndexOf(">", its);
                        if (ite != -1)
                        {
                            String sjstmp = page.Substring(itl + 1, ite - itl - 1);
                            if (int.TryParse(sjstmp, out its))
                             {
                                    zjs = its;
                                    String sql = "update movie set zjs = '" + zjs + "' where id = " + id;
                                    Apis.runSql(sql, strdbUrl);
                             }
                            if(itl != -1)
                            {
                                ite = page.IndexOf(">", itl);
                                sjstmp = Apitool.FindStrByName(page.Substring(itl,ite - itl), "更新至", "<");

                                if (sjstmp != null)
                                {
                                    if (int.TryParse(sjstmp, out ite))
                                    {
                                        jsnew = ite;
                                        String sql = "";
                                        if (urlid == "")
                                        {
                                            sql = "update movieurl set jsnew = '" + jsnew + "' where movieid = " + id + " and js = '0' and videotype = '" + Capmovie.SMovieType[3] + "'";
                                        }
                                        else
                                        {
                                            sql = "update movieurl set jsnew = '" + jsnew + "' where id = " + urlid;
                                        }
                                        Apis.runSql(sql, strdbUrl);
                                    }
                                }
                                else
                                {
                                    jsnew = zjs;
                                    String sql = "";
                                    if (urlid == "")
                                    {
                                        sql = "update movieurl set jsnew = '" + jsnew + "' where movieid = " + id + " and js = '0' and videotype = '" + Capmovie.SMovieType[3] + "'";
                                    }
                                    else
                                    {
                                        sql = "update movieurl set jsnew = '" + jsnew + "' where id = " + urlid;
                                    }
                                    Apis.runSql(sql, strdbUrl);
                                }
                            }
                        }
                    }
                }
                getOnePageInfo(page,id,tvnum);

                UseStatic.soutTdRtb2("had got file! id= " + id);
            }
            catch (Exception ex)
            {
                UseStatic.soutTdRtb2(ex);
            }
            finally
            {
            }
        }

        private static void getOnePageInfo(String page,String id,int tvnum) 
        {
            String start = "id=\"vMarker\"";
            String send = "showList -->";
            page = Apitool.FindStrByName(page, start, send);
            if (page == null)
            {
                return;
            }
            String regex = "<li class=\"v_link\"[^!]*";
            Match m = Apitool.GetResultOfReg(page, regex);
            String[] rs2 = null;
            if (m != null)
            {
                while (m.Success)
                {
                    //
                    String surl = "", sname = "", sjs = "9999";  //没有集数就定义为9999
                    String st = m.Value;
                    rs2 = Apitool.getValueByName(st, "href=\"", "\"", "");
                    if (rs2 != null)
                    {
                        surl = rs2[0].Trim().Replace("http://", "");

                    }
                    //title="13 男子汉的战歌" or title="复仇 第一季 10" or "明星的恋人05"
                    rs2 = Apitool.getValueByName(st, "title=\"", "\"", "");
                    if (rs2 != null)
                    {
                        sname = rs2[0].Trim();
                        if (tvnum == Capmovie.ITvNum[1])
                        {
                            int tmp = sname.LastIndexOf(" ");
                            if (tmp != -1)
                            {
                                sjs = sname.Substring(tmp + 1);
                                if (!int.TryParse(sjs, out tmp))
                                {
                                    sjs = "9999";
                                }
                            }
                            else
                            {
                                sjs = getLastNumForStr(sname);
                                if (sjs == "") sjs = "9999";
                            }
                        }
                        else if (tvnum == Capmovie.ITvNum[2])//动漫
                        {
                            int tmp = sname.IndexOf(" ");
                            if (tmp != -1)
                            {
                                sjs = sname.Substring(0,tmp);
                                if (!int.TryParse(sjs, out tmp))
                                {
                                    sjs = "9999";
                                }
                            }
                            else
                            {
                                sjs = getLastNumForStr(sname);
                                if (sjs == "") sjs = "9999";
                            }
                            if (sjs == "9999")
                            {
                                sjs = getFirstNumForStr(sname);
                                if (sjs == "") sjs = "9999";
                            }
                        }
                    }
                    if (Capmovie.isInMovieUrl(surl, id) == "")
                    {
                        Capmovie.insertDBAddress(id, sname, Capmovie.SMovieType[3], surl, sjs, "", "0");
                        wf.addRtb2ForThread(sname + " ID: " + id + " : " + surl + " 已入库");
                    }
                    else
                    {
                        wf.addRtb2ForThread(sname + " No. " + sjs + " ID: " + id + " 已存在");
                    }
                    m = m.NextMatch();
                }
            }
            //有下一页要翻页
            //<li title="下一页" class="next"><a href="/show_eplist/showid_z637a8354787211e0a046_type_pic_from_ajax_page_3.html"
            // onclick="nova_update('eplist',this.href);return false;"><em class="ico_next"/>下一页</a></li>
            rs2 = Apitool.getValueByName(page, "title=\"下一页\"", ">下一页<", "href");
            if (rs2 != null && rs2[1] != "")
            {
                String youkuUrl = StrYoukuAddress + rs2[1];
                if (youkuUrl.IndexOf("http://") == -1)
                        youkuUrl = "http://" + youkuUrl;
                try
                {
                    page = Apis.GetPageCode(youkuUrl, "utf-8");
                    if (page == null)
                    {
                        return;
                    }
                    getOnePageInfo(page, id,tvnum);
                }
                catch 
                {

                }
            }
        }

        //取得形如"ABC47" 这样的字符串的最后一串数字
        private static String getLastNumForStr(String str)
        {
            int tmp = -1;
            str = str.Replace("大结局", "").Replace("全", "").Replace("完", "").Replace("集", "").Replace("话","");
            for (int tt = str.Length - 1; tt > -1; tt--)
            {
                if (!int.TryParse(str[tt] + "", out tmp))
                {
                    if (tt == str.Length - 1) return "";
                    return str.Substring(tt + 1);
                }
            }
            if (int.TryParse(str, out tmp)) return str;
            return "";
        }

        //取得形如"第一话" 这样的字符串的第一串数字
        private static String getFirstNumForStr(String str)
        {
            int tmp = -1;
            str = str.Replace("大结局", "").Replace("全", "").Replace("完", "").Replace("集", "").Replace("话", "").Replace("第","");
            for (int tt = 0; tt < str.Length; tt++)
            {
                if (!int.TryParse(str[tt] + "", out tmp))
                {
                    if (tt == 0) return "";
                    return str.Substring(0,tt);
                }
            }
            if (int.TryParse(str, out tmp)) return str;
            return "";
        }

    }
}
