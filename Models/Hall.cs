namespace SinemaBiletOtomasyonu.Models
{
    /// <summary>
    /// Salon bilgilerini tutan model sınıfı
    /// </summary>
    public class Hall
    {
        public int HallId { get; set; }
        public string HallName { get; set; }
        public int TotalSeats { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
    }
}
