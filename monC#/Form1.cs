using System;
using System.Windows.Forms;
using AxWMPLib;
using System.IO;

namespace monC_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void PlayBackgroundMusic()
        {
            // Ensure the path is correct and file exists
            string musicPath = @"D:\C#\bai1\nhac\băt dau.mp3";
            if (File.Exists(musicPath))
            {
                nhac.URL = musicPath;
                nhac.settings.setMode("loop", true);
                nhac.Ctlcontrols.play();

                // Hide and disable the media player
                nhac.Enabled = false;
                nhac.Visible = false;
            }
            else
            {
                MessageBox.Show("Music file not found: " + musicPath);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Start playing background music when the form loads
            PlayBackgroundMusic();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // Stop background music before opening the new form
            nhac.Ctlcontrols.stop();

          
            OpenNextForm(); // Mở form tiếp theo
        }
        private void OpenNextForm() // Mở form tiếp theo
        {
            nhac.Ctlcontrols.stop(); // Dừng nhạc nền
            this.Hide(); // Ẩn form hiện tại
           chuongtong nextForm = new chuongtong(); // Tạo form mới
            nextForm.ShowDialog(); // Hiển thị form mới
            this.Close(); // Đóng form hiện tại
        }
    }
}
