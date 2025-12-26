using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using SinemaBiletOtomasyonu.Helpers;
using SinemaBiletOtomasyonu.Models;

namespace SinemaBiletOtomasyonu.Controls
{
    /// <summary>
    /// Filmlerin görsel olarak listelendiği kullanıcı kontrolü.
    /// Görsel efektler (gölge, animasyon) ve dinamik metin çizimi içerir.
    /// </summary>
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
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Kart Alanı
            Rectangle rect = this.ClientRectangle;
            rect.Inflate(-5, -5);

            // Hover Efekti: Yukarı çıkma ve Gölge/Parlaklık
            if (isHovered)
            {
                rect.Y -= 3; // Yukarı hareket
                
                // Arka plan parlaması (Glow)
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(30, ModernUIHelper.PrimaryColor)))
                {
                   using(GraphicsPath glowInfo = ModernUIHelper.GetRoundedPath(rect, 15))
                   {
                       g.FillPath(shadowBrush, glowInfo);
                   }
                }
                // Kenarlık
                using (Pen p = new Pen(ModernUIHelper.PrimaryColor, 2))
                {
                    using(GraphicsPath border = ModernUIHelper.GetRoundedPath(rect, 15))
                    {
                        g.DrawPath(p, border);
                    }
                }
            }

            // Glass Arka Plan
            ModernUIHelper.DrawGlassPanel(g, rect, 15);

            // Resim Alanı
            Rectangle imgRect = new Rectangle(rect.X + 10, rect.Y + 10, rect.Width - 20, 180);
            
            Image poster = ImageHelper.LoadImage(FilmData.ImagePath);

            // Resmi Yuvarlak Köşeli Çizme (Clip)
            using (GraphicsPath imgPath = ModernUIHelper.GetRoundedPath(imgRect, 10))
            {
                g.SetClip(imgPath);
                if (poster != null)
                {
                    g.DrawImage(poster, imgRect);
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(40, 40, 40)))
                    {
                        g.FillRectangle(brush, imgRect);
                    }
                    TextRenderer.DrawText(g, FilmData.FilmName.ToUpper(), new Font("Segoe UI", 10, FontStyle.Bold), imgRect, Color.DarkGray, 
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
                }
                g.ResetClip();
            }
            
            // Fiyat Etiketi (Sol Üst Köşe - Resmin Üstüne)
            Rectangle priceRect = new Rectangle(imgRect.Right - 60, imgRect.Top + 10, 50, 25);
            using(GraphicsPath pricePath = ModernUIHelper.GetRoundedPath(priceRect, 5))
            {
                using(SolidBrush pBrush = new SolidBrush(ModernUIHelper.PrimaryColor))
                {
                    g.FillPath(pBrush, pricePath);
                }
                TextRenderer.DrawText(g, $"{FilmData.Price:0} TL", new Font("Segoe UI", 9, FontStyle.Bold), priceRect, Color.White, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }


            // Film Başlığı - Multi-line & Auto-size Logic
            Rectangle titleRect = new Rectangle(rect.X + 5, imgRect.Bottom + 10, rect.Width - 10, 45); // Yüksekliği artırdık
            Color titleColor = isHovered ? ModernUIHelper.PrimaryColor : Color.White;
            
            // Font küçültme mantığı (Basit)
            Font titleFont = new Font("Segoe UI", 11, FontStyle.Bold);
            if(FilmData.FilmName.Length > 25) titleFont = new Font("Segoe UI", 9, FontStyle.Bold);
            
            TextRenderer.DrawText(g, FilmData.FilmName, titleFont, titleRect, titleColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak); // WordBreak ekledik

            // Tür
            Rectangle genreRect = new Rectangle(rect.X + 5, titleRect.Bottom + 2, rect.Width - 10, 20);
            TextRenderer.DrawText(g, FilmData.Genre, new Font("Segoe UI", 8, FontStyle.Regular), genreRect, Color.Gray,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top);
                
            // Süre butonu/ikonu benzeri bilgi
            Rectangle durRect = new Rectangle(rect.X + 5, genreRect.Bottom + 0, rect.Width - 10, 20);
            TextRenderer.DrawText(g, $"{FilmData.Duration} dk", new Font("Segoe UI", 8, FontStyle.Regular), durRect, Color.DarkGray,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top);

        }
    }
}
