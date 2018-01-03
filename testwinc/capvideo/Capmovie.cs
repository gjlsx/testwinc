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
using System.Threading;


namespace testwinc.capvideo
{
    class Capmovie
    {
        private static WindForm wf = null;
        public static String strdbUrl = "Server=218.85.133.206;database=tvsport;uid=sa;pwd=FJwz%@)50";
        private static MovieMember vmb = null;
        public static readonly String StrResult = "select top 1 SCOPE_IDENTITY() from movie";
        private static readonly String StrQQVideoAddress = "v.qq.com";
        //private static String strCut = "";
        //private static System.Windows.Forms.WebBrowser wb = null;
        public static int iTvnumum = 1;

        public static readonly String[] SMovieType = new String[] { "腾讯视频", "搜狐视频", "CNTV", "优酷视频", "LETV", "迅雷视频", "土豆视频" };
        //tvnum = 0 -电影， 1-电视剧 2-动漫 3-央视名栏目,4-综艺,5-音乐mtv,999-其他
        public static readonly int[] ITvNum = new int[] { 0, 1, 2, 3 ,4,5,999};
        public static String sUrlQQDsj = @"http://v.qq.com/list/2_-1_-1_2012_1_0_0_20_-1_-1.html"; //腾讯电视剧频道
        public static String sUrlQQDy = "http://v.qq.com/list/1_-1_-1_2012_1_0_0_20_0_-1.html";//腾讯电影频道
        public static String sUrlQQmusic = "http://v.qq.com/mvlist/0/22_-1_2012_-1_-1_1_0_0_28.html";//腾讯MV频道

        public static LinkedList<String> llUrls = null;
        private static String[] strsplit = { "=mod_item " };
        public static int iPageNow = 1;
        public static int IQQMovieTime = 1;
        public static String strYear = "2012"; 
        public static Boolean isLastpage = false;

        static Capmovie()
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
        public static void getMovieVideo()
        {
            try
            {
                String surl = wf.getTbxAddress().ToLower();
                if (surl.IndexOf("v.qq.com") == -1)
                {
                    wf.addRtb2ForThread("开始抓取2012 QQ电影,电视剧,MV，要抓其他请在浏览器中输入腾讯视频页面地址！\r\n");
                    if (llUrls == null) llUrls = new LinkedList<string>();
                    llUrls.Clear();
                    //llUrls.AddLast(sUrlQQDsj);
                    llUrls.AddLast(sUrlQQDy);
                    llUrls.AddLast(sUrlQQmusic);   
                }
                else 
                {
                    if (llUrls == null) llUrls = new LinkedList<string>();
                    llUrls.Clear();
                    llUrls.AddLast(surl);
                }

                iTvnumum = 1;

                wf.addRtb2ForThread("开始抓 " + llUrls.First.Value + " 数据啦：）");

               // getPageDetail(page);
               // wf.addRtb2ForThread(url + " " + " 数据已被抓取");

                //if (wb == null)
                //{
                //    wb = wf.getWebBrowser();
                //    //wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(getWebpage);
                //}
                //wf.Invoke((MethodInvoker)delegate {
                
                //wb.Navigate(url);

                    //while (strCut == "")
                    //{
                    //    Application.DoEvents();//等待本次加载完毕才执行下次循环,期间界面可以做别的事
                    //}
                wf.Invoke((MethodInvoker)delegate { wf.getPageByTimer(); });
               // });
                
            }
            catch (Exception e)
            {
                wf.addRtb2ForThread(e.StackTrace);
                wf.getbtnpptv().Invoke((MethodInvoker)delegate { wf.getbtnpptv().Enabled = true; }); 
            }
            finally
            {
               // WindForm.isCappptv = false;
               // wf.getbtnpptv().Invoke((MethodInvoker)delegate { wf.getbtnpptv().Enabled = true; }); 
            }
        }


        //private static void getWebpage(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    strCut = wb.Document.Body.InnerHtml.ToString();
        //}

        public static void getPageDetail(String page)
        {
            String slist = "mod_video_list";
            String slend = "mod_pagenav";
            String spage = "=mod_pagenav_count2";
            String spagenow = "class=current>";

            int isl = page.IndexOf(slist);
            int iend = -1;
            if (isl != -1)
            {
                iend = page.IndexOf(slend, isl);
            }
            if (isl == -1 || iend == -1)
            {
                wf.addRtb2ForThread("数据格式错");
                return;
            }

            IQQMovieTime = 1;
            iPageNow = 1;
            isLastpage = false;

            int itype = CapYouku.getParaByInput("N");   //供手动修改type和页数
            if (itype != -1) Capmovie.iTvnumum = itype;
            itype = CapYouku.getParaByInput("y");
            if (itype != -1) Capmovie.strYear = itype +"";  //年份
            else Capmovie.strYear = "";

            int ipage = CapYouku.getParaByInput("p");
            if (ipage != -1) Capmovie.IQQMovieTime = ipage;
            else
            {
                ipage = page.IndexOf(spage);
                if (ipage != -1)
                {
                    String stmp = page.Substring(ipage, 200);
                    ipage = stmp.IndexOf(spagenow);
                    if (ipage != -1)
                    {
                        String[] spnow = Apitool.getValueByName(stmp, spagenow, "</", "");
                        short tt = 1;
                        if (spnow != null && Int16.TryParse(spnow[0], out tt))
                        {
                            iPageNow = tt;
                        }
                        ipage = stmp.IndexOf(spagenow);
                        spnow = Apitool.getValueByName(stmp, ">/", "</", "");
                        if (spnow != null && Int16.TryParse(spnow[0], out tt))
                        {
                            IQQMovieTime = tt;
                            if (iPageNow == IQQMovieTime)
                            {
                                isLastpage = true;
                            }
                        }
                    }
                }
            }

            page = page.Substring(isl, iend - isl);

            String[] strsplit = { "=mod_poster_130" };
            String[] pages = page.Split(strsplit, StringSplitOptions.RemoveEmptyEntries);
            if (pages.Count() < 2)
            {
                pages = page.Split(new String[] { "mod_poster_130" }, StringSplitOptions.RemoveEmptyEntries);
            }

            foreach (String stmp in pages)
            {
                if (WindForm.isCappptv == false)
                {
                    wf.addRtb2ForThread("抓取已被人工终止!");
                    break;
                }
                getMovieOneInfo(stmp);
            }
        }

        //获得一条信息，string形如:
        private static void getMovieOneInfo(String page)
        {
            if (page == null || page.Trim() == "") return;
           try{
            //腾讯的页面写的很好,css配js,很好很强大,但是可恶的IE....
            vmb = new MovieMember();

            String[] rs = Apitool.getValueByName(page, "<IMG", ">", "src");
            if (rs != null)
            {
                if(rs[1]!=null)
                    vmb.simageUrl = rs[1];
            }
            String reg = "=scores";
            String stmp = "";
            int it = 0,ik = 0;
            it = page.IndexOf(reg);
            if (it == -1) return;

            page = page.Substring(it);
            rs = Apitool.getValueByName(page, "<A", "</A>", "href");
            if (rs != null)
            {
                vmb.sUrl = StrQQVideoAddress + rs[1];
                it = vmb.sUrl.ToLower().IndexOf("http://");
                if (it != -1)
                {
                    vmb.sUrl = vmb.sUrl.Substring(it + "http://".Length);
                }
                vmb.sTitle = rs[0];
            }

            page = page.ToLower();

            if (iTvnumum == ITvNum[5])
            {
                rs = Apitool.getValueByName(page, "<p>", "</p>", "");
                if (rs != null)
                {
                    vmb.sContent = rs[0];
                }

                reg = ">歌手";
                it = page.IndexOf(reg);
                if (it != -1)
                {
                    int itz = page.IndexOf("</a>", it);
                    ik = page.IndexOf(">标签");
                    if (itz != -1 && ik != -1 && itz < ik)
                    {
                        itz = page.IndexOf("\">", it);
                        ik = page.IndexOf("</", it);
                        if (ik != -1)
                        {
                            vmb.presenter = page.Substring(itz + 2, ik - itz - 2);
                        }
                    }
                }
                if (strYear != "")
                    vmb.syear = strYear;
                else
                    vmb.syear = "2012";
            }
            else
            {
                reg = ">导演";
                it = page.IndexOf(reg);
                if (it != -1)
                {
                    int itz = page.IndexOf("</a>", it);
                    ik = page.IndexOf(">主演");
                    if (itz != -1 && ik != -1 && itz < ik)
                    {
                        itz = page.IndexOf(">", it+1);
                        ik = page.IndexOf("</", it+1);
                        if (ik != -1)
                        {
                            vmb.director = page.Substring(itz + 1, ik - itz - 1);
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
                        //还有  <P>播放：72681393</P></LI>
                        int itx = page.IndexOf("</p>", it);
                        if (itx != -1)
                        {
                            if (itx > ik)
                            {
                                itx = ik;
                            }
                        }
                        else
                        {
                            itx = ik;
                        }

                        int itz = page.IndexOf("</a>", it);
                        if (itz != -1 && itz < itx)
                        {
                            stmp = page.Substring(it + 3, itx - it - 3);
                            page = page.Substring(ik);
                            reg = ">[^<]*";
                            Match m = Apitool.GetResultOfReg(stmp, reg);
                            if (m != null)
                            {
                                StringBuilder sb = new StringBuilder();
                                while (m.Success)
                                {
                                    sb.Append(m.Value.Substring(1) + "/");
                                    m = m.NextMatch();
                                }
                                vmb.presenter = sb.ToString();
                            }
                        }

                    }
                }
                reg = ">地区";
                it = page.IndexOf(reg);
                if (it != -1)
                {
                    ik = page.IndexOf(">年份", it);
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
                reg = ">年份：";
                it = page.IndexOf(reg);
                if (it != -1)
                {
                    ik = page.IndexOf(">类型", it);
                    if (ik != -1)
                    {
                        int itz = page.IndexOf("</a>", it);
                        page = page.Substring(it);
                        if (itz != -1 && itz < ik)
                        {
                            rs = Apitool.getValueByName(page, "<a", "</a>", "");
                            if (rs != null)
                            {
                                vmb.syear = rs[0];
                            }
                        }
                        else
                        {//其他：时候没有</a>
                            itz = page.IndexOf("<");
                            if (itz != -1)
                            {
                                String syt = page.Substring(">年份：".Length, itz - ">年份：".Length).Trim();
                                try
                                {
                                    int a = Int16.Parse(syt);
                                    if (a < 2047 && a > 0)
                                        vmb.syear = syt;
                                }
                                catch { }
                            }
                        }
                    }
                }
                if (vmb.syear == "")
                {
                    if (Capmovie.strYear != "")
                    {
                        vmb.syear = Capmovie.strYear;
                    }
                    else
                    {
                        vmb.syear = "2012";//今年抓的都定为2012
                    }
                }

                reg = ">类型";
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
                            vmb.sType = sb.ToString();
                        }

                    }
                }
                reg = ">简介";
                it = page.IndexOf(reg);
                if (it != -1)
                {
                    rs = Apitool.getValueByName(page.Substring(it), "<", "</p>", "");
                    if (rs != null)
                    {
                        vmb.sContent = Apis.shtmlQuick(rs[0]);
                    }
                }
            }
            //if (vmb.sTitle.ToLower().IndexOf("断喉弩") != -1)
            //{
            //    int aaa = 0;
            //}
            vmb.shtmlString();


            vmb.tvnum = iTvnumum;// ITvNum[0];//电影
            //vmb.tvnum = ITvNum[1];//电视剧
            //vmb.tvnum = ITvNum[2];//动漫

            String  sid =  inserDBAll(vmb);
            if (sid != "" && vmb.sUrl != "")
            {
                if (isInMovieUrl(vmb.sUrl, sid) == "")
                {
                    insertDBAddress(sid, vmb.sTitle, SMovieType[0],vmb.sUrl,"0",vmb.sPlayTime,vmb.time);
                    if (vmb.tvnum == Capmovie.ITvNum[1] || vmb.tvnum == Capmovie.ITvNum[2])
                    {
                        getDsjfile(vmb.sUrl, sid, "", vmb.tvnum);
                    }
                    wf.addRtb2ForThread(vmb.sTitle + " ID: " + sid+ " : " + vmb.sUrl + " 已入库");
                }
            }
           
        }
        catch
        {
        }
        }//end getMovieOneInfo
        
        /*
         * qq 的cs 2011,12.06,
                    {for movie in movies}
            <div class="mod_item">
	            <div class="mod_pic">
		            <a href="${movie.cover_id|getPlayUrl}" class="mod_poster_v">
			            <img src="${movie.pic_url}" onerror="picerr(this);" alt="${movie.title}" class="_tipimg" />
            			
		            </a>
	            </div>
	            <div class="mod_txt">
		            <div class="mod_item_tit">
			            <h3><a href="${movie.cover_id|getPlayUrl}">${movie.title}</a></h3>
		            </div>
		            <div class="mod_scores"><strong class="c_txt3">${movie.s0}<span>.${movie.s1}</span></strong><em>分</em></div>
		            <ul class="mod_data">
			            <li class="director">导演：
				            {for act in movie.director}
				            <a href="/search.html?pagetype=3&ms_key=${act|encode}">${act}</a>
				            {/for}
			            </li>
			            <li class="actor">主演：
				            {for act in movie.actor}
				            <a href="/search.html?pagetype=3&ms_key=${act|encode}">${act}</a>
				            {/for}
			            </li>
			            <li>地区：
				            {if movie.area_index==9999}${movie.area_s}
				            {else}
					            <a href="list_2_${movie.area_index}_${op.mi_sort}_${op.mi_show_type}_1.html">${movie.area_s}</a>
				            {/if}
			            </li>
			            <li>年份：{if movie.year_index==9999}${movie.year}
				            {else}
					            <a href="list_3_${movie.year_index}_${op.mi_sort}_${op.mi_show_type}_1.html">${movie.year}</a>
				            {/if}
			            </li>
			            <li>类型：
				            {for sub in movie.subtype}
				            {if sub.id==9999}${sub.n}
				            {else}
					            <a href="list_1_${sub.id}_${op.mi_sort}_${op.mi_show_type}_1.html">${sub.n}</a>
				            {/if}
				            {/for}
			            </li>									
		            </ul>
		            <p class="mod_details_desc c_txt4"><span class="c_txt1">简介：</span>${movie.desc}</p>
	            </div>						
            </div>
            {/for}
         */

        /// <summary>
        /// 插入一条数据，1.title,2.years,3 type,4 froeid 5.time 6 creattime 7 director 8 presenter 9 url 10 .content 
        /// 返回已插入或已存在数据的movie id
        /// </summary>
        public static String inserDBAll(MovieMember vm)
        {
            String isexist = isInDb(vm);
            if (isexist != "")
            {
                wf.addRtb2ForThread( vm.sTitle + " 已经存在, movie ID = " + isexist);
                return isexist;
            }
            String strInser = "insert into movie(title,years,type,time,lang,creattime,edittime,foreid,director,presenter,imageurl,content,tvnum,zjs) values('" + vm.sTitle
                + "','" + vm.syear + "','" + vm.sType + "','" + vm.time + "','" + vm.slang + "','" + DateTime.Now.ToString() + "','" + DateTime.Now.ToString() + "','" 
                + vm.forenoticid+ "','" + vm.director + "','" + vm.presenter + "','" + vm.simageUrl + "','" + vm.sContent + "','" + vm.tvnum + "','" + vm.zjs + "')";

            return Apis.runSqlOneResult(strInser, StrResult, strdbUrl) + "";
        }

        /// <summary>
        /// 根据movie名和年份，导演等判断是否数据已经入库,已存在返回该id,否则返回空字符
        /// </summary>
        /// <param name="vm">MovieMember</param>
        /// <returns>id或""</returns>
        public static String isInDb(MovieMember vm)
        {
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
            SqlConnection cn = null;
            try
            {
                String strfind = "select id,title,years,director,zjs from movie where title like '%" + vm.sTitle + "%' and tvnum = " + vm.tvnum;
                cn = Apis.GetCon(strdbUrl);//连接数据库
                SqlCommand msc = cn.CreateCommand();
                //这里要判断是否数据已经入库，add code here
                msc.CommandText = strfind;

                dr1 = Apis.searchInDB(msc, strfind);
                while (dr1.Read())
                {
                    String dire = dr1["director"].ToString().Trim();
                    String title = dr1["title"].ToString().Trim();
                    //名字完全相同，导演相同，则年份不重要
                    String year = dr1["years"].ToString().Trim();
                    String zjs = dr1["zjs"].ToString().Trim();
                        if (title == vm.sTitle)
                        {
                            if(vm.tvnum == Capmovie.ITvNum[0])
                            {
                                if (dire == vm.director || year == vm.syear)
                                    return dr1["id"].ToString();
                            }
                            else 
                            {
                                if (year == vm.syear) return dr1["id"].ToString();
                            }
                        }
                        else
                        {   //名字类似，导演相同，还需年份相同
                            if (vm.tvnum == Capmovie.ITvNum[0])
                            {
                                if (dire == vm.director)
                                {
                                    if (year == vm.syear)
                                        return dr1["id"].ToString();
                                }
                            }
                            else // //名字类似，导演相同，总集数相同
                            {
                                if (year == vm.syear && zjs == (vm.zjs+""))
                                    return dr1["id"].ToString();
                            }
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

        /// <summary>
        /// 根据url和videoid判断是否数据已经在MovieUrl表中,已存在返回该id,否则返回空字符
        /// </summary>
        /// <param name="videoid"></param>
        /// <param name="surl"></param>
        /// <returns></returns>
        public static String isInMovieUrl(String surl, String movieid)
        {
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
            SqlConnection cn = null;
            try
            {
                String strfind = "select surl,id from movieurl where movieid = " + movieid;
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


        //入库movieurl表
        public static void insertDBAddress(String movieid, String title, String videoType, String surl,String js,String playtime,String times)
        {
            String strInser = "insert into movieurl(movieid,title,videotype,surl,creattime,js,playtime,times) values('" + movieid + "','"
             + title + "','" + videoType + "','" + surl + "','" + DateTime.Now.ToString() + "','" + js + "','" + playtime + "','" + times + "')";
            Apis.runSql(strInser, strdbUrl);
        }

        //入库movieurl表
        public static void insertDBAddress(String movieid, String title, String videoType, String surl, String js, String playtime, String times,int jsnew)
        {
            String strInser = "insert into movieurl(movieid,title,videotype,surl,creattime,js,playtime,times,jsnew) values('" + movieid + "','"
             + title + "','" + videoType + "','" + surl + "','" + DateTime.Now.ToString() + "','" + js + "','" + playtime + "','" + times + "','" + jsnew + "')";
            Apis.runSql(strInser, strdbUrl);
        }


        public static void capQQdsj() 
        {
            SqlConnection cn = null;
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接

            try
            {
                UseStatic.soutTdRtb2("开始抓QQ电视剧分集");

                if (cn == null)
                    cn = Apis.GetCon(Apis.sDBUrl206);
                SqlCommand msc = cn.CreateCommand();

                int[] tvNums = { 2,1 };
                foreach (int tvnum in tvNums)
                {
                    if (WindForm.isCapqqdsj == false)
                    {
                        return;
                    }
                    msc.CommandText =
                        "select id,movieid,surl from movieurl where movieid in (select id from movie where years > 2011 and tvnum = " + tvnum +
                    " and id > 28888) and videotype = '腾讯视频' and js = 0 order by movieid desc";
                    if (dr1 != null)
                    {
                        dr1.Close();
                    }
                    dr1 = msc.ExecuteReader();
                    while (dr1.Read())
                    {
                        if (WindForm.isCapqqdsj == false)
                        {
                            return;
                        }
                        String id = dr1["movieid"].ToString().Trim();
                        String url = dr1["surl"].ToString().Trim();
                        String urlid = dr1["id"].ToString().Trim();
                        getDsjfile(url, id,urlid,tvnum);
                    }
                    UseStatic.soutTdRtb2("所有QQ tvnum= " + tvnum + " 频道节目已抓取!");
                }
                UseStatic.soutTdRtb2("所有QQ电视剧已抓取!");
            }
            catch (Exception ex)
            {
                UseStatic.soutTdRtb2(ex);
            }
            finally
            {
                if (WindForm.isCapqqdsj == false)
                {
                    UseStatic.soutTdRtb2("抓取已经由人工中断!");
                }
                else WindForm.isCapqqdsj = false;
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
                WindForm.oneCatchEnd("isCapqqdsj");
            }
        }

        //http://218.85.133.206:81/
        private static void getDsjfile(String url, String id,String urlid,int tvnum)
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
                    String start = "id=\"mod_videolist\"";
                    String send = "id=\"recommand\"";
                    if (url.IndexOf("/detail/") != -1)
                    {
                        send="\"mod_grade\"";
                    }
                    //<li>剧集：全26集</li>
                    //<li>剧集：更新至30集</li>
                    int zjs = 0;
                    int jsnew = 0;
                    if (tvnum == 1)
                    {
                        int its = page.IndexOf(">剧集：");
                        if (its != -1)
                        {
                            int ite = page.IndexOf("集<", its);
                            if (ite != -1)
                            {
                                String sjstmp = page.Substring(its, ite - its);
                                its = sjstmp.IndexOf("全");
                                if (its != -1)
                                {
                                    if (int.TryParse(sjstmp.Substring(its + 1), out ite))
                                    {
                                        zjs = ite;
                                        String sql = "update movie set zjs = '" + zjs + "' where id = " + id;
                                        Apis.runSql(sql, strdbUrl);
                                        if (urlid == "")
                                        {
                                            sql = "update movieurl set jsnew = '" + zjs + "' where movieid = " + id + " and js = '0' and videotype = '" + Capmovie.SMovieType[0] + "'";
                                        }
                                        else
                                        {
                                            sql = "update movieurl set jsnew = '" + zjs + "' where id = " + urlid;
                                        }
                                        Apis.runSql(sql, strdbUrl);
                                    }
                                }
                                else
                                {
                                    its = sjstmp.IndexOf("更新至");
                                    if (its != -1)
                                    {
                                        if (int.TryParse(sjstmp.Substring(its + 3), out ite))
                                        {
                                            jsnew = ite;
                                             String sql = "";
                                             if (urlid == "")
                                             {
                                                sql = "update movieurl set jsnew = '" + jsnew + "' where movieid = " + id + " and js = '0' and videotype = '" + Capmovie.SMovieType[0] + "'";
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
                    }
                    else if (tvnum == 2)
                    {
                        int its = page.IndexOf(">视频：");
                         if (its != -1)
                         {
                             int ite = page.IndexOf("个<", its);
                             if (ite != -1)
                             {
                                 String sjstmp = page.Substring(its, ite - its);

                                 if (int.TryParse(sjstmp.Substring(">视频：".Length), out ite))
                                     {
                                         zjs = ite;
                                         String sql = "update movie set zjs = '" + zjs + "' where id = " + id;
                                         Apis.runSql(sql, strdbUrl);
                                     }
  
                             }
                         }
                    }
                    page = Apitool.FindStrByName(page, start, send);
                    if (page == null)
                    {
                        return;
                    }
                    String regex = "<a [^<]*";
                    Match m = Apitool.GetResultOfReg(page, regex);
                    if (m != null)
                    {
                        while (m.Success)
                        {
                            //<a target="_self" href="/cover/m/m2oobmidz797a2w/6CIECXKTU20.html" id="6CIECXKTU20"  title="棒球英豪 第1集"  tp="3">1
                            String surl = "", sname = "", sjs = "9999";  //没有集数就定义为9999
                            String st = m.Value;
                            String[] rs2 = Apitool.getValueByName(st, "href=\"", "\"", "");
                            if (rs2 != null)
                            {
                                surl = StrQQVideoAddress + rs2[0].Trim().Replace("http://", "");

                            }
                            rs2 = Apitool.getValueByName(st, "title=\"", "\"", "");
                            if (rs2 != null)
                            {
                                sname = rs2[0].Trim();
                            }
                            int tmp = st.LastIndexOf(">");
                            if (tmp != -1)
                            {
                                sjs = st.Substring(tmp + 1);
                                sjs = sjs.Replace("集", "").Replace("第", "");
                                if (!int.TryParse(sjs, out tmp))
                                {
                                    sjs = "9999";
                                }
                            }
                            if (isInMovieUrl(surl, id) == "")
                            {
                                insertDBAddress(id, sname, SMovieType[0], surl, sjs, "", "0");
                                wf.addRtb2ForThread(sname + " ID: " + id + " : " + surl + " 已入库");
                            }
                            else
                            {
                                wf.addRtb2ForThread(sname + " No. " + sjs + " ID: " + id + " 已存在");
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



        public static void dispose()
        {
            if (vmb != null)
            {
                vmb = null;
            }
        }

    }//end Class Capmovie
}
