using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using SinemaBiletOtomasyonu.Models;

namespace SinemaBiletOtomasyonu.Database
{
    /// <summary>
    /// Uygulama içi veri depolamasýný yöneten yardýmcý sýnýf
    /// </summary>
    public class DatabaseHelper
    {
        private static readonly object SyncRoot = new object();
        private static readonly string DataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cinema-data.xml");
        private static DataStore store = LoadStore();

        /// <summary>
        /// Veritabanýný ve tablolarý oluþturur (dosya tabanlý)
        /// </summary>
        public static void InitializeDatabase()
        {
            lock (SyncRoot)
            {
                store = store ?? new DataStore();
                EnsureCollections();
                SaveStore();
            }
        }

        /// <summary>
        /// Örnek verileri veritabanýna ekler
        /// </summary>
        public static void SeedData()
        {
            lock (SyncRoot)
            {
                EnsureCollections();

                if (store.Films.Count == 0)
                {
                    store.Films.AddRange(new[]
                    {
                        new Film { FilmId = 1, FilmName = "Avatar: Su Yolu", Genre = "Bilim Kurgu", Duration = 192, Price = 75.00m },
                        new Film { FilmId = 2, FilmName = "Dünyanýn Merkezinde", Genre = "Drama", Duration = 140, Price = 65.00m },
                        new Film { FilmId = 3, FilmName = "Hýzlý ve Öfkeli 10", Genre = "Aksiyon", Duration = 141, Price = 70.00m },
                        new Film { FilmId = 4, FilmName = "Küçük Deniz Kýzý", Genre = "Animasyon", Duration = 135, Price = 60.00m },
                        new Film { FilmId = 5, FilmName = "Oppenheimer", Genre = "Biyografi", Duration = 180, Price = 80.00m },
                        new Film { FilmId = 6, FilmName = "Barbie", Genre = "Komedi", Duration = 114, Price = 65.00m }
                    });
                }

                if (store.Halls.Count == 0)
                {
                    store.Halls.AddRange(new[]
                    {
                        new Hall { HallId = 1, HallName = "Salon 1", TotalSeats = 60, RowCount = 6, ColumnCount = 10 },
                        new Hall { HallId = 2, HallName = "Salon 2", TotalSeats = 80, RowCount = 8, ColumnCount = 10 },
                        new Hall { HallId = 3, HallName = "VIP Salon", TotalSeats = 40, RowCount = 5, ColumnCount = 8 }
                    });
                }

                if (store.Seats.Count == 0 && store.Halls.Count > 0)
                {
                    foreach (var hall in store.Halls)
                    {
                        CreateSeatsForHall(hall.HallId, hall.RowCount, hall.ColumnCount);
                    }
                }

                SaveStore();
            }
        }

        /// <summary>
        /// Belirtilen salon için koltuklarý oluþturur
        /// </summary>
        private static void CreateSeatsForHall(int hallId, int rows, int columns)
        {
            for (int row = 1; row <= rows; row++)
            {
                for (int col = 1; col <= columns; col++)
                {
                    store.Seats.Add(new Seat
                    {
                        SeatId = GetNextSeatId(),
                        HallId = hallId,
                        RowNumber = row,
                        SeatNumber = col,
                        IsAvailable = true
                    });
                }
            }
        }

        /// <summary>
        /// Tüm filmleri getirir
        /// </summary>
        public static List<Film> GetAllFilms()
        {
            lock (SyncRoot)
            {
                return store.Films.Select(f => Clone(f)).ToList();
            }
        }

        /// <summary>
        /// Tüm salonlarý getirir
        /// </summary>
        public static List<Hall> GetAllHalls()
        {
            lock (SyncRoot)
            {
                return store.Halls.Select(h => Clone(h)).ToList();
            }
        }

        /// <summary>
        /// Belirtilen salona ait koltuklarý getirir
        /// </summary>
        public static List<Seat> GetSeatsByHall(int hallId)
        {
            lock (SyncRoot)
            {
                return store.Seats
                    .Where(s => s.HallId == hallId)
                    .OrderBy(s => s.RowNumber)
                    .ThenBy(s => s.SeatNumber)
                    .Select(s => Clone(s))
                    .ToList();
            }
        }

        /// <summary>
        /// Yeni bilet ekler
        /// </summary>
        public static bool InsertTicket(Ticket ticket)
        {
            lock (SyncRoot)
            {
                EnsureCollections();
                var seat = store.Seats.FirstOrDefault(s => s.SeatId == ticket.SeatId);
                if (seat == null || !seat.IsAvailable)
                {
                    return false;
                }

                ticket.TicketId = GetNextTicketId();
                store.Tickets.Add(Clone(ticket));
                seat.IsAvailable = false;
                SaveStore();
                return true;
            }
        }

        /// <summary>
        /// Bilet koduna göre bileti getirir
        /// </summary>
        public static Ticket GetTicketByCode(string ticketCode)
        {
            lock (SyncRoot)
            {
                var ticket = store.Tickets.FirstOrDefault(t => t.TicketCode == ticketCode);
                return ticket != null ? Clone(ticket) : null;
            }
        }

        /// <summary>
        /// Bileti iptal eder
        /// </summary>
        public static bool CancelTicket(string ticketCode)
        {
            lock (SyncRoot)
            {
                var ticket = store.Tickets.FirstOrDefault(t => t.TicketCode == ticketCode);
                if (ticket == null)
                {
                    return false;
                }

                var seat = store.Seats.FirstOrDefault(s => s.SeatId == ticket.SeatId);
                if (seat != null)
                {
                    seat.IsAvailable = true;
                }

                store.Tickets.Remove(ticket);
                SaveStore();
                return true;
            }
        }

        /// <summary>
        /// Film ID'sine göre film getirir
        /// </summary>
        public static Film GetFilmById(int filmId)
        {
            lock (SyncRoot)
            {
                var film = store.Films.FirstOrDefault(f => f.FilmId == filmId);
                return film != null ? Clone(film) : null;
            }
        }

        /// <summary>
        /// Koltuk ID'sine göre koltuk getirir
        /// </summary>
        public static Seat GetSeatById(int seatId)
        {
            lock (SyncRoot)
            {
                var seat = store.Seats.FirstOrDefault(s => s.SeatId == seatId);
                return seat != null ? Clone(seat) : null;
            }
        }

        /// <summary>
        /// Salon ID'sine göre salon getirir
        /// </summary>
        public static Hall GetHallById(int hallId)
        {
            lock (SyncRoot)
            {
                var hall = store.Halls.FirstOrDefault(h => h.HallId == hallId);
                return hall != null ? Clone(hall) : null;
            }
        }

        private static void EnsureCollections()
        {
            store.Films = store.Films ?? new List<Film>();
            store.Halls = store.Halls ?? new List<Hall>();
            store.Seats = store.Seats ?? new List<Seat>();
            store.Tickets = store.Tickets ?? new List<Ticket>();
        }

        private static int GetNextSeatId()
        {
            return store.Seats.Count == 0 ? 1 : store.Seats.Max(s => s.SeatId) + 1;
        }

        private static int GetNextTicketId()
        {
            return store.Tickets.Count == 0 ? 1 : store.Tickets.Max(t => t.TicketId) + 1;
        }

        private static DataStore LoadStore()
        {
            try
            {
                if (File.Exists(DataFilePath))
                {
                    using (var fs = File.OpenRead(DataFilePath))
                    {
                        var serializer = new XmlSerializer(typeof(DataStore));
                        var loaded = serializer.Deserialize(fs) as DataStore;
                        if (loaded != null)
                        {
                            return loaded;
                        }
                    }
                }
            }
            catch
            {
                // Bozuk dosya durumunda yeni depo oluþturulacak
            }
            return new DataStore();
        }

        private static void SaveStore()
        {
            EnsureCollections();
            try
            {
                using (var fs = File.Create(DataFilePath))
                {
                    var serializer = new XmlSerializer(typeof(DataStore));
                    serializer.Serialize(fs, store);
                }
            }
            catch
            {
                // Yazma hatasý durumunda sessiz geç
            }
        }

        private static Film Clone(Film film)
        {
            return new Film
            {
                FilmId = film.FilmId,
                FilmName = film.FilmName,
                Genre = film.Genre,
                Duration = film.Duration,
                Price = film.Price,
                ImagePath = film.ImagePath
            };
        }

        private static Hall Clone(Hall hall)
        {
            return new Hall
            {
                HallId = hall.HallId,
                HallName = hall.HallName,
                TotalSeats = hall.TotalSeats,
                RowCount = hall.RowCount,
                ColumnCount = hall.ColumnCount
            };
        }

        private static Seat Clone(Seat seat)
        {
            return new Seat
            {
                SeatId = seat.SeatId,
                HallId = seat.HallId,
                RowNumber = seat.RowNumber,
                SeatNumber = seat.SeatNumber,
                IsAvailable = seat.IsAvailable
            };
        }

        private static Ticket Clone(Ticket ticket)
        {
            return new Ticket
            {
                TicketId = ticket.TicketId,
                FilmId = ticket.FilmId,
                HallId = ticket.HallId,
                SeatId = ticket.SeatId,
                CustomerName = ticket.CustomerName,
                PurchaseDate = ticket.PurchaseDate,
                TotalPrice = ticket.TotalPrice,
                TicketCode = ticket.TicketCode
            };
        }

        [Serializable]
        public class DataStore
        {
            public List<Film> Films { get; set; } = new List<Film>();
            public List<Hall> Halls { get; set; } = new List<Hall>();
            public List<Seat> Seats { get; set; } = new List<Seat>();
            public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        }
    }
}
