using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using testwinc.tools;
using System.Windows.Forms;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.OleDb;

namespace testwinc.capvideo
{
    class Pipei
    {
        private static WindForm wf = null;

        /// <summary>
        /// 匹配视频数据

        /// </summary>
        public static void piPeiMovie()
        {
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
            SqlConnection cn = null;
            SqlConnection cn2 = null;
            SqlDataAdapter cmd = null;
            DataSet ds = null;
            SqlConnection cn3 = null;
            SqlDataReader sdr3 = null;
            if (wf == null) wf = UseStatic.getWindForm();
            try
            {
                wf.addRtb2ForThread("开始匹配数据 " + DateTime.Now.ToString() + "===========================  \r\n");
                for (int it = 1; it < 5; it++)//Capmovie.ITvNum.Length
                {
                    if (WindForm.isPiPei == false)
                    {
                        return;
                    }

                    int line = 0, itime = 0;

                    String strfind = "select id,title,years,type,foreid,time,director,presenter,lang,tvnum,zjs,edittime from movie where tvnum = ";
                    String strfind2 =
                        "select  top 12 id,title,brief_Content,properties,director,presenter,keytype2,js,keytype,years,nian from forenotice where  keytype2 = 2 and title like '%";
                    String strfind3 = "%'";
                    //String strupdate = "update movie set foreid = '";
                    //String strupdate2 = "' where id = ";
                    //String sql = "";
                    String strfind4 = "select top 1 ordernum from program_preview where forenoticeID = ";
                    String strfind5 = " order by ordernum desc";

                    if(cn == null)
                        cn = Apis.GetCon(Apis.sDBUrl206);
                    SqlCommand msc = cn.CreateCommand();
                    msc.CommandText = strfind + it +" and foreid = 0 order by id desc";

                    cmd = new SqlDataAdapter(msc);  //adapter可用于写和更新
                    SqlCommandBuilder builder = new SqlCommandBuilder(cmd);
                    ds = new DataSet();

                    cmd.Fill(ds, "movie");
                    DataTable myDataTable = ds.Tables["movie"];
                    if (cn2 == null)
                         cn2 = Apis.GetCon(Apis.sDBUrl247);//连接247  access不能用，数据不够
                    SqlCommand msc2 = cn2.CreateCommand();

                    SqlCommand msc3 = null;
                    Boolean isrun = true;

                    if (it == Capmovie.ITvNum[4])
                    {
                        strfind2 = 
                            "select  top 12 id,title,director,presenter,js,years,nian from forenotice where  title like '%";
                    }
                    if (it == Capmovie.ITvNum[0])
                    {
                        strfind2 =
                            "select  top 12 id,title,brief_Content,properties,director,presenter,js,years,nian from forenotice where  keytype2 = 1 and title like '%";
                    }
                    if (it == Capmovie.ITvNum[3])
                    {
                        strfind2 =
                            "select top 12 id,title,director,js,years,nian from forenotice where  keytype = 0 and title like '%";
                    }

                    foreach (DataRow row in myDataTable.Rows)
                    {
                        if (WindForm.isPiPei == false)
                        {
                            return;
                        }
                        line++;
                        if (line == 300)
                        {
                            itime++;
                            line = 0;
                            wf.addRtb2ForThread("已匹配" + itime * 100 + "条数据!================================");
                            int runnum = cmd.Update(myDataTable);
                            wf.addRtb2ForThread("had update " + runnum + " 行数据");
                        }
                        // wf.addRtb2ForThread("id: " + row[0] + "  title:  " + row[1]);////积奇玛莉(粤语版)
                        String ryear = row["years"].ToString().Trim();
                        String rtitle = getTitle(row[1].ToString().Trim(), it);
                        msc2.CommandText = strfind2 + rtitle + strfind3;
                        dr1 = msc2.ExecuteReader();
                        isrun = true;

                        while (dr1.Read() && isrun)
                        {
                            if (WindForm.isPiPei == false)
                            {
                                return;
                            }
                            String dire = dr1["director"].ToString().Trim();  //尹弢,吕舸,蒋文倩<BR>
                            String title = dr1["title"].ToString().Trim();

                            String year = dr1["years"].ToString().Trim();
                            if (year == "") year = dr1["nian"]+"";
                            String zjs = dr1["js"].ToString().Trim();

                            isOneInTwo(row["director"] + "", getDirector(dire));
                            StringBuilder sbd = new StringBuilder();
                            if (title == rtitle)
                            {
                                if (it == Capmovie.ITvNum[0])
                                {
                                    //名字完全相同，导演相同，则年份不重要//,原数据不全，没办法只能用名字了
                                    if (isOneInTwo(row["director"] + "", getDirector(dire)) || year == ryear)
                                    {
                                        //sql = strupdate + dr1[0].ToString().Trim() + strupdate2 + row[0];
                                        row["foreid"] = dr1[0].ToString();
                                        row["edittime"] = DateTime.Now.ToString();
                                        isrun = false;
                                        sbd.Append("id: ");
                                        sbd.Append(row[0]);
                                        sbd.Append("  title:  ");
                                        sbd.Append(row[1]);
                                        sbd.Append(" foreid: ");
                                        sbd.Append(row["foreid"]);
                                        wf.addRtb2ForThread(sbd.ToString());
                                        ////积奇玛莉(粤语版)
                                        //wf.addRtb2ForThread("id: " + row[0] + "  title:  " + row[1] +"foreid: " +row["foreid"]);  
                                    }
                                }
                                else if (it == Capmovie.ITvNum[1] || it == Capmovie.ITvNum[2])
                                {
                                    row["foreid"] = dr1[0].ToString();
                                    row["edittime"] = DateTime.Now.ToString();
                                    isrun = false;
                                    sbd.Append("id: ");
                                    sbd.Append(row[0]);
                                    sbd.Append("  title:  ");
                                    sbd.Append(row[1]);
                                    sbd.Append(" foreid: ");
                                    sbd.Append(row["foreid"]);
                                    wf.addRtb2ForThread(sbd.ToString());
                                }
                                else if (it == Capmovie.ITvNum[4])
                                {
                                    row["foreid"] = dr1[0].ToString();
                                    row["edittime"] = DateTime.Now.ToString();
                                    isrun = false;
                                    sbd.Append("id: ");
                                    sbd.Append(row[0]);
                                    sbd.Append("  title:  ");
                                    sbd.Append(row[1]);
                                    sbd.Append(" foreid: ");
                                    sbd.Append(row["foreid"]);
                                    wf.addRtb2ForThread(sbd.ToString());
                                }
                                else if (it == Capmovie.ITvNum[3]) 
                                {
                                    row["foreid"] = dr1[0].ToString();
                                    row["edittime"] = DateTime.Now.ToString();
                                    isrun = false;
                                    sbd.Append("id: ");
                                    sbd.Append(row[0]);
                                    sbd.Append("  title:  ");
                                    sbd.Append(row[1]);
                                    sbd.Append(" foreid: ");
                                    sbd.Append(row["foreid"]);
                                    wf.addRtb2ForThread(sbd.ToString());
                                }
                            }
                            else
                            {   //名字类似，导演相同，还需年份相同
                                if (it == Capmovie.ITvNum[0])
                                {
                                    if (isOneInTwo(row["director"] + "", getDirector(dire)))
                                    {
                                        if (year == ryear)
                                        {
                                            row["foreid"] = dr1[0].ToString();
                                            row["edittime"] = DateTime.Now.ToString();
                                            isrun = false;
                                            sbd.Append("id: ");
                                            sbd.Append(row[0]);
                                            sbd.Append("  title:  ");
                                            sbd.Append(row[1]);
                                            sbd.Append(" == ");
                                            sbd.Append(rtitle);
                                            sbd.Append(" foreid: ");
                                            sbd.Append(row["foreid"]);
                                            wf.addRtb2ForThread(sbd.ToString());
                                            //wf.addRtb2ForThread("id: " + row[0] + "  title:  " + row[1] + " == " + rtitle);
                                        }
                                    }
                                }
                                else if (it == Capmovie.ITvNum[1] || it == Capmovie.ITvNum[2])
                                {
                                    String szjs = row["zjs"].ToString();
                                    if (szjs != "")
                                    {
                                        if (zjs != "")
                                        {
                                            if (zjs == szjs)
                                            {
                                                row["foreid"] = dr1[0].ToString();
                                                row["edittime"] = DateTime.Now.ToString();
                                                isrun = false;
                                                sbd.Append("id: ");
                                                sbd.Append(row[0]);
                                                sbd.Append("  title:  ");
                                                sbd.Append(row[1]);
                                                sbd.Append(" == ");
                                                sbd.Append(rtitle);
                                                sbd.Append(" foreid: ");
                                                sbd.Append(row["foreid"]);
                                                wf.addRtb2ForThread(sbd.ToString());
                                            }
                                        }
                                        else
                                        {//从剧集表中读集数
                                            if (cn3 == null)
                                            {
                                                cn3 = Apis.GetCon(Apis.sDBUrl247);
                                            }
                                            if (msc3 == null) msc3 = cn3.CreateCommand(); 
                                            sdr3 = Apis.searchInDB(msc3, strfind4 + dr1[0].ToString() + strfind5);
                                            if (sdr3.Read())
                                            {
                                                if (szjs == sdr3[0].ToString())
                                                {
                                                    row["foreid"] = dr1[0].ToString();
                                                    row["edittime"] = DateTime.Now.ToString();
                                                    isrun = false;
                                                    sbd.Append("id: ");
                                                    sbd.Append(row[0]);
                                                    sbd.Append("  title:  ");
                                                    sbd.Append(row[1]);
                                                    sbd.Append(" == ");
                                                    sbd.Append(rtitle);
                                                    sbd.Append(" foreid: ");
                                                    sbd.Append(row["foreid"]);
                                                    wf.addRtb2ForThread(sbd.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (it == Capmovie.ITvNum[4])
                                {
                                    if (rtitle.Length >= 4)//4个以上名字类似就认为是
                                    {
                                        row["foreid"] = dr1[0].ToString();
                                        row["edittime"] = DateTime.Now.ToString();
                                        isrun = false;
                                        sbd.Append("id: ");
                                        sbd.Append(row[0]);
                                        sbd.Append("  title:  ");
                                        sbd.Append(row[1]);
                                        sbd.Append(" == ");
                                        sbd.Append(rtitle);
                                        sbd.Append(" foreid: ");
                                        sbd.Append(row["foreid"]);
                                        wf.addRtb2ForThread(sbd.ToString());
                                    }
                                }
                            }
                            if (sdr3 != null) sdr3.Close();
                        }
                        dr1.Close();
                    }
                    int runnum2 = cmd.Update(myDataTable);
                    ds.Dispose();
                    wf.addRtb2ForThread("had update " + runnum2 + " 行数据 tvnum = " + it + " 已被匹配 " + DateTime.Now.ToString());
                }
            }
            catch (Exception e)
            {
                wf.addRtb2ForThread(e.StackTrace);
            }
            finally
            {
                if(WindForm.isPiPei == false)
                    wf.addRtb2ForThread("匹配已经被人工终止！");
                WindForm.isPiPei = false;
             
                if (ds != null)
                {
                    ds.Dispose();
                    ds = null;
                }
                if (dr1 != null)
                {
                    dr1.Close();
                    dr1.Dispose();
                    dr1 = null;
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
                if (sdr3 != null)
                {
                    sdr3.Close();
                    sdr3.Dispose();
                    sdr3 = null;
                }
                if (cn3 != null)
                {
                    cn3.Close();
                    cn3.Dispose();
                    cn3 = null;
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
                wf.Invoke((MethodInvoker)delegate { wf.setbtnpipei(true); });
                WindForm.oneCatchEnd("isPiPei");
            }

        }

        private static String[] getDirector(String strdir) 
        {
            if (strdir == null || strdir.Trim() == "") return null;
            //尹弢,吕舸,蒋文倩<BR>
            strdir = strdir.Replace("<BR>","/").Replace(",","/").Trim();
            return strdir.Split(new char[]{'/'});

        }

        private static String getTitle(String strtit,int inum)
        {
            int tt = strtit.IndexOf("(");
            if (tt != -1)
            {
                strtit = strtit.Substring(0, tt);
            }
            if (inum == Capmovie.ITvNum[4])
            {
                tt = strtit.IndexOf("200");
                if (tt != -1)
                {
                    strtit = strtit.Substring(0, tt).Trim();
                }
                else 
                {
                    tt = strtit.IndexOf("201");
                    if (tt != -1)
                    {
                        strtit = strtit.Substring(0, tt).Trim();
                    }
                }
            }
            return strtit;
        }

        private static Boolean isOneInTwo(String one,String[] two)
        {
            if(one == null || two == null || one == "") return false;
            foreach (String tw in two) 
            {
                if (one == tw) return true;
            }
            return false;
        }

    }//end Pipei
}
