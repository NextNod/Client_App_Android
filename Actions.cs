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
    [Activity(Label = "Actions", NoHistory = true)]
    public class Actions : Activity
    {
        string key;
        string server;
        int port;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle bundle = Intent.Extras.GetBundle("data");
            SetContentView(Resource.Layout.layout1);
            key = bundle.GetString("key");
            server = bundle.GetString("server");
            port = bundle.GetInt("port");

            Button sendButton = FindViewById<Button>(Resource.Id.sendButton);
            Button viewButton = FindViewById<Button>(Resource.Id.viewButton);

            sendButton.Click += (o, e) => sendMessage();
            viewButton.Click += (o, e) => viewMessage();
            FindViewById<Button>(Resource.Id.test).Click += (o, e) => SetContentView(Resource.Layout.ListOfMessages);
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
        }

        protected void sendMessage()
        {
            SetContentView(Resource.Layout.Send);

            FindViewById<Button>(Resource.Id.sendMessageButton).Click += (o, e) =>
            {
                string to = FindViewById<TextView>(Resource.Id.ed_to).Text;
                string message = FindViewById<TextView>(Resource.Id.et_message).Text;
            };
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
    }
}