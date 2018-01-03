using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using testwinc.tools;
using System.Windows.Forms;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;

namespace testwinc.capvideo
{
    class Tongbu
    {
        private static WindForm wf = null;
        private static String strFinddb =
            "select movie.id,movie.foreid,movie.tvnum,movieurl.js,movieurl.isTongbu,movieurl.id ,movie.title,movie.isTJ,movieurl.videotype from movieurl inner join movie on movieurl.movieid = movie.id where movieurl.movieid in (select id from movie where foreid > 0) ";
        private static String strInsertdb = "insert into commonMovie(js,movieId,foreid,createTime,tvnum,title,istj,videotype,urlid) values(";//'0','12345','99999','2011-11-26 10:03:57','5')";
        private static String strUpdatedb = "update movieurl set isTongbu = '1' where id = '";
        private static String strUpdb247 = "update commonMovie set videotype = ";//'0','12345','99999','2011-11-26 10:03:57','5')";
       

        private static readonly String StrTongbuTime = "tongbutime";//抓取间隔，初始值12，在配置文件确定
        private static int iTongbuTime = 12;//每隔12小时同步一次，初始值12，具体根据配置文件确定

        public static Boolean isAllwaysTongbu = true;
        public static Boolean isAutoRun = false;  // 线程是不是已经在运行


        static Tongbu()
        {
            String st = Apis.getValueByStrName(StrTongbuTime);
            if (st != "")
            {
                int tmp = 0;
                if (int.TryParse(st, out tmp))
                    iTongbuTime = tmp;
                else
                    Apis.setValueByStrName(StrTongbuTime, iTongbuTime + "");

            }
            else
            {
                Apis.setValueByStrName(StrTongbuTime, iTongbuTime + "");
            }
        } 


        /// <summary>
        /// 同步数据库

        /// </summary>
        public static void tongbuDb()
        {
            tongbuDb(false);
        }

        private static void tongbuDb(Boolean isrun)
        {
            SqlConnection cn = null;
            SqlConnection cn2 = null;
            SqlConnection cn3 = null;
            SqlDataAdapter cmd = null;
            DataSet ds = null;

            if (wf == null) wf = UseStatic.getWindForm();
            try
            {
                int itype = CapYouku.getParaByInput("t");
                String sFrocetb = "";
                if (itype != 1)  //强制同步所有已经同步的
                {
                    sFrocetb = "and movieurl.isTongbu = 0";
                }

                wf.addRtb2ForThread("开始同步,每隔12小时同步一次，同步时间可在配置文件中修改！     "+ DateTime.Now.ToString());

                int line = 0, itime = 0;

                if (cn == null)
                    cn = Apis.GetCon(Apis.sDBUrl206);
                SqlCommand msc = cn.CreateCommand();
                msc.CommandText = strFinddb + sFrocetb;
                if (cn3 == null)
                    cn3 = Apis.GetCon(Apis.sDBUrl206);
                SqlCommand msc3 = cn.CreateCommand();

                cmd = new SqlDataAdapter(msc);  //adapter可用于写和更新
                SqlCommandBuilder builder = new SqlCommandBuilder(cmd);
                ds = new DataSet();

                cmd.Fill(ds, "commonMovie");
                DataTable myDataTable = ds.Tables["commonMovie"];
                if (cn2 == null)
                    cn2 = Apis.GetCon(Apis.sDBUrl247);//连接247
                SqlCommand msc2 = cn2.CreateCommand();


                //SqlCommand msc3 = null;
                //Boolean isrun = true;


                foreach (DataRow row in myDataTable.Rows)
                {
                    if (WindForm.isTongbu == false)
                    {
                        return;
                    }
                    line++;
                    if (line == 200)
                    {
                        itime++;
                        line = 0;
                        wf.addRtb2ForThread("已同步" + itime * 200 + "条数据!================================");

                    }

                    String sid = row["id"] + "";
                    String sfid = row["foreid"] + "";
                    String stvnum = row["tvnum"] + "";
                    String sjs = row["js"] + "";
                    String stb = row["isTongbu"] + "";
                    String surlid = row[5] + "";   //movieurlid
                    String stitle = row[6] + "";
                    String stj = row["isTJ"] + "";
                    String svt = row["videotype"] + "";


                    StringBuilder sb = new StringBuilder();
                    if (stb == "1")
                    {
                        sb.Append(strUpdb247);
                        sb.Append(" '");
                        sb.Append(svt);
                        sb.Append("' where urlid = '");
                        sb.Append(surlid);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(strInsertdb);
                        sb.Append(" '");
                        sb.Append(sjs);
                        sb.Append("','");
                        sb.Append(sid);
                        sb.Append("','");
                        sb.Append(sfid);
                        sb.Append("','");
                        sb.Append(DateTime.Now.ToString());
                        sb.Append("','");
                        sb.Append(stvnum);
                        sb.Append("','");
                        sb.Append(stitle);
                        sb.Append("','");
                        sb.Append(stj);
                        sb.Append("','");
                        sb.Append(svt);
                        sb.Append("','");
                        sb.Append(surlid);
                        sb.Append("')");
                    }

                    String sins = sb.ToString(); // + '0','12345','99999','2011-11-26 10:03:57','5')";js,movieId,foreid,createTime,tvnum
                    msc2.CommandText = sins;
                    if (msc2.ExecuteNonQuery() == 1)
                    {
                        if (stb != "1")
                        {
                            String sup = strUpdatedb + surlid + "'";//同步完设置标志位为1 
                            msc3.CommandText = sup;
                            msc3.ExecuteNonQuery();
                        }
                    }
                }
                if(line != 0)
                    wf.addRtb2ForThread("已同步 " + line + " 条数据!================================");
                ds.Dispose();
                wf.addRtb2ForThread("全部同步已经完成！" +DateTime.Now.ToString());  
            }
            catch (Exception e)
            {
                wf.addRtb2ForThread(e.StackTrace);
            }
            finally
            {
                if (WindForm.isTongbu == false)
                    wf.addRtb2ForThread("同步已经被人工终止！");
                WindForm.isTongbu = false;
                //wf.Invoke((MethodInvoker)delegate { wf.setbtnpipei(true); });
                if (ds != null)
                {
                    ds.Dispose();
                    ds = null;
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
                if (cn2 != null)
                {
                    cn2.Close();
                    cn2.Dispose();
                    cn2 = null;
                }
                if (cn != null)
                {
                    cn.Close();
                    cn.Dispose();
                    cn = null;
                }
                if (cn3 != null)
                {
                    cn3.Close();
                    cn3.Dispose();
                    cn3 = null;
                }
                WindForm.oneCatchEnd("isTongbu");
            }

            if (isAllwaysTongbu)//一旦开始同步就不能停下来，要停就关闭程序 
            {
                if ((!isAutoRun) || isrun) //auto run 已经运行 ，无需再次运行
                {
                    isAutoRun = true;
                    Thread.Sleep(3600000 * iTongbuTime);//1000*60*60*12 = 60000*720
                    WindForm.isTongbu = true;
                    tongbuDb(true);
                }
            }

        }
    }
}
