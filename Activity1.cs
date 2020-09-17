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

namespace Client_App_Android
{
    [Activity(Label = "Acc", Theme = "@style/AppTheme")]
    public class Activity1 : Activity
    {
        protected string key;
        Button Send_button;
        Button View_button;
        TextView username;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.layout1);

            Send_button = FindViewById<Button>(Resource.Id.button_sendMessage);
            View_button = FindViewById<Button>(Resource.Id.button_viewMessage);
            username = FindViewById<TextView>(Resource.Id.textView_welcome);

            var keys = Intent.GetStringArrayListExtra("key");
            key = keys[0];
            string log = keys[1];

            username.Text = "Welcome back " + log + "!";


        }
    }
}