using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App1.Droid
{
    [Activity(Label = "Page1Activity")]
    public class Page1Activity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.page1);

            //根据ID查找button
            var button = FindViewById<Button>(Resource.Id.button1);
            //添加Click事件
            button.Click += delegate
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.SingleTop);
                StartActivityForResult(intent, 1);
            };

            // Create your application here
        }
    }
}