using System;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Database;
using SinemaBiletOtomasyonu.Forms;
namespace SinemaBiletOtomasyonu
{
    static class Program
    {
        /// <summary>
        /// Uygulamanın ana giriş noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Veritabanını başlat
            DatabaseHelper.InitializeDatabase();
            DatabaseHelper.SeedData();

            // Ana formu başlat
            Application.Run(new MainForm());
        }
    }
}