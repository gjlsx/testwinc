using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;

using System;
using System.Net;
using System.IO;
using System.Data;

using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Data.SQLite;
using System.Data.SqlClient;



namespace testwinc.tools
{
	/// <summary>
	/// Apis 的摘要说明。
	/// </summary>
    public class Apis
    {
        public static String sDBUrl247 = 
            "Data Source=117.25.129.247,1433;Network Library=DBMSSOCN;Initial Catalog=TV;User ID=010_exe;Password=FdiTk#5i9k8";
        private static readonly String STR_DB_PWD = "FdiTk#5i9k8";
        private static readonly String STR_DB_NAME = "010_exe";
        private static readonly String StrProjname = "hao123";
        public static String sDBUrl206 = @"Server=.\sql2008;database=tvsport;uid=g;pwd=931193";
        private static readonly String STR_DB_PWD206 = "FJwz%@)50";
        private static readonly String STR_DB_NAME206 = "sa";
        private static readonly String StrProjname206 = "capsport";
        public static readonly String StrProjname94 = "hztvsou";
        public static readonly String StrProjname38 = "hztv38";
        public static String sDBUrl94 = "";
        public static String sDBAcess = "";
        public static String sDBUrl38 = "";

        private static String[] SqlStrSplit = null;


        static Apis()
        {
            String st = Apis.getDbstr(StrProjname, STR_DB_PWD, STR_DB_NAME);
            if (st != "")
                sDBUrl247 = st;
            st = Apis.getDbstr(StrProjname206, STR_DB_PWD206, STR_DB_NAME206);
            if (st != "")
                sDBUrl206 = st;
            st = Apis.getDbstr(StrProjname94, "", "");
            if (st != "")
                sDBUrl94 = st;
            st = Apis.getDbstr(StrProjname38, "", "");
            if (st != "")
                sDBUrl38 = st;
            
            sDBAcess = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + System.Environment.CurrentDirectory + @"\forenotice.mdb";
       
        }


        //获取数据库连接
        public SqlConnection GetCon()
        {
            SqlConnection cn = null;
            cn = new SqlConnection();
            cn.ConnectionString = sDBUrl247;
            //cn.ConnectionString = "Server=localhost;database=tv;uid=sa;pwd=sa";
            cn.Open();
            return cn;
        }

        public static SqlConnection GetCon(String dbUrl)
        {
            SqlConnection cn = null;
            cn = new SqlConnection(dbUrl);
            cn.Open();
            return cn;
        }

        public static SQLiteConnection getSqliteCon(String dburl)
        {
            if(dburl.Equals(""))
                dburl = System.Environment.CurrentDirectory + @"\twnaifen.sqlite";
            string strCon = "Data Source="+dburl + ";Pooling=true;FailIfMissing=false";
            SQLiteConnection sqliteCon = new SQLiteConnection(strCon);
            sqliteCon.Open();
            return sqliteCon;
        }

        /// <summary>
        /// 数据库操作类
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int runSqlite(string sql, String strdbUrl)
        {
            SQLiteConnection conn = null;
            try
            {
                conn = getSqliteCon(strdbUrl);
                if (conn != null)
                {
                    return upDB(conn, sql);
                }
                return -1;
            }
            catch (Exception ex)
            {
                UseStatic.getWindForm().addRtbNfjl(ex.ToString());
                return -1;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// 获得datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable GetSqliteDataTable(string sql)
        {
            SQLiteConnection conn = null;
            DataSet ds = null;
            SQLiteDataAdapter sda = null;
            try
            {
                conn = getSqliteCon("");
                sda = new SQLiteDataAdapter(sql, conn);//DataAdapter：网络适配器
                ds = new DataSet();
                sda.Fill(ds,"sqlitetbl");//将结果填充到ds中
                DataTable dt = ds.Tables["sqlitetbl"];
                return dt;
            }
            catch (Exception ex)
            {
                UseStatic.getWindForm().addRtbNfjl(ex.ToString());
                return null;
            }
            finally
            {
                if (ds != null)
                {
                    ds.Dispose();
                }
                if(sda != null)
                {
                    sda.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

        }
        /// <summary>
        /// 返回记录总条数
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns>-1,无该记录，0记录条数为0，n条数</returns>
        public static int GetSqliteCount(string strTableName)
        {
            string strSql = "select count(*) from " + strTableName;
            int count = -1;
            DataTable dtCount = GetSqliteDataTable(strSql);
            count = int.Parse(dtCount.Rows[0][0].ToString());
            return count;
        }

        //执行SQL语句
        public static void upDB(SqlConnection cnst, String sql)
        {
            SqlCommand Comm = null;
            try
            {
                Comm = cnst.CreateCommand();
                UpdateDB(Comm, sql);
            }
            finally
            {
                if(Comm != null)
                    Comm.Dispose();
            }
        }

        //执行SQL语句
        public static void upDB(OleDbConnection cnst, String sql)
        {
            OleDbCommand Comm = null;
            try
            {
                Comm = cnst.CreateCommand();
                UpdateDB(Comm, sql);
            }
            finally
            {
                if (Comm != null)
                    Comm.Dispose();
            }
        }

        //执行SQLite语句 update
        public static int upDB(SQLiteConnection cnst, String sql)
        {
            SQLiteCommand Comm = null;
            try
            {
                Comm = cnst.CreateCommand();
                return UpdateDB(Comm, sql);
            }
            finally
            {
                if (Comm != null)
                    Comm.Dispose();
            }
        }

        //执行SQL语句,并返回查询结果的第一行第一列，用来获得ID
        public static Object upDBOneResult(SqlConnection cnst, String sql,String sqlresult)
        {
             SqlCommand Comm = null;
             Object obj = null;
            try{
                Comm = cnst.CreateCommand();
                obj =  UpdateDBOneResult(Comm, sql, sqlresult);
                return obj;
            }
            finally
            {
                if(Comm != null)
                    Comm.Dispose();   
            }
        }

        //执行SQL语句,并返回查询结果的第一行第一列，用来获得ID
        public static Object upDBOneResult(SqlConnection cnst, String[] sql, String sqlresult)
        {
            SqlCommand Comm = null;
            Object obj = null;
            try
            {
                Comm = cnst.CreateCommand();
                obj = UpdateDBOneResult(Comm, sql, sqlresult);
                return obj;
            }
            finally
            {
                if (Comm != null)
                    Comm.Dispose();
            }
        }

        //执行SQL语句
        public static void UpdateDB(SqlCommand Comm, String sql)
        {
            Comm.CommandText = sql;
            Comm.ExecuteNonQuery();
        }

        //执行SQL语句
        public static void UpdateDB(OleDbCommand Comm, String sql)
        {
            Comm.CommandText = sql;
            Comm.ExecuteNonQuery();
        }

          //执行SQLite语句
        public static int UpdateDB(SQLiteCommand Comm, String sql)
        {
            Comm.CommandText = sql;
            return Comm.ExecuteNonQuery();
        }

        /// <summary>
        ///  执行语句并返回查询结果的第一行第一列，用来获得ID
        /// </summary>
        /// <param name="Comm"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static Object UpdateDBOneResult(SqlCommand Comm, String sql,String sqlresult)
        {
           // sql += " ; SELECT @@IDENTITY";  另一种用法
                Comm.CommandText = sql;
                Comm.ExecuteNonQuery();
                Comm.CommandText = sqlresult;
                return Comm.ExecuteScalar();
        }

        public static Object UpdateDBOneResult(SqlCommand Comm, String[] sql, String sqlresult)
        {
            for (int it = 0; it < sql.Count(); it++)
            {
                Comm.CommandText = sql[it];
                Comm.ExecuteNonQuery();
            }
            Comm.CommandText = sqlresult;
            return Comm.ExecuteScalar();
        }


        //执行SQL语句
        public void UpdateDB(SqlConnection cn, String sql)
        {
            SqlCommand Comm = cn.CreateCommand();
            Comm.CommandText = sql;
            Comm.ExecuteNonQuery();
            Comm.Dispose();
        }

        //打开数据库链接(access库)
        public static OleDbConnection GetConAcess()
        {
            String strConnection = "Provider=Microsoft.Jet.OleDb.4.0;";
            //strConnection+=@"Data Source="+MapPath("dvbbs7.MDB");//这里是相对路径
            strConnection += "Data Source=forenotice.mdb";//这里用的是绝对路径

            OleDbConnection cn = new OleDbConnection(strConnection);
            cn.Open();
            return cn;
        }

        //打开数据库链接(access库)
        public static OleDbConnection GetConAcess(String url)
        {
            OleDbConnection cn = new OleDbConnection(url);
            cn.Open();
            return cn;
        }

        //执行SQL语句(access库)
        public void UpdateDBs(OleDbConnection cn, String sql)
        {
            OleDbCommand Comm = cn.CreateCommand();
            Comm.CommandText = sql;
            Comm.ExecuteNonQuery();
            Comm.Dispose();
        }

        //执行SQL语句(access库)
        public int GetResult(OleDbConnection cn,String sql) 
        {
            OleDbCommand cmd = cn.CreateCommand();
            int num = 0;
            cmd.CommandText = sql;
            num =Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            return num;
        }

        //转换成Unicode编码形式;
        public string ConvertToGb2312(string str)
        {
            //String m_Start = str;

            ////String s=HttpUtility. 
            ////把unicode的转换为GB2312 
            //System.Text.UnicodeEncoding unicode = new UnicodeEncoding();

            //System.Text.Encoding gb2312 = System.Text.Encoding.GetEncoding("GB2312");

            //byte[] m = unicode.GetBytes(m_Start);

            //byte[] s;
            ////进行转换 
            //s = System.Text.Encoding.Convert(unicode, gb2312, m);

            ////string m_End=gb2312.GetString(s); 

            ////string m_End=System.Web.HttpUtility.UrlDecode("http://www.baidu.com/s?ie=gb2312&bs=C%23%2Curl%B5%D8%D6%B7%B1%E0%C2%EB&sr=&z=&wd=C%23%2Cunicode%2C%D7%AA%BB%BB%2CGB2312&ct=0&cl=3&f=8"); 

            //return System.Web.HttpUtility.UrlEncode(s);
            return "革命尚未成功，同志们仍需努力!";
        }

        //获取指定页面源代码，另外自己写的获取页面代码，锁定url。
        public String GetPageCode2(String UrlAddress, String Charset)
        {
            //存放目标网页的html
            String html = "";
            try
            {
                Uri myUri = new Uri(UrlAddress);
                //连接到目标网页
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(myUri);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(Charset));
                html = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                response.Close();
                return html;
            }
            catch (Exception ei)	//遇到错误，打印错误
            {
                return html = ei.ToString();
            }
            finally
            {
                
            }
        }

        //获取指定页面的源代码
        public static String GetPageCode(String PageURL, String Charset)
        {
            HttpWebResponse wresp = null;
            try
            {
                //连接到目标网页
                WebRequest wreq = WebRequest.Create(PageURL);
                wreq.Method = "GET";
                wresp = (HttpWebResponse)wreq.GetResponse();

                //采用流读取，并确定编码方式
                using (Stream s = wresp.GetResponseStream())
                {
                    using (StreamReader objReader = new StreamReader(s, System.Text.Encoding.GetEncoding(Charset)))
                    {
                        String sread = getStringFromReader(objReader);
                        return sread;
                    }
                }
            }
            catch (Exception ei)	//遇到错误，打印错误
            {
                return ei.ToString();
            }
            finally 
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
        }

        //WebClient取网页源码
        public static string getHtmlSource(string Url)
        {
            string text1 = "";
            System.Net.WebClient wc = null;
            try
            {
                wc = new WebClient();
                text1 = wc.DownloadString(Url);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            finally 
            {
                if (wc != null)
                    wc.Dispose();
            }
            return text1;
        }

        /// <summary>
        ///解决： 远程服务器返回错误:  (411) 所需的长度
        /// </summary>
        /// <param name="PageURL">地址</param>
        /// <param name="Charset">编码</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static String GetPageCode411(String PageURL, String Charset, String param, String method)
        {
            byte[] bs = Encoding.ASCII.GetBytes(param);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(PageURL);
            req.Method = method;
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }

            WebResponse wr = req.GetResponse();   
            using (Stream s = wr.GetResponseStream())
            {
                using (StreamReader objReader = new StreamReader(s, System.Text.Encoding.GetEncoding(Charset)))
                {
                    String sread = getStringFromReader(objReader);
                    if (wr != null)
                    {
                        wr.Close();
                        wr = null;
                    }
                    return sread;
                }
            }
        }

        /// <summary>
        /// 解决：抓取出现500错误时候，用这种方法抓取
        /// </summary>
        /// <param name="PageURL"></param>
        /// <param name="Charset"></param>
        /// <returns></returns>
        public static String GetPageCodeBy500Error(String PageURL, String Charset)
        {
            try
            {
                //连接到目标网页
                HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(PageURL);
                wreq.Method = "GET";
                //wreq.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2.8) Gecko/20100722 Firefox/3.6.8";
                wreq.Timeout = 20000;
                wreq.UserAgent = 
                    "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1) ; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
                //wreq.ContentType = "text/html";
                //wreq.ContentType = "application/x-www-form-urlencoded";

                HttpWebResponse wresp = (HttpWebResponse)wreq.GetResponse();

                //采用流读取，并确定编码方式
                Stream s = wresp.GetResponseStream();
                StreamReader objReader = new StreamReader(s, System.Text.Encoding.GetEncoding(Charset));


                return getStringFromReader(objReader);
            }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;

                if (res.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Stream s = res.GetResponseStream();
                    StreamReader objReader = new StreamReader(s, System.Text.Encoding.GetEncoding(Charset));

                    return getStringFromReader(objReader);
                }
                else
                {
                    return ex.Message;
                }
            }
        }

        /// <summary>
        /// 模仿IE浏览器保存Cookie
        /// 获取网页源码
        /// 第三个参数为网站地址
        /// </summary>
        /// <param name="PageURL"></param>
        /// <param name="Charset"></param>
        /// <returns></returns>
        public static String GetPageCode1(String PageURL, String Charset, String WebsiteAddress, String method)
        {
            try
            {
                //存放目标网页的html
                String strHtml = "";
                //连接到目标网页
                HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(PageURL);
                wreq.Referer = WebsiteAddress; //设置HTTP_REFERER
                wreq.Headers.Add("X_FORWARDED_FOR", "101.0.0.11"); //发送X_FORWARDED_FOR头(若是用取源IP的方式，可以用这个来造假IP,对日志的记录无效)   

                wreq.Method = method;//get/post
                wreq.KeepAlive = true;
                wreq.ContentType = "application/x-www-form-urlencoded";
                wreq.AllowAutoRedirect = true;
                wreq.Accept = 
                    "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                wreq.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322)";

                CookieContainer cookieCon = new CookieContainer();
                wreq.CookieContainer = cookieCon;

                HttpWebResponse wresp = (HttpWebResponse)wreq.GetResponse();

                //9tvtest

                //采用流读取，并确定编码方式
                Stream s = wresp.GetResponseStream();
                StreamReader objReader = new StreamReader(s, System.Text.Encoding.GetEncoding(Charset));

                strHtml = getStringFromReader(objReader);
                return strHtml.Replace("<br />", "\r\n");
            }
            catch (Exception n) //遇到错误，打印错误
            {
                return n.Message;
            }
        }

        public static String GetPageCodeStream(String PageURL, String Charset)
        {
            try
            {
                //连接到目标网页
                WebRequest wreq = WebRequest.Create(PageURL);

                HttpWebResponse wresp = (HttpWebResponse)wreq.GetResponse();

                //采用流读取，并确定编码方式
                Stream s = wresp.GetResponseStream();

                StreamReader objReader = new StreamReader(s, System.Text.Encoding.GetEncoding(Charset));

                StringBuilder sb = new StringBuilder(100);
                string strLine = objReader.ReadLine();
                //读取
                while (strLine != null)
                {
                    sb.Append(strLine.Trim());
                    sb.Append("<br>");
                    strLine = objReader.ReadLine();
                }

                return sb.ToString();
            }
            catch (Exception e)	//遇到错误，打印错误
            {
                return e.ToString();
            }
        }
    

        public DataTable searchs(OleDbConnection conn, string sql)
        {
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            DataTable dt = new DataTable();

            OleDbDataAdapter adapter = new OleDbDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
                return null;
            return dt;

        }
        public DataTable search(string sql, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;

            DataTable dt = new DataTable();

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(dt);
            if (dt.Rows.Count == 0)
                return null;
            return dt;

        }
        public static string StripHTML(string strHtml)
        {
            string[] aryReg ={
          @"<script[^>]*?>.*?</script>",

          @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
          @"([\r\n])[\s]+",
          @"&(quot|#34);",
          @"&(amp|#38);",
          @"&(lt|#60);",
          @"&(gt|#62);", 
          @"&(nbsp|#160);", 
          @"&(iexcl|#161);",
          @"&(cent|#162);",
          @"&(pound|#163);",
          @"&(copy|#169);",
          @"&#(\d+);",
          @"-->",
          @"<!--.*\n"
         
         };

            string[] aryRep = {
           "",
           "",
           "",
           "\"",
           "&",
           "<",
           ">",
           " ",
           "\xa1",//chr(161),
           "\xa2",//chr(162),
           "\xa3",//chr(163),
           "\xa9",//chr(169),
           "",
           "\r\n",
           ""
          };

            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, aryRep[i]);
            }

            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");
            return strOutput;
        }

        //返回汉字首拼音
        public static string getSpell(string cnChar)
        {
            byte[] arrCN = Encoding.Default.GetBytes(cnChar);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119,49062, 49324, 49896,
                                     50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25) max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
                    }
                }
                return "*";
            }
            else return cnChar;
        }



        public string BtwString(string str1, string beginstr, string endstr)
        {
            string tstr = str1;
            if (tstr.IndexOf(beginstr) > -1)
            {
                int int1 = tstr.IndexOf(beginstr) + beginstr.Length;
                tstr = tstr.Substring(int1, tstr.Length - int1);
                if (tstr.IndexOf(endstr) > -1)
                {
                    tstr = tstr.Substring(0, tstr.IndexOf(endstr));
                }

                return tstr;
            }
            else
            {
                return tstr = "";
            }
        }

        public string BtwString2(string str1, string beginstr, string endstr, int tint)
        {
            string tstr = str1;
            int int1 = tstr.IndexOf(beginstr) + beginstr.Length;
            tstr = tstr.Substring(int1, tstr.Length - int1);
            for (int i = 1; i < tint; i++)
            {
                int int2 = tstr.IndexOf(endstr) + endstr.Length;
                tstr = tstr.Substring(int2, tstr.Length - int2);

            }

            if (tstr.IndexOf(endstr) > 0)
            {
                tstr = tstr.Substring(0, tstr.IndexOf(endstr));
            }

            return tstr;
        }

        public string GetHtmlPart(string PageCode, string regExp, int GetGroupNum)
        {
            try
            {
                Match GPhtml = Regex.Match(PageCode, regExp);
                if (GPhtml.Success)
                {
                    return GPhtml.Groups[GetGroupNum].Value.ToString();
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        //取当前时间是第几周
        public int WeekOfYear(string date)
        {
            DateTime curDay = Convert.ToDateTime(date);

            int firstdayofweek = Convert.ToInt32(Convert.ToDateTime(curDay.Year.ToString() + "- " + "1-1 ").DayOfWeek);

            int days = curDay.DayOfYear;
            int daysOutOneWeek = days - (7 - firstdayofweek);

            if (daysOutOneWeek <= 0)
            {
                return 1;
            }
            else
            {
                int weeks = daysOutOneWeek / 7;
                if (daysOutOneWeek % 7 != 0)
                    weeks++;

                return weeks + 1;
            }
        }

        //格式化数据(转为两位数)
        public string FormatNum2Double(int inputNum)
        {
            if (inputNum < 10)
            {
                return "0" + inputNum.ToString();
            }
            else
            {
                return inputNum.ToString();
            }
        }

        public String ChanngeDirectorProsenter(String Str1)
        {
            String mmstr = Str1;
            //批量替换成单个'/'字符
            mmstr = Regex.Replace(mmstr, "([ ]{0,}<BR>[ ]{0,}|[ ]{0,}<br>[ ]{0,}){1,}", "/");
            mmstr = Regex.Replace(mmstr, "([ ]{0,}[:]{1,}[ ]{0,}|[ ]{0,}[：]{1,}[ ]{0,}){1,}", "/");
            mmstr = Regex.Replace(mmstr, "([ ]{0,}[,]{1,}[ ]{0,}){1,}", "/");
            //底下备用
            //mmstr = Regex.Replace(mmstr, "[ ]{1,}", "/");
            //mmstr = Regex.Replace(mmstr, "[/]{1,}", "/");

            //避免分割字符串报错
            //if(mmstr.Length>0)
            if (mmstr.Trim() != "")
            {
                if (mmstr.Substring(0, 1) == "/")
                {
                    mmstr = mmstr.TrimStart('/');//除头字符
                }
                if (mmstr.Substring(mmstr.Length - 1, 1) == "/")
                {
                    mmstr = mmstr.TrimEnd('/');//除尾字符
                }
            }
            return mmstr.Trim();
        }

        ////MD5加密, 单向加密
        //public String CRC_PassWord(String str)
        //{
        //    String SS;
        //    SS = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        //    return SS;
        //}

        ////SHA1加密,单向
        //public String CRC_PassWord2(String str2)
        //{
        //    String SS2;
        //    SS2 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str2, "SHA1");
        //    return SS2;
        //}

        //判断网页是否存在
        public String Pd_HrefisCZ(String SiteUrl)
        {
            //bool TimeoutCheck =false;
            String bl;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SiteUrl);
                request.Timeout = 3000;
                if (request != null)
                {
                    //if (request.Timeout > 1000)
                    //{
                    //    MessageBox.Show("超时");
                    //}
                    //else
                    //{
                    HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse();
                    if (myResponse.StatusDescription.ToString().ToUpper() == "OK")
                    {
                        bl = "是";
                        myResponse.Close();

                    }
                    else
                    {
                        bl = "否";
                        myResponse.Close();
                    }
                    myResponse.Close();
                    //}
                }
                //request.Timeout = 3000;
                //Console.WriteLine("\nThe timeout time of the request after setting the timeout is {0}  milliSeconds.", request.Timeout);
                //if (request.Timeout > 3000)
                //{
                //    bl = "否";
                //}
                else
                {
                    bl = "否";
                }

            }
            catch
            {
                bl = "否";
            }

            return bl;
        }


        /// <summary>
        /// 转全角的函数(SBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>全角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public String ToSBC(string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }


        /// <summary> 转半角的函数(DBC case) </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public String ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// unicode转中文（符合js规则的）
        /// </summary>
        /// <returns></returns>
        public string unicode_js_1(string str)
        {
            string outStr = "";
            Regex reg = new Regex(@"(?i)\\u([0-9a-f]{4})");
            outStr = reg.Replace(str, delegate(Match m1)
            {
                return ((char)Convert.ToInt32(m1.Groups[1].Value, 16)).ToString();
            });
            return outStr;
        }

        private int nextNum=0;

        public String GetDateByStartTimeAndEndTime(int start,int end)
        {
            String resultTime = "";int num = 0;

            DateTime nowdate;
            num=end - start;

            nextNum=num+nextNum;

           nowdate = Convert.ToDateTime("0:00");

           resultTime = nowdate.AddSeconds(nextNum).ToString("HH:mm");

            return resultTime;
        }

        /// <summary>
        /// 从流里用stringbuffer读取字符串最后组成字符返回
        /// </summary>
        /// <param name="sbd"></param>
        /// <returns></returns>
        public static String getStringFromReader(StreamReader sbd)
        {
            StringBuilder sb = new StringBuilder(100);
            string strLine = sbd.ReadLine();
            //读取
            while (strLine != null)
            {
                sb.Append(strLine.Trim()+"\r\n");
                strLine = sbd.ReadLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 替换掉网页无用的字符
        /// </summary>
        /// <param name="shtml"></param>
        /// <returns></returns>
        public static string shtml(string shtml)
        {
            shtml = shtml.Replace("<title>", "").Replace("</title>", "").Replace("</font>", "").Replace("</tr>", "&nbsp;&nbsp;")
                .Replace("</td>", "").Replace("</table>", "").Replace("</a>", "").Replace("<tr>", "").Replace("<!---title--->", "")
                .Replace("</span>", "").Replace("</script>", "").Replace("center", "").Replace("</A>", "").Replace("</div>", "")
                .Replace("</TD>", "").Replace("</TR>", "").Replace("</TABLE>", "").Replace("</CENTER>", "").Replace("<TR>", "").Replace("<TD>", "");
            shtml = Regex.Replace(shtml, "<a [^<]*>", "");
            shtml = Regex.Replace(shtml, "<font [^<]*>", "");
            shtml = Regex.Replace(shtml, "<FONT [^<]*>", "");
            shtml = Regex.Replace(shtml, "<hr [^<]*>", "");
            shtml = Regex.Replace(shtml, "<tr [^<]*>", "");
            shtml = Regex.Replace(shtml, "<td [^<]*>", "");
            shtml = Regex.Replace(shtml, "<font color=[^<]*>", "");
            shtml = Regex.Replace(shtml, "<A [^<]*>", "");
            shtml = Regex.Replace(shtml, "<script [^<]*>", "");
            shtml = Regex.Replace(shtml, "<TR [^<]*>", "");
            shtml = Regex.Replace(shtml, "<TD [^<]*>", "");
            shtml = Regex.Replace(shtml, "<TABLE [^<]*>", "");
            shtml = Regex.Replace(shtml, "<a [^<]*>", "");
            shtml = Regex.Replace(shtml, "<div.*?[^<]*>", "");
            shtml = Regex.Replace(shtml, "<DIV.*?[^<]*>", "");
            shtml = Regex.Replace(shtml, "<CENTER", "");
            shtml = Regex.Replace(shtml, "<TBODY>", "");
            shtml = Regex.Replace(shtml, "</TBODY>", "");
            shtml = Regex.Replace(shtml, "<table.*?[^<]>", "");

            shtml = Regex.Replace(shtml, "<P>", "");
            shtml = Regex.Replace(shtml, "</P>", "");

            shtml = Regex.Replace(shtml, "<p>", "");
            shtml = Regex.Replace(shtml, "</p>", "");

            shtml = Regex.Replace(shtml, "<strong>", "");
            shtml = Regex.Replace(shtml, "</strong>", "");

            shtml = Regex.Replace(shtml, "<STRONG>", "");
            shtml = Regex.Replace(shtml, "</STRONG>", "");
            shtml = shtml.Replace("<td>", "");
            shtml = shtml.Replace("</hr>", "");


            // shtml = shtml.Replace("<br>", "");
            shtml = Regex.Replace(shtml, "<P.*?[^<]*>", "");
            shtml = shtml.Replace("()", "");

            return shtml;
        }

        /// <summary>
        /// 替换掉网页无用的字符,简化快速版
        /// </summary>
        /// <param name="shtml"></param>
        /// <returns></returns>
        public static string shtmlQuick(string shtml)
        {
            shtml = shtml.Replace("</font>", "");
            shtml = Regex.Replace(shtml, "<font [^<]*>", "");
            shtml = shtml.Replace("&nbsp;", "");
            shtml = shtml.Replace("&lt;", "").Replace("&gt;", "").Replace("&amp;", "");
            shtml = shtml.Replace("<b>", "").Replace("</b>","");
            shtml = shtml.Replace("<strong>", "").Replace("</strong>", "");
            shtml = shtml.Replace("'", "");//数据库入库不能带 ' ' 号
            shtml = shtml.Replace("</td>", "").Replace("</tr>", "").Replace("</tr>", "").Replace("</table>", "").Replace("</div>", "");
            return shtml;
        }

        //该方法不好，要保证开的数据被关闭
        public static SqlDataReader searchInDB(SqlCommand sc, String ssql)
        {
            sc.CommandText = ssql;
            SqlDataReader dr1;  //只能用于读，要更新需要再开一个连接
            dr1 = sc.ExecuteReader();
            return dr1;
        }

        //执行命令返回一个表
        #region 返回datatable
        public static DataTable GetDateTable(string safeSql,SqlConnection conn)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(safeSql, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            return ds.Tables[0];
        }
        #endregion

        /// <summary>
        /// 执行sql，每次需要单独打开一个数据库链接，但是C#会自己维护一个连接池！
        /// </summary>
        /// <param name="strsql"></param>
        public static void runSqlAccess(String strsql, String strdbUrl)
        {
            OleDbConnection cn = null;
            try
            {
                cn = GetConAcess(strdbUrl);//连接数据库
                upDB(cn, strsql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn != null)
                {
                    cn.Close();
                    cn.Dispose();
                }
            }
        }

        /// <summary>
        /// 执行sql，每次需要单独打开一个数据库链接，但是C#会自己维护一个连接池！
        /// </summary>
        /// <param name="strsql"></param>
        public static void runSql(String strsql, String strdbUrl)
        {
            SqlConnection cn = null;
            try
            {
                cn = GetCon(strdbUrl);//连接数据库
                upDB(cn, strsql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn != null)
                {
                    cn.Close();
                    cn.Dispose();
                }
            }
        }

        public static Object runSqlOneResult(String strsql, String strResultTable, String strdbUrl)
        {
            SqlConnection cn = null;
            try
            {
                cn = GetCon(strdbUrl);//连接数据库
                return upDBOneResult(cn, strsql, strResultTable);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn != null)
                {
                    cn.Close();
                    cn.Dispose();
                }
            }
        }

        /// <summary>
        /// 分析用户请求是否正常,(有注入攻击?)
        /// </summary>
        /// <param name="Str">传入用户提交数据 </param>
        /// <returns>返回 false:含有SQL注入式攻击代码,otherwise True </returns>
        public static bool ProcessSqlStr(string Str)
        {
            if(Str == null) return false;
                if (Str.Trim() != "")
                {
                    if (SqlStrSplit == null)
                    {
                        SqlStrSplit =  
                            "and|exec|insert|select|delete|update|count|*|chr|mid|master|truncate|char|declare|xp_cmdshell|from|truncate|localgroup|net|user|'|\""
                            .Split('|');
                    }

                    foreach (string ss in SqlStrSplit)
                    {
                        if (Str.ToLower().IndexOf(ss) != -1)
                        {
                            return  false;
                        }
                    }
                }
            return true;
        }


        public static String getValueByStrName(String filename,String name)
        {
            string pathnow = "";
            if (filename == null || filename == "")
            {
                pathnow = System.Environment.CurrentDirectory  + @"\configwg.dat";
            }
            else
            {
                pathnow = filename;
            }
            try
            {
                XmlDocument xdom = new XmlDocument();
                xdom.Load(pathnow);
                XmlNodeList NDEvent = xdom.GetElementsByTagName("obj");

                for (int i = 0; i < NDEvent.Count; i++)
                {
                    XmlElement xe = (XmlElement)NDEvent.Item(i);
                    string st = xe.GetAttribute("name");
                    if (st != null && st.Trim() == name)
                    {
                        XmlNodeList cns = xe.ChildNodes;
                        String value = "";
                        if (cns != null)
                        {
                            foreach (XmlNode tcnode in cns)
                            {
                                if (tcnode.Name == "value")
                                {
                                    value = ((XmlCDataSection)(tcnode.FirstChild)).Data;
                                    return value;
                                }
                            }
                        }
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        //
        //  <obj name="460915期">    //46915期为 name  ,obj 为parentnodename
        //  <value><![CDATA[第460915期 (17:55) : 3.6.7.10.2(单倍: 6  双倍: 10)]]></value>
        //  <value1><![CDATA[单双      冠军        双        4      未中]]></value1>  //value1，value2为nodename
        //  <value2><![CDATA[单双      亚军        双        4      中奖]]></value2>		
        //</obj>
        public static String getValueByStrName(String filename, String name,String parentnodename,String nodename)
        {
            string pathnow = "";
            if (filename == null || filename == "")
            {
                pathnow = System.Environment.CurrentDirectory + @"\configwg.dat";
            }
            else
            {
                pathnow = filename;
            }
            try
            {
                XmlDocument xdom = new XmlDocument();
                xdom.Load(pathnow);
                XmlNodeList NDEvent = xdom.GetElementsByTagName(parentnodename);

                for (int i = 0; i < NDEvent.Count; i++)
                {
                    XmlElement xe = (XmlElement)NDEvent.Item(i);
                    string st = xe.GetAttribute("name");
                    if (st != null && st.Trim() == name)
                    {
                        XmlNodeList cns = xe.ChildNodes;
                        String value = "";
                        if (cns != null)
                        {
                            foreach (XmlNode tcnode in cns)
                            {
                                if (tcnode.Name.Equals(nodename))
                                {
                                    value = ((XmlCDataSection)(tcnode.FirstChild)).Data;
                                    return value;
                                }
                            }
                        }
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 根据名字从配置文件configwg.dat读数据,无数据返回空字符
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static String getValueByStrName(String name)
        {
            return getValueByStrName("", name);
        }


       
        public static int setValueByStrName(String filename,String name, String setvalue)
        {
            string pathnow = "";
            if (filename == null || filename == "")
            {
                pathnow = System.Environment.CurrentDirectory  +@"\configwg.dat";
            }
            else
            {
                pathnow = filename;
            }
            try
            {

                if (!File.Exists(pathnow))
                {
                    int ipoint = pathnow.LastIndexOf(@"\");
                    if (ipoint != -1)
                    {
                        String sdir = pathnow.Substring(0, ipoint);
                        if (!Directory.Exists(sdir))
                        {
                            Directory.CreateDirectory(sdir);
                        }
                    }
                    FileStream fs = File.Create(pathnow);
                    Byte[] info = new UTF8Encoding(true).GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "\r\n" + "<wind>" + "\r\n" + "</wind>");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                    fs.Close();
                }
                XmlDocument xdom = new XmlDocument();
                xdom.Load(pathnow);
                XmlNodeList NDEvent = xdom.GetElementsByTagName("obj");
                if (NDEvent.Count != 0)
                {
                    for (int i = 0; i < NDEvent.Count; i++)
                    {
                        XmlElement xe = (XmlElement)NDEvent.Item(i);
                        string st = xe.GetAttribute("name");
                        if (st != null && st.Trim() == name)
                        {
                            XmlNodeList cns = xe.ChildNodes;
                            //String value = "";
                            if (cns != null)
                            {
                                foreach (XmlNode tcnode in cns)
                                {
                                    if (tcnode.Name == "value")
                                    {
                                        ((XmlCDataSection)(tcnode.FirstChild)).Data = setvalue;
                                        xdom.Save(pathnow);
                                        //value = ((XmlCDataSection)(tcnode.FirstChild)).Data;
                                        return 0;
                                    }
                                }
                            }
                        }
                    }
                }
                XmlNode root = xdom.DocumentElement;
                NDEvent = xdom.GetElementsByTagName("data");
                XmlNode dataNode = null;
                if (NDEvent.Count != 0)
                {
                    dataNode = NDEvent.Item(0);
                }
                else
                {
                    dataNode = xdom.CreateElement("data");
                    root.AppendChild(dataNode);
                }
                XmlElement xe2 = xdom.CreateElement("obj");
                bxdatap(xdom, "value", setvalue, xe2);
                XmlAttribute courseNameAttr = xdom.CreateAttribute("name");
                courseNameAttr.Value = name;
                xe2.Attributes.Append(courseNameAttr);
                dataNode.AppendChild(xe2);
                xdom.Save(pathnow);
                return 0;
                //原来没有现在新增加
            }
            catch
            {
                return -1;
            }
        }

            //
            //  <obj name="460915期">    //46915期为 name  ,obj 为parentnodename
            //  <value><![CDATA[第460915期 (17:55) : 3.6.7.10.2(单倍: 6  双倍: 10)]]></value>
        //  <value1><![CDATA[单双      冠军        双        4      未中]]></value1>  //value1，value2为nodename
            //  <value2><![CDATA[单双      亚军        双        4      中奖]]></value2>		
            //</obj>
        public static int setValueByStrName(String filename, String name, String setvalue,
            String parentnodename,String nodename)
        {
            string pathnow = "";
            if (filename == null || filename == "")
            {
                pathnow = System.Environment.CurrentDirectory + @"\configwg.dat";
            }
            else
            {
                pathnow = filename;
            }
            try
            {

                if (!File.Exists(pathnow))
                {
                    int ipoint = pathnow.LastIndexOf(@"\");
                    if (ipoint != -1)
                    {
                        String sdir = pathnow.Substring(0, ipoint);
                        if (!Directory.Exists(sdir))
                        {
                            Directory.CreateDirectory(sdir);
                        }
                    }
                    FileStream fs = File.Create(pathnow);
                    Byte[] info = new UTF8Encoding(true).GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "\r\n" + "<wind>" + "\r\n" + "</wind>");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                    fs.Close();
                }
                XmlDocument xdom = new XmlDocument();
                xdom.Load(pathnow);
                XmlNodeList NDEvent = xdom.GetElementsByTagName(parentnodename);
                if (NDEvent.Count != 0)
                {
                    for (int i = 0; i < NDEvent.Count; i++)
                    {
                        XmlElement xe = (XmlElement)NDEvent.Item(i);
                        string st = xe.GetAttribute("name");
                        if (st != null && st.Trim() == name)
                        {
                            XmlNodeList cns = xe.ChildNodes;
                            //String value = "";
                            if (cns != null)
                            {
                                foreach (XmlNode tcnode in cns)
                                {
                                    if (tcnode.Name.Equals(nodename))
                                    {
                                        ((XmlCDataSection)(tcnode.FirstChild)).Data = setvalue;
                                        xdom.Save(pathnow);
                                        //value = ((XmlCDataSection)(tcnode.FirstChild)).Data;
                                        return 0;
                                    }
                                }
                           }
                            //节点没有value要加入
                            bxdatap(xdom, nodename, setvalue, xe);
                            xdom.Save(pathnow);
                            return 0;
                        }
                    }
                }
                XmlNode root = xdom.DocumentElement;
                NDEvent = xdom.GetElementsByTagName("data");
                XmlNode dataNode = null;
                if (NDEvent.Count != 0)
                {
                    dataNode = NDEvent.Item(0);
                }
                else
                {
                    dataNode = xdom.CreateElement("data");
                    root.AppendChild(dataNode);
                }
                XmlElement xe2 = xdom.CreateElement(parentnodename);
                bxdatap(xdom, nodename, setvalue, xe2);
                XmlAttribute courseNameAttr = xdom.CreateAttribute("name");
                courseNameAttr.Value = name;
                xe2.Attributes.Append(courseNameAttr);
                dataNode.AppendChild(xe2);
                xdom.Save(pathnow);
                return 0;
                //原来没有现在新增加
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 根据名字设置配置文件configwg.dat 中参数的cdata值
        /// </summary>
        /// <param name="name"></param>
        /// <returns>0 成功设置 ， -1 设置不成功</returns>
        /// 
        //    <data>
        //    <obj name="cappicnum">
        //    <value>
        //    <![CDATA[37847]]>
        //    </value>
        //    </obj>
        //    </data>
        public static int setValueByStrName(String name,String setvalue)
        {
            return setValueByStrName("", name, setvalue);
        }


        /// <summary>
        /// 从配置文件configwg.dat读数据,无数据返回空字符
        /// </summary>
        /// <returns></returns>
        public static String getDbstr(String projectName,String spwd,String sname)
        {
            string pathnow = System.Environment.CurrentDirectory;
            pathnow = pathnow + @"\configwg.dat";
            try
            {
                XmlDocument xdom = new XmlDocument();
                xdom.Load(pathnow);
                XmlNodeList NDEvent = xdom.GetElementsByTagName("mm");

                for (int i = 0; i < NDEvent.Count; i++)
                {
                    XmlElement xe = (XmlElement)NDEvent.Item(i);
                    string st = xe.GetAttribute("name");
                    if (st != null && st.Trim() == projectName)
                    {
                        XmlNodeList cns = xe.ChildNodes;
                        if (cns != null)
                        {
                            String name = "", ip = "", catalog = "", pwd = "";
                            foreach (XmlNode tcnode in cns)
                            {
                                if (tcnode.Name == "dbip")
                                {
                                    ip = tcnode.InnerText.Trim();
                                }
                                else if (tcnode.Name == "name")
                                {
                                    name = ((XmlCDataSection)(tcnode.FirstChild)).Data;
                                }
                                else if (tcnode.Name == "pwd")
                                {
                                    pwd = ((XmlCDataSection)(tcnode.FirstChild)).Value;
                                }
                                else if (tcnode.Name == "catalog")
                                {
                                    catalog = tcnode.InnerText.Trim();
                                }
                            }
                            if (pwd == null || pwd == "") pwd = spwd;
                            if (name == null || name == "") name = sname;
                            return "Server=" + ip + ";database=" + catalog + ";uid=" + name + ";pwd=" + pwd;
                        }
                    }
                }
                return "";
            }
            catch 
            {
                return "";
            }
        }

        //取得形如"ABC 47 集" 这样的字符串的最后一串数字
        public static String getLastNumForStr(String str)
        {
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

        //取得形如"第36 话" 这样的字符串的第一串数字
        public static String getFirstNumForStr(String str)
        {
            int tmp = -1;
            str = str.Replace("大结局", "").Replace("全", "").Replace("完", "").Replace("集", "").Replace("话", "").Replace("第", "");
            for (int tt = 0; tt < str.Length; tt++)
            {
                if (!int.TryParse(str[tt] + "", out tmp))
                {
                    if (tt == 0) return "";
                    return str.Substring(0, tt);
                }
            }
            if (int.TryParse(str, out tmp)) return str;
            return "";
        }

        /// <summary>
        /// 根据name和cdata文本，创建cdata节点并返回
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="name"></param>
        /// <param name="cdata"></param>
        /// <returns></returns>
        public static XmlElement bxdata(XmlDocument xmlDoc, String name, String cdata)
        {
            XmlElement tenode = xmlDoc.CreateElement(name.Trim());
            XmlCDataSection cda = xmlDoc.CreateCDataSection(cdata.Trim());
            tenode.AppendChild(cda);
            return tenode;
        }

        /// <summary>
        /// 根据name和cdata文本，创建cdata节点并加入父节点
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="name"></param>
        /// <param name="cdata"></param>
        /// <param name="parent"></param>
        public static void bxdatap(XmlDocument xmlDoc, String name, String cdata, XmlNode parent)
        {
            XmlElement tenode = xmlDoc.CreateElement(name.Trim());
            XmlCDataSection cda = xmlDoc.CreateCDataSection(cdata.Trim());
            tenode.AppendChild(cda);
            parent.AppendChild(tenode);
        }

        /// <summary>
        ///根据name和data文本(innertext)，创建xnode节点并加入parent节点
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="parent"></param>
        public static void btxtdata(XmlDocument xmlDoc, String name, String data, XmlNode parent)
        {
            XmlNode xid = xmlDoc.CreateElement(name);
            xid.InnerText = data;
            parent.AppendChild(xid);
        }
    }
}

