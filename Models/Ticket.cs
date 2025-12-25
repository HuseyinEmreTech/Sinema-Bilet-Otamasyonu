using System;

namespace SinemaBiletOtomasyonu.Models
{
    /// <summary>
    /// Bilet bilgilerini tutan model sınıfı
    /// </summary>
    public class Ticket
    {
        public int TicketId { get; set; }
        public int FilmId { get; set; }
        public int HallId { get; set; }
        public int SessionId { get; set; }
        public int SeatId { get; set; }
        public string CustomerName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string TicketCode { get; set; }
    }
}
