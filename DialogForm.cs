using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace MinesweeperGame {

    public class DialogForm : Form {

		/// <summary>
		/// DialogFormクラスのコンストラクタ
		/// </summary>
		public DialogForm() {

			this.Text = Settings.WINDOW_TITLE;
			this.Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
			this.ClientSize = new Size(240, 160);
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.WindowsDefaultLocation;
			this.Opacity = 1.0;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.BackColor = Color.Silver;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.AllowDrop = false;
			this.Font = new Font(Settings.FONT_FACE, Settings.FONT_SIZE);
            this.Click += new EventHandler(Form_Click);

			BitmapImages bitmapimages = BitmapImages.GetInstance();

			PictureBox picturebox = new PictureBox();
			picturebox.Location = new Point(this.Width - 40, this.Height - 60);
			picturebox.Size = new Size(17, 17);
			picturebox.BorderStyle = BorderStyle.None;
			picturebox.SizeMode = PictureBoxSizeMode.Zoom;
			picturebox.Image = bitmapimages.SMILE[2];
			this.Controls.Add(picturebox);

            Label label = new Label();
            label.Location = new Point(10, 10);
            label.Size = new Size(this.Width - 20, this.Height - 20);
            label.Text = Settings.DIALOG_TEXT;
            label.Click += new EventHandler(Form_Click);
            this.Controls.Add(label);
		}

        private void Form_Click(object sender, EventArgs e) {

            this.Close();
        }
    }
}