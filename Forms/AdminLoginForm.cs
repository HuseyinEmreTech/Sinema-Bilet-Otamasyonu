using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Helpers;

namespace SinemaBiletOtomasyonu.Forms
{
    public class AdminLoginForm : Form
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;
        private Button btnCancel;
        private Label lblTitle;

        public AdminLoginForm()
        {
            this.Text = "Yönetici Girişi";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            ModernUIHelper.ApplyTheme(this);

            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Title
            lblTitle = new Label();
            lblTitle.Text = "YÖNETİCİ GİRİŞİ";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = ModernUIHelper.PrimaryColor;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point((this.Width - 180) / 2, 40);
            this.Controls.Add(lblTitle);

            // User
            Label lblUser = new Label { Text = "Kullanıcı Adı:", ForeColor = Color.LightGray, Location = new Point(50, 100), AutoSize = true };
            txtUser = new TextBox { Location = new Point(150, 100), Width = 200, BackColor = Color.FromArgb(50, 50, 50), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(lblUser);
            this.Controls.Add(txtUser);

            // Pass
            Label lblPass = new Label { Text = "Şifre:", ForeColor = Color.LightGray, Location = new Point(50, 140), AutoSize = true };
            txtPass = new TextBox { Location = new Point(150, 140), Width = 200, BackColor = Color.FromArgb(50, 50, 50), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, PasswordChar = '*' };
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPass);

            // Buttons
            btnLogin = new Button { Text = "GİRİŞ", Size = new Size(100, 40), Location = new Point(80, 200) };
            StyleButton(btnLogin, ModernUIHelper.PrimaryColor);
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);

            btnCancel = new Button { Text = "İPTAL", Size = new Size(100, 40), Location = new Point(220, 200) };
            StyleButton(btnCancel, Color.Gray);
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
            
            // Draw Border
            this.Paint += (s, e) => 
            {
                 using(Pen p = new Pen(ModernUIHelper.PrimaryColor, 2))
                 {
                     e.Graphics.DrawRectangle(p, 0, 0, Width-1, Height-1);
                 }
            };
        }

        private void StyleButton(Button btn, Color color)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if(txtUser.Text == "admin" && txtPass.Text == "1234")
            {
                this.Hide();
                AdminDashboardForm dashboard = new AdminDashboardForm();
                dashboard.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Hatalı kullanıcı adı veya şifre!");
            }
        }
    }
}
