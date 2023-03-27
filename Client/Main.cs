using Client.Properties;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;

namespace Client
{
    public partial class Main : Form
    {
        // Ссылка на клиент
        Client client;
        // Id пользователя с которым открыт диалог
        public int OpenedUserId { get; private set; }
        // Экземпляр класса SoundPlayer для воспроизведения звуков на форме
        SoundPlayer player = new SoundPlayer();

        public Main(Client client)
        {
            this.client = client;
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            client.BindMainForm(this);
            client.Receive();
            this.BackColor = Color.FromArgb(15, 27, 48);
            this.ChatUserNameLabel.ForeColor = Color.Wheat;
            this.MessageTextBox.BackColor = Color.FromArgb(15, 27, 48);
            this.SearchTextBox.BackColor = Color.FromArgb(15, 27, 48);
            this.MessageTextBox.ForeColor = Color.Wheat;
            this.MessageTextBox.BorderStyle = BorderStyle.FixedSingle;
            this.SearchLabel.ForeColor = Color.Wheat;
            this.SearchTextBox.ForeColor = Color.Wheat;
            ChatPanel.Visible = false;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // При нажатии клавиши Enter на SearchTextBox будем посылать запрос на сервер
            // для получения пользователей, которым можно написать

            // Проверяем на нажатие Enter
            if (e.KeyCode == Keys.Enter)
            {
                // Создаем запрос серверу
                Request request = new Request();
                // Тип - получение пользователей
                request.Type = Request.requestType.GETUSERS;
                // Передаем строку, которую пользователь написал в SearchTextBox
                request.Content = new object[] { SearchTextBox.Text };

                // Отправляем запрос серверу
                client.Send(JsonSerializer.Serialize(request));

                // Принимаем ответ
                client.Receive();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        public void UpdateSearchResult(List<UserInfo> searchResult)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                SearchResultFlowPanel.Controls.Clear();
            }));
            foreach (UserInfo user in searchResult)
            {
                Panel panel = new Panel();
                panel.Size = new Size(SearchResultFlowPanel.Width, 75);
                panel.Margin = new Padding(0,0,0,3);
                panel.Padding = new Padding(0);
                panel.BackColor = Color.FromArgb(41, 74, 134);

                Label username = new Label();
                username.Text = user.Username;
                username.Location = new Point(10, 5);
                username.Font = new Font("Arial", 12, FontStyle.Bold);
                username.ForeColor = Color.Wheat;

                Label online = new Label();

                if (user.Online)
                {
                    online.Text = "online";
                    online.ForeColor = Color.Green;
                }
                else
                {
                    online.Text = "offline";
                    online.ForeColor = Color.Red;
                }

                online.Location = new Point(SearchResultFlowPanel.Width - 45, 5);
                online.Font = new Font("Arial", 9);

                Label lastMessage = new Label();

                if (user.LastMessage != null)
                    lastMessage.Text = user.LastMessage;
                else 
                    lastMessage.Text = string.Empty;
                lastMessage.Size = new Size(SearchResultFlowPanel.Width, 30);
                lastMessage.Location = new Point(10, 30);
                lastMessage.Font = new Font("Arial", 10);
                lastMessage.ForeColor = Color.White;

                Label id = new Label();
                id.Text = user.Id.ToString();
                id.Location = new Point(20, 40);
                id.Visible = false;

                panel.Controls.Add(username);           // [0]
                panel.Controls.Add(id);                 // [1]
                panel.Controls.Add(lastMessage);        // [2]
                panel.Controls.Add(online);             // [3]

                if (username.Text == this.Text) continue;

                panel.Click += SearchPanel_Click;
                id.Click += SearchPanel_Click;
                lastMessage.Click += SearchPanel_Click;
                username.Click += SearchPanel_Click;
                online.Click += SearchPanel_Click;


                this.Invoke(new MethodInvoker(() =>
                {
                    SearchResultFlowPanel.Controls.Add(panel);
                }));
            }
        }

        public void GetMessagesForUser(int userId)
        {
            // Создаем новый запрос серверу
            Request request = new Request();
            request.Type = Request.requestType.GETMESSAGE;
            request.Content = new object[] { userId };

            // Отправляем запрос серверу
            client.Send(JsonSerializer.Serialize(request));
        }

        private void SearchPanel_Click(object sender, EventArgs e)
        {
            string username = string.Empty;
            try
            {
                OpenedUserId = int.Parse(((sender as Panel).Controls[1] as Label).Text);
                username = ((sender as Panel).Controls[0] as Label).Text;
            }
            catch
            {
                OpenedUserId = int.Parse(((sender as Label).Parent.Controls[1] as Label).Text);
                username = ((sender as Label).Parent.Controls[0] as Label).Text;
            }
            ChatUserNameLabel.Text = $"Чат с {username} [ID: {OpenedUserId}]";
            ChatPanel.Visible = true;

            GetMessagesForUser(OpenedUserId);
        }

        public void UpdateMessagesFlowPanel(List<MessageInfo> messages)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                MessagesFlowPanel.Controls.Clear();
            }));

            foreach (MessageInfo msg in messages)
            {
                GroupBox panel = new GroupBox();
                panel.Size = new Size(460, 75);
                panel.Font = new Font("Arial", 11, FontStyle.Bold);
                panel.ForeColor = Color.Wheat;
                panel.Text = msg.SenderName;

                if (panel.Text == this.Text)
                    panel.BackColor = Color.FromArgb(54, 97, 176);
                else
                    panel.BackColor = Color.FromArgb(41, 74, 134);

                Panel messagePanel = new Panel();
                messagePanel.Dock = DockStyle.Fill;

                Label messageLabel = new Label();
                messageLabel.Font = new Font("Arial", 10);
                messageLabel.Text = msg.Text;
                messageLabel.AutoSize = true;
                messageLabel.MaximumSize = new Size(panel.Size.Width - 10, 500);
                messageLabel.TextAlign = ContentAlignment.TopLeft;

                Label sendTime = new Label();
                sendTime.Font = new Font("Arial", 9);
                sendTime.Text = msg.SendTime.TimeOfDay.ToString(@"hh\:mm");
                sendTime.AutoSize = false;
                sendTime.Dock = DockStyle.Bottom;

                messagePanel.Controls.Add(messageLabel);
                panel.Controls.Add(sendTime);
                panel.Controls.Add(messagePanel);

                panel.Size = new Size(460, messageLabel.Height + sendTime.Height + 30);


                this.Invoke(new MethodInvoker(() =>
                {
                    MessagesFlowPanel.Controls.Add(panel);
                    MessagesFlowPanel.VerticalScroll.Value = MessagesFlowPanel.VerticalScroll.Maximum;
                }));
            }
        }

        public void UpdateLastMessage(string lastMessage, int userId)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                // Обновляем последнее сообщение
                foreach (Panel searchPanel in SearchResultFlowPanel.Controls)
                {
                    if (int.Parse((searchPanel.Controls[1] as Label).Text) == userId) // Controls[1] - id label
                        (searchPanel.Controls[2] as Label).Text = lastMessage; // Controls[2] - lastMessage label
                }
            }));
        }

        public void ShowNewMessageNotify(int userId)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                // Обновляем последнее сообщение
                foreach (Panel searchPanel in SearchResultFlowPanel.Controls)
                {
                    if (int.Parse((searchPanel.Controls[1] as Label).Text) == userId) // Controls[1] - id label
                        (searchPanel.Controls[0] as Label).Text += " (новое сообщение)"; // Controls[0] - username label
                }
            }));
        }

        public void UpdateOnlineStatus(bool status, int userId)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                // Обновляем последнее сообщение
                foreach (Panel searchPanel in SearchResultFlowPanel.Controls)
                {
                    if (int.Parse((searchPanel.Controls[1] as Label).Text) == userId) // Controls[1] - id label
                    {  
                        (searchPanel.Controls[3] as Label).Text = status ? "online" : "offline"; // Controls[3] - online label
                        (searchPanel.Controls[3] as Label).ForeColor = status ? Color.Green : Color.Red;
                    }
                }
            }));
        }

        public void ShowNewMessagePopup(string senderName, string msg)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                // Проигрываем звук
                player.Stream = Properties.Resources.receivedMessageSound;
                player.Play();

                PopupNotifier popup = new PopupNotifier();
                popup.TitleText = $"Новое сообщение от {senderName}";
                popup.ContentText = msg;
                popup.BodyColor = Color.FromArgb(15, 27, 48);
                popup.ContentColor = Color.Wheat;
                popup.TitleColor = Color.Wheat;
                popup.TitleFont = new Font("Arial", 12, FontStyle.Underline);
                popup.ContentFont = new Font("Arial", 10);
                popup.ContentPadding = new Padding(10, 5, 10, 5);
                popup.Popup();
            }));

        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Проигрываем звук набора
            player.Stream = Properties.Resources.keyPressSound;
            player.Play();

            if (e.KeyCode == Keys.Enter)
            {
                // Проигрываем звук отправки сообщения
                player.Stream = Properties.Resources.sendMessageSound;
                player.Play();

                // Создаем запрос серверу
                Request request = new Request();
                request.Type = Request.requestType.ADDMESSAGE;
                request.Content = new object[] { OpenedUserId, MessageTextBox.Text };

                // Отправляем запрос серверу
                client.Send(JsonSerializer.Serialize(request));

                // Если выбрана переписка с этим человеком, то обновляем сообщения
                GroupBox panel = new GroupBox();
                panel.Size = new Size(460, 75);
                panel.Font = new Font("Arial", 11, FontStyle.Bold);
                panel.ForeColor = Color.Wheat;
                panel.Text = this.Text;

                if (panel.Text == this.Text)
                    panel.BackColor = Color.FromArgb(54, 97, 176);
                else
                    panel.BackColor = Color.FromArgb(41, 74, 134);

                Panel messagePanel = new Panel();
                messagePanel.Dock = DockStyle.Fill;

                Label messageLabel = new Label();
                messageLabel.Font = new Font("Arial", 10);
                messageLabel.Text = MessageTextBox.Text;
                messageLabel.AutoSize = true;
                messageLabel.MaximumSize = new Size(panel.Size.Width - 10, 500);
                messageLabel.TextAlign = ContentAlignment.TopLeft;

                Label sendTime = new Label();
                sendTime.Font = new Font("Arial", 9);
                sendTime.Text = DateTime.Now.TimeOfDay.ToString(@"hh\:mm");
                sendTime.AutoSize = false;
                sendTime.Dock = DockStyle.Bottom;

                messagePanel.Controls.Add(messageLabel);
                panel.Controls.Add(sendTime);
                panel.Controls.Add(messagePanel);

                panel.Size = new Size(460, messageLabel.Height + sendTime.Height + 30);

                this.Invoke(new MethodInvoker(() =>
                {
                    MessagesFlowPanel.Controls.Add(panel);
                    MessagesFlowPanel.VerticalScroll.Value = MessagesFlowPanel.VerticalScroll.Maximum;

                    // Обновляем последнее сообщение
                    UpdateLastMessage(MessageTextBox.Text, OpenedUserId);

                    // Очищаем поле с сообщением
                    MessageTextBox.Clear();
                }));

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
