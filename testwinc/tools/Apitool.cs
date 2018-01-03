using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Security;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.DirectX.AudioVideoPlayback; 

 //md5 加密


 //需要再引用中加System.web dll引用，默认是不引用的!!!by wind 10.21

namespace testwinc.tools
{
    class Apitool
    {
        private static System.Windows.Forms.WebBrowser wb = null;
        private static String strForWebbrowser = "";

        private static Audio audio = null;

        /// <summary>
        /// 根据str字符串查找由字符串name结尾开始与最近字符串endname间信息,找不到返回null
        /// </summary>
        /// <param name="str"></param>
        /// <param name="name"></param>
        /// <param name="endname"></param>
        /// <returns></returns>
        public static String FindStrByName(String str, String name, String endname)
        {
            if (str == null || name == null || endname == null) return null;
            int iname = str.IndexOf(name, 0);
            if (iname == -1)
            {
                return null;
            }
            int ll = name.Length + iname;
            if (endname == "") 
            {
                return str.Substring(ll);
            }
            int inameend = str.IndexOf(endname, ll);
            if (inameend == -1)
            {
                return null;
            }
            return str.Substring(ll, inameend - ll);
        }

        /// <summary>
        /// 根据str字符串查找由字符串name结尾开始与最近字符串endname间信息,找不到返回null
        /// endindex ：字符串 endname 位置
        /// </summary>
        /// <param name="str"></param>
        /// <param name="name"></param>
        /// <param name="endname"></param>
        /// <param name="endindex">endindex ：字符串 endname 位置</param>
        /// <returns></returns>
        public static String FindStrByName(String str, String name, String endname,out int endindex)
        {
            endindex = -1;
            if (str == null || name == null || endname == null) return null;
            int iname = str.IndexOf(name, 0);
            if (iname == -1)
            {
                return null;
            }
            int ll = name.Length + iname;
            endindex = str.IndexOf(endname, ll);
            if (endindex == -1)
            {
                return null;
            }
            return str.Substring(ll, endindex - ll);
        }

        /// <summary>
        /// 取出形如 a href="http://www.188bifen.com/lanqiubifen.htm" target="_blank"> 比分直播 /a>的信息
        /// 返回String[0] 值，String [1]参数值 ，没找到返回 null,参数1没找到返回""
        /// </summary>
        /// <param name="input"></param>
        /// <param name="Startname"></param>
        /// <param name="endname"></param>
        /// <param name="paraname"></param>
        /// <returns></returns>
        public static String[] getValueByName(String input, String Startname,String endname,String paraname)
        {
            input = FindStrByName(input, Startname, endname);
            if (input == null || input == "") return null;
            String[] result = new String[2];
            result[0] = input.Substring(input.IndexOf(">", 0) + ">".Length);
            result[1] = "";
            if (paraname == null || paraname == "") return result;
            else
            {
                result[1] = FindStrByName(input, paraname + "=\"", "\"");
                if (result[1] == null) result[1] = "";
                return result;
            }
        }

        /// <summary>
        /// 取出形如 a href="http://www.188bifen.com/lanqiubifen.htm" target="_blank"> 比分直播 /a>的信息
        /// 返回String[0] 值，String [1]参数值 ，没找到返回 null,参数1没找到返回""
        /// </summary>
        /// <param name="input"></param>
        /// <param name="Startname"></param>
        /// <param name="endname"></param>
        /// <param name="paraname"></param>
        /// <param name="endindex">结束字符串位置，没找到返回 -1 </param>
        /// <returns></returns>
        public static String[] getValueByName(String input, String Startname, String endname, String paraname, out int endindex)
        {
            endindex = -1;
            input = FindStrByName(input, Startname, endname,out endindex);
            if (input == null || input == "") return null;
            String[] result = new String[2];
            result[0] = input.Substring(input.IndexOf(">", 0) + ">".Length);
            result[1] = "";
            if (paraname == null || paraname == "") return result;
            else
            {
                result[1] = FindStrByName(input, paraname + "=\"", "\"");
                return result;
            }
        }

        //通过正则表达式从字符串中得到对应的结果(所有结果无视大小写)
        public static Match GetResultOfReg(String PageCode, String Regexp)
        {
            if (PageCode == "" || Regexp == "") return null;
            Match m = null;
            try
            {
                Regex r = new Regex(Regexp, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                m = r.Match(PageCode);
            }
            catch (Exception)
            {
                return null;
            }
            return m;

        }

        //通过正则表达式从字符串中替换对应的结果(所有结果无视大小写)
        public static String GetReplaceOfReg(String PageCode, String Regexp, String ReplaceStr)
        {
            try
            {
                Regex r = new Regex(Regexp, RegexOptions.IgnoreCase);
                return r.Replace(PageCode, ReplaceStr);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        /// <summary>
        /// md5Run16 用md5 16位 加密输入字符串
        /// </summary>
        /// <param name="strInput">输入字符串 </param>
        /// param name="num">md5指示位（16 or 32） </param>
        /// <returns>默认返回用md5 16位 加密后的字符串</returns>
        public static String md5Run(String strInput, int num) 
        {
            if (strInput != null && strInput != "")
            {
                //byte[] result = Encoding.Default.GetBytes(strInput.Trim());    //tbPass为输入密码的文本框
                //MD5 md5 = new MD5CryptoServiceProvider();
                //byte[] output = md5.ComputeHash(result);
                //this.tbMd5pass.Text = BitConverter.ToString(output).Replace("-", "");  //tbMd5pass为输出加密文本的文本框

                string hashedPassword =
                 FormsAuthentication.HashPasswordForStoringInConfigFile(strInput, "MD5");//md5 32位
                if (num == 32)
                {
                    return hashedPassword;
                }
                else
                {
                    string MD5pwd16 = hashedPassword.Substring(8, 16);//MD5 16位
                }
            }
            return "";
        }

        /// <summary>
        /// MD5 16位加密 加密后密码为大写
        /// </summary>
        /// <param name="ConvertString"></param>
        /// <returns></returns>
        public static string md5Run16(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }

        /// <summary>
        /// MD5　32位加密大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string md5Run32(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 

                pwd = pwd + s[i].ToString("X");

            }
            return pwd.ToUpper(); ;
        }




        public static String getUrlByBrowser(String url)
        {
            try
            {
                if (url == "") return null;
                //add code here,这段代码有问题！
                //if(wb==null)
                //    getWebBrowser();
                //wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(getWebDocument);
                //strForWebbrowser = "";
                //wb.Navigate(url);
                //while (strForWebbrowser == "")
                //{
                //    Application.DoEvents();//等待本次加载完毕才执行下次循环.
                    
                //}
                return strForWebbrowser;
            }
            catch (Exception e1)
            {
                return e1.ToString();
            }
        }



        public static WebBrowser getWebBrowser() 
        {
            if (wb == null)
            {
                wb = new System.Windows.Forms.WebBrowser();
            }
            return wb;
        }

        public static String getStrCut()
        {
            return strForWebbrowser;
        }

        private static void getWebDocument(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            strForWebbrowser = wb.Document.Body.InnerHtml.ToString();
        }

        //简单过滤字符串
        public static String filterDBStr(String str)
        {
            str = str.Replace("'", "\"").Trim();
            return str;
        }

        //获得len长度随机字符串
        public static String getRandomString(int len)
        {
            if (len < 1) len = 3;
            Random rd = new Random();
            StringBuilder sb = new StringBuilder();
            for (int it = 0; it < len; it++)
            {
                sb.Append(rd.Next(10) + "");
            }
            return sb.ToString();
        }
        public static String getLastNumForStr(String str)
        {
            if (str == null || str == "") return "";
            int tmp = -1;
            str = str.Replace("大结局", "").Replace("全", "").Replace("完", "").Replace("集", "").Replace("话", "");
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

        /// <summary>
        /// 获得字符st所表示的数字，st不是数字时返回0
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static int getIntValue(String st)
        {
            if (st == null || st.Equals("") || st.Equals('-') || st.Equals("-"))
                return 0;
            int it = 0;
            if (Int32.TryParse(st, out it))
            {
                return it;
            }
            return 0;
        }
        
         /// <summary>
         /// 简单文字加密
         /// </summary>
         /// <param name="data"></param>
         /// <returns></returns>
        public static string Encode(string data)
        {
            string KEY_64 = "VavicApp";
            string IV_64 = "VavicApp";
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);
            try
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                int i = cryptoProvider.KeySize;
                MemoryStream ms = new MemoryStream();
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cst);
                sw.Write(data);
                sw.Flush();
                cst.FlushFinalBlock();
                sw.Flush();
                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }
            catch
            {
                return null;
            }
        }
        
         /// <summary>
         /// 简单文字解密
         /// </summary>
         /// <param name="data"></param>
         /// <returns></returns>
        public static string Decode(string data)
        {
            string KEY_64 = "VavicApp";
            string IV_64 = "VavicApp";
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);
            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream(byEnc);
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cst);
                return sr.ReadToEnd();
            }
            catch
            {
                return null;
            }
           
        }
       
        /// <summary>
        /// 设置richtextbox单行文本颜色输入
        /// </summary>
        /// <param name="rtb"></param>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        public static void setTbnfColor(RichTextBox rtb, Object msg, System.Drawing.Color color)
        {
       	 if (WindForm.isDebug)
            {
       	 	  if (null == msg) msg = "null";
       	 	   if (rtb.InvokeRequired)
                {
                    rtb.Invoke((MethodInvoker)delegate 
                    {
                        rtb.SelectionStart = rtb.Text.Length;//设置插入符位置为文本框末
                        rtb.SelectionColor = color;//设置文本颜色
                        rtb.AppendText(msg + "\r\n");//输出文本，换行
                        // rtb.ScrollToCaret();//滚动条滚到到最新插入行
                    });                  
                }
                else
                {
                    rtb.SelectionStart = rtb.Text.Length;//设置插入符位置为文本框末
	                rtb.SelectionColor = color;//设置文本颜色
	                rtb.AppendText(msg + "\r\n");//输出文本，换行
	               // rtb.ScrollToCaret();//滚动条滚到到最新插入行
                }	           
          }
       }

        /// <summary>
        /// 获得指定文本文件内容
        /// </summary>
        /// <param name="filename">eg. d:\\temp\\configwg.txt</param>
        /// <returns>string ,未找到返回空字符串""</returns>
        public static String getFileTxt(String filename,String encoding)
        {
            if(!File.Exists(filename))
            {
                return "";
            }            
            try
            {
                //使用using 对象后，在using块外，所使用的对象被自动关闭回收，避免内存泄露
                // FileStream fs = null;
                if (encoding.Equals(""))
                {
                    encoding = "gb2312";
                }
                using (FileStream fs = File.OpenRead(filename))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding(encoding)))
                    {
                        //Encoding e1 = Encoding.Default;               //取得本页默认代码
                        //Byte[] bytes = e1.GetBytes("中国人民解放军"); //转为二进制
                        //string str = Encoding.GetEncoding("UTF-8").GetString(bytes); //转回UTF-8编码
                           string s = "";
                            StringBuilder sb = new StringBuilder();
                            while ((s = sr.ReadLine()) != null)
                            {
                                sb.Append(s);
                            }
                                        
                            if (sr != null)
                            {
                                sr.Close();
                                sr.Dispose();
                            }
                            if (fs != null)
                            {
                                fs.Close();
                                fs.Dispose();
                            }
                            return sb.ToString();         
                    }//end using fs
                }//end using sr
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
        
		 /// <summary>
         /// 打开命令行软件并执行命令
         /// </summary>
         /// <param name="programName">软件路径加名称（.exe文件）</param>
         /// <param name="cmd">要执行的命令</param>
         // D:\work\tools\tessnet2_32\Tesseract-OCR\tesseract.exe
         //tesseract 6883.jpg 26 -l eng
         public static String runProgram(string programName, string cmd,int milliseconds)
         {
             Process proc = new Process();
             proc.StartInfo.CreateNoWindow = true;
             if (programName.Equals(""))
             {
                 //无命令则启动命令行
                 programName = "cmd.exe";
             }
             proc.StartInfo.FileName = programName;
             proc.StartInfo.UseShellExecute = false;
             proc.StartInfo.RedirectStandardError = true;
             proc.StartInfo.RedirectStandardInput = true;
             proc.StartInfo.RedirectStandardOutput = true;
           
             String res = "";
             try
             {
                 proc.Start();
               
                 if (cmd.Length != 0)
                 {
                     proc.StandardInput.WriteLine(cmd);
                   //  proc.StandardInput.AutoFlush = true;
                    
                 }
                 if (milliseconds == 0)
                 {
                     proc.WaitForExit();
                 }   //这里无限等待进程结束
                 else
                 {
                     proc.WaitForExit(milliseconds);
                 } //这里等待进程结束，等待时间为指定的毫秒     

                
                 //退出cmd
                 if (programName.Equals("cmd.exe"))
                 {
                     proc.StandardInput.WriteLine("exit");
                 }
                 
                 res = proc.StandardOutput.ReadToEnd();                                         
             }
             catch (Exception)
             {              
             }
             finally
             {
                 if (proc != null)
                 {
                     proc.Close();
                     proc = null;
                 }              
             }
             return res;
         } 

        /// <summary>
         /// playMp3 类
        /// </summary>
         /// <param name="mp3file">mp3file path</param>
        public static void playMp3(String mp3file)
        {
            if(mp3file.Equals(""))
            {
                mp3file = System.Environment.CurrentDirectory + @"\alarm.mid";
            }
            try
            {
                if (audio == null)
                {
                    audio = new Audio(mp3file);
                }
                else
                {
                    audio.Dispose();
                    audio = new Audio(mp3file);
                }               
                audio.Play();
            }
            catch(Exception ex)
            {
                UseStatic.soutTdRtb2(ex.ToString());
                if (audio != null)
                {
                    audio.Dispose();
                }
            }
        }
          
        /// <summary>
        /// 释放该类所占资源
        /// </summary>
        public static void dispose() 
        {
            if(wb != null){
                wb.Dispose();
                wb = null;
            }
            if (audio != null)
            {
                audio.Dispose();
            }
            strForWebbrowser = "";
        }

    }
}
