using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Database;
using SinemaBiletOtomasyonu.Models;
namespace SinemaBiletOtomasyonu.Forms
{
    public partial class PaymentForm : Form
    {
        private int filmId;
        private int hallId;
        private List<int> seatIds;
        private decimal totalPrice;
        public PaymentForm(int filmId, int hallId, List<int> seatIds, decimal totalPrice)
        {
            InitializeComponent();
            this.filmId = filmId;
            this.hallId = hallId;
            this.seatIds = seatIds;
            this.totalPrice = totalPrice;
            this.Load += PaymentForm_Load;
            this.Paint += PaymentForm_Paint;
            this.DoubleBuffered = true;
        }
        private void PaymentForm_Load(object sender, EventArgs e)
        {
            Film film = DatabaseHelper.GetFilmById(filmId);
            Hall hall = DatabaseHelper.GetHallById(hallId);
            lblFilmInfo.Text = $"Film: {film.FilmName}\nSalon: {hall.HallName}\nKoltuk Sayısı: {seatIds.Count}";
            lblTotalPrice.Text = $"Toplam Tutar: {totalPrice:C}";
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
        private void PaymentForm_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(26, 26, 46),
                Color.FromArgb(15, 52, 96),
                90F);
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
        }
        private void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Lütfen adınızı soyadınızı girin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                // Her koltuk için bilet oluştur
                foreach (int seatId in seatIds)
                {
                    Ticket ticket = new Ticket
                    {
                        FilmId = filmId,
                        HallId = hallId,
                        SeatId = seatId,
                        CustomerName = txtCustomerName.Text.Trim(),
                        PurchaseDate = DateTime.Now,
                        TotalPrice = DatabaseHelper.GetFilmById(filmId).Price,
                        TicketCode = GenerateTicketCode()
                    };
                    bool success = DatabaseHelper.InsertTicket(ticket);
                    if (!success)
                    {
                        MessageBox.Show("Bilet kaydedilirken hata oluştu!", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                // Başarı mesajı
                MessageBox.Show(
                    $"✅ Bilet satın alımı başarılı!\n\n" +
                    $"Müşteri: {txtCustomerName.Text}\n" +
                    $"Koltuk Sayısı: {seatIds.Count}\n" +
                    $"Toplam: {totalPrice:C}\n\n" +
                    $"Biletleriniz kaydedildi. İyi seyirler!",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GenerateTicketCode()
        {
            // Benzersiz bilet kodu oluştur (örn: TCK20251225174532)
            return "TCK" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblFilmInfo = new System.Windows.Forms.Label();
            this.lblTotalPrice = new System.Windows.Forms.Label();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.btnConfirmPayment = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(30, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(280, 46);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "💳 Ödeme İşlemi";

            // lblFilmInfo
            this.lblFilmInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblFilmInfo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblFilmInfo.ForeColor = System.Drawing.Color.White;
            this.lblFilmInfo.Location = new System.Drawing.Point(35, 100);
            this.lblFilmInfo.Name = "lblFilmInfo";
            this.lblFilmInfo.Size = new System.Drawing.Size(500, 100);
            this.lblFilmInfo.TabIndex = 1;
            this.lblFilmInfo.Text = "Film: ...\nSalon: ...\nKoltuk: ...";

            // lblTotalPrice
            this.lblTotalPrice.AutoSize = true;
            this.lblTotalPrice.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalPrice.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTotalPrice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(215)))), ((int)(((byte)(0)))));
            this.lblTotalPrice.Location = new System.Drawing.Point(32, 210);
            this.lblTotalPrice.Name = "lblTotalPrice";
            this.lblTotalPrice.Size = new System.Drawing.Size(220, 37);
            this.lblTotalPrice.TabIndex = 2;
            this.lblTotalPrice.Text = "Toplam: 0 ₺";

            // lblCustomerName
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.BackColor = System.Drawing.Color.Transparent;
            this.lblCustomerName.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblCustomerName.ForeColor = System.Drawing.Color.White;
            this.lblCustomerName.Location = new System.Drawing.Point(35, 270);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(160, 28);
            this.lblCustomerName.TabIndex = 3;
            this.lblCustomerName.Text = "Adınız Soyadınız:";

            // txtCustomerName
            this.txtCustomerName.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtCustomerName.Location = new System.Drawing.Point(40, 310);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(400, 34);
            this.txtCustomerName.TabIndex = 4;

            // btnConfirmPayment
            this.btnConfirmPayment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnConfirmPayment.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmPayment.FlatAppearance.BorderSize = 0;
            this.btnConfirmPayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmPayment.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnConfirmPayment.ForeColor = System.Drawing.Color.White;
            this.btnConfirmPayment.Location = new System.Drawing.Point(290, 380);
            this.btnConfirmPayment.Name = "btnConfirmPayment";
            this.btnConfirmPayment.Size = new System.Drawing.Size(250, 60);
            this.btnConfirmPayment.TabIndex = 5;
            this.btnConfirmPayment.Text = "✅ Ödemeyi Onayla";
            this.btnConfirmPayment.UseVisualStyleBackColor = false;
            this.btnConfirmPayment.Click += new System.EventHandler(this.btnConfirmPayment_Click);

            // btnCancel
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(40, 380);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(200, 60);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "❌ İptal";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // PaymentForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 480);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirmPayment);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.lblCustomerName);
            this.Controls.Add(this.lblTotalPrice);
            this.Controls.Add(this.lblFilmInfo);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PaymentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ödeme";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblFilmInfo;
        private System.Windows.Forms.Label lblTotalPrice;
        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Button btnConfirmPayment;
        private System.Windows.Forms.Button btnCancel;
    }
}