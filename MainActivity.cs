using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using System.Net.Sockets;
using System.Text;
using System;
using Android.Support.V4.App;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;
using System.Collections.Generic;
using Android.Views;

namespace Client_App_Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        string key;

        string server = "nextrun.mykeenetic.by";
        int port = 801;

        string dserver = "nextrun.mykeenetic.by";
        int dport = 801;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            main();
        }

        protected void main() 
        {
            SetContentView(Resource.Layout.activity_main);

            EditText Login_et = FindViewById<EditText>(Resource.Id.editText1);
            EditText Pass = FindViewById<EditText>(Resource.Id.editText2);
            EditText Email = FindViewById<EditText>(Resource.Id.editText3);
            Button Enter = FindViewById<Button>(Resource.Id.button1);
            RadioButton Register = FindViewById<RadioButton>(Resource.Id.radioButton1);
            RadioButton Login_rb = FindViewById<RadioButton>(Resource.Id.radioButton2);
            LinearLayout linearLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout3);

            if (Register.Checked)
            {
                linearLayout.Visibility = Android.Views.ViewStates.Visible;
            }
            else
            {
                linearLayout.Visibility = Android.Views.ViewStates.Invisible;
            }

            CreateNotificationChannel();
            SendNotify("Hello!", "This is my firts notification!");

            Register.Click += (o, e) => Reg(Register, Login_rb, linearLayout, false);
            Login_rb.Click += (o, e) => Reg(Register, Login_rb, linearLayout, true);
            Enter.Click += (o, e) => Event(Register, Login_et, Pass, Email);
        }

        public override bool OnCreateOptionsMenu(IMenu menu) 
        {
            MenuInflater.Inflate(Resource.Layout.menu1, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_search:
                {
                    Settings(); 
                    return true;
                }
                case Resource.Id.action_search1:
                {
                    item.SetChecked(!item.IsChecked);
                    if (item.IsChecked)
                    {
                        server = "nextrun.mykeenetic.by";
                        port = 801;
                    }
                    else 
                    {
                        server = dserver;
                        port = dport;
                    }
                    return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        public void Settings() 
        {
            SetContentView(Resource.Layout.Settings);

            FindViewById<Button>(Resource.Id.backToMain).Click += (o, e) => main();

            FindViewById<Button>(Resource.Id.defaultButton).Click += (o, e) =>
            {
                server = "nextrun.mykeenetic.by";
                port = 801;

                Toast toast = Toast.MakeText(BaseContext, "Settings saved!", ToastLength.Short);
                toast.Show();

                layout1();
            };

            FindViewById<Button>(Resource.Id.Save_settings).Click += (o, e) =>
            {
                dserver = FindViewById<EditText>(Resource.Id.sAddres).Text;
                dport = Convert.ToInt32(FindViewById<EditText>(Resource.Id.sPort).Text);

                Toast toast = Toast.MakeText(BaseContext, "Settings saved!", ToastLength.Short);
                toast.Show();

                layout1();
            };
        }

        protected void Reg(RadioButton reg, RadioButton log, LinearLayout linear, bool logrb)
        {
            if (logrb)
            {
                reg.Checked = false;
                log.Checked = true;
                linear.Visibility = Android.Views.ViewStates.Invisible;
            }
            else 
            {
                reg.Checked = true;
                log.Checked = false;
                linear.Visibility = Android.Views.ViewStates.Visible;
            }
        }

        protected void Event(RadioButton radio, EditText log, EditText pass, EditText email)
        {
            try
            {
                TcpClient client = new TcpClient(server, port);
                NetworkStream stream = client.GetStream();

                if (radio.Checked)
                {
                    sendData(stream, "reg");
                    getData(stream);
                    sendData(stream, log.Text);
                    getData(stream);
                    sendData(stream, pass.Text);
                    getData(stream);
                    sendData(stream, email.Text);
                    getData(stream);

                    stream.Close();
                    client.Close();

                    client = new TcpClient(server, port);
                    stream = client.GetStream();

                    sendData(stream, "log");
                    getData(stream);
                    sendData(stream, log.Text);
                    getData(stream);
                    sendData(stream, pass.Text);
                    key = getData(stream);
                }
                else
                {
                    sendData(stream, "log");
                    getData(stream);
                    sendData(stream, log.Text);
                    getData(stream);
                    sendData(stream, pass.Text);
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
                        stream.Close();
                        client.Close();
                        return;
                    }
                }

                stream.Close();
                client.Close();
                // Переход
                layout1();
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

        protected void layout1() 
        {
            SetContentView(Resource.Layout.layout1);

            Button sendButton = FindViewById<Button>(Resource.Id.sendButton);
            Button viewButton = FindViewById<Button>(Resource.Id.viewButton);

            sendButton.Click += (o, e) => sendMessage();
            viewButton.Click += (o, e) => viewMessage();
            FindViewById<Button>(Resource.Id.test).Click += (o, e) => SetContentView(Resource.Layout.ListOfMessages);
        }

        protected void sendMessage() 
        {
            SetContentView(Resource.Layout.Send);

            FindViewById<Button>(Resource.Id.sendMessageButton).Click += (o, e) => {
                string to = FindViewById<TextView>(Resource.Id.ed_to).Text;
                string message = FindViewById<TextView>(Resource.Id.et_message).Text;
                Send(to, message);
            };
        }

        protected void Send(string to, string message) 
        {
            TcpClient client = new TcpClient(server, port);
            NetworkStream stream = client.GetStream();

            sendData(stream, "send");
            getData(stream);
            sendData(stream, key);
            getData(stream);
            sendData(stream, to);
            getData(stream);
            sendData(stream, message);
            getData(stream);

            stream.Close();
            client.Close();

            Toast toast = Toast.MakeText(BaseContext, "Message sendet!", ToastLength.Long);
            layout1();
        }

        protected void viewMessage() 
        {
            SetContentView(Resource.Layout.Read);

            TcpClient client = new TcpClient(server, port);
            NetworkStream stream = client.GetStream();

            sendData(stream, "messages");
            getData(stream);
            sendData(stream, key);
            FindViewById<TextView>(Resource.Id.messagesView).Text = getData(stream);

            stream.Close();
            client.Close();

            FindViewById<Button>(Resource.Id.back_to_act).Click += (o, e) => layout1();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var name = "My channel";
            var description = "Example)";
            var channel = new NotificationChannel("location_notification", name, NotificationImportance.Default)
            {
                Description = description
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        void SendNotify(string title, string content)
        {
            var builder = new NotificationCompat.Builder(this, "location_notification")
                          .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                          .SetContentTitle(title) // Set the title
                          .SetSmallIcon(Resource.Drawable.Notifi)
                          .SetContentText(content); // the message to display.

            // Finally, publish the notification:
            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(1000, builder.Build());

            // Increment the button press count:
        }
    }
}