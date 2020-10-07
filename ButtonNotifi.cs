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
    [Activity(Label = "ButtonNotifi")]
    public class ButtonNotifi : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Toast toast = Toast.MakeText(BaseContext, "Button Example pressed!", ToastLength.Long);
            toast.Show();
        }
    }
}