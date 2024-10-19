using System.Windows.Forms;
using System;
using System.Linq;

namespace monC_
{
    public partial class gameduaxe : Form
    {
        private int speed = 5; // Tốc độ di chuyển của các vạch trắng
        private int xeSpeed = 15; // Tốc độ di chuyển của xe vàng
        private int carSpeed = 5; // Tốc độ di chuyển của xe xanh
        private int leftBoundary = 150; // Tọa độ biên trái của đường xám
        private int rightBoundary; // Tọa độ biên phải của đường xám
        private int score = 0; // Biến lưu trữ điểm số
        private bool[] passedCars = new bool[4]; // Đánh dấu nếu đã tính điểm cho xe xanh
        private bool hasShownVictoryMessage = false; // Biến theo dõi thông báo chiến thắng

        public gameduaxe()
        {
            InitializeComponent();

            // Cài đặt timer và sự kiện phím
            timer1.Interval = 20;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();

            this.KeyDown += new KeyEventHandler(gameduaxe_KeyDown);

            // Tính toán tọa độ biên phải dựa trên chiều rộng của đường
            rightBoundary = this.ClientSize.Width - 150;

            // Đặt điểm ban đầu
            diem.Text = score.ToString(); // `diem` là Label thay vì TextBox
            diem.Enabled = false; // Đặt Label diem thành không thể tương tác
            diem.BackColor = this.BackColor; // Đặt màu nền của Label giống với màu nền của Form
        }

        private void gameduaxe_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && xe.Left > leftBoundary)
            {
                xe.Left -= xeSpeed;
            }
            else if (e.KeyCode == Keys.Right && xe.Right < rightBoundary)
            {
                xe.Left += xeSpeed;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MoveLine(vac1);
            MoveLine(vac2);
            MoveLine(vac3);

            MoveCar(xe1, 0);
            MoveCar(xe2, 1);
            MoveCar(xe3, 2);
            MoveCar(xe4, 3);

            CheckCollision();
            CheckPass(); // Gọi hàm kiểm tra vượt qua
        }

        private void MoveLine(PictureBox line)
        {
            if (line.Top >= this.ClientSize.Height)
            {
                line.Top = -line.Height;
            }
            else
            {
                line.Top += speed;
            }
        }

        private void MoveCar(PictureBox car, int index)
        {
            if (car.Top >= this.ClientSize.Height)
            {
                Random rand = new Random();
                int newLeft;

                // Lặp cho đến khi tìm được vị trí hợp lệ cho xe
                do
                {
                    newLeft = rand.Next(leftBoundary, rightBoundary - car.Width);
                } while (IsOverlapping(newLeft, car.Height, index));

                car.Top = -car.Height;
                car.Left = newLeft;
                passedCars[index] = false; // Đặt lại trạng thái khi xe vượt qua
            }
            else
            {
                car.Top += carSpeed;
            }
        }

        // Kiểm tra xem xe có bị chồng lên xe khác không
        private bool IsOverlapping(int newLeft, int height, int currentIndex)
        {
            // Kiểm tra với tất cả các xe khác
            for (int i = 0; i < passedCars.Length; i++)
            {
                if (i != currentIndex)
                {
                    // Lấy xe khác
                    PictureBox otherCar = this.Controls.Find($"xe{i + 1}", true).FirstOrDefault() as PictureBox;
                    if (otherCar != null && otherCar.Visible)
                    {
                        // Kiểm tra chồng lên
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

        // Kiểm tra nếu xe vàng đã vượt qua xe xanh và tính điểm
        private void CheckPass()
        {
            // Kiểm tra từng xe xanh
            if (xe.Top < xe1.Top && !passedCars[0])
            {
                score++;
                passedCars[0] = true; // Đánh dấu là đã tính điểm cho xe1
            }

            if (xe.Top < xe2.Top && !passedCars[1])
            {
                score++;
                passedCars[1] = true; // Đánh dấu là đã tính điểm cho xe2
            }

            if (xe.Top < xe3.Top && !passedCars[2])
            {
                score++;
                passedCars[2] = true; // Đánh dấu là đã tính điểm cho xe3
            }

            if (xe.Top < xe4.Top && !passedCars[3])
            {
                score++;
                passedCars[3] = true; // Đánh dấu là đã tính điểm cho xe4
            }

            // Cập nhật điểm lên Label
            diem.Text = score.ToString(); // `diem` là Label thay vì TextBox

            // Hiển thị thông báo khi đạt 50 điểm
            if (score == 50 && !hasShownVictoryMessage)
            {
                hasShownVictoryMessage = true; // Đánh dấu là đã hiển thị thông báo
                timer1.Stop(); // Dừng game
                MessageBox.Show("Chúc mừng! Bạn đã đạt 50 điểm!", "Chiến thắng", MessageBoxButtons.OK);
                ShowNextLevelDialog(); // Hiện thông báo để chuyển sang màn tiếp theo
            }
        }

        private void ShowNextLevelDialog()
        {
            DialogResult result = MessageBox.Show("Bạn có muốn tiếp tục sang màn tiếp theo không?", "Chuyển Màn", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                // Tăng tốc độ xe xanh
                carSpeed += 2; // Tăng tốc độ thêm 2

                // Reset trạng thái cho màn chơi mới
                ResetForNextLevel();
                timer1.Start(); // Khởi động lại game sau khi chuyển màn
            }
            else
            {
                Application.Exit(); // Đóng ứng dụng nếu người dùng không muốn tiếp tục
            }
        }

        private void ResetForNextLevel()
        {
            // Đặt lại trạng thái cho xe và điểm số
            xe.Left = (this.ClientSize.Width - xe.Width) / 2;
            xe.Top = this.ClientSize.Height - xe.Height - 10;

            xe1.Top = -xe1.Height;
            xe2.Top = -xe2.Height;
            xe3.Top = -xe3.Height;
            xe4.Top = -xe4.Height;

            // Đặt lại trạng thái điểm đã qua
            Array.Clear(passedCars, 0, passedCars.Length);
        }


        private void CheckCollision()
        {
            if (xe.Bounds.IntersectsWith(xe1.Bounds) ||
                xe.Bounds.IntersectsWith(xe2.Bounds) ||
                xe.Bounds.IntersectsWith(xe3.Bounds) ||
                xe.Bounds.IntersectsWith(xe4.Bounds))
            {
                timer1.Stop();

                DialogResult result = MessageBox.Show("Game Over! Xe vàng đã va chạm! Bạn có muốn chơi lại không?", "Thông báo", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    RestartGame();
                }
                else if (result == DialogResult.No)
                {
                    Application.Exit();
                }
            }
        }

        private void RestartGame()
        {
            xe.Left = (this.ClientSize.Width - xe.Width) / 2;
            xe.Top = this.ClientSize.Height - xe.Height - 10;

            xe1.Top = -xe1.Height;
            xe2.Top = -xe2.Height;
            xe3.Top = -xe3.Height;
            xe4.Top = -xe4.Height;

            score = 0; // Đặt lại điểm số
            Array.Clear(passedCars, 0, passedCars.Length); // Đặt lại trạng thái xe
            diem.Text = score.ToString(); // Đặt lại điểm hiển thị trong Label

            hasShownVictoryMessage = false; // Đặt lại trạng thái thông báo chiến thắng cho game mới

            timer1.Start();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
    }
}
