using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using testwinc;
using testwinc.tools;
using testwinc.capvideo;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Collections;
using testwinc.others;

namespace testwinc.test
{

    /**
    * <p>Title:  JustFortest </p>
    * <p>Description: 运行测试类</p>
    * <p>Copyright: 2011</p>
    * @author wind
    * @version 1.0
    */
    class JustFortest
    {
        private WindForm wf = null;

        static JustFortest() 
        {
           
        }


        public JustFortest()
        {
            wf = UseStatic.getWindForm();
        }

        // todo:运行测试
        public void runtest() 
        {
            try
            {
                wf.getBtngo().Enabled = false;

                // TestXML txml = new TestXML();
                //txml.readXML2("");

                UseStatic.sout(1001 / 200);

                UseStatic.sout(999 / 200);

                UseStatic.sout(Encoding.Default.ToString());

                String sDate = Convert.ToDateTime("2012/01/02").ToString("yyyy-MM-dd");
                UseStatic.sout("strs is: " + sDate);

                Hashtable ht = new Hashtable();
                String stp2 = "123";
                String stp3 = "345";
                ht.Add(1, stp2);
                ht.Add(2, stp3);
                stp2 = "lalaal";

                String str = "明天";
                //str = "剧场  35集电视剧 抗日";

                char[] ss = str.ToCharArray();
                UseStatic.sout("String is: " + str);
                int it = 0;
                foreach (char vs in ss)
                {
                    int ic = vs;
                    UseStatic.sout("char " + it + " is:" + vs + " num:" + ic);
                    it++;
                }

               
                UseStatic.sout("3,3,3,1    " + TestACMPoint24.can24(4, 13, 5, 7) + "  " + TestACMPoint24.getExpress());
                UseStatic.sout("13,13,1,1   " + TestACMPoint24.can24(13, 13, 1, 1) + "  " + TestACMPoint24.getExpress());
                UseStatic.sout("1,1,1,1   " + TestACMPoint24.can24(1, 1, 1, 1) + "  " + TestACMPoint24.getExpress());

                UseStatic.sout("2,2,2,12   " + TestACMPoint24.can24(9, 11, 7, 5) + "  " + TestACMPoint24.getExpress());

                UseStatic.sout(DateTime.Now.ToString("MMdd-hh:mm"));

                UseStatic.sout(Apitool.md5Run32("771230"));

               //TestDbo tdb = new TestDbo();
               // tdb.run();

                //UseStatic.sout("ass is " + ass );

                //Boolean isCappptv= true;//标志是否抓取pptv
                //Boolean isCapyuku = true;//标志是否抓取pptv
                //Boolean[] isCapture = {isCappptv, isCapyuku};

                //isCapture[0] = false;
                //isCapture[1] = false;
                //UseStatic.sout("isCappptv " + isCappptv);

                //String tmp = "";
                //String name = "黄axZ过";
                //String pwd = "1234567890abcABCxyzXYZ";
                //String st = name.Substring(0, 1);
                //String se = name.Substring(name.Length - 1, 1);
                //pwd = se + pwd + st;
                //Char[] cs = pwd.ToCharArray();
                //Char[] cs2 = new char[cs.Length];

                //for(int it = 0; it< cs.Length;it++)
                //{
                //    cs2[it] = (char)(cs[cs.Length - it-1]+7 );
                //}

                //tmp = new String(cs2);

                //Char[] cs3 = "黋a`_JIHjih7@?>=<;:98迎".ToCharArray();
                //Char[] cs4 = tmp.ToCharArray();

                //for (int it = 0; it < cs3.Length; it++)
                //{
                //    cs4[it] = (char)(cs3[cs3.Length - it - 1] -7);
                //}

                //tmp = new String(cs2);

                //String pwdnew = new String(cs4);
                //pwdnew= pwdnew.Substring(1,pwdnew.Length - 2);



                // String ss = Apis.getDbConnStr();

                //TestDelegate td = new TestDelegate();
                //WindMonitor wm2 = new WindMonitor(td);
                //td.setString("abc");
                //td.setString("i love u");
                //wm2.removeListener(td);
                //td.setString("i love u");


                // TestDbo td = new TestDbo();
                //td.run();
                // String ss = Apitool.FindStrByName("ab\"" + "asffcde", "b", "e");
                //wf.sout("show me the money 500W！");
                //TestString ts = new TestString();
                // ts.buildTestString();
                //Charpinyin.testpinyin();


                // TestXML testX = new TestXML();
                // string pathnow = System.Environment.CurrentDirectory;
                //testX.testXML(1);
                // testX.readXML(pathnow + @"\test\" + "testy.xml");
                // testX.writeXML("");


            }
            catch (Exception ex)
            {
                UseStatic.soutTd(ex.ToString());
            }
            finally
            {
                wf.getBtngo().Enabled = true;
            }
        }//end runtest


        private static void isInDb()
        {
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
            SqlConnection cn = null;
            SqlConnection cn2 = null;
            try
            {
                String strfind = "select id,surl from movieurl";
                cn = Apis.GetCon(Capmovie.strdbUrl);//连接数据库
                cn2 = Apis.GetCon(Capmovie.strdbUrl);//连接数据库
                SqlCommand msc = cn.CreateCommand();
                SqlCommand msc2 = cn2.CreateCommand();
                msc.CommandText = strfind;

                dr1 = Apis.searchInDB(msc, strfind);
                while (dr1.Read())
                {
                    String dire = dr1["surl"].ToString().Trim();
                   
                    int it = dire.IndexOf("http://");
                    if (it != -1)
                    {
                        dire = dire.Substring(it + "http://".Length);
                        String id = dr1["id"].ToString().Trim();
                        String strUP = "update movieurl set surl = '" + dire + "' where id = '" + id + "'";
                        Apis.UpdateDB(msc2,strUP);
                    }
                }
            }
            catch (Exception ex)
            {
                UseStatic.soutTd(ex.ToString());
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
            return ;
        }

        private String inverstStr(String str)
        {
            if (str == null) return null;

            Char[] ch = str.ToCharArray();
           
            int il = ch.Length;
            Char[] ca = new char[il];

            for (int it = 0; it < il; it++)
            {
                ca[il - it - 1] = ch[it];
            }

            return new String(ca);
        }


    }// end JustFortest
}
