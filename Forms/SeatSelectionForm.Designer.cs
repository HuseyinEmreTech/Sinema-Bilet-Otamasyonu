namespace SinemaBiletOtomasyonu.Forms
{
    partial class SeatSelectionForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.cmbHalls = new System.Windows.Forms.ComboBox();
            this.cmbSessions = new System.Windows.Forms.ComboBox();
            this.panelSeats = new System.Windows.Forms.Panel();
            this.lblFilmName = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblSelectedSeats = new System.Windows.Forms.Label();
            this.btnProceedToPayment = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbHalls
            // 
            this.cmbHalls.FormattingEnabled = true;
            this.cmbHalls.Location = new System.Drawing.Point(30, 80);
            this.cmbHalls.Name = "cmbHalls";
            this.cmbHalls.Size = new System.Drawing.Size(200, 21);
            this.cmbHalls.TabIndex = 0;
            this.cmbHalls.SelectedIndexChanged += new System.EventHandler(this.cmbHalls_SelectedIndexChanged);
            // 
            // cmbSessions
            // 
            this.cmbSessions.FormattingEnabled = true;
            this.cmbSessions.Location = new System.Drawing.Point(250, 80);
            this.cmbSessions.Name = "cmbSessions";
            this.cmbSessions.Size = new System.Drawing.Size(150, 21);
            this.cmbSessions.TabIndex = 8;
            this.cmbSessions.SelectedIndexChanged += new System.EventHandler(this.cmbSessions_SelectedIndexChanged);
            // 
            // panelSeats
            // 
            this.panelSeats.Location = new System.Drawing.Point(30, 200);
            this.panelSeats.Name = "panelSeats";
            this.panelSeats.Size = new System.Drawing.Size(740, 300);
            this.panelSeats.TabIndex = 1;
            this.panelSeats.AutoScroll = true;
            // 
            // lblFilmName
            // 
            this.lblFilmName.AutoSize = true;
            this.lblFilmName.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblFilmName.ForeColor = System.Drawing.Color.White;
            this.lblFilmName.Location = new System.Drawing.Point(30, 20);
            this.lblFilmName.Name = "lblFilmName";
            this.lblFilmName.Size = new System.Drawing.Size(100, 25);
            this.lblFilmName.TabIndex = 2;
            this.lblFilmName.Text = "Film Adı";
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblPrice.ForeColor = System.Drawing.Color.LightGray;
            this.lblPrice.Location = new System.Drawing.Point(30, 50);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(50, 20);
            this.lblPrice.TabIndex = 3;
            this.lblPrice.Text = "0.00 ₺";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.White;
            this.lblTotal.Location = new System.Drawing.Point(600, 520);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(72, 25);
            this.lblTotal.TabIndex = 4;
            this.lblTotal.Text = "0.00 ₺";
            // 
            // lblSelectedSeats
            // 
            this.lblSelectedSeats.AutoSize = true;
            this.lblSelectedSeats.ForeColor = System.Drawing.Color.White;
            this.lblSelectedSeats.Location = new System.Drawing.Point(30, 520);
            this.lblSelectedSeats.Name = "lblSelectedSeats";
            this.lblSelectedSeats.Size = new System.Drawing.Size(100, 13);
            this.lblSelectedSeats.TabIndex = 5;
            this.lblSelectedSeats.Text = "0 SEÇİLİ KOLTUK";
            // 
            // btnProceedToPayment
            // 
            this.btnProceedToPayment.Location = new System.Drawing.Point(600, 550);
            this.btnProceedToPayment.Name = "btnProceedToPayment";
            this.btnProceedToPayment.Size = new System.Drawing.Size(150, 40);
            this.btnProceedToPayment.TabIndex = 6;
            this.btnProceedToPayment.Text = "Ödemeye Geç";
            this.btnProceedToPayment.Click += new System.EventHandler(this.btnProceedToPayment_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(30, 550);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(100, 40);
            this.btnBack.TabIndex = 7;
            this.btnBack.Text = "Geri";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // SeatSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnProceedToPayment);
            this.Controls.Add(this.lblSelectedSeats);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.lblFilmName);
            this.Controls.Add(this.panelSeats);
            this.Controls.Add(this.cmbSessions);
            this.Controls.Add(this.cmbHalls);
            this.Name = "SeatSelectionForm";
            this.Text = "Koltuk Seçimi";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbHalls;
        private System.Windows.Forms.ComboBox cmbSessions;
        private System.Windows.Forms.Panel panelSeats;
        private System.Windows.Forms.Label lblFilmName;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblSelectedSeats;
        private System.Windows.Forms.Button btnProceedToPayment;
        private System.Windows.Forms.Button btnBack;
    }
}
