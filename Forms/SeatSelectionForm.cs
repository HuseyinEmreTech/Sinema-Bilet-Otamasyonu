using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Database;
using SinemaBiletOtomasyonu.Models;
namespace SinemaBiletOtomasyonu.Forms
{
    public partial class SeatSelectionForm : Form
    {
        private int selectedFilmId;
        private Film selectedFilm;
        private List<Hall> halls;
        private List<Seat> seats;
        private List<int> selectedSeatIds = new List<int>();
        private Button[,] seatButtons;
        public SeatSelectionForm(int filmId)
        {
            InitializeComponent();
            this.selectedFilmId = filmId;
            this.Load += SeatSelectionForm_Load;
            this.Paint += SeatSelectionForm_Paint;
            this.DoubleBuffered = true;
        }
        private void SeatSelectionForm_Load(object sender, EventArgs e)
        {
            selectedFilm = DatabaseHelper.GetFilmById(selectedFilmId);
            halls = DatabaseHelper.GetAllHalls();
            lblFilmName.Text = $"Film: {selectedFilm.FilmName}";
            lblPrice.Text = $"Bilet Fiyatı: {selectedFilm.Price:C}";
            // Salonları combobox'a ekle
            cmbHalls.DataSource = halls;
            cmbHalls.DisplayMember = "HallName";
            cmbHalls.ValueMember = "HallId";
            // Fade-in animasyonu
            this.Opacity = 0;
            Timer fadeTimer = new Timer();
            fadeTimer.Interval = 30;
            fadeTimer.Tick += (s, args) =>
            {
                if (this.Opacity < 1)
                    this.Opacity += 0.05;
                else
                    fadeTimer.Stop();
            };
            fadeTimer.Start();
        }
        private void SeatSelectionForm_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(26, 26, 46),
                Color.FromArgb(15, 52, 96),
                90F);
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
        }
        private void cmbHalls_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbHalls.SelectedValue != null)
            {
                int hallId = (int)cmbHalls.SelectedValue;
                LoadSeats(hallId);
            }
        }
        private void LoadSeats(int hallId)
        {
            panelSeats.Controls.Clear();
            selectedSeatIds.Clear();
            UpdateTotalPrice();
            seats = DatabaseHelper.GetSeatsByHall(hallId);
            Hall hall = DatabaseHelper.GetHallById(hallId);
            int rows = hall.RowCount;
            int cols = hall.ColumnCount;
            seatButtons = new Button[rows, cols];
            int buttonSize = 40;
            int spacing = 5;
            int startX = 20;
            int startY = 50;
            // PERDE (Ekran) gösterimi
            Label screenLabel = new Label();
            screenLabel.Text = "🎬 PERDE";
            screenLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            screenLabel.ForeColor = Color.White;
            screenLabel.BackColor = Color.Transparent;
            screenLabel.AutoSize = true;
            screenLabel.Location = new Point(startX + (cols * (buttonSize + spacing)) / 2 - 50, 10);
            panelSeats.Controls.Add(screenLabel);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Seat seat = seats.FirstOrDefault(s => s.RowNumber == row + 1 && s.SeatNumber == col + 1);
                    Button btnSeat = new Button();
                    btnSeat.Size = new Size(buttonSize, buttonSize);
                    btnSeat.Location = new Point(startX + col * (buttonSize + spacing), startY + row * (buttonSize + spacing));
                    btnSeat.Text = $"{row + 1}{(char)(65 + col)}";
                    btnSeat.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                    btnSeat.FlatStyle = FlatStyle.Flat;
                    btnSeat.FlatAppearance.BorderSize = 0;
                    btnSeat.Cursor = Cursors.Hand;
                    btnSeat.Tag = seat.SeatId;
                    if (seat.IsAvailable)
                    {
                        btnSeat.BackColor = Color.FromArgb(39, 174, 96); // Yeşil - Boş
                        btnSeat.ForeColor = Color.White;
                        btnSeat.Click += SeatButton_Click;
                        btnSeat.MouseEnter += SeatButton_MouseEnter;
                        btnSeat.MouseLeave += SeatButton_MouseLeave;
                    }
                    else
                    {
                        btnSeat.BackColor = Color.FromArgb(192, 57, 43); // Kırmızı - Dolu
                        btnSeat.ForeColor = Color.White;
                        btnSeat.Enabled = false;
                    }
                    seatButtons[row, col] = btnSeat;
                    panelSeats.Controls.Add(btnSeat);
                }
            }
            // Renk açıklamaları
            int legendY = startY + rows * (buttonSize + spacing) + 20;
            AddLegend("🟢 Boş", Color.FromArgb(39, 174, 96), startX, legendY);
            AddLegend("🔴 Dolu", Color.FromArgb(192, 57, 43), startX + 120, legendY);
            AddLegend("🔵 Seçili", Color.FromArgb(52, 152, 219), startX + 240, legendY);
        }
        private void AddLegend(string text, Color color, int x, int y)
        {
            Label legend = new Label();
            legend.Text = text;
            legend.ForeColor = Color.White;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            legend.AutoSize = true;
            legend.Location = new Point(x, y);
            panelSeats.Controls.Add(legend);
        }
        private void SeatButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int seatId = (int)btn.Tag;
            if (selectedSeatIds.Contains(seatId))
            {
                selectedSeatIds.Remove(seatId);
                btn.BackColor = Color.FromArgb(39, 174, 96); // Yeşil - Boş
            }
            else
            {
                selectedSeatIds.Add(seatId);
                btn.BackColor = Color.FromArgb(52, 152, 219); // Mavi - Seçili
            }
            UpdateTotalPrice();
        }
        private void SeatButton_MouseEnter(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (!selectedSeatIds.Contains((int)btn.Tag))
            {
                btn.BackColor = Color.FromArgb(46, 204, 113); // Daha açık yeşil
            }
        }
        private void SeatButton_MouseLeave(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (!selectedSeatIds.Contains((int)btn.Tag))
            {
                btn.BackColor = Color.FromArgb(39, 174, 96); // Orijinal yeşil
            }
        }
        private void UpdateTotalPrice()
        {
            decimal total = selectedSeatIds.Count * selectedFilm.Price;
            lblTotal.Text = $"Toplam: {total:C}";
            lblSelectedSeats.Text = $"Seçili Koltuk: {selectedSeatIds.Count}";
        }
        private void btnProceedToPayment_Click(object sender, EventArgs e)
        {
            if (selectedSeatIds.Count == 0)
            {
                MessageBox.Show("Lütfen en az bir koltuk seçin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int hallId = (int)cmbHalls.SelectedValue;
            decimal totalPrice = selectedSeatIds.Count * selectedFilm.Price;
            PaymentForm paymentForm = new PaymentForm(selectedFilmId, hallId, selectedSeatIds, totalPrice);
            this.Hide();
            paymentForm.ShowDialog();
            this.Close();
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void InitializeComponent()
        {
            this.lblFilmName = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.cmbHalls = new System.Windows.Forms.ComboBox();
            this.panelSeats = new System.Windows.Forms.Panel();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblSelectedSeats = new System.Windows.Forms.Label();
            this.btnProceedToPayment = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblFilmName
            this.lblFilmName.AutoSize = true;
            this.lblFilmName.BackColor = System.Drawing.Color.Transparent;
            this.lblFilmName.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblFilmName.ForeColor = System.Drawing.Color.White;
            this.lblFilmName.Location = new System.Drawing.Point(20, 20);
            this.lblFilmName.Name = "lblFilmName";
            this.lblFilmName.Size = new System.Drawing.Size(150, 37);
            this.lblFilmName.TabIndex = 0;
            this.lblFilmName.Text = "Film: ...";

            // lblPrice
            this.lblPrice.AutoSize = true;
            this.lblPrice.BackColor = System.Drawing.Color.Transparent;
            this.lblPrice.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblPrice.ForeColor = System.Drawing.Color.White;
            this.lblPrice.Location = new System.Drawing.Point(22, 60);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(120, 28);
            this.lblPrice.TabIndex = 1;
            this.lblPrice.Text = "Fiyat: 0 ₺";

            // cmbHalls
            this.cmbHalls.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHalls.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbHalls.FormattingEnabled = true;
            this.cmbHalls.Location = new System.Drawing.Point(27, 100);
            this.cmbHalls.Name = "cmbHalls";
            this.cmbHalls.Size = new System.Drawing.Size(250, 33);
            this.cmbHalls.TabIndex = 2;
            this.cmbHalls.SelectedIndexChanged += new System.EventHandler(this.cmbHalls_SelectedIndexChanged);

            // panelSeats
            this.panelSeats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSeats.BackColor = System.Drawing.Color.Transparent;
            this.panelSeats.Location = new System.Drawing.Point(27, 145);
            this.panelSeats.Name = "panelSeats";
            this.panelSeats.Size = new System.Drawing.Size(745, 330);
            this.panelSeats.TabIndex = 3;

            // lblTotal
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.AutoSize = true;
            this.lblTotal.BackColor = System.Drawing.Color.Transparent;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(215)))), ((int)(((byte)(0)))));
            this.lblTotal.Location = new System.Drawing.Point(550, 490);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(120, 32);
            this.lblTotal.TabIndex = 4;
            this.lblTotal.Text = "Toplam: 0 ₺";

            // lblSelectedSeats
            this.lblSelectedSeats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSelectedSeats.AutoSize = true;
            this.lblSelectedSeats.BackColor = System.Drawing.Color.Transparent;
            this.lblSelectedSeats.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblSelectedSeats.ForeColor = System.Drawing.Color.White;
            this.lblSelectedSeats.Location = new System.Drawing.Point(30, 495);
            this.lblSelectedSeats.Name = "lblSelectedSeats";
            this.lblSelectedSeats.Size = new System.Drawing.Size(170, 28);
            this.lblSelectedSeats.TabIndex = 5;
            this.lblSelectedSeats.Text = "Seçili Koltuk: 0";

            // btnProceedToPayment
            this.btnProceedToPayment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProceedToPayment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(69)))), ((int)(((byte)(96)))));
            this.btnProceedToPayment.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProceedToPayment.FlatAppearance.BorderSize = 0;
            this.btnProceedToPayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProceedToPayment.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnProceedToPayment.ForeColor = System.Drawing.Color.White;
            this.btnProceedToPayment.Location = new System.Drawing.Point(552, 530);
            this.btnProceedToPayment.Name = "btnProceedToPayment";
            this.btnProceedToPayment.Size = new System.Drawing.Size(220, 50);
            this.btnProceedToPayment.TabIndex = 6;
            this.btnProceedToPayment.Text = "💳 Ödemeye Geç";
            this.btnProceedToPayment.UseVisualStyleBackColor = false;
            this.btnProceedToPayment.Click += new System.EventHandler(this.btnProceedToPayment_Click);

            // btnBack
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(27, 530);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(150, 50);
            this.btnBack.TabIndex = 7;
            this.btnBack.Text = "⬅ Geri";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);

            // SeatSelectionForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnProceedToPayment);
            this.Controls.Add(this.lblSelectedSeats);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.panelSeats);
            this.Controls.Add(this.cmbHalls);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.lblFilmName);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "SeatSelectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Koltuk Seçimi";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.Label lblFilmName;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.ComboBox cmbHalls;
        private System.Windows.Forms.Panel panelSeats;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblSelectedSeats;
        private System.Windows.Forms.Button btnProceedToPayment;
        private System.Windows.Forms.Button btnBack;
    }
}