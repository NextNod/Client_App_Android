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
using System.Security.Cryptography;
using Android.Views;
using Android.Graphics;

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

            try
            {
                if (!Intent.Extras.IsEmpty)
                {
                    dserver = Intent.Extras.GetString("server");
                    dport = Intent.Extras.GetInt("port");
                }
            }
            catch { }

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
            SendNotify("Hello!", "This is my first notification!", (new Random()).Next());

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
                        Intent intent = new Intent(BaseContext, typeof(Settings));
                        StartActivity(intent);
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
                bool reg = false;

                if (radio.Checked)
                {
                    TcpClient client = new TcpClient(server, port);
                    NetworkStream stream = client.GetStream();
                    string salt = GenSalt(16);

                    sendData(stream, "reg");
                    getData(stream);
                    sendData(stream, log.Text);
                    getData(stream);
                    sendData(stream, ComputePasswordHash(pass.Text, salt));
                    getData(stream);
                    sendData(stream, salt);
                    getData(stream);
                    sendData(stream, email.Text);
                    string msg = getData(stream);

                    if (msg.Contains("{ER1}")) 
                    {
                        Toast toast = Toast.MakeText(BaseContext, msg.Substring(5), ToastLength.Short);
                        toast.Show();

                        stream.Close();
                        client.Close();

                        return;
                    }

                    stream.Close();
                    client.Close();
                    reg = true;
                }
                else if(!radio.Checked || reg)
                {
                    TcpClient client = new TcpClient(server, port);
                    NetworkStream stream = client.GetStream();

                    sendData(stream, "log");
                    getData(stream);
                    sendData(stream, log.Text);
                    string msg = getData(stream);

                    if (msg.Contains("{ER1}"))
                    {
                        Toast toast = Toast.MakeText(BaseContext, "Error!", ToastLength.Short);
                        toast.Show();

                        stream.Close();
                        client.Close();

                        return;
                    }

                    sendData(stream, ComputePasswordHash(pass.Text, msg));
                    msg = getData(stream);

                    if (msg.Contains("{ER1}")) 
                    {
                        Toast toast = Toast.MakeText(BaseContext, "Error!", ToastLength.Short);
                        toast.Show();

                        stream.Close();
                        client.Close();

                        return;
                    }

                    key = msg;
                    stream.Close();
                    client.Close();
                }

                Bundle bundle = new Bundle();
                bundle.PutString("key", key);
                bundle.PutString("server", server);
                bundle.PutInt("port", port);

                Intent intent = new Intent(BaseContext, typeof(Actions));
                intent.PutExtra("data", bundle);
                StartActivity(intent); // Переход
                Finish();
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
            string temp = Encoding.UTF8.GetString(vs), res = "";

            for (int i = 0; temp[i] != '\0'; i++) 
            {
                res += temp[i];
            }

            return res;
        }

        protected void sendData(NetworkStream stream, string data)
        {
            byte[] vs = new byte[255];
            vs = Encoding.UTF8.GetBytes(data);
            stream.Write(vs, 0, vs.Length);
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
        }

        // Подвал!

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            var name = "My channel";
            var description = "Example)";
            var channel = new NotificationChannel("location_notification", name, NotificationImportance.Default)
            {
                Description = description
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        void SendNotify(string title, string content, int id_notification)
        {
            PendingIntent pending = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(MainActivity)), PendingIntentFlags.OneShot);

            var builder = new NotificationCompat.Builder(this, "location_notification")
                .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                .SetContentTitle(title) // Set the title
                .SetContentIntent(pending)
                .SetSmallIcon(Resource.Mipmap.ic_launcher)
                .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.N))
                .AddAction(Resource.Mipmap.ic_launcher, "example", pending)
                .SetContentText(content); // the message to display.

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(id_notification, builder.Build());
        }

        string ComputePasswordHash(string password, string salt)
        {
            SHA256 SHA = new SHA256Managed();

            if (password == null || salt == null)
                return null;

            return BitConverter.ToString(SHA.ComputeHash(Encoding.UTF8.GetBytes(password + salt))).Replace("-", "").ToLower();
        }

        string GenSalt(int length)
        {
            RNGCryptoServiceProvider p = new RNGCryptoServiceProvider();
            var salt = new byte[length];
            p.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        protected override void OnStop()
        {
            base.OnStop();

            SendNotify("System", "Application work in backround!", (new Random()).Next());

        }
    }
}