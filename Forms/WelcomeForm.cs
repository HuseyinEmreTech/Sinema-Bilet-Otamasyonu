using System;
using System.Drawing;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Helpers;

namespace SinemaBiletOtomasyonu.Forms
{
    public class WelcomeForm : Form
    {
        private Timer timer;
        private int duration = 0;
        private Label lblWelcome;
        private Label lblSub;

        public WelcomeForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(600, 350);
            this.BackColor = ModernUIHelper.DarkBackground;
            this.Opacity = 0;

            lblWelcome = new Label();
            lblWelcome.Text = "SİNEMA DÜNYASI";
            lblWelcome.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblWelcome.ForeColor = ModernUIHelper.PrimaryColor;
            lblWelcome.AutoSize = true;
            lblWelcome.Location = new Point((this.Width - 300) / 2, 120); // Center approx
            
            lblSub = new Label();
            lblSub.Text = "Hoş Geldiniz...";
            lblSub.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblSub.ForeColor = Color.White;
            lblSub.AutoSize = true;
            lblSub.Location = new Point((this.Width - 100) / 2, 180);

            this.Controls.Add(lblWelcome);
            this.Controls.Add(lblSub);

            // Center manually
            lblWelcome.Location = new Point((this.Width - lblWelcome.PreferredWidth) / 2, 120);
            lblSub.Location = new Point((this.Width - lblSub.PreferredWidth) / 2, 170);

            timer = new Timer();
            timer.Interval = 30; // 30ms render
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            duration += 30;

            // Fade In (First 1 sec)
            if (duration < 1000)
            {
                if (this.Opacity < 1) this.Opacity += 0.05;
            }
            
            // Wait (1-2.5 sec)
            if (duration > 2500)
            {
                // Fade Out
                if (this.Opacity > 0) 
                    this.Opacity -= 0.05;
                else
                {
                    timer.Stop();
                    this.DialogResult = DialogResult.OK; // Sinyal ver
                    this.Close();
                }
            }
        }
    }
}
