using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GPSmallTools
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private string ShowWebClient(string url)
        {
            string strHtml = string.Empty;
            WebClient wc = new WebClient();
            Stream myStream = wc.OpenRead(url);
            StreamReader sr = new StreamReader(myStream, Encoding.Default);
            strHtml = sr.ReadToEnd();
            myStream.Close();
            return strHtml;
        }

        private string ShowWebRequest(string url)
        {
            Uri uri = new Uri(url);
            WebRequest myReq = WebRequest.Create(uri);
            WebResponse result = myReq.GetResponse();
            Stream receviceStream = result.GetResponseStream();
            StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("gbk"));
            string strHTML = readerOfStream.ReadToEnd();
            readerOfStream.Close();
            receviceStream.Close();
            result.Close();
            return strHTML;
        }

        private string ShowHttpWebRequest(string url)
        {
            Uri uri = new Uri(url);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(uri);
            myReq.UserAgent = "User-Agent:Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705";
            myReq.Accept = "*/*";
            myReq.KeepAlive = true;
            myReq.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            HttpWebResponse result = (HttpWebResponse)myReq.GetResponse();
            Stream receviceStream = result.GetResponseStream();
            StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("gbk"));
            string strHTML = readerOfStream.ReadToEnd();
            readerOfStream.Close();
            receviceStream.Close();
            result.Close();
            return strHTML;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pageContent = ShowHttpWebRequest("http://data.eastmoney.com/bkzj/hgt.html");
            int st = pageContent.IndexOf("defset1({");
            int en = pageContent.IndexOf("})", st);
            string sMessage = pageContent.Substring(st + 8, en - st - 7);
            JObject jo = (JObject)JsonConvert.DeserializeObject(sMessage);
            //var wc = new WebClient();
            //var html = wc.DownloadString("http://zhidao.baidu.com/question/499087825.html?seed=0");
            //var regex = new Regex("<span class=\"question-title\" data-accusearea=\"qTitle\">(?<title>.*?)</span>");
            //if (regex.IsMatch(html))
            //{
            //    var title = regex.Match(html).Groups["title"].Value;
            //    Console.Write(title);
            //}
        }


        //var wc = new WebClient();
        //    var html = wc.DownloadString("http://zhidao.baidu.com/question/499087825.html?seed=0");
        //    var regex = new Regex("<span class=\"question-title\" data-accusearea=\"qTitle\">(?<title>.*?)</span>");
        //    if (regex.IsMatch(html))
        //    {
        //        var title = regex.Match(html).Groups["title"].Value;
        //        Console.Write(title);
        //    }
        //    Console.Read();
    }
}
