namespace SinemaBiletOtomasyonu.Models
{
    /// <summary>
    /// Koltuk bilgilerini tutan model sınıfı
    /// </summary>
    public class Seat
    {
        public int SeatId { get; set; }
        public int HallId { get; set; }
        public int RowNumber { get; set; }
        public int SeatNumber { get; set; }
        public bool IsAvailable { get; set; }
    }
}
