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
    class Capletv
    {
        private static WindForm wf = null;
        private static MovieMember vmb = null;
        private static readonly String start = "=\"info2_box\"";//数据页面开始
       // private static readonly String start2 = "class=\"items\">";////
        //  private static readonly String strpage = ">上一页<";
        private static readonly String strend = ">下一页<";// ">下一页</a>";
        private static String sUrl = ""; 
        private static String sYear = "";
        private static int Itvnum = -1;
        private static readonly String StrLetvAddress = "http://www.letv.com";


        private static readonly String[] years = new String[] { 
                   "2012"
                   //, "2011"
                  // , "2010", "2009", "2008",
                  //  "2007", "2006", "2005", "2004", "2003", "2002"
    };

        private static readonly int[] StypeNum = new int[] { 
                   Capmovie.ITvNum[1],Capmovie.ITvNum[0], Capmovie.ITvNum[2] //, Capmovie.ITvNum[4]
        };


        static Capletv()
        {
            if (wf == null)
            {
                wf = UseStatic.getWindForm();
            }
        }
        
        //抓视频数据
        public static void getLetvVideo() 
        {
            try
            {
                sUrl = wf.getTbxAddress().ToLower();
                Boolean isCapByBrowserweb = true;
                //如果浏览器栏是letv页面，就从浏览器栏的页面抓取
                if (sUrl.IndexOf("letv.com") != -1)
                {
                    wf.addRtb2ForThread("开始抓 " + sUrl + " " + " 数据");
                }
                else
                {
                    sUrl = "";
                    isCapByBrowserweb = false;
                    wf.addRtb2ForThread("开始抓乐视网电影，电视剧，动画 数据 ============================== \r\n");
                }
                //  http://so.letv.com/list/c1_t-1_a-1_y2011_f_at_o1_p.html
                Itvnum = CapYouku.getParaByInput("N");   //供手动修改type和页数
                int itype = CapYouku.getParaByInput("y");
                if (itype != -1) sYear = itype + "";  //年份
                else sYear = "";

                if (isCapByBrowserweb) 
                {
                    if (Itvnum == -1 || sYear == "")
                    {
                        wf.addRtb2ForThread("请输入tvnum! year, example:  y-2012|n-0");
                        return;
                    }
                }

                String su1 = "http://so.letv.com/list/c1_t-1_a-1_y";
                String su2 = "_f_at_o1_p.html";

                foreach (int tnum in StypeNum)
                {
                    if (WindForm.isCapletv == false)
                    {
                        break;
                    }
                    if (!isCapByBrowserweb)
                    {
                        if (Itvnum != -1 && Itvnum != tnum)
                        {
                            continue;
                        }
                        else if (tnum == Capmovie.ITvNum[2])//优酷动画
                        {
                            su1 = "http://so.letv.com/list/c3_t-1_a-1_y";
                            su2 = "_f-1_at-1_o1_p.html";
                        }
                        //else if (tnum == Capmovie.ITvNum[4])
                        //{//http://zy.youku.com/search?ccat40640[r]=2010&m40641[cc-ms-q]=a|releaseyear%3A2010 //综艺
                        //    su1 = "http://zy.youku.com/search?ccat40640[r]=";
                        //    su2 = "&m40641[cc-ms-q]=a|releaseyear%3A";
                        //}
                        else if (tnum == Capmovie.ITvNum[0])
                        {//http://so.letv.com/list/c1_t-1_a-1_y2010_f_at_o1_p.html //电影
                            su1 = "http://so.letv.com/list/c1_t-1_a-1_y";
                            su2 = "_f_at_o1_p.html";
                        }
                        else if (tnum == Capmovie.ITvNum[1])//！电视剧节目手动输入网址抓
                        {
                            su1 = "http://so.letv.com/list/c2_t-1_a-1_y";
                            su2 = "_f_at_o1_p.html";  
                        }
                        //Itvnum = tnum;
                    }
                    foreach (String sye in years)
                    {
                        if (WindForm.isCapletv == false)
                        {
                            break;
                        }
                        String page = "";
                        if (!isCapByBrowserweb)
                        {
                            //sUrl = su1 + stp + su2 + sye + su3;
                            sUrl = su1 + sye + su2;
                        }
                        page = Apis.GetPageCode(sUrl, "utf-8");
                        if (page == null)
                        {
                            wf.addRtb2ForThread(" 网址打不开!");
                            continue;
                        }
                        if (sYear == "") sYear = sye;

                        String snext = getOneInfo(page, tnum);

                        while (snext != null && snext != "") 
                        {
                            if(snext.IndexOf("http://") == -1)
                                snext = StrLetvAddress + snext;
                            page = Apis.GetPageCode(snext, "utf-8");

                            snext = getOneInfo(page, tnum);
                            if (WindForm.isCapletv == false)
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
                if (WindForm.isCapletv == false)
                {
                    UseStatic.soutTdRtb2("抓取已经由人工中断!");
                }
                else
                    WindForm.isCapletv = false;
                //if (dr1 != null)
                //{
                //    dr1.Close();
                //    dr1.Dispose();
                //    dr1 = null;
                //}
                //if (cn != null)
                //{
                //    cn.Close();
                //    cn.Dispose();
                //    cn = null;
                //}
                wf.Invoke((MethodInvoker)delegate { wf.setbtnLetv(true); });
                WindForm.oneCatchEnd("isCapletv");
            }
        }

        private static String getOneInfo(String page,int tvnum)
        {
            int istart = page.IndexOf(start);
            if (istart == -1) return "";
            int iprev = page.IndexOf(">上一页<", istart);
            if (iprev == -1) return "";
            int inext = page.IndexOf(strend,iprev);
            if (inext == -1) return "";
            String strpage = page.Substring(iprev, inext+27 - iprev);

            page = page.Substring(istart, iprev-istart);
            String[] strsplit = { start };
            String[] pages = page.Split(strsplit, StringSplitOptions.RemoveEmptyEntries);

            foreach (String stmp in pages)
            {
                if (WindForm.isCapletv == false)
                {
                    break;
                }
                getMovieOneInfo(stmp,tvnum);
            }
            //有下一页要翻页
            //<a href="http://so.letv.com/list/c1_t-1_a-1_y2011_f-1_at1_o1_p14.html">14</a>
            //<a href="http://so.letv.com/list/c1_t-1_a-1_y2011_f-1_at1_o1_p2.html">下一页</a>
            //或<a class="on" href="#">3</a><span>下一页</span></div>
            int itmp = strpage.IndexOf(">下一页</a>");
            if (itmp == -1) return "";

            inext = strpage.LastIndexOf("href=\"");
            int ilenh = "href=\"".Length;
            if (inext != -1)
            {
                iprev = strpage.IndexOf("\"", inext + ilenh);
                if (iprev != -1)
                    return strpage.Substring(inext + ilenh, iprev - inext - ilenh);
            }
            return "";
        }

        private static void getMovieOneInfo(String page,int tvnum) 
        {
       
                vmb = new MovieMember();
                int it = 0, ik = 0, ix;
                String reg = "", stmp = "";
                String[] rs = new string[2];

                rs = Apitool.getValueByName(page, "<img", "/>", "src", out ik);
                if (rs != null)
                {
                    vmb.simageUrl = rs[1];
                    it = vmb.simageUrl.IndexOf("http://");
                    if (it != -1)
                    {
                        vmb.sUrl = vmb.simageUrl.Substring(it + "http://".Length);
                    }
                    vmb.sTitle = rs[0];
                }

                if (ik == -1) return;
                page = page.Substring(ik);
                rs = Apitool.getValueByName(page, "<a", "</a>", "href", out it);
                if (rs != null)
                {
                    vmb.sUrl = rs[1];
                    ik = vmb.sUrl.IndexOf("http://");
                    if (it != -1)
                    {
                        vmb.sUrl = vmb.sUrl.Substring(ik + "http://".Length);
                    }
                    vmb.sTitle = rs[0];
                }

                reg = ">导演";
                it = page.IndexOf(reg);
                if (it != -1)
                {
                    page = page.Substring(it + 3);
                    rs = Apitool.getValueByName(page, ">", "<", "", out it);
                    if (rs != null)
                    {
                        if (rs[0] != "")
                            vmb.director = rs[0];
                    }
                }

                reg = ">年份";
                ik = page.IndexOf(reg);
                if (ik != -1)
                {
                    rs = Apitool.getValueByName(page.Substring(ik + 3), ">", "<", "");
                    if (rs != null)
                    {
                            if (int.TryParse(rs[0], out ix))
                                vmb.syear = rs[0];
                    }
                }

                reg = ">主演";
                it = page.IndexOf(reg);
                if (it != -1)
                {
                    if (it < ik)
                    {
                        stmp = page.Substring(it, ik - it);
                        page = page.Substring(ik);
                        StringBuilder sb = new StringBuilder();
                        do
                        {
                            rs = Apitool.getValueByName(stmp, "<a", "</a>", "", out it);
                            if (rs != null)
                            {
                                if (rs[0] != null && rs[0].Trim() != "")
                                    sb.Append(rs[0] + "/");

                            }
                            if (it != -1) stmp = stmp.Substring(it + 3);
                            else
                                break;

                        } while (rs != null);
                        vmb.presenter = sb.ToString();
                    }
                }

                reg = "=\"ind24\">";
                rs = Apitool.getValueByName(page, reg, "<a", "");
                if (rs != null)
                {
                    vmb.sContent = rs[0].Trim();
                }

                //reg = ">年份";//如果手动输入就强制更新年份
                if (sYear != "" && int.TryParse(sYear, out ik))
                {
                    vmb.syear = sYear;
                }

                //if (vmb.sTitle.IndexOf("月亮坪的秘密") != -1)
                //{
                //    int aaa = 0;
                //}
                vmb.shtmlString();

                vmb.tvnum = tvnum;

                String sid = Capmovie.inserDBAll(vmb);
                if (sid != "" && vmb.sUrl != "")
                {
                    if (Capmovie.isInMovieUrl(vmb.sUrl, sid) == "")
                    {
                        Capmovie.insertDBAddress(sid, vmb.sTitle, Capmovie.SMovieType[4], vmb.sUrl, "0", vmb.sPlayTime, vmb.time, vmb.jsnew);
                        if (vmb.tvnum == Capmovie.ITvNum[1] || vmb.tvnum == Capmovie.ITvNum[2])//连续剧要分开抓
                        {
                            getDsjfile(vmb.sUrl, sid, "", vmb.tvnum);
                        }
                        wf.addRtb2ForThread(vmb.sTitle + " ID: " + sid + " : " + vmb.sUrl + " 已入库");
                    }
                   
                }
        }//end getMovieOneInfo

          //http://218.85.133.206:81/
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
                //url = "http://so.letv.com/comic/53666.html";
                page = Apis.GetPageCode(url, "utf-8");
                String start = "<title>";
                String send = "/title";
                String page2 = page;
                page = Apitool.FindStrByName(page, start, send);
                if (page == null)
                {
                    return;
                }
                //<title>花篮的花儿香-花篮的花儿香全集（更新至25集_共25集）-乐视网</title>
                int zjs = 0;
                int jsnew = 0;
                Boolean isAll = false;
                if (tvnum == 1)
                {
                    //（更新至25集_共25集）
                    page = Apitool.FindStrByName(page, "（", "）");
                    if (page != null)
                    {
                        page = page.Replace("至", "").Replace("到", "");

                        String snew = Apitool.FindStrByName(page, "更新", "集");
                        String sall = Apitool.FindStrByName(page, "共", "集");
                        int ite = 0;
                        if (sall != null)
                        {
                            if (int.TryParse(sall, out ite))
                            {
                                zjs = ite;
                                String sql = "update movie set zjs = '" + zjs + "' where id = " + id;
                                Apis.runSql(sql, Capmovie.strdbUrl);
                            }
                        }

                        if (snew != null)
                        {

                                    if (int.TryParse(snew, out ite))
                                    {
                                        jsnew = ite;
                                        if (jsnew != 0)
                                        {
                                            String sql = "";
                                            if (urlid == "")
                                            {
                                                sql = "update movieurl set jsnew = '" + jsnew + "' where movieid = " + id + " and js = '0' and videotype = '" + Capmovie.SMovieType[4] + "'";
                                            }
                                            else
                                            {
                                                sql = "update movieurl set jsnew = '" + jsnew + "' where id = " + urlid;
                                            }
                                            Apis.runSql(sql, Capmovie.strdbUrl);
                                        }
                                    }
                        }
                    }
                }
                else if (tvnum == 2)
                {
                    //<small>共40集全</small>  //<small>   共16集 更新至16集</small>
                    int its = page.IndexOf("<small>");
                    if (its != -1)
                    {
                        int ite = page.IndexOf("集全</", its);
                        if (ite == -1)
                            ite = page.IndexOf("集</", its);
                        else
                        {
                            isAll = true;
                        }
                        if (ite != -1)
                        {
                            String sjstmp = page.Substring(its + "<small>".Length, ite - its - "<small>".Length).Trim();
                            if (sjstmp.Substring(0, 1) == "共")
                            {
                                sjstmp = sjstmp.Substring(1);
                                its = sjstmp.IndexOf("集");
                                if (isAll) its = sjstmp.Length;
                                if (its != -1)
                                {
                                    if (int.TryParse(sjstmp.Substring(0, its), out ite))
                                    {
                                        zjs = ite;
                                        String sql = "update movie set zjs = '" + zjs + "' where id = " + id;
                                        Apis.runSql(sql, Capmovie.strdbUrl);

                                        if (isAll) { jsnew = zjs; }
                                        else
                                        {
                                            its = sjstmp.IndexOf("更新至");
                                            if (its != -1)
                                            {
                                                if (int.TryParse(sjstmp.Substring(its + "更新至".Length), out ite))
                                                {
                                                    jsnew = ite;

                                                }
                                            }
                                        }
                                        if (jsnew != 0)
                                        {
                                            sql = "";
                                            if (urlid == "")
                                            {
                                                sql = "update movieurl set jsnew = '" + jsnew + "' where movieid = " + id + " and js = '0' and videotype = '" + Capmovie.SMovieType[4] + "'";
                                            }
                                            else
                                            {
                                                sql = "update movieurl set jsnew = '" + jsnew + "' where id = " + urlid;
                                            }
                                            Apis.runSql(sql, Capmovie.strdbUrl);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                start = "class=\"w120\"";
                send = "div class=\"tab\"";
                page = Apitool.FindStrByName(page2, start, send);
                if (page == null) return;

                String regex = "href=[^<]*";
                Match m = Apitool.GetResultOfReg(page, regex);
                if (m != null)
                {
                    while (m.Success)
                    {
                       
                        String surl = "", sname = "", sjs = "9999";  //没有集数就定义为9999
                        String st = m.Value;

                        //href="http://www.letv.com/ptv/pplay/47395/2.html"title="超时空甩尾 DEMO">第2集
                        String[]  rs2 = Apitool.getValueByName(st, ">", "", "");
                        if (rs2 != null)
                        {
                            sname = rs2[0].Trim();
                            rs2 = Apitool.getValueByName(st, "href=\"", "\"", "");
                            if (rs2 != null)
                            {
                                surl = rs2[0].Trim();
                            }

                           String tmp = getLastNumForStr(sname);
                           if (tmp != "")
                           {
                               sjs = tmp;
                           }
                           else
                           {
                               sjs = "9999";
                           }
                            if (Capmovie.isInMovieUrl(surl, id) == "")
                            {
                                Capmovie.insertDBAddress(id, sname, Capmovie.SMovieType[4], surl, sjs, "", "0");
                                wf.addRtb2ForThread(sname + " ID: " + id + " : " + surl + " 已入库");
                            }
                            else
                            {
                                wf.addRtb2ForThread(sname + " No. " + sjs + " ID: " + id + " 已存在");
                            }
                        }
                        m = m.NextMatch();
                    }
                }

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

        //取得形如"ABC47" 这样的字符串的最后一串数字
        private static String getLastNumForStr(String str) 
        {
            int tmp = -1;
            str = str.Replace("大结局","").Replace("全","").Replace("完","").Replace("集","");
            for (int tt = str.Length - 1; tt > -1; tt--)
            {
                if (!int.TryParse(str[tt]+"", out tmp))
                {
                    if (tt == str.Length - 1) return "";
                    return str.Substring(tt + 1);
                }
            }
            if (int.TryParse(str, out tmp)) return str;
            return "";
        }


        public static void capLesidsj()
        {
            SqlConnection cn = null;
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接

            try
            {
                UseStatic.soutTdRtb2("开始抓乐视网电视剧动画片");

                if (cn == null)
                    cn = Apis.GetCon(Apis.sDBUrl206);
                SqlCommand msc = cn.CreateCommand();

                int[] tvNums = {  1,2 };
                foreach (int tvnum in tvNums)
                {
                    if (WindForm.isCapLesiudsj == false)
                    {
                        return;
                    }
                    msc.CommandText =
                        "select id,movieid,surl from movieurl where movieid in (select id from movie where years > 2011 and tvnum = " + tvnum +
                        ") and videotype = 'LETV' and js = 0  order by movieid desc";
                    if (dr1 != null)
                    {
                        dr1.Close();
                    }
                    dr1 = msc.ExecuteReader();
                    while (dr1.Read())
                    {
                        if (WindForm.isCapLesiudsj == false)
                        {
                            return;
                        }
                        String id = dr1["movieid"].ToString().Trim();
                        String url = dr1["surl"].ToString().Trim();
                        String urlid = dr1["id"].ToString().Trim();
                        getDsjfile(url, id, urlid, tvnum);
                    }
                    UseStatic.soutTdRtb2("所有乐视 tvnum= " + tvnum + " 频道节目已抓取!");
                }
                //getDsjfile("http://v.qq.com/cover/v/vwfr2scuby3xbgm.html", "77777");
                UseStatic.soutTdRtb2("所有乐视电视剧已抓取!");
            }
            catch (Exception ex)
            {
                UseStatic.soutTdRtb2(ex);
            }
            finally
            {
                if (WindForm.isCapLesiudsj == false)
                {
                    UseStatic.soutTdRtb2("抓取已经由人工中断!");
                }
                else
                    WindForm.isCapLesiudsj = false;
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
                WindForm.oneCatchEnd("isCapLesiudsj");
            }
        }

    }//end capletv
}



