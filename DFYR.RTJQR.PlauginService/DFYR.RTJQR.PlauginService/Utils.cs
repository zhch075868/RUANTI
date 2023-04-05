using Kingdee.BOS;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Msg;
using Kingdee.BOS.JSON;
using Kingdee.BOS.Msg;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Orm.Drivers;
using Kingdee.BOS.ServiceHelper;
using Kingdee.BOS.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace DFYR.RTJQR.PlauginService
{
    [Kingdee.BOS.Util.HotUpdate, Description("工具类")]
    public class Utils
    {
        
        public static string filePath = "\\App_Data\\DataBase\\JQR_ReadData.xml";//HZ_ReadData
        public static XDocument document = getXmlFile(filePath);
        public static IEnumerable<XElement> elements;//该表单id下的所有元素

        //免密等录配置
        public static string appId = GetValueByEleName("appId", "ConstantConfig");
        public static string appSecret = GetValueByEleName("appSecret", "ConstantConfig");
        public static string loginserviceUrl = GetValueByEleName("loginserviceUrl", "ConstantConfig");
        public static string SaveServiceUrl = GetValueByEleName("SaveServiceUrl", "ConstantConfig");
        public static string dbid = GetValueByEleName("dbid", "ConstantConfig");
        public static string unAuditUrl = GetValueByEleName("unAuditUrl", "ConstantConfig");
        public static string deleteUrl = GetValueByEleName("deleteUrl", "ConstantConfig");
        public static string batchSaveUrl = GetValueByEleName("BatchSaveUrl", "ConstantConfig");
        public static string fileUrl = GetValueByEleName("fileUrl", "ConstantConfig");

        public static string OAHostUrl = GetValueByEleName("OAHostUrl", "ConstantConfig");
        //OA参数
        public static string OAAppid = GetValueByEleName("OAAppid", "ConstantConfig");
        public static string OAPublicKey = GetValueByEleName("OAPublicKey", "ConstantConfig");//MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxyeCS+pg7Dk73VuuDVgCSxvCyS9V/OQkDKoIyaBYrJBo35jJtRRn80Jcdr7s2Yjd9ent+ZMwygdc9URy3sc8MirJY9vkyRIca8rSZoc3CMIPrXbPb19FLuS6A7x4dnTzZy7CScu+defXUvzm3p6Fxf2yKLx38HFTNt8ULEwjQUracLOwuCitLuyYUoJK0pVErWpFhpn02JD7zbj5N3D6MZ0eIDz0VAA+L7EPvZCGSOhU3ubaNL+EmfC9MoXkcGoskFqHA+jOeClgMm70T5fpbA2evbQorJh4Baz+rFcTaBoZT3q6vhJM2cjTaevpc/cWpXqAl9dSs1RS4VzWf+/tMwIDAQAB
        public static string OASecret = GetValueByEleName("OASecret", "ConstantConfig");//bugxHyhmIgBymaULd9BIL+eAAdPGdbSD36QV1SRfY3Cm/JjlAk3MvUvRtdnRaLg98vp3cGXRidHHBKuF4391SSUB8UazzZ7ZxB7Y7/nJSMdVBS4IhEH2tMJmJWhlFkoqsG+N6lX+L7D+fHtyX/TXmczzupUffnKz9cmbiid9+124Tz1tKuPF7cfShNlQB5x/p5Gsz4ZjYTQUoCCNq2WBAYRZlsTbCO1gqjkXf4OKYHselboyBZPpsF621Fpmb2t0/qS3POPzxmhqlJZvn0PXLCoP/oWrzRYxH4tc7LXrR1JcAsanlaJBtJp0pB6VIngaKN4fAZdeAehtjjm6pxQRKg==

        public static string pushOAGYSurl = OAHostUrl + GetValueByEleName("pushOAGYSurl", "ConstantConfig");
        public static string pushCGRKurl = OAHostUrl + GetValueByEleName("pushCGRKurl", "ConstantConfig");
        public static string getTokenUrl = OAHostUrl + GetValueByEleName("getTokenUrl", "ConstantConfig");
        public static string pushAddWF = OAHostUrl + GetValueByEleName("pushWFid", "ConstantConfig");
        public static string pushKHurl = OAHostUrl + GetValueByEleName("pushKHurl", "ConstantConfig");
        public static string pushWLurl = OAHostUrl + GetValueByEleName("pushWLurl", "ConstantConfig");
        public static string pushFYXMurl = OAHostUrl + GetValueByEleName("pushFYXMurl", "ConstantConfig");
        public static string pushJMBurl = OAHostUrl + GetValueByEleName("pushJMBurl", "ConstantConfig");
        public static string pushYWYurl = OAHostUrl + GetValueByEleName("pushYWYurl", "ConstantConfig");
        public static string pushCGDDurl = OAHostUrl + GetValueByEleName("pushCGDDurl", "ConstantConfig");
        public static string pushBBurl = OAHostUrl + GetValueByEleName("pushBBurl", "ConstantConfig");
        public static string pushYWZurl = OAHostUrl + GetValueByEleName("pushYWZurl", "ConstantConfig");
        public static string pushPerson = GetValueByEleName("pushPerson", "ConstantConfig");//正式环境17/测试55
        public static string pushFileUrl = OAHostUrl+GetValueByEleName("pushFileUrl", "ConstantConfig");

        static CookieContainer Cookie = new CookieContainer();
        /// <summary>
        /// htpp post请求 发送json格式的参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string sendPost(string url, string param)
        {
            //WriteLog(param);
            try
            {
                string result = string.Empty;
                HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
                req.Method = "POST";
                req.Timeout = 10000 * 60;//设置请求超时时间，单位为毫秒
                req.ContentType = "application/json";
                req.CookieContainer = Cookie;
                byte[] data = Encoding.UTF8.GetBytes(param);
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                    //WriteLog(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new KDException("", ex.Message);
            }
        }

        /// <summary>
        /// 免密登录的方法
        /// </summary>
        public static string LoginByAppSecret(Kingdee.BOS.Context ctx)
        {
            JSONObject param = new JSONObject();
            JSONArray paramArry = new JSONArray();
            int lcId = 2052;
            paramArry.Add(ctx.DBId);
            paramArry.Add(ctx.UserName);
            paramArry.Add(appId);
            paramArry.Add(appSecret);
            paramArry.Add(lcId);

            param.Add("parameters", paramArry);

            string reulst = sendPost(loginserviceUrl, param.ToString());
            return reulst;
        }

        /// <summary>
        /// 按元素名称获取元素值
        /// </summary>
        /// <param name="eleName">元素名称</param>
        /// <param name="pathName">路径下，例如：configs.config.summary</param>
        /// <returns></returns>
        public static string GetValueByEleName(string eleName, string pathName)
        {
            IEnumerable<XElement> enumerable = getXmlEles(pathName);
            foreach (XElement item in enumerable)
            {
                //foreach (XElement item1 in item.Elements())
                //只寻找下一层元素
                if (item.Name.LocalName.Equals(eleName))//找元素名称为eleName
                {
                    return item.Value;//返元素节点的值
                }
            }
            return "";
            //Console.ReadKey();
        }


        /// <summary>
        /// 公共：获取节点路径中末节点中的所有元素集
        /// </summary>
        /// <param name="pathName">获取节点路径</param>
        /// <returns></returns>
        public static IEnumerable<XElement> getXmlEles(string pathName)
        {

            //将XML文件加载进来
            //XDocument document = getXmlFile(filePath);
            //获取到XML的根元素进行操作
            //XElement root = document.Root;
            XElement root = document.Root;
            IEnumerable<XElement> eles = root.Elements();
            //获取根元素下objectName标签的元素
            if (pathName.Contains("."))
            {
                string objectName = "";
                string[] pathStr = pathName.Split('.');
                objectName = pathStr[0].ToString();
                for (int i = 0; i < pathStr.Count(); i++)
                {
                    objectName = pathStr[i].ToString();
                    foreach (XElement item in eles)
                    {
                        if (item.Name.LocalName.Equals(objectName))
                        {
                            eles = item.Elements();
                            break;
                        }
                    }
                }
                return eles;
            }
            else
            {
                return eles.Elements();
            }
        }

        /// <summary>
        /// 公共：获取xml文件
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <returns></returns>
        public static XDocument getXmlFile(string filePath)
        {
            try
            {
                //将XML文件加载进来
                XDocument document = XDocument.Load(System.Web.HttpRuntime.AppDomainAppPath + filePath);
                return document;
            }
            catch (IOException e)
            {
                throw new KDException("", "xml读取出现错误");
            }
        }
        /// <summary>
        /// 判断是否保存成功
        /// </summary>
        /// <param name="resultsJsonStr"></param>
        /// <returns></returns>
        public static bool isSave(string resultsJsonStr)
        {
            try
            {
                bool flag = false;
                JSONObject resultsJson = JSONObject.Parse(resultsJsonStr);
                JSONObject result = resultsJson["Result"] as JSONObject;
                JSONObject responseStatus = result["ResponseStatus"] as JSONObject;
                flag = Convert.ToBoolean(responseStatus["IsSuccess"]);
                return flag;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否需要登录
        /// </summary>
        /// <param name="resultsJsonStr"></param>
        /// <returns></returns>
        public static bool isLoginSuccess(string resultsJsonStr)
        {
            try
            {
                bool flag = true;
                JSONObject resultsJson = JSONObject.Parse(resultsJsonStr);
                JSONObject result = resultsJson["Result"] as JSONObject;
                JSONObject responseStatus = result["ResponseStatus"] as JSONObject;
                string msgCode = Convert.ToString(responseStatus["MsgCode"]);
                if (msgCode.Equals("1"))
                {
                    flag = false;
                }
                return flag;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// 获取错误的信息
        /// </summary>
        /// <param name="resultsJsonStr"></param>
        /// <returns></returns>
        public static string getErroInfo(string resultsJsonStr)
        {
            try
            {
                JSONObject resultsJson = JSONObject.Parse(resultsJsonStr);
                JSONObject result = resultsJson["Result"] as JSONObject;
                JSONObject responseStatus = result["ResponseStatus"] as JSONObject;
                JSONArray errors = responseStatus["Errors"] as JSONArray;
                StringBuilder message = new StringBuilder();
                foreach (JSONObject error in errors)
                {
                    message.Append(Convert.ToString(error["Message"]));
                }
                return message.ToString();
            }
            catch (Exception e)
            {
                return "【返回未知错误】 : " + e.Message;
            }
        }

        /// <summary>
        /// 获取错误的信息
        /// </summary>
        /// <param name="resultsJsonStr"></param>
        /// <returns></returns>
        public static string getId(string resultsJsonStr)
        {
            try
            {
                string Id = string.Empty;
                JSONObject resultsJson = JSONObject.Parse(resultsJsonStr);
                JSONObject result = resultsJson["Result"] as JSONObject;
                Id = Convert.ToString(result["Id"]);
                return Id;
            }
            catch (Exception e)
            {
                throw new KDException("", "【返回未知错误】：" + e.Message);
            }
        }


        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="strLog"></param>
        public static void WriteLog(string strLog)
        {
            string sFilePath = MapPath("App_Data/DataBase/");
            string sFileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            sFileName = sFilePath + sFileName; //文件的绝对路径
            if (!Directory.Exists(sFilePath))//验证路径是否存在
            {
                Directory.CreateDirectory(sFilePath);
                //不存在则创建
            }
            FileStream fs;
            StreamWriter sw;
            if (File.Exists(sFileName))
            //验证文件是否存在，有则追加，无则创建
            {
                fs = new FileStream(sFileName, FileMode.Append, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(sFileName, FileMode.Create, FileAccess.Write);
            }
            sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss   ") + "INFO:==========> " + strLog);
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        private static string MapPath(string strPath)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.TrimStart('\\');
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }

        ///// <summary>
        ///// 向日志表写入日志
        ///// </summary>
        ///// <param name="formid"></param>
        ///// <param name="txt"></param>
        ///// <param name="logType"></param>
        ///// <param name="isSuccess"></param>
        ///// <param name="billNo"></param>
        ///// <returns></returns>
        //public static bool WriteBillLog(Context ctx, LogBean logBean)
        //{
        //    try
        //    {
        //        JSONObject Parameters = new JSONObject();
        //        //业务对象Id 
        //        string logFormid = "TXGM_LogInfo";//拆分中间表

        //        Parameters.Add("formid", logFormid);
        //        JSONObject model = new JSONObject();
        //        JSONObject data = new JSONObject();//数据json
        //        //data.Add("FId", logBean.fid);
        //        data.Add("FCreateDate", DateTime.Now.ToString());
        //        data.Add("FLogType", logBean.logType);
        //        data.Add("FIsuccess", logBean.isSuccess);
        //        data.Add("FNumber", logBean.billNo);
        //        data.Add("FSendPar", logBean.sendTxt);
        //        data.Add("FRetPar", logBean.rettxt);
        //        data.Add("FOrgId", parmaterSplcing(logBean.OrgNumber));
        //        model.Add("Model", data);
        //        Parameters.Add("data", model);
        //    post:
        //        string reulst = sendPost(SaveServiceUrl, Parameters.ToString());
        //        if (!isLoginSuccess(reulst))
        //        {
        //            LoginByAppSecret(ctx);
        //            goto post;
        //        }
        //        else
        //        {
        //            if (isSave(reulst))
        //            {
        //                Logger.Info("", "写日志出错啦！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！");
        //            }
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Info("", "写日志出错啦！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！");
        //    }
        //    return true;
        //}

        /// <summary>
        /// 冗余代码处理方法1
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JSONObject parmaterSplcing(string value)
        {
            JSONObject retjson = new JSONObject();
            retjson.Add("FNUMBER", value);
            return retjson;
        }

        /// <summary>
        /// 冗余代码处理方法2
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string getBaseDataNumber(object obj)
        {
            string retStr = string.Empty;
            DynamicObject dyObj = obj as DynamicObject;
            try
            {
                if (obj != null)
                {
                    retStr = Convert.ToString(dyObj["Number"]);
                }
            }
            catch
            {
                retStr = retStr = Convert.ToString(dyObj["FNumber"]);
            }
            return retStr;
        }

        /// <summary>
        /// 发送金蝶云消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="messageInfo"></param>
        /// <param name="requestId"></param>
        /// <param name="formId"></param>
        /// <param name="billId"></param>
        public static void saveMeg(Context ctx, string title, string messageInfo, long requestId, string formId, string billId)
        {

            string messageId = SequentialGuid.NewGuid().ToString();
            Message msg = new DynamicObject(Message.MessageDynamicObjectType);
            msg.MessageId = messageId;
            msg.Title = title;
            msg.Content = messageInfo;
            msg.CreateTime = DateTime.Now;
            msg.SenderId = requestId;
            msg.ObjectTypeId = formId;
            msg.KeyValue = billId;
            msg.ReceiverId = requestId;
            msg.MsgType = MsgType.CommonMessage;
            IDbDriver driver = new OLEDbDriver(ctx);
            var dataManager = Kingdee.BOS.Orm.DataManagerUtils.GetDataManager(Message.MessageDynamicObjectType, driver);
            dataManager.Save(msg.DataEntity);
        }

        /// <summary>
        /// 日志表插入数据
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="sendPar"></param>
        /// <param name="retPar"></param>
        /// <param name="type"></param>
        public static void logTableInfo(Context ctx, string startTime, string endTime, string sendPar, string retPar, string type)
        {
            try
            {
                string sql = string.Format("insert into t_dfyr_logtable values('{0}','{1}','{2}','{3}','{4}','{5}')",
                                    DateTime.Now.ToString("yyyy-MM-dd mm:hh:ss"), startTime, endTime, sendPar, retPar, type);
                DBUtils.Execute(ctx, sql);
            }
            catch (Exception e)
            {
                return;
            }
        }

        /// <summary>
        /// json字符串将属性值中的英文双引号变成中文双引号
        /// </summary>
        /// <param name="strJson">json字符串</param>
        /// <returns></returns>
        public static string JsonReplaceSign(string strJson)
        {
            //获取每个字符
            char[] temp = strJson.ToCharArray();
            //获取字符数组长度
            int n = temp.Length;
            //循环整个字符数组
            for (int i = 0; i < n; i++)
            {
                //查找json属性值（:+" ）
                if (temp[i] == ':' && temp[i + 1] == '"')
                {
                    //循环属性值内的字符（：+2 推算到value值）
                    for (int j = i + 2; j < n; j++)
                    {
                        //判断是否是英文双引号
                        if (temp[j] == '"')
                        {
                            //排除json属性的双引号
                            if (temp[j + 1] != ',' && temp[j + 1] != '}')
                            {
                                //替换成中文双引号
                                temp[j] = '”';
                            }
                            else if (temp[j + 1] == ',' || temp[j + 1] == '}')
                            {
                                break;
                            }
                        }
                        else if (temp[j] == ':' && temp[j - 1] == '*')
                        {
                            temp[j] = ' ';
                        }
                        else if (true)
                        {
                            // 要过虑其他字符，继续添加判断就可以
                        }
                    }
                }
            }
            return new String(temp);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns>返回32位字符串</returns>
        public static string StringToMD5Hash(string inputString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 但觉
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string PostUrl(string url, string postData)
        {
            string result = "";
            try
            {
                WriteLog("保存修改数据参数：" + postData);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.Timeout = 10000 * 60;//请求超时时间
                byte[] data = Encoding.UTF8.GetBytes(postData);
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                    WriteLog("保存修改数据返回：" + result);
                }
            }
            catch (Exception e) { }
            return result;
        }
        /// <summary>
        /// 流程
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string wkPostUrl(string url, string postData,string OAUserid)
        {
            Utils.WriteLog("流程参数"+postData +"提交人id："+OAUserid);
            string result = "";
            string publickey = RSAConvert.RSAPublicKeyJava2DotNet(OAPublicKey);
            string userid = RSAConvert.RSAEncrypt(publickey, OAUserid);
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.Timeout = 10000 * 60;//请求超时时间
                req.Headers.Add("appid", OAAppid);
                req.Headers.Add("userid", userid);
                req.Headers.Add("token", getToken());
                byte[] data = Encoding.UTF8.GetBytes(postData);
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                    Utils.WriteLog("流程返回" + result);
                }
            }
            catch (Exception e) { }
            return result;
        }

        public static string token = "";

       /// <summary>
       /// 获取token
       /// </summary>
       /// <returns></returns>
        public static string getToken()
        {
            string result = "";
            if (!token.Equals(""))
            {
                result = token;
                return result;
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(getTokenUrl);
            req.Method = "POST";
            req.Headers.Add("appid", OAAppid);
            req.Headers.Add("secret", OASecret);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            Stream stream = resp.GetResponseStream();
            //获取内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            JSONObject resultJson = JSONObject.Parse(result);
            if (resultJson.GetInt("code") == 0)
            {
                result = resultJson.GetString("token");
                token = result;
            }
            return result;
        }

        public static string getPersonOAid(Context ctx,string erpNumber)
        {
            try{
                string sql = string.Format(@"
                                SELECT fnumber, F_PYEO_TEXT_OAID xqr 
                                from T_HR_EMPINFO where FNUMBER = '{0}'",erpNumber);
                return DBUtils.ExecuteDynamicObject(ctx, sql)[0]["xqr"].ToString();
            }
            catch(Exception e)
            {
                return "";
            }
        }

        //附件相关方法********************************************

        public static string UploadRequest(string url,string FileName)
        {
            string returnValue = string.Empty;
            string publickey = RSAConvert.RSAPublicKeyJava2DotNet(OAPublicKey);
            string userid = RSAConvert.RSAEncrypt(publickey, "1");//管理员上传
            // 时间戳，用做boundary
            string timeStamp = DateTime.Now.Ticks.ToString("x");

            //根据uri创建HttpWebRequest对象
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(pushFileUrl));
            httpReq.Method = "POST";
            httpReq.AllowWriteStreamBuffering = false; //对发送的数据不使用缓存
            httpReq.Timeout = 300000;  //设置获得响应的超时时间（300秒）
            httpReq.ContentType = "multipart/form-data; boundary=" + timeStamp;
            //请求头部信息
            httpReq.Headers.Add("appid", OAAppid);
            httpReq.Headers.Add("token", getToken());
            httpReq.Headers.Add("userid", userid);
            //文件
            //FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            string kdurl = url;//"http://192.168.132.21/k3cloud/FileUpLoadServices/Download.aspx?fileId=226028f286474af89dab7a07d4c0ff3e&token=b5563950-2240-4d7b-a4f1-20994530f47e&nail=1";
            Stream fileStream = DownloadData(kdurl);
            FileStream memoryStream = StreamToMemoryStream(fileStream, Path.GetFileName(FileName));
            BinaryReader binaryReader = new BinaryReader(memoryStream);


            //头信息
            string boundary = "--" + timeStamp;
            string dataFormat = boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\nContent-Type:application/octet-stream\r\n\r\n";
            string header = string.Format(dataFormat, "file", Path.GetFileName(FileName));
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(header);

            //结束边界
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + timeStamp + "--\r\n");
            long length = memoryStream.Length + postHeaderBytes.Length + boundaryBytes.Length;

            httpReq.ContentLength = length;//请求内容长度

            try
            {
                //每次上传4k
                int bufferLength = 4096;
                byte[] buffer = new byte[bufferLength];

                //已上传的字节数
                long offset = 0;
                int size = binaryReader.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();

                //发送请求头部消息
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;
                    size = binaryReader.Read(buffer, 0, bufferLength);
                }

                //添加尾部边界
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();

                //获取服务器端的响应
                using (HttpWebResponse response = (HttpWebResponse)httpReq.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    returnValue = readStream.ReadToEnd();

                    response.Close();
                    readStream.Close();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                fileStream.Close();
                binaryReader.Close();
            }
            return returnValue;
        }
        /// <summary>
        /// 获取金蝶云附件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Stream DownloadData(string url)
        {
            //FileStream pFileStream = null;
            //try
            //{
            List<byte> content = new List<byte>();
            byte[] buffer = new byte[1024 * 1024];
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
            httpWebRequest.Method = "GET";
            return httpWebRequest.GetResponse().GetResponseStream();
            //using (Stream responseStream = httpWebRequest.GetResponse().GetResponseStream())
            //{
            //    int size;
            //    while ((size = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            //    {
            //        byte[] tempBuffer = new byte[size];
            //        Array.Copy(buffer, tempBuffer, size);
            //        content.AddRange(tempBuffer);


            //    }
            //}
            //pFileStream = new FileStream("E:/123.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //pFileStream.Write(content.ToArray(), 0, content.ToArray().Length);

            //}
            //catch
            //{

            //}
            //finally
            //{
            //    if (pFileStream != null)
            //    {
            //        pFileStream.Close();
            //    }

            //}        
            //return pFileStream;
        }
        /// <summary>
        /// web获取的流转文件流
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static FileStream StreamToMemoryStream(Stream stream,string filleName)
        {
            FileStream memoryStream = new FileStream(string.Format(System.Web.HttpRuntime.AppDomainAppPath+"\\tempfilePath\\{0}", filleName), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //将基础流写入内存流
            const int bufferLength = 1024;
            byte[] buffer = new byte[bufferLength];
            int actual = stream.Read(buffer, 0, bufferLength);
            while (actual > 0)
            {
                // 读、写过程中，流的位置会自动走。
                memoryStream.Write(buffer, 0, actual);
                actual = stream.Read(buffer, 0, bufferLength);
            }
            memoryStream.Position = 0;

            return memoryStream;
        }

        /// <summary>
        /// 获取附件id
        /// </summary>
        /// <param name="billsId">单据id</param>
        /// <param name="formId">表单唯一标识</param>
        /// <param name="ctx">上下文对象</param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> GetUrl(string billsId, string formId, Context ctx)
        {
            
            List<Dictionary<string, string>> retArray = new List<Dictionary<string, string>>();
            if (formId.Equals("CRM_Contract") || formId.Equals("CRM_XContract"))
            {
                string sql = string.Format("select F_PYEO_ATTACHMENTS from T_CRM_CONTRACT where FID = {0}", billsId);
                DynamicObjectCollection attcArray = DBUtils.ExecuteDynamicObject(ctx, sql);
                if (attcArray.Count != 0)
                {
                    string attInfoStr = Convert.ToString(attcArray[0]["F_PYEO_ATTACHMENTS"]);
                    JSONArray attIinfos = JSONArray.Parse(attInfoStr);
                    foreach (object attInfoObject in attIinfos)
                    {
                        Dictionary<string, object> attInfo = attInfoObject as Dictionary<string, object>;
                        Dictionary<string, string> obj = new Dictionary<string, string>();
                        //string extName = Convert.ToString(attInfo["FileName"]);//获取该附件的后缀名
                        string attachmentName = Convert.ToString(attInfo["FileName"]);//文件名,带后缀名
                        string fileIdStr = Convert.ToString(attInfo["ServerFileName"]);//文件ID：0e6737788b084b229e808230b183b6ce

                        string url = fileUrl;//"http://10.1.255.223/K3Cloud/FileUpLoadServices";//BusinessDataServiceHelper.GetFileServerUrl(ctx);// 获取文件服务器地址
                        if (url.IsNullOrEmptyOrWhiteSpace())
                        {
                            continue;
                        }
                        if (!url.EndsWith("/"))
                        {
                            url += "/";
                        }

                        string dbIdSyr = ctx.DBId;//DBId
                        string userToken = ctx.UserToken;//获取token
                        // 调用文件服务的下载文件服务，生成一个url资源，用户点击此URL时时，可以下载文件
                        string fileurl = string.Format("{0}download.aspx?fileId={1}&dbId={2}&t={3}&token={4}", url, fileIdStr, dbIdSyr, DateTime.Now.Ticks, userToken);
                        obj.Add(attachmentName, fileurl);
                        retArray.Add(obj);
                    }
                }
            }
            OQLFilter ofilter = OQLFilter.CreateHeadEntityFilter(string.Format("FINTERID = '{0}' AND FBILLTYPE = '{1}'", billsId, formId));
            DynamicObject[] attachmentObject = BusinessDataServiceHelper.Load(ctx, "BOS_Attachment", null, ofilter);
            foreach (DynamicObject attObj in attachmentObject)
            {
                Dictionary<string, string> obj = new Dictionary<string, string>();
                string extName = Convert.ToString(attObj["ExtName"]);//获取该附件的后缀名
                string attachmentName = Convert.ToString(attObj["AttachmentName"]);//文件名,带后缀名
                string fileIdStr = Convert.ToString(attObj["FileId"]);//文件ID：0e6737788b084b229e808230b183b6ce

                string url = fileUrl;//"http://10.1.255.223/K3Cloud/FileUpLoadServices";//BusinessDataServiceHelper.GetFileServerUrl(ctx);// 获取文件服务器地址
                if (url.IsNullOrEmptyOrWhiteSpace())
                {
                    continue;
                }
                if (!url.EndsWith("/"))
                {
                    url += "/";
                }

                string dbIdSyr = ctx.DBId;//DBId
                string userToken = ctx.UserToken;//获取token
                // 调用文件服务的下载文件服务，生成一个url资源，用户点击此URL时时，可以下载文件
                string fileurl = string.Format("{0}download.aspx?fileId={1}&dbId={2}&t={3}&token={4}", url, fileIdStr, dbIdSyr, DateTime.Now.Ticks, userToken);
                obj.Add(attachmentName, fileurl);
                retArray.Add(obj);
            }
            

            return retArray;
        }
        //附件相关方法********************************************
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static DynamicObject GetUser(Context ctx, string userID)
        {
            try
            {
                if (userID.IsNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }
                OQLFilter filter = OQLFilter.CreateHeadEntityFilter(string.Format("FUSERID={0}", userID));
                return BusinessDataServiceHelper.Load(ctx, FormIdConst.SEC_User, null, filter).FirstOrDefault();
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("服务器出现异常，请联系相关人员！");
                sb.AppendLine(e.ToString());
                throw new KDCancelException(sb.ToString());

            }
        }

        /// <summary>
        /// 判断是否登录
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool isCloudSSO(Context context)
        {
            if (!context.IsNullOrEmptyOrWhiteSpace())
            {
                return true;
            }
            return false;
        }
    }
}
