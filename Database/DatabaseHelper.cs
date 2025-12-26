using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using SinemaBiletOtomasyonu.Models;

namespace SinemaBiletOtomasyonu.Database
{
    // Verileri tutacak ana sınıf (XML Root)
    [XmlRoot("DataStore")]
    public class DataStore
    {
        public List<Film> Films { get; set; } = new List<Film>();
        public List<Hall> Halls { get; set; } = new List<Hall>();
        public List<Seat> Seats { get; set; } = new List<Seat>();
        public List<Session> Sessions { get; set; } = new List<Session>();
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }

    /// <summary>
    /// Veritabanı bağlantısı ve veri işlemlerini (CRUD) yöneten merkezi sınıf.
    /// </summary>
    public static class DatabaseHelper
    {
        private static DataStore store = new DataStore();
        public static DataStore DataStoreInstance => store; // Raporlama için açık erişim
        private static string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cinema-data.xml");

        static DatabaseHelper()
        {
            LoadData();
        }

        public static void LoadData()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DataStore));
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {
                        store = (DataStore)serializer.Deserialize(fs);
                    }
                }
                catch (Exception ex)
                {
                    // Hata durumunda boş store ile devam et ama hatayı logla varsa
                    System.Diagnostics.Debug.WriteLine("Veri yükleme hatası: " + ex.Message);
                }
            }
        }

        public static void InitializeDatabase()
        {
            LoadData();
        }

        public static void SeedData()
        {
            if (store.Films.Count == 0)
            {
                // 10+ Popüler Film Verisi
                store.Films.Add(new Film { FilmId = 1, FilmName = "Avatar: Su Yolu", Genre = "Bilim Kurgu", Duration = 192, Price = 85.00m, ImagePath = "avatar2" });
                store.Films.Add(new Film { FilmId = 2, FilmName = "Hızlı ve Öfkeli 10", Genre = "Aksiyon", Duration = 140, Price = 75.00m, ImagePath = "fastx" });
                store.Films.Add(new Film { FilmId = 3, FilmName = "Barbie", Genre = "Komedi", Duration = 114, Price = 80.00m, ImagePath = "barbie" });
                store.Films.Add(new Film { FilmId = 4, FilmName = "Oppenheimer", Genre = "Biyografi", Duration = 180, Price = 85.00m, ImagePath = "oppenheimer" });
                store.Films.Add(new Film { FilmId = 5, FilmName = "Örümcek-Adam: Örümcek Evrenine Geçiş", Genre = "Animasyon", Duration = 140, Price = 70.00m, ImagePath = "spiderman" });
                store.Films.Add(new Film { FilmId = 6, FilmName = "John Wick 4", Genre = "Aksiyon", Duration = 169, Price = 80.00m, ImagePath = "johnwick4" });
                store.Films.Add(new Film { FilmId = 7, FilmName = "Görevimiz Tehlike 7", Genre = "Aksiyon", Duration = 163, Price = 80.00m, ImagePath = "mi7" });
                store.Films.Add(new Film { FilmId = 8, FilmName = "Transformers: Canavarların Yükselişi", Genre = "Bilim Kurgu", Duration = 127, Price = 75.00m, ImagePath = "transformers" });
                store.Films.Add(new Film { FilmId = 9, FilmName = "Galaksinin Koruyucuları 3", Genre = "Bilim Kurgu", Duration = 150, Price = 75.00m, ImagePath = "gotg3" });
                store.Films.Add(new Film { FilmId = 10, FilmName = "The Flash", Genre = "Aksiyon", Duration = 144, Price = 70.00m, ImagePath = "flash" });
                store.Films.Add(new Film { FilmId = 11, FilmName = "Dune: Çöl Gezegeni Bölüm İki", Genre = "Bilim Kurgu", Duration = 166, Price = 90.00m, ImagePath = "dune2" });

                // Varsayılan Salonlar
                if (!store.Halls.Any())
                {
                    store.Halls.Add(new Hall { HallId = 1, HallName = "Salon 1 (IMAX)", RowCount = 5, ColumnCount = 8, PriceMultiplier = 1.2m });
                    store.Halls.Add(new Hall { HallId = 2, HallName = "Salon 2 (Gold Class)", RowCount = 4, ColumnCount = 6, PriceMultiplier = 1.5m });
                    store.Halls.Add(new Hall { HallId = 3, HallName = "Salon 3", RowCount = 6, ColumnCount = 10, PriceMultiplier = 1.0m });
                }

                // Koltukları oluştur (3 Salon için)
                CreateSeatsForHall(1);
                CreateSeatsForHall(2);
                CreateSeatsForHall(3);

                SaveData();
            }

            // Seansları Seed et
            if (store.Sessions.Count == 0 && store.Films.Count > 0 && store.Halls.Count > 0) 
            {
                int sId = 1;
                foreach(var film in store.Films)
                {
                    // Her film için rastgele salon ve seans ata
                    foreach(var hall in store.Halls)
                    {
                         // Basitleştirme için her film her salonda oynasın
                        store.Sessions.Add(new Session { SessionId = sId++, FilmId = film.FilmId, HallId = hall.HallId, Time = "11:00" });
                        store.Sessions.Add(new Session { SessionId = sId++, FilmId = film.FilmId, HallId = hall.HallId, Time = "15:00" });
                        store.Sessions.Add(new Session { SessionId = sId++, FilmId = film.FilmId, HallId = hall.HallId, Time = "19:00" });
                        store.Sessions.Add(new Session { SessionId = sId++, FilmId = film.FilmId, HallId = hall.HallId, Time = "22:00" });
                    }
                }
                SaveData();
            }
        }
        
        private static void CreateSeatsForHall(int hallId)
        {
            Hall hall = store.Halls.FirstOrDefault(h => h.HallId == hallId);
            if(hall == null) return;
            
            int seatIdCounter = store.Seats.Any() ? store.Seats.Max(s => s.SeatId) + 1 : 1;
            for(int r=1; r<=hall.RowCount; r++)
            {
                for(int c=1; c<=hall.ColumnCount; c++)
                {
                    if(!store.Seats.Any(s => s.HallId == hallId && s.RowNumber == r && s.SeatNumber == c))
                    {
                         store.Seats.Add(new Seat { SeatId = seatIdCounter++, HallId = hallId, RowNumber = r, SeatNumber = c, IsAvailable = true });
                    }
                }
             }
        }

        public static void SaveData()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DataStore));
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    serializer.Serialize(fs, store);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Veri kaydetme hatası: " + ex.Message);
            }
        }

        // --- Film İşlemleri ---
        public static List<Film> GetAllFilms()
        {
            return store.Films.Where(f => !f.IsDeleted).ToList();
        }

        public static Film GetFilmById(int filmId)
        {
            return store.Films.FirstOrDefault(f => f.FilmId == filmId);
        }

        public static void AddFilm(Film film)
        {
             int newId = store.Films.Any() ? store.Films.Max(f => f.FilmId) + 1 : 1;
             film.FilmId = newId;
             store.Films.Add(film);
             
              // Auto-Session Creation REMOVED for Manual Management
              // Users will now add sessions manually via Admin Dashboard

             SaveData();
        }

        public static bool DeleteFilm(int filmId)
        {
            var film = GetFilmById(filmId);
            if (film != null)
            {
                // Remove related sessions/tickets?
                // Soft delete
                film.IsDeleted = true;
                
                // Keep sessions! 
                // store.Sessions.RemoveAll(s => s.FilmId == filmId);
                
                SaveData();
                return true;
            }
            return false;
        }

        // --- Salon İşlemleri ---
        public static List<Hall> GetAllHalls()
        {
            return store.Halls;
        }

        public static Hall GetHallById(int hallId)
        {
            return store.Halls.FirstOrDefault(h => h.HallId == hallId);
        }

        // --- Koltuk İşlemleri ---
        public static List<Seat> GetSeatsByHall(int hallId)
        {
            return store.Seats.Where(s => s.HallId == hallId).ToList();
        }

        public static Seat GetSeatById(int seatId)
        {
            return store.Seats.FirstOrDefault(s => s.SeatId == seatId);
        }

        // --- Bilet İşlemleri ---
        public static Ticket GetTicketByCode(string code)
        {
            return store.Tickets.FirstOrDefault(t => t.TicketCode == code);
        }

        public static List<int> GetOccupiedSeatIds(int sessionId)
        {
            return store.Tickets.Where(t => t.SessionId == sessionId).Select(t => t.SeatId).ToList();
        }

        public static Session GetSessionById(int id) => store.Sessions.FirstOrDefault(s => s.SessionId == id); 
        
        public static List<Session> GetSessionsByFilmAndHall(int filmId, int hallId) 
        {
            return store.Sessions.Where(s => s.FilmId == filmId && s.HallId == hallId).ToList();
        }

        public static Ticket GetTicketBySessionAndSeat(int sessionId, int seatId) 
        {
             return store.Tickets.FirstOrDefault(t => t.SessionId == sessionId && t.SeatId == seatId);
        }

        public static bool InsertTicket(Ticket ticket)
        {
            try
            {
                int newId = store.Tickets.Any() ? store.Tickets.Max(t => t.TicketId) + 1 : 1;
                ticket.TicketId = newId;

                // Çifte rezervasyon kontrolü
                if (store.Tickets.Any(t => t.SessionId == ticket.SessionId && t.SeatId == ticket.SeatId))
                {
                    return false; // Koltuk zaten dolu
                }

                store.Tickets.Add(ticket);
                SaveData();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CancelTicket(string ticketCode)
        {
             var ticket = store.Tickets.FirstOrDefault(t => t.TicketCode == ticketCode);
             if (ticket == null) return false;

             store.Tickets.Remove(ticket);
             SaveData();
             return true;
        }

        // --- Seans İşlemleri (Manuel) ---
        public static List<Session> GetAllSessions()
        {
            return store.Sessions;
        }

        public static bool AddSession(Session session)
        {
            // Basit çakışma kontrolü: Aynı Salon, Aynı Saat
            if(store.Sessions.Any(s => s.HallId == session.HallId && s.Time == session.Time))
            {
                // Çakışma var!
                // Daha ileri seviye kontrol: Film süresi vs. eklenebilir ama şimdilik exact match
                return false; 
            }

            int newId = store.Sessions.Any() ? store.Sessions.Max(s => s.SessionId) + 1 : 1;
            session.SessionId = newId;
            store.Sessions.Add(session);
            SaveData();
            return true;
        }

        public static void DeleteSession(int sessionId)
        {
             var session = store.Sessions.FirstOrDefault(s => s.SessionId == sessionId);
             if(session != null)
             {
                 store.Sessions.Remove(session);
                 // Bileti alınan seans silinirse biletleri ne yapacağız?
                 // Şimdilik biletleri de silelim ki tutarsızlık olmasın (Cascading Delete)
                 store.Tickets.RemoveAll(t => t.SessionId == sessionId);
                 SaveData();
             }
        }
    }
}
