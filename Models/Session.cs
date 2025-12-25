using System;

namespace SinemaBiletOtomasyonu.Models
{
    public class Session
    {
        public int SessionId { get; set; }
        public int FilmId { get; set; }
        public int HallId { get; set; }
        public string Time { get; set; } // "10:00", "14:00" etc.
    }
}
