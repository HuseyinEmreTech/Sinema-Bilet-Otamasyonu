using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Database;
using SinemaBiletOtomasyonu.Models;
using SinemaBiletOtomasyonu.Helpers;

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
            this.DoubleBuffered = true;
            this.selectedFilmId = filmId;
            // Modern Tema Uygula
            ModernUIHelper.ApplyTheme(this);
            
            this.Load += SeatSelectionForm_Load;
            // Paint override tema içinde yapıldı ama screen efekti için ekleme yapabiliriz
            this.Paint += SeatSelectionForm_Paint_Custom; 
        }

        private void SeatSelectionForm_Load(object sender, EventArgs e)
        {
            selectedFilm = DatabaseHelper.GetFilmById(selectedFilmId);
            halls = DatabaseHelper.GetAllHalls();
            lblFilmName.Text = $" {selectedFilm.FilmName}";
            lblPrice.Text = $"{selectedFilm.Price:C}";
            
            // ComboBox Modernleştirme (Basitçe)
            cmbHalls.FlatStyle = FlatStyle.Flat;
            cmbHalls.BackColor = ModernUIHelper.DarkBackground;
            cmbHalls.ForeColor = Color.White;
            
            // Salonları combobox'a ekle
            cmbHalls.DisplayMember = "HallName";
            cmbHalls.ValueMember = "HallId";
            cmbHalls.DataSource = halls;
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
            
            
            // Sessions ComboBox Styling
            cmbSessions.FlatStyle = FlatStyle.Flat;
            cmbSessions.BackColor = ModernUIHelper.DarkBackground;
            cmbSessions.ForeColor = Color.White;
            cmbSessions.DisplayMember = "Time";
            cmbSessions.ValueMember = "SessionId";

            StyleButton(btnProceedToPayment, ModernUIHelper.PrimaryColor);
            StyleButton(btnBack, Color.Gray);
        }
        
        private void StyleButton(Button btn, Color color)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.Transparent; // Paint event ile çizeceğiz
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btn.Paint += (s, e) =>
            {
                Rectangle rect = btn.ClientRectangle;
                rect.Inflate(-2, -2);
                
                // Arka plan rengini glass efektiyle karıştır
                using(SolidBrush brush = new SolidBrush(Color.FromArgb(200, color)))
                {
                   // ModernUIHelper.DrawGlassPanel(e.Graphics, rect, 20); // Üstüne renkli dolgu
                   e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                   using (GraphicsPath path = ModernUIHelper.GetRoundedPath(rect, 20))
                   {
                       e.Graphics.FillPath(brush, path);
                   }
                }
                
                TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, rect, btn.ForeColor, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
        }

        private void SeatSelectionForm_Paint_Custom(object sender, PaintEventArgs e)
        {
            // Perde (Screen) Efekti - Minimalist
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            int screenWidth = 200;
            // int screenHeight = 6; // Unused
            int x = (panelSeats.Width - screenWidth) / 2 + panelSeats.Left;
            int y = panelSeats.Top - 30; 
            
            // Düz Çizgi Perde (Kavisli yerinde düz daha modern durabilir veya hafif kavis)
            // Kavisli yapalım
            using (GraphicsPath screenPath = new GraphicsPath())
            {
                screenPath.AddArc(x, y, screenWidth, 20, 180, 180);
                using (Pen p = new Pen(Color.Gray, 3))
                {
                    g.DrawPath(p, screenPath);
                }
            }
            
            TextRenderer.DrawText(g, "SAHNE", new Font("Segoe UI", 9, FontStyle.Regular), 
                new Rectangle(x, y + 15, screenWidth, 20), Color.Gray, TextFormatFlags.HorizontalCenter);

            // LEGEND (Açıklamalar)
            int ly = cmbSessions.Bottom + 10;
            int lx = panelSeats.Right - 150;
            if(lx < panelSeats.Left + 10) lx = panelSeats.Left + 10;

            DrawLegendItem(g, "Boş", ModernUIHelper.SecondaryColor, ref lx, ly);
            DrawLegendItem(g, "Dolu", Color.FromArgb(192, 57, 43), ref lx, ly);
            DrawLegendItem(g, "Seçili", ModernUIHelper.PrimaryColor, ref lx, ly);
        }

        private void DrawLegendItem(Graphics g, string text, Color color, ref int x, int y)
        {
            Rectangle rect = new Rectangle(x, y, 20, 20);
            using (SolidBrush b = new SolidBrush(color))
            {
                g.FillEllipse(b, rect);
            }
            TextRenderer.DrawText(g, text, new Font("Segoe UI", 8), new Point(x + 25, y + 2), Color.LightGray);
            x -= 70; // Sola doğru git (yada sağa, layouta göre değişir. Burada Right aligned başladık gibi)
        }

        private void cmbHalls_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbHalls.SelectedValue != null && cmbHalls.SelectedValue is int)
            {
                int hallId = (int)cmbHalls.SelectedValue;
                // Seansları güncelle
                var sessions = DatabaseHelper.GetSessionsByFilmAndHall(selectedFilmId, hallId);
                cmbSessions.DataSource = sessions;
                // İlk seans seçili gelirse, LoadSeats değişen seansa göre tetiklenmeli
                if (sessions.Any())
                {
                     // Otomatik ilkini seçer ve cmbSessions_SelectedIndexChanged tetiklenir
                }
                else
                {
                     // Seans yoksa koltukları temizle ve kullanıcıyı uyar
                     LoadSeats(hallId, -1);
                }
            }
        }

        private void cmbSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
             if(cmbSessions.SelectedValue != null && cmbSessions.SelectedValue is int)
             {
                 int hallId = (int)cmbHalls.SelectedValue;
                 int sessionId = (int)cmbSessions.SelectedValue;
                 LoadSeats(hallId, sessionId);
             }
        }

        private void LoadSeats(int hallId, int sessionId)
        {
            panelSeats.Controls.Clear();
            selectedSeatIds.Clear();
            UpdateTotalPrice();
            
            if(sessionId == -1) return;

            seats = DatabaseHelper.GetSeatsByHall(hallId);
            List<int> occupiedSeatIds = DatabaseHelper.GetOccupiedSeatIds(sessionId);
            
            Hall hall = DatabaseHelper.GetHallById(hallId);
            int rows = hall.RowCount;
            int cols = hall.ColumnCount;
            seatButtons = new Button[rows, cols];
            int buttonSize = 40;
            int spacing = 10;
            
            // Koltukları ortala
            int totalWidth = cols * (buttonSize + spacing);
            int startX = (panelSeats.Width - totalWidth) / 2;
            if (startX < 0) startX = 20;
            int startY = 40; // Perde için üstten boşluk bırak
            
            // Koltukları oluştur döngüsü
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Seat seat = seats.FirstOrDefault(s => s.RowNumber == row + 1 && s.SeatNumber == col + 1);
                    Button btnSeat = new Button();
                    btnSeat.Size = new Size(buttonSize, buttonSize);
                    btnSeat.Location = new Point(startX + col * (buttonSize + spacing), startY + row * (buttonSize + spacing));
                    btnSeat.Text = $"{row + 1}{(char)(65 + col)}";
                    btnSeat.Font = new Font("Segoe UI", 7, FontStyle.Bold); // Küçük font
                    btnSeat.FlatStyle = FlatStyle.Flat;
                    btnSeat.FlatAppearance.BorderSize = 0;
                    btnSeat.Cursor = Cursors.Hand;
                    btnSeat.Tag = seat.SeatId;
                    
                    bool isOccupied = occupiedSeatIds.Contains(seat.SeatId);
                    // interactionEnabled removed (unused)
                    
                    // Görsel İptal Özelliği: Dolu olanlara da tıklama izni veriyoruz
                    
                    btnSeat.BackColor = Color.Transparent; // Custom paint
                    
                    // Event'i paint ile bağla
                    btnSeat.Paint += (s, ev) => DrawSeat(ev.Graphics, btnSeat.ClientRectangle, 
                        selectedSeatIds.Contains(seat.SeatId) ? ModernUIHelper.PrimaryColor : 
                        (isOccupied ? Color.FromArgb(192, 57, 43) : ModernUIHelper.SecondaryColor), 
                        btnSeat.Text);
                        
                    // Her türlü tıklanabilir, mantık SeatButton_Click içinde
                    btnSeat.Click += (s, args) => SeatButton_Click(s, args, isOccupied);

                    seatButtons[row, col] = btnSeat;
                    panelSeats.Controls.Add(btnSeat);
                }
            }
        }
        
        private void DrawSeat(Graphics g, Rectangle rect, Color color, string text)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            rect.Inflate(-4, -4);
            
            // Koltuk Şekli - Basit Yuvarlatılmış Kare (Flat)
            using (GraphicsPath path = ModernUIHelper.GetRoundedPath(rect, 8))
            {
                 using (SolidBrush brush = new SolidBrush(color))
                 {
                     g.FillPath(brush, path);
                 }
            }
            
            // Seçili veya Dolu değilse border ekle
            if (color == ModernUIHelper.SecondaryColor) // Boş
            {
                using (Pen pen = new Pen(Color.FromArgb(50, 50, 50), 1))
                {
                     using(var path = ModernUIHelper.GetRoundedPath(rect, 8))
                     {
                         g.DrawPath(pen, path);
                     }
                }
            }

            // Text
            Color textColor = (color == ModernUIHelper.SecondaryColor) ? Color.Black : Color.White;
            TextRenderer.DrawText(g, text, new Font("Segoe UI", 7), rect, textColor, 
                  TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // Koltuk tıklama olayı
        private void SeatButton_Click(object sender, EventArgs e, bool isOccupied)
        {
            Button btn = (Button)sender;
            int seatId = (int)btn.Tag;
            
            if (isOccupied)
            {
                // Görsel İptal İşlemi
                int sessionId = (int)cmbSessions.SelectedValue;
                Ticket ticket = DatabaseHelper.GetTicketBySessionAndSeat(sessionId, seatId);
                
                if (ticket != null)
                {
                    DialogResult result = MessageBox.Show(
                        $"Dolu Koltuk!\n\nBilet Bilgisi:\nMüşteri: {ticket.CustomerName}\nBu bileti İPTAL etmek istiyor musunuz?",
                        "Bilet İptali",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                        
                    if(result == DialogResult.Yes)
                    {
                        DatabaseHelper.CancelTicket(ticket.TicketCode);
                        MessageBox.Show("Bilet iptal edildi!", "Bilgi");
                        // Ekranı yenile
                        LoadSeats((int)cmbHalls.SelectedValue, (int)cmbSessions.SelectedValue);
                    }
                }
                return;
            }

            if (selectedSeatIds.Contains(seatId))
            {
                selectedSeatIds.Remove(seatId);
            }
            else
            {
                selectedSeatIds.Add(seatId);
            }
            btn.Invalidate(); // Rengi güncelle
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            decimal total = selectedSeatIds.Count * selectedFilm.Price;
            lblTotal.Text = $"{total:C}";
            lblSelectedSeats.Text = $"{selectedSeatIds.Count} SEÇİLİ KOLTUK";
        }
        
        private void btnProceedToPayment_Click(object sender, EventArgs e)
        {
            if (selectedSeatIds.Count == 0)
            {
                MessageBox.Show("Lütfen en az bir koltuk seçin!", "Uyarı");
                return;
            }
            int hallId = (int)cmbHalls.SelectedValue;
            decimal totalPrice = selectedSeatIds.Count * selectedFilm.Price;
            // SessionId'yi taşıyalım ama PaymentForm'u da güncellemeliyiz. Şimdilik constructor değişmediği için hata verebilir?
            // Hata vermemesi için PaymentForm'u güncelleyeceğiz veya SessionId'yi global tutabiliriz.
            // PaymentForm'a SessionId eklemeliyiz implementasyon planımda yoktu ama Ticket oluştururken lazım!
            // Ticket oluştururken SessionId lazım.
            int sessionId = (int)cmbSessions.SelectedValue;
            PaymentForm paymentForm = new PaymentForm(selectedFilmId, hallId, selectedSeatIds, totalPrice, sessionId);
            // paymentForm.SessionId = sessionId; // Constructor'a taşıdık
            this.Hide();
            paymentForm.ShowDialog();
            this.Close();
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}