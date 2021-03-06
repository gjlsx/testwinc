﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using testwinc;
using testwinc.tools;
using System.Xml;
using System.Collections;

namespace testwinc.test
{
    class TestXML
    {
            public void testXML(int filecopytime)
            {
                string pathnow = System.Environment.CurrentDirectory;
                pathnow = pathnow + @"\test\" + "testy.xml";
                UseStatic.soutTd(pathnow);

                try
                {
                    XmlDocument xdom = new XmlDocument();
                    xdom.Load(pathnow);

                    XmlNodeList NDEvent = xdom.GetElementsByTagName("student");

                    StringBuilder sb = new StringBuilder();
                    String strget = "";
                        for (int i = 0; i < NDEvent.Count; i++)
                        {
                            XmlElement xe = (XmlElement)NDEvent.Item(i);
                            string st = xe.GetAttribute("name");
                            sb.Append(st);
                        }
                        strget = sb.ToString();
                        UseStatic.soutTd(strget);
                    }
                catch (Exception ei)
                {
                    UseStatic.soutTd(ei.ToString());
                }

            }//end testXML


      //读取Xml文档
      public void readXML(String path) 
      {
          string pathnow = "";
          if (path == "")
          {
              pathnow = System.Environment.CurrentDirectory + @"\test\" + "testy.xml";
          }
          else
          {
              pathnow = path;
          }
            XmlDocument doc = new XmlDocument();
            doc.Load(pathnow);
 
            //使用xpath表达式选择文档中所有的student子节点
            XmlNodeList nodeList = doc.SelectNodes("school/students/student");
            getInfoxml(nodeList);
            nodeList = doc.GetElementsByTagName("teacher");
            getInfoxml(nodeList);
        }


        //读取Xml文档
        public void readXML2(String path)
        {
            string pathnow = "";
            if (path == "")
            {
                pathnow = System.Environment.CurrentDirectory + @"\test\" + "kuntaisport.xml";
            }
            else
            {
                pathnow = path;
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(pathnow);

            string statu = getNodeByName(doc, "status");
            if (statu == null)
                return;
            if (statu.ToUpper().Equals("N"))
                return;

            if (statu.ToUpper().Equals("Y"))
            {
                //使用xpath表达式选择文档中所有的data子节点
                XmlNodeList nodeList = doc.SelectNodes("site/Result/data/data/data");
               // XmlNodeList nodeList4 = doc.SelectNodes("Result/data/data/data");
                List<string> lstr = new List<string>{ "id", "bottomPrice" };
                List<Hashtable> ltbl =  getValuexml(nodeList, lstr);
                if(ltbl != null)
                {
                    foreach (Hashtable tbl in ltbl)
                    {
                        string value0 = tbl[lstr[0]]+"";
                        string value1 = tbl[lstr[1]] + "";
                        UseStatic.soutTd("id is: " + value0);
                        UseStatic.soutTd("bottomPrice is: " + value1);
                    }
                }
            }
        }


        //根据唯一标签，获得xml文档中标签对应第一个值
        public string getNodeByName(XmlDocument doc, string sname)
        {
            if (sname == null || sname.Trim().Equals("") || doc == null)
                return null;
            sname = sname.Trim();
            XmlNodeList nodeList = doc.GetElementsByTagName(sname);
            if (nodeList == null)
                return null;
            foreach (XmlNode studentNode in nodeList)
            {
                string value = studentNode.InnerText.Trim();
                return value;
                //UseStatic.soutTd("\t");
            }
            return null;
        }

        /// <summary>
        /// 获得参数值列表
        /// </summary>
        /// <param name="nl">XmlNodeList</param>
        /// <param name="keys">List keys</param>
        /// <returns> List<Hashtable></returns>
        public List<Hashtable> getValuexml(XmlNodeList nl,List<string> keys)
        {
            if (nl == null || keys == null)
                return null; 

                List<Hashtable> listhash = new List<Hashtable>() ;

                foreach (XmlNode snode in nl)
                {
                    Hashtable ht = null;
                    //通过SelectSingleNode方法获得当前节点下的courses子节点
                    //XmlNode nodeid = snode.SelectSingleNode("id");
                    //XmlNode nodeprice = snode.SelectSingleNode("bottomPrice");
                    /**
                    if (nodeid != null)
                    {
                        string stpid = nodeid.InnerText.Trim();
                        if (ht == null)
                            ht = new Hashtable();
                        ht.Add("id", stpid);
                    }
                **/
                    if (keys != null)
                    { 
                        foreach(string key in keys)
                        {
                            XmlNode nodetmp = snode.SelectSingleNode(key);
                            if (nodetmp != null)
                            {
                                string stmp2 = nodetmp.InnerText.Trim();
                                if (ht == null)
                                    ht = new Hashtable();
                                ht.Add(key, stmp2);
                            }
                        }
                    }

                    if (ht != null)
                    {
                        listhash.Add(ht);
                    }

                    /**
                    //通过ChildNodes属性获得courseNode的所有一级子节点
                    XmlNodeList courseNodeList = coursesNode.ChildNodes;
                    if (courseNodeList != null)
                    {
                        foreach (XmlNode courseNode in courseNodeList)
                        {
                            //UseStatic.soutTd("科目名: " + courseNode.Attributes["name"].Value);
                            //通过FirstNode属性可以获得课程节点的第一个子节点，LastNode可以获得最后一个子节点
                            //读取CData节点
                            //XmlCDataSection cdata = (XmlCDataSection)courseNode.FirstChild;

                            XmlNodeList cns = courseNode.ChildNodes;
                            foreach (XmlNode tcnode in cns)
                            {
                               // UseStatic.soutTd("群众评语: " + tcnode.InnerText.Trim());
                            }
                        }
                    }
                    **/


                }
            return listhash;
        }

        private void getInfoxml(XmlNodeList nl) 
      {
          if (nl != null)
          {
              foreach (XmlNode studentNode in nl)
              {
                  //通过Attributes获得属性名字为name的属性
                  string name = studentNode.Attributes["name"].Value;
                  string sex = studentNode.Attributes["sex"].Value;
                  UseStatic.soutTd("Student: " + name + " sex: " + sex);

                  //通过SelectSingleNode方法获得当前节点下的courses子节点
                  XmlNode coursesNode = studentNode.SelectSingleNode("courses");

                  //通过ChildNodes属性获得courseNode的所有一级子节点
                  XmlNodeList courseNodeList = coursesNode.ChildNodes;
                  if (courseNodeList != null)
                  {
                      foreach (XmlNode courseNode in courseNodeList)
                      {
                          UseStatic.soutTd("科目名: " + courseNode.Attributes["name"].Value);
                          //通过FirstNode属性可以获得课程节点的第一个子节点，LastNode可以获得最后一个子节点
                              //读取CData节点
                          //XmlCDataSection cdata = (XmlCDataSection)courseNode.FirstChild;

                          XmlNodeList cns = courseNode.ChildNodes;
                          foreach (XmlNode tcnode in cns)
                          {
                              UseStatic.soutTd("群众评语: " + tcnode.InnerText.Trim());
                          }
                      }
                  }
                  UseStatic.soutTd("\t");
              }

          }
      }

       public void writeXML(String path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建Xml声明部分，即<?xml version="1.0" encoding="utf-8" ?>
             XmlDeclaration xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
 
            //创建根节点
            XmlNode rNode = xmlDoc.CreateElement("school");
            XmlElement rootNode = xmlDoc.CreateElement("students");
            XmlNode tenode = xmlDoc.CreateElement("teachers");
 
            //创建student子节点
            List<String> ls = new List<String>(); ; 
            ls.Add("她现在是位英语老师. 得分80");
            ls.Add("外号： 阿管");
            XmlNode studentNode = writeXMLInfo(xmlDoc, "student", "管同学", "女", "英语", ls);
            rootNode.AppendChild(studentNode);
            ls.Clear();
            ls.Add("他现在是管同学的那个他. 得分 61！");
            rootNode.AppendChild(writeXMLInfo(xmlDoc, "student", "郭同学", "男","电子", ls ));
            ls.Clear();
            ls.Add("她现在是郭同学的那个她. 得分47.");
            rootNode.AppendChild(writeXMLInfo(xmlDoc, "student", "马同学", "女", "通信", ls));
            ls.Clear();
            ls.Add("她的那个他是郭同学. 得分 54");
            ls.Add("她需要变的更温柔些");
            rootNode.AppendChild(writeXMLInfo(xmlDoc, "student", "胡同学", "女", "市场", ls));

            ls.Clear();
            ls.Add("她以前有点笨，但是比较可爱. 得分 80+");
            tenode.AppendChild(writeXMLInfo(xmlDoc, "teacher", "管老师","女", "英语", ls));

            ls.Clear();
            ls.Add("他不但笨，而且犟，好在有大家帮助他. 得分 60+");
            tenode.AppendChild(writeXMLInfo(xmlDoc, "teacher", "郭老师","男", "DSP", ls));
            ls.Clear();
            ls.Add("她以前有点聪明，但是不可爱. 得分 50-");
            tenode.AppendChild(writeXMLInfo(xmlDoc, "teacher", "马老师","女", "通信", ls));

         
            rNode.AppendChild(rootNode);
            rNode.AppendChild(tenode);
            //附加根节点
            xmlDoc.AppendChild(rNode);
            xmlDoc.InsertBefore(xmldecl, rNode);
 
            //保存Xml文档
           string pathnow = "";
           if (path == "")
           {
               pathnow = System.Environment.CurrentDirectory + @"\test\" + "testy.xml";
           }
           else
           {
               pathnow = path;
           }

            try
            {
                //如果文件不存在则创建该文件  
                if (!File.Exists(pathnow))
                {
                    //创建一个File对象  
                    FileStream fs = File.Create(pathnow);
                    //关闭文件流  
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                UseStatic.soutTd("文件不存在： " + pathnow);
                UseStatic.soutTd("error： " + ex.ToString());
                return;
            }

            xmlDoc.Save(pathnow);
            UseStatic.soutTd("数据已保存在: " + pathnow);
 
        }

          
       private XmlNode writeXMLInfo(XmlDocument xmlDoc,String nodeName, String name, String sex,String courseName, List<String> Data)
       {

           //创建student子节点
           XmlElement node = xmlDoc.CreateElement(nodeName);

           //创建一个属性
           XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
           nameAttribute.Value = name;
           //xml节点附件属性
           node.Attributes.Append(nameAttribute);

           nameAttribute = xmlDoc.CreateAttribute("sex");
           nameAttribute.Value = sex;
           node.Attributes.Append(nameAttribute);


           //创建courses子节点
           XmlNode coursesNode = xmlDoc.CreateElement("courses");
           XmlNode courseNode1 = xmlDoc.CreateElement("course");
           XmlAttribute courseNameAttr = xmlDoc.CreateAttribute("name");
           courseNameAttr.Value = courseName;
           courseNode1.Attributes.Append(courseNameAttr);

           foreach (String sData in Data)
           {
           XmlElement teacherCommentNode = xmlDoc.CreateElement("Comment");
           //创建Cdata块
           //XmlCDataSection cdata = xmlDoc.CreateCDataSection(Data);
           //teacherCommentNode.AppendChild(cdata);
           teacherCommentNode.InnerText = sData;
           courseNode1.AppendChild(teacherCommentNode);
           }

           coursesNode.AppendChild(courseNode1);
           //附加子节点
           node.AppendChild(coursesNode);

           return node;
       }

       public static XmlElement bxdata(XmlDocument xmlDoc, String name, String cdata)
       {
           XmlElement tenode = xmlDoc.CreateElement(name.Trim());
           XmlCDataSection cda = xmlDoc.CreateCDataSection(cdata.Trim());
           tenode.AppendChild(cda);
           return tenode;
       }

       private String[] getStrbySplit(String str)
       {
           if (str == "") return new String[] { "" };
           str = str.Replace("<BR>", "/");
           char[] split = { ',', '/' };
           return str.Split(split);
       }

    }//end TestXML
}//end namespace
