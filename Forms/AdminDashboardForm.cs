using System;
using System.Drawing;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Database;
using SinemaBiletOtomasyonu.Models;
using SinemaBiletOtomasyonu.Helpers;
using System.Linq;

namespace SinemaBiletOtomasyonu.Forms
{
    public class AdminDashboardForm : Form
    {
        private TabControl tabControl;
        private TabPage tabFilms;
        private TabPage tabReports;
        
        private DataGridView dgvFilms;
        private Button btnAddFilm;
        private Button btnDeleteFilm;
        private Label lblTotalRevenue;

        public AdminDashboardForm()
        {
            this.Text = "Yönetici Paneli";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            ModernUIHelper.ApplyTheme(this);
            
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            
            tabFilms = new TabPage("Film Yönetimi");
            tabFilms.BackColor = ModernUIHelper.PanelBackground;
            
            tabReports = new TabPage("Raporlar");
            tabReports.BackColor = ModernUIHelper.PanelBackground;
            
            tabControl.Controls.Add(tabFilms);
            tabControl.Controls.Add(tabReports);
            this.Controls.Add(tabControl);
            
            InitializeFilmsTab();
            InitializeReportsTab();
        }

        private void InitializeFilmsTab()
        {
            // Grid
            dgvFilms = new DataGridView();
            dgvFilms.Location = new Point(20, 20);
            dgvFilms.Size = new Size(720, 450);
            dgvFilms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFilms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFilms.BackgroundColor = Color.FromArgb(40,40,40);
            dgvFilms.ForeColor = Color.Black; 
            tabFilms.Controls.Add(dgvFilms);
            
            RefreshFilmList();

            // Buttons
            btnAddFilm = new Button { Text = "Yeni Film Ekle", Location = new Point(20, 490), Size = new Size(150, 40), BackColor = ModernUIHelper.PrimaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAddFilm.Click += BtnAddFilm_Click;
            tabFilms.Controls.Add(btnAddFilm);
            
            btnDeleteFilm = new Button { Text = "Seçili Filmi Sil", Location = new Point(190, 490), Size = new Size(150, 40), BackColor = Color.DarkRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDeleteFilm.Click += BtnDeleteFilm_Click;
            tabFilms.Controls.Add(btnDeleteFilm);
        }

        private void InitializeReportsTab()
        {
            decimal totalRevenue = DatabaseHelper.DataStoreInstance.Tickets.Sum(t => t.TotalPrice);
            int totalTickets = DatabaseHelper.DataStoreInstance.Tickets.Count;
            var mostPopular = DatabaseHelper.DataStoreInstance.Tickets
                .GroupBy(t => t.FilmId)
                .OrderByDescending(g => g.Count())
                .Select(g => new { FilmId = g.Key, Count = g.Count() })
                .FirstOrDefault();
                
            string popularFilmName = "-";
            if(mostPopular != null)
            {
                var f = DatabaseHelper.GetFilmById(mostPopular.FilmId);
                if(f != null) popularFilmName = f.FilmName;
            }

            Label lblReport = new Label();
            lblReport.AutoSize = true;
            lblReport.Location = new Point(50, 50);
            lblReport.Font = new Font("Segoe UI", 14);
            lblReport.ForeColor = Color.White;
            lblReport.Text = $"Toplam Hasılat: {totalRevenue:C}\n" +
                             $"Satılan Bilet: {totalTickets}\n" +
                             $"En Çok İzlenen: {popularFilmName}";
            
            tabReports.Controls.Add(lblReport);
            
            Button btnRefresh = new Button { Text = "Yenile", Location = new Point(50, 200), Size = new Size(100, 40) };
            btnRefresh.Click += (s,e) => {
                 tabReports.Controls.Clear();
                 InitializeReportsTab();
            };
            tabReports.Controls.Add(btnRefresh);
        }

        private void RefreshFilmList()
        {
            dgvFilms.DataSource = null;
            dgvFilms.DataSource = DatabaseHelper.GetAllFilms();
        }

        private void BtnAddFilm_Click(object sender, EventArgs e)
        {
            // Basit Input Dialogları ile Film Ekle
            string name = Microsoft.VisualBasic.Interaction.InputBox("Film Adı:", "Yeni Film", "Yeni Film");
            if(string.IsNullOrWhiteSpace(name)) return;
            
            string genre = Microsoft.VisualBasic.Interaction.InputBox("Tür:", "Yeni Film", "Aksiyon");
            string durStr = Microsoft.VisualBasic.Interaction.InputBox("Süre (dk):", "Yeni Film", "120");
            string priceStr = Microsoft.VisualBasic.Interaction.InputBox("Fiyat:", "Yeni Film", "80");
            string img = Microsoft.VisualBasic.Interaction.InputBox("Resim Adı (Resource):", "Yeni Film", "placeholder"); // Basitçe

            if(int.TryParse(durStr, out int duration) && decimal.TryParse(priceStr, out decimal price))
            {
                Film f = new Film { FilmName = name, Genre = genre, Duration = duration, Price = price, ImagePath = img };
                DatabaseHelper.AddFilm(f);
                RefreshFilmList();
                MessageBox.Show("Film Eklendi!");
            }
            else
            {
                MessageBox.Show("Hatalı giriş!");
            }
        }

        private void BtnDeleteFilm_Click(object sender, EventArgs e)
        {
            if (dgvFilms.SelectedRows.Count > 0)
            {
                int filmId = (int)dgvFilms.SelectedRows[0].Cells["FilmId"].Value;
                if(MessageBox.Show("Silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DatabaseHelper.DeleteFilm(filmId);
                    RefreshFilmList();
                }
            }
        }
    }
}
