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
            // DatabaseHelper.SeedData(); // Veritabanı boş gelsin istendiği için devre dışı bırakıldı.

            // Ana formu başlat
            // Splash Screen
            WelcomeForm splash = new WelcomeForm();
            if (splash.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new MainForm());
            }
        }
    }
}