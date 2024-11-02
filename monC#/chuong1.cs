using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using AxWMPLib;

namespace monC_
{
    public partial class chuong1 : Form
    {
        private Timer timer; // Bộ đếm thời gian để cập nhật vị trí của các đối tượng
        private int speed = 15; // Tốc độ di chuyển của các vật thể trên đường
        private List<PictureBox> vachList; // Danh sách các vạch trắng trên đường
        private List<PictureBox> xeDichList; // Danh sách các xe đối thủ và các vật cản
        private List<PictureBox> vienList; // Danh sách chứa các đường biên (vien1, vien2)
        private int distance = 150; // Khoảng cách giữa các vạch trắng
        private int xeSpeed = 20; // Tốc độ di chuyển của xe người chơi khi nhấn phím
        private Random rand; // Đối tượng ngẫu nhiên để tạo vị trí ngẫu nhiên cho các xe đối thủ
        private int score = 0; // Điểm số của người chơi

        // Đường dẫn đến tệp âm thanh
        private string musicPath = @"D:\C#\bai1\nhac\chuong1.mp3"; // Đổi tên tệp âm thanh thành nhac.mp3

        public chuong1()
        {
            InitializeComponent();
            InitializeVachList(); // Khởi tạo danh sách các vạch trắng trên đường

            xeDichList = new List<PictureBox> { xe1, xe2, xe3, da }; // Danh sách các xe đối thủ và vật cản
            vienList = new List<PictureBox> { vien1, vien2 }; // Khởi tạo danh sách đường biên

            InitializeTimer(); // Thiết lập bộ đếm thời gian
            rand = new Random(); // Tạo đối tượng ngẫu nhiên
            this.KeyDown += new KeyEventHandler(OnKeyDown); // Bắt sự kiện nhấn phím

            // Phát nhạc nền
            PlayBackgroundMusic();

            // Thiết lập ô điểm (score) chỉ đọc
            diem.Text = score.ToString();
            diem.Enabled = false;
        }

        private void InitializeVachList()
        {
            // Khởi tạo danh sách vạch trắng
            vachList = new List<PictureBox> { vac, vac1, vac2 };
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 30; // Đặt khoảng thời gian cho mỗi lần cập nhật
            timer.Tick += Timer_Tick; // Gán sự kiện khi timer đếm đến tick
            timer.Start(); // Bắt đầu bộ đếm thời gian
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Di chuyển từng vạch trắng
            foreach (var vach in vachList)
            {
                MoveVach(vach);
            }

            // Di chuyển và kiểm tra va chạm cho từng xe đối thủ
            for (int i = 0; i < xeDichList.Count; i++)
            {
                MoveXeDich(xeDichList[i], i); // Gọi hàm MoveXeDich với chỉ số
                CheckCollision(xeDichList[i]); // Kiểm tra va chạm với xe người chơi
            }
        }

        private void MoveVach(PictureBox vach)
        {
            vach.Top += speed;

            // Nếu vạch trắng ra khỏi màn hình, đặt lại vị trí của nó
            if (vach.Top >= this.ClientSize.Height)
            {
                vach.Top = -vach.Height;
                vach.Top -= distance;
            }
        }

        private void MoveXeDich(PictureBox xeDich, int index)
        {
            // Di chuyển xe đối thủ xuống dưới
            xeDich.Top += speed;

            // Nếu xe đối thủ vượt qua màn hình, đặt lại vị trí và tăng điểm
            if (xeDich.Top >= this.ClientSize.Height)
            {
                xeDich.Top = -xeDich.Height;

                // Giới hạn vị trí ngẫu nhiên trong phạm vi đường đi
                int roadLeftBoundary = 150; // Giới hạn trái của đường
                int roadRightBoundary = Math.Max(this.ClientSize.Width - 200, roadLeftBoundary + 1); // Đảm bảo right boundary lớn hơn left boundary

                int newX;
                do
                {
                    // Tạo vị trí ngẫu nhiên cho xe đối thủ trong phạm vi đường đi
                    newX = rand.Next(roadLeftBoundary, roadRightBoundary);
                    xeDich.Left = newX;
                } while (xe.Bounds.IntersectsWith(xeDich.Bounds) || OverlapsWithVien(xeDich)); // Kiểm tra xe không đè lên đường biên hoặc xe người chơi

                // Tăng điểm khi xe đối thủ được đặt lại
                score++;
                diem.Text = score.ToString();

                // Kiểm tra xem điểm số có đạt 30 không
                if (score == 20)
                {
                    timer.Stop(); // Dừng bộ đếm thời gian
                    nhac.Ctlcontrols.stop(); // Dừng nhạc khi có thắng lợi

                    // Điều hướng về Form1
                    MessageBox.Show("Chúc mừng! Bạn đã đạt được 20 điểm và thắng!", "Thắng!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Form1 form1 = new Form1(); // Khởi tạo đối tượng Form1
                    form1.Show(); // Hiển thị Form1
                    this.Close(); // Đóng form hiện tại (chuong1)
                }
            }
        }

        private bool OverlapsWithVien(PictureBox xeDich)
        {
            // Kiểm tra xem xe đối thủ có đè lên đường biên không
            foreach (var vien in vienList)
            {
                if (xeDich.Bounds.IntersectsWith(vien.Bounds))
                {
                    return true; // Nếu có đè, trả về true
                }
            }
            return false; // Nếu không đè, trả về false
        }

        private void CheckCollision(PictureBox xeDich)
        {
            // Kiểm tra va chạm giữa xe người chơi và xe đối thủ
            if (xe.Bounds.IntersectsWith(xeDich.Bounds))
            {
                timer.Stop(); // Dừng bộ đếm thời gian
                nhac.Ctlcontrols.stop(); // Dừng nhạc khi có va chạm

                // Hiển thị thông báo thua và thoát trò chơi
                MessageBox.Show("Bạn đã thua.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Application.Exit(); // Thoát trò chơi
            }
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Xử lý di chuyển xe người chơi khi nhấn phím
            PictureBox xeDo = xe;
            int newLeft = xeDo.Left;

            // Di chuyển xe người chơi sang trái hoặc phải
            if (e.KeyCode == Keys.Left && xeDo.Left - xeSpeed >= 89)
            {
                newLeft = xeDo.Left - xeSpeed;
            }
            else if (e.KeyCode == Keys.Right && xeDo.Right + xeSpeed <= this.ClientSize.Width - 90)
            {
                newLeft = xeDo.Left + xeSpeed;
            }

            // Kiểm tra xem xe người chơi có đè lên đường biên không
            xeDo.Left = newLeft;
            if (OverlapsWithVien(xeDo))
            {
                // Nếu đè lên, trả xe về vị trí cũ
                xeDo.Left = (e.KeyCode == Keys.Left) ? xeDo.Left + xeSpeed : xeDo.Left - xeSpeed;
            }
        }

        private void PlayBackgroundMusic()
        {
            // Phát nhạc nền khi bắt đầu trò chơi
            nhac.URL = musicPath;
            nhac.settings.setMode("loop", true); // Lặp lại nhạc
            nhac.Ctlcontrols.play(); // Bắt đầu phát nhạc
            nhac.Enabled = false; // Ẩn trình phát nhạc
            nhac.Visible = false;
        }

        private void chuong1_Load(object sender, EventArgs e)
        {
            // Sự kiện khi form được load (không dùng)
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            // Sự kiện click vào pictureBox4 (không dùng)
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            // Sự kiện click vào pictureBox5 (không dùng)
        }

        // Hàm di chuyển xe xanh
        private void MoveCar(PictureBox car, int index)
        {
            // Di chuyển xe xanh xuống dưới
            if (car.Top >= this.ClientSize.Height)
            {
                car.Top = -car.Height; // Nếu xe vượt quá chiều cao, đặt lại vị trí
                int newLeft;

                // Đặt vị trí mới cho xe xanh
                do
                {
                    int leftBoundary = 50; // Giới hạn bên trái
                    int rightBoundary = Math.Max(this.ClientSize.Width - 50, leftBoundary + 1); // Giới hạn bên phải

                    newLeft = rand.Next(leftBoundary, rightBoundary);
                    car.Left = newLeft;
                } while (car.Bounds.IntersectsWith(xe.Bounds) || OverlapsWithVien(car)); // Đảm bảo không chồng lấp
            }
        }
    }
}
