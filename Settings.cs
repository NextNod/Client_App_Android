using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Client_App_Android
{
    [Activity(Label = "Settings")]
    public class Settings : Activity
    {
        string dserver = "nextrun.mykeenetic.by";
        int dport = 801;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Settings);

            FindViewById<Button>(Resource.Id.backToMain).Click += (o, e) =>
            {
                goToMain();
            };

            FindViewById<Button>(Resource.Id.defaultButton).Click += (o, e) =>
            {
                dserver = "nextrun.mykeenetic.by";
                dport = 801;

                Toast toast = Toast.MakeText(BaseContext, "Settings saved!", ToastLength.Short);
                toast.Show();
            };

            FindViewById<Button>(Resource.Id.Save_settings).Click += (o, e) =>
            {
                dserver = FindViewById<EditText>(Resource.Id.sAddres).Text;
                dport = Convert.ToInt32(FindViewById<EditText>(Resource.Id.sPort).Text);

                Toast toast = Toast.MakeText(BaseContext, "Settings saved!", ToastLength.Short);
                toast.Show();

                goToMain();
            };
        }

        void goToMain() 
        {
            Intent intent = new Intent(BaseContext, typeof(MainActivity));
            Bundle bundle = new Bundle();
            intent.PutExtra("server", dserver);
            intent.PutExtra("port", dport);
            StartActivity(intent);
            Finish();
        }
    }
}