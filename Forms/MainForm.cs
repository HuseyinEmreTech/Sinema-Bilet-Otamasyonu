using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Database;
using SinemaBiletOtomasyonu.Models;
using SinemaBiletOtomasyonu.Controls;
using SinemaBiletOtomasyonu.Helpers;
using System.Collections.Generic;

namespace SinemaBiletOtomasyonu.Forms
{
    public partial class MainForm : Form
    {
        private List<Film> films;
        private FlowLayoutPanel flowPanel;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            this.DoubleBuffered = true; 
            ModernUIHelper.ApplyTheme(this); 
            InitializeCustomComponents(); 
            LoadFilms();
        }

        private void InitializeCustomComponents()
        {
            // FlowLayoutPanel ayarları
            flowPanel = new FlowLayoutPanel();
            flowPanel.Dock = DockStyle.Fill;
            flowPanel.AutoScroll = true;
            // Top padding increased to avoid overlap with Label
            flowPanel.Padding = new Padding(20, 80, 20, 20);
            flowPanel.BackColor = Color.Transparent;
            
            // flowPanel.Margin useless with Dock.Fill 
            
            // Mevcut dgvFilms'i kaldır (Designer'da var ama kullanmıyoruz)
            this.Controls.Remove(dgvFilms);
            
            // Paneli ekle
            this.Controls.Add(flowPanel);
            
            // Butonları ve başlığı en öne getir
            if(Controls.Contains(lblTitle)) this.Controls.SetChildIndex(lblTitle, 0);

            // Admin Button
            Button btnAdmin = new Button();
            btnAdmin.Text = "Yönetici";
            btnAdmin.Size = new Size(80, 30);
            btnAdmin.Location = new Point(this.ClientSize.Width - 100, 20);
            btnAdmin.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdmin.FlatStyle = FlatStyle.Flat;
            btnAdmin.FlatAppearance.BorderSize = 0;
            btnAdmin.BackColor = Color.Transparent; // Şeffaf
            btnAdmin.ForeColor = Color.Gray;
            btnAdmin.Cursor = Cursors.Hand;
            btnAdmin.Click += (s, e) => {
                AdminLoginForm login = new AdminLoginForm();
                login.ShowDialog();
                // Refresh films after admin closes (if added new film)
                LoadFilms();
            };
            this.Controls.Add(btnAdmin);
            this.Controls.SetChildIndex(btnAdmin, 0);
            
            // Buton stillerini güncelle
        }

        private void StyleButton(Button btn, Color color)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(40, color);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btn.Paint += (s, e) =>
            {
                // Özel yuvarlak çizim
                Rectangle rect = btn.ClientRectangle;
                rect.Inflate(-2, -2);
                ModernUIHelper.DrawGlassPanel(e.Graphics, rect, 20);
                
                TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, rect, btn.ForeColor, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
        }


        // Filmleri veritabanından çeker ve ekranda listeler
        private void LoadFilms()
        {
            films = DatabaseHelper.GetAllFilms();
            flowPanel.Controls.Clear(); // Önceki listeyi temizle

            foreach (var film in films)
            {
                // Her film için özel bir kart oluştur
                FilmCard card = new FilmCard(film);
                card.CardClick += FilmCard_Click; // Tıklama olayını bağla
                flowPanel.Controls.Add(card);
            }
        }

        private void FilmCard_Click(object sender, EventArgs e)
        {
            FilmCard card = sender as FilmCard;
            if (card != null)
            {
                // Koltuk seçim formuna geç
                SeatSelectionForm seatForm = new SeatSelectionForm(card.FilmData.FilmId);
                seatForm.ShowDialog();
                // Geri dönünce belki yenileme gerekebilir
            }
        }

        // btnBuyTicket removed from code logic since it is not used in the new flow
        // private void btnBuyTicket_Click(object sender, EventArgs e) {} 
        // private void btnBuyTicket_MouseEnter(object sender, EventArgs e) {}
        // private void btnBuyTicket_MouseLeave(object sender, EventArgs e) {}

        // Designer generated code overrides için, InitializeComponent'i elle değiştirmiyoruz
        // Sadece form yüklendiğinde arayüzü manipüle ediyoruz.
    }
}