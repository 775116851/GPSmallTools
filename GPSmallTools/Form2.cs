using HtmlAgilityPack;
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
using System.Windows.Forms.DataVisualization.Charting;

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
            if(jo.Count > 0)
            {
                string hgtList = jo["data"][0].ToString().Trim('"');
                string ggtList = jo["data"][1].ToString().Trim('"');

            }
            //沪港通资金流向
            //http://data.eastmoney.com/bkzj/graph/hgt_1.html
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            DrawM();
            int height = 500;
            int width = 700;
            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            Font font = new System.Drawing.Font("宋体",30f);
            Brush brush = Brushes.Red;
            Pen myPen = new Pen(brush, 1);
            //g.FillClosedCurve(Brushes.WhiteSmoke,0,0,);

            //pictureBox1.Image = image;

            return;
            //ChartArea1属性设置
            //设置网格的颜色
            chartHGT.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineColor = Color.LightGray;
            chartHGT.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.LightGray;
            //设置坐标轴名称
            chartHGT.ChartAreas["ChartArea1"].AxisX.Title = "随机数";
            chartHGT.ChartAreas["ChartArea1"].AxisY.Title = "数值";
            //启用3D显示
            chartHGT.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;

            //Series属性设置
            //设置显示类型-线型
            chartHGT.Series["随机数"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            //设置坐标轴Value显示类型
            //chartHGT.Series["随机数"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            //是否显示标签的数值
            //chartHGT.Series["随机数"].IsValueShownAsLabel = true;
            //设置标记图案
            chartHGT.Series["随机数"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
            //设置图案颜色
            chartHGT.Series["随机数"].Color = Color.Green;
            //设置图案的宽度
            chartHGT.Series["随机数"].BorderWidth = 1;


            ////添加随机数
            //Random rd = new Random();
            //for (int i = 1; i < 20; i++)
            //{
            //    int m = rd.Next(10000);
            //    //chartHGT.Series["随机数"].Points.AddXY(DateTime.Now.Minute + i, rd.Next(10000));
            //    chartHGT.Series["随机数"].Points.AddY(m);
            //    chartHGT.Series["随机数"].Points[i-1].AxisLabel = i.ToString();
            //    chartHGT.Series["随机数"].Points[i - 1].Label = m.ToString();
            //    chartHGT.Series["随机数"].Points[i - 1].ToolTip = m.ToString();

            //}

            List<HGT> listHGT = new List<HGT>();
            string pageContent = ShowHttpWebRequest("http://data.eastmoney.com/bkzj/graph/hgt_1.html");
            if (!string.IsNullOrEmpty(pageContent))
            {
                string[] pageCList = pageContent.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < pageCList.Length; i++)
                {
                    HGT hgt = new HGT();
                    string[] hgtS = pageCList[i].Split(new char[] { ';' });
                    double hV = 0;
                    double gV = 0;
                    hgt.Data = hgtS[0];
                    if(!string.IsNullOrEmpty(hgtS[1]))
                    {
                        hV = Convert.ToDouble(hgtS[1]);
                        hgt.HValue = hgtS[1];
                    }
                    if (!string.IsNullOrEmpty(hgtS[2]))
                    {
                        gV = Convert.ToDouble(hgtS[2]);
                        hgt.GValue = hgtS[2];
                    }
                    listHGT.Add(hgt);
                    chartHGT.Series["随机数"].Points.AddY(hV);
                    //if(i + 10 >= pageCList.Length)
                    //{
                    chartHGT.Series["随机数"].Points[i].AxisLabel = hgtS[0];
                    //chartHGT.Series["随机数"].Points[i].Label = hV.ToString();
                    //chartHGT.Series["随机数"].Points[i].ToolTip = hV.ToString();
                    chartHGT.Series["随机数"].Points[i].LabelBackColor = Color.Red;
                    //}
                }
            }
            //chartHGT.Series["随机数"].Sort(PointSortOrder.Ascending);
        }

        public void DrawM()
        {
            Bitmap objBitmap = new Bitmap(400, 300);
            Graphics objGraphics = Graphics.FromImage(objBitmap);
            objGraphics.Clear(Color.White);
            objGraphics.DrawRectangle(Pens.Black, 1, 1, 398, 298);
            objGraphics.DrawString("XXV", new Font("宋体", 16, FontStyle.Bold), Brushes.Black, new PointF(60, 5));

            objGraphics.DrawLine(new Pen(Color.Blue, 1),10,2,10,320);
            objGraphics.DrawLine(new Pen(Color.Blue, 1), 10, 150, 400, 150);
            objGraphics.DrawString("0",new Font("宋体",12,FontStyle.Regular),Brushes.Red,new PointF(-1,142));
            //Y轴坐标
            for (int i = 0; i < 6; i++ )
            {
                objGraphics.DrawLine(new Pen(Color.Blue, 1), 10, i * 50 + 1, 20, i * 50 + 1);
                objGraphics.DrawString((300 - i * 50).ToString(), new Font("宋体", 12, FontStyle.Regular), Brushes.Red, new PointF(12, i * 50 -1));
            }
            //X轴坐标
            for (int i = 0; i < 9; i++)
            {
                objGraphics.DrawLine(new Pen(Color.Blue, 1), i * 50 + 1, 150, i * 50 + 1, 160);
                objGraphics.DrawString((400 - i * 50).ToString(), new Font("宋体", 12, FontStyle.Regular), Brushes.Red, new PointF(i * 50 + 1, 160));
            }
            //objGraphics.DrawLine(new Pen(Color.Blue, 1), 10, 2, 10, 320);

            //objGraphics.DrawLine(new Pen(Color.Blue, 1), 10, 270, 200, 270);          

            pictureBox1.Image = objBitmap;
        }

        private string[,] data = new string[6, 2];                
        private void DrawingAPic()       
        {                       int i;            // 实例化Bitmap对象             
            Bitmap objbitmap;            
            objbitmap = new Bitmap(400, 300);            
            Graphics objGraphics;            
            // 实例化Graphics类             
            objGraphics = Graphics.FromImage(objbitmap);          
            // 填充背景色                         
            objGraphics.Clear(Color.White);            
            // 画外围矩形            
            objGraphics.DrawRectangle(Pens.Black, 1, 1, 398, 298);          
            // 写标题           
            objGraphics.DrawString("本公司上半年营业额统计图", new Font("宋体", 16, 
FontStyle.Bold), Brushes.Black, new PointF(60, 5));     
            // 获取数据，这里模拟出6个月的公司业务数据，实际应用可以从数据库读取            
            getdata();           
            PointF monthcolor = new PointF(260, 40);            
            PointF fontinfor = new PointF(285, 40);            
            for (i = 0; i <= 5; i++)             
            {   //  画出填充矩形               
                objGraphics.FillRectangle(new SolidBrush(getcolor(i)), monthcolor.X, 
monthcolor.Y, 20, 10);   
                //画出矩形边框。               
                objGraphics.DrawRectangle(Pens.Black, monthcolor.X, monthcolor.Y, 20, 
10);           
                //画出图例说明文字－－data(i, 0)                 
                objGraphics.DrawString(data[i, 0], new Font("宋体", 10), 
Brushes.Black, fontinfor);              
                //移动坐标位置，只移动Y方向的值即可。               
                monthcolor.Y += 15;   
                fontinfor.Y += 15;            
            }         
            // 遍历数据源的每一项数据，并根据数据的大小画出矩形图（即柱形图的柱）。        
            for (i = 0; i <= 5; i++)           
            {               
                //画出填充矩形。                
                objGraphics.FillRectangle(new SolidBrush(getcolor(i)), (i * 25) + 35, 
270 - System.Convert.ToInt32(data[i, 1]), 15, System.Convert.ToInt32(data[i, 1]));     
                //'画出矩形边框线。               
                objGraphics.DrawRectangle(Pens.Black, (i * 25) + 35, 270 - 
System.Convert.ToInt32(data[i, 1]), 15, System.Convert.ToInt32(data[i, 1]));            
            }            
            //画出示意坐标           
            objGraphics.DrawLine(new Pen(Color.Blue, 1), 10, 2, 10, 320);           
            objGraphics.DrawLine(new Pen(Color.Blue, 1), 10, 270, 200, 270);          
            // 在示意坐标上添加数值标志，注意坐标的计算           
            for (i = 0; i <= 5; i++)            
            {             
                objGraphics.DrawLine(new Pen(Color.Blue, 1), 10, i * 50 + 20, 20, i * 
50 + 20);              
                objGraphics.DrawString((250 - i * 50).ToString(), new Font("宋体", 10), 
Brushes.Black, 12, i * 50 + 8);           
            }          
            //统计总销售额         
            float scount = 0;           
            for (i = 0; i <= 5; i++)           
            {               
                scount += float.Parse((data[i, 1]));          
            }           
            //定义画出扇形角度变量       
            float scg = 0;        
            float stg = 0;          
            for (i = 0; i <= 5; i++)    
            {     
                //计算当前角度值：当月销售额 / 总销售额 * 360，得到饼图中当月所占的角度大小。      
                float num = float.Parse(data[i, 1]);          
                scg = (num / scount) * 360;              
                //画出填充圆弧。              
                objGraphics.FillPie(new SolidBrush(getcolor(i)), 220, 150, 120, 120, 
stg, scg);     
                //画出圆弧线。         
                objGraphics.DrawPie(Pens.Black, 220, 150, 120, 120, stg, scg);         
                //  把当前圆弧角度加到总角度上。              
                stg += scg;           
            }          
            // 画出说明文字  
            objGraphics.DrawString("柱状图", new Font("宋体", 15, FontStyle.Bold), 
Brushes.Blue, 50, 272);        
            objGraphics.DrawString("饼状图", new Font("宋体", 15, FontStyle.Bold), 
Brushes.Blue, 250, 272);      
             
            // 输出到客户端     
            //objbitmap.Save(System.Web.HttpContext.Current.Response.OutputStream,System.Drawing.Imaging.ImageFormat.Gif);        
            pictureBox1.Image = objbitmap;
        }      
    // 为数组赋值    
    // 即生成模拟业务数据     
        private void getdata()     
        {          
            data[0, 0] = "一月份";      
            data[1, 0] = "二月份";     
            data[2, 0] = "三月份";       
            data[3, 0] = "四月份";     
            data[4, 0] = "五月份";     
            data[5, 0] = "六月份";    
            data[0, 1] = "85";     
            data[1, 1] = "135";   
            data[2, 1] = "85";       
            data[3, 1] = "110";       
            data[4, 1] = "130";      
            data[5, 1] = "200";     
        }       
        // 产生色彩值，便于显示区别     
        private Color getcolor(int i)  
        {            Color newcolor;  
            i += 1;          
            if (i == 1)          
            {            
                newcolor = Color.Blue;    
            }           
            else if (i == 2)   
            {             
                newcolor = Color.ForestGreen;   
            }       
            else if (i == 3)   
            {            
                newcolor = Color.Gainsboro;    
            }       
            else if (i == 4)  
            {         
                newcolor = Color.Moccasin; 
            }          
            else if (i == 5)     
            {            
                newcolor = Color.Indigo; 
            }      
            else if (i == 6)      
            {             
                newcolor = Color.BurlyWood;    
            }          
            else     
                newcolor = Color.Goldenrod; 
            return newcolor;  
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ServicePointManager.DefaultConnectionLimit = 50;
            //WebClient wc = new WebClient();
            string sUrl = "http://www.haha365.com/joke/index_{0}.htm";
            string mUrl = "";
            //for (int i = 1; i < 100000;i++ )
            //{
            //    mUrl = string.Format(sUrl, i);
            //    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(mUrl);
            //    myRequest.Method = "HEAD";　              //设置提交方式可以为＂get＂，＂head＂等
            //    myRequest.Timeout = 10000;　             //设置网页响应时间长度
            //    myRequest.AllowAutoRedirect = false;//是否允许自动重定向
            //    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            //    if (myResponse.StatusCode == HttpStatusCode.OK)//返回响应的状态
            //    {
            //        WebClient wc = new WebClient();
            //        wc.DownloadFile(mUrl, @"C:\中文幽默笑话\" + System.IO.Path.GetFileName(mUrl));
            //        System.Net.Cache.RequestCachePolicy policy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            //        wc.CachePolicy = policy;
            //        wc.Dispose();
            //    }
            //}
            FileStream fsMode = null;
            for (int i = 1; i < 100000; i++)
            {
                mUrl = string.Format(sUrl, i);
                //string mContent = ShowWebRequest(mUrl);
                HtmlWeb htmlWeb = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument htmlDoc = htmlWeb.Load(mUrl);
                HtmlNode htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='r_c']");
                string mContent = htmlNode.InnerText;
                if(string.IsNullOrEmpty(mContent) || !mContent.Contains("class=\"cat_llb\""))
                {
                    MessageBox.Show("完成：" + i);
                    return;
                }
                try
                {
                    byte[] fMode = Encoding.UTF8.GetBytes(mContent);
                    fsMode = new FileStream(@"C:\中文幽默笑话\" + System.IO.Path.GetFileName(mUrl), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fsMode.Write(fMode, 0, fMode.Length);
                    fsMode.Close();
                    fsMode.Dispose();
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (fsMode != null)
                    {
                        fsMode.Dispose();
                    }
                }
            }

            MessageBox.Show("完成：OK");
        }  
  
        
    }

    public class HGT
    {
        public string Data { get; set; }
        public string HValue { get; set; }
        public string GValue { get; set; }
    }
}
