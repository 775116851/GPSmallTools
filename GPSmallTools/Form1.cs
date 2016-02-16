using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using log4net;
using System.Configuration;
using System.Threading;
using System.Xml;
using System.Web.Caching;

namespace GPSmallTools
{
    public enum ErrorTitleType : int
    {
        [Description("提醒")]
        GJMsg = 0,
        [Description("消息提醒")]
        PTMsg = 1,
        [Description("错误提醒")]
        CWMsg = 2
    }
    public partial class Form1 : Form
    {
        //https://www.showapi.com/api/lookPoint/131/45
        //http://www.21andy.com/new/20090530/1313.html
        //https://api.wmcloud.com/docs/pages/viewpage.action?pageId=1867781
        //http://tushare.org/datayes.html#id2
        //sz000789,sh600030,sz002673,sh601872,sh601727,sh600067,sz000709

        //http://www.crifan.com/files/doc/docbook/web_scrape_emulate_login/release/html/web_scrape_emulate_login.html  网站数据抓取
        public Form1()
        {
            InitializeComponent();
        }
        private ILog log = log4net.LogManager.GetLogger(typeof(Form1));
        public static Queue<ErrorMsg> queueMsgList = new Queue<ErrorMsg>();//股价提醒
        public static Queue<ErrorMsg> queueTXMsgList = new Queue<ErrorMsg>();//禁止股价提醒
        public static object locker = new object();
        private bool BCanExec = true;
        private bool BIsTX = true;//提醒
        private string BTXTime = "1";//提醒间隔时间
        private void Form1_Load(object sender, EventArgs e)
        {
            //Image pic = Image.FromStream(WebRequest.Create("http://hqgnqhpic.eastmoney.com/EM_Futures2010PictureProducter/Picture/IF16021RS.png?dt=1454222469738").GetResponse().GetResponseStream());
            //pictureBox1.Image = pic;
            //log.Info("呵呵");
            BindConfig();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            Thread tShow = new Thread(ShowTT);
            tShow.IsBackground = true;
            tShow.Start();

            Show(1);

            //GetAPIDataPushGPYJ();

            //消息队列
            Thread tMsg = new Thread(OutMsg);
            tMsg.Start();
        }

        //消息提醒
        private void OutMsg()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                while (true)
                {
                    try
                    {
                        if (queueMsgList.Count > 0)
                        {
                            //从队列中拿出数据
                            ErrorMsg eMsg = queueMsgList.Dequeue();
                            this.notifyIconMsg.ShowBalloonTip(eMsg.ShowTime, GetDescription(typeof(ErrorTitleType),Convert.ToInt32(eMsg.TitleType)), eMsg.Content, eMsg.ToolTripType);
                            Thread.Sleep(eMsg.ShowTime);
                        }
                        else
                        {
                            Thread.Sleep(30);//避免了CPU空转。
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorMsg eMsg = new ErrorMsg();
                        eMsg.TitleType = (int)ErrorTitleType.CWMsg;
                        eMsg.Content = ex.Message;
                        eMsg.CreateTime = DateTime.Now;
                        eMsg.ShowTime = 4000;
                        eMsg.ToolTripType = ToolTipIcon.Error;
                        queueMsgList.Enqueue(eMsg);
                        log.Error(string.Format("读取队列数据异常,异常信息:{0} ;异常详情:{1}", ex.Message, ex));
                    }
                }
            });
        }

        //基础数据绑定
        private void BindConfig()
        {
            timer1.Enabled = true;
            //置顶
            string sTopMost = Convert.ToString(ConfigurationManager.AppSettings["GPTopMost"]);
            if (string.IsNullOrEmpty(sTopMost) || sTopMost != "1")
            {
                cboTopMost.Checked = false;
                this.TopMost = false;
            }
            else
            {
                cboTopMost.Checked = true;
                this.TopMost = true;
            }
            //是否提醒
            string sIsTX = Convert.ToString(ConfigurationManager.AppSettings["GPIsTX"]);
            if (string.IsNullOrEmpty(sIsTX) || sIsTX != "1")
            {
                cboIsTX.Checked = false;
                BIsTX = false;
            }
            else
            {
                cboIsTX.Checked = true;
                BIsTX = true;
            }
            //提醒间隔时间
            BTXTime = Convert.ToString(ConfigurationManager.AppSettings["GPTXTime"]);
            //透明度
            string sOpacity = Convert.ToString(ConfigurationManager.AppSettings["GPOpacity"]);
            int mOpacity = 0;
            if (string.IsNullOrEmpty(sOpacity) || int.TryParse(sOpacity, out mOpacity) == false || mOpacity > 100 || mOpacity < 20)
            {
                trackBarOpacity.Value = 100;
                txtOpacity.Text = "100";
                this.Opacity = 100.00/100;
            }
            else
            {
                trackBarOpacity.Value = mOpacity;
                txtOpacity.Text = sOpacity;
                this.Opacity = Convert.ToDouble(mOpacity)/100;
            }
        }

        bool BSelect = true;
        private void ShowTT()
        {
            int fCount = 0;
            while (true)
            {
                //lock (locker)
                //{
                if (BSelect)
                {
                    BSelect = false;
                    //fCount++;
                    //Show(fCount);
                    ShowHGT();
                    Thread.Sleep(1500);
                    //log.Info("运行次数：" + fCount);
                    BSelect = true;
                }
                //}
            }
        }

        public void Show(int fCount)
        {
            List<APIStock> listStock = new List<APIStock>();
            List<APIMarket> listMarket = new List<APIMarket>();
            try
            {
                //if(!((DateTime.Now.Hour > 9 && DateTime.Now.Hour < 12) || (DateTime.Now.Hour >= 13 && DateTime.Now.Hour < 16)))
                //{
                //    return;
                //}
                string sUrl = Convert.ToString(ConfigurationManager.AppSettings["GPURL"]);
                //stockid=sz002673,sh600030,sz002673,sz002673,sz002673,sz002673,sz002673,sz002673,sz002673,sz002673&list=1
                string stockidList = ConfigurationManager.AppSettings["GPPamamList"].Trim();
                if (string.IsNullOrEmpty(stockidList))
                {
                    MessageBox.Show("请先添加股票");
                    return;
                }
                string sParam = "stockid=" + stockidList + "&list=1";
                string sMessage = HttpGet(sUrl, sParam);
                if (!string.IsNullOrEmpty(sMessage))
                {
                    JObject jo = (JObject)JsonConvert.DeserializeObject(sMessage);
                    string sReturnCode = Convert.ToString(jo["errNum"]);
                    string sReturnMsg = Convert.ToString(jo["errMsg"]);
                    if (string.IsNullOrEmpty(sReturnCode))
                    {
                        log.Error("请求异常,次数：" + fCount);
                        //MessageBox.Show("请求异常");
                        this.Text = "请求异常";
                        return;
                    }
                    int mReturnCode = Convert.ToInt32(sReturnCode);
                    if (mReturnCode == (int)ErrorCode.Success)
                    {
                        string sReturnContent = Convert.ToString(jo["retData"]);
                        if (!string.IsNullOrEmpty(sReturnContent))
                        {
                            JObject jContent = (JObject)JsonConvert.DeserializeObject(sReturnContent);
                            string sGSList = Convert.ToString(jContent["stockinfo"]);//上市公司列表
                            string sDPList = Convert.ToString(jContent["market"]);//上证,深证
                            int mGSCount = jContent["stockinfo"].AsQueryable().Count();
                            //涨跌判断
                            List<APIStock> listGPYJ = GetGPYJList();
                            for (int i = 0; i < mGSCount; i++)
                            {
                                APIStock stock = new APIStock();
                                JObject jStock = (JObject)jContent["stockinfo"][i];
                                stock.name = Convert.ToString(jStock["name"]).Trim('"');
                                if (string.IsNullOrEmpty(stock.name))
                                {
                                    continue;
                                }
                                stock.SysNo = Convert.ToString(jStock["code"]).Trim('"');
                                stock.currentPrice = Convert.ToString(jStock["currentPrice"]);
                                string mIncrease = Convert.ToString(Math.Round(Convert.ToDouble(jStock["increase"].ToString()), 2, MidpointRounding.AwayFromZero));//涨跌幅
                                if (BIsTX == true)
                                {
                                    int txT = 1;
                                    if (string.IsNullOrEmpty(BTXTime) || int.TryParse(BTXTime, out txT) == false)
                                    {
                                        txT = 1;
                                    }
                                    else
                                    {
                                        txT = Convert.ToInt32(BTXTime);
                                    }
                                    //当前股票已提醒的间隔时间
                                    var mGPYTX = (from t in queueTXMsgList where t.Code == stock.SysNo && t.CreateTime >= DateTime.Now.AddMinutes(-txT) select t).ToList();
                                    if (mGPYTX.Count <= 0)
                                    {
                                        if (listGPYJ != null && listGPYJ.Count > 0)
                                        {
                                            var mGP = (from t in listGPYJ where t.SysNo == stock.SysNo && t.ISYJ == 1 select t).ToList();
                                            if (mGP.Count > 0)
                                            {
                                                string returnMsg = PriceYJ(stock.currentPrice, mIncrease, mGP[0]);
                                                if (!string.IsNullOrEmpty(returnMsg))
                                                {
                                                    ErrorMsg eMsg = new ErrorMsg();
                                                    eMsg.Code = stock.SysNo;
                                                    eMsg.TitleType = (int)ErrorTitleType.GJMsg;
                                                    eMsg.Content = returnMsg;
                                                    eMsg.CreateTime = DateTime.Now;
                                                    eMsg.ShowTime = 5000;
                                                    eMsg.ToolTripType = ToolTipIcon.Warning;
                                                    queueMsgList.Enqueue(eMsg);
                                                    queueTXMsgList.Enqueue(eMsg);
                                                    //this.notifyIconMsg.ShowBalloonTip(5000, "消息提醒", returnMsg, ToolTipIcon.Warning);
                                                }
                                            }
                                        }
                                    }
                                }
                                
                                stock.name = Convert.ToString(jStock["name"]).Trim('"');
                                stock.code = Convert.ToString(jStock["code"]).Trim('"').Substring(2);
                                stock.date = jStock["time"].ToString().Trim('"');
                                stock.openningPrice = Convert.ToString(jStock["OpenningPrice"]);
                                stock.closingPrice = Convert.ToString(jStock["closingPrice"]);
                                stock.hPrice = Convert.ToString(jStock["hPrice"]);
                                stock.lPrice = Convert.ToString(jStock["lPrice"]);

                                //stock.growth = Convert.ToString(jStock["growth"]);
                                //stock.growthPercent = Convert.ToString(jStock["growthPercent"]);
                                //stock.dealnumber = Convert.ToString(jStock["dealnumber"]);
                                stock.turnover = Convert.ToString(Math.Round(Convert.ToDouble(jStock["turnover"].ToString()) / 100000000, 2, MidpointRounding.AwayFromZero));
                                //stock.hPrice52 = Convert.ToString(jStock["52hPrice"]);
                                //stock.lPrice52 = Convert.ToString(jStock["52lPrice"]);

                                stock.competitivePrice = Convert.ToString(jStock["competitivePrice"]);
                                stock.auctionPrice = Convert.ToString(jStock["auctionPrice"]);
                                stock.totalNumber = Convert.ToString(Math.Round(Convert.ToDouble(jStock["totalNumber"].ToString()) / 1000000, 2, MidpointRounding.AwayFromZero));
                                stock.increase = (mIncrease.Substring(0, 1) == "-") ? mIncrease.Substring(1) : mIncrease;
                                listStock.Add(stock);
                            }
                            JObject jMarket = (JObject)jContent["market"];
                            APIMarket mSH = new APIMarket();
                            mSH.name = jMarket["shanghai"]["name"].ToString().Trim('"');
                            mSH.curdot = Convert.ToString(Math.Round(Convert.ToDouble(jMarket["shanghai"]["curdot"].ToString()), 2, MidpointRounding.AwayFromZero));
                            mSH.curprice = Convert.ToString(Math.Round(Convert.ToDouble(jMarket["shanghai"]["curprice"].ToString()), 2, MidpointRounding.AwayFromZero));
                            mSH.rate = Convert.ToString(Math.Round(Convert.ToDouble(jMarket["shanghai"]["rate"].ToString()), 2, MidpointRounding.AwayFromZero));
                            mSH.dealnumber = Convert.ToString(Math.Round(Convert.ToDouble(jMarket["shanghai"]["dealnumber"].ToString()) / 100000000, 2, MidpointRounding.AwayFromZero));
                            mSH.turnover = Convert.ToString(Math.Round(Convert.ToDouble(jMarket["shanghai"]["turnover"].ToString()) / 10000, 2, MidpointRounding.AwayFromZero));
                            APIMarket mSZ = new APIMarket();
                            mSZ.name = Convert.ToString(jMarket["shenzhen"]["name"]).Trim('"');
                            mSZ.curdot = Convert.ToString(Math.Round(Convert.ToDouble(jMarket["shenzhen"]["curdot"].ToString()), 2, MidpointRounding.AwayFromZero));
                            mSZ.curprice = Convert.ToString(Math.Round(Convert.ToDouble(jMarket["shenzhen"]["curprice"].ToString()), 2, MidpointRounding.AwayFromZero));
                            mSZ.rate = Convert.ToString(Math.Round(Convert.ToDouble(jMarket["shenzhen"]["rate"].ToString()), 2, MidpointRounding.AwayFromZero));
                            mSZ.dealnumber = Convert.ToString(Math.Round(Convert.ToDouble(jMarket["shenzhen"]["dealnumber"].ToString()) / 100000000, 2, MidpointRounding.AwayFromZero));
                            mSZ.turnover = Convert.ToString(Math.Round(Convert.ToDouble(jMarket["shenzhen"]["turnover"].ToString()) / 10000, 2, MidpointRounding.AwayFromZero));
                            listMarket.Add(mSH);
                            listMarket.Add(mSZ);
                        }
                    }
                    else if (!Enum.IsDefined(typeof(ErrorCode), mReturnCode))
                    {
                        log.Error("返回未知异常,次数：" + fCount + ",异常编号:" + mReturnCode + ";异常说明：" + sReturnMsg);
                        //MessageBox.Show("返回未知异常,异常编号:" + mReturnCode + ";异常说明：" + sReturnMsg);
                        this.Text = "返回未知异常,异常编号:" + mReturnCode + ";异常说明：" + sReturnMsg;
                        return;
                    }
                    else
                    {
                        MessageBox.Show(GetDescription(typeof(ErrorCode), mReturnCode));
                    }
                }
                else
                {
                    log.Error("返回数据空,次数：" + fCount);
                    //MessageBox.Show("返回数据空");
                    this.Text = "返回数据空";
                    return;
                }
            }
            catch (Exception ex)
            {
                log.Error("获取股票数据1，出现异常："+ex.Message+";异常详情：" + ex);
            }

            try
            {
                if (listMarket.Count > 0)
                {
                    APIMarket apiSHZS = listMarket[0];
                    double shRate = Convert.ToDouble(apiSHZS.rate);
                    Color shColor = Color.Red;
                    string shZF = "+";
                    string shJT = "↑";
                    if (shRate >= 0)
                    {
                        shColor = Color.Red;
                        shZF = "+";
                        shJT = "↑";
                    }
                    else
                    {
                        shColor = Color.Green;
                        shZF = "";
                        shJT = "↓";
                    }
                    lblSHZS1.Text = apiSHZS.name;
                    lblSHZS1.ForeColor = shColor;
                    lblSHZS2.Text = shJT + apiSHZS.curdot + " " + shZF + apiSHZS.rate + "%";
                    lblSHZS2.ForeColor = shColor;
                    //lblSHZS3.Text = shZF + apiSHZS.curprice + " " + shZF + apiSHZS.rate + "%";
                    //lblSHZS3.ForeColor = shColor;
                    APIMarket apiSZZS = listMarket[1];
                    double szRate = Convert.ToDouble(apiSZZS.rate);
                    Color szColor = Color.Red;
                    string szZF = "+";
                    string szJT = "↑";
                    if (szRate >= 0)
                    {
                        szColor = Color.Red;
                        szZF = "+";
                        szJT = "↑";
                    }
                    else
                    {
                        szColor = Color.Green;
                        szZF = "";
                        szJT = "↓";
                    }
                    lblSZZS1.Text = apiSZZS.name;
                    lblSZZS1.ForeColor = szColor;
                    lblSZZS2.Text = szJT + apiSZZS.curdot + " " + szZF + apiSZZS.rate + "%";
                    lblSZZS2.ForeColor = szColor;
                    //lblSZZS3.Text = szZF + apiSZZS.curprice + " " + szZF + apiSZZS.rate + "%";
                    //lblSZZS3.ForeColor = szColor;
                }
            }
            catch (Exception ex)
            {
                log.Error("获取股票数据2，出现异常："+ex.Message+";异常详情：" + ex);
            }

            try
            {
                lock (locker)
                {
                    #region 绑定数据到DataGridView

                    dataGridView1.Columns.Clear();
                    if (listStock.Count > 0)
                    {
                        //dataGridView1.Columns[4].DataPropertyName
                        DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
                        col1.HeaderText = "股票名称";
                        col1.DataPropertyName = "name";
                        col1.Name = "name";
                        col1.Width = 80;
                        col1.ReadOnly = true;
                        col1.Frozen = true;
                        //col1.ContextMenuStrip = contextMenuStripGridView;
                        dataGridView1.Columns.Insert(0, col1);
                        DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn();
                        col3.HeaderText = "当前价";
                        col3.DataPropertyName = "currentPrice";
                        col3.Name = "currentPrice";
                        col3.Width = 70;
                        col3.ReadOnly = true;
                        //col3.ContextMenuStrip = contextMenuStripGridView;
                        dataGridView1.Columns.Insert(1, col3);
                        DataGridViewTextBoxColumn col18 = new DataGridViewTextBoxColumn();
                        col18.HeaderText = "涨幅(%)";
                        col18.DataPropertyName = "increase";
                        col18.Name = "increase";
                        col18.Width = 75;
                        col18.ReadOnly = true;
                        //col18.ContextMenuStrip = contextMenuStripGridView;
                        dataGridView1.Columns.Insert(2, col18);
                        DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
                        col2.HeaderText = "股票代码";
                        col2.DataPropertyName = "code";
                        col2.Name = "code";
                        col2.Width = 80;
                        col2.ReadOnly = true;
                        //col2.ContextMenuStrip = contextMenuStripGridView;
                        dataGridView1.Columns.Insert(3, col2);
                        DataGridViewTextBoxColumn col6 = new DataGridViewTextBoxColumn();
                        col6.HeaderText = "今日最高价";
                        col6.DataPropertyName = "hPrice";
                        col6.Name = "hPrice";
                        col6.Width = 90;
                        col6.ReadOnly = true;
                        dataGridView1.Columns.Insert(4, col6);
                        DataGridViewTextBoxColumn col7 = new DataGridViewTextBoxColumn();
                        col7.HeaderText = "今日最低价";
                        col7.DataPropertyName = "lPrice";
                        col7.Name = "lPrice";
                        col7.Width = 90;
                        col7.ReadOnly = true;
                        dataGridView1.Columns.Insert(5, col7);
                        DataGridViewTextBoxColumn col8 = new DataGridViewTextBoxColumn();
                        col8.HeaderText = "开盘价";
                        col8.DataPropertyName = "openningPrice";
                        col8.Name = "openningPrice";
                        col8.Width = 70;
                        col8.ReadOnly = true;
                        dataGridView1.Columns.Insert(6, col8);
                        DataGridViewTextBoxColumn col9 = new DataGridViewTextBoxColumn();
                        col9.HeaderText = "昨日收盘价";
                        col9.DataPropertyName = "closingPrice";
                        col9.Name = "closingPrice";
                        col9.Width = 90;
                        col9.ReadOnly = true;
                        dataGridView1.Columns.Insert(7, col9);
                        DataGridViewTextBoxColumn col17 = new DataGridViewTextBoxColumn();
                        col17.HeaderText = "成交量(万)";
                        col17.DataPropertyName = "totalNumber";
                        col17.Name = "totalNumber";
                        col17.Width = 90;
                        col17.ReadOnly = true;
                        dataGridView1.Columns.Insert(8, col17);
                        DataGridViewTextBoxColumn col13 = new DataGridViewTextBoxColumn();
                        col13.HeaderText = "成交金额(亿)";
                        col13.DataPropertyName = "turnover";
                        col13.Name = "turnover";
                        col13.Width = 100;
                        col13.ReadOnly = true;
                        dataGridView1.Columns.Insert(9, col13);
                        DataGridViewTextBoxColumn col14 = new DataGridViewTextBoxColumn();
                        col14.HeaderText = "刷新日期";
                        col14.DataPropertyName = "date";
                        col14.Name = "date";
                        col14.Width = 90;
                        col14.ReadOnly = true;
                        dataGridView1.Columns.Insert(10, col14);

                        //DataGridViewTextBoxColumn col4 = new DataGridViewTextBoxColumn();
                        //col4.HeaderText = "涨幅比例";
                        //col4.DataPropertyName = "growthPercent";
                        //col4.Name = "growthPercent";
                        //col4.Width = 100;
                        //col4.ReadOnly = true;
                        //dataGridView1.Columns.Add(col4);
                        //DataGridViewTextBoxColumn col5 = new DataGridViewTextBoxColumn();
                        //col5.HeaderText = "价格涨幅";
                        //col5.DataPropertyName = "growth";
                        //col5.Name = "growth";
                        //col5.Width = 100;
                        //col5.ReadOnly = true;
                        //dataGridView1.Columns.Add(col5);


                        //DataGridViewTextBoxColumn col10 = new DataGridViewTextBoxColumn();
                        //col10.HeaderText = "52周最高价";
                        //col10.DataPropertyName = "hPrice52";
                        //col10.Name = "hPrice52";
                        //col10.Width = 100;
                        //col10.ReadOnly = true;
                        //dataGridView1.Columns.Add(col10);
                        //DataGridViewTextBoxColumn col11 = new DataGridViewTextBoxColumn();
                        //col11.HeaderText = "52周最低价";
                        //col11.DataPropertyName = "lPrice52";
                        //col11.Name = "lPrice52";
                        //col11.Width = 100;
                        //col11.ReadOnly = true;
                        //dataGridView1.Columns.Add(col11);
                        //DataGridViewTextBoxColumn col12 = new DataGridViewTextBoxColumn();
                        //col12.HeaderText = "成交量股";
                        //col12.DataPropertyName = "dealnumber";
                        //col12.Name = "dealnumber";
                        //col12.Width = 100;
                        //col12.ReadOnly = true;
                        //dataGridView1.Columns.Add(col12);

                        //DataGridViewTextBoxColumn col15 = new DataGridViewTextBoxColumn();
                        //col15.HeaderText = "未知competitivePrice";
                        //col15.DataPropertyName = "competitivePrice";
                        //col15.Name = "competitivePrice";
                        //col15.Width = 100;
                        //col15.ReadOnly = true;
                        //dataGridView1.Columns.Add(col15);
                        //DataGridViewTextBoxColumn col16 = new DataGridViewTextBoxColumn();
                        //col16.HeaderText = "未知auctionPrice";
                        //col16.DataPropertyName = "auctionPrice";
                        //col16.Name = "auctionPrice";
                        //col16.Width = 100;
                        //col16.ReadOnly = true;
                        //dataGridView1.Columns.Add(col16);

                        //dataGridView1.AllowUserToAddRows = false;
                        //dataGridView1.AllowUserToOrderColumns = true;        //允许用户调整列的位置
                        //dataGridView1.Columns[0].Visible = false; 
                        //dataGridView1.ColumnHeadersVisible = false; // 列头隐藏 
                        //dataGridView1.RowHeadersVisible = false; // 行头隐藏
                        dataGridView1.ReadOnly = true;
                        dataGridView1.AutoGenerateColumns = false;

                        dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dataGridView1.DataSource = listStock;
                        //http://wsh1798.iteye.com/blog/601592
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                log.Error("获取股票数据3，出现异常："+ex.Message+";异常详情：" + ex);
            }

            try
            {
                //ShowHGT();
            }
            catch (Exception ex)
            {
                log.Error("获取股票数据4，出现异常："+ex.Message+";异常详情：" + ex);
            }
        }

        public void ShowHGT()
        {
            string pageContent = ShowWebClient("http://data.eastmoney.com/bkzj/hgt.html");
            if (!string.IsNullOrEmpty(pageContent))
            {
                int st = pageContent.IndexOf("defset1({");
                int en = pageContent.IndexOf("})", st);
                string sMessage = pageContent.Substring(st + 8, en - st - 7);
                JObject jo = (JObject)JsonConvert.DeserializeObject(sMessage);
                if (jo.Count > 0)
                {
                    string hgtList = jo["data"][0].ToString().Trim('"');
                    string ggtList = jo["data"][1].ToString().Trim('"');
                    if (!string.IsNullOrEmpty(hgtList))
                    {
                        string[] hgtLists = hgtList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        string hV = hgtLists[3];
                        lblSHZS3.Text = hgtLists[0] + " " + hgtLists[3];
                        if (hV.Contains("-"))
                        {
                            lblSHZS3.ForeColor = Color.Blue;
                        }
                        else
                        {
                            lblSHZS3.ForeColor = Color.Red;
                        }
                    }
                    if (!string.IsNullOrEmpty(ggtList))
                    {
                        string[] ggtLists = ggtList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        string gV = ggtLists[3];
                        lblSZZS3.Text = ggtLists[0] + " " + ggtLists[3];
                        if (gV.Contains("-"))
                        {
                            lblSZZS3.ForeColor = Color.Blue;
                        }
                        else
                        {
                            lblSZZS3.ForeColor = Color.Red;
                        }
                    }
                }
            }
            else
            {
                lblSHZS3.Text = "";
                lblSZZS3.Text = "";

            }
        }

        //接口请求数据
        public string HttpGet(string Url, string postDataStr)
        {
            string retString = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                //添加header
                request.Headers.Add("apikey", Convert.ToString(ConfigurationManager.AppSettings["GPKey"]));
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                //string retString = myStreamReader.ReadToEnd();
                string StrDate = "";

                while ((StrDate = myStreamReader.ReadLine()) != null)
                {
                    retString += StrDate + "\r\n";
                }
                myStreamReader.Close();
                myResponseStream.Close();
            }
            catch (Exception ex)
            {
                log.Error("调用接口请求数据出现异常,异常信息:" + ex.Message + ";异常详情：" + ex);
            }
            return retString;
        }

        //下载页面数据
        private string ShowWebClient(string url)
        {
            string strHtml = string.Empty;
            try
            {
                WebClient wc = new WebClient();
                Stream myStream = wc.OpenRead(url);
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                strHtml = sr.ReadToEnd();
                myStream.Close();
            }
            catch (Exception ex)
            {
                log.Error("下载沪港通页面出错，错误信息："+ex.Message+";错误详情：" + ex);
            }
            return strHtml;
        }

        //刷新
        private void btnSelect_Click(object sender, EventArgs e)
        {
            //notifyIconMsg.ShowBalloonTip(4000, "消息提醒", "刷新了数据", ToolTipIcon.Info);
            //FormMessageBox.Show(LoadMode.Warning, "呵呵呵呵");//弹出窗口(暂不使用)
            int fCount = 0;
            //while(true)
            //{
            //    fCount++;
                Show(fCount);
            //    Thread.Sleep(1000);
            //    log.Info("运行次数：" + fCount);
            //}
        }

        //左右扩展
        private void btnRight_Click(object sender, EventArgs e)
        {
            if (btnRight.Text.Trim() == "》")
            {
                this.Width = 985;
                dataGridView1.Width = 968;
                btnRight.Text = "《";

                this.Height = 505;
                btnDown.Text = "︽";

                Cache cache = System.Web.HttpRuntime.Cache;
                List<APIStock> listYJ = (List<APIStock>)cache.Get("MGPYJ");
                if (listYJ != null && listYJ.Count > 0)
                {
                    listYJ = GetGPYJList();
                }
                dataGridView2.AutoGenerateColumns = false;
                dataGridView2.Columns[0].ReadOnly = true;
                dataGridView2.Columns[1].ReadOnly = true;
                dataGridView2.Columns[2].ReadOnly = true;
                dataGridView2.DataSource = listYJ;
            }
            else
            {
                this.Width = 280;
                dataGridView1.Width = 270;
                btnRight.Text = "》";

                this.Height = 400;//默认设置400
                btnDown.Text = "︾";
            }
        }

        //上下扩展
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (btnDown.Text.Trim() == "︾")
            {
                this.Height = 505;
                btnDown.Text = "︽";
            }
            else
            {
                this.Height = 400;//默认设置400
                btnDown.Text = "︾";
                btnDownX.Text = "︾";

                this.Width = 280;
                dataGridView1.Width = 270;
                btnRight.Text = "》";
            }
        }

        //上下扩展X
        private void btnDownX_Click(object sender, EventArgs e)
        {
            //if (btnDownX.Text.Trim() == "︾")
            //{
            //    this.Height = 730;
            //    btnDownX.Text = "︽";
            //    //GetAPIDataPushGPYJ();
            //    Cache cache = System.Web.HttpRuntime.Cache;
            //    List<APIStock> listYJ = (List<APIStock>)cache.Get("MGPYJ");
            //    if (listYJ != null && listYJ.Count > 0)
            //    {
            //        listYJ = GetGPYJList();
            //    }
            //    dataGridView2.AutoGenerateColumns = false;
            //    dataGridView2.Columns[0].ReadOnly = true;
            //    dataGridView2.Columns[1].ReadOnly = true;
            //    dataGridView2.DataSource = listYJ;
            //}
            //else
            //{
            //    this.Height = 400;//默认设置400
            //    btnDownX.Text = "︾";
            //    btnDown.Text = "︾";
            //}
        }

        //添加
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string mCode = txtCode.Text.Trim();
            if(string.IsNullOrEmpty(mCode))
            {
                MessageBox.Show("请填入证券代码");
                return;
            }
            //判断是否全数字及长度是否为六位
            int kCode;
            if (int.TryParse(mCode, out kCode) == false)
            {
                MessageBox.Show("证券代码必须为六位数字");
                return;
            }
            if (mCode.Length != 6)
            {
                MessageBox.Show("证券代码必须为六位数字");
                return;
            }
            if (mCode.Substring(0, 1) == "6")
            {
                mCode = "sh" + mCode;
            }
            else
            {
                mCode = "sz" + mCode;
            }
            string YYCode = Convert.ToString(ConfigurationManager.AppSettings["GPPamamList"]).Trim();
            if (YYCode.Contains(mCode))
            {
                MessageBox.Show("添加成功");
            }
            else
            {
                if (string.IsNullOrEmpty(YYCode))
                {
                    YYCode = mCode;
                }
                else
                {
                    YYCode = YYCode + "," + mCode;
                }
                
                UpdateAppConfig("GPPamamList", YYCode);
            }
            Show(1);
        }

        //删除
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您确定要删除吗？", "删除提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly) == DialogResult.No)
            {
                return;
            }
            string mCode = txtCode.Text.Trim();
            if (string.IsNullOrEmpty(mCode))
            {
                MessageBox.Show("请填入证券代码");
                return;
            }
            //判断是否全数字及长度是否为六位
            int kCode;
            if (int.TryParse(mCode, out kCode) == false)
            {
                MessageBox.Show("证券代码必须为六位数字");
                return;
            }
            if (mCode.Length != 6)
            {
                MessageBox.Show("证券代码必须为六位数字");
                return;
            }
            if (mCode.Substring(0, 1) == "6")
            {
                mCode = "sh" + mCode;
            }
            else
            {
                mCode = "sz" + mCode;
            }
            //sh600030,sz002673,sz002673
            string YYCode = Convert.ToString(ConfigurationManager.AppSettings["GPPamamList"]).Trim();
            if (YYCode.Contains(mCode))
            {
                int a = YYCode.ToUpper().IndexOf(mCode.ToUpper());
                string sT = "";
                string eT = "";
                if (a <= 0)
                {
                    eT = YYCode.Substring(9);
                }
                else if(a + 7 <= YYCode.Length)
                {
                    sT = YYCode.Substring(0, a - 1);
                }
                else
                {
                    sT = YYCode.Substring(0, a - 1);
                    eT = YYCode.Substring(a + 9);
                    
                }
                YYCode = sT + eT;
                UpdateAppConfig("GPPamamList", YYCode);
                MessageBox.Show("删除成功");
            }
            else
            {
                MessageBox.Show("删除成功");
                return;
            }
            Show(1);
        }

        //全删除
        private void btnAllRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您确定要全删除吗？", "删除提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly) == DialogResult.Yes)
            {
                string YYCode = Convert.ToString(ConfigurationManager.AppSettings["GPPamamList"]).Trim();
                UpdateAppConfig("GPPamamList", "");
                MessageBox.Show("全删除成功");
                Show(1);
            }
        }
        
        //涨红跌绿
        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            try
            {
                dataGridView1.Columns[e.RowIndex].SortMode = DataGridViewColumnSortMode.NotSortable;//表头不排序 为标题居中
                //昨日收盘价
                object sValue = this.dataGridView1.Rows[e.RowIndex].Cells["closingPrice"].Value;
                //当前价
                object cValue = this.dataGridView1.Rows[e.RowIndex].Cells["currentPrice"].Value;
                //差价
                double iValue = Convert.ToDouble(cValue) - Convert.ToDouble(sValue);
                //涨跌幅( (当前价-昨收盘价)/昨收盘价 )
                if (iValue > 0)//差价
                {
                    this.dataGridView1.Rows[e.RowIndex].Cells["currentPrice"].Style.ForeColor = Color.Red;
                    this.dataGridView1.Rows[e.RowIndex].Cells["increase"].Style.ForeColor = Color.Red;
                    this.dataGridView1.Rows[e.RowIndex].Cells["increase"].Style.Font = new Font("宋体", 15);
                }
                else if (iValue < 0)
                {
                    this.dataGridView1.Rows[e.RowIndex].Cells["currentPrice"].Style.ForeColor = Color.Green;
                    this.dataGridView1.Rows[e.RowIndex].Cells["increase"].Style.ForeColor = Color.Green;
                    this.dataGridView1.Rows[e.RowIndex].Cells["increase"].Style.Font = new Font("宋体", 12);
                }
                else
                {
                    this.dataGridView1.Rows[e.RowIndex].Cells["currentPrice"].Style.ForeColor = Color.White;
                    this.dataGridView1.Rows[e.RowIndex].Cells["increase"].Style.ForeColor = Color.White;
                    this.dataGridView1.Rows[e.RowIndex].Cells["increase"].Style.Font = new Font("宋体", 9);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        //消息提示相关
        private void notifyIconMsg_DoubleClick(object sender, EventArgs e)
        {
            this.notifyIconMsg.ShowBalloonTip(3000, "消息提醒", "叫你小子别碰我，你还敢碰，你以为你是吊炸天么？", ToolTipIcon.Error);
        }

        //退出系统
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            this.Close();
        }

        //是否置顶
        private void cboTopMost_CheckedChanged(object sender, EventArgs e)
        {
            if (cboTopMost.Checked == true)
            {
                this.TopMost = true;
                UpdateAppConfig("GPTopMost", "1");
            }
            else
            {
                this.TopMost = false;
                UpdateAppConfig("GPTopMost", "0");
            }
        }

        //透明度修改
        private void txtOpacity_MouseLeave(object sender, EventArgs e)
        {
            string sOpacity = txtOpacity.Text.Trim();
            int mOpactity = 0;
            if (string.IsNullOrEmpty(sOpacity) || int.TryParse(sOpacity, out mOpactity) == false || mOpactity > 100 || mOpactity < 20)
            {
                this.notifyIconMsg.ShowBalloonTip(3000, "消息提醒", "透明度设置有误，请重新设置", ToolTipIcon.Error);
                txtOpacity.Text = "40";
                return;
            }
            trackBarOpacity.Value = mOpactity;
            this.Opacity = Convert.ToDouble(mOpactity) / 100;
            UpdateAppConfig("GPOpacity", sOpacity);
        }

        //透明度修改
        private void trackBarOpacity_Scroll(object sender, EventArgs e)
        {
            int mOpactity = trackBarOpacity.Value;
            if (mOpactity > 100 || mOpactity < 30)
            {
                this.notifyIconMsg.ShowBalloonTip(3000, "消息提醒", "透明度设置有误，设置范围为(20 -> 100)，请重新设置", ToolTipIcon.Error);
                txtOpacity.Text = "40";
                trackBarOpacity.Value = 40;
                return;
            }
            txtOpacity.Text = mOpactity.ToString();
            this.Opacity = Convert.ToDouble(mOpactity) / 100;
            UpdateAppConfig("GPOpacity", mOpactity.ToString());
        }


        //更新配置文件
        private void UpdateAppConfig(string newKey, string newValue)
        {
            bool isExits = false;
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key == newKey)
                {
                    isExits = true;
                }
            }
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (isExits)
            {
                config.AppSettings.Settings.Remove(newKey);
            }
            config.AppSettings.Settings.Add(newKey, newValue);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        #region 快捷键相关
        //KeyPreview = true
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(e.KeyValue.ToString());
            if (e.KeyCode == Keys.F1 && e.Modifiers == Keys.Control)
            {
                MessageBox.Show("Ctrl + F1");
            }
            if ((int)e.Modifiers == ((int)Keys.Control + (int)Keys.Alt) && e.KeyCode == Keys.D0)
            {
                MessageBox.Show("Ctrl + Alt + 0");
            }
            this.AcceptButton = btnSelect;
            //this.CancelButton = btnSelect;
        }

        //在Form的Activate事件中注册热键，本例中注册Win+Q，Win+W，Win+Z这三个热键。这里的Id号可任意设置，但要保证不被重复。
        private void Form1_Activated(object sender, EventArgs e)
        {
            //注册热键Win+Q，Id号为100。HotKey.KeyModifiers.Shift也可以直接使用数字4来表示。
            HotKey.RegisterHotKey(Handle, 100, HotKey.KeyModifiers.WindowsKey, Keys.Q);//显示
            //注册热键Win+W，Id号为101。HotKey.KeyModifiers.Ctrl也可以直接使用数字2来表示。
            HotKey.RegisterHotKey(Handle, 101, HotKey.KeyModifiers.WindowsKey, Keys.W);//隐藏
            //注册热键Win+Z，Id号为102。HotKey.KeyModifiers.Alt也可以直接使用数字1来表示。
            HotKey.RegisterHotKey(Handle, 102, HotKey.KeyModifiers.WindowsKey, Keys.Z);//退出

            HotKey.RegisterHotKey(Handle, 103, HotKey.KeyModifiers.Alt, Keys.C);//临时提高透明度
            HotKey.RegisterHotKey(Handle, 104, HotKey.KeyModifiers.Alt, Keys.V);//临时减低透明度
        }

        //在Form的Leave事件中注销热键。
        private void Form1_Leave(object sender, EventArgs e)
        {
            //注销Id号为100的热键设定
            HotKey.UnregisterHotKey(Handle, 100);
            //注销Id号为101的热键设定
            HotKey.UnregisterHotKey(Handle, 101);
            //注销Id号为102的热键设定
            HotKey.UnregisterHotKey(Handle, 102);

            HotKey.UnregisterHotKey(Handle, 103);
            HotKey.UnregisterHotKey(Handle, 104);
        }

        /// <summary>
        /// 重载From中的WndProc函数
        /// 监视Windows消息
        /// 重载WndProc方法，用于实现热键响应
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            //按快捷键 
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:    //按下的是Shift+S
                            //此处填写快捷键响应代码 
                            //MessageBox.Show("Win + Q");
                            this.Show();
                            break;
                        case 101:    //按下的是Ctrl+B
                            //此处填写快捷键响应代码
                            //MessageBox.Show("Win+ W");
                            this.Hide();
                            break;
                        case 102:    //按下的是Alt+D
                            //此处填写快捷键响应代码
                            //MessageBox.Show("Win + Z");
                            timer1.Enabled = false;
                            this.Close();
                            break;
                        case 103://临时提高透明度
                            if(this.Opacity + 0.1 <= 1)
                            {
                                this.Opacity = this.Opacity + 0.1;
                            }
                            break;
                        case 104://临时降低透明度
                            if (this.Opacity - 0.1 >= 0)
                            {
                                this.Opacity = this.Opacity - 0.1;
                            }
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        #endregion

        #region 错误码相关
        //错误码
        public enum ErrorCode : int
        {
            [Description("成功")]
            Success = 0,
            [Description("用户请求过期")]
            Error1 = 300101,
            [Description("用户日调用量超限")]
            Error2 = 300102,
            [Description("服务每秒调用量超限")]
            Error3 = 300103,
            [Description("服务日调用量超限")]
            Error4 = 300104,
            [Description("url无法解析")]
            Error5 = 300201,
            [Description("请求缺少apikey，登录即可获取")]
            Error6 = 300202,
            [Description("服务没有取到apikey或secretkey")]
            Error7 = 300203,
            [Description("apikey不存在")]
            Error8 = 300204,
            [Description("api不存在")]
            Error9 = 300205,
            [Description("api已关闭服务")]
            Error10 = 300206,
            [Description("余额不足，请充值")]
            Error11 = 300207,
            [Description("未通过实名验证")]
            Error12 = 300208,
            [Description("服务商响应status非200")]
            Error13 = 300209,
            [Description("内部错误")]
            Error14 = 300301,
            [Description("系统繁忙稍候再试")]
            Error15 = 300302,
        }

        /// <summary>
        /// 返回指定枚举类型的指定值的描述
        /// </summary>
        /// <param name="t">枚举类型</param>
        /// <param name="v">枚举值</param>
        /// <returns></returns>
        public static string GetDescription(System.Type t, object v)
        {
            try
            {
                FieldInfo fi = t.GetField(GetName(t, v));
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attributes.Length > 0) ? attributes[0].Description : GetName(t, v);
            }
            catch
            {
                return "UNKNOWN";
            }
        }

        private static string GetName(System.Type t, object v)
        {
            try
            {
                return Enum.GetName(t, v);
            }
            catch
            {
                return "UNKNOWN";
            }
        }
        #endregion

        //定时器
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (BCanExec)
            {
                BCanExec = false;
                Show(11);
                //MessageBox.Show("333");
                BCanExec = true;
            }
        }

        #region 预警相关
        //预警配置文件API更新(停用)
        private bool GetAPIDataPushGPYJ()
        {
            Cache cache = System.Web.HttpRuntime.Cache;
            List<APIStock> listYJ = (List<APIStock>)cache.Get("MGPYJ");
            if (listYJ != null && listYJ.Count > 0)
            {
                dataGridView2.AutoGenerateColumns = false;
                dataGridView2.Columns[0].ReadOnly = true;
                dataGridView2.Columns[1].ReadOnly = true;
                dataGridView2.DataSource = listYJ;
                return true;
            }

            string sUrl = Convert.ToString(ConfigurationManager.AppSettings["GPURL"]);
            string stockidList = ConfigurationManager.AppSettings["GPPamamList"].Trim();
            if (string.IsNullOrEmpty(stockidList))
            {
                MessageBox.Show("请先添加股票");
                return false;
            }
            string sParam = "stockid=" + stockidList + "&list=1";
            string sMessage = HttpGet(sUrl, sParam);
            if (!string.IsNullOrEmpty(sMessage))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(sMessage);
                string sReturnCode = Convert.ToString(jo["errNum"]);
                string sReturnMsg = Convert.ToString(jo["errMsg"]);
                if (string.IsNullOrEmpty(sReturnCode))
                {
                    this.Text = "请求异常";
                    return false;
                }
                int mReturnCode = Convert.ToInt32(sReturnCode);
                if (mReturnCode == (int)ErrorCode.Success)
                {
                    string sReturnContent = Convert.ToString(jo["retData"]);
                    if (!string.IsNullOrEmpty(sReturnContent))
                    {
                        JObject jContent = (JObject)JsonConvert.DeserializeObject(sReturnContent);
                        string sGSList = Convert.ToString(jContent["stockinfo"]);//上市公司列表
                        string sDPList = Convert.ToString(jContent["market"]);//上证,深证
                        int mGSCount = jContent["stockinfo"].AsQueryable().Count();

                        XmlDocument GridXmlX = new XmlDocument();
                        GridXmlX.Load(Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App_Data/GPYJ.xml")));
                        XmlNode rootX = GridXmlX.SelectSingleNode("Root");
                        for (int i = 0; i < mGSCount; i++)
                        {
                            APIStock stock = new APIStock();
                            JObject jStock = (JObject)jContent["stockinfo"][i];
                            string name = Convert.ToString(jStock["name"]).Trim('"');
                            string code = Convert.ToString(jStock["code"]).Trim('"');
                            if (string.IsNullOrEmpty(name))
                            {
                                continue;
                            }
                            XmlNode GridNode = GridXmlX.DocumentElement.SelectSingleNode("//GPDM[@Code='" + code + "']");
                            if (GridNode != null)//存在节点仅修改名称
                            {
                                XmlElement GridElement = (XmlElement)GridNode;
                                GridElement.SetAttribute("Name", name);
                            }
                            else
                            {
                                XmlElement oneGPDM = GridXmlX.CreateElement("GPDM");
                                oneGPDM.SetAttribute("Code", code);
                                oneGPDM.SetAttribute("Name", name);
                                XmlElement twoZFB = GridXmlX.CreateElement("ZFB");
                                //twoZFB.InnerText = "5";
                                oneGPDM.AppendChild(twoZFB);
                                XmlElement twoDFB = GridXmlX.CreateElement("DFB");
                                oneGPDM.AppendChild(twoDFB);
                                XmlElement twoZFY = GridXmlX.CreateElement("ZFY");
                                oneGPDM.AppendChild(twoZFY);
                                XmlElement twoDFY = GridXmlX.CreateElement("DFY");
                                oneGPDM.AppendChild(twoDFY);
                                rootX.AppendChild(oneGPDM);
                            }
                        }
                        GridXmlX.Save(Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App_Data/GPYJ.xml")));
                    }
                }
                else if (!Enum.IsDefined(typeof(ErrorCode), mReturnCode))
                {
                    this.Text = "返回未知异常,异常编号:" + mReturnCode + ";异常说明：" + sReturnMsg;
                    return false;
                }
                else
                {
                    MessageBox.Show(GetDescription(typeof(ErrorCode), mReturnCode));
                }
            }
            else
            {
                this.Text = "返回数据空";
                return false;
            }

            listYJ = GetGPYJList();
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.Columns[0].ReadOnly = true;
            dataGridView2.Columns[1].ReadOnly = true;
            dataGridView2.DataSource = listYJ;
            return true;
        }

        //预警列表(停用)
        private void GetGPYJListM()
        {
            List<APIStock> listYJ = new List<APIStock>();
            XmlDocument GridXml = new XmlDocument();
            GridXml.Load(Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App_Data/GPYJ.xml")));
            XmlNode root = GridXml.SelectSingleNode("Root");
            XmlElement rootElement = (XmlElement)root;

            string YYCode = Convert.ToString(ConfigurationManager.AppSettings["GPPamamList"]).Trim();
            string[] YYCodes = YYCode.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string mCode in YYCodes)
            {
                XmlNode GridNode = GridXml.DocumentElement.SelectSingleNode("//GPDM[@Code='" + mCode + "']");
                if (GridNode != null)//存在节点仅修改名称
                {
                    XmlElement oneElement = (XmlElement)GridNode;
                    APIStock stock = new APIStock();
                    stock.SysNo = oneElement.Attributes["Code"].Value;
                    stock.name = oneElement.Attributes["Name"].Value;
                    stock.code = oneElement.Attributes["Code"].Value.Substring(2);
                    stock.ZFB = oneElement.SelectSingleNode("ZFB").InnerText;
                    stock.DFB = oneElement.SelectSingleNode("DFB").InnerText;
                    stock.ZFY = oneElement.SelectSingleNode("ZFY").InnerText;
                    stock.DFY = oneElement.SelectSingleNode("DFY").InnerText;
                    listYJ.Add(stock);
                }
            }
            //XmlNodeList ColumnList = rootElement.SelectNodes("GPDM");
            //for (int i = 0; i < ColumnList.Count;i++ )
            //{
            //    XmlElement oneElement = (XmlElement)ColumnList[i];
            //}
            if(listYJ.Count > 0)
            {
                Cache c = System.Web.HttpRuntime.Cache;
                CacheDependency cdd = new CacheDependency(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "GPSmallTools.exe.Config"));
                c.Insert("MGPYJ", listYJ, cdd, System.DateTime.UtcNow.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);

                dataGridView2.AutoGenerateColumns = false;
                dataGridView2.Columns[0].ReadOnly = true;
                dataGridView2.Columns[1].ReadOnly = true;
                dataGridView2.DataSource = listYJ;
            }
        }

        //预警列表
        private List<APIStock> GetGPYJList()
        {
            List<APIStock> listYJ = new List<APIStock>();
            Cache cache = System.Web.HttpRuntime.Cache;
            listYJ = (List<APIStock>)cache.Get("MGPYJ");
            if (listYJ != null && listYJ.Count > 0)
            {
                return listYJ;
            }

            #region 预警配置文件API更新
            string sUrl = Convert.ToString(ConfigurationManager.AppSettings["GPURL"]);
            string stockidList = ConfigurationManager.AppSettings["GPPamamList"].Trim();
            if (string.IsNullOrEmpty(stockidList))
            {
                MessageBox.Show("请先添加股票");
                return null;
            }
            string sParam = "stockid=" + stockidList + "&list=1";
            string sMessage = HttpGet(sUrl, sParam);
            if (!string.IsNullOrEmpty(sMessage))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(sMessage);
                string sReturnCode = Convert.ToString(jo["errNum"]);
                string sReturnMsg = Convert.ToString(jo["errMsg"]);
                if (string.IsNullOrEmpty(sReturnCode))
                {
                    this.Text = "请求异常";
                    return null;
                }
                int mReturnCode = Convert.ToInt32(sReturnCode);
                if (mReturnCode == (int)ErrorCode.Success)
                {
                    string sReturnContent = Convert.ToString(jo["retData"]);
                    if (!string.IsNullOrEmpty(sReturnContent))
                    {
                        JObject jContent = (JObject)JsonConvert.DeserializeObject(sReturnContent);
                        string sGSList = Convert.ToString(jContent["stockinfo"]);//上市公司列表
                        string sDPList = Convert.ToString(jContent["market"]);//上证,深证
                        int mGSCount = jContent["stockinfo"].AsQueryable().Count();

                        XmlDocument GridXmlX = new XmlDocument();
                        GridXmlX.Load(Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App_Data/GPYJ.xml")));
                        XmlNode rootX = GridXmlX.SelectSingleNode("Root");
                        for (int i = 0; i < mGSCount; i++)
                        {
                            APIStock stock = new APIStock();
                            JObject jStock = (JObject)jContent["stockinfo"][i];
                            string name = Convert.ToString(jStock["name"]).Trim('"');
                            string code = Convert.ToString(jStock["code"]).Trim('"');
                            if (string.IsNullOrEmpty(name))
                            {
                                continue;
                            }
                            XmlNode GridNode = GridXmlX.DocumentElement.SelectSingleNode("//GPDM[@Code='" + code + "']");
                            if (GridNode != null)//存在节点仅修改名称
                            {
                                XmlElement GridElement = (XmlElement)GridNode;
                                GridElement.SetAttribute("Name", name);
                            }
                            else
                            {
                                XmlElement oneGPDM = GridXmlX.CreateElement("GPDM");
                                oneGPDM.SetAttribute("Code", code);
                                oneGPDM.SetAttribute("Name", name);
                                XmlElement twoZFB = GridXmlX.CreateElement("ZFB");
                                //twoZFB.InnerText = "5";
                                oneGPDM.AppendChild(twoZFB);
                                XmlElement twoDFB = GridXmlX.CreateElement("DFB");
                                oneGPDM.AppendChild(twoDFB);
                                XmlElement twoZFY = GridXmlX.CreateElement("ZFY");
                                oneGPDM.AppendChild(twoZFY);
                                XmlElement twoDFY = GridXmlX.CreateElement("DFY");
                                oneGPDM.AppendChild(twoDFY);
                                rootX.AppendChild(oneGPDM);
                            }
                        }
                        GridXmlX.Save(Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App_Data/GPYJ.xml")));
                    }
                }
                else if (!Enum.IsDefined(typeof(ErrorCode), mReturnCode))
                {
                    this.Text = "返回未知异常,异常编号:" + mReturnCode + ";异常说明：" + sReturnMsg;
                    return null;
                }
                else
                {
                    MessageBox.Show(GetDescription(typeof(ErrorCode), mReturnCode));
                }
            }
            else
            {
                this.Text = "返回数据空";
                return null;
            }
            #endregion

            #region 根据配置文件获取集合
            listYJ = new List<APIStock>();
            XmlDocument GridXml = new XmlDocument();
            GridXml.Load(Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App_Data/GPYJ.xml")));
            XmlNode root = GridXml.SelectSingleNode("Root");
            XmlElement rootElement = (XmlElement)root;

            string YYCode = Convert.ToString(ConfigurationManager.AppSettings["GPPamamList"]).Trim();
            string[] YYCodes = YYCode.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string mCode in YYCodes)
            {
                XmlNode GridNode = GridXml.DocumentElement.SelectSingleNode("//GPDM[@Code='" + mCode + "']");
                if (GridNode != null)//存在节点仅修改名称
                {
                    XmlElement oneElement = (XmlElement)GridNode;
                    APIStock stock = new APIStock();
                    stock.SysNo = oneElement.Attributes["Code"].Value;
                    stock.name = oneElement.Attributes["Name"].Value;
                    stock.code = oneElement.Attributes["Code"].Value.Substring(2);
                    stock.ZFB = oneElement.SelectSingleNode("ZFB").InnerText;
                    stock.DFB = oneElement.SelectSingleNode("DFB").InnerText;
                    stock.ZFY = oneElement.SelectSingleNode("ZFY").InnerText;
                    stock.DFY = oneElement.SelectSingleNode("DFY").InnerText;
                    stock.ISYJ = 0;
                    if (!string.IsNullOrEmpty(stock.ZFB) || !string.IsNullOrEmpty(stock.DFB) || !string.IsNullOrEmpty(stock.ZFY) || !string.IsNullOrEmpty(stock.DFY))
                    {
                        stock.ISYJ = 1;//价格预警
                    }
                    listYJ.Add(stock);
                }
            }
            //XmlNodeList ColumnList = rootElement.SelectNodes("GPDM");
            //for (int i = 0; i < ColumnList.Count;i++ )
            //{
            //    XmlElement oneElement = (XmlElement)ColumnList[i];
            //}
            CacheDependency cdd = new CacheDependency(new string[] { Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "GPSmallTools.exe.Config"), Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App_Data/GPYJ.xml") });
            cache.Insert("MGPYJ", listYJ, cdd, System.DateTime.UtcNow.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);//级联缓存(指定配置文件发生修改时，触发缓存清空)
            #endregion

            return listYJ;
        }

        //预警编辑
        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int rows = e.RowIndex;
            int cells = e.ColumnIndex;
            string AttributesName = string.Empty;
            if (cells > 2)
            {
                string sSysNo = Convert.ToString(dataGridView2.Rows[rows].Cells[0].Value);
                string sValue = Convert.ToString(dataGridView2.Rows[rows].Cells[cells].Value);
                switch (cells)
                {
                    case 3:
                        AttributesName = "ZFB";
                        break;
                    case 4:
                        AttributesName = "DFB";
                        break;
                    case 5:
                        AttributesName = "ZFY";
                        break;
                    case 6:
                        AttributesName = "DFY";
                        break;
                }
                if (string.IsNullOrEmpty(sValue))
                {
                    //更新
                    SaveXML(sSysNo, AttributesName, sValue);
                    return;
                }
                double mValue = 0.0;
                if (double.TryParse(sValue, out mValue) == false)
                {
                    this.notifyIconMsg.ShowBalloonTip(3000, "消息提醒", "预警设置有误", ToolTipIcon.Error);
                    dataGridView2.Rows[rows].Cells[cells].Value = "";
                    return;
                }
                if (cells == 3 || cells == 4)
                {
                    if (mValue > 10 || mValue < 0)
                    {
                        this.notifyIconMsg.ShowBalloonTip(3000, "消息提醒", "预警设置有误2", ToolTipIcon.Error);
                        dataGridView2.Rows[rows].Cells[cells].Value = "";
                        return;
                    }
                    //更新
                    SaveXML(sSysNo, AttributesName, sValue);
                }
                else
                {
                    //更新
                    SaveXML(sSysNo, AttributesName, sValue);
                }
            }
        }

        //右键匹配当前行
        private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    dataGridView2.ClearSelection();
                    dataGridView2.Rows[e.RowIndex].Selected = true;
                    if (e.ColumnIndex >= 0)
                    {
                        dataGridView2.CurrentCell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    }
                }
            }
        }

        private void 置顶ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string mCode = Convert.ToString(this.dataGridView2.SelectedRows[0].Cells[0].Value);
            string YYCode = Convert.ToString(ConfigurationManager.AppSettings["GPPamamList"]).Trim();
            if (YYCode.Contains(mCode))
            {
                int a = YYCode.ToUpper().IndexOf(mCode.ToUpper());
                string sT = "";
                string eT = "";
                if (a <= 0)
                {
                    sT = mCode + ",";
                    eT = YYCode.Substring(9);
                }
                else if (a + 9 >= YYCode.Length)
                {
                    sT = mCode + ",";
                    eT = YYCode.Substring(0, a - 1);
                }
                else
                {
                    sT = mCode + ",";
                    eT = YYCode.Substring(0, a) + YYCode.Substring(a + 9);
                }
                YYCode = sT + eT;
                UpdateAppConfig("GPPamamList", YYCode);
                this.notifyIconMsg.ShowBalloonTip(3000, "消息提醒", "置顶成功", ToolTipIcon.Info);
            }
            else
            {
                this.notifyIconMsg.ShowBalloonTip(3000, "消息提醒", "置顶失败", ToolTipIcon.Error);
                return;
            }
            Show(1);
            GetGPYJList();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string mCode = Convert.ToString(this.dataGridView2.SelectedRows[0].Cells[0].Value);
            string YYCode = Convert.ToString(ConfigurationManager.AppSettings["GPPamamList"]).Trim();
            if (YYCode.Contains(mCode))
            {
                int a = YYCode.ToUpper().IndexOf(mCode.ToUpper());
                string sT = "";
                string eT = "";
                if (a <= 0)//首
                {
                    sT = "";
                    eT = YYCode.Substring(9);
                }
                else if (a + 9 >= YYCode.Length)//尾
                {
                    sT = "";
                    eT = YYCode.Substring(0, a - 1);
                }
                else
                {
                    sT = YYCode.Substring(0, a);
                    eT = YYCode.Substring(a + 9);
                }
                YYCode = sT + eT;
                UpdateAppConfig("GPPamamList", YYCode);
                this.notifyIconMsg.ShowBalloonTip(3000, "消息提醒", "删除成功", ToolTipIcon.Info);
            }
            else
            {
                this.notifyIconMsg.ShowBalloonTip(3000, "消息提醒", "删除失败", ToolTipIcon.Error);
                return;
            }
            Show(1);
            GetGPYJList();
        }

        //保存XML节点
        private void SaveXML(string Code, string AttributesName, string AttributesValue)
        {
            XmlDocument GridXml = new XmlDocument();
            GridXml.Load(Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App_Data/GPYJ.xml")));
            XmlNode root = GridXml.SelectSingleNode("Root");
            XmlNode GridNode = GridXml.DocumentElement.SelectSingleNode("//GPDM[@Code='" + Code + "']");
            if (GridNode != null)//存在节点仅修改名称
            {
                GridNode.SelectSingleNode(AttributesName).InnerText = AttributesValue;
            }
            GridXml.Save(Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App_Data/GPYJ.xml")));
        }

        /// <summary>
        /// 价格判断
        /// </summary>
        /// <param name="currentPrice">当前价</param>
        /// <param name="increase">当前涨幅</param>
        /// <param name="stock">预警对象</param>
        /// <returns>提醒信息</returns>
        private string PriceYJ(string currentPrice, string increase, APIStock stock)
        {
            string returnMsg = "";
            //先价格后涨幅
            if (string.IsNullOrEmpty(currentPrice) || string.IsNullOrEmpty(increase) || string.IsNullOrEmpty(stock.SysNo))
            {
                return returnMsg;
            }
            double CurrentPrice = Convert.ToDouble(currentPrice);//当前价
            double Increase = Convert.ToDouble(increase);//当前涨幅
            string sZFB = stock.ZFB;//涨幅百分比
            string sDFB = stock.DFB;//跌幅百分比
            string sZFY = stock.ZFY;//涨幅元
            string sDFY = stock.DFY;//跌幅元
            if(!string.IsNullOrEmpty(sZFY))
            {
                double ZFY = 0.0;
                if (double.TryParse(sZFY, out ZFY) == true)
                {
                    if (CurrentPrice >= ZFY)
                    {
                        returnMsg = "当前股票:" + stock.name + " 的价格已涨到预警价:" + ZFY + "元;当前价格：" + sZFY + "元";
                        return returnMsg;
                    }
                }
            }
            if (!string.IsNullOrEmpty(sDFY))
            {
                double DFY = 0.0;
                if (double.TryParse(sDFY, out DFY) == true)
                {
                    if (CurrentPrice <= DFY)
                    {
                        returnMsg = "当前股票:" + stock.name + " 的价格已跌到预警价:" + DFY + "元;当前价格：" + sDFY + "元";
                        return returnMsg;
                    }
                }
            }
            if (!string.IsNullOrEmpty(sZFB))
            {
                double ZFB = 0.0;
                if (double.TryParse(sZFB, out ZFB) == true)
                {
                    if (Increase >= ZFB)
                    {
                        returnMsg = "当前股票:" + stock.name + " 的幅度已涨到预警涨幅:" + ZFB + "%;当前涨幅：" + Increase + "%";
                        return returnMsg;
                    }
                }
            }
            if (!string.IsNullOrEmpty(sDFB))
            {
                double DFB = 0.0;
                if (double.TryParse(sDFB, out DFB) == true)
                {
                    if (Increase <= -DFB)
                    {
                        returnMsg = "当前股票:" + stock.name + " 的幅度已跌到预警跌幅:" + DFB + "%;当前跌幅：" + sDFY +　"%";
                        return returnMsg;
                    }
                }
            }
            return returnMsg;
        }
        #endregion

        //提醒
        private void cboIsTX_CheckedChanged(object sender, EventArgs e)
        {
            if (cboIsTX.Checked == true)
            {
                UpdateAppConfig("GPIsTX", "1");
                BIsTX = true;
            }
            else
            {
                UpdateAppConfig("GPIsTX", "0");
                BIsTX = false;
                queueMsgList.Clear();
            }
        }
    }

    //股票类
    public class APIStock
    {
        public APIStock()
        {
        }

        public string name { get; set; }//股票名称
        public object code { get; set; }//股票代码
        public string date { get; set; }//日期
        public string openningPrice { get; set; }//开盘价
        public string closingPrice{ get; set; }//昨日收盘价
        public string hPrice{ get; set; }//今日最高价
        public string lPrice{ get; set; }//今日最低价
        public string currentPrice{ get; set; }//当前价
        //public string growth{ get; set; }//价格涨幅
        //public string growthPercent{ get; set; } //价格涨幅比例，单位%
        //public string dealnumber{ get; set; }//成交量股
        public string turnover{ get; set; }//成交金额，单位港币
        //public string hPrice52{ get; set; }//52周最高价
        //public string lPrice52{ get; set; }//52周最低价

        public string competitivePrice{ get; set; }//竞价
        public string auctionPrice{ get; set; }//拍卖价
        public string totalNumber{ get; set; }//成交量
        public string increase { get; set; }//价格涨幅比例，单位%
        public string buyOne{ get; set; }
        public string buyOnePrice{ get; set; }
        public string buyTwo{ get; set; }
        public string buyTwoPrice{ get; set; }
        public string buyThree{ get; set; }
        public string buyThreePrice{ get; set; }
        public string buyFour{ get; set; }
        public string buyFourPrice{ get; set; }
        public string buyFive{ get; set; }
        public string buyFivePrice{ get; set; }
        public string sellOne{ get; set; }
        public string sellOnePrice{ get; set; }
        public string sellTwo{ get; set; }
        public string sellTwoPrice{ get; set; }
        public string sellThree{ get; set; }
        public string sellThreePrice{ get; set; }
        public string sellFour{ get; set; }
        public string sellFourPrice{ get; set; }
        public string sellFive{ get; set; }
        public string sellFivePrice{ get; set; }
        public string minurl{ get; set; }
        public string dayurl{ get; set; }
        public string weekurl{ get; set; }
        public string monthurl{ get; set; }

        //个人添加字段-股票预警(非返回字段)
        public string SysNo { get; set; }
        public string ZFB { get; set; }
        public string DFB { get; set; }
        public string ZFY { get; set; }
        public string DFY { get; set; }
        public int ISYJ { get; set; }//是否有预警 1是
    }

    //大盘类
    public class APIMarket
    {
        public APIMarket()
        {
        }

        public string englishname{ get; set; }
        public string name{ get; set; }
        public string curdot{ get; set; }//52周最低价
        public string curprice{ get; set; }//当前价格
        public string rate{ get; set; }//涨跌率
        public string dealnumber{ get; set; }//成交量（手）
        public string turnover{ get; set; }//成交金额（万元）

        //沪深两市的国外市场
        public string date{ get; set; }//日期
        public string growth{ get; set; }//增长点数
        public string startdot{ get; set; }//开盘点数
        public string closedot{ get; set; }//昨收盘点数
        public string hdot{ get; set; }//今日最高点位
        public string ldot{ get; set; }//今日最低点位
        public string hdot52{ get; set; }//52周最高点位
        public string ldot52{ get; set; }//52周最低点位
    }

    //消息提醒类
    public class ErrorMsg
    {
        public string Code { get; set; }//股票代码
        public int TitleType { get; set; }
        public string Content { get; set; }
        public ToolTipIcon ToolTripType { get; set; }
        public int ShowTime { get; set; }//显示时间毫秒
        public DateTime CreateTime { get; set; }
    }
}
