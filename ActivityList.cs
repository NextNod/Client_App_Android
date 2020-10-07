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
    [Activity(Label = "ActivityList")]
    public class ActivityList : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            SetContentView(Resource.Layout.layout1);

            ActionBar.Tab tab = ActionBar.NewTab();
            tab.SetText("Main");
            tab.TabSelected += (sender, args) => {
                Toast toast = Toast.MakeText(BaseContext, "Main tab", ToastLength.Short);
                toast.Show();
            };
            ActionBar.AddTab(tab);

            tab = ActionBar.NewTab();
            tab.SetText("Tab1");
            tab.TabSelected += (sender, args) => {
                Toast toast = Toast.MakeText(BaseContext, "Tab1", ToastLength.Short);
                toast.Show();
            };
            ActionBar.AddTab(tab);
        }
    }
}