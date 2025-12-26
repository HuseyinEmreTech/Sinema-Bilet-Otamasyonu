using System;
using System.Drawing;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Helpers;
using SinemaBiletOtomasyonu.Models;

namespace SinemaBiletOtomasyonu.Forms
{
    public class AddEditHallForm : Form
    {
        private TextBox txtHallName;
        private NumericUpDown numRows;
        private NumericUpDown numCols;
        private NumericUpDown numMultiplier;
        private Button btnSave;
        private Button btnCancel;

        public Hall RESULT_HALL { get; private set; }

        public AddEditHallForm()
        {
            InitializeComponent();
            ModernUIHelper.ApplyTheme(this);
            this.Text = "Salon Ekle";
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 20;
            int inputW = 200;
            int gap = 40;

            // Hall Name
            Label lblName = new Label { Text = "Salon Adı:", Location = new Point(20, y), AutoSize = true, ForeColor = Color.White };
            txtHallName = new TextBox { Location = new Point(150, y - 2), Width = inputW };
            this.Controls.Add(lblName);
            this.Controls.Add(txtHallName);

            y += gap;

            // Rows
            Label lblRows = new Label { Text = "Sıra Sayısı:", Location = new Point(20, y), AutoSize = true, ForeColor = Color.White };
            numRows = new NumericUpDown { Location = new Point(150, y - 2), Width = 60, Minimum = 1, Maximum = 20, Value = 5 };
            this.Controls.Add(lblRows);
            this.Controls.Add(numRows);

            y += gap;

            // Columns
            Label lblCols = new Label { Text = "Sütun Sayısı:", Location = new Point(20, y), AutoSize = true, ForeColor = Color.White };
            numCols = new NumericUpDown { Location = new Point(150, y - 2), Width = 60, Minimum = 1, Maximum = 20, Value = 8 };
            this.Controls.Add(lblCols);
            this.Controls.Add(numCols);

            y += gap;

            // Multiplier
            Label lblMult = new Label { Text = "Fiyat Çarpanı:", Location = new Point(20, y), AutoSize = true, ForeColor = Color.White };
            numMultiplier = new NumericUpDown { Location = new Point(150, y - 2), Width = 60, Minimum = 0.5m, Maximum = 5.0m, Value = 1.0m, DecimalPlaces = 1, Increment = 0.1m };
            this.Controls.Add(lblMult);
            this.Controls.Add(numMultiplier);

            y += 60;

            // Buttons
            btnSave = new Button { Text = "Kaydet", Location = new Point(150, y), Width = 90, Height = 40, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button { Text = "İptal", Location = new Point(260, y), Width = 90, Height = 40, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHallName.Text))
            {
                MessageBox.Show("Salon adı zorunludur!");
                return;
            }

            RESULT_HALL = new Hall
            {
                HallName = txtHallName.Text.Trim(),
                RowCount = (int)numRows.Value,
                ColumnCount = (int)numCols.Value,
                PriceMultiplier = numMultiplier.Value
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
