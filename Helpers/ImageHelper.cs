using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SinemaBiletOtomasyonu.Helpers
{
    /// <summary>
    /// Resim dosyalarının yönetimi, kaydedilmesi ve yüklenmesi için yardımcı sınıf.
    /// Dosya kilitleme sorunlarını önleyen güvenli yöntemler içerir.
    /// </summary>
    public static class ImageHelper
    {
        // Directory to store uploaded images relative to the executable
        private static string ImageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

        public static void EnsureImageDirectory()
        {
            if (!Directory.Exists(ImageDirectory))
            {
                Directory.CreateDirectory(ImageDirectory);
            }
        }

        public static string SaveImage(string sourceFilePath)
        {
            EnsureImageDirectory();
            string extension = Path.GetExtension(sourceFilePath);
            string uniqueName = Guid.NewGuid().ToString() + extension;
            string destPath = Path.Combine(ImageDirectory, uniqueName);
            
            File.Copy(sourceFilePath, destPath);
            return uniqueName; // Save this to database
        }

        public static string SaveImage(Image image)
        {
            EnsureImageDirectory();
            string uniqueName = Guid.NewGuid().ToString() + ".png"; // Default to PNG for pasted images
            string destPath = Path.Combine(ImageDirectory, uniqueName);
            
            image.Save(destPath, System.Drawing.Imaging.ImageFormat.Png);
            return uniqueName;
        }



        public static string DownloadImageFromUrl(string url)
        {
            try
            {
                using (var client = new System.Net.WebClient())
                {
                    byte[] data = client.DownloadData(url);
                    using (var ms = new MemoryStream(data))
                    {
                        using (var image = Image.FromStream(ms))
                        {
                            return SaveImage(image);
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Resim indirilemedi. URL'yi kontrol edin.");
            }
        }

        public static Image LoadImage(string imageName)
        {
            if (string.IsNullOrEmpty(imageName)) return null;

            // 1. Check File System
            string localPath = Path.Combine(ImageDirectory, imageName);
            if (File.Exists(localPath))
            {
                try
                {
                    // Read fully to memory to avoid locking file and GDI+ errors
                    byte[] bytes = File.ReadAllBytes(localPath);
                    MemoryStream ms = new MemoryStream(bytes);
                    return Image.FromStream(ms);
                }
                catch
                {
                    // Fallback on error
                }
            }

            // 2. Check Resources (Fallback for old data)
            try
            {
                object obj = Properties.Resources.ResourceManager.GetObject(imageName);
                if (obj is Image img)
                {
                    return img;
                }
            }
            catch
            {
                // Resource not found
            }

            return null; // Both failed
        }
    }
}
