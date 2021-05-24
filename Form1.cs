using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace MulticastClient
{
    public partial class Form1 : Form
    {
        Socket socket;
        IPAddress ip; 
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try { ip = IPAddress.Parse(textBox1.Text); }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Пожалуйста, введите IP", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (FormatException)
            {
                MessageBox.Show("Пожалуйста, введите корректный IP", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            IPEndPoint endPoint;
            try
            {
                endPoint = new IPEndPoint(IPAddress.Any, int.Parse(textBox2.Text));
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Пожалуйста, введите корректный порт", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Пожалуйста, введите порт", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            socket.Bind(endPoint);
            try { socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any)); }
            catch (SocketException)
            {
                MessageBox.Show("Пожалуйста, введите корректный IP (для многоадресной передачи используйте IP в диапозоне от 224.0.0.0 до 239.255.255.255)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textBox3.Text += DateTime.Now;
            textBox3.Text += Environment.NewLine + "Клиент подключен ";
            textBox3.Text += Environment.NewLine + Environment.NewLine;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = true;
            this.Text = "Клиент (подключен)";
           }

        private void MessageRecieved(string filename)
        {
            textBox3.Text += DateTime.Now;
            textBox3.Text += Environment.NewLine + "Получен файл " + filename;
            textBox3.Text += Environment.NewLine + Environment.NewLine;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            this.Text = "Клиент (отключен)";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            socket.Close();
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            this.Text = "Клиент (отключен)";
            textBox3.Text += DateTime.Now;
            textBox3.Text += Environment.NewLine + "Клиент отключен";
            textBox3.Text += Environment.NewLine + Environment.NewLine;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            byte[] data = new byte[2048];
            int size = socket.Receive(data);
            string filename = System.Text.Encoding.Default.GetString(data, 0, size);
            socket.Receive(data);
            MessageRecieved(filename);
            string path = textBox4.Text + "\\" + filename;
           // MessageRecieved(path);
            try
            {
                File.WriteAllBytes(path, data);
                textBox3.Text += DateTime.Now;
                textBox3.Text += Environment.NewLine + "Файл " + filename;
                textBox3.Text += " сохранён. Расположение файла: " + path +Environment.NewLine + Environment.NewLine;
            }
            catch (DirectoryNotFoundException)
            {
                textBox3.Text += DateTime.Now;
                textBox3.Text += Environment.NewLine + "Не удалось сохранить " + filename;
                textBox3.Text += ": указан недопустимый путь" +Environment.NewLine + Environment.NewLine;
            }
            catch (UnauthorizedAccessException)
            {
                textBox3.Text += DateTime.Now;
                textBox3.Text += Environment.NewLine + "Не удалось сохранить " + filename;
                textBox3.Text += ": файл с таким именем недоступен для записи" + Environment.NewLine + Environment.NewLine;
            }
            catch (NotSupportedException)
            {
                textBox3.Text += DateTime.Now;
                textBox3.Text += Environment.NewLine + "Не удалось сохранить " + filename;
                textBox3.Text += ": указан недопустимый путь" + Environment.NewLine + Environment.NewLine + path;
            }
            catch (ArgumentException)
            {
                textBox3.Text += DateTime.Now;
                textBox3.Text += Environment.NewLine + "Не удалось сохранить " + filename;
                textBox3.Text += ": указан недопустимый путь" + Environment.NewLine + Environment.NewLine + path;
            }
            catch (PathTooLongException)
            {
                textBox3.Text += DateTime.Now;
                textBox3.Text += Environment.NewLine + "Не удалось сохранить " + filename;
                textBox3.Text += ": указан недопустимо длинный путь" + Environment.NewLine + Environment.NewLine + path;
            }
            button3.Enabled = true;
        }
    }
}
