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

namespace GPSmallTools
{
    public partial class Form1 : Form
    {
        //https://www.showapi.com/api/lookPoint/131/45
        //http://www.21andy.com/new/20090530/1313.html
        //https://api.wmcloud.com/docs/pages/viewpage.action?pageId=1867781
        //http://tushare.org/datayes.html#id2
        public Form1()
        {
            InitializeComponent();
        }
        private ILog log = log4net.LogManager.GetLogger(typeof(Form1));
        private void Form1_Load(object sender, EventArgs e)
        {
            //log.Info("呵呵");
            Show(1);
        }

        public void Show(int fCount)
        {
            List<APIStock> listStock = new List<APIStock>();
            List<APIMarket> listMarket = new List<APIMarket>();
            string sUrl = Convert.ToString(ConfigurationManager.AppSettings["GPURL"]);
            //stockid=sz002673,sh600030,sz002673,sz002673,sz002673,sz002673,sz002673,sz002673,sz002673,sz002673&list=1
            string sParam = "stockid=" + ConfigurationManager.AppSettings["GPPamamList"] + "&list=1";
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
                        for (int i = 0; i < mGSCount; i++)
                        {
                            APIStock stock = new APIStock();
                            JObject jStock = (JObject)jContent["stockinfo"][i];
                            stock.name = Convert.ToString(jStock["name"]).Trim('"');
                            stock.code = Convert.ToString(jStock["code"]).Trim('"').Substring(2); ;
                            stock.date = jStock["time"].ToString().Trim('"');
                            stock.openningPrice = Convert.ToString(jStock["OpenningPrice"]);
                            stock.closingPrice = Convert.ToString(jStock["closingPrice"]);
                            stock.hPrice = Convert.ToString(jStock["hPrice"]);
                            stock.lPrice = Convert.ToString(jStock["lPrice"]);
                            stock.currentPrice = Convert.ToString(jStock["currentPrice"]);
                            //stock.growth = Convert.ToString(jStock["growth"]);
                            //stock.growthPercent = Convert.ToString(jStock["growthPercent"]);
                            //stock.dealnumber = Convert.ToString(jStock["dealnumber"]);
                            stock.turnover = Convert.ToString(Math.Round(Convert.ToDouble(jStock["turnover"].ToString())/100000000, 2, MidpointRounding.AwayFromZero));
                            //stock.hPrice52 = Convert.ToString(jStock["52hPrice"]);
                            //stock.lPrice52 = Convert.ToString(jStock["52lPrice"]);

                            stock.competitivePrice = Convert.ToString(jStock["competitivePrice"]);
                            stock.auctionPrice = Convert.ToString(jStock["auctionPrice"]);
                            stock.totalNumber = Convert.ToString(Math.Round(Convert.ToDouble(jStock["totalNumber"].ToString())/1000000, 2, MidpointRounding.AwayFromZero));
                            string mIncrease = Convert.ToString(Math.Round(Convert.ToDouble(jStock["increase"].ToString()), 2, MidpointRounding.AwayFromZero));//涨跌幅
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
            if(listMarket.Count > 0)
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
                    shZF = "-";
                    shJT = "↓";
                }
                lblSHZS1.Text = apiSHZS.name;
                lblSHZS1.ForeColor = shColor;
                lblSHZS2.Text = shJT + apiSHZS.curdot;
                lblSHZS2.ForeColor = shColor;
                lblSHZS3.Text = shZF + apiSHZS.curprice + " " + shZF + apiSHZS.rate;
                lblSHZS3.ForeColor = shColor;
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
                    szZF = "-";
                    szJT = "↓";
                }
                lblSZZS1.Text = apiSZZS.name;
                lblSZZS1.ForeColor = szColor;
                lblSZZS2.Text = szJT + apiSZZS.curdot;
                lblSZZS2.ForeColor = szColor;
                lblSZZS3.Text = szZF + apiSZZS.curprice + " " + szZF + apiSZZS.rate;
                lblSZZS3.ForeColor = szColor;
            }
            #region 绑定数据到DataGridView
            
            dataGridView1.Columns.Clear();
            //dataGridView1.Columns[4].DataPropertyName
            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
            col1.HeaderText = "股票名称";
            col1.DataPropertyName = "name";
            col1.Name = "name";
            col1.Width = 80;
            col1.ReadOnly = true;
            dataGridView1.Columns.Insert(0, col1); 
            DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn();
            col3.HeaderText = "当前价";
            col3.DataPropertyName = "currentPrice";
            col3.Name = "currentPrice";
            col3.Width = 70;
            col3.ReadOnly = true;
            dataGridView1.Columns.Insert(1, col3);
            DataGridViewTextBoxColumn col18 = new DataGridViewTextBoxColumn();
            col18.HeaderText = "涨幅(%)";
            col18.DataPropertyName = "increase";
            col18.Name = "increase";
            col18.Width = 75;
            col18.ReadOnly = true;
            dataGridView1.Columns.Insert(2, col18);
            DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
            col2.HeaderText = "股票代码";
            col2.DataPropertyName = "code";
            col2.Name = "code";
            col2.Width = 80;
            col2.ReadOnly = true;
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
            #endregion
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

        //刷新
        private void btnSelect_Click(object sender, EventArgs e)
        {
            FormMessageBox.Show(LoadMode.Warning, "呵呵呵呵");
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
                this.Width = 919;
                dataGridView1.Width = 893;
                btnRight.Text = "《";
            }
            else
            {
                this.Width = 280;
                dataGridView1.Width = 270;
                btnRight.Text = "》";
            }
        }

        //上下扩展
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (btnDown.Text.Trim() == "︾")
            {
                this.Height = 630;
                btnDown.Text = "︽";
            }
            else
            {
                this.Height = 355;
                btnDown.Text = "︾";
            }
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
            string YYCode = Convert.ToString(ConfigurationManager.AppSettings["GPPamamList"]);
            if (YYCode.Contains(mCode))
            {
                MessageBox.Show("添加成功");
            }
            else
            {
                YYCode = YYCode + "," + mCode;
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
                this.dataGridView1.Rows[e.RowIndex].Cells["increase"].Style.Font = new Font("宋体", 12);
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
}
