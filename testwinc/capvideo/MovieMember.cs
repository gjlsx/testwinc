using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace testwinc.capvideo
{
    /// <summary>
    /// movie表成员类
    /// </summary>
    class MovieMember
    {
        public String sid = "", sTitle = "", sType = "", slang = "", director = "", presenter = "", sContent = "", sUrl = ""
            , simageUrl = "",syear = "",sPlayTime = "",time = "0";
        //tvnum = 0 -电影 1-电视剧 2-动漫 3-央视名栏目,4,综艺 ,5,音乐mtv ,999-其他.   zjs : 若是电视剧 这个是总集数 jsnew：更新至
        public int forenoticid = 0,tvnum = 0,zjs = 0,jsnew = 0;

        /// <summary>
        /// 去掉数据库中不允许或不要的字符
        /// </summary>
        public void shtmlString() 
        {

            sTitle = sTitle.Replace("'", "").Replace("[", "(").Replace("]", ")").Trim();
            sType = sType.Replace("'", "").Replace("[", "(").Replace("]", ")").Trim();
            slang = slang.Replace("'", "").Trim();
            director = director.Replace("'", "").Replace("[", "(").Replace("]", ")").Trim();
            presenter = presenter.Replace("'", "").Trim();
            if (presenter.Length > 447) presenter = presenter.Substring(0, 447);

            sContent = sContent.Replace("'", "").Trim();
            sUrl = sUrl.Replace("'", "").Trim();
            simageUrl = simageUrl.Replace("'", "").Trim();
            syear = syear.Replace("'", "").Trim();
        }
    }
}

