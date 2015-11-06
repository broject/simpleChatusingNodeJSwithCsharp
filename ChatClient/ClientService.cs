using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    class ClientService
    {
        public static int PORT = 6891;
        public static string HOST = "127.0.0.1";
        public static int BUFFER_SIZE = 4096;
        public class Roles
        {
            public const int Worker = 1;
            public const int Foreman = 2;
            public const int Chairman = 3;
            public const int Director = 4;
            public const int Admin = 5;
            public const int Root = 9;
        }
        public class Status
        {
            public const int Connected = 11;
            public const int LoggedIn = 22;
            public const int LoggedOut = 33;
            public const int LoggingError = 44;
            public const int CannotPass = 66;
            public const int CannotSend = 77;
            public const int CannotReceived = 88;
            public const int Disconnected = 99;
        }
        public class MessageType : Status
        {
            public const int Text = 1001;
            public const int Link = 1002;
            public const int Html = 1003;
            public const int Image = 2001;
            public const int Images = 2002;
            public const int Anim = 3001;
            public const int Anims = 3002;
            public const int File = 4001;
            public const int Files = 4002;
        }

        private ClientServiceControl _Control = null;
        private TcpClient _TcpClient = new TcpClient();
        private NetworkStream _ServerStream = default(NetworkStream);
        private Dictionary<int, Thread> _Threads = new Dictionary<int, Thread>();

        UserAccount _User = null;
        UserSetting _Setting = null;

        public ClientService(ClientServiceControl control)
        {
            this._Control = control;
        }

        private Thread startChatListen()
        {
            Thread task = new Thread(() =>
            {
                while (true)
                {
                    if (!_TcpClient.Connected)
                    {
                        Prompt prompt = new Prompt();
                        prompt.status = (int)Status.Disconnected;
                        this.ShowPrompt(prompt);
                        break;
                    }

                    _ServerStream = _TcpClient.GetStream();

                    try
                    {
                        byte[] bytes = new byte[BUFFER_SIZE];
                        int bytesRead = _ServerStream.Read(bytes, 0, bytes.Length);
                        Message message = JS.Deserialize<Message>(bytes, bytesRead);

                        this.ShowMessage(message);
                    }
                    catch
                    {
                        Prompt prompt = new Prompt();
                        prompt.status = (int)Status.CannotReceived;
                        this.ShowPrompt(prompt);
                        break;
                    }

                    Thread.Sleep(500);
                }
            });
            _Threads.Add(task.GetHashCode(), task);
            task.Start();
            return task;
        }

        /*----*/
        public void ShowPrompt(Prompt prompt)
        {
            try
            {
                if (_Control is Control)
                {
                    ((Control)_Control).Invoke((ThreadStart)(
                        () =>
                        {
                            _Control.ShowPrompt(prompt);
                        }));
                }
            }
            catch
            {
            }
        }

        public void ShowMessage(Message message)
        {
            try
            {
                if (_Control is Control)
                {
                    ((Control)_Control).Invoke((ThreadStart)(
                        () =>
                        {
                            _Control.ShowMessage(message);
                        }));
                }
            }
            catch
            {
            }
        }

        public void Connect(string username, string password)
        {
            if (_TcpClient.Connected == false)
            {
                _TcpClient.Connect(HOST, PORT);

                Prompt prompt = new Prompt();
                prompt.status = Status.Connected;
                this.ShowPrompt(prompt);
            }
            if (_ServerStream == null)
            {
                _ServerStream = _TcpClient.GetStream();
            }

            UserAccount usr = new UserAccount();
            usr.username = string.IsNullOrEmpty(username) ? "Guest" : username;
            usr.email = "guest@yahoo.com";
            string jstr = JS.Serialize<UserAccount>(usr);

            this.SendRaw(jstr);

            Thread task = new Thread(() =>
            {
                _ServerStream = _TcpClient.GetStream();

                try
                {
                    byte[] bytes = new byte[BUFFER_SIZE];
                    int bytesRead = _ServerStream.Read(bytes, 0, bytes.Length);
                    UserAccount user = JS.Deserialize<UserAccount>(bytes, bytesRead);

                    if (this.IsLoggedIn(user))
                    {
                        this._User = user;

                        Prompt prompt = new Prompt();
                        prompt.status = Status.LoggedIn;
                        this.ShowPrompt(prompt);

                        startChatListen();
                    }
                }
                catch(Exception ex)
                {
                    Prompt prompt = new Prompt();
                    prompt.status = (int)Status.LoggingError;
                    prompt.description = ex.Message;
                    this.ShowPrompt(prompt);
                }
            });
            _Threads.Add(task.GetHashCode(), task);
            task.Start();
        }

        public bool IsLoggedIn(UserAccount user = null)
        {
            if (user != null)
                return (user != null && !string.IsNullOrEmpty(user.username) && !string.IsNullOrWhiteSpace(user.username));
            else
                return (_User != null && !string.IsNullOrEmpty(_User.username) && !string.IsNullOrWhiteSpace(_User.username));
        }

        public void SendRaw(string jstr)
        {
            if (_ServerStream != null && _TcpClient.Connected)
            {
                byte[] outBytes = Encoding.UTF8.GetBytes(jstr);
                _ServerStream.Write(outBytes, 0, outBytes.Length);
                _ServerStream.Flush();
            }
            else
            {
                Prompt prompt = new Prompt();
                prompt.status = Status.CannotPass;
                this.ShowPrompt(prompt);
            }
        }

        public void SendMessage(string msg, string to = null)
        {
            if (_ServerStream != null && _TcpClient.Connected && this.IsLoggedIn())
            {
                Message message = new Message();
                message.from = this._User.username;
                message.to = to;
                message.data = msg;
                string jstr = JS.Serialize<Message>(message);

                byte[] outBytes = Encoding.UTF8.GetBytes(jstr);
                _ServerStream.Write(outBytes, 0, outBytes.Length);
                _ServerStream.Flush();
            }
            else
            {
                Prompt prompt = new Prompt();
                prompt.status = Status.CannotSend;
                this.ShowPrompt(prompt);
            }
        }

        public void Stops()
        {
            _TcpClient.Close();
            _ServerStream = null;
            foreach (int key in _Threads.Keys)
            {
                Thread task = _Threads[key];
                if (task.IsAlive)
                {
                    task.Abort();
                }
            }
        }
    }
}
