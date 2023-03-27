using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Windows.Forms.VisualStyles;
using System.Runtime.CompilerServices;

namespace Client
{
    public partial class Auth : Form
    {
        // Объект client, через который проходит связь с сервером
        Client client;
        public Auth()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // При загрузке формы создаем экземпляр класса Client,
            // в конструктор передаем IP сервера и его порт
            client = new Client("45.141.102.64", 8006, this);

            // Подключаемся к серверу
            client.Connect();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            // При авторизации создаем новый запрос, который будет направлен на севрер
            // Запрос содержит логин и пароль, который введен в форме
            Request loginRequest = new Request();
            loginRequest.Type = Request.requestType.AUTH;
            loginRequest.Content = new object[] { LoginTextBox.Text, PasswordTextBox.Text };

            // Отправляем запрос на сервер
            client.Send(JsonSerializer.Serialize(loginRequest));
        }

        private void RegButton_Click(object sender, EventArgs e)
        {
            // При регистрации создаем новый запрос, который будет направлен на север,
            // Запрос также содержит логин и пароль, который введен в форме
            Request regRequest = new Request();
            regRequest.Type = Request.requestType.REG;
            regRequest.Content = new object[] { LoginTextBox.Text, PasswordTextBox.Text };

            // Отправляем запрос на сервер
            client.Send(JsonSerializer.Serialize(regRequest));

        }

        private void euePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            PasswordTextBox.UseSystemPasswordChar = false;
            euePictureBox.Image = Properties.Resources.eye_hidden;
        }

        private void euePictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            PasswordTextBox.UseSystemPasswordChar = true;
            euePictureBox.Image = Properties.Resources.eye_showen;
        }
    }
}
