using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class frmMain : Form, ClientServiceControl
    {
        ClientService _ClientService;

        public frmMain()
        {
            InitializeComponent();

            this.cmdConnect.Click += cmdConnect_Click;
            this.cmdSendMessage.Click += cmdSendMessage_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _ClientService = new ClientService(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            _ClientService.Stops();
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            _ClientService.Connect(txtChatName.Text, null);
        }

        private void cmdSendMessage_Click(object sender, EventArgs e)
        {
            _ClientService.SendMessage(txtOutMsg.Text);
        }

        /*---*/

        public void ShowMessage(Message message)
        {
            string msg = message.data;
            switch (message.type)
            {
                default:
                case ClientService.MessageType.Text:
                    break;
                case ClientService.MessageType.Image:
                    break;
                case ClientService.MessageType.Anim:
                    break;
                case ClientService.MessageType.File:
                    break;
                case ClientService.MessageType.LoggedIn:
                    break;
            }

            this.txtConversation.Text += string.Format(
                              Environment.NewLine + Environment.NewLine
                              + " >> {0}", msg);
        }

        public void ShowPrompt(Prompt prompt)
        {
            string msg = prompt.description;
            switch (prompt.status)
            {
                case ClientService.Status.Connected:
                    msg = "Холбогдлоо.";
                    break;
                case ClientService.Status.LoggedIn:
                    msg = "Нэвтэрч орлоо.";
                    break;
                case ClientService.Status.LoggedOut:
                    msg = "Нэвтэрч гарлаа.";
                    break;
                case ClientService.Status.LoggingError:
                    msg = "Нэвтрэх үед алдаа гарлаа!";
                    break;
                case ClientService.Status.CannotPass:
                    msg = "Илгээхэд асуудал гарлаа!";
                    break;
                case ClientService.Status.CannotSend:
                    msg = "Илгээж чадсангүй!";
                    break;
                case ClientService.Status.CannotReceived:
                    msg = "Хүлээн авч чадсангүй!";
                    break;
                case ClientService.Status.Disconnected:
                    msg = "Холболт салсан!";
                    break;
                default:
                    return;
            }

            txtConversation.Text = txtConversation.Text
                + Environment.NewLine + " >> " + msg;
        }
    }
}
