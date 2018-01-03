using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using testwinc.tools;
using testwinc.test;
using System.Drawing;
using System.Net;
using System.IO;
using System.Threading;
using mshtml;
using System.Diagnostics;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data;



// using tessnet2;

namespace testwinc.capvideo
{
    class ForNaifen : InterfaceWBDoc
    {
        private static WindForm wf;
        private static WebBrowser wb = null;
        private static String url = "http://162.218.31.179:8211/ssxcx49107f/user/login.html.auth";
        //static String url2 = "www.baidu.com";  // 时时测试: 1.5158.us 安全码: 16239
        private static String sname = "aaff05";
        private static String spwd = "aa1234";
       // static String stmp = "96321";

        private static String STRConfigFileOld = @"\wlre.dat";//配置文件
        public static String STRConfigFile = "";
        public static String STR_LogFile = "";

        private static System.Windows.Forms.Timer myTimer = null;

        public static Boolean isTestSubmit = true;//是否真正提交
        
        //public static String[] STR_Save_Name = new String[]{"nf11","nf12","nf21","nf22","nf31","nf32",
        //                            "nft11", "nft12", "nft21", "nft22", "nft31", "nft32"};

        public static Boolean isForceBet = false;//是否强制投注

        private static System.Windows.Forms.Timer ttime = null;
        private static Color[] Col_RightNum = new Color[] {Color.Black,Color.Blue, Color.Red};
        private static String Str_menu = "   投注类型  投注位置  投注号码  投注倍率  中奖状态";
        private static String Str_split = "-------------------------------------------------------";

        //测试用timer
        private static System.Windows.Forms.Timer ti = null; //两面盘和开奖页面timer
        private static System.Windows.Forms.Timer ttest = null;
        private static int iwebid = 0;// 0代表1页面，1代表另一页面
        public static int iturnpage = 0;// 0代表未跳转，1代表已经跳转一次

        private static String sAddrWinMoney = "";
        //盈亏页面地址//http://103.9.229.123:8211/ssxcx49107f_945/klc/history/index/&kw=1   //盈亏页面

        private static Object ObjLockNf = new Object();

        static ForNaifen()
        {
            if (wf == null)
            {
                wf = UseStatic.getWindForm();
            }
            if(wb == null)
                wb = wf.getWebBrowser();

            STRConfigFile = System.Environment.CurrentDirectory + STRConfigFileOld;
            STR_LogFile = System.Environment.CurrentDirectory + @"\log\" 
                + DateTime.Now.Year+DateTime.Now.Month +DateTime.Now.Day+ ".xml";
        }

          //去到frame url
        public void goFrame(WebBrowser wtb)
        {
            if (wb == null)
            {
                wb = wtb;
            }

            //原作为接口调用在浏览器页面加载完时执行，但是不好用，屏蔽，by wind
            //if (url.IndexOf("hh2.vip-1s1s.9761dtqs.com:12014") != -1)
            //{
            //    if (!(WindForm.llIfaceWB.Contains(this)))
            //        WindForm.llIfaceWB.AddLast(this);
            //}
            String st = wf.getNfAddress();
            if (!st.Equals(""))
            {
                wb.Navigate(st);
            }
            else
            {
                wb.Navigate(url);
            }

            clearDataWhenLogin();

            ttime = new System.Windows.Forms.Timer();
            ttime.Tick += new EventHandler(ttimerEvent);
            ttime.Interval = 3000;
            ttime.Start();
        }

        public void doAfterDocumentCompleted()
        {
           // getFrames(wb);
           
        }

        /// <summary>
        /// //每次登录就需要清空记录
        /// </summary>
        public void clearDataWhenLogin() 
        {
            ForNaiStrategy.clearHistory();
            ForNaiStrategy.iDanCpu = -1;
            ForNaiStrategy.iTwoCpu = -1;
            ForNaifen.sAddrWinMoney = "";
            wf.setNaifenAddValue();
        }

        private void ttimerEvent(Object myObject, EventArgs myEventArgs)
        {
            if (ttime != null)
            {
                ttime.Stop();
                ttime = null;
            }
            getFrames(wb);
        }

        ////webbrowser装载完毕后执行,这里加不好，主类中用委托来更好
        //private void wb_DocumentCompleted(object sender,WebBrowserDocumentCompletedEventArgs e)
        //{ 
        //    //do sth;
        //    //注销事件
        //    //wb.DocumentCompleted -=
        //     //   new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);
        //}

        //获得frames下所指定frame内容
        public void getFrames(WebBrowser wtb)
        {
            try
            {

                if (wb == null)
                {
                    wb = wtb;
                }
                wf.addRtb2ForThread("开始测试 " + url + " " + " 数据 \r\n");

                //wb.Navigate(url2);//测试百度
                //setFrameValue("kw", "1234");
               // submitform("f");

               // wb.Navigate(url);

                //应对老版本，这个版本主界面是frame嵌套 ,15.1.19
                //HtmlDocument hdtmp = getFrameByIframeName(wb.Document, "topFrame");

                //应对新版本，这个版本主界面没有frame嵌套，by wind ,15.1.19
                HtmlDocument hdtmp = wb.Document;

                if (hdtmp != null)
                {
                    Thread th = new Thread(new ParameterizedThreadStart(ForNaifen.setLoginValueByThread));//登录
                    th.IsBackground = true;                    

                    //设置要设置的值
                    String stp = wf.getNfLoginUser();
                    if (!stp.Equals(""))
                    {
                        sname = stp;
                    }
                    stp = wf.getNfLoginPwd();
                    if (!stp.Equals(""))
                    {
                        spwd = stp;
                    }

                    //老版本
                    //String[] s1 = { "loginName", sname };
                    //String[] s2 = { "loginPwd", spwd };

                    //新版本。15.1.19
                    String[] s1 = { "__name", sname };
                    String[] s2 = { "password", spwd };

                    LinkedList<String[]> ll = new LinkedList<String[]>();
                    ll.AddFirst(s1);
                    ll.AddFirst(s2);

                    Object[] obj = new Object[] { hdtmp, ll };
                    
                    th.Start(obj);

                    // setFrameValueByName(hdtmp, "loginName", sname);
                    //  setFrameValueByName(hdtmp, "loginPwd", spwd);
                   
                }
                else
                {
                    wf.addRtb2ForThread("Frame is null!");
                }

                wf.addRtb2ForThread(url + " " + " 测试完成");
            }
            catch (Exception e)
            {
                wf.addRtb2ForThread(e.StackTrace);
            }
            finally
            {
               
            }
        }

        /// <summary>
        /// 获得iframe嵌套的子frame内容  //by wind,14.11.30
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="franame"></param>
        /// <returns></returns>
        public static HtmlDocument getFrameByIframeName(System.Windows.Forms.HtmlDocument doc, String franame)
        {
            if (doc == null)
                return null;
           
            if (doc.Window.Frames != null)
            {
                int ic = doc.Window.Frames.Count;
                if (ic == 0) return null;

              //  HtmlWindowCollection hwc = doc.Window.Frames["user"];
                //if (hwc != null)
                //{
                //    HtmlDocument hd2 = hwc.Document;
                //}

                ////例如：frame框架如下，而实际页面在user.htm 里
                ////<frameset rows="*,0" frameborder="0" border="0">
                ////<frame name="topFrame" src="user">
                ////<frame name="belowFrame" src="index.htm">
                ////</frameset>

                 //for (int i = 0; i < ic; i++)
                 //{
                 //   //取到包含input标签的元素,这里的sframename 就是topframe 和belowframe
                 //    String sframename = doc.Window.Frames[i].Name;
                 //   wf.addRtb2ForThread("frame no: " + i + " is " + sframename);
                 //   ////MessageBox.Show("iFrameName is: " + doc.Window.Frames[i].Name);                  
                 //}
                if (doc.Window.Frames[franame] != null)
                {
                    HtmlDocument hdtmp = doc.Window.Frames[franame].Document;
                    return hdtmp;   
                }
            }
            return null;        
        }

        public static void showFrames(System.Windows.Forms.HtmlDocument doc)
        {
            if (doc == null)
                return;

            if (doc.Window.Frames != null)
            {
                int ic = doc.Window.Frames.Count;
                if (ic == 0) return;
                wf.addRtb2ForThread("doc.Window.Frames.Count is:  " + ic);

                for (int i = 0; i < ic; i++)
                {
                    //取到包含input标签的元素,这里的sframename 就是topframe 和belowframe
                    // String sframename = doc.Window.Frames[i].Name;

                    // MessageBox.Show("iFrameName is: " + doc.Window.Frames[i].Name);                                    
                    if (doc.Window.Frames[i] != null)
                    {
                        //因为跨域问题会报无权限访问异常，所以这里要加判断
                        try
                        {
                            HtmlDocument hdtmp = doc.Window.Frames[i].Document;
                            wf.addRtb2ForThread("-----------------" + doc.Window.Frames[i].Name + "-----------------");
                            if (hdtmp != null)
                            {
                                wf.addRtb2ForThread(hdtmp.Body.InnerHtml + "");
                            }
                        }
                        catch (Exception ex)
                        {
                            wf.addRtb2ForThread("----------------" + doc.Window.Frames[i]
                                +"--  "+i + " -----包含跨域内容！------");
                            IHTMLWindow2 hw2 = doc.Window.Frames[i].DomWindow as IHTMLWindow2;
                            if (hw2 != null)
                            {                                                          
                                IHTMLDocument3 hd3 = CorssDomainHelper.GetDocumentFromWindow(hw2);
                                //if (hd3 != null)
                                //{
                                ////  //当需要操作时候,可以转化成IHTMLDocument3,就可以做各种操作
                                //    hd3.getElementById("kw").setAttribute("value", testValue);
                                //    hd3.getElementById("su").click();
                                //};
                                IHTMLDocument2 hdtmp = hd3 as IHTMLDocument2;
                                wf.addRtb2ForThread("-----HW2.frame name :  " + hdtmp.title + " ------ ");
                                wf.addRtb2ForThread("-----HW2.frame url :  " + hdtmp.url + " ------  ");
                                wf.addRtb2ForThread("----------------------------------------------------- ");  
                                if (hdtmp != null)
                                {
                                    wf.addRtb2ForThread(hdtmp.body.innerHTML + " ");
                                }
                            }
                        }
                        finally 
                        {

                        }
                    }                
                }
            }
 
        }

        //查找文本内容获得上期开奖数据,以"."分割,//  462853.9.4.5.......
        ////class=Font_Y>462853</B><B>期賽果期賽果</B></TD>
            ////<TD id=BaLL_No1 class=No_9 width=27>&nbsp;</TD>
            ////<TD id=BaLL_No2 class=No_4 width=27>&nbsp;</TD>
            ////<TD id=BaLL_No3 class=No_5 width=27>&nbsp;</TD>
            ////<TD id=BaLL_No4 class=No_10 width=27>&nbsp;</TD>
            ////<TD id=BaLL_No5 class=No_1 width=27>&nbsp;</TD>
            ////<TD id=BaLL_No6 class=No_2 width=27>&nbsp;</TD>
            ////<TD id=BaLL_No7 class=No_3 width=27>&nbsp;</TD>
            ////<TD id=BaLL_No8 class=No_7 width=27>&nbsp;</TD>
            ////<TD id=BaLL_No9 class=No_8 width=27>&nbsp;</TD>
            ////<TD id=BaLL_No10 class=No_6 width=27>&nbsp;</TD></TR>
        public static String getNowCodeByFrame_bak()
        {
           HtmlDocument doc = ForNaifen.getFrameByIframeName(wb.Document, "topFrame");
            if (doc == null)
                return null;

            HtmlDocument doc2 = getFrameByIframeName(doc, "mainFrame");
                if (doc2 == null)
                    return null;

                String page = doc2.Body.InnerHtml.ToString();
              if (page == null||page == "") 
                 return null;

              String sget = Apitool.FindStrByName(page, "id=UP_LID", "TR>");
            if (sget == null)
                return null;

            String reg = @"s=No_\d\d?";
            String st = TestReg.findRegNoshow(sget, reg);
            /*
             * //class=Font_Y>462853</B><B>期賽果
             * 
                ////s=No_9 
                ////s=No_10
               //s=No_2
             */
            st = st.Replace("s=No_", ".").Replace("\r\n", "");

            reg = @"_Y>[^<]+";
            //_Y> 462853 
            String sqishu = TestReg.findRegNoshow(sget, reg).Replace("_Y>", "").Trim();

            st = sqishu + st;
            return st;          
        }

          //北京賽車(PK10) </STRONG></TD>
        //<TD><STRONG class=red>今天輸贏：<SPAN id=win>0</SPAN></STRONG></TD>
        //<TD id=resultnum class=pk10 colSpan=3 align=right>
        //<STRONG class=c_blue><B id=timesold>470362</B>期赛果</STRONG>
        //<SPAN class="number num9"></SPAN><SPAN class="number num1"></SPAN><SPAN class="number num10"></SPAN>
        //<SPAN class="number num5"></SPAN><SPAN class="number num2"></SPAN><SPAN class="number num7"></SPAN>
        //<SPAN class="number num6"></SPAN><SPAN class="number num8"></SPAN><SPAN class="number num3"></SPAN>
        //<SPAN class="number num4"></SPAN></TD></TR>
        
        public static String getNowCodeByFrame()
        {
            HtmlDocument doc2 = ForNaifen.getFrameByIframeName(wb.Document, "rightLoader");
            if (doc2 == null)
                return null;

            String page = doc2.Body.InnerHtml.ToString();
            if (page == null || page == "")
                return null;

            String sget = Apitool.FindStrByName(page, "id=resultnum", "TR>");
            if (sget == null)
                return null;

            String reg = @"num\d\d?";
            String st = TestReg.findRegNoshow(sget, reg);

            st = st.Replace("num", ".").Replace("\r\n", "");

            reg = @"id=timesold>[^<]+";
            //id=timesold>470362
            String sqishu = TestReg.findRegNoshow(sget, reg).Replace("id=timesold>", "").Trim();

            st = sqishu + st;
            return st;     
        }


        ////看是否封盘
        //public static Boolean isFengPan_bak()
        //{
        //    HtmlDocument doc = ForNaifen.getFrameByIframeName(wb.Document, "topFrame");
        //    if (doc == null)
        //        return true;

        //    HtmlDocument doc2 = getFrameByIframeName(doc, "mainFrame");
        //    if (doc2 == null)
        //        return true;

        //    String page = doc2.Body.InnerHtml.ToString();
        //    if (page == null || page == "")
        //        return true;

        //    //<TD id=jeu_m_52_548 width=62>封盤</TD></TR>
        //    String sf = Apitool.FindStrByName(page,"id=jeu_m_52_548","</");
        //    if (sf == null)
        //    {
        //        return false;
        //    }
        //    if(sf.IndexOf("封盤")!=-1)
        //    {
        //        return true;
        //    }                  
        //    return false;          
        //}

        public static Boolean isFengPan()
        {
            HtmlDocument doc2 = ForNaifen.getFrameByIframeName(wb.Document, "rightLoader");
                                 
            if (doc2 == null)
                return true;

            String page = doc2.Body.InnerHtml.ToString();
         
            if (page == null || page == "")
                return true;


            //>距離封盤：<SPAN style="COLOR: #511e02" id=timeclose second="2">00:00</SPAN>

            //>距離封盤：<SPAN style="COLOR: #511e02" id=timeclose second="2">03:09</SPAN></TD>

            String sf = Apitool.FindStrByName(page, ">距離封盤", "/SPAN>");
            if (sf == null)
            {
                return true;
            }
            String sf2 = Apitool.FindStrByName(sf, ">", "<");
            if (sf2 == null)
            {
                return true;
            }
            if (sf2.Trim().Equals("00:00"))
            {
                return true;
            }
            else
                return false;



            //假封盘
			//  <TH colSpan=3>冠軍</TH></TR>
			//<TBODY sizset="8" sizcache08643451542791556="52">
			//<TR sizset="8" sizcache08643451542791556="52">
			//<TD class=fontBlue>大</TD>
			//<TD number="0" playType="010" jQuery1710435915343508742="61">
			//<TD class=amount>
			//           	<INPUT class=amount-input style="Bd; DISPLAY: inline" maxLength=9 jQuery1710435915343508742="1"> 
			//           	<SPAN style="DISPLAY: none" jQuery1710435915343508742="116">封盤</SPAN></TD></TR>
            
			//下面这个是真封盘,重点在DISPLAY: inline
           // <TD class=fontBlue>雙</TD>   
         //<TD class=amount>
         //<INPUT class=amount-input style="BORd; DISPLAY: none" maxLength=9 jQuery1710435915343508742="4"> 
         //<SPAN style="DISPLAY: inline" jQuery1710435915343508742="119">封盤</SPAN></TD></TR>

            //String sf = Apitool.FindStrByName(page, "colSpan=3>冠軍</TH></TR>", "</TR>");
            //if (sf == null)
            //{
            //    return false;
            //}
            //String sf2 = Apitool.FindStrByName(sf, "<SPAN style=\"DISPLAY: inline", ">封盤");
            //if (sf2 == null)
            //{
            //    return false;
            //}
            //else
            //    return true;
        }


        //对页面文本框根据id赋值, 
        public static void setFrameValue(HtmlDocument hd ,String name,String value)
        {
           // hd.GetElementById(name).InnerText= value;//文本框赋值,根据id赋值 
            if (hd == null) return;
            HtmlElement hte = hd.GetElementById(name);
            if (hte != null)
            {
                hte.InnerText = value;
               // hte.SetAttribute("selectedIndex", "1");//这个用在下拉框等下面
            }

        }

        //对页面按id提交表单
        private static void submitform(HtmlDocument hd,String name)
        {
            if (hd == null) return;
            HtmlElement hte = hd.GetElementById(name);//提交表单
            if (hte != null)
            {
                hte.InvokeMember("submit");
            }

           // HtmlElement btnSubmit = webBrowser.Document.All["submitbutton"];
            //btnSubmit.InvokeMember("click");
        }

        //对页面文本框根据name赋值, 
        public static void setFrameValueByName(HtmlDocument hd, String name, String value)
        {
             if (hd == null) return;
              // if (num == 1){  //////防止页面多次刷新页面执行
               // int at = 1;
               // int all = hd.Body.All.Count;

                HtmlElement he = hd.All[name];
                if (he != null)
                {
                    //wf.addRtb2ForThread("find name: " + name +"  TagName is: " + he.TagName + "  : name is:  " + he.Name);
                    he.Focus();
                    he.SetAttribute("value", value);
                    he.RemoveFocus();
                }

                //这个代码找不到大多name里的东西,所以无效
                //for (int i = 0; i < all;i++)
                //{
                //    htmlelement getelement = hd.all[i];

                //    //取到包含input标签的元素 //for test,
                //    //messagebox.show("tagname is: " + getelement.tagname);  
                //    wf.addrtb2forthread("tagname is: " + getelement.tagname  + "  : name is:  " +  getelement.name);

                //    if(getelement.tagname.toupper().tostring() == "input")
                //    {
                //        wf.addrtb2forthread("tagname input is: " + getelement.tagname);
                //        //根据input的name属性，找到该元素并赋值：给用户名输入框赋值
                //        if(getelement.name.tostring() == name)
                //        {
                //           hd.all[i].setattribute("value",value);

                //           wf.addrtb2forthread("tagname is find: " + name);
                //          // messagebox.show("tagname is: " + getelement.name);
                //        }                      
                //    } 
                //}
              //  num++;}
        }

        //新版本登录用
        private static void setLoginValueByThread(Object obj)
        {
            if (obj == null) return;
            Object[] bb = obj as Object[];
            HtmlDocument hd = bb[0] as HtmlDocument;
            LinkedList<String[]> lls = bb[1] as LinkedList<String[]>;

            //新版
              //<input type="radio" checked="checked" id="shenjiban" name="banbeng" value="1">
              //<input type="radio" id="chuantongban" name="banbeng" value="2"> 
            HtmlElement het = hd.All["chuantongban"];
            if (het != null)
            {
                String sch = het.GetAttribute("checked");
                if (sch != null && sch.ToLower().Equals("false"))
                {
                    het.InvokeMember("click");                  
                }
            }

            //安全控件
            // <input type='checkbox' id='sec' checked='checked' />启用安全控件</label>
            het = hd.GetElementById("sec");
            if(het != null)
            {
                //het.SetAttribute("checked", "");
                String sch = het.GetAttribute("checked");
                if (sch!= null && sch.ToLower().Equals("true"))
                {
                    het.InvokeMember("click");
                    //ischeckSafe = true;
                    Thread.Sleep(1000);
                }
            }

            // int ik = lls.Count;
            // setWebbroConfirmDlg(hd);
            // wf.addRtb2ForThread("lls .count is:  " + lls.Count);

            for (int ik = lls.Count -1; ik >=0; ik--)
            {
                String[] str = lls.ElementAt(ik);
                String name = str[0];
                String value = str[1];
                HtmlElement he = hd.All[name];
                // String nid = name.Replace("jeuM", "jeu_m");
                // HtmlElement he2 = hd.GetElementById(nid);   

                if (he != null)
                {
                    he.Focus(); //如果没有focus,则因为页面js,控件可能出问题，
                    he.SetAttribute("value", value);
                    Thread.Sleep(500);
                    he.RemoveFocus();
                }
                else
                {
                    wf.addRtb2ForThread(" ========  cant find name: ==========" + name);
                   // wf.addRtbNfft(" ========  cant find name: ==========" + name);
                }
            }
            // Thread.Sleep(2000);
            //因为验证码问题暂时不登陆
           // submitFormByName(hd, "Submit");         
        }


        ////在线程中操作页面，否则速度执行太快会出错！
        ////通过obj是个数组，数组第一项传递doc,第二项是个string[2]链表,string[0][1],0为name，1为value
        //public static void setFrameValueByNameThread(Object obj)
        //{
        //    if (obj == null) return;
        //    Object[] bb = obj as Object[];
        //    HtmlDocument hd = bb[0] as HtmlDocument;

        //    LinkedList<String[]> lls = bb[1] as LinkedList<String[]>;
           
        //    //是否自动投注，需要保存单号
        //    ForNaiTouzhu ftz = null;
        //    if(bb.Count() > 2)
        //    {
        //        ftz = bb[2] as ForNaiTouzhu;                                        
        //    }
            
        //   // int ik = lls.Count;
        //   // setWebbroConfirmDlg(hd);

        //   // wf.addRtb2ForThread("lls .count is:  " + lls.Count);
        //    foreach(String[] str in lls)
        //    {
        //        String name = str[0];
        //        String value = str[1];
        //        HtmlElement he = hd.All[name];
        //       // String nid = name.Replace("jeuM", "jeu_m");
        //       // HtmlElement he2 = hd.GetElementById(nid);   

        //        if (he != null)
        //        {             
        //            if (ftz != null && (value.Equals("1") || value.Equals("0")))
        //            {
        //                wf.addRtb2ForThread(" ========  投注额小于2，本次不投！ ==========");
        //                wf.addRtbNfft(" ========  投注额小于2，本次不投！ ==========");                       
        //                  //存储投注信息
        //                ftz.sTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
        //                    ForNaiStrategy.llmy.AddLast(ftz);
        //                    wf.addRtb2ForThread(ftz.ToString());
        //                    wf.addRtbNfft(ftz.ToString());
        //                    return;
        //            }
        //           // wf.addRtb2ForThread("find name: " + name + "  TagName is: " + he.TagName + "  : name is:  " + he.Name);
                    
        //            he.Focus();
        //            he.SetAttribute("value", value);
        //            he.RemoveFocus();
        //            Thread.Sleep(1000);
        //        }
        //        else
        //        {
        //            wf.addRtb2ForThread(" ========  cant find name: ==========" + name );
        //            wf.addRtbNfft(" ========  cant find name: ==========" + name);
        //        }
        //        //if (he2 != null)
        //        //{
        //        //    wf.addRtb2ForThread("find id  name: " + nid );
        //        //    //he.Focus();
        //        //    //he.SetAttribute("value", value);
        //        //    //he.RemoveFocus();
        //        //    //Thread.Sleep(1500);
        //        //}
        //        //else
        //        //{
        //        //    wf.addRtb2ForThread(" ========  cant find id name: ==========" + nid);
        //        //}

        //    }
        //  // Thread.Sleep(2000);

        //    isTestSubmit = wf.getCheckBoxNfDebug();

        //    if (ftz != null && isTestSubmit)
        //    {
        //        wf.addRtb2ForThread("=====测试，不真正提交！=====");
        //        wf.addRtbNfft("=====测试，不真正提交！=====");
        //    }
        //    else if (ftz != null)
        //    {
        //        submitFormByName(hd, "confirm");
        //    }
        //    else 
        //    {   //用户登录用这里
        //        submitFormByName(hd, "Submit");
        //    }
			
        //    //这里应判断投注是否成功，add code here,by wind,12.18
            
        //    if (ftz != null)
        //    {
        //       //存储投注信息
        //        ftz.sTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
        //        ftz.idannum = ForNaiStrategy.iDanCpu;
        //        ftz.itwonum = ForNaiStrategy.iTwoCpu;
        //        ForNaiStrategy.llmy.AddLast(ftz);

        //        wf.setRtbNfText("");//清空


        //        wf.addRtbNfft(Str_menu);
        //        wf.addRtbNfft(Str_split);
        //        int lcount = ForNaiStrategy.llmy.Count;
        //        int imin = Math.Min(lcount, 5);
        //        //这里要打印5个
        //        for (int im = imin-1; im >=0; im--)
        //        {
        //            ForNaiTouzhu  ftmp = ForNaiStrategy.llmy.ElementAt(lcount - im - 1);
        //            soutFntz(ftmp);
        //        }

        //        //wf.addRtb2ForThread(ftz.ToString());
        //        //wf.addRtb2ForThread("目前单数倍率: " + ForNaiStrategy.iDanCpu + "   双数倍率： " + 
        //        //    ForNaiStrategy.iTwoCpu);

        //        //wf.addRtbNfft(ftz.ToString());
        //        //wf.addRtbNfft("目前单数倍率: " + ForNaiStrategy.iDanCpu + "   双数倍率： " +
        //        //    ForNaiStrategy.iTwoCpu);
        //        //wf.addRtbNfft("---------------------------------------------------------------");
        //    }          
        //}

        //在线程中设置html element的vaule,并提交
        public static void setFrameValueByHtmlElem(Object obj)
        {
            if (obj == null) return;
            Object[] bb = obj as Object[];

            HtmlDocument hdoc = bb[0] as HtmlDocument;
            LinkedList<HtmlElement> lhe = bb[1] as LinkedList<HtmlElement>;
            LinkedList<String> lls = bb[2] as LinkedList<String>;

            for (int it = 0; it < lhe.Count; it++)
            {
                HtmlElement he = lhe.ElementAt(it);
                String sv = lls.ElementAt(it);
                if (he != null)
                {
                    //he.Focus();
                    he.SetAttribute("value", sv);
                    //he.RemoveFocus();
                    Thread.Sleep(1000);
                }
                else
                {
                    wf.addRtb2ForThread(" ========  cant set value ==========" + sv);
                }
            }
            clickItemById(hdoc, "submit",1);
        }

        //在线程中设置naifen 投注值的vaule,并提交
        public static void submitNfTzByHtmlElem(Object obj)
        {
            if (obj == null) return;
            try
            {
                Object[] bb = obj as Object[];

                HtmlDocument hdoc = bb[0] as HtmlDocument;
                LinkedList<HtmlElement> lhe = bb[1] as LinkedList<HtmlElement>;
                LinkedList<String> lls = bb[2] as LinkedList<String>;

                //是否自动投注，需要保存单号
                ForNaiTouzhu ftz = null;
                ftz = bb[3] as ForNaiTouzhu;

                isTestSubmit = wf.getCheckBoxNfDebug();

                for (int it = 0; it < lhe.Count; it++)
                {
                    HtmlElement he = lhe.ElementAt(it);
                    String sv = lls.ElementAt(it);
                    if (he != null)
                    {
                        if (ftz != null && (sv.Equals("1") || sv.Equals("0")))
                        {
                            wf.addRtb2ForThread(" ========  投注额小于2，本次不投！ ==========");
                            wf.addRtbNfft(" ========  投注额小于2，本次不投！ ==========");
                            //存储投注信息
                            ftz.sTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                            ftz.idannum = ForNaiStrategy.iDanCpu;
                            ftz.itwonum = ForNaiStrategy.iTwoCpu;
                            ForNaiStrategy.llmy.AddLast(ftz);
                            wf.addRtb2ForThread(ftz.ToString());
                            wf.addRtbNfft(ftz.ToString());
                            return;
                        }
                        if (!isTestSubmit)
                        {
                            //he.Focus();
                            he.SetAttribute("value", sv);
                            //he.RemoveFocus();
                            Thread.Sleep(500);
                        }
                    }
                    else
                    {
                        wf.addRtb2ForThread(" ========  cant set value ==========" + sv);
                    }
                }

                if (ftz != null)
                {
                    wf.setRtbNfText("");//清空
                    if (isTestSubmit)
                    {
                        wf.addRtb2ForThread("=====测试，不真正提交！=====");
                        wf.addRtbNfft("=====测试，不真正提交！=====");
                    }
                    else
                    {
                        clickItemById(hdoc, "submit", 1);
                    }
                }

                if (ftz != null)
                {
                    //存储投注信息
                    ftz.sTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                    ftz.idannum = ForNaiStrategy.iDanCpu;
                    ftz.itwonum = ForNaiStrategy.iTwoCpu;
                    ForNaiStrategy.llmy.AddLast(ftz);


                    wf.addRtbNfft(Str_menu);
                    wf.addRtbNfft(Str_split);
                    int lcount = ForNaiStrategy.llmy.Count;
                    int imin = Math.Min(lcount, 5);
                    //这里要打印5个
                    for (int im = 0; im < imin; im++)
                    {
                        ForNaiTouzhu ftmp = ForNaiStrategy.llmy.ElementAt(lcount - im - 1);
                        soutFntz(ftmp);
                    }
                    //wf.addRtb2ForThread(ftz.ToString());
                    //wf.addRtb2ForThread("目前单数倍率: " + ForNaiStrategy.iDanCpu + "   双数倍率： " + 
                    //    ForNaiStrategy.iTwoCpu);

                    //wf.addRtbNfft(ftz.ToString());
                    //wf.addRtbNfft("目前单数倍率: " + ForNaiStrategy.iDanCpu + "   双数倍率： " +
                    //    ForNaiStrategy.iTwoCpu);
                    //wf.addRtbNfft("---------------------------------------------------------------");
                    if (!isTestSubmit)
                    {

                        //这里应判断投注是否成功，add code here,by wind,12.18
                        Thread.Sleep(3000);
                        Boolean isTouzhuok = true;
                        HtmlDocument doc4 = null;
                        if (wb.InvokeRequired)
                        {
                            wb.Invoke((MethodInvoker)delegate
                            {
                                doc4 = getFrameByIframeName(wb.Document, "leftLoader");
                            });
                        }
                        else
                        {
                            doc4 = getFrameByIframeName(wb.Document, "leftLoader");
                        }
                        if (doc4 != null)
                        {
                            isTouzhuok = false;
                            ////<TH class=inner_text colSpan=2>下註結果反饋</TH></TR>
                            //// <A class="back btn_m elem_btn cancel_btn" href="javascript:void(0)" jQuery17109490390707430416="7">返 囬</A> </DIV></TD></TR>
                            ////<TD class="inner_text align-c bold  qishu" colSpan=2>471129期</TD></TR>
                            ////<TBODY id=s-list>
                            ////<TR>
                            ////<TD colSpan=3>
                            ////<P>註單號:<SPAN class=greener>00016578</SPAN></P>
                            ////<P class=text-i-em3><SPAN class=bluer>第三名 雙</SPAN>&nbsp; @ &nbsp;<B class=red>1.983</B></P>
                            ////<P>下注额:<SPAN class=black>2</SPAN></P>
                            ////<P>可贏額:<SPAN class=black>1</SPAN></P></TD></TR>
                            ////<TR>
                            //String stm = Apitool.FindStrByName(doc4.Body.InnerHtml.ToString(), "下註結果反饋", "");
                            //if (stm != null)
                            //{
                            //    String sts = Apitool.FindStrByName(stm, "註單號", "SPAN><");
                            //    if (sts != null)
                            //    {
                            //        String stv = Apitool.FindStrByName(sts, ">", "<");
                            //        if (stv != null && stv.Length > 3)
                            //        {
                            //            isTouzhuok = true;
                            //        }
                            //    }
                            //}

                            //点击返回按钮
                            HtmlElement he4 = findElmByOutHtml(doc4, "back btn_m elem_btn cancel_btn", "A", "");
                            if (he4 != null)
                            {
                                he4.InvokeMember("click");
                            }

                            //<TD class=inner_text>已下金額</TD>
                            //<TD id=total_amount class=total_amount>12</TD></TR>
                               Thread.Sleep(6000);
                               if (wb.InvokeRequired)
                                {
                                    wb.Invoke((MethodInvoker)delegate
                                    {
                                        doc4 = getFrameByIframeName(wb.Document, "leftLoader");
                                    });
                                }
                                else
                                {
                                    doc4 = getFrameByIframeName(wb.Document, "leftLoader");
                                }
                               String value = "";
                             if (doc4 != null)
                             { 
                                value = getBetMoney(doc4.Body.InnerHtml.ToString());
                             }
                            if (!value.Equals(""))
                            {
                                if (Int32.Parse(value) >= (Int32.Parse(ftz.sBeilv) * 5))
                                {//已经下注总额大于等于 5倍的投注额
                                    isTouzhuok = true;
                                }
                            }
                        }

                        if (isTouzhuok)
                        {
                            wf.addRtbNfft(ftz.sQihao + " 期投注成功！");
                            saveFntz(ftz);
                            logFile();
                        }
                        else
                        {
                            //投注失败就删除
                            wf.addRtbNfft(ftz.sQihao + " 期投注失败！");
                            ForNaiStrategy.llmy.RemoveLast();
                            ForNaiStrategy.llKaiJiang.RemoveLast();
                           //idancpu 和 itwocpu要对应减掉,投注失败，回退
                            ForNaiStrategy.loadCpuValue();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                wf.addRtbNfft(ex.ToString());
                wf.addRtb2ForThread(ex.ToString());
            }
            finally 
            {

            }
        }


        //frame的自动点击弹出确认或弹出提示,应该在documentcomplete触发后执行一次.否则就每次执行时来一次！
        public static void setWebbroConfirmDlg(HtmlDocument hd2)
        {
            if (hd2 == null) return;
            try
            {
                IHTMLDocument2 vDocument = (IHTMLDocument2)(hd2.DomDocument);//需要对执行该script的页面做操作！
                vDocument.parentWindow.execScript("function confirm(str){return true;} ", "javascript"); //弹出确认
                vDocument.parentWindow.execScript("function alert(str){return true;} ", "javaScript");//弹出提示

                //下面是应对所有frame界面
                //foreach (IHTMLElement vElement in vDocument.all)
                //    if (vElement.tagName.ToUpper() == "FRAME")
                //    {
                //        IHTMLFrameBase2 vFrameBase2 = vElement as IHTMLFrameBase2;
                //        vFrameBase2.contentWindow.execScript(
                //            "function alert(str){confirm('[' + str + ']');}", "javaScript");
                //    } 

            }
            catch (Exception ex)
            {
                wf.addRtb2ForThread(ex.ToString());
            }
        }


        /// <summary>
        /// frame的自动点击弹出确认或弹出提示
        /// </summary>
        /// <param name="wbro"></param>
        public static void setWebbrowserConfirmDlg(WebBrowser wbro)
        {
            try
            {
                IHTMLDocument2 vDocument = (IHTMLDocument2)(wbro.Document.DomDocument);
                vDocument.parentWindow.execScript("function confirm(str){return true;} ", "javascript"); //弹出确认
                vDocument.parentWindow.execScript("function alert(str){return true;} ", "javaScript");//弹出提示
            }
            catch (Exception ex)
            {
                wf.addRtb2ForThread(ex.ToString());
            }
        }




        //对页面按name提交表单
        public static void submitFormByName(HtmlDocument hd, String name)
        {
            if (hd == null) return;

            HtmlElement he = hd.All[name];
            if (he != null)
            {
               // wf.addRtb2ForThread("find name: " + name + " submitName is: " + he.TagName + "  : name is:  " + he.Name);
                //he.Focus();
                he.InvokeMember("click");
            }

            //for (int i = 0; i < all; i++)
            //{
            //    HtmlElement GetElement = hd.All[i];


            //    wf.addRtb2ForThread("GetElement name is: " + GetElement.Name);
            //    //根据input的Name属性，找到提交按钮并执行动作
            //    if (GetElement.Name.ToString() == name)
            //    {
            //        wf.addRtb2ForThread("提交按钮找到：" + name);
            //        //过滤点击页面中相同"name=Submit"的元素
            //        if (a == 1)
            //        {
            //            hd.All[i].InvokeMember("click");
            //        }
            //        a++;
            //    }
            //}

            // HtmlElement btnSubmit = webBrowser.Document.All["submitbutton"];
            //btnSubmit.InvokeMember("click");
        }

        //点击页面id元素
        public static void clickItemById(HtmlDocument hd, String id, int delaytime)
        {
            if (hd == null || id == null|| delaytime < 0)
                return;
            if (id.Equals(""))
                return;
            HtmlElement he = hd.GetElementById(id);
            if (he != null)
            {
                he.InvokeMember("click");
            }
        }


        //this.webBrowser1.Url= new System.Uri("url地址", System.UriKind.Absolute);


        //给一般不是框架网页中的文本框赋值
        //webbrowser1.document.getelementbyid("文本框id").innertext= "weiling";//文本框赋值根据id赋值
        //或者：this.webbrowser1.document.all["文本框name"].setattribute("value", "0924");//文本框赋值根据name赋值


        //表单提交，也可以看成是一个点击事件
        //htmlelement form= webbrowser1.document.getelementbyid("formid");//提交表单
        //form.invokemember("submit");


        //            /// <summary>  
        //    /// 获取WebBrowser指定的图片  
        //    /// </summary>  
        //    /// <param name="webBrowser">需要获取图片的WebBrowser</param>  
        //    /// <param name="imgID">指定的图片的id(优先查找指定id)</param>  
        //    /// <param name="imgSrc">指定的图片的src，支持模糊查询</param>  
        //    /// <param name="imgAlt">指定的图片的src， 支持模糊查询</param>  
        //    /// <returns></returns>  
        //    public static Image GetRegCodePic(ref WebBrowser webBrowser, String imgID, String imgSrc, String imgAlt)  
        //    {

        //        HtmlDocument doc = (HtmlDocument)webBrowser.Document.DomDocument;
        //        HtmlElement body = (HtmlElement)doc.Body.;  
        //        IHtmlControlRange rang = (IHtmlControlRange)body.createControlRange();  
        //        IHTMLControlElement img;  
               
        //        // 如果没有图片的ID,通过Src或Alt中的关键字来取  
        //        if (imgID.Length == 0)  
        //        {  
        //            Int32 ImgNum = GetPicIndex(ref webBrowser, ref imgSrc, ref imgAlt);  
               
        //            if (ImgNum == -1)  
        //                return null;  
               
        //            img = (IHTMLControlElement)webBrowser.Document.Images[ImgNum].DomElement;  
        //        }  
        //        else 
        //            img = (IHTMLControlElement)webBrowser.Document.All[imgID].DomElement;  
               
        //        rang.add(img);  
        //        rang.execCommand("Copy", false, null);  
        //        Image regImg = Clipboard.GetImage();  
        //        Clipboard.Clear();  
        //        return regImg;  
        //    }  
   
        ///// <summary>  
        ///// 获取WebBrowser指定图片的索引  
        ///// </summary>  
        ///// <param name="webBrowser">指定的WebBrowser</param>  
        ///// <param name="imgSrc">指定的图片src，支持模糊查询</param>  
        ///// <param name="imgAlt">指定的图片alt，支持模糊查询</param>  
        ///// <returns></returns>  
        //public static Int32 GetPicIndex(ref WebBrowser webBrowser, ref String imgSrc, ref String imgAlt)
        //{
        //    IHTMLImgElement img;

        //    // 获取所有的Image元素  
        //    for (Int32 i = 0; i < webBrowser.Document.Images.Count; i++)
        //    {
        //        img = (IHTMLImgElement)webBrowser.Document.Images[i].DomElement;

        //        if (imgAlt.Length == 0)
        //        {
        //            if (img.src.IndexOf(imgSrc) >= 0)
        //                return i;
        //        }
        //        else
        //        {
        //            if (imgSrc.Length == 0)
        //            {
        //                // 当imgSrc为空时，只匹配imgAlt  
        //                if (img.alt.IndexOf(imgAlt) >= 0)
        //                    return i;
        //            }
        //            else
        //            {
        //                // 当imgSrc不为空时，匹配imgAlt和imgSrc任意一个  
        //                if (img.alt.IndexOf(imgAlt) >= 0 || img.src.IndexOf(imgSrc) >= 0)
        //                    return i;
        //            }
        //        }
        //    }
        //    return -1;
        //}


         public void getPic(WebBrowser wtb)
        { 
            try
            {
                if (wb == null)
                {
                    wb = wtb;
                }

                //HtmlDocument hdtmp = getFrameByIframeName(wb.Document, "xxxFrame");//topFrame
                //if (hdtmp != null)
                //{
                //   // setFrameValueByName(hdtmp, "loginName", sname);
                //   // setFrameValueByName(hdtmp, "loginPwd", spwd);
                //   // submitFormByName(hdtmp, "Submit");
                //}


               // String spic = "http://hh2.ts9-bdvg.9761dtqs.com:12014/user/CodeVali.aspx";
               // String spic = "http://images.cnblogs.com/cnblogs_com/ivanyb/201111/201111251245395649.png";
               // wb.Navigate(spic);
               // getImageCode(spic);

                wf.addRtb2ForThread(url + " " + " get pic ok!");
            }
            catch (Exception e)
            {
                wf.addRtb2ForThread(e.StackTrace);
            }
            finally
            {

            }
        }

        //执行下单程序
         public void doBet(WebBrowser wtb)
         {
             //mainFrame里面下单，选择
             try
             {
                 if (wb == null)
                 {
                     wb = wtb;
                 }

                 //HtmlDocument hdtmp = getFrameByIframeName(wb.Document, "xxxFrame");//topFrame
                 //if (hdtmp != null)
                 //{
                 //   // setFrameValueByName(hdtmp, "loginName", sname);
                 //   // setFrameValueByName(hdtmp, "loginPwd", spwd);
                 //   // submitFormByName(hdtmp, "Submit");
                 //}

                 HtmlDocument doc2 = getFrameByIframeName(wb.Document, "rightLoader");
                 if (doc2 != null)
                 {
                     // HtmlDocument doc2 = getFrameByIframeName(doc,"mainFrame");
                     // if (doc2 != null)
                      {
                          //wf.addRtb2ForThread("doc2 is ok ===============");

                          String sn1 = "\"49\"";
                          String sn2 = "\"50\"";
                          String value1 = "2";
                          String value2 = "3";

                          HtmlElement he = findElmByOutHtml(doc2,sn1,"INPUT","amount-input");
                          HtmlElement he2 = findElmByOutHtml(doc2,sn2,"INPUT","amount-input");

                          LinkedList<HtmlElement> lhe = new LinkedList<HtmlElement>();
                          LinkedList<String> lls = new LinkedList<String>();

                          if (he != null)
                          {
                              lhe.AddLast(he);
                              lls.AddLast(value1);
                          }
                          else
                          {
                              wf.addRtb2ForThread(" ======  cant find HtmlElement name ==========" + sn1);
                          }
                          if(he2!=null)
                          {
                              lhe.AddLast(he2);
                              lls.AddLast(value2);                           
                          }
                          else
                          {
                              wf.addRtb2ForThread(" ======  cant find HtmlElement name ==========" + sn2);
                          }


                          ForNaifen.setWebbroConfirmDlg(doc2);
                          ForNaifen.setWebbroConfirmDlg(wb.Document); 
                       
                          //应该放到线程中去执行
                          //Thread thcntv = new Thread(new ThreadStart(ForNaifen.setFrameValueByName));
                          // Thread th = new Thread(new ParameterizedThreadStart(ForNaifen.setFrameValueByNameThread));  
                          
                          Thread th = new Thread(new ParameterizedThreadStart(ForNaifen.setFrameValueByHtmlElem));  
                          th.IsBackground = true;
                          
                          //设置要设置的值
                          //String[] s1 = { "jeuM_53_575", "2" };
                          //String[] s2 = { "jeuM_53_574", "2" };
                          //LinkedList<String[]> ll = new LinkedList<String[]>();
                          //ll.AddFirst(s1);
                          //ll.AddFirst(s2);
                          //Object[] obj = new Object[]{doc2,ll};

                          Object[] obj = new Object[] {doc2,lhe,lls};
                          th.Start(obj);

                          
                         //setFrameValueByName(doc2, "jeuM_52_582", "2");//8
                         // setFrameValueByName(doc2, "jeuM_52_594", "2");                        
                         // submitFormByName(doc2, "confirm");
                      }
                 }
                 wf.addRtb2ForThread(url + " " + " do bet ok!");
             }
             catch (Exception e)
             {
                 wf.addRtb2ForThread(e.StackTrace);
             }
             finally
             {

             }        
         }

         ////执行下单程序,旧版本
         //public void doBetByStrategy_bak(WebBrowser wtb)
         //{
         //    try
         //    {
         //        if (wb == null)
         //        {
         //            wb = wtb;
         //        }
         //        HtmlDocument doc = getFrameByIframeName(wb.Document, "topFrame");
         //        if (doc != null)
         //        {
         //            HtmlDocument doc2 = getFrameByIframeName(doc, "mainFrame");
         //            if (doc2 != null)
         //            {
         //             wf.addRtb2ForThread("doc2 is ok ===============");

         //             String st = ForNaifen.getNowCodeByFrame();                     
         //             if (st == null || st.Equals(""))
         //             {
         //                 wf.addRtb2ForThread("获得上期号码为空！");
         //                 wf.addRtbNfft("未获得上期开奖号码，本期投注中止！");
         //                 return;
         //             }
         //             //非测试时，看是否封盘，封盘不投注
         //             if (ForNaifen.isFengPan() && (!wf.getCheckBoxNfDebug()))
         //             {
         //                 return;
         //             }

         //             wf.addRtb2ForThread("上期数据为:  " + st);
         //             wf.addRtbNfft("上期数据为:  " + st);
                                         

         //            //如果上期号码已经存在，说明已经投过了，就不投注
         //            Boolean  issave = ForNaiStrategy.setKaiJiang(st);
         //            if (!issave)
         //            {
         //                wf.addRtb2ForThread("=====这期已经投注过了！....=====");
         //                wf.addRtbNfft("=====这期已经投注过了！=====");
         //                if (!isForceBet&&(!(wf.getCheckBoxNfDebug())))
         //                {
         //                    wf.addRtbNfft("---------------------------------------------------------------");
         //                    return;
         //                }
         //            }
         //            isForceBet = false;

         //              //fortest 根据单双1来
         //           //  String [] strcode = ForNaiStrategy.getCodeByBefore(st, ForNaiStrategy.STRATE_BIG_SMALL);
         //           //  if (strcode == null) 
         //           //  {
         //              //   wf.addRtb2ForThread("根据策略生成号码为空！！！");
         //               //  return;
         //            // }
         //             //   //for test,全部填2

         //               int svalue = ForNaiStrategy.getNBeiNum(ForNaiStrategy.STRATE_BIG_SMALL);                       
                        
         //               LinkedList<String[]> ll = ForNaiStrategy.getvalueForEachCode(svalue);
						
         //              if (ll == null)
         //               {
         //                  wf.addRtb2ForThread("生成号码出错！！！");
         //                  wf.addRtbNfft("生成号码出错！！！");
         //                  return;
         //              }
         //             ForNaifen.setWebbroConfirmDlg(doc2);
                      
         //                //保存投注信息
         //             ForNaiTouzhu ftz = new ForNaiTouzhu();
         //             if (svalue >= 0)
         //             {
         //                 ftz.sOnetwo = "单";
         //                 ftz.sBeilv = svalue+"";
         //             }
         //             else 
         //             {
         //                 ftz.sOnetwo = "双";
         //                 int stmp = 0 - svalue;
         //                 ftz.sBeilv = stmp + "";
         //             }
         //             int shqh = Int32.Parse(ForNaiStrategy.llKaiJiang.Last());
         //                ftz.sQihao = (shqh +1)+"";
                      

         //                //应该放到线程中去执行
         //                Thread th = new Thread(new ParameterizedThreadStart(ForNaifen.setFrameValueByNameThread));
         //                th.IsBackground = true;

         //                Object[] obj = new Object[] { doc2, ll, ftz};
         //                th.Start(obj);
         //                wf.addRtb2ForThread(url + " " + " do bet strategy ok!");
         //            }
         //        }
               
         //    }
         //    catch (Exception e)
         //    {
         //        wf.addRtb2ForThread(e.StackTrace);
         //    }
         //    finally
         //    {
         //    } 
         //}

        //下单程序新版本,15.1.20
         public void doBetByStrategy(WebBrowser wtb)
         {
             try
             {
                 if (wb == null)
                 {
                     wb = wtb;
                 }

                // Monitor.Enter(ObjLockNf);
                 {
                     HtmlDocument doc2 = getFrameByIframeName(wb.Document, "rightLoader");
                     if (doc2 == null)
                         return;

                         //HtmlDocument doc2 = getFrameByIframeName(doc, "mainFrame");
                         wf.addRtb2ForThread("doc2 is ok ===============");

                         String st = ForNaifen.getNowCodeByFrame();
                         if (st == null || st.Equals(""))
                         {
                             //如果不是自动投注就不切换
                             if (wf.getcbnfAutopage()&&WindForm.isRunNaifen && iturnpage==0)
                             {
                                 //点击一次主页面北京赛车链接
                                 //<A id=pk10_sys class="switch  nav_longstring switch-on" href="javascript:void(0)" 
                                 //subnav="bothSides_pk10">北京賽車(PK10)</A>
                                 //<A title="" href="javascript:void(0)" nav="history">結算報表</A>
                                 //去到页面历史开奖,
                                 HtmlElement he = null;
                                 iturnpage = 1;
                                 he = findElmByOutHtml(wb.Document, ">北京賽車(PK10)<", "A", "");
                                 if (he != null)
                                 {
                                     he.InvokeMember("click");
                                     //5秒后重新进入该函数
                                     if (ti == null)
                                     {
                                         ti = new System.Windows.Forms.Timer();
                                         ti.Tick += new EventHandler(tiEvent);
                                     }
                                     ti.Interval = 5000;
                                     ti.Start();
                                     return;

                                 }
                                 else
                                 {
                                     wf.addRtb2ForThread("获得上期号码为空！");
                                     wf.addRtbNfft("未获得上期开奖号码，本期投注中止！");
                                     return;
                                 }
                             }
                             else
                             {
                                 wf.addRtb2ForThread("获得上期号码为空！");
                                 wf.addRtbNfft("未获得上期开奖号码，本期投注中止！");
                                 return;
                             }
                         }
                         
                         //非测试时，看是否封盘，封盘不投注
                         if (ForNaifen.isFengPan() && (!wf.getCheckBoxNfDebug()))
                         {
                             wf.addRtbNfft("网站现在已封盘. ");
                             return;
                         }
                         
                         wf.addRtb2ForThread("上期数据为:  " + st);
                         String st2 = st;
                         int ist2 = st.IndexOf(".");
                         if(ist2 != -1)
                         {
                             st2 = st.Substring(0,ist2)+" ("+st.Substring(ist2+1)+")";
                         }
                         wf.addRtbNfft("上期数据为:  " + st2);
                         


                         //如果上期号码已经存在，说明已经投过了，就不投注
                         Boolean issave = ForNaiStrategy.setKaiJiang(st);

                         //这里如果是补投的话有点问题，因为网页刷新时间不一致，导致上期赛果没出来前，当前期已经可投
                         //入上期是471455，而实际可投的是471457，10秒后471455才变成471456

                         //STRONG class=c_blue><B id=timesold>471455</B>期赛果</STRONG> <SPAN class="number num7">
                         //<STRONG id=timesnow class=green>471457</STRONG>期 <B class=blue_h>兩面盤</B></TD>
                         //自动的话没问题
                         if (!issave)
                         {
                             wf.addRtb2ForThread("=====这期已经投注过了！....=====");
                             wf.addRtbNfft("=====这期已经投注过了！=====");
                             if (!isForceBet && (!(wf.getCheckBoxNfDebug())))
                             {
                                 wf.addRtbNfft("-------------------------------------------------------------");
                                 return;
                             }
                         }
                         isForceBet = false;

                         //fortest 根据单双1来
                         //  String [] strcode = ForNaiStrategy.getCodeByBefore(st, ForNaiStrategy.STRATE_BIG_SMALL);
                         //  if (strcode == null) 
                         //  {
                         //   wf.addRtb2ForThread("根据策略生成号码为空！！！");
                         //  return;
                         // }
                         //  

                         ForNaiStrategy.backCpuValue();//保存参数供回退
                         //int svalue = ForNaiStrategy.getNBeiNum(ForNaiStrategy.STRATE_BIG_SMALL);
                        
                         //获得上2期开奖号
                          String skpre1 = ForNaiStrategy.llKaiJiang.Last();
                         
                          String skjpre1val = ForNaiStrategy.htKai[skpre1]+"";
                          String skjpre2val = "";
                          if(ForNaiStrategy.llKaiJiang.Count>1)
                          {
                          	String skpre2 = ForNaiStrategy.llKaiJiang.ElementAt(ForNaiStrategy.llKaiJiang.Count-2);
                          	skjpre2val = ForNaiStrategy.htKai[skpre2]+"";


                            //这里判断是否中断1期以上，有中断的话播放声音提示，停止自动投注
                            Boolean isbreakOne = false;
                            if ((Int32.Parse(skpre1) - Int32.Parse(skpre2)) != 1)
                            {
                                isbreakOne = true;
                            }
                            if (isbreakOne)
                            {
                                wf.addRtbNfft("-------------------------------------------------------------------------");
                                wf.addRtbNfft("因投注中断了1期以上，投注停止！请模拟投注获得参数后，再清除数据重新开始！");
                                ForNaiStrategy.llKaiJiang.RemoveLast();
                                ForNaiStrategy.loadCpuValue();

                                if (WindForm.isRunNaifen)
                                {
                                    wf.btnnfstop_Click(null, null);
                                }
                                String mp3file = System.Environment.CurrentDirectory + @"\alarm.mid";
                                Apitool.playMp3(mp3file);
                                return;
                            }
                          }
                                             
					    int svalue = ForNaiStrategy.getNBeiNum2(ForNaiStrategy.STRATE_BIG_SMALL2,skpre1,skjpre1val,skjpre2val);
                        					    
                        //根据开奖号判断投单投双
                        if (!(skjpre1val.Equals("")))
                        {
                            String[] shs = skjpre1val.Split('.');
                            int ifirdan = ForNaiStrategy.isDan(shs[0]);//判断第一个数是否是单

                            if (ifirdan == 0)
                            {
                                if (svalue > 0)
                                {//是左边投双，右边单，则实际投双
                                    svalue = 0 - svalue;
                                }
                                else if (svalue < 0)
                                {//是左边投双，右边单，则实际投单
                                    svalue = 0 - svalue;
                                }
                            }
                            else if (ifirdan == 1)
                            {
                                if (svalue > 0)
                                {//是左边投单，右边双，则不变
                                   //投单，则不变
                                }
                                else if(svalue < 0)
                                {  //投双，也不变
                                }
                            }
                        }

                         LinkedList<String[]> ll = ForNaiStrategy.getvalueForEachCode(svalue);

                         if (ll == null)
                         {
                             wf.addRtb2ForThread("生成号码出错！！！");
                             wf.addRtbNfft("生成号码出错！！！");
                             ForNaiStrategy.llKaiJiang.RemoveLast();
                             //idancpu 和 itwocpu要对应减掉,投注失败，回退
                             ForNaiStrategy.loadCpuValue();
                             return;
                         }

                         LinkedList<HtmlElement> lhe = new LinkedList<HtmlElement>();
                         LinkedList<String> lls = new LinkedList<String>();


                     //这里有两种情况
                         //1 <INPUT class=amount-input maxLength=9 jQuery171040092058461596597="10">
                     //2: <INPUT style="DISPLAY: inline" class="amount-input hide" maxLength=9 jQuery17101941748="8">

                         int nowusetitle = -1;
                         foreach (String[] ss in ll)
                         {
                             HtmlElement het = null;
                             //根据第一个数判断是那种标签
                             if (nowusetitle == -1)
                             {
                                 het = findElmByOutHtml(doc2, ss[0], "INPUT", "amount-input");
                                 if (het != null)
                                 {
                                     nowusetitle = 0;
                                 }
                                 else
                                 {
                                     String sktmp = ForNaiStrategy.getTitle2By1(ss[0]);
                                     het = findElmByOutHtml(doc2, sktmp, "INPUT", "amount-input hide");
                                     if (het != null)
                                     {
                                         nowusetitle = 1;
                                     }
                                 }
                             }

                             if (nowusetitle == -1)
                             {
                                 wf.addRtb2ForThread(" ======  cant find HtmlElement name ==========" + ss[0]);
                             }
                             else
                             {
                                 if (nowusetitle == 0)
                                 {
                                     if (het == null)
                                     {
                                         het = findElmByOutHtml(doc2, ss[0], "INPUT", "amount-input");
                                     }
                                 }
                                 else if (nowusetitle == 1)
                                 {
                                     if (het == null)
                                     {
                                         String sktmp = ForNaiStrategy.getTitle2By1(ss[0]);
                                         het = findElmByOutHtml(doc2, sktmp, "INPUT", "amount-input hide");
                                     }
                                 }

                                 if (het != null)
                                 {
                                     
                                     lhe.AddLast(het);
                                     lls.AddLast(ss[1]);
                                 }
                                 else
                                 {
                                     wf.addRtb2ForThread(" ======  cant find HtmlElement name ==========" + ss[0]);
                                 }
                             }
                         }

                         //保存投注信息
                         ForNaiTouzhu ftz = new ForNaiTouzhu();
                         if (svalue >= 0)
                         {
                             ftz.sOnetwo = "单";
                             ftz.sBeilv = svalue + "";
                         }
                         else
                         {
                             ftz.sOnetwo = "双";
                             int stmp = 0 - svalue;
                             ftz.sBeilv = stmp + "";
                         }
                         int shqh = Int32.Parse(ForNaiStrategy.llKaiJiang.Last());
                         ftz.sQihao = (shqh + 1) + "";


                         ForNaifen.setWebbroConfirmDlg(doc2);
                         ForNaifen.setWebbroConfirmDlg(wb.Document); 

                         Thread th = new Thread(new ParameterizedThreadStart(ForNaifen.submitNfTzByHtmlElem));
                         th.IsBackground = true;


                         Object[] obj = new Object[] { doc2, lhe, lls, ftz };
                         th.Start(obj);

                         wf.addRtb2ForThread(DateTime.Now + " " + " do bet strategy ok!");
                 }
             }
             catch (Exception e)
             {
                 wf.addRtb2ForThread(e.StackTrace);
             }
             finally
             {
                 isForceBet = false;
                 //Monitor.Exit(ObjLockNf);
                
             }
         }



        //解析验证码，使用Tessnet2,发现效果不好，取消！！！，by wind,14,12.01

       //不知道楼主有没有试过循环调用该组件去ocr，我有个软件中使用这个组件循环不停的ocr识别文本，然后问题来了，
        //内存不停的往上飙，调用Dispose方法根本没用!
       //很简单，你把识别过程的代码封闭成一个控制台应用程序，通过参数调用，这样每识别一次，就会自动结束进程，内在也就释放了
        //public static void getImageCode(String spath)
        //{
        //    String path = "";
        //    if (spath == null || spath == "")
        //        path = "http://images.cnblogs.com/cnblogs_com/ivanyb/201111/201111251245395649.png";
        //    else
        //    {
        //        path = spath;
        //    }

        //    tessnet2.Tesseract ocr = null;
        //    try
        //    {
        //        WebRequest request = WebRequest.Create(path);
        //        WebResponse response = request.GetResponse();
        //        Stream st = response.GetResponseStream();
        //        Bitmap bitmap = (Bitmap)Bitmap.FromStream(st);

        //        // //这里使用降噪，但是不好用？！！
        //        //UnCodebase ud = new UnCodebase(bitmap );
        //        //bitmap = ud.GrayByPixels();
        //        //ud.ClearNoise(128, 2);

        //        ocr = new tessnet2.Tesseract();//声明一个OCR类
        //        //运行到该句，程序自动退出？！！ //for test
        //       // ocr.Init("D:\\work\\tools\\tessnet2_32\\Tesseract-OCR\\tessdata", "eng", true);
        //        ocr.Init(Application.StartupPath + @"\\lib", "eng", true);
               
        //        ocr.SetVariable("tessedit_char_whitelist", "0123456789"); //设置识别变量，当前只能识别数字。

        //        //tessocr.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        //        //上面应该可以识别字母
        //        //应用当前语言包。注，Tessnet2是支持多国语的。语言包下载链接：http://code.google.com/p/tesseract-ocr/downloads/list

        //        List<tessnet2.Word> result = ocr.DoOCR(bitmap, Rectangle.Empty);//执行识别操作
        //        String code = result[0].Text;
        //        wf.addRtb2ForThread("find pic: " + code);
        //    }
        //    catch (Exception ex)
        //    {
        //        wf.addRtb2ForThread(ex.ToString());
               
        //    }
        //    finally 
        //    {
        //        if (ocr != null)
        //        {
        //            ocr.Clear();
        //            ocr.Dispose();
        //        }
        //    }

         //  }


         //启动定时器，执行下单程序
         public void dobetByTimer()
         {
             //这里Timer应该为唯一值，timer不能生成多个否则关闭时内存泄露出问题！
             if (myTimer == null)
             {
                 myTimer = new System.Windows.Forms.Timer();
                 //事件侦听只能加一次，并且每个timer应该专用
                 myTimer.Tick += new EventHandler(TimerEvent);
             }
             else
             {
                 if (myTimer.Enabled)
                 {
                     myTimer.Stop();
                 }
             }
            
             //5分钟//应该把时间设成两次间隔的一半，并判断是否和上期号码相同来决定是否下单
             if (wf.getCheckBoxNfDebug())
             {
                 myTimer.Interval = 15000;//测试时10秒一次
             }
             else
             {
                 myTimer.Interval = 51000;//50秒因为有3分钟时间可投注
             }
             myTimer.Start();
         }        

         private void TimerEvent(Object myObject, EventArgs myEventArgs)
         {
             try
             {
                 myTimer.Stop();
                 iturnpage = 0;
                 if (WindForm.isRunNaifen)
                 {
                   //时间到，执行下单程序，然后重启定时器
                   //  getConfigFileByName("", "");   
                     //for test
                   // wf.addRtb2ForThread(DateTime.Now+ "  TimerEvent 时间到.");                  
                   doBetByStrategy(null);
                   dobetByTimer();
                 }
                 else
                 {
                     //停止了
                     wf.addRtb2ForThread("===========自动下单结束！===========");
                     wf.getbtnNaifen().Enabled = true;
                 }
             }
             catch (Exception ex)
             {
                 wf.addRtb2ForThread(ex.ToString());
                 wf.addRtb2ForThread("===出错了！==自动下单结束！===========");
                 wf.getbtnNaifen().Enabled = true;
             }
             finally 
             {
             }
         }

         public void stopTimer()
         {
             if (myTimer != null)
             {
                 if (myTimer.Enabled)
                 {
                     myTimer.Stop();
                 }
             }
         }

         public static void setConfigFileByName(TextBox[] st)
         {
             for (int ik = 0; ik < st.Length; ik++)
         	{
         		if(st[ik].Text.Equals(""))
         		{
         			st[ik].Text = "0"; //input none means 0!
         		}
                Apis.setValueByStrName(STRConfigFile, st[ik].Name, st[ik].Text);
         	} 
         }

         public static String[] getConfigFileByName(TextBox[] stmp)
         {
             String[] st = new String[stmp.Length];
            for (int ik = 0; ik < stmp.Length; ik++)
         	{        		
         		String value = Apis.getValueByStrName(STRConfigFile, stmp[ik].Name);
         		st[ik] = value;         		
         	} 
         	return st;
         }
         
         //人工补投本期
         public void dobBetForce() 
         {
             isForceBet = true;
             iturnpage = 0;
             doBetByStrategy(wb);
         }

         //人工补投第期号期 qihao：上期期号， sqkj：
         public void buTou(String qihao,String sqkj) 
         {
             //isForceBet = true;
             //doBetByStrategy(wb);
             
             ////1判断历史期号里面是否有qihao
             ////2.1若有根据历史期号投注
             ////2.2若无，判断当前页面是否历史开奖页面
             ////4.1若是，获得页面数据，存储，判断是否有qihao,            
             ////4.1.1若有则转2
             ////4.1.2若无，输出错误信息，返回
             ////4.2若不是，转历史开奖页面，然后转4.1

            //因为不能补投上期，所以只能是test来获得增加值


         }
        

         //测试按钮
         public void testMenuitem() 
         {           
             ForNaiTouzhu fntz = new ForNaiTouzhu();
             fntz.sBeilv = "4";
             fntz.sOnetwo = "双";
             fntz.sQihao = "460915";
             fntz.sTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
             fntz.sKaijiang = "3.6.7.10.2";


             wf.addRtbNfft(Str_menu);
             wf.addRtbNfft(Str_split);

             soutFntz(fntz);
             saveFntz(fntz);
             fntz.sQihao = "460916";
             fntz.sKaijiang = "4.5.9.10.3";
             soutFntz(fntz);
             saveFntz(fntz);
             fntz.sQihao = "460917";
             fntz.sKaijiang = "6.5.1.10.2";
             soutFntz(fntz);
             saveFntz(fntz);
             fntz.sQihao = "460918";
             fntz.sKaijiang = "";
             soutFntz(fntz);
             saveFntz(fntz);
             fntz.sQihao = "460919";
             fntz.sKaijiang = "8.5.9.7.6";
             soutFntz(fntz);
             saveFntz(fntz);
             ForNaiStrategy.iDanMax = 47;
             ForNaiStrategy.iTwoMax = 16;
             logFile();

             //wf.addTbnfColor(fntz.ToString(), Color.Red);
             //wf.addRtbNfft(fntz.ToString());
             //wf.addRtbNfft(fntz.ToString());
             //wf.addTbnfColor(fntz.ToString(), Color.Blue);                                
         }

         private static void soutFntz(ForNaiTouzhu fntz) 
         {
         	 	if(ForNaiStrategy.htKai.Contains(fntz.sQihao))
                {
                    fntz.sKaijiang = ForNaiStrategy.htKai[fntz.sQihao]+"";
                }

                String sqqh = (Int32.Parse(fntz.sQihao) - 1) + "";

                String sqkj = "";
                if (ForNaiStrategy.htKai.Contains(sqqh))
                {
                    sqkj = ForNaiStrategy.htKai[sqqh] + "";
                }
                if (!sqkj.Equals(""))
                {
                    if (fntz.sKjPre.Equals(""))
                    {
                        fntz.sKjPre = sqkj;
                    }
                }

             wf.addRtbNfft(fntz.ToString());

             if (fntz.sKaijiang.Equals(""))
             {
                 String[] stp = fntz.testOut();
                 for (int ik = 0; ik < stp.Count(); ik++)
                 {
                     wf.addRtbNfft(stp[ik]);
                 }
             }
             else
             {
                 String[] stp = fntz.testOut();
                 for (int ik = 0; ik < stp.Count(); ik++)
                 {

                     wf.addTbnfColor(stp[ik], Col_RightNum[fntz.isRightNum[ik]]);
                 }
             }
             wf.addTbnfColor("", Color.Black);
         }

        //保存投注信息到log文件中
         private static void saveFntz(ForNaiTouzhu fntz)
         {
             String sq = fntz.sQihao + "期";
             Apis.setValueByStrName(ForNaifen.STR_LogFile, sq, fntz.ToString(), "T", "v");

             String[] stp = fntz.testOut();
             for (int ik = 0; ik < stp.Count(); ik++)
             {
                 //wf.addRtbNfft(stp[ik]);
                 Apis.setValueByStrName(ForNaifen.STR_LogFile, sq, stp[ik], "T", "v"+(ik+1));
             }
            // if (fntz.sKaijiang.Equals("")){}           
             wf.addRtb2ForThread("本次投注信息已经保存！");
         }

        //测试两面盘和页面历史开奖的互换
         public void goLmp(WebBrowser wtb)
         {
             if (wb == null)
             {
                 wb = wtb;
             }

             //老版本 <A id=MC21 onclick=MF_CI(21) href="javascript:void(0)">兩面盤</A> 

             //新版本
             //<A title="" href="javascript:void(0)" nav="status">下註明細</A> </SPAN>|
             //<SPAN><A title="" href="javascript:void(0)" nav="history">結算報表</A> </SPAN>| <SPAN>
             //<A title="" href="javascript:void(0)" nav="result">歷史開獎</A> </SPAN>|

             HtmlDocument doc = wb.Document;
             if (doc == null)
             {
                 wf.addRtbNfft("当前主页面地址不对.");
                 return;
             }

             HtmlElement he = findElmByOutHtml(doc, ">結算報表<", "A", "");
             if (he == null)
             {
                 wf.addRtbNfft("当前页面地址不对.");
                 return;
             }

             if (ti == null)
             {
                 ti = new System.Windows.Forms.Timer();
                 ti.Tick += new EventHandler(tiEvent);
             }
            
             ti.Interval = 5000;
             ti.Start();
         }

         private void tiEvent(Object myObject, EventArgs myEventArgs)
         {
             if (ti != null)
             {
                 ti.Stop();
             }
             //if(!WindForm.isRunNaifen)
             //{
             //  //如果不是自动投注就不切换
             //     wf.addRtb2ForThread("因自动投注已经停止，切换页面停止！");
             //     return;
             //}
                
             ////去到页面历史开奖
             //HtmlElement he = null;
             //if (iwebid == 0)
             //{
             //    iwebid = 1;
             //    he = findElmByOutHtml(wb.Document, ">結算報表<", "A", "");
             //}
             //else if (iwebid == 1)
             //{
             //    iwebid = 0;//去到页面北京赛车
             //   // <A id=pk10_sys class="switch  nav_longstring switch-on" href="javascript:void(0)"
             //    //subnav="bothSides_pk10">北京賽車(PK10)</A>
             //    he = findElmByOutHtml(wb.Document,">北京賽車(PK10)<" , "A", "");
             //}
             //if (he != null)
             //{
             //    he.InvokeMember("click");
             //    ti.Start();
             //}
             //else
             //{
             //    wf.addRtbNfft("当前主页面地址不对或无法连接！");
             //    return;
             //}            

             doBetByStrategy(null);

             //wf.addRtbNfft("自动跳转了一次！");

         }

        /// <summary>
         /// 遍历所有htmldoc元素,查找没有id没有name的含html的元素,by web
        /// </summary>
        /// <param name="hd"></param>
        /// <param name="html"></param>
        /// <returns></returns>
         public static HtmlElement findElmByOutHtml(HtmlDocument hd, String html)
        {
            if (hd == null || html == null)
                return null;
            foreach (HtmlElement he in hd.All)
            {
              
                   if (he.OuterHtml.IndexOf(html)!=-1)
                      {
                         return he;                        
                      }              
            }
            return null;
        }

         /// <summary>
         /// 遍历所有htmldoc元素,查找所有没有id没有name的含html内容元素,by web
         /// </summary>
         /// <param name="hd"></param>
         /// <param name="html"></param>
         /// <returns></returns>
         public static LinkedList<HtmlElement> findElmsByOutHtml(HtmlDocument hd, String html,String tagname)
         {
             if (hd == null || html == null || tagname==null || html.Equals("") || tagname.Equals(""))
                 return null;
             LinkedList<HtmlElement> llhe = null;
             foreach (HtmlElement he in hd.All)
             {              
                     if (he.TagName.Equals(tagname))// "A"))

                         if (he.OuterHtml.IndexOf(html) != -1)
                         {
                             if (llhe == null)
                             {
                                 llhe = new LinkedList<HtmlElement>();
                             }
                             llhe.AddLast(he);
                         }
             }
             return llhe;
         }

         //
        /// <summary>
         /// 遍历所有html元素,并通过tagname和classname查找没有id没有name的元素,
        /// </summary>
        /// <param name="hd"></param>
        /// <param name="html"></param>
        /// <param name="tagname"></param>
        /// <param name="classname"></param>
         /// <returns>HtmlElement 没找到返回null</returns>
         public static HtmlElement findElmByOutHtml(HtmlDocument hd, String html,String tagname,String classname)
         {
             if (hd == null || html == null)
                 return null;
             foreach (HtmlElement he in hd.All)
             {
                 if (!(tagname.Equals("")))
                 {
                     if (he.TagName.Equals(tagname))// "A"))
                     {
                         if (!(classname.Equals("")))
                         {
                             if (he.GetAttribute("className").Equals(classname))//  "T_a"))
                             {
                                 //had find！
                                 if (he.OuterHtml.IndexOf(html) != -1)
                                 {
                                     return he;
                                     //he.InvokeMember("click");
                                 }
                             }
                         }
                         else
                         {
                             if (he.OuterHtml.IndexOf(html) != -1)
                             {
                                 return he;
                             }                            
                         }
                     }                   
                 }
                 else
                 {//no tagname
                     if (!(classname.Equals("")))
                     {
                         if (he.GetAttribute("className").Equals(classname))//  "T_a"))
                         {
                             if (he.OuterHtml.IndexOf(html) != -1)
                             {
                                 return he;
                             }
                         }
                     }
                     else
                     {
                         if (he.OuterHtml.IndexOf(html) != -1)
                         {
                             return he;
                         }
                     }
                 }
            
                 //var link = (from n in top.All.Cast<HtmlElement>()
                 //            where n.TagName == "A" && n.GetAttribute("className") == "btn_1"
                 //            select n).First();
                 //link.InvokeMember("click");
             }
             return null;
         }

        //测试两个页面点击
         public void testHtmlElement() 
         {   
             if(wb == null)
                 wb = wf.getWebBrowser();
             /* //http://www.baidu.com/    测试用
              * <a data-id="1" data-tId="0001" href="#" onclick="return false;" class="s-menu s-opacity-menu1 "
              * hidefocus>导航</a>   
              * <a data-id="2" data-tId="0002" href="#" onclick="return false;" class="s-menu s-opacity-menu1 " 
              * hidefocus>新闻</a> 
             */
             //he.outerhtml如下:
             // <A hideFocus class="s-menu s-opacity-menu1" onclick="return false;" href="#" data-tId="0001"
             //data-id="1" jQuery1102009024941420544463="36">导航</A>

           
             //等待20秒
             if (ttest == null)
             {
                 ttest = new System.Windows.Forms.Timer();
                 ttest.Tick += new EventHandler(ttestEvent);
             }
              ttest.Interval = 3000;
              ttest.Start();
             //Thread th = new Thread(new ParameterizedThreadStart(threadJustWait));
             //th.IsBackground = true;
             //String waittiem = "4000";
             //th.Start(waittiem);  
         }

         //测试两个页面点击
         private void ttestEvent(Object myObject, EventArgs myEventArgs)
         {
             try
             {
                 ttest.Stop();
                 //ttest = null;

                 HtmlElement he  = null;
                 if (iwebid == 0)
                 {
                     iwebid = 1;
                     he = findElmByOutHtml(wb.Document, ">导航<", "A", "s-menu s-opacity-menu1");
                 }
                 else if(iwebid == 1)
                 {
                     iwebid = 0;
                     he = findElmByOutHtml(wb.Document, ">新闻<", "A", "s-menu s-opacity-menu1");
                 }
                 if (he != null)
                 {
                     he.InvokeMember("click");
                 }
                 ttest.Start();
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.ToString());
             }
             finally 
             {
             }
         }

        //保存今日两方案最大值
         public static void logFile() 
         {
            //<M name="参数最大值">
            //  <单方案><![CDATA[47]]></单方案>
            //  <双方案><![CDATA[16]]></双方案>
            //</M>
             String sdt = Apis.getValueByStrName(ForNaifen.STR_LogFile, "参数最大值", "M", "单方案");
             String stt = Apis.getValueByStrName(ForNaifen.STR_LogFile, "参数最大值", "M", "双方案");
             int idt = -1;
             if (!sdt.Equals(""))
             {
                 Int32.TryParse(sdt, out idt);
             }
             ForNaiStrategy.iDanMax = Math.Max(idt, ForNaiStrategy.iDanMax);
             ForNaiStrategy.iDanMax = Math.Max(ForNaiStrategy.iDanMax, ForNaiStrategy.iDanCpu);
             Apis.setValueByStrName(ForNaifen.STR_LogFile, "参数最大值", ForNaiStrategy.iDanMax + "", "M", "单方案");
             idt = -1;
             if (!stt.Equals(""))
             {
                 Int32.TryParse(stt, out idt);
             }
             ForNaiStrategy.iTwoMax = Math.Max(idt, ForNaiStrategy.iTwoMax);
             ForNaiStrategy.iTwoMax = Math.Max(ForNaiStrategy.iTwoCpu, ForNaiStrategy.iTwoMax);
             Apis.setValueByStrName(ForNaifen.STR_LogFile, "参数最大值", ForNaiStrategy.iTwoMax + "", "M", "双方案");

             wf.setTbnfState("*左方案最大值: " + ForNaiStrategy.iDanMax + " 右方案最大值: " +
                ForNaiStrategy.iTwoMax + " | ");
             //显示今日输赢值
             getWinMoeyByPage();

         }

        //测试验证码
         public void testYzm() 
         {
            // String spra = @"D:\work\tools\tessnet2_32\Tesseract-OCR\6883.jpg 26 -l eng";

             String scmd = "";
             String programName = "";
             String comd = @"D:\work\tools\tessnet2_32\Tesseract-OCR\tesseract";    
             String filename = @" D:\work\tools\tessnet2_32\Tesseract-OCR\6883.jpg";
             String outfile = " tmp";
             String para = " -l eng";

             filename = @" D:\work\temp\pic\" + "4539.png";      
             scmd = comd + filename + outfile + para;
             //scmd = "ping www.baidu.com";

             String res = Apitool.runProgram(programName, scmd, 1500);
            // wf.addRtbNfft(filename+" is: " + res);

             String sfout = outfile.Trim() + ".txt";
             string fpathnow = System.Environment.CurrentDirectory + "\\" + sfout;
             if (File.Exists(fpathnow))
             {
                String st = File.ReadAllText(fpathnow);
                File.Delete(fpathnow);
                wf.addRtbNfft(filename + " is: " + st);
             }
         
         }

        //测试修改脚本
         public void testScript() 
         {            
            //<SCRIPT>
              //setTimeout(function(){if(document.getElementById("s_main").offsetWidth==0 && typeof(F)=='undefined'){new Image().src=s_domain.baseuri+'/page/data/pageserver?errno=2015&msg=cdn_failed'}},2000);
              //if(typeof ns_c == "undefined"){var ns_c=function(){}}
              //</SCRIPT>
             //经测试，脚本只能添加，已经执行脚本找不到方法修改，要修改只能httprequest获得页面然后再修改，再webbrowser显示
             HtmlDocument doc = getFrameByIframeName(wb.Document, "rightLoader");
             if (doc == null)
                 return ;
             String tagname = "INPUT";
             String classname = "";
             String html = "testabc";
             foreach (HtmlElement he in doc.All)
             {
                 if (!(tagname.Equals("")))
                 {
                     if (he.TagName.Equals(tagname))// "A"))
                     {
                         if (!(classname.Equals("")))
                         {
                             if (he.GetAttribute("className").Equals(classname))//  "T_a"))
                             {
                                 //had find！
                                 if (he.OuterHtml.IndexOf(html) != -1)
                                 {
                                     wf.addRtb2ForThread("he's outhtml is: " + he.OuterHtml);
                                     //he.InvokeMember("click");
                                 }
                             }
                         }
                         else
                         {
                             if (he.OuterHtml.IndexOf(html) != -1)
                             {
                                 wf.addRtb2ForThread("he's outhtml is: " + he.OuterHtml);
                             }
                             else
                             {
                                 wf.addRtb2ForThread("he's class name is: " + he.GetAttribute("className")+"    .");
                             }
                         }
                     }
                 }
             }
             return ;
         }

         public static void getNfHistory(Object sspage) 
         {
             //点击历史开奖页面，选择北京赛车开奖
             //-----------------rightLoader-----------------
                // <TR jQuery17108886833156988404="3">
                //<A href="?kw=1&amp;pager=2">後一頁&nbsp;</A>
             try
             {                
                 String st = "";
                 if (sspage == null)
                 {
                     HtmlDocument doc = null;
                     if (wb.InvokeRequired)
                     {
                         wb.Invoke((MethodInvoker)delegate
                         {
                             doc = getFrameByIframeName(wb.Document, "rightLoader");
                         });

                     }
                     else
                     {
                         doc = getFrameByIframeName(wb.Document, "rightLoader");
                     }
                    // HtmlDocument doc = getFrameByIframeName(wb.Document, "rightLoader");
                     if (doc == null)
                     {
                         wf.addRtbNfjl("当前页面不对，请转到北京赛车历史开奖页面！");
                         return;
                     }
                     st = doc.Body.InnerHtml;
                 }
                 else
                 {
                     String[] ssp = sspage as String[];
                     st = ssp[0];
                 }

                 if (st.Equals(""))
                 {
                     //数据已经抓完或者无数据
                     return;
                 }

                 //如果自动翻页已经勾选
                 String snextpageAddr = getNFhisPageCode(st);

                 if (wf.getCbNFjlgetnow())
                 {
                     while (!snextpageAddr.Equals(""))
                     {
                         String spage = Apis.GetPageCode(snextpageAddr, "gb2312");
                         if (spage == null)
                         {
                             wf.addRtb2ForThread("网址打不开! " + snextpageAddr);
                             break;
                         }
                         else
                         {
                             snextpageAddr = getNFhisPageCode(spage);
                         }
                     }
                 }                
            }
             catch (Exception e)
             {
                 wf.addRtbNfjl(e.StackTrace);
             }
             finally
             {               
             }  
         }

        //获得当前页面开奖数据数据，返回下一页链接地址或者空字符
         private static String getNFhisPageCode(String st)
         {
             //点击历史开奖页面，选择北京赛车开奖
             //-----------------rightLoader-----------------
             // <TR jQuery17108886833156988404="3">
             //<TD>471799</TD>
             //<TD>01-28 三 21:22</TD>
             //<TD><SPAN class="number num2"></SPAN></TD>
             //<TD><SPAN class="number num4"></SPAN></TD>
             //<TD><SPAN class="number num10"></SPAN></TD>
             //<TD><SPAN class="number num9"></SPAN></TD>
             //<TD><SPAN class="number num7"></SPAN></TD>
             //<TD><SPAN class="number num6"></SPAN></TD>
             //<TD><SPAN class="number num8"></SPAN></TD>
             //<TD><SPAN class="number num3"></SPAN></TD>
             //<TD><SPAN class="number num5"></SPAN></TD>
             //<TD><SPAN class="number num1"></SPAN></TD>
             //<TD class=result_pk10_td>6</TD>
             //<TD class=result_pk10_td>小</TD>
             //<TD class=result_pk10_td><SPAN class="red ">雙</SPAN></TD>
             //<TD class=result_pk10_td><SPAN class="bluer ">龍</SPAN></TD>
             //<TD class=result_pk10_td>虎</TD>
             //<TD class=result_pk10_td><SPAN class="bluer ">龍</SPAN></TD>
             //<TD class=result_pk10_td><SPAN class="bluer ">龍</SPAN></TD>
             //<TD class=result_pk10_td><SPAN class="bluer ">龍</SPAN></TD></TR>

             //<A href="?kw=1&amp;pager=2">後一頁&nbsp;</A>

             SQLiteConnection conn = null;
             try
             {
                 wf.addRtbNfjl("开始获取数据...");

                 //Monitor.Enter(ObjLockNf);


                 int ik = -1;
                 String reg = @"number num\d\d?";

                 String stp = Apitool.FindStrByName(st, "/THEAD>", "");
                 if (stp == null)
                 {
                     wf.addRtbNfjl("当前页面不对，请转到北京赛车历史开奖页面！");
                     return "";
                 }
                 st = stp;

                 conn = Apis.getSqliteCon("");
                 SQLiteCommand msc = conn.CreateCommand();
                 SQLiteCommand msc2 = conn.CreateCommand();

                 Boolean isBreak = false;
                 String nextAddr = "";
                 String stime = "";
                 while (!isBreak)
                 {
                     stp = Apitool.FindStrByName(st, "<TR", "/TR>", out ik);
                     if (ik == -1)
                     {
                         isBreak = true;
                         continue;
                     }
                     st = st.Substring(ik);

                     int ip = -1;
                     String stqh = Apitool.FindStrByName(stp, "<TD>", "</TD>", out ip);
                     if (stqh == null)
                     {
                         isBreak = true;
                         break;
                     }
                     stime = "";
                     if (ip != -1)
                     {
                         stp = stp.Substring(ip);
                         String stp2 = Apitool.FindStrByName(stp, "<TD>", "</TD>");
                         if (stp2 != null && (stp2.IndexOf("-") != -1))
                         {
                             stime = stp2.Trim().Substring(0, 5).Replace("-", "");
                         }
                     }

                     String stm = TestReg.findRegNoshow(stp, reg);
                     if (stm.Equals(""))
                     {
                         isBreak = true;
                         break;
                     }
                     stm = stm.Replace("number num", ".").Replace("\r\n", "").Substring(1);

                     //加入数据库
                     {
                         //Thread thadd = new Thread(new ParameterizedThreadStart(ForNaifen.addOneJiluBythread));
                         //String[] obj = new String[] { stqh, stm, stime };
                         //thadd.IsBackground = true;
                         //thadd.Start(obj); 
                         if (stm.IndexOf("0.0.0") == -1)//丢弃0.0.0的数据
                         {
                             addOneJilu(stqh, stm, stime, msc, msc2);
                         }
                     }
                 }

                 if (stime.Equals(DateTime.Now.ToString("MMdd")))
                 {
                   
                 }
                 else
                 {
                     //该页面最后一项纪录已经不是今天纪录 
                     nextAddr = "";
                 }
                 UseStatic.getWindForm().addRtbNfjl("已经获得本页历史数据");
                 return nextAddr;
             }
             catch (Exception e)
             {
                 wf.addRtbNfjl(e.StackTrace);
                 return "";
             }
             finally
             {
                 // Monitor.Exit(ObjLockNf);
                 if (conn != null)
                 {
                     conn.Close();
                     conn.Dispose();
                     conn = null;
                 }
             } 
         }
         
         //模拟计算
         public void compuByHistory(String startqh)
         {            
             //根据设置初始值
             ForNaiStrategy.iDanCput = wf.getTbnfqsbs1value();
             ForNaiStrategy.iTwoCput = wf.getTbnfqsbs2value();

             wf.addRtbNfjl("----------------------------------------------------");
             wf.addRtbNfjl("开始模拟计算！目前单数倍率为: " + ForNaiStrategy.iDanCput + " 双数倍率为：" +
               ForNaiStrategy.iTwoCput);

             Boolean isst = !startqh.Equals("");

             int ic = ForNaiStrategy.llnfls.Count;
             int iruntime = 0;//实际计算期数
             for (int it = 0; it < ic; it++)
             {
                 String sqh = ForNaiStrategy.llnfls.ElementAt(ic-1-it);
                 Boolean isrun = false;
                 if (isst)
                 {
                     if (Int32.Parse(sqh) >= Int32.Parse(startqh))
                     {
                         isrun = true;
                     }
                 }
                 else
                 {
                     isrun = true;
                 }

                 if (isrun)
                 {
                     String skj = ForNaiStrategy.htKails[sqh] + "";

                     int svalue = ForNaiStrategy.getNBeiNum2TestOld(ForNaiStrategy.STRATE_BIG_SMALL, sqh, skj,"");
                     String sdt = "单";
                     if (svalue < 0)
                     {
                         sdt = "双";
                         svalue = 0 - svalue;
                     }
                     iruntime++;
                     wf.addRtbNfjl("第 " + sqh + " 期开奖号码： " + skj);
                     String xqqh = (Int32.Parse(sqh) + 1) + "";
					
                 
                     
                     wf.addRtbNfjl("   " + xqqh + " 期投" + sdt + " ,倍率: " + svalue + "  ，单数: "
                         + ForNaiStrategy.iDanCput + "  ，双数：" + ForNaiStrategy.iTwoCput + "  .");
                 }
               }

             wf.addRtbNfjl("----------------------------------------------------");
             wf.addRtbNfjl("模拟计算完毕!共 " + iruntime + " 期，目前单倍率为: " + ForNaiStrategy.iDanCput + " 双倍率为：" +
                 ForNaiStrategy.iTwoCput);
         }

        //根据数据库数据模拟计算
         public void compuByHistorySql(String startqh)
         {
             //根据设置初始值
             ForNaiStrategy.iDanCput = wf.getTbnfqsbs1value();
             ForNaiStrategy.iTwoCput = wf.getTbnfqsbs2value();

             wf.addRtbNfjl("----------------------------------------------------");
             wf.addRtbNfjl("开始模拟计算！目前左倍率为: " + ForNaiStrategy.iDanCput + " 右倍率为：" +
               ForNaiStrategy.iTwoCput);
             wf.addRtbNfjl("----------------------------------------------------");

             String std = "";
             String sql = "";
             if (startqh.Equals(""))//计算今天的
             {
                 std = " where time >= '" + DateTime.Now.ToString("MMdd") + "' ";
                 sql = "select * from kjhis " + std + " order by sqh limit 2000";
             }
             else
             {
                 sql = "select * from kjhis where sqh >= '" + startqh + "' order by sqh limit 2000";
             }                        
             
             SQLiteConnection conn = null;
             SQLiteCommand msc = null;
             SQLiteDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
             try
             {
                 conn = Apis.getSqliteCon("");
                 msc = conn.CreateCommand();
                 msc.CommandText = sql;

                 dr1 = msc.ExecuteReader();
                
                 if (dr1 == null)
                 {
                     wf.addRtbNfjl("数据库中无所需数据记录！");
                     return;
                 }

                 int iruntime = 0;//实际计算期数
                 String slast2kaijaing = "";
                  while (dr1.Read())
                  {
                      String id = dr1["id"] + "";
                      String sqh = dr1["sqh"] + "";
                      String skj = dr1["skj"]+"";
                      String time = dr1["time"] + "";
                      String other = dr1["other"] + "";

                      int svalue = ForNaiStrategy.getNBeiNum2Test(ForNaiStrategy.STRATE_BIG_SMALL2, sqh, skj,slast2kaijaing);
                      slast2kaijaing = skj;                     

                      String sdt = "单";
                      String[] sh = skj.Split('.');
                      int ifirst = 1;
                          if (sh.Length >= 5)
                          {
                              ifirst = ForNaiStrategy.isDan(sh[0]);//判断第一个数是否是单
                              if (ifirst == 1)
                              {
                                  if (svalue < 0)
                                  {
                                       sdt = "双";
                                  }
                              }
                              else if (ifirst == 0)
                              {
                                  if (svalue >= 0)
                                  {
                                       sdt = "双";
                                  }
                              }
                          }

                          if (svalue < 0)
                          {
                              svalue = 0 - svalue;
                          }

                      iruntime++;
                      wf.addRtbNfjl("第 " + sqh + " 期开奖号码： " + skj);
                      String xqqh = (Int32.Parse(sqh) + 1) + "";
                      
                     String soutdt = "";
                     if (ifirst == 1)
                     {
                     	soutdt = "  ，单: "
                          + ForNaiStrategy.iDanCput + "  ，双：" + ForNaiStrategy.iTwoCput;
                     }
                     else
                     {
                         soutdt = "  ，双：" + ForNaiStrategy.iDanCput + "  ，单: "
                          + ForNaiStrategy.iTwoCput;
                     }
                      wf.addRtbNfjl("  " + xqqh + " 期投: " + sdt + "  倍率: " + svalue + soutdt );
					wf.addRtbNfjl("--------------------------------------------");
                      //wf.addRtbNfjl("id: " + id+ "  QH: "+ sqh+ "  KJ: "+ skj + " TI: " + time);
                  }

                  wf.addRtbNfjl("----------------------------------------------------");
                  wf.addRtbNfjl("模拟计算完毕!共 " + iruntime + " 期，目前左倍率为: " + ForNaiStrategy.iDanCput + " 右倍率为：" +
                      ForNaiStrategy.iTwoCput);
             }
             catch (Exception ex)
             {
                 wf.addRtbNfjl(ex.ToString());
             }
             finally
             {
                 if (msc != null)
                 {
                     msc.Dispose();
                 }
                 if (dr1 != null)
                 {
                     dr1.Close();
                     dr1.Dispose();
                 }               
                 if (conn != null)
                 {
                     conn.Close();
                     conn.Dispose();
                 }
             }
         }

         //显示数据库中sqihao数以后的历史记录，sqihao为空则显示全部,showtoday 为true则只显示今天以后的数据
         public void getDBHistory(String sqihao,Boolean showtoday) 
         {

             String std = "";
             if (showtoday)
             {
                 std = " where time >= '"+DateTime.Now.ToString("MMdd")+"' ";
             }
             String sql = "select * from kjhis "+std+" order by sqh limit 2400";
             if(!sqihao.Equals(""))
             {
                 if(std.Equals(""))
                    sql = "select * from kjhis where sqh >= '"+sqihao+"' order by sqh limit 2400";
                 else
                     sql = "select * from kjhis " + std + " and sqh >= '" + sqihao + "' order by sqh limit 2400";
             }
             
             //wf.addRtbNfjl("kjhistory count is: " + Apis.GetSqliteCount("kjhistory"));

             SQLiteConnection conn = null;
             //DataSet ds = null;
             //SQLiteDataAdapter sda = null;
             SQLiteCommand msc = null;
             SQLiteDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
             try
             {
                 conn = Apis.getSqliteCon("");
                 msc = conn.CreateCommand();
                 msc.CommandText = sql;

                 dr1 = msc.ExecuteReader();

                 //sda = new SQLiteDataAdapter(msc);
                 ////sda = new SQLiteDataAdapter(sql, conn);//DataAdapter：网络适配器,可读.写
                 //ds = new DataSet();
                 //sda.Fill(ds, "sqlitetbl");//将结果填充到ds中
                 //DataTable dt = ds.Tables["sqlitetbl"];

                 if (dr1 == null)
                 {
                     wf.addRtbNfjl("数据库中无所查询数据！");
                     return;
                 }

                  while (dr1.Read())
                  {
                      String id = dr1["id"] + "";
                      String sqh = dr1["sqh"] + "";
                      String skj = dr1["skj"]+"";
                      String time = dr1["time"] + "";
                      String other = dr1["other"] + "";

                      wf.addRtbNfjl(id+ ".  Q: "+ sqh+ "  K: "+ skj + " T: " + time);
                  }
             }
             catch (Exception ex)
             {
                 wf.addRtbNfjl(ex.ToString());
             }
             finally
             {
                 if (msc != null)
                 {
                     msc.Dispose();
                 }
                 if (dr1 != null)
                 {
                     dr1.Close();
                     dr1.Dispose();
                 }
                 //if (ds != null)
                 //{                     
                 //    ds.Dispose();
                 //}
                 //if (sda != null)
                 //{
                 //    sda.Dispose();
                 //}
                 if (conn != null)
                 {
                     conn.Close();
                     conn.Dispose();
                 }
             }
         }

         public void addOneJilu(String sqh,String skj,String stime,String other) 
         {

            SQLiteDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
            SQLiteConnection cn = null;            
            String sql = "select * from kjhis where sqh = '" + sqh + "' limit 1";           
            String strInser = "";
            try
            {
                cn = Apis.getSqliteCon("");
                SQLiteCommand msc = cn.CreateCommand();
                msc.CommandText = sql;
                dr1 = msc.ExecuteReader();

                if (dr1.Read())
                {
                    String skjr = dr1["skj"] + "";
                    if (skjr.Equals(skj))
                    {
                        wf.addRtbNfjl("数据库中已有该数据！");
                        return;
                    }
                    else
                    {
                        strInser = "update kjhis set skj = '" + skj +"', time= '"+stime+"', other='"+other
                            + "' where sqh = '" + sqh + "'";
                    }
                }
                else
                {
                    strInser = "insert into kjhis(sqh,skj,time,other) values('" + sqh + "','"
                      + skj + "','" + stime + "','" + other + "')";     
                }
                if (dr1 != null)
                {
                    dr1.Close();
                }

             //DataTable dt = Apis.GetSqliteDataTable(sql);
             //if (dt != null && dt.Rows != null && dt.Rows.Count==1)
             //{
             //    String skjr= dt.Rows[0]["skj"] + "";
             //    if (skjr.Equals(skj))
             //    {
             //        wf.addRtbNfjl("数据库中已有该数据！");
             //        return;
             //    }
             //    else
             //    {
             //         strInser = "update kjhis set skj = '" + skj + "' where sqh = '" + sqh + "'";
             //    }               
             //}
             //else
             //{
             //     strInser = "insert into kjhis(sqh,skj,time,other) values('" + sqh + "','"
             //    + skj + "','" + " " + "','" + DateTime.Now.ToString() + "')";               
             //}

             int iresult = -1;
             iresult = Apis.runSqlite(strInser, "");
             if (iresult == 1)
             {
                 wf.addRtbNfjl("添加记录成功!");
             }
             else if(iresult == 0)
             {
                 wf.addRtbNfjl("添加记录失败!");
             }
           }
            catch (Exception ex)
            {
                wf.addRtbNfjl(ex.StackTrace);
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
        }
         
          public static void addOneJiluBythread(Object obj) 
         {
             if (obj == null) return;
             String[] bb = obj as String[];
             String sqh = bb[0];
             String skj = bb[1];
             String time = bb[2];
             addOneJilu(sqh, skj, time);           
        }

          public static void addOneJilu(String sqh,String skj,String time)
          {
              String sqldel = "delete from kjhis where sqh = '" + sqh + "'";
              String strInser = "";
              try
              {
                  int iresult = -1;
                  iresult = Apis.runSqlite(sqldel, "");

                  strInser = "insert into kjhis(sqh,skj,time,other) values('" + sqh + "','"
                       + skj + "','" + time + "','" + DateTime.Now.ToString() + "')";
                  iresult = Apis.runSqlite(strInser, "");
              }
              catch (Exception ex)
              {
                  wf.addRtbNfjl(ex.StackTrace);
              }
              finally
              {
              }
          }

          public static void addOneJilu(String sqh, String skj, String time,SQLiteCommand smcdel,SQLiteCommand smcadd)
          {
              String sqldel = "delete from kjhis where sqh = '" + sqh + "'";
              String strInser = "";
              try
              {
                  int iresult = -1;
                  smcdel.CommandText = sqldel;
                  iresult = smcdel.ExecuteNonQuery();

                  strInser = "insert into kjhis(sqh,skj,time,other) values('" + sqh + "','"
                       + skj + "','" + time + "','" + DateTime.Now.ToString() + "')";
                  smcadd.CommandText = strInser;
                  iresult = smcadd.ExecuteNonQuery();
              }
              catch (Exception ex)
              {
                  wf.addRtbNfjl(ex.StackTrace);
              }
              finally
              {
              }
          }

          //获得盈亏页面
          public static void getWinMoeyByPage()
          {
              if (sAddrWinMoney.Equals(""))
              {
                  String now = wb.Url + "";
                  //http://103.9.229.123:8211/ssxcx49107f_945/index.htm?21015_21026_4.5.trunk_20150127&kw=1
                  ///index.htm?
                  int ia = now.IndexOf("/index.htm");
                  if (ia != -1)
                  {
                      now = now.Substring(0, ia);
                      sAddrWinMoney = now + "/klc/history/index/&kw=1";
                  }
              }
              if (sAddrWinMoney.Equals(""))
              {
                  return;
              }

              String page = "";
              // String page = Apis.GetPageCode(sAddrWinMoney, "GB2312");
              //  page =  Apis.getHtmlSource(sAddrWinMoney);
              //上面方法因为网页判断了浏览器id，所以上面方法限制不行，只有新开webbrowser才行

              wf.getWebBrowser2().Navigate(sAddrWinMoney);
              Thread.Sleep(3000);

              WebBrowser wbr2 = wf.getWebBrowser2();
              if (wbr2.InvokeRequired)
              {

                  wbr2.Invoke((MethodInvoker)delegate
                  {
                      HtmlDocument doc = wf.getWebBrowser2().Document;
                      if (doc == null)
                      {
                          wbr2.Navigate(new Uri("about:blank"));
                          return;
                      }
                      page = doc.Body.InnerHtml.ToString();
                      wbr2.Navigate(new Uri("about:blank"));
                  });
              }
              else
              {
                  HtmlDocument doc = wf.getWebBrowser2().Document;
                  if (doc == null)
                  {
                      wbr2.Navigate(new Uri("about:blank"));
                      return;
                  }
                  page = doc.Body.InnerHtml.ToString();
                  wbr2.Navigate(new Uri("about:blank"));
              }

              if (page == null || page.Equals(""))
              {
                  return;
              }

              String money = getWinMoney(page);

              wf.setTbnfState("*左方案最大值: " + ForNaiStrategy.iDanMax + " 右方案最大值: " +
                ForNaiStrategy.iTwoMax);
              String stbs = wf.getTbnfStateTxt();
              int itk = stbs.IndexOf("|");
              if (itk != -1)
              {
                  wf.setTbnfState(stbs.Substring(0, itk).Trim() + " |" + " 今日盈亏: " + money);
              }
              else
              {
                  wf.setTbnfState(stbs.Trim() + " |" + " 今日盈亏: " + money);
              }

              //设置止盈止损
              if (!money.Equals(""))
              {
                  setWinLostMoney(money);
              }
              return;
          }

          //设置止盈止损
          private static void setWinLostMoney(String money) 
          {
              String szy = wf.geTBNfzyin().Trim();
              String szs = wf.getTBNfzsun().Trim();
              if (!(szy.Equals("")) && !(szs.Equals("")) && !(money.Equals("")))
              {
                  int izy = 0;
                  int izs = 0;
                  int imoney = 0;
                  if (Int32.TryParse(szy, out izy) && Int32.TryParse(szs, out izs) && Int32.TryParse(money, out imoney))
                  {
                      if ((izs < imoney)  && (imoney< izy))
                      {
                      }
                      else
                      {
                          wf.addRtbNfft("因输赢值已超过止盈止损设置，自动下单停止！！！");
                          if (wf.InvokeRequired)
                          {
                              wf.Invoke((MethodInvoker)delegate
                              {
                                  wf.btnnfstop_Click(null, null);
                              });
                          }
                          else
                          {
                              wf.btnnfstop_Click(null, null);
                          }
                          
                      }
                  }
              }
          }


        /// <summary>
        /// 获得盈亏数据
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
          public static String getWinMoney(String page) 
          {
             //<TR jQuery171044570576137885775="16">
            //<TD>
            //<A href="../listplay/?date=2015-02-15+%E6%98%9F%E6%9C%9F%E6%97%A5&amp;kw=1">2015-02-15 星期日</A></TD>
            //<TD><A href="../listplay/?date=2015-02-15+%E6%98%9F%E6%9C%9F%E6%97%A5&amp;kw=1">95</A></TD>
            //<TD class=history_tdright><A href="../listplay/?date=2015-02-15+%E6%98%9F%E6%9C%9F%E6%97%A5&amp;kw=1">3,005</A></TD>
            //<TD class=history_tdright><A href="../listplay/?date=2015-02-15+%E6%98%9F%E6%9C%9F%E6%97%A5&amp;kw=1">25</A></TD>
            //<TD class=history_tdright><A href="../listplay/?date=2015-02-15+%E6%98%9F%E6%9C%9F%E6%97%A5&amp;kw=1">
            //<FONT class=red>-201</FONT></A></TD></TR></TBODY>

              //<TD class=history_tdright><A href="../listplay/?date=2015-02-11+%E6%98%9F%E6%98%89&amp;kw=1">7,815</A></TD>
               //<TD class=history_tdright><A href="../listplay/?date=2015-02-11+%E6%98%9F%B8%89&amp;kw=1">65</A></TD>
              ////<TD class=history_tdright><A href="../listplay/?date=2015-02-11+%E6%988%89&amp;kw=1">91</A></TD></TR>
                ////<TR jQuery171044570576137885775="13">
                ////<TD><A href="../listplay/?date=2015-02-12+%EB&amp;kw=1">2015-02-12 星期四</A></TD>

              //获得本日盈亏数据
              if (page == null || page.Equals(""))
              {
                  HtmlDocument doc2 = ForNaifen.getFrameByIframeName(wb.Document, "rightLoader");

                  if (doc2 == null)
                      return "";

                  page = doc2.Body.InnerHtml.ToString();

                  if (page == null || page == "")
                      return "";
              }

              String snow = DateTime.Now.ToString("yy-MM-dd");
              String sf = Apitool.FindStrByName(page, snow, "</TR>");
              if (sf == null)
              {
                  return "";
              }
              //一共要找6次，才是盈亏数据,还有5次
              for (int ik = 0; ik < 5; ik++)
              {
                  sf = Apitool.FindStrByName(sf, snow,"");
                  if (sf == null)
                  {
                      return "";
                  }
              }
              sf = sf.Replace("<FONT class=red>", "");

              String svalue = Apitool.FindStrByName(sf, ">", "<"); 
              if (svalue == null)
              {
                  return "";
              }
              return svalue.Trim(); ;
          }

          /// <summary>
          /// 获得已下Money数据
          /// </summary>
          /// <param name="page"></param>
          /// <returns></returns>
          private static String getBetMoney(String page)
          {
                //              <TR>
                //<TD class=inner_text>已下金額</TD>
                //<TD id=total_amount class=total_amount>10</TD></TR>
                //<TR>

              if (page == null || page.Equals(""))
              {
                  HtmlDocument doc2 = ForNaifen.getFrameByIframeName(wb.Document, "leftLoader");
                  if (doc2 == null)
                      return "";
                  page = doc2.Body.InnerHtml.ToString();
                  if (page == null || page == "")
                      return "";
              }

              String sf = Apitool.FindStrByName(page, "id=total_amount", "</TR>");
              if (sf == null)
              {
                  return "";
              }            
              String svalue = Apitool.FindStrByName(sf, ">", "<");
              if (svalue == null)
              {
                  return "";
              }
              return svalue.Trim();
          }


         public void delOneJilu(String sqh)
         {
             SQLiteDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
             SQLiteConnection cn = null;
             String sql = "select * from kjhis where sqh = '" + sqh + "' limit 1";
             try
             {
                 cn = Apis.getSqliteCon("");
                 SQLiteCommand msc = cn.CreateCommand();
                 msc.CommandText = sql;
                 dr1 = msc.ExecuteReader();

                 String strdel = "";
                 if (dr1.Read())
                 {
                     strdel = "delete from kjhis where sqh = '" + sqh + "'";                  
                 }
                 else
                 {
                     wf.addRtbNfjl("数据库中无该条记录!");
                     return;
                 }
                 if (dr1 != null)
                 {
                     dr1.Close();
                 }
                 if (strdel.Equals(""))
                     return;

                 int iresult = -1;
                 iresult = Apis.runSqlite(strdel, "");
                 if (iresult == 1)
                 {
                     wf.addRtbNfjl("删除记录成功!");
                 }
                 else if (iresult == 0)
                 {
                     wf.addRtbNfjl("删除记录失败!");
                 }
             }
             catch (Exception ex)
             {
                 wf.addRtbNfjl(ex.StackTrace);
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
         }

         public static void dispose()
        {
            if (wf != null)
            {
                wf = null;
            }
            if (wb != null)
            {
                wb = null;
            }            
        }

    }//end class ForNaifen
}
