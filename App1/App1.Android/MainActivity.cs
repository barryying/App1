using System;

using Android.App;
using Android.Content.PM;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using Android.Content;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Xamarin.Forms;
using Android.Views;

namespace App1.Droid
{
    [Activity(Label = "App1", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        static readonly string cuid = "fkjaxF2EKjiDTigVWFmaCPbk";//用户唯一识别码，官方建议使用MAC地址
        int count = 1;
        static Android.Webkit.WebView webview;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            ////隐藏标题栏(须放在SetContentView函数之前)  
            //this.RequestWindowFeature(WindowFeatures.NoTitle);
            ////设置全屏  
            //this.Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.mypage);
                        
            //根据ID查找button
            var button1 = FindViewById<Android.Widget.Button>(Resource.Id.button1);
            //添加Click事件
            button1.Click += delegate
            {
                button1.Text = string.Format("这是第{0} 单击!", count++);
            };


            //根据ID查找button
            var button2 = FindViewById<Android.Widget.Button>(Resource.Id.button2);
            //添加Click事件
            button2.Click += delegate
            {
                Intent intent = new Intent(this, typeof(Page1Activity));
                intent.AddFlags(ActivityFlags.SingleTop);
                StartActivityForResult(intent, 1);
            };


            var edtPhone = FindViewById<EditText>(Resource.Id.editText1);
            //根据ID查找button
            var button3 = FindViewById<Android.Widget.Button>(Resource.Id.button3);
            //添加Click事件
            button3.Click += delegate
            {
                Regex rx = new Regex(@"^0{0,1}(13[4-9]|15[7-9]|15[0-2]|18[7-8])[0-9]{8}$");
                if (rx.IsMatch(edtPhone.Text)) //不匹配
                {
                    Android.Net.Uri uri = Android.Net.Uri.Parse("tel:" + edtPhone.Text);
                    Intent it = new Intent(Intent.ActionDial, uri);
                    StartActivity(it);
                }
                else
                {
                    Toast.MakeText(this, "请正确填写号码", ToastLength.Short).Show();
                }
            };


            //根据ID查找button
            var button4 = FindViewById<Android.Widget.Button>(Resource.Id.button4);
            //添加Click事件
            button4.Click += delegate
            {
                //使用WebView控件打开指定网页  
                Android.Webkit.WebView webview2 = FindViewById<Android.Webkit.WebView>(Resource.Id.webView1);
                webview = webview2;
                PalyAudio();
            };

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        #region Http访问辅助类
        /// <summary>
        /// 访问类
        /// </summary>
        /// <param name="url">访问地址</param>
        /// <param name="data">参数</param>
        /// <param name="encoding">编码类型</param>
        /// <returns></returns>
        public static string DoPost(string url, string data, Encoding encoding)
        {
            HttpWebRequest req = GetWebRequest(url, "POST");
            byte[] postData = Encoding.UTF8.GetBytes(data);
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();
            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();//这里报错的话请检查你的APIKey和SecretKey，一般是401，也就是服务器拒绝了你的请求
            return GetResponseAsString(rsp, encoding);
        }
        /// <summary>
        /// 请求头
        /// </summary>
        /// <param name="url">访问路径</param>
        /// <param name="method">访问类型</param>
        /// <returns></returns>
        public static HttpWebRequest GetWebRequest(string url, string method)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            req.ContentType = "text/json";
            req.Method = method;
            req.KeepAlive = true;
            req.UserAgent = "getbeijingtime";
            req.Timeout = 1 * 60 * 60;
            req.Proxy = null;
            return req;
        }

        /// <summary>
        /// 转码
        /// </summary>
        /// <param name="rsp">相应类型</param>
        /// <param name="encoding">编码类型</param>
        /// <returns></returns>
        public static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            StringBuilder result = new StringBuilder();
            Stream stream = null;
            StreamReader reader = null;
            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);
                // 每次读取不大于256个字符，并写入字符串
                char[] buffer = new char[256];
                int readBytes = 0;
                while ((readBytes = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    result.Append(buffer, 0, readBytes);
                }
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
            return result.ToString();
        }

        [DllImport("wininet")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        ///// <summary>
        /// 检测本机是否联网
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectedInternet()
        {
            System.Int32 dwFlag = new Int32();
            if (InternetGetConnectedState(out dwFlag, 0))
            {
                //已联网
                return true;
            }
            else
            {
                //未联网
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 获取Token数据
        /// </summary>
        public class BdVideoTokenModel
        {
            /// <summary>
            /// Token令牌，有效期30天
            /// </summary>
            public string access_token { get; set; }
            public string session_key { get; set; }
            public string scope { get; set; }
            public string refresh_token { get; set; }
            public string expires_in { get; set; }
        }

        /// <summary>
        /// 获取API的Token
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            //if (IsConnectedInternet() != true)
            //{
            //    return "";
            //}
            //官网地址http://ai.baidu.com/tech/speech/tts
            //官方文档地址http://ai.baidu.com/docs#/TTS-API/top
            //在Url里的client_id后输入你的APIKey，client_secret后输入你的SecretKey，我下面的两个key已经被改过了，直接请求会被拒绝
            string TokenUrl = "https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials&client_id=" + cuid + "&client_secret=gmSiC7pHX7WMVdGD6Ux7YWVQDhNraYDK&";
            Encoding ec = Encoding.UTF8;//官方要求以UTF-8请求
            string tokenJson = DoPost(TokenUrl, "", ec);//请求出Json字符串
            BdVideoTokenModel b = JsonConvert.DeserializeObject<BdVideoTokenModel>(tokenJson);//解析出Token
            return b.access_token;
        }

        public static void PalyAudio()
        {
            string url = "";//完整url路径
            string url1 = "http://tsn.baidu.com/text2audio?tex=";//调用路径
            //必须的几个参数
            string text = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");//本人懒省事，直接让他报现在的时间
            string tok = GetToken();//Token令牌,有效期为30天，可以缓存在本地
            if (text.Length >= 2048)//官方要求长度为小于2048位
            {
                Console.WriteLine("字符串过长！");
                return;
            }
            //if (IsConnectedInternet() != true)
            //{
            //    Console.WriteLine("您尚未联网！");
            //    return;
            //}
            if (string.IsNullOrEmpty(tok))
            {
                Console.WriteLine("未获取到Token，请检查！");
                return;
            }

            //所有参数
            //tex 必填  合成的文本，使用UTF-8编码。小于2048个中文字或者英文数字。（文本在百度服务器内转换为GBK/后，/长度必须小于4096字节）
            //tok 必填  开放平台获取到的开发者access_token（见上面的“鉴权认证机制”段落）
            //cuid 必填  用户唯一标识，用来计算UV值。建议填写能区分用户的机器 MAC 地址或 IMEI 码，长度为60字符以内
            //ctp 必填 客户端类型选择，web端填写固定值1
            //lan 必填 固定值zh。语言选择,目前只有中英文混合模式，填写固定值zh
            //spd 选填 语速，取值0 - 15，默认为5中语速
            //pit 选填 音调，取值0 - 15，默认为5中语调
            //vol 选填 音量，取值0 - 15，默认为5中音量
            //per 选填 发音人选择, 0为普通女声，1为普通男生，3为情感合成 - 度逍遥，4为情感合成 - 度丫丫，默认为普通女声
            // aue 选填  3为mp3格式(默认)； 4为pcm - 16k；5为pcm - 8k；6为wav（内容同pcm - 16k）; 注意aue = 4或者6是语音识别要求的格式，但是音频内容不是语音识别要求的自然人发音，所以识别效果会受影响。


            //其他参数我就不声明了，直接写在url里了
            url = url1 + text + "&lan=zh&cuid=" + cuid + "&ctp=1&tok=" + tok + "&vol=15&per=4&aue=3&spd=4";
            //DependencyService.Get<IAudio>().PlayAudioFile(url);
            //System.Diagnostics.Process.Start(url);//这里我直接让浏览器读了

            //需引用using Android.Webkit;命名空间  
            //使用默认的浏览器打开网页  
            // Android.Webkit.WebView webview = new Android.Webkit.WebView(this);  
            // webview.LoadUrl("http://www.baidu.com");  

            webview.LoadUrl(url);
            //加载项目中本地文件夹Assets下的test.html文件  
            // webview2.LoadUrl("file:///android_asset/test.html");  
            // webview2.LoadUrl("file:///android_asset/abc/test.html");  
            //启用脚本  
            webview.Settings.JavaScriptEnabled = true;

            #region --缩放--  
            //设置支持缩放(前提是网页自身支持缩放)  
            webview.Settings.SetSupportZoom(true);
            webview.Settings.BuiltInZoomControls = true;

            //支持任意比例缩放  
            webview.Settings.UseWideViewPort = true;
            //显示缩放控件(放大/缩小按钮)  
            webview.Settings.DisplayZoomControls = false;

            //自适应屏幕  
            //  webview2.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);  
            //webview2.Settings.LoadWithOverviewMode = true;  
            #endregion

            //webview2.ClearCache(true);  
            //后退  
            //webview2.GoBack();  
            //webview2.SetWebViewClient(new ExtWebViewClient());
        }

        class MyCommWebClient : WebViewClient
        {
            //重写页面加载的方法
            public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, string url)
            {
                //使用本控件加载
                view.LoadUrl(url);
                //并返回true
                return true;
            }
        }    
    }
}