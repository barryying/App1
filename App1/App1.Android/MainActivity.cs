using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using Android.Content;
using System.Text.RegularExpressions;

namespace App1.Droid
{
    [Activity(Label = "App1", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        int count = 1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.mypage);

            //根据ID查找button
            var button1 = FindViewById<Button>(Resource.Id.button1);
            //添加Click事件
            button1.Click += delegate
            {
                button1.Text = string.Format("这是第{0} 单击!", count++);
            };


            //根据ID查找button
            var button2 = FindViewById<Button>(Resource.Id.button2);
            //添加Click事件
            button2.Click += delegate
            {
                Intent intent = new Intent(this, typeof(Page1Activity));
                intent.AddFlags(ActivityFlags.SingleTop);
                StartActivityForResult(intent, 1);
            };


            var edtPhone = FindViewById<EditText>(Resource.Id.editText1);
            //根据ID查找button
            var button3 = FindViewById<Button>(Resource.Id.button3);
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

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
    }
    class MyCommWebClient : WebViewClient
    {
        //重写页面加载的方法
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            //使用本控件加载
            view.LoadUrl(url);
            //并返回true
            return true;
        }
    }
}