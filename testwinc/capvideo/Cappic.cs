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
using System.Net;
using System.IO;

namespace testwinc.capvideo
{
    /// <summary>
    /// 抓取图片到本地再上传到服务器
    /// </summary>
    class Cappic
    {
        private static readonly String strCapPic = "select id,imageurl from movie where id > ";
        private static int iGotFileNum = 0;
        private static String saveUrl = "e:\\webvideo\\pic";
        private static String Idnow = "37847";
        private static int IReConnetTime = 97;
        public static Boolean isAllwayscap = true;//是否始终抓取
        private static readonly String StrCapPicNum = "cappicnum";
        private static int iLastUpdate = 37927;

        private static readonly String StrCapPicTime = "cappictime";//抓取间隔，初始值12，在配置文件确定
        private static int iCapTime = 12;//每隔12小时抓取一次，初始值12，具体根据配置文件确定
        public static Boolean isAutoRun = false;  // 抓取线程是不是已经在运行>////

        //static int timetmp = 5;

        static Cappic()
        {
            String st = Apis.getValueByStrName(StrCapPicNum);
            if (st != "")
                Idnow = st;
            st = Apis.getValueByStrName(StrCapPicTime);
            if (st != "")
            {
                int tmp = 0;
                if (int.TryParse(st, out tmp))
                    iCapTime = tmp;
                else
                    Apis.setValueByStrName(StrCapPicTime, iCapTime + "");

            }
            else
            {
                Apis.setValueByStrName(StrCapPicTime, iCapTime + "");
            }

        }


        public static void capicture() 
        {
            capicture(false);
        }

        private static void capicture(bool isrun)
        {
            SqlConnection cn = null;
            SqlDataReader dr1 = null;  //只能用于读，要更新需要再开一个连接
            iGotFileNum = 0;
            IReConnetTime = 497;

            try
            {
                UseStatic.soutTdRtb2("开始抓介绍图片，保存在 " + saveUrl + " 下! "+ DateTime.Now.ToString() + " 每隔12小时重复抓取！");

                if (!Directory.Exists(saveUrl))
                {
                    Directory.CreateDirectory(saveUrl);
                    UseStatic.soutTdRtb2("creat " + saveUrl + " directory");
                }

                if (cn == null || cn.State == ConnectionState.Closed || cn.State == ConnectionState.Broken)
                    cn = Apis.GetCon(Apis.sDBUrl206);
                SqlCommand msc = cn.CreateCommand();
                msc.CommandText = strCapPic + Idnow + " order by id desc";
                dr1 = msc.ExecuteReader();

                while (dr1 != null)
                {
                    dr1 = tryfReconnect(dr1, cn, msc);
                }
                Apis.setValueByStrName(StrCapPicNum, iLastUpdate-77 + "");//每次更新上次更新后-77张图片开始
                UseStatic.soutTdRtb2("所有图片已抓取! " +DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                UseStatic.soutTdRtb2(ex);
            }
            finally 
            {
                WindForm.isCappic = false;
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
                WindForm.oneCatchEnd("isCappic");
            }
            if (isAllwayscap)//一旦开始抓取就不能停下来，要停就关闭程序 
            {
                if ((!isAutoRun) || isrun) //auto run 已经运行 ，无需再次运行
                {
                    isAutoRun = true;
                    Thread.Sleep(3600000 * iCapTime);//1000*60*60*12 = 60000*720
                    WindForm.isCappic = true;
                    IReConnetTime = 497;
                    capicture(true);
                }
            }
        }

        //重连IReConnetTime 次数，成功返回dr1, 出错返回null，
        private static SqlDataReader tryfReconnect(SqlDataReader dr1, SqlConnection cn, SqlCommand msc) 
        {
            try
            {
                while (dr1.Read())
                {
                    if (WindForm.isCappic == false)
                    {
                        if (!isAllwayscap)
                        {
                            UseStatic.soutTdRtb2("pic抓取已被人工终止!");
                            return null;
                        }
                    }
                    String id = dr1["id"].ToString().Trim();
                    String url = dr1["imageurl"].ToString().Trim();
                    downloadfile(url, id);
                    int idn = 0;
                    if (int.TryParse(id, out idn))
                    {
                        if (idn > iLastUpdate) iLastUpdate = idn;
                    }
                    //timetmp--;
                    //if (timetmp == 0) return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                UseStatic.soutTdRtb2(ex);
                if ( cn.State == ConnectionState.Closed || cn.State == ConnectionState.Broken)
                {
                    IReConnetTime--;
                    if (IReConnetTime == 0) return null;
                    UseStatic.soutTdRtb2("重新建立连接！ （︶︿︶）╭∩╮鄙视这网络速度");
                    cn.Open();
                    msc = cn.CreateCommand();
                    msc.CommandText = strCapPic + Idnow;
                    dr1 = msc.ExecuteReader();
                    return dr1;
                }
                return null; //其余出错原因
            }
        }

        private static void downloadfile(String url,String id)
        {
            if (url == null || url == "") return;
            FileStream writer = null;
            Stream reader = null;
            try
            {
                String sfile = saveUrl + "\\" + id + ".jpg";
                if (File.Exists(sfile))
                {
                    UseStatic.soutTdRtb2("exist file! id= " + id);
                }
                else
                {
                    iGotFileNum++;
                    if (iGotFileNum == 120)
                    {
                        iGotFileNum = 0;
                        Thread.Sleep(10000);//休息，时间太快，数据库会拒绝连接
                    }

                    WebRequest request = WebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    reader = response.GetResponseStream();
                    writer = new FileStream(sfile, FileMode.OpenOrCreate, FileAccess.Write);
                    byte[] buff = new byte[512];
                    int c = 0; //实际读取的字节数
                    while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                    {
                        writer.Write(buff, 0, c);
                    }
                    Idnow = id;
                    UseStatic.soutTdRtb2("got file! id= " + id);
                }
            }
            catch (Exception ex)
            {
                UseStatic.soutTdRtb2(ex);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
        }

        //private String UpfileImgInfoByUrl(String Url)
        //{
        //    String PageCode = "", regex = "", resultUrl = "", imgUrl = "", saveUrl = "";
        //    try
        //    {

        //        PageCode = Apis.GetPageCode(Url, "GB2312");

        //        if (PageCode.Length > 0)
        //        {
        //            regex = "imgname[[0-9]{1,2}]=\"([^\"]*)\";";

        //            Match match = Apitool.GetResultOfReg(PageCode, regex);

        //            if (match.Success)
        //            {
        //                while (match.Success)
        //                {
        //                    imgUrl = "http://img1.ddmapimg.com/poi/small/" + match.Groups[1].Value.ToString();

        //                    saveUrl = "http://www.qjt123.com/UploadFile/VenueImg/";

        //                    // resultUrl = resultUrl +apis.UpLoadFile(imgUrl,saveUrl,true);
        //                    // resultUrl = resultUrl + apis.UpFileImgByimgUrlandSaveUrl(imgUrl,saveUrl);

        //                    WebRequest request = WebRequest.Create(imgUrl);
        //                    WebClient client = new WebClient();
        //                    System.Random a = new Random(System.DateTime.Now.Millisecond);
        //                    int RandKey = a.Next(1000); //随机数
        //                    saveUrl = System.Environment.CurrentDirectory + "//photo//" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + RandKey + ".jpg";


        //                    if (Directory.Exists((System.Environment.CurrentDirectory + "//photo")))
        //                    {
        //                    }
        //                    else
        //                    {
        //                        Directory.CreateDirectory((System.Environment.CurrentDirectory + "//photo")); //创建文件

        //                    }

        //                    resultUrl = resultUrl + "/VenueImg/" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + RandKey + ".jpg|";

        //                    client.DownloadFile(imgUrl, saveUrl);

        //                    match = match.NextMatch();

        //                }
        //            }
        //            else
        //            {
        //                UseStatic.soutTdRtb2(U rl + "图片匹配失败!\r\n");
        //            }

        //            resultUrl = resultUrl.Replace("null", "").Replace(" ", "");

        //            if (resultUrl.Length == 0 || resultUrl == "")
        //            {
        //                resultUrl = "npic.gif";
        //            }
        //        }
        //        else
        //        {
        //            UseStatic.soutTdRtb2(Url + "网址打不开/r/n");
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return resultUrl;
        //}


    }
}//end capvideo
