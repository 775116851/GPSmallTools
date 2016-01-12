using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace GPSmallTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<APIStock> listStock = new List<APIStock>();
            List<APIMarket> listMarket = new List<APIMarket>();
            string sUrl = Convert.ToString(ConfigurationSettings.AppSettings["GPURL"]);
            string sParam = "stockid=sz002673,sh600030,sz002673,sz002673,sz002673,sz002673,sz002673,sz002673,sz002673,sz002673&list=1";
            string sMessage = HttpGet(sUrl, sParam);
            if (!string.IsNullOrEmpty(sMessage))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(sMessage);
                string sReturnCode = Convert.ToString(jo["errNum"]);
                string sReturnMsg = Convert.ToString(jo["errMsg"]);
                if (string.IsNullOrEmpty(sReturnCode))
                {
                    MessageBox.Show("请求异常");
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
                            stock.name = Convert.ToString(jStock["name"]);
                            stock.code = Convert.ToString(jStock["code"]);
                            stock.date = Convert.ToString(jStock["date"]) + " " + Convert.ToString(jStock["time"]);
                            stock.openningPrice = Convert.ToString(jStock["openningPrice"]);
                            stock.closingPrice = Convert.ToString(jStock["closingPrice"]);
                            stock.hPrice = Convert.ToString(jStock["hPrice"]);
                            stock.lPrice = Convert.ToString(jStock["lPrice"]);
                            stock.currentPrice = Convert.ToString(jStock["currentPrice"]);
                            stock.growth = Convert.ToString(jStock["growth"]);
                            stock.growthPercent = Convert.ToString(jStock["growthPercent"]);
                            stock.dealnumber = Convert.ToString(jStock["dealnumber"]);
                            stock.turnover = Convert.ToString(jStock["turnover"]);
                            stock.hPrice52 = Convert.ToString(jStock["52hPrice"]);
                            stock.lPrice52 = Convert.ToString(jStock["52lPrice"]);
                            listStock.Add(stock);
                        }
                        JObject jMarket = (JObject)jContent["market"];
                        APIMarket mSH = new APIMarket();
                        mSH.name = Convert.ToString(jMarket["shanghai"]["name"]);
                        mSH.curdot = Convert.ToString(jMarket["shanghai"]["curdot"]);
                        mSH.curprice = Convert.ToString(jMarket["shanghai"]["curprice"]);
                        mSH.rate = Convert.ToString(jMarket["shanghai"]["rate"]);
                        mSH.dealnumber = Convert.ToString(jMarket["shanghai"]["dealnumber"]);
                        mSH.turnover = Convert.ToString(jMarket["shanghai"]["turnover"]);
                        APIMarket mSZ = new APIMarket();
                        mSZ.name = Convert.ToString(jMarket["shenzhen"]["name"]);
                        mSZ.curdot = Convert.ToString(jMarket["shenzhen"]["curdot"]);
                        mSZ.curprice = Convert.ToString(jMarket["shenzhen"]["curprice"]);
                        mSZ.rate = Convert.ToString(jMarket["shenzhen"]["rate"]);
                        mSZ.dealnumber = Convert.ToString(jMarket["shenzhen"]["dealnumber"]);
                        mSZ.turnover = Convert.ToString(jMarket["shenzhen"]["turnover"]);
                        listMarket.Add(mSH);
                        listMarket.Add(mSZ);
                    }
                }
                else if (!Enum.IsDefined(typeof(ErrorCode), mReturnCode))
                {
                    MessageBox.Show("返回未知异常,异常编号:" + mReturnCode + ";异常说明：" + sReturnMsg);
                    return;
                }
                else
                {
                    MessageBox.Show(GetDescription(typeof(ErrorCode), mReturnCode));
                }
            }
            else
            {
                MessageBox.Show("返回数据空");
                return;
            }
            dataGridView1.DataSource = listStock;
        }

        //接口请求数据
        public string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            //添加header
            request.Headers.Add("apikey", Convert.ToString(ConfigurationSettings.AppSettings["GPKey"]));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            //string retString = myStreamReader.ReadToEnd();
            string StrDate = "";
            string retString = "";
            while ((StrDate = myStreamReader.ReadLine()) != null)
            {
                retString += StrDate + "\r\n";
            }
            myStreamReader.Close();
            myResponseStream.Close();

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
        
    }

    //股票类
    public class APIStock
    {
        public APIStock()
        {
        }

        public string name;//股票名称
        public string code;//股票代码
        public string date;//日期
        public string openningPrice;//开盘价
        public string closingPrice;//昨日收盘价
        public string hPrice;//今日最高价
        public string lPrice;//今日最低价
        public string currentPrice;//当前价
        public string growth;//价格涨幅
        public string growthPercent; //价格涨幅比例，单位%
        public string dealnumber;//成交量股
        public string turnover;//成交金额，单位港币
        public string hPrice52;//52周最高价
        public string lPrice52;//52周最低价

        public string competitivePrice;
        public string auctionPrice;
        public string totalNumber;
        public string increase;
        public string buyOne;
        public string buyOnePrice;
        public string buyTwo;
        public string buyTwoPrice;
        public string buyThree;
        public string buyThreePrice;
        public string buyFour;
        public string buyFourPrice;
        public string buyFive;
        public string buyFivePrice;
        public string sellOne;
        public string sellOnePrice;
        public string sellTwo;
        public string sellTwoPrice;
        public string sellThree;
        public string sellThreePrice;
        public string sellFour;
        public string sellFourPrice;
        public string sellFive;
        public string sellFivePrice;
        public string minurl;
        public string dayurl;
        public string weekurl;
        public string monthurl;
    }

    //大盘类
    public class APIMarket
    {
        public APIMarket()
        {
        }

        public string englishname;
        public string name;
        public string curdot;//52周最低价
        public string curprice;//当前价格
        public string rate;//涨跌率
        public string dealnumber;//成交量（手）
        public string turnover;//成交金额（万元）

        //沪深两市的国外市场
        public string date;//日期
        public string growth;//增长点数
        public string startdot;//开盘点数
        public string closedot;//昨收盘点数
        public string hdot;//今日最高点位
        public string ldot;//今日最低点位
        public string hdot52;//52周最高点位
        public string ldot52;//52周最低点位
    }
}
