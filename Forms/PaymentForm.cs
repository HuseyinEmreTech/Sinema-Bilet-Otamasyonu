using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Database;
using SinemaBiletOtomasyonu.Models;
using SinemaBiletOtomasyonu.Helpers;

namespace SinemaBiletOtomasyonu.Forms
{
    public partial class PaymentForm : Form
    {
        private int filmId;
        private int hallId;
        private List<int> seatIds;
        private decimal totalPrice;
        private CheckBox chkStudent;
        public int SessionId { get; set; } // Property olarak eklendi
        public PaymentForm(int filmId, int hallId, List<int> seatIds, decimal totalPrice, int sessionId)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.filmId = filmId;
            this.hallId = hallId;
            this.seatIds = seatIds;
            this.totalPrice = totalPrice;
            this.SessionId = sessionId;
            
            ModernUIHelper.ApplyTheme(this);
            this.Load += PaymentForm_Load;
            // Paint override tema içinde
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            Film film = DatabaseHelper.GetFilmById(filmId);
            Hall hall = DatabaseHelper.GetHallById(hallId);

            // Receipt Panel (Bilet Kartı Görünümü)
            Panel pnlReceipt = new Panel();
            pnlReceipt.Location = new Point(50, 20);
            pnlReceipt.Size = new Size(400, 160);
            pnlReceipt.BackColor = Color.Transparent; // Paint ile çizeceğiz
            pnlReceipt.Paint += (s, ev) => 
            {
                Graphics g = ev.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = pnlReceipt.ClientRectangle;
                rect.Inflate(-2,-2);
                
                // Kağıt Görünümü (Hafif Sarımtırak veya Beyaz)
                using (SolidBrush paperBrush = new SolidBrush(Color.FromArgb(240, 240, 240)))
                {
                   using (GraphicsPath path = ModernUIHelper.GetRoundedPath(rect, 5))
                   {
                       g.FillPath(paperBrush, path);
                   }
                }
                
                // Tırtıklı Çizgi (Makas İzi)
                using (Pen pen = new Pen(Color.Gray, 2) { DashStyle = DashStyle.Dash })
                {
                    g.DrawLine(pen, 10, 110, rect.Width - 10, 110);
                }
                
                // Film Bilgisi
                TextRenderer.DrawText(g, film.FilmName.ToUpper(), new Font("Segoe UI", 12, FontStyle.Bold), new Point(20, 15), Color.Black);
                TextRenderer.DrawText(g, $"{hall.HallName} | {seatIds.Count} Koltuk", new Font("Segoe UI", 10), new Point(20, 45), Color.DarkGray);
                
                // Fiyat
                TextRenderer.DrawText(g, "TOPLAM TUTAR", new Font("Segoe UI", 8), new Point(20, 120), Color.Gray);
                TextRenderer.DrawText(g, $"{totalPrice:C}", new Font("Segoe UI", 14, FontStyle.Bold), new Point(20, 135), ModernUIHelper.PrimaryColor);
            };
            this.Controls.Add(pnlReceipt);
            
            // Labels (artık gereksiz ama kod çökmesin diye gizleyebiliriz veya kaldırabiliriz)
            lblFilmInfo.Visible = false;
            lblTotalPrice.Visible = false; 
            
            // Input Stili (Receipt altına taşı)
            int startY = 200;
            Label lblName = new Label { Text = "Müşteri Adı Soyadı:", ForeColor = Color.LightGray, Location = new Point(50, startY), AutoSize = true, Font = new Font("Segoe UI", 10) };
            this.Controls.Add(lblName);

            txtCustomerName.Location = new Point(50, startY + 25);
            txtCustomerName.Width = 300;
            // Input Stili
            txtCustomerName.BorderStyle = BorderStyle.FixedSingle; 
            txtCustomerName.BackColor = Color.FromArgb(50, 50, 50); 
            txtCustomerName.ForeColor = Color.White;
            txtCustomerName.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            
            // CheckBox for Student Discount
            chkStudent = new CheckBox();
            chkStudent.Text = "Öğrenci İndirimi (%20)";
            chkStudent.ForeColor = Color.White;
            chkStudent.Font = new Font("Segoe UI", 10);
            chkStudent.Location = new Point(50, startY + 65);
            chkStudent.AutoSize = true;
            chkStudent.CheckedChanged += (s, args) => CalculatePrice();
            this.Controls.Add(chkStudent);

            // Button re-position
            btnConfirmPayment.Location = new Point(50, startY + 110);
            btnConfirmPayment.Size = new Size(180, 45);
            
            btnCancel.Location = new Point(250, startY + 110);
            btnCancel.Size = new Size(100, 45);

            CalculatePrice(); // İlk hesaplama

            StyleButton(btnConfirmPayment, Color.FromArgb(39, 174, 96));
            StyleButton(btnCancel, Color.FromArgb(192, 57, 43));

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

        private void CalculatePrice()
        {
            Film film = DatabaseHelper.GetFilmById(filmId);
            Hall hall = DatabaseHelper.GetHallById(hallId);
            Session session = DatabaseHelper.GetSessionById(SessionId);
            
            decimal basePrice = film.Price;
            decimal multiplier = hall.PriceMultiplier; // Salon çarpanı (VIP vs)
            
            decimal unitPrice = basePrice * multiplier;

            // Matine İndirimi (Saat 14:00'dan önce ise)
            bool isMatinee = false;
            if(session != null)
            {
                 if (TimeSpan.TryParse(session.Time, out TimeSpan time))
                 {
                     if(time.Hours < 14) isMatinee = true;
                 }
            }
            
            if (isMatinee) unitPrice *= 0.90m; // %10 Matine indirimi

            // Öğrenci İndirimi
            if (chkStudent.Checked)
            {
                unitPrice *= 0.80m; // %20 Öğrenci indirimi
            }
            
            this.totalPrice = unitPrice * seatIds.Count;
            // lblTotalPrice.Text = ... No longer used
            
             // Panele force redraw yapmamız lazım ama panele erişim scope dışı. 
             // En temizi pnlReceipt'i sınıf seviyesine taşımak veya buradaki controllerdan bulmak.
             foreach(Control c in this.Controls)
             {
                 if(c is Panel p) p.Invalidate();
             }
        }
        
        private void StyleButton(Button btn, Color color)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.Transparent;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btn.Paint += (s, e) =>
            {
                Rectangle rect = btn.ClientRectangle;
                rect.Inflate(-2, -2);
                
                using(SolidBrush brush = new SolidBrush(color))
                {
                   e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                   using (GraphicsPath path = ModernUIHelper.GetRoundedPath(rect, 10))
                   {
                       e.Graphics.FillPath(brush, path);
                   }
                }
                
                TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, rect, btn.ForeColor, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
        }

        // ... Diğer metodlar (btnConfirmPayment_Click, vb.) orijinaliyle aynı kalabilir ...
        // Ancak context'i korumak için buraya ekliyorum
        private void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Lütfen adınızı soyadınızı girin!", "Uyarı"); // İsim kontrolü
                return;
            }
            try
            {
                foreach (int seatId in seatIds)
                {
                    Ticket ticket = new Ticket
                    {
                        FilmId = filmId,
                        HallId = hallId,
                        SeatId = seatId,
                        SessionId = this.SessionId,
                        CustomerName = txtCustomerName.Text.Trim(),
                        PurchaseDate = DateTime.Now,
                        TotalPrice = this.totalPrice / seatIds.Count, // Birim fiyat (ortalama)
                        TicketCode = GenerateTicketCode()
                    };
                    if (!DatabaseHelper.InsertTicket(ticket))
                    {
                        MessageBox.Show("Hata oluştu!", "Hata");
                        return;
                    }
                }
                MessageBox.Show("Biletleriniz hazır! İyi seyirler.", "Başarılı");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }
        // Benzersiz bilet kodu oluşturur (TCK + Tarih + Rastgele Sayı)
        private string GenerateTicketCode()
        {
            return "TCK" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}