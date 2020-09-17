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

            Login_et = FindViewById<EditText>(Resource.Id.editText_login);
            Pass = FindViewById<EditText>(Resource.Id.editText_pass);
            Email = FindViewById<EditText>(Resource.Id.autoCompleteTextView_email);
            Enter = FindViewById<Button>(Resource.Id.button_Enter);
            Register = FindViewById<RadioButton>(Resource.Id.radioButton_reg);
            Login_rb = FindViewById<RadioButton>(Resource.Id.radioButton_log);
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
                        FindViewById<TextView>(Resource.Id.textView_err_log).Text = "Error";
                    }
                }
                else
                {
                    Actions(Login_et.Text);
                }

                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                FindViewById<TextView>(Resource.Id.textView_err_log).Text = ex.Message;
            }
        }

        protected string getData(NetworkStream stream)
        {
            byte[] vs = new byte[255];
            stream.Read(vs, 0, vs.Length);
            return Encoding.UTF8.GetString(vs);
        }

        protected void Actions(string log) 
        {
            SetContentView(Resource.Layout.layout1);

            Button button_Send = FindViewById<Button>(Resource.Id.button_sendMessage);
            Button button_View = FindViewById<Button>(Resource.Id.button_viewMessage);
            TextView textView = FindViewById<TextView>(Resource.Id.textView_welcome);

            textView.Text = "Welcome back " + log;

            button_Send.Click += (o, e) => ;
            button_View.Click += (o, e) => ;
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