using System;
using System.Drawing;
using System.Windows.Forms;
using SinemaBiletOtomasyonu.Helpers;
using SinemaBiletOtomasyonu.Models;

namespace SinemaBiletOtomasyonu.Forms
{
    /// <summary>
    /// Film ekleme ve düzenleme işlemlerini yöneten form.
    /// Resim yükleme (Dosya, URL, Pano) ve veri doğrulama işlemlerini kapsar.
    /// </summary>
    public class AddEditFilmForm : Form
    {
        private TextBox txtName;
        private ComboBox cmbGenre;
        private NumericUpDown numDuration;
        private NumericUpDown numPrice;
        private PictureBox pbPoster;
        private Button btnSelectImage;
        private Button btnSave;
        private Button btnCancel;
        
        public Film RESULT_FILM { get; private set; }
        private string _selectedImagePath = null; // Path of the newly selected file

        public AddEditFilmForm()
        {
            InitializeComponent();
            ModernUIHelper.ApplyTheme(this);
            this.Text = "Film Ekle";
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 20;
            int inputW = 300;
            int gap = 40;

            // Name
            Label lblName = new Label { Text = "Film Adı:", Location = new Point(20, y), AutoSize = true, ForeColor = Color.White };
            txtName = new TextBox { Location = new Point(130, y - 2), Width = inputW };
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);

            y += gap;
            
            // Genre
            Label lblGenre = new Label { Text = "Tür:", Location = new Point(20, y), AutoSize = true, ForeColor = Color.White };
            cmbGenre = new ComboBox { Location = new Point(130, y - 2), Width = inputW, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbGenre.Items.AddRange(new object[] { "Aksiyon", "Bilim Kurgu", "Komedi", "Dram", "Korku", "Animasyon", "Biyografi", "Macera", "Fantastik" });
            cmbGenre.SelectedIndex = 0;
            this.Controls.Add(lblGenre);
            this.Controls.Add(cmbGenre);

            y += gap;

            // Duration
            Label lblDur = new Label { Text = "Süre (dk):", Location = new Point(20, y), AutoSize = true, ForeColor = Color.White };
            numDuration = new NumericUpDown { Location = new Point(130, y - 2), Width = 100, Minimum = 1, Maximum = 999, Value = 120 };
            this.Controls.Add(lblDur);
            this.Controls.Add(numDuration);

            y += gap;

            // Price
            Label lblPrice = new Label { Text = "Fiyat (TL):", Location = new Point(20, y), AutoSize = true, ForeColor = Color.White };
            numPrice = new NumericUpDown { Location = new Point(130, y - 2), Width = 100, Minimum = 0, Maximum = 1000, Value = 100, DecimalPlaces = 2 };
            this.Controls.Add(lblPrice);
            this.Controls.Add(numPrice);

            y += gap;

            // Image
            Label lblImg = new Label { Text = "Poster:", Location = new Point(20, y), AutoSize = true, ForeColor = Color.White };
            pbPoster = new PictureBox 
            { 
                Location = new Point(130, y), 
                Size = new Size(150, 220), 
                BorderStyle = BorderStyle.FixedSingle, 
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(50, 50, 50),
                AllowDrop = true
            };
            pbPoster.DragEnter += PbPoster_DragEnter;
            pbPoster.DragDrop += PbPoster_DragDrop;
            
            btnSelectImage = new Button { Text = "Resim Seç", Location = new Point(300, y), Width = 100, Height = 30, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSelectImage.Click += BtnSelectImage_Click;

            this.Controls.Add(lblImg);
            this.Controls.Add(pbPoster);
            this.Controls.Add(btnSelectImage);

            y += 240;

            // Buttons
            btnSave = new Button { Text = "Kaydet", Location = new Point(130, y), Width = 100, Height = 40, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.Click += BtnSave_Click;
            
            btnCancel = new Button { Text = "İptal", Location = new Point(240, y), Width = 100, Height = 40, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
            
            // URL Button
            Button btnUrl = new Button { Text = "URL'den Yükle", Location = new Point(300, y + 40), Width = 120, Height = 30, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnUrl.Click += BtnUrl_Click;
            this.Controls.Add(btnUrl);

            // Help Label
            Label lblHelp = new Label();
            lblHelp.Text = "İPUCU: Resim yüklemek için dosyayı buraya sürükleyip bırakabilir,\nCtrl+V ile yapıştırabilir veya URL butonunu kullanabilirsiniz.";
            lblHelp.Location = new Point(300, y + 80);
            lblHelp.Size = new Size(250, 60);
            lblHelp.ForeColor = Color.DarkGray;
            lblHelp.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            this.Controls.Add(lblHelp);

            this.KeyPreview = true;
            this.KeyDown += AddEditFilmForm_KeyDown;
        }

        private void BtnUrl_Click(object sender, EventArgs e)
        {
            string url = Microsoft.VisualBasic.Interaction.InputBox("Resim URL'sini yapıştırın:", "Resim İndir");
            if(!string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    string savedName = ImageHelper.DownloadImageFromUrl(url);
                    _selectedImagePath = null; // Reset file path
                    pbPoster.Image = ImageHelper.LoadImage(savedName);
                    // We temporarily store the downloaded image in PB. 
                    // Since it's ALREADY saved by helper, we can just set the tag or similar.
                    // Actually, LoadImage returns an Image object. 
                    // Let's store the name in Tag so we know it's already saved.
                     pbPoster.Tag = savedName;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void AddEditFilmForm_KeyDown(object sender, KeyEventArgs e)
        {
             if(e.Control && e.KeyCode == Keys.V)
             {
                 PasteImage();
             }
        }

        private void BtnSelectImage_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Title = "Film Posteri Seç";
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                   _selectedImagePath = ofd.FileName;
                   pbPoster.Image = Image.FromFile(_selectedImagePath);
                   pbPoster.Tag = null; // Clear URL tag
                }
            }
        }
        
        private void PasteImage()
        {
            if(Clipboard.ContainsImage())
            {
                pbPoster.Image = Clipboard.GetImage();
                _selectedImagePath = null; // Mark as "Memory Image"
                pbPoster.Tag = null; // Clear any previous URL tag
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Film adı zorunludur!");
                return;
            }

            string imageName = "placeholder"; // Default
            
            try 
            {
                if(_selectedImagePath != null)
                {
                    // File based
                    imageName = ImageHelper.SaveImage(_selectedImagePath);
                }
                else if(pbPoster.Tag != null && pbPoster.Tag is string saved)
                {
                    // Already saved (URL download)
                    imageName = saved;
                }
                else if(pbPoster.Image != null)
                {
                    // Paste from clipboard (Memory)
                    imageName = ImageHelper.SaveImage(pbPoster.Image);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Resim kaydedilirken hata oluştu: " + ex.Message);
                return;
            }

            RESULT_FILM = new Film
            {
                FilmName = txtName.Text.Trim(),
                Genre = cmbGenre.SelectedItem.ToString(),
                Duration = (int)numDuration.Value,
                Price = numPrice.Value,
                ImagePath = imageName
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void PbPoster_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void PbPoster_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                string path = files[0];
                string ext = System.IO.Path.GetExtension(path).ToLower();
                if (ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".bmp")
                {
                    _selectedImagePath = path;
                    pbPoster.Image = Image.FromFile(path);
                    pbPoster.Tag = null; // Clear URL tag
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (pbPoster.Image != null) pbPoster.Image.Dispose();
            base.OnFormClosed(e);
        }
    }
}
