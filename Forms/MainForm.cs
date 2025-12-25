using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Database;
using SinemaBiletOtomasyonu.Models;
using System.Collections.Generic;
namespace SinemaBiletOtomasyonu.Forms
{
    public partial class MainForm : Form
    {
        private List<Film> films;
        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
            this.Paint += MainForm_Paint;

            // Double buffering - titreme önleme
            this.DoubleBuffered = true;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadFilms();

            // Fade-in animasyonu
            this.Opacity = 0;
            Timer fadeTimer = new Timer();
            fadeTimer.Interval = 30;
            fadeTimer.Tick += (s, args) =>
            {
                if (this.Opacity < 1)
                    this.Opacity += 0.05;
                else
                    fadeTimer.Stop();
            };
            fadeTimer.Start();
        }
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            // Gradient arka plan
            LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(26, 26, 46),    // #1A1A2E
                Color.FromArgb(15, 52, 96),    // #0F3460
                90F);
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
        }
        private void LoadFilms()
        {
            films = DatabaseHelper.GetAllFilms();
            dgvFilms.DataSource = films;
            // DataGridView düzenleme
            dgvFilms.BackgroundColor = Color.FromArgb(236, 240, 241);
            dgvFilms.BorderStyle = BorderStyle.None;
            dgvFilms.DefaultCellStyle.SelectionBackColor = Color.FromArgb(233, 69, 96);
            dgvFilms.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvFilms.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(26, 26, 46);
            dgvFilms.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvFilms.EnableHeadersVisualStyles = false;

            // Sütun başlıklarını Türkçeleştir
            dgvFilms.Columns["FilmId"].Visible = false;
            dgvFilms.Columns["ImagePath"].Visible = false;
            dgvFilms.Columns["FilmName"].HeaderText = "Film Adı";
            dgvFilms.Columns["Genre"].HeaderText = "Tür";
            dgvFilms.Columns["Duration"].HeaderText = "Süre (dk)";
            dgvFilms.Columns["Price"].HeaderText = "Fiyat (₺)";

            dgvFilms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void btnBuyTicket_Click(object sender, EventArgs e)
        {
            if (dgvFilms.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir film seçin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int selectedFilmId = Convert.ToInt32(dgvFilms.SelectedRows[0].Cells["FilmId"].Value);

            // Koltuk seçim formuna geç
            SeatSelectionForm seatForm = new SeatSelectionForm(selectedFilmId);
            seatForm.ShowDialog();

            // Form kapandıktan sonra listeyi yenile
            LoadFilms();
        }
        private void btnCancelTicket_Click(object sender, EventArgs e)
        {
            CancelTicketForm cancelForm = new CancelTicketForm();
            cancelForm.ShowDialog();
        }
        private void InitializeComponent()
        {
            this.dgvFilms = new System.Windows.Forms.DataGridView();
            this.btnBuyTicket = new System.Windows.Forms.Button();
            this.btnCancelTicket = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilms)).BeginInit();
            this.SuspendLayout();

            // 
            // dgvFilms
            // 
            this.dgvFilms.AllowUserToAddRows = false;
            this.dgvFilms.AllowUserToDeleteRows = false;
            this.dgvFilms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFilms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFilms.Location = new System.Drawing.Point(30, 80);
            this.dgvFilms.MultiSelect = false;
            this.dgvFilms.Name = "dgvFilms";
            this.dgvFilms.ReadOnly = true;
            this.dgvFilms.RowHeadersWidth = 51;
            this.dgvFilms.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFilms.Size = new System.Drawing.Size(740, 330);
            this.dgvFilms.TabIndex = 0;

            // 
            // btnBuyTicket
            // 
            this.btnBuyTicket.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuyTicket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(69)))), ((int)(((byte)(96)))));
            this.btnBuyTicket.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBuyTicket.FlatAppearance.BorderSize = 0;
            this.btnBuyTicket.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuyTicket.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnBuyTicket.ForeColor = System.Drawing.Color.White;
            this.btnBuyTicket.Location = new System.Drawing.Point(550, 430);
            this.btnBuyTicket.Name = "btnBuyTicket";
            this.btnBuyTicket.Size = new System.Drawing.Size(220, 50);
            this.btnBuyTicket.TabIndex = 1;
            this.btnBuyTicket.Text = "🎬 Bilet Al";
            this.btnBuyTicket.UseVisualStyleBackColor = false;
            this.btnBuyTicket.Click += new System.EventHandler(this.btnBuyTicket_Click);
            this.btnBuyTicket.MouseEnter += new System.EventHandler(this.btnBuyTicket_MouseEnter);
            this.btnBuyTicket.MouseLeave += new System.EventHandler(this.btnBuyTicket_MouseLeave);

            // 
            // btnCancelTicket
            // 
            this.btnCancelTicket.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelTicket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(52)))), ((int)(((byte)(96)))));
            this.btnCancelTicket.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelTicket.FlatAppearance.BorderSize = 0;
            this.btnCancelTicket.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelTicket.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancelTicket.ForeColor = System.Drawing.Color.White;
            this.btnCancelTicket.Location = new System.Drawing.Point(30, 430);
            this.btnCancelTicket.Name = "btnCancelTicket";
            this.btnCancelTicket.Size = new System.Drawing.Size(220, 50);
            this.btnCancelTicket.TabIndex = 2;
            this.btnCancelTicket.Text = "❌ Bilet İptal";
            this.btnCancelTicket.UseVisualStyleBackColor = false;
            this.btnCancelTicket.Click += new System.EventHandler(this.btnCancelTicket_Click);
            this.btnCancelTicket.MouseEnter += new System.EventHandler(this.btnCancel_MouseEnter);
            this.btnCancelTicket.MouseLeave += new System.EventHandler(this.btnCancel_MouseLeave);

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(22, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(450, 45);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "🎬 Sinema Bilet Otomasyonu";

            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnCancelTicket);
            this.Controls.Add(this.btnBuyTicket);
            this.Controls.Add(this.dgvFilms);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sinema Bilet Sistemi";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilms)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private void btnBuyTicket_MouseEnter(object sender, EventArgs e)
        {
            btnBuyTicket.BackColor = Color.FromArgb(255, 89, 116); // Daha açık
        }
        private void btnBuyTicket_MouseLeave(object sender, EventArgs e)
        {
            btnBuyTicket.BackColor = Color.FromArgb(233, 69, 96); // Orijinal
        }
        private void btnCancel_MouseEnter(object sender, EventArgs e)
        {
            btnCancelTicket.BackColor = Color.FromArgb(35, 72, 116); // Daha açık
        }
        private void btnCancel_MouseLeave(object sender, EventArgs e)
        {
            btnCancelTicket.BackColor = Color.FromArgb(15, 52, 96); // Orijinal
        }
        private System.Windows.Forms.DataGridView dgvFilms;
        private System.Windows.Forms.Button btnBuyTicket;
        private System.Windows.Forms.Button btnCancelTicket;
        private System.Windows.Forms.Label lblTitle;
    }
}