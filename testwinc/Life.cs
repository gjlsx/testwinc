using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using testwinc.test;
using testwinc.tools;
using testwinc.capvideo;
using testwinc.others;
using System.Threading;
using mshtml;


namespace testwinc
{
    public partial class WindForm : Form
    {
        public static Boolean isDebug = true;
        private JustFortest jft = null;

        public static Boolean isCappptv = false;//标志是否抓取pptv
        public static Boolean isCapyuku = false;//标志是否抓取zhibo8
        public static Boolean isCapCntv = false;//标志是否抓取cntv
        public static Boolean isCapyouku = false;//标志是否抓取youku
        public static Boolean isPiPei = false;//标志是否匹配数据
        public static Boolean isCappic = false;//标志是否抓图片
        public static Boolean isCapqqdsj = false;//标志是否抓qq电视剧
        public static Boolean isCapyoukudsj = false;//标志是否抓youku电视剧
        public static Boolean isCapLesiudsj = false;//标志是否抓乐视网电视剧
        public static Boolean isCapXunleidsj = false;//标志是否抓迅雷电视剧
        public static Boolean isCapTudoudsj = false;//标志是否抓土豆电视剧
        public static Boolean isCapletv = false;//标志是否抓乐视网
        public static Boolean isCapxunlei = false;//标志是否抓迅雷影视
        public static Boolean isCapTudou = false;//标志是否抓土豆

        public static Boolean isRunNaifen = false;//标志是否启动Naifen

        public static Boolean isTongbu = false;//标志是否同步数据库
        public static Boolean isBuildStar = false;//标志是否生成明星表


        public static LinkedList<String> llThread = null;

        //public static Boolean[] isCapture = {isCappptv, isCapyuku};

        //给combox中字符按拼音选择用
        private int dtnowms = 0;
        private int dtnowSecond = 0;
        private LinkedList<int> llcharSave = null;
        private LinkedList<LinkedList<String>> llCmbPinyin = null;
        private static String StrCutWebPage = "";

        private static System.Windows.Forms.Timer myTimer = null;
        private static Object lockObj = new Object();
        private int iTimeCount = 0;

        public static LinkedList<InterfaceWBDoc> llIfaceWB = null;//浏览器完成响应事件列表

        private ForNaifen fnf = null;

        public static Boolean isReleaseNF = false;//是否是naifen发布版本

        public TextBox[][] tbNall = null;
        public TextBox[] tbNal1, tbNal2, tbNal3,tbNal4,tbNal5,tbNal6,tbNal7,tbNal8,tbNal9,tbNal10;

       

        public WindForm()
        {
            InitializeComponent();
            initWindForm();
        }

        //初始化
        private void initWindForm()
        {
            UseStatic.setWindForm(this);
            jft = new JustFortest();

            this.cmbtxt.Items.Add("江山如画");
            this.cmbtxt.Items.Add("ta 她爱我");
            this.cmbtxt.Items.Add("美人如玉");

            this.cmbtxt.SelectedIndex = 0;
            this.llCmbPinyin = Charpinyin.getPinyinForCombox(cmbtxt);
            this.llcharSave = new LinkedList<int>();
            cmbTimeStep.SelectedIndex = 0;
            cmbTimeStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTongBu.SelectedIndex = 0;
            cmbTongBu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            llThread = new LinkedList<String>();
            llIfaceWB = new LinkedList<InterfaceWBDoc>();


            wb.NewWindow += new CancelEventHandler(webBrowser_NewWindow);//            
            if (this.fnf == null)
            {
                fnf = new ForNaifen();   
                setTextBoxNaifenFromFile();

                setTextBoxNaifenFromFile2();

                //setNaifenAddValue();
                wb.ScriptErrorsSuppressed = true;//屏蔽脚本错误
                this.tbxaddress.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbxaddress_keyup);

                wb2.ScriptErrorsSuppressed = true;
                this.tbweb2addr.Text = "http://www.ifeng.com/";
            }
            
            hideMenuifNessary(isReleaseNF);                                  
        }
        
        private void hideMenuifNessary(Boolean ih)
        {
        	if(ih)
        	{
        	    tabPage1.Parent = null;// 隐藏tabPage1  
                tabPage3.Parent = null;
                tabPage4.Parent = null;
                tabPage5.Parent = null;
                tabPage8.Parent = null;
        	}
        	else
        	{
        		tabPage1.Parent = this.tabPage6.Parent;// 隐藏tabPage1  
                tabPage3.Parent = this.tabPage6.Parent;
                tabPage4.Parent = this.tabPage6.Parent;
                tabPage5.Parent = this.tabPage6.Parent;
                tabPage8.Parent = this.tabPage6.Parent;
        	}
                // this.tabPage1.Parent = this.tabc.tabControl1;//显示tabPage1
			
                Boolean fa = !ih;
                btncut.Visible = fa;
                btntcut.Visible = fa;
                btnfnf.Visible = fa;
                    btnpic.Visible = fa;
                    go.Visible = fa;
                    menutest.Visible = fa;
                    menuitemlmp.Visible = fa;
                    gethtelmToolStripMenuItem.Visible = fa;
                    //logToolStripMenuItem.Visible = fa;
                    menuitemYzm.Visible = fa;
                    menuItemBtou.Visible = fa;
                    cbnfdebug.Visible = fa;
       
        }

        //取消新窗口打开链接
        private void webBrowser_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;         
            try
            {
                String address = wb.Document.ActiveElement.GetAttribute("href");
                Uri a = new Uri(address);//获取点击中的链接地址 
                wb.Navigate(a);
            }
            catch (Exception)
            {
                return;
            }         
        }


        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        { }

        private void tabPage3_Click(object sender, EventArgs e)
        { }

        private void tabPage4_Click(object sender, EventArgs e)
        { }

        private void btnwait_Click(object sender, EventArgs e)
        {
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //超过6000行删除1000行*20个字符
            setTextboxTxtLength(textBox1, 5000, 1000);
        }

        //输出到textbox
        public void sout(Object str)
        {
            if (WindForm.isDebug)
            {
                if (null == str) str = "null";
                this.textBox1.AppendText(str + "\r\n");
            }
        }

        private void btngo_Click(object sender, EventArgs e)
        {
            jft.runtest();
        }

        public Button getBtngo()
        {
            return this.btngo;
        }

        private void tbinputre_TextChanged(object sender, EventArgs e)
        {
            String sf = TestReg.findReg(this.tbsource.Text, this.tbinputre.Text);
            this.tbruslt.Text = sf;
        }

        private void tbruslt_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnfind_Click(object sender, EventArgs e)
        {
            tbinputre_TextChanged(sender, e);
        }


        private void btnreplace_Click(object sender, EventArgs e)
        {
            String sf = TestReg.replaceReg(this.tbsource.Text, this.tbinputre.Text, this.tbxreplx.Text);
            this.tbruslt.Text = sf;
        }

        private void tbsource_TextChanged(object sender, EventArgs e)
        {

        }
        public RichTextBox getRtbxSource()
        {
            return this.tbsource;
        }

        private void btnstop_Click(object sender, EventArgs e)
        {
            llThread.Clear();
            isCappptv = false;
            isCapyuku = false;
            isCapCntv = false;
            isCapyouku = false;
            isPiPei = false;
            isCappic = false;
            isCapqqdsj = false;
            isCapyoukudsj = false;
            isCapletv = false;
            isCapLesiudsj = false;
            isCapxunlei = false;
            isCapXunleidsj = false;
            isBuildStar = false;
            isCapTudou = false;
            isCapTudoudsj = false;
            //去掉false，避免不好看
            //btnstop.Enabled = false;
        }

        //抓qqtv
        private void btnpptv_Click()
        {
            isCappptv = true;
            this.btnpptv.Enabled = false;
            btnstop.Enabled = true;
            Capmovie.getMovieVideo();
            //Thread threadGetiFengVideo = new Thread(new ThreadStart(Capmovie.getMovieVideo));
            //threadGetiFengVideo.IsBackground = true;
            //threadGetiFengVideo.Start();
        }

        private void btnpptv_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isCappptv");
        }

        public Button getbtnpptv()
        {
            return this.btnpptv;
        }

   
        /// <summary>
        /// 设置直播8按钮
        /// </summary>
        /// <param name="irun"></param>
        public void setbtnyuku(Boolean irun)
        {
            this.btnyuku.Enabled = irun;
        }

        /// <summary>
        /// 设置优酷按钮
        /// </summary>
        /// <param name="irun"></param>
        public void setbtnUku(Boolean irun)
        {
            this.btnyouku.Enabled = irun;
        }

        /// <summary>
        /// 设置按钮
        /// </summary>
        /// <param name="irun"></param>
        public void setbtnLetv(Boolean irun)
        {
            this.btnletv.Enabled = irun;
        }

        /// <summary>
        /// 设置按钮
        /// </summary>
        /// <param name="irun"></param>
        public void setbtnXunlei(Boolean irun)
        {
            this.btnXunlei.Enabled = irun;
        }
        public void setbtnTudou(Boolean irun)
        {
            this.btnTudou.Enabled = irun;
        }

        public void setbtnJiemu(Boolean irun)
        {
            this.btnjiemu.Enabled = irun;
        }

        public void setbtnTongbuDB(Boolean irun)
        {
            this.btnTongbuDB.Enabled = irun;
        }
        public void setbtnTongbuDBTwo(Boolean irun)
        {
            this.btnUpdateDataBaseTwo.Enabled = irun;
        }

        public void setbtnTongbuDBAll(Boolean irun)
        {
            this.btnUpdateAll.Enabled = irun;
        }



        //测试
        private void btnyuku_Click(object sender, EventArgs e)
        {
            runThreadByName("isCapyuku");

        }

        private void btnyuku_Click()
        {
            try
            {
                Thread threadGetiFengVideo = new Thread(new ThreadStart(Capsport.getSportZhibo8));
                threadGetiFengVideo.IsBackground = true;
                threadGetiFengVideo.Start();
                isCapyuku = true;
                this.btnyuku.Enabled = false;
                btnstop.Enabled = true;
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }



        private void btncntv_Click()
        {
            try
            {
                Thread thcntv = new Thread(new ThreadStart(CapCntvXML.getCntvXML));
                thcntv.IsBackground = true;
                thcntv.Start();
                isCapCntv = true;
                this.btncntv.Enabled = false;
                btnstop.Enabled = true;
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }

        private void btncntv_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isCapCntv");
        }

        public void setbtncntv(Boolean irun)
        {
            this.btncntv.Enabled = irun;
        }

        public void setbtnpipei(Boolean irun)
        {
            this.btnpp.Enabled = irun;
        }

        //抓视屏的中间输出框
        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            setTextboxTxtLength(btnAll, 2700, 747);
        }


        //抓视屏的最下方输出框
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            setTextboxTxtLength(richTextBox1, 4000, 1000);
        }

          //richtextbox nf的最下方输出框
        private void rtbnf_TextChanged(object sender, EventArgs e)
        {
            setTextboxTxtLength(rtbnf, 4000, 1000);
        }
        
        //richtextbox nfjl的输出框
        private void rtbnfjl_TextChanged(object sender, EventArgs e)
        {
            setTextboxTxtLength(rtbnfjl, 4000, 1000);
        }


        //添加textbox里信息，可在线程中执行。
        public void addRtb2ForThread(Object stext)
        {
            if (WindForm.isDebug)
            {
                if (null == stext) stext = "null";
                if (this.btnAll.InvokeRequired)
                {
                    this.btnAll.Invoke((MethodInvoker)delegate { btnAll.AppendText(stext + "\r\n"); });
                }
                else
                {
                    btnAll.AppendText(stext + "\r\n");
                }
            }
        }

        //添加textboxNaifen里信息，可在线程中执行。
        public void addRtbNfft(Object stext)
        {
                if (null == stext) stext = "null";
                if (this.rtbnf.InvokeRequired)
                {
                    this.rtbnf.Invoke((MethodInvoker)delegate { rtbnf.AppendText(stext + "\r\n"); });
                }
                else
                {
                    rtbnf.AppendText(stext + "\r\n");
                }
        }

        //添加textboxNaifen历史记录里信息，可在线程中执行。
        public void addRtbNfjl(Object stext)
        {
                if (null == stext) stext = "null";
                if (this.rtbnfjl.InvokeRequired)
                {
                    this.rtbnfjl.Invoke((MethodInvoker)delegate { rtbnfjl.AppendText(stext + "\r\n"); });
                }
                else
                {
                    rtbnfjl.AppendText(stext + "\r\n");
                }
        }


        //设置textboxNaifen里信息，可在线程中执行。
        public void setRtbNfText(String stext) 
        {
            if (WindForm.isDebug)
            {
                if (null == stext) stext = "null";
                if (this.rtbnf.InvokeRequired)
                {
                    this.rtbnf.Invoke((MethodInvoker)delegate
                    {
                        rtbnf.Clear();
                        rtbnf.AppendText(stext + "\r\n");
                    });
                }
                else
                {
                    rtbnf.Clear();
                    rtbnf.AppendText(stext + "\r\n");
                }
            }         
        }

        //设置textboxNaifen里信息，可在线程中执行。
        public void settbNfjl(String stext)
        {

                if (null == stext) stext = "null";
                if (this.tbnfjl.InvokeRequired)
                {
                    this.tbnfjl.Invoke((MethodInvoker)delegate
                    {
                        tbnfjl.Text = stext;
                        
                    });
                }
                else
                {
                    tbnfjl.Text = stext;
                }
        }
        
        //设置richtextbox 的颜色
        public void addTbnfColor(Object msg, Color color)
        {
        	Apitool.setTbnfColor(rtbnf,msg,color);
			//        	int begin = rtbnf.Text.IndexOf("460915"); 
			//			int end = rtbnf.Text.LastIndexOf("460915"); 
			//			rtbnf.Select(begin, end - begin); 
			//			rtbnf.SelectionColor = Color.Blue; 
			//			rtbnf.Select(0, 0);
        }
        

        

        public void addRtb1ForThread(Object stext)
        {
            if (WindForm.isDebug)
            {
                if (null == stext) stext = "null";

                if (this.richTextBox1.InvokeRequired)
                {
                    this.richTextBox1.Invoke((MethodInvoker)delegate { richTextBox1.AppendText(stext + "\r\n"); });
                }
                else
                {
                    richTextBox1.AppendText(stext + "\r\n");
                }
            }
        }

        public void addTxtForThread(Object stext)
        {
            if (WindForm.isDebug)
            {
                if (null == stext) stext = "null";
                if (this.textBox1.InvokeRequired)
                {
                    this.textBox1.Invoke((MethodInvoker)delegate { textBox1.AppendText(stext + "\r\n"); });
                }
                else
                {
                    textBox1.AppendText(stext + "\r\n");
                }
            }
        }

        public void addTbjmForThread(Object stext)
        {
            if (WindForm.isDebug)
            {
                if (null == stext) stext = "null";
                if (this.tbjiemu.InvokeRequired)
                {
                    this.tbjiemu.Invoke((MethodInvoker)delegate { tbjiemu.AppendText(stext + "\r\n"); });
                }
                else
                {
                    tbjiemu.AppendText(stext + "\r\n");
                }
            }
        }

        public void addRTbjm(Object stext)
        {
            if (WindForm.isDebug)
            {
                if (null == stext) stext = "null";
                if (this.rtbjiemu.InvokeRequired)
                {
                    this.rtbjiemu.Invoke((MethodInvoker)delegate { rtbjiemu.AppendText(stext + "\r\n"); });
                }
                else
                {
                    rtbjiemu.AppendText(stext + "\r\n");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.tbtitle.Text.Trim() == "" || tbaddress.Text.Trim() == "")
            {
                btnAll.AppendText("请输入正确数据!" + "\r\n");
                return;
            }
            try
            {
                Capsport.inserDB(this.tbtitle.Text, this.tbaddress.Text, this.tbtype.Text);
                btnAll.AppendText(tbtitle.Text + " 成功入库!" + "\r\n");
            }
            catch (Exception ex)
            {
                btnAll.AppendText(ex.ToString());
            }
        }

        /// <summary>
        /// 设置,textbox中字符显示长度，防止太多导致卡死
        /// </summary>
        /// <param name="tb">textbox</param>
        /// <param name="lengthAll">总长度</param>
        /// <param name="lengthcut">需要cut掉的长度</param>
        private static void setTextboxTxtLength(Object tb, int lengthAll, int lengthcut)
        {
            if (tb is RichTextBox)
            {
                RichTextBox rbt = (RichTextBox)tb;
                if (rbt.Lines.Length > lengthAll)
                {
                    rbt.Text = rbt.Text.Substring(lengthcut * 20);//删除lengthcut行*20个字符
                }
            }
            else if (tb is TextBox)
            {
                TextBox rbt = (TextBox)tb;
                if (rbt.Lines.Length > lengthAll)
                {
                    rbt.Text = rbt.Text.Substring(lengthcut * 20);//删除lengthcut行*20个字符
                }
            }
        }

        /// <summary>
        /// 判断两次键盘输入时间间隔，并相应操作
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private int setKeyTime(KeyEventArgs e)
        {
            if (e.KeyValue < 65 || e.KeyValue > 90) return -1;//只管A-Z

            //根据相应拼音选定标志
            int inputTime = 0;//输入间隔时间

            if (!(this.dtnowSecond == 0 && this.dtnowms == 0))
            {
                inputTime = (DateTime.Now.Second - dtnowSecond) * 1000 + (DateTime.Now.Millisecond - dtnowms);
            }
            if (inputTime > 500 || inputTime <= 0)//重置状态,
            {
                llcharSave.Clear();
            }
            llcharSave.AddLast(e.KeyValue);
            this.dtnowSecond = DateTime.Now.Second;
            this.dtnowms = DateTime.Now.Millisecond;
            return 0;
        }

        private void cmbtxt_keyup(object sender, KeyEventArgs e)
        {
            if (setKeyTime(e) == -1) return;//只管A-Z
            string G_str_Mode = "";
            //e.keycode:  J   e.modifiers: ctrl ,alt    e.keydata:  j ctrl,alt    e.keyvalue: 74   
            string G_str_text = e.KeyCode + ": " + e.Modifiers + ": " + e.KeyData + ": " + "(" + e.KeyValue + ")";
            if (e.Shift == true)
                G_str_Mode = " Shift 键被按下 ";
            if (e.Control == true)
                G_str_Mode += "Ctrl 键被按下 ";
            if (e.Alt == true)
                G_str_Mode += "Alt 键被按下 ";
            this.addRtb1ForThread(G_str_text + G_str_Mode);

            //根据相应拼音选定标志

            int its = Charpinyin.selectComboBoxByPinyin(this.llCmbPinyin, this.llcharSave);
            if (its != -1)
            {
                this.cmbtxt.SelectedIndex = its;
            };
        }

        private void tbxaddress_keyup(object sender, KeyEventArgs e)
        {
            if (e.Alt == true && e.KeyCode == Keys.C)
            {
            	this.setMenuHideByKeyPress();
            }
            else if (e.Alt == true && e.KeyCode == Keys.P)
            {
                fnf.getFrames(wb); 
            }
        }
        
        private void setMenuHideByKeyPress()
        {
        	if(tabPage1.Parent != null)
        	{
        		hideMenuifNessary(true);
        	}
        	else
        	{
        		hideMenuifNessary(false);	
        	}       	        
        }


        public String gettextBox2Input()
        {
            if (textBox2.Text == null) return "";
            return this.textBox2.Text;
        }

        public void go_Click(String url)
        {
            if (url != null)
            {
                this.tbxaddress.Text = url;
                this.wb.Navigate(url);
            }
        }

        private void go_Click(object sender, EventArgs e)
        {
            this.wb.Navigate(this.tbxaddress.Text);
        }


        private void tbxaddress_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                go_Click(sender, e);
            }
        }

        public String getTbxAddress()
        {
            return tbxaddress.Text;
        }

        private void btncut_Click(object sender, EventArgs e)
        {
            //String st = wb.Document.Body.InnerHtml;
            StrCutWebPage = wb.Document.Body.InnerHtml.ToString();

           addRtb2ForThread(StrCutWebPage);

            //for test
            //这时页面都是topFrame在跳
           addRtb2ForThread("************************************************************************");

            ForNaifen.showFrames(wb.Document);

            //老版本的嵌套frame页面
            // HtmlDocument doc = ForNaifen.getFrameByIframeName(wb.Document, "topFrame");
            //if (doc != null)
            //{
            //    //这是两次topFrame里面
            //    ForNaifen.showFrames(doc);
                
            //}

        }


        private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //因为有的iframe框架，wb.ReadyState 始终等于 WebBrowserReadyState.Interactive
            if (wb.ReadyState >= WebBrowserReadyState.Interactive)
            {
                //事件完成(wb装载页面结束)时执行
                this.tbxaddress.Text = wb.Url.ToString(); 

                //fortest
                if (wb.ReadyState > WebBrowserReadyState.Interactive)
                    addRtb2ForThread("Url: "+e.Url+ "  Doc load complete!");

               //if (tbnfaddress.Text.Length > 20)
               //{
               //    //地址和nf一致
               //    if (e.Url.ToString().IndexOf(tbnfaddress.Text.Substring(0, 20)) != -1)
               //    {
               //        //这里去掉声音 ,add code here, by wind 15.1.25
               //        //通过firebug查看，因为下面这段播放声音代码是
               //          //layout.js在时间倒数到0时通过commonjs 动态添加的，暂时找不到替换这段js的方法
               //      //考虑方法：用webrequest获得数据，然后修改页面，然后在webbrowser中显示出来，太烦不用！
                          
               //        //<DIV style="DISPLAY: none" id=player>
               //        //<BGSOUND balance=0 volume=0 src="/wav/kaijiang.mp3" autostart="true">
               //        //</BGSOUND><audio autoplay><SOURCE src="/wav/kaijiang.mp3">
               //        //<SOURCE src="/wav/kaijiang.ogg"></audio></DIV>
               //        HtmlDocument doc = wb.Document;
               //        LinkedList<HtmlElement> llhe = ForNaifen.findElmsByOutHtml(doc, ".mp3\"", "SOURCE");
               //        LinkedList<HtmlElement> llhe2 = ForNaifen.findElmsByOutHtml(doc, ".ogg\"","SOURCE");
               //        LinkedList<HtmlElement> llhe3 = ForNaifen.findElmsByOutHtml(doc, ".mp3\"", "BGSOUND");
               //        if (llhe != null)
               //        {
               //            foreach (HtmlElement hel in llhe)
               //            {
               //                hel.SetAttribute("src", "");
               //                addRtb2ForThread("清除MP3成功！");

               //            }
               //        }
               //        if (llhe2 != null)
               //        {
               //            foreach (HtmlElement hel in llhe2)
               //            {
               //                hel.SetAttribute("src", "");
               //                addRtb2ForThread("清除ogg成功");

               //            }
               //        }
               //        if (llhe3 != null)
               //        {
               //            foreach (HtmlElement hel in llhe3)
               //            {
               //                hel.SetAttribute("src", "");
               //                addRtb2ForThread("清除MP3成功2");
               //            }
               //        }
               //    }
               //}
             
                //for test
               //ForNaifen.setWebbroConfirmDlg(wb.Document);

               //var htmlDoc = (IHTMLDocument3)wb.Document.DomDocument;
               //HTMLHeadElement head = htmlDoc.getElementsByTagName("head").Cast<HTMLHeadElement>().First();
               //var script = (IHTMLScriptElement)((IHTMLDocument2)htmlDoc).createElement("script");
               //script.text = "window.onload=function() { confirm('test') }";
               //head.appendChild((IHTMLDOMNode)script);
             

                //若要执行某个子类的自定义方法，应该  
                //定义一个接口，子窗体对象实现这个接口并把该目标方法提升为该接口的成员。
                //由主窗体适时调用这个接口成员方法。
                
                ////for test           
                  while (llIfaceWB.Count > 0)
                {
                    InterfaceWBDoc ifwbdoc = llIfaceWB.First.Value;
                    //调用一次删除一个，用时再加              
                    llIfaceWB.RemoveFirst();
                    ifwbdoc.doAfterDocumentCompleted();
                }
            }
        }


        public String getCutString()
        {
            StrCutWebPage = wb.Document.Body.InnerHtml.ToString();
            return StrCutWebPage;
        }

        public WebBrowser getWebBrowser()
        {
            return this.wb;
        }

        public WebBrowser getWebBrowser2()
        {
            return this.wb2;
        }

        //通过模拟点击获得页面
        private void getQQPageByClick()
        {
            try
            {
                Monitor.Enter(lockObj);
                String info = wb.Document.Body.InnerHtml.ToString();

                Capmovie.getPageDetail(Apis.shtmlQuick(info));

                if (Capmovie.isLastpage) return;
                HtmlDocument doc = wb.Document;
                HtmlElement btn = null;
                foreach (HtmlElement em in doc.All) //轮循
                {
                    if (em != null)
                    {
                        //    string str = em.Id;
                        //    if (str != null && str.Trim() != "")
                        //    {
                        //        //UseStatic.sout(str);
                        //        if (str == "next") //减少处理
                        //        {
                        //            btn = em;
                        //            break;
                        //        }
                        //    }
                        if (em.TagName == "A")
                        {
                            if (em.OuterText == "下一页")
                            {
                                //UseStatic.sout(em.TagName + "1");
                                //UseStatic.sout(em.OuterText + "3");
                                //UseStatic.sout(em.OuterHtml + "4");
                                btn = em;
                                break;
                            }
                        }
                    }
                }
                if (btn != null)
                    btn.InvokeMember("click"); //触发submit事件
                //while (strCut == "")//qq视频返回页面有两次加载，第一次不是实际数据，是”努力获取中“，所以该方法无效！
                //{
                //    Application.DoEvents();//等待本次加载完毕才执行下次循环.
                //}
                //return strCut;
            }
            catch (Exception e)
            {
                addRtb2ForThread(e.StackTrace);
            }
            finally
            {
                Monitor.Exit(lockObj);
            }
        }

        public void getPageByTimer()
        {
            //TimerCallback timerDelegate = new TimerCallback(getQQPageByClick);
            //System.Threading.Timer tt = new System.Threading.Timer(timerDelegate, null, 0, 4000);
            String surl = Capmovie.llUrls.First.Value;
            Capmovie.llUrls.RemoveFirst();
            wb.Navigate(surl);
            if (surl == Capmovie.sUrlQQDsj) Capmovie.iTvnumum = Capmovie.ITvNum[1];
            else if (surl == Capmovie.sUrlQQDy) Capmovie.iTvnumum = Capmovie.ITvNum[0];
            else if (surl == Capmovie.sUrlQQmusic) Capmovie.iTvnumum = Capmovie.ITvNum[5];
            else
            {
                int Itvnum = CapYouku.getParaByInput("N");   //供手动修改type和页数
                if (Itvnum == -1)
                {
                    addRtb2ForThread("请输入tvnum! year, example:  y-2012|n-0");
                    getbtnpptv().Invoke((MethodInvoker)delegate { getbtnpptv().Enabled = true; });
                    return;
                }
            }

            myTimer = new System.Windows.Forms.Timer();
            myTimer.Tick += new EventHandler(TimerEvent);
            myTimer.Interval = 5000;
            myTimer.Start();
            // Runs the timer, and raises the event.
            //while (exitFlag == false)
            //{
            //    // Processes all the events in the queue.
            //    Application.DoEvents();
            //}
        }

        private void TimerEvent(Object myObject, EventArgs myEventArgs)
        {
            myTimer.Stop();

            getQQPageByClick();
            if (isCappptv)
            {
                this.iTimeCount++;
                if (iTimeCount < Capmovie.IQQMovieTime && (!Capmovie.isLastpage))
                {
                    addRtb2ForThread("\r\n开始抓第 " + iTimeCount + " Page数据!======================================");
                    myTimer.Enabled = true;
                }
                else
                {
                    if (Capmovie.llUrls.Count == 0)
                    {
                        isCappptv = false;
                        iTimeCount = 0;
                        btnpptv.Enabled = true;
                        addRtb2ForThread("所有" + " 数据已被抓取======================================");
                        go_Click("http://www.baidu.com");   //要把页面置回来
                        WindForm.oneCatchEnd("isCappptv");
                    }
                    else
                    {
                        iTimeCount = 0;
                        addRtb2ForThread("\r\n开始抓 " + Capmovie.llUrls.First.Value + "页面数据!======^-^");
                        getPageByTimer();
                    }
                }
            }
            else
            {  //人工终止后设置参数
                iTimeCount = 0;
                this.btnpptv.Enabled = true;
            }
        }

        private void btnyouku_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isCapyouku");
        }

        private void btnyouku_Click()
        {
            try
            {
                Thread tyk = new Thread(new ThreadStart(CapYouku.getYoukuVideo));
                tyk.IsBackground = true;
                tyk.Start();
                isCapyouku = true;
                this.btnyouku.Enabled = false;
                btnstop.Enabled = true;
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }

        private void btnpp_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isPiPei");
        }

        private void btnpp_Click()
        {
            try
            {
                Thread tyk = new Thread(new ThreadStart(Pipei.piPeiMovie));
                tyk.IsBackground = true;
                tyk.Start();
                isPiPei = true;
                this.btnpp.Enabled = false;
                btnstop.Enabled = true;
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }

        private void menuabout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "      Testwindc V2.012      " + "\r\n" + "          by wind        ", "关于本程式", MessageBoxButtons.OK);

            //Form testDialog = new Form();

            //// Show testDialog as a modal dialog and determine if DialogResult = OK.
            //if (testDialog.ShowDialog(this) == DialogResult.OK)
            //{

            //    //this.txtResult.Text = testDialog.TextBox1.Text;

            //}
            //else
            //{
            //    //this.txtResult.Text = "Cancelled";
            //}
            ////模式对话框，暂停
            ////richTextBox2.AppendText("lalala" + "\r\n");
            //testDialog.Dispose();
        }


        private void menucappic_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isCappic");

        }

        private void menucappic_Click()
        {
            try
            {
                if (!isCappic)
                {
                    isCappic = true;
                    Thread tyk = new Thread(new ThreadStart(Cappic.capicture));
                    tyk.IsBackground = true;
                    tyk.Start();
                    btnstop.Enabled = true;
                }
                else
                {
                    btnAll.AppendText("抓取程序已经在运行======================" + "\r\n");
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }

        private void menucapqqdsj_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isCapqqdsj");
        }

        private void menucapqqdsj_Click()
        {
            try
            {
                if (!isCapqqdsj)
                {
                    isCapqqdsj = true;
                    Thread tyk = new Thread(new ThreadStart(Capmovie.capQQdsj));
                    tyk.IsBackground = true;
                    tyk.Start();
                    btnstop.Enabled = true;
                }
                else
                {
                    btnAll.AppendText("抓取程序已经在运行======================" + "\r\n");
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }

        private void menuCapyouku_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isCapyoukudsj");
        }

        private void menuCapyouku_Click()
        {
            try
            {
                if (!isCapyoukudsj)
                {
                    isCapyoukudsj = true;
                    Thread tyk = new Thread(new ThreadStart(CapYouku.capYoukudsj));
                    tyk.IsBackground = true;
                    tyk.Start();
                    btnstop.Enabled = true;
                }
                else
                {
                    btnAll.AppendText("抓取程序已经在运行======================" + "\r\n");
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }

        private void menuCapLesi_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isCapLesiudsj");
        }

        private void menuCapLesi_Click()
        {
            try
            {
                if (!isCapLesiudsj)
                {
                    isCapLesiudsj = true;
                    Thread tyk = new Thread(new ThreadStart(Capletv.capLesidsj));
                    tyk.IsBackground = true;
                    tyk.Start();
                    btnstop.Enabled = true;
                }
                else
                {
                    btnAll.AppendText("抓取程序已经在运行======================" + "\r\n");
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }

        private void btnletv_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isCapletv");
        }

        private void btnletv_Click()
        {
            try
            {
                Thread tyk = new Thread(new ThreadStart(Capletv.getLetvVideo));
                tyk.IsBackground = true;
                tyk.Start();
                isCapletv = true;
                this.btnletv.Enabled = false;
                btnstop.Enabled = true;
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }

        private void btnXunlei_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isCapxunlei");
        }

        private void btnXunlei_Click()
        {
            try
            {
                Thread tyk = new Thread(new ThreadStart(CapXunlei.getVideo));
                tyk.IsBackground = true;
                tyk.Start();
                isCapxunlei = true;
                this.btnXunlei.Enabled = false;
                btnstop.Enabled = true;
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }


        private void menuCapXunlei_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isCapXunleidsj");
        }

        private void menuCapXunlei_Click()
        {
            try
            {
                if (!isCapXunleidsj)
                {
                    isCapXunleidsj = true;
                    Thread tyk = new Thread(new ThreadStart(CapXunlei.capXunleidsj));
                    tyk.IsBackground = true;
                    tyk.Start();
                    btnstop.Enabled = true;
                }
                else
                {
                    btnAll.AppendText("抓取程序已经在运行======================" + "\r\n");
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }

        private void menuTongbu_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isTongbu");
        }

        private void menuTongbu_Click()
        {
            try
            {
                //if (!isTongbu)
                {
                    isTongbu = true;
                    Thread tyk = new Thread(new ThreadStart(Tongbu.tongbuDb));
                    tyk.IsBackground = true;
                    tyk.Start();
                    //btnstop.Enabled = true;
                }
                //else
                //{
                //    richTextBox2.AppendText("抓取程序已经在运行======================" + "\r\n");
                //}
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }

        /// <summary>
        /// 某线程执行结束时，通知链表执行队列中下一项,本函数线程安全
        /// </summary>
        /// <param name="name"></param>
        public static void oneCatchEnd(String name)
        {
            lock (llThread)
            {
                if (llThread.Count == 0) return;
                if (llThread.Contains(name))
                {
                    llThread.Remove(name);
                }

                UseStatic.getWindForm().runThreadByName("");
            }
        }


        private void runThreadByName(String name)
        {
            if (name != "")
            {
                if (llThread.Count != 0)
                {
                    if (llThread.Contains(name))
                    {
                        btnAll.AppendText(name + " 线程已经在运行===========================================" + "\r\n");
                        return;
                    }

                    llThread.AddLast(name);
                    return;
                }
                llThread.AddLast(name);
            }
            else
            {
                if (llThread.Count == 0) return;
                name = llThread.First.Value;
            }


            switch (name)
            {
                case "isCappptv":
                    if (!isCappptv)
                        btnpptv_Click();
                    break;
                case "isCapyuku":
                    if (!isCapyuku)
                        btnyuku_Click();
                    break;
                case "isCapCntv":
                    if (!isCapCntv)
                        btncntv_Click();
                    break;
                case "isCapyouku":
                    if (!isCapyouku)
                        btnyouku_Click();
                    break;
                case "isPiPei":
                    if (!isPiPei)
                        btnpp_Click();
                    break;
                case "isCappic":
                    if (!isCappic)
                        menucappic_Click();
                    break;
                case "isCapqqdsj":
                    if (!isCapqqdsj)
                        menucapqqdsj_Click();
                    break;
                case "isCapyoukudsj":
                    if (!isCapyoukudsj)
                        menuCapyouku_Click();
                    break;
                case "isCapLesiudsj":
                    if (!isCapLesiudsj)
                        menuCapLesi_Click();
                    break;
                case "isCapXunleidsj":
                    if (!isCapXunleidsj)
                        menuCapXunlei_Click();
                    break;
                case "isCapletv":
                    if (!isCapletv)
                        btnletv_Click();
                    break;
                case "isCapxunlei":
                    if (!isCapxunlei)
                        btnXunlei_Click();
                    break;
                case "isCapTudou":
                    if (!isCapTudou)
                        btnTudou_Click();
                    break;
                case "isTongbu":
                    if (!isTongbu)
                        menuTongbu_Click();
                    break;
                case "isBuildStar":
                    if (!isBuildStar)
                        menubuildstar_click();
                    break;
                default:
                    break;
            }

        }

        private void menubuildstar_click()
        {
            try
            {
                Thread tyk = new Thread(new ThreadStart(BuildStar.makeStarNow));
                tyk.IsBackground = true;
                tyk.Start();
                isBuildStar = true;
                btnstop.Enabled = true;
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }

        private void menubuildstar_click(object sender, EventArgs e)
        {
            this.runThreadByName("isBuildStar");
        }

        private void BtnCapAll_Click()
        {
            try
            {

                richTextBox1.AppendText("开始抓取所有数据: " + DateTime.Now.ToString() + " 每隔24小时重复抓取！" + "\r\n");

                ////自动抓取所有数据 //不匹配

                this.runThreadByName("isCappptv");
                this.runThreadByName("isCapCntv");

                //this.runThreadByName("isCapyuku");
                this.runThreadByName("isCapyouku");
                this.runThreadByName("isCapletv");
                this.runThreadByName("isCapxunlei");
                this.runThreadByName("isCapXunleidsj");
                this.runThreadByName("isCapLesiudsj");
                this.runThreadByName("isCapyoukudsj");
                this.runThreadByName("isCapqqdsj");
                this.runThreadByName("isCapTudou");

                //this.runThreadByName("isPiPei");
                this.runThreadByName("isTongbu");
                this.runThreadByName("isCappic");

                //Thread.Sleep(15000);
                Thread.Sleep(3600000 * 24);//1000*60*60*12 = 60000*720
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
            finally
            {

            }
            BtnCapAll_Click();

        }

        private void BtnCapAll_Click(object sender, EventArgs e)
        {
            Thread tyk = new Thread(new ThreadStart(BtnCapAll_Click));
            tyk.IsBackground = true;
            tyk.Start();
            BtnCapAll.Enabled = false;
        }

        private void btnjiemu_Click(object sender, EventArgs e)
        {
            if (this.rabtn1.Checked) { BuildJiemu.iWeek = 0; }
            else if (this.rabtn2.Checked)
            { BuildJiemu.iWeek = 7; }

            BuildJiemu.iTimeStep = this.cmbTimeStep.SelectedIndex;

            Thread tyk = new Thread(new ThreadStart(BuildJiemu.makeJiemuNow));
            tyk.IsBackground = true;
            tyk.Start();
            this.btnjiemu.Enabled = false;
        }

        public void getCmbTimeStepSlct()
        {
            if (this.cmbTimeStep.InvokeRequired)
            {
                int isdex = 0;
                this.cmbTimeStep.Invoke((MethodInvoker)delegate
                {
                    isdex = this.cmbTimeStep.SelectedIndex;
                    BuildJiemu.setTimeStep(isdex);
                });

            }
            else
            {
                BuildJiemu.setTimeStep(this.cmbTimeStep.SelectedIndex);
            }
        }



        private void btnFilePath_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog folder = new FolderBrowserDialog();

            if (folder.ShowDialog() == DialogResult.OK)
            {
                String path3 = folder.SelectedPath + "\\";

                if (path3 == " ")
                {
                    return;
                }
                else
                {
                    BuildJiemu.saveUrl = path3;
                    Apis.setValueByStrName(System.Environment.CurrentDirectory + BuildJiemu.STRConfigFile, BuildJiemu.Str_epgtxt, path3);
                    addTbjmForThread("已设置输出目录为: " + path3);

                }
            }

        }

        private void cbxjiemu_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxjiemu.Checked)
            {
                BuildJiemu.isForceAll = true;
            }
            else
                BuildJiemu.isForceAll = false;
        }

        private void btnTudou_Click(object sender, EventArgs e)
        {
            this.runThreadByName("isCapTudou");
        }
        private void btnTudou_Click()
        {
            try
            {
                Thread tyk = new Thread(new ThreadStart(CapTudou.getTudouVideo));
                tyk.IsBackground = true;
                tyk.Start();
                isCapTudou = true;
                this.btnTudou.Enabled = false;
                btnstop.Enabled = true;
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }


        private void menuCapTudou_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isCapTudoudsj)
                {
                    isCapTudoudsj = true;
                    Thread tyk = new Thread(new ThreadStart(CapTudou.CapTudoudsj));
                    tyk.IsBackground = true;
                    tyk.Start();
                    btnstop.Enabled = true;
                }
                else
                {
                    btnAll.AppendText("抓取程序已经在运行======================" + "\r\n");
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.ToString() + "\r\n");
            }
        }

        private void btnTongbuDB_Click(object sender, EventArgs e)
        {
            Thread tyk = new Thread(new ThreadStart(TongbuDB.TongbuDBNow));
            tyk.IsBackground = true;
            tyk.Start();
            this.btnTongbuDB.Enabled = false;

        }

        private void cmbTongBu_SelectedIndexChanged(object sender, EventArgs e)
        {
            TongbuDB.iTimeStep = cmbTongBu.SelectedIndex;

        }

        private void btnTbDB_Click(object sender, EventArgs e)
        {
            Thread tyk = new Thread(new ThreadStart(TongbuDB.TongbuDBOther));
            tyk.IsBackground = true;
            tyk.Start();
            this.btnTbDB.Enabled = false;
        }

        public String getTbPingdao()
        {
            String st = "";
            if (this.tBPingdao.InvokeRequired)
            {
                this.tBPingdao.Invoke((MethodInvoker)delegate { st = tBPingdao.Text; });
            }
            else
            {
                st = tBPingdao.Text;
            }
            return st;

        }
        #region 单挑sql同步数据库
        // 该按钮只供测试或者技术使用，设置不可见 ，by wind ,2012.6.8
        private void btnUpdateDataBaseTwo_Click(object sender, EventArgs e)
        {
            Thread tyk = new Thread(new ThreadStart(TongbuDB.TongbuDBNowTwo));
            tyk.IsBackground = true;
            tyk.Start();
            this.btnUpdateDataBaseTwo.Enabled = false;
        }
        #endregion

        private void WindForm_Load(object sender, EventArgs e)
        {

        }

        //该按钮只供测试或者技术使用，设置不可见 ，by wind ,2012.6.8
        private void btnUpdateAll_Click(object sender, EventArgs e)
        {

            Thread tyk = new Thread(new ThreadStart(TongbuDB.TongbuDBNowALL));
            tyk.IsBackground = true;
            tyk.Start();
            this.btnUpdateAll.Enabled = false;
        }

        private void btntcut_Click(object sender, EventArgs e)
        {
            try
            {
                //Thread thframes = new Thread(new ThreadStart(ForNaifen.getFrames));
                //thframes.IsBackground = true;
                //thframes.Start();

              //  fnf.getFrames(wb);
              
               //fnf.doBetByStrategy(wb);
               // fnf.testScript(); 
               //wb.Navigate("file:///D:/work/temp/testfirm.htm");

                //String st = Apitool.getFileTxt(System.Environment.CurrentDirectory + "\\tmp.txt", "");
                //String value = ForNaifen.getWinMoney(st);

                //addRtbNfft("盈亏是: " + value);

                //String value = ForNaifen.getWinMoeyByPage();
                //addRtbNfft("主进程盈亏是: " + value);

                Thread thmon = new Thread(new ThreadStart(ForNaifen.getWinMoeyByPage));
                thmon.IsBackground = true;
                thmon.Start();
                
            }
            catch(Exception ex)
            {
                this.addRtb2ForThread(ex.ToString());
            }
        }


        private void btnfnf_Click(object sender, EventArgs e)
        {
            //fnf.goFrame(wb);
            //fnf.doBet(wb);
            String mp3file = System.Environment.CurrentDirectory + @"\alarm.mid";

            Apitool.playMp3(mp3file);
        }

        private void btnpic_Click(object sender, EventArgs e)
        {
            //fnf.getPic(wb); 
            fnf.getFrames(wb);
         
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void btnNfSave_Click(object sender, EventArgs e)
        {
            TextBox[] stmp = new TextBox[]{this.tbnf11, this.tbnf12, this.tbnf21, this.tbnf22,this.tbnf31,
                this.tbnfw1,this.tbnfw2,this.tbnfw3,this.tbnfw4,this.tbnfw5,
        	                         this.tbnft11, this.tbnft12, this.tbnft21, this.tbnft22,this.tbnft31,
                                     this.tbnftw1,this.tbnftw2,this.tbnftw3,this.tbnftw4,this.tbnftw5};
        	ForNaifen.setConfigFileByName( stmp );

            Apis.setValueByStrName(ForNaifen.STRConfigFile, tbnfqsbs1.Name, tbnfqsbs1.Text);
            Apis.setValueByStrName(ForNaifen.STRConfigFile, tbnfqsbs2.Name, tbnfqsbs2.Text);
            Apis.setValueByStrName(ForNaifen.STRConfigFile, tbnflatertime.Name, tbnflatertime.Text);
            
            Apis.setValueByStrName(ForNaifen.STRConfigFile, tbnfuser.Name, tbnfuser.Text); 
            Apis.setValueByStrName(ForNaifen.STRConfigFile, tbnfpwd.Name, Apitool.Encode(tbnfpwd.Text));
            Apis.setValueByStrName(ForNaifen.STRConfigFile, tbnfaddress.Name, tbnfaddress.Text);

            Apis.setValueByStrName(ForNaifen.STRConfigFile, this.tbnfzyin.Name, tbnfzyin.Text);
            Apis.setValueByStrName(ForNaifen.STRConfigFile, this.tbnfzs.Name, tbnfzs.Text);


            setNaifenAddValue();

            MessageBox.Show(this, "设置信息已经保存!","保存");
        }



        private void setTextBoxNaifenFromFile2()
        {
            if (tbNall == null)
            {
                tbNal1 = new TextBox[] { this.tbna11, this.tbna12, this.tbna13, this.tbna14, this.tbna15,
                    this.tbna16, this.tbna17, this.tbna18, this.tbna19};
              
                tbNal2 = new TextBox[] { this.tbna21, this.tbna22, this.tbna23, this.tbna24, this.tbna25,
                    this.tbna26, this.tbna27, this.tbna28, this.tbna29};

                tbNal3 = new TextBox[] { this.tbna31, this.tbna32, this.tbna33, this.tbna34, this.tbna35,
                    this.tbna36, this.tbna37, this.tbna38, this.tbna39};

                tbNal4 = new TextBox[] { this.tbna41, this.tbna42, this.tbna43, this.tbna44, this.tbna45,
                    this.tbna46, this.tbna47, this.tbna48, this.tbna49};

                tbNal5 = new TextBox[] { this.tbna51, this.tbna52, this.tbna53, this.tbna54, this.tbna55,
                    this.tbna56, this.tbna57, this.tbna58, this.tbna59};

                tbNal6 = new TextBox[] { this.tbna61, this.tbna62, this.tbna63, this.tbna64, this.tbna65,
                    this.tbna66, this.tbna67, this.tbna68, this.tbna69};

                tbNal7 = new TextBox[] { this.tbna71, this.tbna72, this.tbna73, this.tbna74, this.tbna75,
                    this.tbna76, this.tbna77, this.tbna78, this.tbna79};

                tbNal8 = new TextBox[] { this.tbna81, this.tbna82, this.tbna83, this.tbna84, this.tbna85,
                    this.tbna86, this.tbna87, this.tbna88, this.tbna89};

                tbNal9 = new TextBox[] { this.tbna91, this.tbna92, this.tbna93, this.tbna94, this.tbna95,
                    this.tbna96, this.tbna97, this.tbna98, this.tbna99};
                tbNal10 = new TextBox[] { this.tbna101, this.tbna102, this.tbna103, this.tbna104, this.tbna105,
                    this.tbna106, this.tbna107, this.tbna108, this.tbna109};

                tbNall = new TextBox[][] { tbNal1, tbNal2, tbNal3, tbNal4, tbNal5, tbNal6, tbNal7, tbNal8, tbNal9, tbNal10 };
            }

            //从配置文件读初始值，add code here
            for (int ik = 0; ik < tbNall.Length; ik++)
            {
                TextBox[] tbnatmp = tbNall[ik];
                for (int it = 0; it < tbnatmp.Length; it++)
                {
                    TextBox tbt = tbnatmp[it];
                    tbt.MaxLength = 4;
                    if (tbt.Text.Equals(""))
                    {
                        tbt.Text = "2";
                    }
                }
            }

        }


        private void setTextBoxNaifenFromFile()
        {
         
            //设置值
            TextBox[] stmp = new TextBox[]{this.tbnf11, this.tbnf12, this.tbnf21, this.tbnf22,this.tbnf31,
                this.tbnfw1,this.tbnfw2,this.tbnfw3,this.tbnfw4,this.tbnfw5,
        	                         this.tbnft11, this.tbnft12, this.tbnft21, this.tbnft22,this.tbnft31,
                                     this.tbnftw1,this.tbnftw2,this.tbnftw3,this.tbnftw4,this.tbnftw5};
            String[] sgv = ForNaifen.getConfigFileByName(stmp);

            for (int ik = 0; ik < stmp.Length; ik++)
            {
                if (!(sgv[ik].Equals("")))
                {
                    stmp[ik].Text = sgv[ik];
                }             
            }
            //起始倍数
           String st = Apis.getValueByStrName(ForNaifen.STRConfigFile, "tbnfqsbs1");
           if (st.Equals("")){
               st = "5";} //默认值为5
           tbnfqsbs1.Text = st;

           ForNaiStrategy.iDanCpu = Int32.Parse(st);
           ForNaiStrategy.istartnumdan = ForNaiStrategy.iDanCpu;

           st = Apis.getValueByStrName(ForNaifen.STRConfigFile, "tbnfqsbs2");
            if (st.Equals(""))
            {
                st = "5";
            }
            tbnfqsbs2.Text = st;
            ForNaiStrategy.iTwoCpu = Int32.Parse(st);
            ForNaiStrategy.istartnumtwo = ForNaiStrategy.iTwoCpu;

            st =Apis.getValueByStrName(ForNaifen.STRConfigFile, "tbnflatertime");
            if (st.Equals(""))
            {
                st = "6";
            }
            tbnflatertime.Text = st;
            ForNaiStrategy.istartLaterTime = Int32.Parse(st);
            
            st = Apis.getValueByStrName(ForNaifen.STRConfigFile, tbnfuser.Name); 
            tbnfuser.Text = st;
            
            st = Apis.getValueByStrName(ForNaifen.STRConfigFile, tbnfpwd.Name);
            tbnfpwd.Text = Apitool.Decode(st);
            
            st = Apis.getValueByStrName(ForNaifen.STRConfigFile, tbnfaddress.Name); 
             tbnfaddress.Text = st;

             st = Apis.getValueByStrName(ForNaifen.STRConfigFile, tbnfzyin.Name);
             tbnfzyin.Text = st;

             st = Apis.getValueByStrName(ForNaifen.STRConfigFile, tbnfzs.Name);
             tbnfzs.Text = st;


            //读取两方案最大值
             String sdt = Apis.getValueByStrName(ForNaifen.STR_LogFile, "参数最大值", "M", "单方案");
             String stt = Apis.getValueByStrName(ForNaifen.STR_LogFile, "参数最大值", "M", "双方案");
             int idt = -1;
             if (!sdt.Equals("")){
                 Int32.TryParse(sdt, out idt);
             }
             ForNaiStrategy.iDanMax = Math.Max(idt, ForNaiStrategy.iDanMax);
             ForNaiStrategy.iDanMax = Math.Max(ForNaiStrategy.iDanMax, ForNaiStrategy.iDanCpu);
            // Apis.setValueByStrName(ForNaifen.STR_LogFile, "参数最大值", ForNaiStrategy.iDanMax + "", "M", "单方案");
             idt = -1;
             if (!stt.Equals("")){
                 Int32.TryParse(stt, out idt);
             }
             ForNaiStrategy.iTwoMax = Math.Max(idt, ForNaiStrategy.iTwoMax);
             ForNaiStrategy.iTwoMax = Math.Max(ForNaiStrategy.iTwoCpu, ForNaiStrategy.iTwoMax);
             //Apis.setValueByStrName(ForNaifen.STR_LogFile, "参数最大值", idt + "", "M", "双方案");
             setTbnfState("* 左方案最大值: " + ForNaiStrategy.iDanMax + "  右方案最大值: " + 
                 ForNaiStrategy.iTwoMax);

             setNaifenAddValue();
            
            //设置历史记录页面
            tbnfsetup.MaxLength = 9;
            this.settbNfjl("记录初始化完成");

            addRtb2ForThread(" --------------设置已经读取！-----------------");
        }




        private void btnnfstart_Click(object sender, EventArgs e)
        {
            btnNaifen_Click();      
        }

        //运行naifen
        private void btnNaifen_Click()
        {
            isRunNaifen = true;
            this.btnnfstart.Enabled = false;
            this.btnnfstop.Enabled = true;
            isRunNaifen = true;

            addRtb2ForThread(DateTime.Now.ToString()+" --------------自动下单开始-----------------");
            addRtbNfft(DateTime.Now.ToString() + " --------------自动下单开始-----------------");
            try
            {
                ////清除历史列表// //这里应该用已经运行的数据来进行启动.,by wind 1.22
                //ForNaiStrategy.clearHistory();
               //setNaifenAddValue();              

                //先执行一次，然后启动定时器
                ForNaifen.iturnpage = 0;
                fnf.doBetByStrategy(wb);

                fnf.dobetByTimer();
            }
            catch (Exception ex)
            {
                addRtb2ForThread(ex.ToString() + "\r\n");
                this.btnnfstart.Enabled = true;
            }
            finally
            {
            }

        }

        public void btnnfstop_Click(object sender, EventArgs e)
        {
             this.btnnfstop.Enabled = false;
             isRunNaifen = false;
             fnf.stopTimer();

             addRtb2ForThread((DateTime.Now.ToString() + "  ====自动下单停止！==========="));
             addRtbNfft((DateTime.Now.ToString() + "  ====自动下单停止！==========="));          
             this.btnnfstart.Enabled = true;
        }

       public Button getbtnNaifen()
        {
            return this.btnnfstart;
        }

       //是否自动翻页
       public Boolean getcbnfAutopage()
       {
           return this.cbnfautopage.Checked;
       }

        //根据当前界面输入设置naifen投注初始值，但保留运行值
        public void setNaifenAddValue()
        {
            TextBox[] stmp = new TextBox[]{this.tbnf11, this.tbnf12, this.tbnf21, this.tbnf22,this.tbnf31,
                this.tbnfw1,this.tbnfw2,this.tbnfw3,this.tbnfw4,this.tbnfw5,
        	                         this.tbnft11, this.tbnft12, this.tbnft21, this.tbnft22,this.tbnft31,
                                     this.tbnftw1,this.tbnftw2,this.tbnftw3,this.tbnftw4,this.tbnftw5};

            ForNaiStrategy.setTbValue(stmp);
            //运行值需要保留
            if (ForNaiStrategy.iDanCpu == -1)
            {
                ForNaiStrategy.iDanCpu = Apitool.getIntValue(tbnfqsbs1.Text);
            }
            if (ForNaiStrategy.iTwoCpu == -1)
            {
                ForNaiStrategy.iTwoCpu = Apitool.getIntValue(tbnfqsbs2.Text);
            }
            if (ForNaiStrategy.iDanCput == -1)
            {
                ForNaiStrategy.iDanCput = Apitool.getIntValue(tbnfqsbs1.Text);
            }
            if (ForNaiStrategy.iTwoCput == -1)
            {
                ForNaiStrategy.iTwoCput = Apitool.getIntValue(tbnfqsbs2.Text);
            }

            ForNaiStrategy.istartLaterTime = Apitool.getIntValue(tbnflatertime.Text);
        }

        public int getTbnfqsbs1value() 
        {
        	 int itmp = -1;
        	   if (this.tbnfqsbs1.InvokeRequired)
                {
        	   		
                     this.tbnfqsbs1.Invoke((MethodInvoker)delegate {itmp = Apitool.getIntValue(tbnfqsbs1.Text); });
                      return itmp;
                }
                else
                {
                   return Apitool.getIntValue(tbnfqsbs1.Text);
                }
                
        }

        public int getTbnfqsbs2value()
        {
        	int itmp = -1;
        	  if (this.tbnfqsbs2.InvokeRequired)
                {
                     this.tbnfqsbs2.Invoke((MethodInvoker)delegate {itmp = Apitool.getIntValue(tbnfqsbs2.Text); });
                      return itmp;
                }
                else
                {
                   return Apitool.getIntValue(tbnfqsbs2.Text);
                }
               
        }

        ////防止webbrowser在新窗口(IE)中打开新建窗口。
        //private void webBrowser_1_NewWindow(object sender, CancelEventArgs e)
        //{
        //    WebBrowser webBrowser_temp = (WebBrowser)sender;
        //    string newUrl = webBrowser_temp.Document.ActiveElement.GetAttribute("href");
        //    this.wb.Url = new Uri(newUrl);
        //    e.Cancel = true;
        //}  


        
        void BtnnfbtClick(object sender, EventArgs e)
        {
        	fnf.dobBetForce();
        }


        private void tooltipNfAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "      Auther:    wind   ,  2014/12  " + "\r\n" + "    联系方式  QQ: 645803940       "
               + "\r\n" + "        email:  gjlsx@163.com    ", "关于本程式", MessageBoxButtons.OK);


        }

        private void tooltipitemNfexit_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.
                Show("您确定要退出程序吗？","确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                     
               // c#通过Application.Exit();无法退出单独开启的线程
                // Environment.Exit(0);    可以解决问题。??
                Environment.Exit(0);

            }
            else
            {
                  //点取消的代码 
            }
        }

        public Boolean getCheckBoxNfDebug() 
        {
            return cbnfdebug.Checked;
        }

        public String getNfAddress()
        {
            return tbnfaddress.Text;
        }

        public String geTBNfzyin()
        {
            String sr = "";
            if (this.tbnfzyin.InvokeRequired)
            {
                this.tbnfzyin.Invoke((MethodInvoker)delegate { sr = tbnfzyin.Text; });
            }
            else
            {
                sr = tbnfzyin.Text;
            }
            return sr;
        }

        public String getTBNfzsun()
        {
            String sr = "";
            if (this.tbnfzs.InvokeRequired)
            {
                this.tbnfzs.Invoke((MethodInvoker)delegate { sr = tbnfzs.Text; });
            }
            else
            {
                sr = tbnfzs.Text;
            }
            return sr;
        }

        public String getNfLoginUser()
        {
            return tbnfuser.Text;
        }

        public String getNfLoginPwd()
        {
            return tbnfpwd.Text;
        }


        
        void BtnnfLoginClick(object sender, EventArgs e)
        {                 	
            this.tblife.SelectTab("tabPage2");
            fnf.goFrame(wb);
           
        }

        private void menutest_Click(object sender, EventArgs e)
        {
           
                fnf.testMenuitem();
          
        }

        //测试两面盘
        private void menuitemlmp_Click(object sender, EventArgs e)
        {      
            fnf.goLmp(wb);
        }

        private void gethtelmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                fnf.testHtmlElement();
             }
            catch (Exception ex)
            {              
                Console.WriteLine(ex.ToString());
            }
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.
               Show("是否要清空数据库中所有历史开奖数据？", "确认", MessageBoxButtons.OKCancel,
               MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                String sqldel = "delete from kjhis where id > 0";
                try
                {
                    int iresult = -1;
                    iresult = Apis.runSqlite(sqldel, "");
                    addRtbNfft("历史数据已经清除！");

                }
                catch (Exception ex)
                {
                    addRtbNfft(ex.StackTrace);
                }
                finally
                {
                }                         
            }
            else
            { 
            }
              
        }

        private void mitemRedoNf_Click(object sender, EventArgs e)
        {
            //清除历史数据
            DialogResult dr = MessageBox.
                Show("是否停止程序运行并清除运行数据？", "确认", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                btnnfstop_Click(null, null);
                setTextBoxNaifenFromFile();
                ForNaiStrategy.clearHistory();
                //btnNaifen_Click();                             
            }
            else
            {}
        }

        public void setTbnfState(String stext)
        {
                if (null == stext)
                    return;
                if (this.tbnfstate.InvokeRequired)
                {
                    this.tbnfstate.Invoke((MethodInvoker)delegate { tbnfstate.Text = stext; });
                }
                else
                {
                    tbnfstate.Text = stext;
                }
        }

        public String getTbnfStateTxt()
        {
            String sr = "";
            if (this.tbnfstate.InvokeRequired)
            {
                this.tbnfstate.Invoke((MethodInvoker)delegate {sr = tbnfstate.Text;});
            }
            else
            {
                sr =  tbnfstate.Text;
            }
            return sr;
        }

        private void menuitemYzm_Click(object sender, EventArgs e)
        {
            fnf.testYzm();

        }

        private void menuItemBtou_Click(object sender, EventArgs e)
        {
            //补投某期,显示窗口
            //FormNfSetup fnfs = new FormNfSetup();
            //fnfs.ShowDialog(this);

            //获取文本文件内容
           String st =  Apitool.getFileTxt(System.Environment.CurrentDirectory + "\\ttmp.txt","");
           //addRtbNfjl("ttmp.txt is: ");
           //addRtbNfjl(st);
            if(!st.Equals(""))
            {
                Thread thadd = new Thread(new ParameterizedThreadStart(ForNaifen.getNfHistory));
                Object obj = new String[] { st };
                thadd.Start(obj); 
               //fnf.getNfHistory(st);
               
            }



        }

        //补投某期 qihao：上期期号 sqkj：上期开奖
        public void fnfButou(String qihao,String sqkj) 
        {
            fnf.buTou(qihao,sqkj);
        }

        private void btnnfxsjl_Click(object sender, EventArgs e)
        {
            Boolean showtoday = this.cbnfjltoday.Checked;

            //this.addRtbNfjl("------------------列表数据----------------");
            //foreach (String sls in ForNaiStrategy.llnfls)
            //{
            //    if (ForNaiStrategy.htKails.Contains(sls))
            //    {
            //        this.addRtbNfjl(sls + "." + ForNaiStrategy.htKails[sls]);
            //    }
            //}
            this.addRtbNfjl("-----------------数据库数据----------------");
            fnf.getDBHistory("",showtoday);
            this.addRtbNfjl("-------------------------------------------");
        }

        /// <summary>
        /// 返回nf是否自动翻页勾选状态
        /// </summary>
        /// <returns></returns>
        public Boolean getCbNFjlgetnow()
        {
            Boolean bl = false;
            if (this.cbnfjlgetnow.InvokeRequired)
            {
                this.cbnfjlgetnow.Invoke((MethodInvoker)delegate 
                { 
                    bl = this.cbnfjlgetnow.Checked;
                });
            }
            else
            {
                bl = this.cbnfjlgetnow.Checked;
            }
            return bl;
        }

        private void btnnfhqjl_Click(object sender, EventArgs e)
        {
            //获取历史记录
            Thread thadd = new Thread(new ParameterizedThreadStart(ForNaifen.getNfHistory));
            thadd.Start(null); 
            //ForNaifen.getNfHistory(null);
        }

        private void btnnfjlcpu_Click(object sender, EventArgs e)
        {
            //fnf.compuByHistory(this.tbnfjlmn.Text);
            fnf.compuByHistorySql(this.tbnfjlmn.Text);
            
        }

        private void btnnfbtjlok_Click(object sender, EventArgs e)
        {
            String sqh = this.tbnfsetup.Text.Trim();
            String skj = this.tbnfsetupkj.Text.Trim();
            String stime = this.tbnfjlrq.Text.Trim();
             if (sqh.Equals(""))
             {
                 return;
             }

             if (skj.Equals(""))
             {
                 return;
             }
             if (stime.Length < 4)
             {
                 stime = DateTime.Now.ToString("MMdd");
             }
             skj = skj.Replace(',', '.');
            String[] st = skj.Split('.');
            if (st.Length < 5 || st.Length > 10)
            {
                return;
            }
            foreach (String ss in st)
            {
               int ik = -1;
                if(Int32.TryParse(ss,out ik))
                {
                    if(ik<0 || ik > 10)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }  
            }
            fnf.addOneJilu(sqh, skj, stime, DateTime.Now.ToString("MMdd-hh:mm"));
        }

        
        void menunfReadmeclick(object sender, EventArgs e)
        {
        	string fpathnow = System.Environment.CurrentDirectory + "\\readme.txt" ;
        	Apitool.runProgram("cmd.exe","notepad "+fpathnow,500);
        }

        private void btnnfjldel_Click(object sender, EventArgs e)
        {
            String sqh = this.tbnfsetup.Text.Trim();
            if (sqh.Equals(""))
            {
                return;
            }
            fnf.delOneJilu(sqh);
        }

        private void btnweb2go_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.tbweb2addr.Text.Equals(""))
                {
                    Uri a = new Uri(tbweb2addr.Text);
                    if (a != null)
                        wb2.Navigate(a);
                }
            }
            catch (Exception ex)
            {
                this.addRtb2ForThread(ex.ToString());
            }

        }

        private void btnaq1_Click(object sender, EventArgs e)
        {

        }

        private void btnaq2_Click(object sender, EventArgs e)
        {

        }

        private void btnaq3_Click(object sender, EventArgs e)
        {

        }

        private void btnnasave_Click(object sender, EventArgs e)
        {

        }

        private void btnnabutou_Click(object sender, EventArgs e)
        {

        }

        private void btnnastart_Click(object sender, EventArgs e)
        {

        }

        private void btnnastop_Click(object sender, EventArgs e)
        {

        }
        
    }//end myForm

}