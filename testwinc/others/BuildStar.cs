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


namespace testwinc.others
{
    class BuildStar
    {
        //private static WindForm wf = null;

        private static String strLxj =
          "select id,title,brief_Content,director,presenter,keytype2 from forenotice where keytype2 = 1 and id >  ";
        //private static String strMovie =
        // "select id,title,brief_Content,director,presenter,keytype2 from forenotice where keytype2 = 2";
        private static String strActor = "select id,compere_name,compere_work,production_mv,production_tv,iszhuci,tvid  from compere";

       // private static String strFindact = "select top 277 *  from compere  where compere_name = '";

        private static int idnow = 0;

        private static String sortid = " order by id";


         //同步数据表
        public static void makeStarNow()
        {
            SqlConnection cn = null;
            SqlConnection cn2 = null;
            SqlConnection cn3 = null;
            SqlDataAdapter cmd = null;
            DataSet ds = null;
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
           // SqlDataReader dr2 = null;  //只能用于读，要更新需要再开一个连接


            try
            {
                Boolean ib = true;
                if (ib)
                {
                    UseStatic.soutTdRtb2("本程序现在只能执行一次，已经被执行过了！");
                    return;
                }
                //1.获得电视节目表数据
                if (cn == null)
                    cn = Apis.GetCon(Apis.sDBUrl247);//连接247
                if (cn2 == null)
                    cn2 = Apis.GetCon(Apis.sDBUrl247);//连接247
                if (cn3 == null)
                    cn3 = Apis.GetCon(Apis.sDBUrl247);//连接247

                SqlCommand msc = cn.CreateCommand();
                SqlCommand msc2 = cn2.CreateCommand();
                SqlCommand msc3 = cn3.CreateCommand();
                msc.CommandText = strLxj + idnow + sortid;
                dr1 = msc.ExecuteReader();

                msc2.CommandText = strActor;
                cmd = new SqlDataAdapter(msc2);  //adapter可用于写和更新
                SqlCommandBuilder builder = new SqlCommandBuilder(cmd);
                ds = new DataSet();
                cmd.Fill(ds, "commonMovie");
                DataTable myDataTable = ds.Tables["commonMovie"];

                if (dr1 == null) return;

                while (dr1.Read())
                {
                    //解析数据得到每一个人名，及角色
                    String id = dr1["id"].ToString().Trim();
                    String title = dr1["title"].ToString().Trim();
                    String brief_Content = dr1["brief_Content"].ToString().Trim();
                    String director = dr1["director"].ToString().Trim();                    //安澜,朱健华  //黎文彦,赖建国,张孝正 ,


                    int.TryParse(id, out idnow);

                    String presenter = dr1["presenter"].ToString().Trim(); 
                    //王刚 , 王亚楠 , 吕中 , 程莉莎 , 苏茂 , 郭晓晓                                                     
                    //何润东饰惠海 ,焦恩俊饰无尘&李如璧,胡静饰红绡 ,张铁林饰老方丈,
                    //林秀明--方子萱　　苏东平--郑斌辉,潘志豪--陈汉玮　　朱晓非--白微秀,martin--许立桦　　郑良一--黄俊雄,



                    String keytype2 = dr1["keytype2"].ToString().Trim();
                    director = director.Replace("，", ",").Replace("(","").Replace(")","");
                    presenter = presenter.Replace("，", ",").Replace("(", "").Replace(")", "");
                    String[] sdirs = director.Split(',');
                    String[] spres = presenter.Split(',');

                    //for 每一个人 查询 startid 及入库
                    foreach (String str2 in sdirs)
                    {
                        String str = str2.Trim();
                        if (str != "")
                        {

                            saveName(id, str, myDataTable, msc3, 2, "");
                        }
                    }

                    foreach (String str2 in spres)
                    {
                        String str = str2.Trim();
                        if (str != "")
                        {
                            str = str.Replace("饰演","/").Replace("饰","/").Replace("--","/").Replace("-","/");
                            String name = "";
                            String sjsm = "";
                            int it = str.IndexOf("/");
                            if (it != -1)
                            {
                                name = str.Substring(0, it);
                                sjsm = str.Substring(it + 1).Replace("/","");//多个斜杠要替换掉
                                if (sjsm.Length > 10) sjsm = "";
                            }
                            else
                            {
                                name = str;
                            }
                            saveName(id, name, myDataTable, msc3, 1, sjsm);
                        }
                    }


                }
 

                //2.获得明星数据
                //对每一个明星及主持人，获得对应影视片，主持栏目，

                //解析数据，入库
                UseStatic.soutTdRtb2("build star 已经完成");
            }
            catch (Exception ex)
            {
                idnow = idnow + 1;
                UseStatic.soutTdRtb2(ex.StackTrace);
            }
            finally
            {
                if (WindForm.isBuildStar == false)
                    UseStatic.soutTdRtb2("build star已经被人工终止！");
                WindForm.isBuildStar = false;
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
                WindForm.oneCatchEnd("isBuildStar");
            }     


        }//end makeStarNow

        private static void saveName(String id,String name,DataTable myDataTable,SqlCommand msc3,int type ,String sjsm) 
        {

            String aname = name;
            String stype = type + "";
            // String sql1 = "";

            foreach (DataRow row in myDataTable.Rows)
            {
                String sid = row["id"] + "";
                String scname = row["compere_name"] + "";
                //String production_mv = row["production_mv"] + "";
                //String production_tv = row["production_tv"] + "";
                //String iszhuci = row["iszhuci"] + "";
                //String tvid = row["tvid"] + "";
                //String compere_work = row["compere_work"] + "";

                if (scname.Trim() == aname.Trim())//数据相等，入库
                {
                    insertDbStar(aname, id, sid, stype, sjsm, msc3);
                    UseStatic.soutTdRtb2(scname + " 已经被入库！sid : " + sid + " foreid : " + id);
                    break;
                }
            }
        }



        private static void insertDbStar(String name,String forid,String starid,String type,String jsm,SqlCommand sc)
        {
            name = Apitool.filterDBStr(name);
            jsm = Apitool.filterDBStr(jsm);
            //此处某些参数无值要赋0
            if (starid == "" || starid == "null" || starid == "NULL") starid = "0";
            if (type == "" || type == "null" || type == "NULL") type = "0";
            String sql = "insert into tab_star (tab_name,tab_forenoticeid,tab_starid,tab_typeid,tab_jiaosheming) values('"+
                name+"','"+forid+"','"+starid+"','"+type+"','"+jsm+"')";
            sc.CommandText = sql;
            sc.ExecuteNonQuery();
        }

    }//end class BuiildStar
}
