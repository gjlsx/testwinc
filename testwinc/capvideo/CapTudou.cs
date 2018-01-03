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
    class CapTudou
    {
        //定义变量
        private static WindForm wf = null;
        private static MovieMember vmb = null;
        private static String sUrl = "";
        private static String sYear = "";
        private static readonly String start = "class=\"pic\"";//数据页面开始
        private static readonly String strend = "下一页";//下一页
        private static int Itvnum = -1;//tvnum = 0 -电影， 1-电视剧 2-动漫 3-央视名栏目,4-综艺,5-音乐mtv,999-其他
        //年份计数数组
        private static readonly String[] years = new String[] { 
                   "2012"
                   //,"2011"
                    //, "2010", "2009", "2008",
                    //"2007", "2006", "2005", "2004", "2003", "2002", "2001", "2000", "1999"
        };
        //抓取节目类型
        private static readonly int[] StypeNum = new int[] { 
                   Capmovie.ITvNum[1],Capmovie.ITvNum[0], Capmovie.ITvNum[2] //, Capmovie.ITvNum[4]
        };
        //调用静态工具类
        static CapTudou()
        {
            if (wf == null)
            {
                wf = UseStatic.getWindForm();
            }
        }
        //抓取土豆视频
        public static void getTudouVideo()
        {
            try
            {
                sUrl = wf.getTbxAddress().ToLower();
                Boolean isCapByBrowserweb = true;
                //如果浏览器栏是土豆页面，就从浏览器栏的页面抓取
                if (sUrl.IndexOf("tudou.com") != -1)
                {
                    wf.addRtb2ForThread("开始抓 " + sUrl + " " + " 数据");
                }
                else
                {
                    sUrl = "";
                    isCapByBrowserweb = false;
                    wf.addRtb2ForThread("开始抓土豆电影，电视剧，动画数据.   http://www.tudou.com/    \r\n");
                }
                String su1 = "";
                //
                foreach (int tnum in StypeNum)
                {
                    if (WindForm.isCapTudou == false)
                    {
                        break;
                    }
                    String page = "";
                    if (!isCapByBrowserweb)
                    {
                        if (tnum == Capmovie.ITvNum[0])
                        {
                            su1 = "http://movie.tudou.com/albumtop/c22t-1v-1z-1a-1y";//土豆电影
                            Itvnum = 0;//记录类型为电影
                            //取出所需要抓取的年份
                            foreach (String sye in years)
                            {
                                if (sYear == "") sYear = sye;
                                if (WindForm.isCapTudou == false)
                                {
                                    break;
                                }
                                if (!isCapByBrowserweb)
                                {
                                    sUrl = su1 + sye + "h-1s1p1.html";
                                    page = Apis.GetPageCode(sUrl, "GBK");
                                }
                            }
                            String nextUrl = getNextUrl(page);
                            while (nextUrl != null && nextUrl != "" && nextUrl != sUrl + "\r\n")
                            {
                                page = Apis.GetPageCode(nextUrl, "GBK");
                                nextUrl = getNextUrl(page);
                                if (WindForm.isCapTudou == false)
                                {
                                    break;
                                }
                            }

                        }
                        else if (tnum == Capmovie.ITvNum[1])//土豆电视剧
                        {

                            su1 = "http://tv.tudou.com/albumtop/c30t-1v-1z-1a-1y";
                            Itvnum = 1;
                            foreach (String sye in years)
                            {
                                if (sYear == "") sYear = sye;
                                if (WindForm.isCapTudou == false)
                                {
                                    break;
                                }
                                if (!isCapByBrowserweb)
                                {
                                    sUrl = su1 + sye + "h-1s1p1.html";
                                    page = Apis.GetPageCode(sUrl, "GBK");
                                }
                            }
                            String nextDsjUrl = getNextUrl(page);
                            while (nextDsjUrl != null && nextDsjUrl != "" && nextDsjUrl != sUrl + "\r\n")
                            {
                                page = Apis.GetPageCode(nextDsjUrl, "GBK");
                                nextDsjUrl = getNextUrl(page);
                                if (WindForm.isCapTudou == false)
                                {
                                    break;
                                }
                            }

                        }
                        else if (tnum == Capmovie.ITvNum[2])//土豆动漫
                        {
                            su1 = "http://cartoon.tudou.com/albumtop/c9t-1v-1z-1a-1y";
                            Itvnum = 2;
                            foreach (String sye in years)
                            {
                                if (sYear == "") sYear = sye;
                                if (WindForm.isCapTudou == false)
                                {
                                    break;
                                }
                                if (!isCapByBrowserweb)
                                {
                                    sUrl = su1 + sye + "h-1s1p1.html";
                                    page = Apis.GetPageCode(sUrl, "GBK");
                                }
                            }
                            String nextCtUrl = getNextUrl(page);
                            while (nextCtUrl != null && nextCtUrl != "" && nextCtUrl != sUrl + "\r\n")
                            {
                                page = Apis.GetPageCode(nextCtUrl, "GBK");
                                nextCtUrl = getNextUrl(page);
                                if (WindForm.isCapTudou == false)
                                {
                                    break;
                                }
                            }
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

                    if (page == null)
                    {
                        wf.addRtb2ForThread(" 网址打不开!");
                        continue;
                    }
                    sUrl = "";
                    sYear = "";
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
                if (WindForm.isCapTudou == false)
                {
                    UseStatic.soutTdRtb2("抓取已经由人工中断!");
                }
                else
                    WindForm.isCapTudou = false;
                wf.Invoke((MethodInvoker)delegate { wf.setbtnTudou(true); });
                WindForm.oneCatchEnd("isCapTudou");
            }
        }
        //获取下一页URL
        private static String getNextUrl(String page)
        {
            int istart = page.IndexOf(start);
            if (istart == -1) return "";
            int iprev = page.IndexOf("page_nav_next", istart);

            int inext = page.IndexOf(strend, istart);
            if (inext == -1) return "";
            String strpage = "";
            if (iprev != -1)
            {
                //有下一页
                strpage = page.Substring(iprev, inext + 27 - iprev);
                page = page.Substring(istart, iprev - istart);
            }
            else
                page = page.Substring(istart, inext - istart);

            String[] strsplit = { "class=\"pic\"" };
            String[] pages = page.Split(strsplit, StringSplitOptions.RemoveEmptyEntries);

            foreach (String stmp in pages)
            {
                if (WindForm.isCapTudou == false)
                {
                    break;
                }
                getMovieInfo(stmp);
            }
            if (strpage == "") return "";
            int itmp = strpage.IndexOf(">下一页<");
            if (itmp == -1)
            {
                return "";
            }
            else
            {  //将下一页的URL返回
                String sNextUrl = Regex.Match(strpage, "href=\"[^>]*", RegexOptions.IgnoreCase).Value.Replace("href=", "").Replace("\"", "");
                return sNextUrl;
            }
        }

        //遇错误调试请将getMovieInfo换为调试getMovieInfo并添加参数
        /*  private static void getMovieInfo(String page)
        {
            getDsjfile("http://www.tudou.com/albumcover/qjxnLznoG1Q.html", "38871", "", 1);
        }*/
        //数据处理
        private static void getMovieInfo(String page)
        {//insertDB
            vmb = new MovieMember();
            String[] rs = new string[2];
            String stemp = Regex.Replace(page, "<a[^>]*", "").ToString().Replace("</a>", "").Replace(">", "");
            //导演入列
            rs = Apitool.getValueByName(stemp, "ext_cast\"", "<", "");
            vmb.director = rs[0].Replace("导演:", "");
            //集数入列
            String MoviePart = stemp.Replace("</span", "");
            rs = Apitool.getValueByName(MoviePart, "vpbg\"", "<", "");
            if (rs != null)
            {
                MoviePart = Regex.Match(rs[0], "[0-9]", RegexOptions.IgnoreCase).Value;
                vmb.jsnew = int.Parse(MoviePart);
            }
            else
            {
                vmb.jsnew = 0;
            }
            //演员入列
            rs = Apitool.getValueByName(stemp, "cast\"", "<", "");
            if (rs != null)
            {
                vmb.presenter = rs[0];
            }
            //简介入列
            rs = Apitool.getValueByName(stemp, "desc\"", "<", "");
            if (rs != null)
            {
                vmb.sContent = rs[0];
            }
            //图片URL入列
            rs = Apitool.getValueByName(stemp, "src=\"", "\"", "");
            vmb.simageUrl = rs[0];
            //地区入列
            rs = Apitool.getValueByName(stemp, "ext_area\"", "<", "");
            vmb.slang = rs[0].Replace("地区:", "");
            //片名入列
            rs = Apitool.getValueByName(stemp, "caption\"", "<", "");
            vmb.sTitle = rs[0];
            //影片类型入列
            rs = Apitool.getValueByName(stemp, "ext_type\"", "<", "");
            vmb.sType = rs[0].Replace("类型:", "");
            //影片URL入列
            rs = Apitool.getValueByName(page, "href=\"", "\"", "");
            vmb.sUrl = rs[0];
            if (Itvnum == 0)
            {
                stemp = Apis.GetPageCode(rs[0], "GBK");
                stemp = Apitool.FindStrByName(stemp, "album-btn", "album-play");
                if (stemp != null)
                {
                    vmb.sUrl = Regex.Match(stemp, "href=\"[^\"]*", RegexOptions.IgnoreCase).Value.Replace("href=\"", "");
                }
                else
                {
                    vmb.sUrl = "";
                }
            }
            //年份入列
            vmb.syear = sYear;
            vmb.shtmlString();
            //数据类型入列
            vmb.tvnum = Itvnum;

            String sid = Capmovie.inserDBAll(vmb);
            if (sid != "" && vmb.sUrl != "")
            {
                if (Capmovie.isInMovieUrl(vmb.sUrl, sid) == "")
                {
                    Capmovie.insertDBAddress(sid, vmb.sTitle, Capmovie.SMovieType[6], vmb.sUrl, "0", vmb.sPlayTime, vmb.time, vmb.jsnew);
                    if (vmb.tvnum == Capmovie.ITvNum[1] || vmb.tvnum == Capmovie.ITvNum[2])//连续剧要分开抓
                    {
                        //错误test数据ID = 38871，http://www.tudou.com/albumcover/qjxnLznoG1Q.html 
                        getDsjfile(vmb.sUrl, sid, "", vmb.tvnum);
                    }
                    wf.addRtb2ForThread(vmb.sTitle + " ID: " + sid + " : " + vmb.sUrl + " 已入库");
                }
            }
        }

        //抓分集
        private static void getDsjfile(String url, String id, String urlid, int tvnum)
        {
            if (url == null || url == "") return;
            if (url.IndexOf("http://") == -1)
            {
                url = "http://" + url;
            }
            String page = null;
            try
            {
                page = Apis.GetPageCode(url, "GBK");
                if (page == null || page == "") return;
                else
                {
                    page = Apitool.FindStrByName(page, "pack_video_card", "page_nav\"");
                    if (page != null)
                    {
                        String[] sPlit = { "pack_video_card" };
                        string[] part = page.Split(sPlit, StringSplitOptions.RemoveEmptyEntries);
                        foreach (String st in part)
                        {
                            String surl = "", sname = "", sjs = "9999";  //没有集数就定义为9999
                            sname = Apitool.FindStrByName(st, "title=\"", "\"");
                            surl = Apitool.FindStrByName(st, "href=\"", "\"");
                            
                            String tmp = Apitool.getLastNumForStr(sname);
                            if (tmp != "")
                            {
                                sjs = tmp;
                            }
                            else
                            {
                                sjs = "9999";
                            }
                            if (surl != "")
                            {
                                if (Capmovie.isInMovieUrl(surl, id) == "")
                                {
                                    Capmovie.insertDBAddress(id, sname, Capmovie.SMovieType[6], surl, sjs, "", "0");
                                    wf.addRtb2ForThread(sname + " ID: " + id + " : " + surl + " 已入库");
                                }
                                else
                                {
                                    wf.addRtb2ForThread(sname + " No. " + sjs + " ID: " + id + " 已存在");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UseStatic.soutTdRtb2(ex);
            }
            finally
            {
            }
        }
        public static void CapTudoudsj()
        {
            SqlConnection cn = null;
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接

            try
            {
                UseStatic.soutTdRtb2("开始抓土豆电视剧动画片");

                if (cn == null)
                    cn = Apis.GetCon(Apis.sDBUrl206);
                SqlCommand msc = cn.CreateCommand();

                int[] tvNums = { 1, 2 };
                foreach (int tvnum in tvNums)
                {
                    if (WindForm.isCapTudoudsj == false)
                    {
                        return;
                    }
                    msc.CommandText =
                        "select id,movieid,surl from movieurl where movieid in (select id from movie where years > 2011 and tvnum = " + tvnum +
                        ") and videotype = '土豆视频' and js = 0 order by movieid desc";
                    if (dr1 != null)
                    {
                        dr1.Close();
                    }
                    dr1 = msc.ExecuteReader();
                    while (dr1.Read())
                    {
                        if (WindForm.isCapTudoudsj == false)
                        {
                            return;
                        }
                        String id = dr1["movieid"].ToString().Trim();
                        String url = dr1["surl"].ToString().Trim();
                        String urlid = dr1["id"].ToString().Trim();
                        getDsjfile(url, id, urlid, tvnum);
                    }
                    UseStatic.soutTdRtb2("所有土豆 tvnum= " + tvnum + " 频道节目已抓取!");
                }
                UseStatic.soutTdRtb2("所有土豆视剧已抓取!");
            }
            catch (Exception ex)
            {
                UseStatic.soutTdRtb2(ex);
            }
            finally
            {
                if (WindForm.isCapTudoudsj == false)
                {
                    UseStatic.soutTdRtb2("抓取已经由人工中断!");
                }
                else
                    WindForm.isCapTudoudsj = false;
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
                WindForm.oneCatchEnd("isCapTudoudsj");
            }
        }
    }
}
