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
    class CapXunlei
    {
        private static WindForm wf = null;
        private static MovieMember vmb = null;
        private static readonly String start = "class=\"movielist\"";//数据页面开始
        // private static readonly String start2 = "class=\"items\">";
        //private static readonly String strpage = ">上一页<";
        private static readonly String strend = "--main  END--";// ">下一页</a>";
        private static String sUrl = "";
        private static String sYear = "";
        private static int Itvnum = -1;
        private static readonly String StrAddress = "http://movie.xunlei.com/";


        private static readonly String[] years = new String[] { 
                   "2012"
                   //,"2011"
                    //, "2010", "2009", "2008",
                    //"2007", "2006", "2005", "2004", "2003", "2002", "2001", "2000", "1999"
        };

        private static readonly int[] StypeNum = new int[] { 
                   Capmovie.ITvNum[1],Capmovie.ITvNum[0], Capmovie.ITvNum[2] //, Capmovie.ITvNum[4]
        };


        static CapXunlei()
        {
            if (wf == null)
            {
                wf = UseStatic.getWindForm();
            }
        }

        //抓视频数据
        public static void getVideo()
        {
            try
            {
                sUrl = wf.getTbxAddress().ToLower();
                Boolean isCapByBrowserweb = true;
                //如果浏览器栏是letv页面，就从浏览器栏的页面抓取
                if (sUrl.IndexOf("xunlei.com") != -1)
                {
                    wf.addRtb2ForThread("开始抓 " + sUrl + " " + " 数据");
                }
                else
                {
                    sUrl = "";
                    isCapByBrowserweb = false;
                    wf.addRtb2ForThread("开始抓迅雷电影，电视剧，动画数据.   http://movie.xunlei.com/type/movie/     \r\n");
                }
                // http://movie.xunlei.com/type,year/movie,2010/
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

                String su1 = "http://movie.xunlei.com/type,year/movie,";

                foreach (int tnum in StypeNum)
                {
                    if (WindForm.isCapxunlei == false)
                    {
                        break;
                    }
                    if (!isCapByBrowserweb)
                    {
                        
                        //else if (tnum == Capmovie.ITvNum[2])//优酷动画
                        //{
                        //    su1 = "http://so.letv.com/list/c3_t-1_a-1_y";
                        //    su2 = "_f-1_at-1_o1_p.html";
                        //}
                        //else if (tnum == Capmovie.ITvNum[4])
                        //{//http://zy.youku.com/search?ccat40640[r]=2010&m40641[cc-ms-q]=a|releaseyear%3A2010 //综艺
                        //    su1 = "http://zy.youku.com/search?ccat40640[r]=";
                        //    su2 = "&m40641[cc-ms-q]=a|releaseyear%3A";
                        //}
                        if (tnum == Capmovie.ITvNum[0])
                        {//http://movie.xunlei.com/type,year/movie,2009/
                            su1 = "http://movie.xunlei.com/type,year,status/movie,";
                        }
                        else if (tnum == Capmovie.ITvNum[1])//！电视剧节目手动输入网址抓
                        {
                            su1 = "http://movie.xunlei.com/type,year/teleplay,";
                        }
                        else if (tnum == Capmovie.ITvNum[2])//！电视剧节目手动输入网址抓
                        {
                            su1 = "http://movie.xunlei.com/type,year/anime,";
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
                        if (WindForm.isCapxunlei == false)
                        {
                            break;
                        }
                        String page = "";
                        if (!isCapByBrowserweb)
                        {
                            if (tnum == Capmovie.ITvNum[1] || tnum == Capmovie.ITvNum[2])
                                sUrl = su1 + sye + "/";
                            else if (tnum == Capmovie.ITvNum[0])
                                sUrl = su1 + sye + ",ydb/";
                        }
                        page = Apis.GetPageCode(sUrl, "utf-8");
                        if (page == null)
                        {
                            wf.addRtb2ForThread(" 网址打不开!");
                            continue;
                        }
                        if (sYear == "") sYear = sye;

                        String snext = getOneInfo(page);

                        while (snext != null && snext != "")
                        {
                            if (snext.IndexOf("http://") == -1)
                                snext = StrAddress + snext;
                            page = Apis.GetPageCode(snext, "utf-8");

                            snext = getOneInfo(page);
                            if (WindForm.isCapxunlei == false)
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
                if (WindForm.isCapxunlei == false)
                {
                    UseStatic.soutTdRtb2("抓取已经由人工中断!");
                }
                else
                    WindForm.isCapxunlei = false;
                wf.Invoke((MethodInvoker)delegate { wf.setbtnXunlei(true); });
                WindForm.oneCatchEnd("isCapxunlei");
            }
        }

        private static String getOneInfo(String page)
        {
            int istart = page.IndexOf(start);
            if (istart == -1) return "";
            int iprev = page.IndexOf("class=\"list-pager\"", istart);
           
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

            String[] strsplit = { "title=\"收藏\"" };
            String[] pages = page.Split(strsplit, StringSplitOptions.RemoveEmptyEntries);

            foreach (String stmp in pages)
            {
                if (WindForm.isCapxunlei == false)
                {
                    break;
                }
                getMovieOneInfo(stmp);
            }
            //有下一页要翻页
            //<a href="http://so.letv.com/list/c1_t-1_a-1_y2011_f-1_at1_o1_p14.html">14</a>
            //<a href="http://so.letv.com/list/c1_t-1_a-1_y2011_f-1_at1_o1_p2.html">下一页</a>
            //或<a class="on" href="#">3</a><span>下一页</span></div>
            if (strpage == "") return "";
            int itmp = strpage.IndexOf(">下一页<");
            if (itmp == -1)
            {
                return "";
            }

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

        private static void getMovieOneInfo(String page)
        {

            vmb = new MovieMember();
            int it = 0, ik = 0;
            String reg = "", stmp = "";
            String[] rs = new string[2];
            String[] splitd = {"','"};

            rs = Apitool.getValueByName(page, "onmouseover=", "onmouseout=", "", out ik);
            if (rs != null)
            {
                stmp = rs[0];
                String[] pages = stmp.Split(splitd, StringSplitOptions.RemoveEmptyEntries);
                if(pages.Length > 10)
                {
                    vmb.sTitle = pages[2];
                    vmb.slang = pages[5].Replace("&nbsp;","");
                    vmb.sType = pages[6].Replace("&nbsp;", "/"); ;
                    vmb.presenter = pages[7].Replace("&nbsp;", "/"); 
                    vmb.director = pages[8].Replace("&nbsp;","");;
                    vmb.syear = pages[9];  
                }
            }


            rs = Apitool.getValueByName(page, "<img", "/>", "src", out ik);
            if (rs != null)
            {
                vmb.simageUrl = rs[1];
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

            //reg = ">导演";
            //it = page.IndexOf(reg);
            //if (it != -1)
            //{
            //    page = page.Substring(it + 3);
            //    rs = Apitool.getValueByName(page, ">", "<", "", out it);
            //    if (rs != null)
            //    {
            //        if (rs[0] != "")
            //            vmb.director = rs[0];
            //    }
            //}

            //reg = ">年份";
            //ik = page.IndexOf(reg);
            //if (ik != -1)
            //{
            //    rs = Apitool.getValueByName(page.Substring(ik + 3), ">", "<", "");
            //    if (rs != null)
            //    {
            //        if (int.TryParse(rs[0], out ix))
            //            vmb.syear = rs[0];
            //    }
            //}

            //reg = ">主演";
            //it = page.IndexOf(reg);
            //if (it != -1)
            //{
            //    if (it < ik)
            //    {
            //        stmp = page.Substring(it, ik - it);
            //        page = page.Substring(ik);
            //        StringBuilder sb = new StringBuilder();
            //        do
            //        {
            //            rs = Apitool.getValueByName(stmp, "<a", "</a>", "", out it);
            //            if (rs != null)
            //            {
            //                if (rs[0] != null && rs[0].Trim() != "")
            //                    sb.Append(rs[0] + "/");

            //            }
            //            if (it != -1) stmp = stmp.Substring(it + 3);
            //            else
            //                break;

            //        } while (rs != null);
            //        vmb.presenter = sb.ToString();
            //    }
            //}

            reg = ">查看详情<";
            it = page.IndexOf(reg);
            if (it != -1)
            {
                rs = Apitool.getValueByName(page.Substring(it), "<p>", "</p>", "");
                if (rs != null)
                {
                    vmb.sContent = rs[0].Trim();
                }
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

            vmb.tvnum = Itvnum;

            String sid = Capmovie.inserDBAll(vmb);

            //getDsjfile("data.movie.xunlei.com/movie/64399", sid, "", vmb.tvnum);//for test

            if (sid != "" && vmb.sUrl != "")
            {
                if (Capmovie.isInMovieUrl(vmb.sUrl, sid) == "")
                {
                    Capmovie.insertDBAddress(sid, vmb.sTitle, Capmovie.SMovieType[5], vmb.sUrl, "0", vmb.sPlayTime, vmb.time, vmb.jsnew);
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
                page = Apis.GetPageCode(url, "utf-8");
                if (page == null) return;

                //<p class="latest_update">40集完</p>
                //或者是
                //<div class="moviedteail_tt">
                //<h1>单身公主相亲记</h1><span>30集全</span></div>
                //或者
                //<p class="latest_update">已更新至<a title="第5集" target="_blank" 
                //href="http://kankan.xunlei.com/vod/mp4/64/64311/211397.shtml">第5集</a></p>
                //或者
                //class="latest_update">共40集 已更新至<a title="第27集" target="_blank" 
                // href="http://kankan.xunlei.com/vod/mp4/63/63751/220007.shtml">第27集</a>、<a title="第28集" target="_blank" 
                //href="http://kankan.xunlei.com/vod/mp4/63/63751/220097.shtml">第28集</a></p>


                String zjs = "";
                String jsnew = "";
                //Boolean isAll = false;
                String sjstmp = "";
                String stp2 = "";
                if (tvnum == 1 || tvnum == 2)
                {
                    int its = page.IndexOf("class=\"moviedteail_tt\"");
                    if (its != -1)
                    {
                        int ite = page.IndexOf("</div>", its);
                        if (ite != -1)
                        {
                            sjstmp = Apitool.FindStrByName(page.Substring(its, ite - its), "<span>", "</span>");
                        }
                    }
                    else
                    {
                        its = page.IndexOf("class=\"latest_update\"");
                        if (its != -1)
                        {
                            int ite = page.IndexOf("</p>", its);
                            if (ite != -1)
                            {
                                sjstmp = page.Substring(its, ite - its);
                                sjstmp = Regex.Replace(sjstmp,"<[^>]*",">").Replace(">>","").Replace(" ","");
                            }
                        }
                    }
                    if (sjstmp != null && sjstmp != "")
                    {
                        //class="latest_update">共40集 已更新至第27集  、  第28集   
                        //class="latest_update">共? 40集完  40集全
                        int itmp = sjstmp.IndexOf("集全");
                        if (itmp == -1) itmp = sjstmp.IndexOf("集完");
                        if (itmp != -1)
                        {
                            stp2 = getLastNumForStr(sjstmp.Substring(0, itmp));
                            if (stp2 != "")
                            {
                                zjs = stp2;
                                jsnew = zjs;
                            }
                        }
                        else
                        {
                            stp2 = Apitool.FindStrByName(sjstmp, "共", "集");
                            if (stp2 != null)
                            {
                                if (int.TryParse(stp2, out itmp))
                                {
                                    zjs = itmp + "";
                                    jsnew = zjs;
                                }
                            }
                        }
                        if (jsnew == "")
                        {
                            stp2 = Apitool.FindStrByName(sjstmp, "更新至", "集");
                            stp2 = getLastNumForStr(stp2);
                            if (stp2 != "") jsnew = stp2;
                        }
                    }

                    if (zjs == "") 
                    {
                        stp2 = Apitool.FindStrByName(page, "movieInfo.version='", "'");
                        if(stp2 != null)
                            zjs = getLastNumForStr(stp2);
                    }

                    String sql = "";
                    if (zjs != "")
                    {
                        sql = "update movie set zjs = '" + zjs + "' where id = " + id;
                        Apis.runSql(sql, Capmovie.strdbUrl);
                    }
                    if (jsnew != "")
                    {
                        sql = "";
                        if (urlid == "")
                        {
                            sql = "update movieurl set jsnew = '" + jsnew + "' where movieid = " + id + " and js = '0' and videotype = '" + Capmovie.SMovieType[5] + "'";
                        }
                        else
                        {
                            sql = "update movieurl set jsnew = '" + jsnew + "' where id = " + urlid;
                        }
                        Apis.runSql(sql, Capmovie.strdbUrl);
                    }
                        
                       
                 }
                else if (tvnum == 4)
                {
                   
                }


                String start = "class=\"imglist";
                String send = "id=\"fenji_turn";
                String sptmp = "";
                sptmp = Apitool.FindStrByName(page, start, send);
                if (sptmp == null)
                {
                    start = "class=\"movielist";
                    send = "class=\"movielist";
                    sptmp = Apitool.FindStrByName(page, start, send);
                    if (sptmp == null)
                    {
                        return;
                    }
                }

                String[] stmps = sptmp.Split(new String[] { "<li>" }, StringSplitOptions.RemoveEmptyEntries);

                /**
                 * <li><a class="pic" clickarea="fenji" 
                    href="http://kankan.xunlei.com/vod/mp4/57/57011/151362.shtml" target="_blank" 
                    title="三国 第1集" subid="151362" subid2="218394">
                 * <img _src="http://images.movie.xunlei.com/submovie_img/57/57011/1_1_115x70.jpg" 
                    src="http://images.movie.xunlei.com/img_default.gif" alt="三国 第1集" title="三国 
                    第1集"></a><a clickarea="fenji" 
                    href="http://kankan.xunlei.com/vod/mp4/57/57011/151362.shtml" target="_blank" 
                    title="三国 第1集">三国 第1集</a></li>
                 *  
                 * <li><a subid2="31877" subid="20454" title="单身公主相亲记 第5集" target="_blank" 
                 * href="http://kankan.xunlei.com/vod/mp4/59/59551/20454.shtml" clickarea="fenji" class="pic">
                 * <img title="单身公主相亲记 第5集" alt="单身公主相亲记 第5集"
                 *  src="http://images.movie.xunlei.com/submovie_img/59/59551/5_1_115x70.jpg" 
                 * _src="http://images.movie.xunlei.com/submovie_img/59/59551/5_1_115x70.jpg" 
                 * id="http%3A%2F%2Fimages.movie.xunlei.com%2Fsubmovie_img%2F59%2F59551%2F5_1_115x70.jpg0.9805881130562455"/>
                 * </a><a title="单身公主相亲记 第5集" target="_blank" href="http://kankan.xunlei.com/vod/mp4/59/59551/20454.shtml"
                 * clickarea="fenji">单身公主相亲记 第5集</a></li>
                 */

                foreach (String st in stmps) 
                {
                    String surl = "", sname = "", sjs = "9999";  //没有集数就定义为9999

                    String[] rs2 = Apitool.getValueByName(st, "title=\"", "\"", "");
                    if (rs2 != null)
                    {
                        sname = rs2[0].Trim();
                    }
                    rs2 = Apitool.getValueByName(st, "<a", ">", "href");
                    if (rs2 != null)
                    {
                        surl = rs2[1].Trim();
                       
                        String tmp = getLastNumForStr(sname);
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
                                Capmovie.insertDBAddress(id, sname, Capmovie.SMovieType[5], surl, sjs, "", "0");
                                wf.addRtb2ForThread(sname + " ID: " + id + " : " + surl + " 已入库");
                            }
                            else
                            {
                                wf.addRtb2ForThread(sname + " No. " + sjs + " ID: " + id + " 已存在");
                            }
                        }
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

        //取得形如"ABC47" 这样的字符串的最后一串数字,没找到返回""
        private static String getLastNumForStr(String str)
        {
            if (str == null || str == "") return "";
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
        
        
        public static void capXunleidsj()
        {
            SqlConnection cn = null;
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接

            try
            {
                UseStatic.soutTdRtb2("开始抓迅雷电视剧动画片");

                if (cn == null)
                    cn = Apis.GetCon(Apis.sDBUrl206);
                SqlCommand msc = cn.CreateCommand();

                int[] tvNums = { 1, 2 };
                foreach (int tvnum in tvNums)
                {
                    if (WindForm.isCapXunleidsj == false)
                    {
                        return;
                    }
                    msc.CommandText =
                        "select id,movieid,surl from movieurl where movieid in (select id from movie where years > 2011 and tvnum = " + tvnum +
                        ") and videotype = '迅雷视频' and js = 0 order by movieid desc";
                    if (dr1 != null)
                    {
                        dr1.Close();
                    }
                    dr1 = msc.ExecuteReader();
                    while (dr1.Read())
                    {
                        if (WindForm.isCapXunleidsj == false)
                        {
                            return;
                        }
                        String id = dr1["movieid"].ToString().Trim();
                        String url = dr1["surl"].ToString().Trim();
                        String urlid = dr1["id"].ToString().Trim();
                        getDsjfile(url, id, urlid, tvnum);
                    }
                    UseStatic.soutTdRtb2("所有迅雷 tvnum= " + tvnum + " 频道节目已抓取!");
                }
                //getDsjfile("http://v.qq.com/cover/v/vwfr2scuby3xbgm.html", "77777");
                UseStatic.soutTdRtb2("所有迅雷电视剧已抓取!");
            }
            catch (Exception ex)
            {
                UseStatic.soutTdRtb2(ex);
            }
            finally
            {
                if (WindForm.isCapXunleidsj == false)
                {
                    UseStatic.soutTdRtb2("抓取已经由人工中断!");
                }
                else
                    WindForm.isCapXunleidsj = false;
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
                WindForm.oneCatchEnd("isCapXunleidsj");
            }
        }

    }//end capXunlei
}
