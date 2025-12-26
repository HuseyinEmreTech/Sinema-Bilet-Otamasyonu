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
        
        // Curtain Panels
        private Panel pnlLeftCurtain;
        private Panel pnlRightCurtain;
        private int curtainWidth;

        public WelcomeForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(600, 350);
            this.BackColor = ModernUIHelper.DarkBackground; // Logo background
            this.Opacity = 1; // Start fully visible for curtains

            // Content (Behind Curtains)
            lblWelcome = new Label();
            lblWelcome.Text = "SİNEMA DÜNYASI";
            lblWelcome.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblWelcome.ForeColor = ModernUIHelper.PrimaryColor;
            lblWelcome.AutoSize = true;
            lblWelcome.Location = new Point((this.Width - 300) / 2, 120);
            
            lblSub = new Label();
            lblSub.Text = "Hoş Geldiniz...";
            lblSub.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblSub.ForeColor = Color.White;
            lblSub.AutoSize = true;
            lblSub.Location = new Point((this.Width - 100) / 2, 180);

            this.Controls.Add(lblWelcome);
            this.Controls.Add(lblSub);

            // Center Content
            lblWelcome.Location = new Point((this.Width - lblWelcome.PreferredWidth) / 2, 120);
            lblSub.Location = new Point((this.Width - lblSub.PreferredWidth) / 2, 170);

            // Initialize Curtains
            curtainWidth = this.Width / 2;
            
            pnlLeftCurtain = new Panel();
            pnlLeftCurtain.Size = new Size(curtainWidth, this.Height);
            pnlLeftCurtain.Location = new Point(0, 0);
            pnlLeftCurtain.BackColor = Color.FromArgb(192, 57, 43); // Red Curtain
            pnlLeftCurtain.Paint += Curtain_Paint; // Add some folds?
            this.Controls.Add(pnlLeftCurtain);
            this.Controls.SetChildIndex(pnlLeftCurtain, 0); // Bring to front

            pnlRightCurtain = new Panel();
            pnlRightCurtain.Size = new Size(curtainWidth, this.Height);
            pnlRightCurtain.Location = new Point(curtainWidth, 0);
            pnlRightCurtain.BackColor = Color.FromArgb(192, 57, 43);
            pnlRightCurtain.Paint += Curtain_Paint;
            this.Controls.Add(pnlRightCurtain);
            this.Controls.SetChildIndex(pnlRightCurtain, 0);

            timer = new Timer();
            timer.Interval = 20; // Faster
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Curtain_Paint(object sender, PaintEventArgs e)
        {
            // Simple Fold Effect
            Panel p = sender as Panel;
            Graphics g = e.Graphics;
            using (Pen pen = new Pen(Color.FromArgb(50, 0, 0, 0), 2))
            {
                for(int i=0; i<p.Width; i+=40)
                {
                    g.DrawLine(pen, i, 0, i, p.Height);
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            duration += 20;

            // Wait 500ms then Open
            if(duration > 500)
            {
                // Open Curtains
                if (pnlLeftCurtain.Width > 0)
                {
                    int step = 10;
                    pnlLeftCurtain.Width -= step;
                    
                    pnlRightCurtain.Width -= step;
                    pnlRightCurtain.Left += step;
                }
                else
                {
                    // Fully Open, wait a bit then close
                    if (duration > 2500)
                    {
                        timer.Stop();
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }
    }
}
