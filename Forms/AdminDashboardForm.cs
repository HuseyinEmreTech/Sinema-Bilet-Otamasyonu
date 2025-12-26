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
        private TabPage tabSessions;
        private TabPage tabHalls;
        private TabPage tabReports;
        
        private DataGridView dgvFilms;
        private Button btnAddFilm;
        private Button btnDeleteFilm;

        // Hall Tab Controls
        private DataGridView dgvHalls;
        private Button btnAddHall;
        private Button btnDeleteHall;
        
        // Session Tab Controls
        private DataGridView dgvSessions;
        private ComboBox cmbFilms;
        private ComboBox cmbHalls;
        private MaskedTextBox txtTime;
        private Button btnAddSession;
        private Button btnDeleteSession;

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
            
            
            tabSessions = new TabPage("Seans Yönetimi");
            tabSessions.BackColor = ModernUIHelper.PanelBackground;
            
            tabHalls = new TabPage("Salon Yönetimi");
            tabHalls.BackColor = ModernUIHelper.PanelBackground;
            
            tabReports = new TabPage("Raporlar");
            tabReports.BackColor = ModernUIHelper.PanelBackground;
            
            tabControl.Controls.Add(tabFilms);
            tabControl.Controls.Add(tabHalls);
            tabControl.Controls.Add(tabSessions);
            tabControl.Controls.Add(tabReports);
            this.Controls.Add(tabControl);
            
            InitializeFilmsTab();
            InitializeHallsTab();
            InitializeSessionsTab();
            InitializeReportsTab();
        }

        private void InitializeFilmsTab()
        {
            // Grid Styling
            dgvFilms = new DataGridView();
            dgvFilms.Location = new Point(20, 20);
            dgvFilms.Size = new Size(740, 450);
            dgvFilms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFilms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFilms.BackgroundColor = ModernUIHelper.PanelBackground;
            dgvFilms.BorderStyle = BorderStyle.None;
            dgvFilms.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvFilms.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvFilms.RowHeadersVisible = false;
            
            // Header Style
            dgvFilms.EnableHeadersVisualStyles = false;
            dgvFilms.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvFilms.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvFilms.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvFilms.ColumnHeadersHeight = 40;

            // Row Style
            dgvFilms.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dgvFilms.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
            dgvFilms.DefaultCellStyle.SelectionBackColor = ModernUIHelper.PrimaryColor;
            dgvFilms.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvFilms.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvFilms.RowTemplate.Height = 35;
            dgvFilms.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(35, 35, 35);

            tabFilms.Controls.Add(dgvFilms);
            
            RefreshFilmList();

            // Buttons
            btnAddFilm = new Button { Text = " + YENİ FİLM", Location = new Point(20, 490), Size = new Size(180, 45), BackColor = ModernUIHelper.PrimaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnAddFilm.FlatAppearance.BorderSize = 0;
            btnAddFilm.Click += BtnAddFilm_Click;
            tabFilms.Controls.Add(btnAddFilm);
            
            btnDeleteFilm = new Button { Text = "SİL", Location = new Point(220, 490), Size = new Size(120, 45), BackColor = Color.FromArgb(200, 50, 50), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnDeleteFilm.FlatAppearance.BorderSize = 0;
            btnDeleteFilm.Click += BtnDeleteFilm_Click;
            tabFilms.Controls.Add(btnDeleteFilm);
        }

        private void InitializeHallsTab()
        {
            dgvHalls = new DataGridView();
            dgvHalls.Location = new Point(20, 20);
            dgvHalls.Size = new Size(740, 400);
            dgvHalls.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHalls.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHalls.BackgroundColor = ModernUIHelper.PanelBackground;
            dgvHalls.BorderStyle = BorderStyle.None;
            dgvHalls.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvHalls.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvHalls.RowHeadersVisible = false;
            dgvHalls.EnableHeadersVisualStyles = false;
            dgvHalls.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvHalls.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHalls.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dgvHalls.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
            dgvHalls.DefaultCellStyle.SelectionBackColor = ModernUIHelper.PrimaryColor;
            
            tabHalls.Controls.Add(dgvHalls);
            
            RefreshHallList();

            btnAddHall = new Button { Text = " + YENİ SALON", Location = new Point(20, 440), Size = new Size(180, 45), BackColor = ModernUIHelper.PrimaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnAddHall.FlatAppearance.BorderSize = 0;
            btnAddHall.Click += BtnAddHall_Click;
            tabHalls.Controls.Add(btnAddHall);
            
            btnDeleteHall = new Button { Text = "SİL", Location = new Point(220, 440), Size = new Size(120, 45), BackColor = Color.FromArgb(200, 50, 50), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnDeleteHall.FlatAppearance.BorderSize = 0;
            btnDeleteHall.Click += BtnDeleteHall_Click;
            tabHalls.Controls.Add(btnDeleteHall);
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
            using (AddEditFilmForm form = new AddEditFilmForm())
            {
                if(form.ShowDialog() == DialogResult.OK)
                {
                    DatabaseHelper.AddFilm(form.RESULT_FILM);
                    RefreshFilmList();
                    MessageBox.Show("Film Başarıyla Eklendi!");
                }
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


        private void InitializeSessionsTab()
        {
            // Grid
            dgvSessions = new DataGridView();
            dgvSessions.Location = new Point(20, 20);
            dgvSessions.Size = new Size(720, 350);
            dgvSessions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSessions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSessions.BackgroundColor = Color.FromArgb(40,40,40);
            dgvSessions.ForeColor = Color.Black; 
            tabSessions.Controls.Add(dgvSessions);
            
            // Input Panel
            Panel pnlInput = new Panel { Location = new Point(20, 390), Size = new Size(720, 100), BackColor = Color.FromArgb(50,50,50) };
            tabSessions.Controls.Add(pnlInput);

            Label lblF = new Label { Text = "Film:", Location = new Point(10, 10), ForeColor = Color.White, AutoSize = true };
            cmbFilms = new ComboBox { Location = new Point(10, 35), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            
            Label lblH = new Label { Text = "Salon:", Location = new Point(220, 10), ForeColor = Color.White, AutoSize = true };
            cmbHalls = new ComboBox { Location = new Point(220, 35), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            
            Label lblT = new Label { Text = "Saat (HH:mm):", Location = new Point(380, 10), ForeColor = Color.White, AutoSize = true };
            txtTime = new MaskedTextBox { Mask = "00:00", Location = new Point(380, 35), Width = 80 };
            
            pnlInput.Controls.AddRange(new Control[] { lblF, cmbFilms, lblH, cmbHalls, lblT, txtTime });

            // Buttons
            btnAddSession = new Button { Text = "Seans Ekle", Location = new Point(500, 30), Size = new Size(100, 30), BackColor = ModernUIHelper.PrimaryColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAddSession.Click += BtnAddSession_Click;
            pnlInput.Controls.Add(btnAddSession);
            
            btnDeleteSession = new Button { Text = "Seçiliyi Sil", Location = new Point(610, 30), Size = new Size(100, 30), BackColor = Color.DarkRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDeleteSession.Click += BtnDeleteSession_Click;
            pnlInput.Controls.Add(btnDeleteSession);
            
            RefreshSessionList();
        }

        private void RefreshSessionList()
        {
            // Grid Update
            dgvSessions.DataSource = null;
            // Join data for better display
            var sessions = DatabaseHelper.GetAllSessions();
            var films = DatabaseHelper.GetAllFilms(); // Gets only active films
            var halls = DatabaseHelper.GetAllHalls();
            
            var displayList = sessions.Select(s => new 
            {
                s.SessionId,
                FilmName = films.FirstOrDefault(f => f.FilmId == s.FilmId)?.FilmName ?? "Silinmiş Film",
                HallName = halls.FirstOrDefault(h => h.HallId == s.HallId)?.HallName,
                s.Time
            }).OrderBy(x => x.FilmName).ThenBy(x => x.Time).ToList();
            
            dgvSessions.DataSource = displayList;

            // Combobox Update
            cmbFilms.DataSource = null;
            cmbFilms.DataSource = films;
            cmbFilms.DisplayMember = "FilmName";
            cmbFilms.ValueMember = "FilmId";

            cmbHalls.DataSource = null;
            cmbHalls.DataSource = halls;
            cmbHalls.DisplayMember = "HallName";
            cmbHalls.ValueMember = "HallId";
        }

        private void BtnAddSession_Click(object sender, EventArgs e)
        {
            if(cmbFilms.SelectedItem == null || cmbHalls.SelectedItem == null || !txtTime.MaskCompleted)
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }

            int filmId = (int)cmbFilms.SelectedValue;
            int hallId = (int)cmbHalls.SelectedValue;
            string time = txtTime.Text;

            Session s = new Session { FilmId = filmId, HallId = hallId, Time = time };
            
            if(DatabaseHelper.AddSession(s))
            {
                MessageBox.Show("Seans eklendi.");
                RefreshSessionList();
            }
            else
            {
                MessageBox.Show("Seans eklenemedi! (Çakışma olabilir)");
            }
        }

        private void BtnDeleteSession_Click(object sender, EventArgs e)
        {
             if (dgvSessions.SelectedRows.Count > 0)
            {
                int sId = (int)dgvSessions.SelectedRows[0].Cells["SessionId"].Value;
                if(MessageBox.Show("Seansı silmek istiyor musunuz? İlgili biletler de silinecektir.", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DatabaseHelper.DeleteSession(sId);
                    RefreshSessionList();
                }
            }
        }

        private void RefreshHallList()
        {
            dgvHalls.DataSource = null;
            dgvHalls.DataSource = DatabaseHelper.GetAllHalls();
            // Also refresh sessions tab comboboxes because halls might have changed
            if (cmbHalls != null && cmbHalls.IsHandleCreated) RefreshSessionList();
        }

        private void BtnAddHall_Click(object sender, EventArgs e)
        {
            using (AddEditHallForm form = new AddEditHallForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    DatabaseHelper.AddHall(form.RESULT_HALL);
                    RefreshHallList();
                    MessageBox.Show("Salon Başarıyla Eklendi! Koltuklar otomatik oluşturuldu.");
                }
            }
        }

        private void BtnDeleteHall_Click(object sender, EventArgs e)
        {
            if (dgvHalls.SelectedRows.Count > 0)
            {
                int hallId = (int)dgvHalls.SelectedRows[0].Cells["HallId"].Value;
                if (MessageBox.Show("Bu salonu silmek istediğinize emin misiniz? Bu işlem bağlı seansları ve biletleri de SİLECEKTİR!", "Kritik Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DatabaseHelper.DeleteHall(hallId);
                    RefreshHallList();
                }
            }
        }
    }
}
