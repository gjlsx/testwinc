using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using testwinc;
using testwinc.tools;


using Microsoft.SqlServer.Server;

namespace testwinc.test
{
    class TestDbo
    {
        private SqlConnection cn = null;
        private Apis ap = null;

        public TestDbo()
        {
   
        }

        public void run()
        {
            try
            {
                //若数据库中已有该openid,则跳转到我的频道页面，
                ap = new Apis();
                cn = Apis.GetCon(Apis.sDBUrlrain);//连接数据库
                SqlCommand myCommand1 = cn.CreateCommand();
                //SqlDataReader dr1;  //只能用于读，要更新需要再开一个连接
                // String strqq = "select top 10 * from userid_other where qqid = '6A2DCAFA87A69B768329AB789D342A63'";
                // String strInser = "insert into userid_other values('tempgg','dadsdssBB9A3160F013515C087B6DA2259','')";

                myCommand1.CommandText = "select top 10 * from temp";
                SqlDataAdapter cmd = new SqlDataAdapter(myCommand1);  //adapter可用于写和更新
                // SqlCommandBuilder builder = new SqlCommandBuilder(cmd);  //for update 用

                DataSet ds = new DataSet();
                
                cmd.Fill(ds, "others");
                DataTable myDataTable = ds.Tables["others"];

                foreach (DataRow row in myDataTable.Rows)
                {
                    UseStatic.sout("id is: " + row[0] + "  name is:  " + row[1]);// + "  qqid is" + row[2]);
                }

                //dr1 = myCommand1.ExecuteReader();
                /**
                String name3 = "ss23";
                Boolean isExist = isUserExistInDB(myCommand1, name3);

                if (testPwd("lxcjian", ""))
                {
                    UseStatic.sout("pwd iis true");
                }
                else
                {
                    UseStatic.sout("pwd iis false");
                }

                if (!isExist)
                {
                    //若无则插入数据
                    inserDB(name3, "dads3515C087B6DAee");
                }
                 **/

                ds = null;
                cn.Close();//关闭数据库连接
                cn.Dispose();	//释放资源

            }
            catch (Exception e)
            {
                UseStatic.sout(e.StackTrace);
            }
            finally 
            {
                cn.Close();//关闭数据库连接
                cn.Dispose();	//释放资源
            }
        }

        private void inserDB(String sname, String sqqid)
        {
            String strInser = "insert into userid_other values('" + sname + "','" + sqqid + "','')";
            if (ap == null)
            {
                ap = new Apis();
            }
            cn = ap.GetCon();//连接数据库
            SqlCommand sc = cn.CreateCommand();
            sc.CommandText = strInser;
            sc.ExecuteNonQuery();
            cn.Close();
        }

        private Boolean isUserExistInDB(SqlCommand sc, String username)
        {
            String strFind = "select top 1 * from userid_other where name = '" + username + "'";
            sc.CommandText = strFind;
            SqlDataReader dr1;  //只能用于读，要更新需要再开一个连接
            dr1 = sc.ExecuteReader();
            if (dr1.Read())
            {
                UseStatic.sout(dr1["name"] + "is already exist");
                return true;
            }
            return false;
        }

        private bool testPwd(string UserName, string Password)
        {
            // Insert code that implements a site-specific custom 
            // authentication method here.
            //从数据库中查用户信息对不对
            String strFind = "select top 1 id,username,password,is_vip,num,truename,mobile,dt,province,city from users where username= '"
                + UserName + "'";
            if (ap == null)
            {
                ap = new Apis();
            }
            cn = ap.GetCon();//连接数据库
            SqlCommand myCommand1 = cn.CreateCommand();

            SqlDataReader dr1 = Apis.searchInDB(myCommand1, strFind);
            while (dr1.Read())
            {
                //String snamemd5 = Apitool.md5Run(Password,16);

                UseStatic.sout(dr1["password"] + "is eaual user inputpwd");
                if (dr1["username"].ToString() == UserName)
                {
                    dr1.Close();
                    cn.Close();
                    return true;
                }
            }
            dr1.Close();
            cn.Close();
            return false;
        }


        //测试存储过程
        [Microsoft.SqlServer.Server.SqlProcedure]
        public static void SPOne()
        {

            SqlPipe p;

            SqlCommand sCmd = new SqlCommand();
            sCmd.CommandText = "Select top 2 * from movie";

            p = SqlContext.Pipe;
            p.ExecuteAndSend(sCmd);

        }

    }
}
