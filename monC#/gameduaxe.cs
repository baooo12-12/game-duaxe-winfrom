using AxWMPLib; // Thư viện để phát âm thanh
using System;
using System.Windows.Forms;
using WMPLib;
using System.Linq;

namespace monC_
{
    public partial class gameduaxe : Form
    {
        private int speed = 5; // Tốc độ di chuyển của các vạch trắng
        private int xeSpeed = 20; // Tốc độ di chuyển của xe vàng
        private int carSpeed = 5; // Tốc độ di chuyển của xe xanh
        private int leftBoundary = 150; // Tọa độ biên trái của đường xám
        private int rightBoundary; // Tọa độ biên phải của đường xám
        private int score = 0; // Biến lưu trữ điểm số
        private bool[] passedCars = new bool[4]; // Đánh dấu nếu đã tính điểm cho xe xanh
        private bool hasShownVictoryMessage = false; // Biến theo dõi thông báo chiến thắng
        private Random rand = new Random(); // Đối tượng ngẫu nhiên để đặt vị trí xe
        private AxWindowsMediaPlayer collisionPlayer; // Đối tượng phát âm thanh va chạm

        public gameduaxe()
        {
            InitializeComponent();
            InitializeGame(); // Khởi tạo game
        }

        private void InitializeGame()
        {
            timer1.Interval = 20; // Thời gian giữa các lần tick của timer
            timer1.Tick += new EventHandler(timer1_Tick); // Đăng ký sự kiện tick của timer
            timer1.Start(); // Bắt đầu timer
            this.KeyDown += new KeyEventHandler(gameduaxe_KeyDown); // Đăng ký sự kiện nhấn phím

            rightBoundary = this.ClientSize.Width - 150; // Thiết lập tọa độ biên phải
            diem.Text = score.ToString(); // Hiển thị điểm số
            diem.Enabled = false; // Không cho phép chỉnh sửa điểm
            diem.BackColor = this.BackColor; // Đặt màu nền của điểm giống màu nền form

            PlayBackgroundMusic(); // Phát nhạc nền
            InitializeCollisionSound(); // Khởi tạo âm thanh va chạm
        }

        private void InitializeCollisionSound()
        {
            collisionPlayer = new AxWindowsMediaPlayer(); // Tạo đối tượng phát âm thanh va chạm
            collisionPlayer.CreateControl(); // Tạo điều khiển cho đối tượng
            collisionPlayer.URL = @"D:\C#\bai1\nhac\va_cham.mp3"; // Đường dẫn âm thanh va chạm
            collisionPlayer.settings.volume = 100; // Đặt âm lượng
        }

        private void PlayBackgroundMusic()
        {
            nhac.URL = @"D:\C#\bai1\nhac\game.mp3"; // Đường dẫn âm thanh nền
            nhac.settings.setMode("loop", true); // Đặt chế độ lặp lại cho nhạc nền
            nhac.Ctlcontrols.play(); // Phát nhạc nền
            nhac.Enabled = false; // Không cho phép điều khiển nhạc nền từ giao diện
            nhac.Visible = false; // Ẩn nhạc nền
        }

        private void gameduaxe_KeyDown(object sender, KeyEventArgs e)
        {
            // Điều khiển di chuyển xe vàng bằng phím trái và phải
            if (e.KeyCode == Keys.Left && xe.Left > leftBoundary)
            {
                xe.Left -= xeSpeed; // Di chuyển xe sang trái
            }
            else if (e.KeyCode == Keys.Right && xe.Right < rightBoundary)
            {
                xe.Left += xeSpeed; // Di chuyển xe sang phải
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MoveGameElements(); // Di chuyển các phần tử trong game
            CheckCollision(); // Kiểm tra va chạm
            CheckPass(); // Kiểm tra xem đã vượt qua xe xanh chưa
        }

        private void MoveGameElements()
        {
            // Di chuyển các vạch trắng
            MoveLine(vac1);
            MoveLine(vac2);
            MoveLine(vac3);

            // Di chuyển các xe xanh
            MoveCar(xe1, 0);
            MoveCar(xe2, 1);
            MoveCar(xe3, 2);
            MoveCar(xe4, 3);
        }

        private void MoveLine(PictureBox line)
        {
            // Di chuyển vạch trắng xuống dưới
            if (line.Top >= this.ClientSize.Height)
            {
                line.Top = -line.Height; // Nếu vạch vượt quá chiều cao, đặt lại vị trí
            }
            else
            {
                line.Top += speed; // Di chuyển vạch xuống
            }
        }

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
                    newLeft = rand.Next(leftBoundary, rightBoundary - car.Width); // Tọa độ ngẫu nhiên trong biên
                } while (IsOverlapping(newLeft, car.Height, index)); // Kiểm tra xem có bị chồng lên không

                car.Left = newLeft; // Thiết lập vị trí mới cho xe xanh
                passedCars[index] = false; // Đánh dấu xe xanh chưa được tính điểm
            }
            else
            {
                car.Top += carSpeed; // Di chuyển xe xanh xuống
            }
        }

        private bool IsOverlapping(int newLeft, int height, int currentIndex)
        {
            // Kiểm tra xem có chồng lên xe xanh khác không
            for (int i = 0; i < passedCars.Length; i++)
            {
                if (i != currentIndex)
                {
                    PictureBox otherCar = this.Controls.Find($"xe{i + 1}", true).FirstOrDefault() as PictureBox;
                    if (otherCar != null && otherCar.Visible)
                    {
                        // Kiểm tra va chạm
                        if (newLeft < otherCar.Left + otherCar.Width &&
                            newLeft + height > otherCar.Left &&
                            xe.Top < otherCar.Top + otherCar.Height &&
                            xe.Top + height > otherCar.Top)
                        {
                            return true; // Có chồng lên
                        }
                    }
                }
            }
            return false; // Không chồng lên
        }

        private void CheckPass()
        {
            // Kiểm tra xem xe vàng có vượt qua xe xanh không
            for (int i = 0; i < passedCars.Length; i++)
            {
                if (xe.Top < this.Controls.Find($"xe{i + 1}", true).FirstOrDefault().Top && !passedCars[i])
                {
                    score++; // Tăng điểm số
                    passedCars[i] = true; // Đánh dấu đã vượt qua xe
                }
            }

            diem.Text = score.ToString(); // Cập nhật điểm số

            // Kiểm tra chiến thắng
            if (score == 10
                
                && !hasShownVictoryMessage)
            {
                hasShownVictoryMessage = true; // Đánh dấu đã hiển thị thông báo chiến thắng
                EndGame(); // Kết thúc game
            }
        }

        private void EndGame()
        {
            timer1.Stop(); // Dừng timer
            nhac.Ctlcontrols.stop(); // Dừng nhạc nền
            MessageBox.Show("Chúc mừng! Bạn đã đạt 10 điểm! Sang chương tiếp theo", "Chiến thắng", MessageBoxButtons.OK);
            OpenNextForm(); // Mở chương tiếp theo
        }

        private void OpenNextForm()
        {
            nhac.Ctlcontrols.stop(); // Dừng nhạc nền
            this.Hide(); // Ẩn form hiện tại
            chuong1 nextForm = new chuong1(); // Tạo form mới
            nextForm.ShowDialog(); // Hiển thị form mới
            this.Close(); // Đóng form hiện tại
        }

        private void CheckCollision()
        {
            // Kiểm tra va chạm giữa xe vàng và các xe xanh
            if (xe.Bounds.IntersectsWith(xe1.Bounds) ||
                xe.Bounds.IntersectsWith(xe2.Bounds) ||
                xe.Bounds.IntersectsWith(xe3.Bounds) ||
                xe.Bounds.IntersectsWith(xe4.Bounds))
            {
                GameOver(); // Gọi hàm kết thúc game nếu có va chạm
            }
        }

        private void GameOver()
        {
            timer1.Stop(); // Dừng timer
            nhac.Ctlcontrols.stop(); // Dừng nhạc nền
            PlayCollisionSound(); // Phát âm thanh va chạm
            MessageBox.Show("Game Over! Xe vàng đã va chạm!", "Thông báo", MessageBoxButtons.OK);
            Application.Exit(); // Đóng ứng dụng khi game over
        }

        private void PlayCollisionSound()
        {
            collisionPlayer.Ctlcontrols.play(); // Phát âm thanh va chạm
        }

        private void gameduaxe_Load(object sender, EventArgs e)
        {
            // Hàm gọi khi form được tải
        }

        private void xe_Click(object sender, EventArgs e)
        {
            pictureBox1.BorderStyle = BorderStyle.None; // Thay đổi kiểu viền khi click vào xe
        }
    }
}
