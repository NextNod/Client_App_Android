using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using System.Net.Sockets;
using System.Text;
using System;
using System.Collections.Generic;

namespace Client_App_Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText Login_et;
        EditText Pass;
        EditText Email;
        Button Enter;
        RadioButton Register;
        RadioButton Login_rb;
        LinearLayout linearLayout;

        string key;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Login_et = FindViewById<EditText>(Resource.Id.editText1);
            Pass = FindViewById<EditText>(Resource.Id.editText2);
            Email = FindViewById<EditText>(Resource.Id.autoCompleteTextView1);
            Enter = FindViewById<Button>(Resource.Id.button1);
            Register = FindViewById<RadioButton>(Resource.Id.radioButton1);
            Login_rb = FindViewById<RadioButton>(Resource.Id.radioButton2);
            linearLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout3);

            if (Register.Checked)
            {
                linearLayout.Visibility = Android.Views.ViewStates.Visible;
            }
            else
            {
                linearLayout.Visibility = Android.Views.ViewStates.Invisible;
            }


            Register.Click += (o, e) => Reg();
            Login_rb.Click += (o, e) => Reg();
            Enter.Click += (o, e) => Event();
        }

        protected void Reg()
        {
            if (Register.Checked)
            {
                linearLayout.Visibility = Android.Views.ViewStates.Visible;
            }
            else
            {
                linearLayout.Visibility = Android.Views.ViewStates.Invisible;
            }
        }

        protected void Event()
        {
            try
            {
                TcpClient client = new TcpClient("nextrun.mykeenetic.by", 801);
                NetworkStream stream = client.GetStream();

                if (Register.Checked)
                {
                    sendData(stream, "reg");
                    getData(stream);
                    sendData(stream, Login_et.Text);
                    getData(stream);
                    sendData(stream, Pass.Text);
                    getData(stream);
                    sendData(stream, Email.Text);
                    getData(stream);

                    client = new TcpClient("nextrun.mykeenetic.by", 801);
                    stream = client.GetStream();

                    sendData(stream, "log");
                    getData(stream);
                    sendData(stream, Login_et.Text);
                    getData(stream);
                    sendData(stream, Pass.Text);
                    key = getData(stream);
                }
                else
                {
                    sendData(stream, "log");
                    getData(stream);
                    sendData(stream, Login_et.Text);
                    getData(stream);
                    sendData(stream, Pass.Text);
                    key = getData(stream);
                }

                if (key[0] == '{')
                {
                    string err = "";
                    for (int i = 1; i < 4; i++)
                    {
                        err += key[i];
                    }

                    if (err == "Err")
                    {
                        FindViewById<TextView>(Resource.Id.textView3).Text = "Error";
                    }
                }
                else
                {
                    List<string> vs = new List<string>();
                    vs.Add(key);
                    vs.Add(Login_et.Text);
                    navigation(vs, typeof(Activity1), "key");
                }

                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                FindViewById<TextView>(Resource.Id.textView3).Text = ex.Message;
            }
        }

        protected string getData(NetworkStream stream)
        {
            byte[] vs = new byte[255];
            stream.Read(vs, 0, vs.Length);
            return Encoding.UTF8.GetString(vs);
        }

        protected void sendData(NetworkStream stream, string data)
        {
            byte[] vs = new byte[255];
            vs = Encoding.UTF8.GetBytes(data);
            stream.Write(vs, 0, vs.Length);
        }

        protected void navigation(List<string> vs, Type type, string name)
        {
            var intent = new Intent(this, type);
            intent.PutStringArrayListExtra(name, vs);
            StartActivity(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}