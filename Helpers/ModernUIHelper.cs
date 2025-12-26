using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SinemaBiletOtomasyonu.Helpers
{
    public static class ModernUIHelper
    {
        // Renk Paleti
        // Renk Paleti - Minimalist Dark
        public static Color DarkBackground = Color.FromArgb(20, 20, 20); // Çok koyu gri / Siyah
        public static Color PanelBackground = Color.FromArgb(30, 30, 30); // Paneller için
        public static Color PrimaryColor = Color.FromArgb(229, 9, 20);   // Netflix Red
        public static Color SecondaryColor = Color.FromArgb(200, 200, 200); // Açık gri metin
        public static Color TextColor = Color.White;
        
        public static void ApplyTheme(Form form)
        {
            form.BackColor = DarkBackground;
            // BackgroundImage varsa Paint ile temizleme yapılmasın
            if (form.BackgroundImage == null)
            {
                form.Paint += (s, e) =>
                {
                   e.Graphics.Clear(DarkBackground);
                };
            }
        }

        public static GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float r = radius;
            float d = r * 2;

            if (r < 1) r = 1;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90); // Sol Üst
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90); // Sağ Üst
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90); // Sağ Alt
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90); // Sol Alt
            path.CloseFigure();
            return path;
        }

        public static void DrawGlassPanel(Graphics g, Rectangle rect, int radius)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = GetRoundedPath(rect, radius))
            {
                // Düz, opakımsı dolgu (Flat Design)
                using (SolidBrush brush = new SolidBrush(PanelBackground))
                {
                    g.FillPath(brush, path);
                }
            }
        }
        
        public static Image MakeImageRound(Image img, int size)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, size, size);
                    g.SetClip(path);
                    g.DrawImage(img, 0, 0, size, size);
                }
            }
            return bmp;
        }
    }
}
