using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace monC_
{
    public partial class chuongtong : Form
    {
        public chuongtong()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenNextForm(); // Mở form tiếp theo
        }
        private void OpenNextForm() // Mở form tiếp theo
        {
            
            this.Hide(); // Ẩn form hiện tại
            gameduaxe nextForm = new gameduaxe(); // Tạo form mới
            nextForm.ShowDialog(); // Hiển thị form mới
            this.Close(); // Đóng form hiện tại
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenNextForm1(); // Mở form tiếp theo
        }
        private void OpenNextForm1() // Mở form tiếp theo
        {

            this.Hide(); // Ẩn form hiện tại
            chuong1 nextForm = new chuong1(); // Tạo form mới
            nextForm.ShowDialog(); // Hiển thị form mới
            this.Close(); // Đóng form hiện tại
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
