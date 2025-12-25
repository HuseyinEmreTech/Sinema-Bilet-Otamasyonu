using System;
using System.Drawing;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Helpers;
using SinemaBiletOtomasyonu.Models;

namespace SinemaBiletOtomasyonu.Controls
{
    public partial class FilmCard : UserControl
    {
        public Film FilmData { get; private set; }
        public event EventHandler CardClick;

        private bool isHovered = false;

        public FilmCard(Film film)
        {
            this.FilmData = film;
            this.DoubleBuffered = true;
            this.Size = new Size(200, 320); // Kart boyutu
            this.Padding = new Padding(10);
            this.Cursor = Cursors.Hand;
            this.BackColor = Color.Transparent;

            // Hover eventleri
            this.MouseEnter += (s, e) => { isHovered = true; this.Invalidate(); };
            this.MouseLeave += (s, e) => { isHovered = false; this.Invalidate(); };
            this.Click += (s, e) => CardClick?.Invoke(this, e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Hover animasyonu (Hafif büyüme/parlama)
            Rectangle rect = this.ClientRectangle;
            rect.Inflate(-5, -5); // Gölge payı

            // Minimalist Hover: Sadece hafif yukarı çıkma veya parlaklık (seçime gerek yok basit)
            if (isHovered)
            {
                rect.Y -= 2; // Hafif yukarı
            }

            // Glass Arka Plan
            ModernUIHelper.DrawGlassPanel(g, rect, 15);

            // Resim Alanı (Placeholder) veya Film Resmi
            // Resim Alanı
            Rectangle imgRect = new Rectangle(rect.X + 10, rect.Y + 10, rect.Width - 20, 180);
            
            // 1. Resource'dan Resmi Çekmeye Çalış
            Image poster = null;
            if (!string.IsNullOrEmpty(FilmData.ImagePath))
            {
                object obj = Properties.Resources.ResourceManager.GetObject(FilmData.ImagePath);
                if (obj is Image)
                {
                    poster = (Image)obj;
                }
            }

            if (poster != null)
            {
                // Resmi sığdırarak çiz
                g.DrawImage(poster, imgRect);
            }
            else
            {
                    // Placeholder - Flat Dark
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(40, 40, 40)))
                {
                   g.FillRectangle(brush, imgRect);
                }
                TextRenderer.DrawText(g, FilmData.FilmName.ToUpper(), new Font("Segoe UI", 10, FontStyle.Bold), imgRect, Color.DarkGray, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
            }
            
            // Film Bilgileri
            // Başlık
            Rectangle titleRect = new Rectangle(rect.X + 10, imgRect.Bottom + 10, rect.Width - 20, 20);
            // Hover durumunda başlık rengi değişsin
            Color titleColor = isHovered ? ModernUIHelper.PrimaryColor : Color.White;
            
            TextRenderer.DrawText(g, FilmData.FilmName, new Font("Segoe UI", 10, FontStyle.Bold), titleRect, titleColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.EndEllipsis);

            // Tür ve Fiyat
            Rectangle infoRect = new Rectangle(rect.X + 10, titleRect.Bottom + 5, rect.Width - 20, 40);
            string info = $"{FilmData.Genre}";
            TextRenderer.DrawText(g, info, new Font("Segoe UI", 8), infoRect, Color.Gray,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top);
        }
    }
}
