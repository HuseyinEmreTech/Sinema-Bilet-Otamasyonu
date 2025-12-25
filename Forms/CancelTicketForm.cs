using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Database;
using SinemaBiletOtomasyonu.Models;
namespace SinemaBiletOtomasyonu.Forms
{
    public partial class CancelTicketForm : Form
    {
        private Ticket currentTicket;
        public CancelTicketForm()
        {
            InitializeComponent();
            this.Load += CancelTicketForm_Load;
            this.Paint += CancelTicketForm_Paint;
            this.DoubleBuffered = true;
        }
        private void CancelTicketForm_Load(object sender, EventArgs e)
        {
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
        private void CancelTicketForm_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(26, 26, 46),
                Color.FromArgb(15, 52, 96),
                90F);
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTicketCode.Text))
            {
                MessageBox.Show("Lütfen bilet kodunu girin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            currentTicket = DatabaseHelper.GetTicketByCode(txtTicketCode.Text.Trim());
            if (currentTicket == null)
            {
                MessageBox.Show("Bilet bulunamadı! Lütfen kodu kontrol edin.", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblTicketInfo.Text = "Bilet bilgisi bulunamadı.";
                btnCancelTicket.Enabled = false;
                return;
            }
            // Bilet detaylarını göster
            Film film = DatabaseHelper.GetFilmById(currentTicket.FilmId);
            Hall hall = DatabaseHelper.GetHallById(currentTicket.HallId);
            Seat seat = DatabaseHelper.GetSeatById(currentTicket.SeatId);
            lblTicketInfo.Text = $"🎬 Film: {film.FilmName}\n" +
                                $"🏢 Salon: {hall.HallName}\n" +
                                $"🪑 Koltuk: {seat.RowNumber}. Sıra - {seat.SeatNumber}. Koltuk\n" +
                                $"👤 Müşteri: {currentTicket.CustomerName}\n" +
                                $"💰 Tutar: {currentTicket.TotalPrice:C}\n" +
                                $"📅 Tarih: {currentTicket.PurchaseDate:dd/MM/yyyy HH:mm}";
            btnCancelTicket.Enabled = true;
        }
        private void btnCancelTicket_Click(object sender, EventArgs e)
        {
            if (currentTicket == null)
            {
                MessageBox.Show("Önce bilet kodunu arayın!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult result = MessageBox.Show(
                "Bu bileti iptal etmek istediğinizden emin misiniz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                bool success = DatabaseHelper.CancelTicket(currentTicket.TicketCode);
                if (success)
                {
                    MessageBox.Show(
                        "✅ Bilet başarıyla iptal edildi!\nKoltuk tekrar müsait hale geldi.",
                        "Başarılı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    // Formu temizle
                    txtTicketCode.Clear();
                    lblTicketInfo.Text = "Bilet bilgileri burada görünecek...";
                    btnCancelTicket.Enabled = false;
                    currentTicket = null;
                }
                else
                {
                    MessageBox.Show("Bilet iptal edilirken hata oluştu!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.txtTicketCode = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblTicketInfo = new System.Windows.Forms.Label();
            this.btnCancelTicket = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(30, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(220, 46);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "❌ Bilet İptal";

            // lblInstruction
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.BackColor = System.Drawing.Color.Transparent;
            this.lblInstruction.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblInstruction.ForeColor = System.Drawing.Color.White;
            this.lblInstruction.Location = new System.Drawing.Point(35, 90);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(350, 25);
            this.lblInstruction.TabIndex = 1;
            this.lblInstruction.Text = "İptal etmek istediğiniz bilet kodunu girin:";

            // txtTicketCode
            this.txtTicketCode.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtTicketCode.Location = new System.Drawing.Point(40, 130);
            this.txtTicketCode.Name = "txtTicketCode";
            this.txtTicketCode.Size = new System.Drawing.Size(300, 34);
            this.txtTicketCode.TabIndex = 2;
            // this.txtTicketCode.PlaceholderText = "örn: TCK20251225123456789"; // Not supported in .NET Framework 4.7.2

            // btnSearch
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(360, 130);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(150, 34);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "🔍 Ara";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            // lblTicketInfo
            this.lblTicketInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.lblTicketInfo.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTicketInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblTicketInfo.Location = new System.Drawing.Point(40, 190);
            this.lblTicketInfo.Name = "lblTicketInfo";
            this.lblTicketInfo.Padding = new System.Windows.Forms.Padding(15);
            this.lblTicketInfo.Size = new System.Drawing.Size(470, 180);
            this.lblTicketInfo.TabIndex = 4;
            this.lblTicketInfo.Text = "Bilet bilgileri burada görünecek...";

            // btnCancelTicket
            this.btnCancelTicket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnCancelTicket.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelTicket.Enabled = false;
            this.btnCancelTicket.FlatAppearance.BorderSize = 0;
            this.btnCancelTicket.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelTicket.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancelTicket.ForeColor = System.Drawing.Color.White;
            this.btnCancelTicket.Location = new System.Drawing.Point(300, 400);
            this.btnCancelTicket.Name = "btnCancelTicket";
            this.btnCancelTicket.Size = new System.Drawing.Size(210, 50);
            this.btnCancelTicket.TabIndex = 5;
            this.btnCancelTicket.Text = "🗑️ Bileti İptal Et";
            this.btnCancelTicket.UseVisualStyleBackColor = false;
            this.btnCancelTicket.Click += new System.EventHandler(this.btnCancelTicket_Click);

            // btnClose
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(40, 400);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(200, 50);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "⬅ Kapat";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // CancelTicketForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 480);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCancelTicket);
            this.Controls.Add(this.lblTicketInfo);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtTicketCode);
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CancelTicketForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bilet İptal";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.TextBox txtTicketCode;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblTicketInfo;
        private System.Windows.Forms.Button btnCancelTicket;
        private System.Windows.Forms.Button btnClose;
    }
}