using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SinemaBiletOtomasyonu.Forms
{
    public class VideoWelcomeForm : Form
    {
        private WebBrowser webBrowser;
        private string videoPath;

        public VideoWelcomeForm(string path)
        {
            this.videoPath = path;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Black;

            InitializeWebBrowser();
        }

        private void InitializeWebBrowser()
        {
            webBrowser = new WebBrowser();
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.ScrollBarsEnabled = false;
            webBrowser.IsWebBrowserContextMenuEnabled = false;
            webBrowser.AllowNavigation = false;
            
            // HTML to play video using HTML5 video tag
            string absolutePath = Path.GetFullPath(videoPath).Replace("\\", "/");
            string html = $@"
                <html>
                <body style='margin:0; padding:0; background-color:black; overflow:hidden;'>
                    <video id='introVideo' width='100%' height='100%' autoplay onended='window.external.CloseForm()'>
                        <source src='file:///{absolutePath}' type='video/mp4'>
                        Your browser does not support the video tag.
                    </video>
                    <script>
                        document.body.onclick = function() {{
                            window.external.CloseForm();
                        }};
                    </script>
                </body>
                </html>";

            this.Controls.Add(webBrowser);
            webBrowser.ObjectForScripting = new ScriptManager(this);
            webBrowser.DocumentText = html;
        }

        [System.Runtime.InteropServices.ComVisible(true)]
        public class ScriptManager
        {
            private VideoWelcomeForm _form;
            public ScriptManager(VideoWelcomeForm form)
            {
                _form = form;
            }

            public void CloseForm()
            {
                _form.DialogResult = DialogResult.OK;
                _form.Close();
            }
        }
    }
}
