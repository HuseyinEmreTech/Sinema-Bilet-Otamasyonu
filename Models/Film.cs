namespace SinemaBiletOtomasyonu.Models
{
    /// <summary>
    /// Film bilgilerini tutan model sınıfı
    /// </summary>
    public class Film
    {
        public int FilmId { get; set; }
        public string FilmName { get; set; }
        public string Genre { get; set; }
        public int Duration { get; set; } // Dakika cinsinden
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
    }
}
