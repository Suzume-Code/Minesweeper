using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data;
using System.ComponentModel;

namespace MinesweeperGame {

	public class MainForm : Form {
		
		private BitmapImages _BITMAP_IMAGES = BitmapImages.GetInstance();
		private BitmapManiprator _MANIPRATOR = new BitmapManiprator();

		private PictureBox panel1 = new PictureBox();
		private PictureBox panel2 = new PictureBox();
		private PictureBox panel3 = new PictureBox();
		private PictureBox panel4 = new PictureBox();

		private Games _GAMES = Games.GetInstance();

		private System.Windows.Forms.Timer _TIMER = new System.Windows.Forms.Timer(); 


		/// <summary>
		/// MainFormクラスのコンストラクタ
		/// </summary>
		public MainForm() {

			this.Text = Settings.WINDOW_TITLE;
			this.Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
			this.ClientSize = GetFormSize();
			this.ShowInTaskbar = true;
			this.StartPosition = FormStartPosition.WindowsDefaultLocation;
			this.Opacity = 1.0;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.BackColor = Color.Silver;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.AllowDrop = false;
			this.Font = new Font(Settings.FONT_FACE, Settings.FONT_SIZE);
			this.Resize += new System.EventHandler(this.Form_Resize);

    		_TIMER.Interval = 1000;
			_TIMER.Tick += new EventHandler(TimerEventProcessor);
        	_TIMER.Enabled = false;

			InitMenus();
			InitViews();
		}

		/// <summary>
		/// メニューを作成する。
		/// </summary>
		private void InitMenus() {

			MenuStrip ms = new MenuStrip();

            // ゲームメニュー
            ToolStripMenuItem gameMenu = new ToolStripMenuItem(Settings.GAME_MENU);

            ToolStripMenuItem newGameMenu = new ToolStripMenuItem(Settings.NEW_GAME, null, new EventHandler(NewGame));
			newGameMenu.ShortcutKeys = Keys.F2;
            gameMenu.DropDownItems.Add(newGameMenu);

			gameMenu.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem easyMenu = new ToolStripMenuItem(Settings.EASY_MENU, null, new EventHandler(ChangeToEasyMode));
            gameMenu.DropDownItems.Add(easyMenu);
            ToolStripMenuItem mediumMenu = new ToolStripMenuItem(Settings.MEDIUM_MENU, null, new EventHandler(ChangeToMediumMode));
            gameMenu.DropDownItems.Add(mediumMenu);
			ToolStripMenuItem expartMenu = new ToolStripMenuItem(Settings.EXPART_MENU, null, new EventHandler(ChangeToExpartMode));
            gameMenu.DropDownItems.Add(expartMenu);

			gameMenu.DropDownItems.Add(new ToolStripSeparator());

			ToolStripMenuItem exitMenu = new ToolStripMenuItem(Settings.EXIT_MENU, null, new EventHandler(ExitGame));
			exitMenu.ShortcutKeys = Keys.F4;
			gameMenu.DropDownItems.Add(exitMenu);

            ms.Items.Add(gameMenu);

            // ヘルプメニュー
            ToolStripMenuItem helpMenu = new ToolStripMenuItem(Settings.HELP_MENU);

			// Minesweeperについて...
            ToolStripMenuItem helpAboutMenu = new ToolStripMenuItem(Settings.ABOUT_MENU, null, new EventHandler(ShowDialog));
            helpMenu.DropDownItems.Add(helpAboutMenu);

			this.Controls.Add(panel2);
			helpMenu.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem NormalSizeMenu = new ToolStripMenuItem(Settings.SINGLE_SCALE, null, new EventHandler(NormalScreenSize));
            helpMenu.DropDownItems.Add(NormalSizeMenu);

            ToolStripMenuItem DoubleSizeMenu = new ToolStripMenuItem(Settings.DOUBLE_SCALE, null, new EventHandler(DoubleScreenSize));
            helpMenu.DropDownItems.Add(DoubleSizeMenu);

            ToolStripMenuItem MaxSizeMenu = new ToolStripMenuItem(Settings.QUAD_SCALE, null, new EventHandler(MaxScreenSize));
            helpMenu.DropDownItems.Add(MaxSizeMenu);

            ms.Items.Add(helpMenu);

            Controls.Add(ms);
		}

		/// <summary>
		/// フォーム上のオブジェクトを配置する。
		/// </summary>
		private void InitViews() {
			
			panel1.Location = new Point(Settings.BORDER_WIDTH * Settings.SCALING_FACTOR, (Settings.BORDER_WIDTH * Settings.SCALING_FACTOR) + Settings.MENU_HEIGHT);
			panel1.Size = GetScorePanelSize();
			panel1.BorderStyle = BorderStyle.None;
			panel1.SizeMode = PictureBoxSizeMode.Zoom;
			panel1.MouseClick += new MouseEventHandler(panel1_Click);
			this.Controls.Add(panel1);

			panel1.Image = _MANIPRATOR.DrawScorePanel();

			panel4.Top = panel1.Top - (2 * Settings.SCALING_FACTOR);
			panel4.Left = panel1.Left - (2 * Settings.SCALING_FACTOR);
			panel4.Width = panel1.Width + (4 * Settings.SCALING_FACTOR);
			panel4.Height = panel1.Height + (4 * Settings.SCALING_FACTOR);
			panel4.BorderStyle = BorderStyle.Fixed3D;
			this.Controls.Add(panel4);

			panel2.Location = new Point(Settings.BORDER_WIDTH * Settings.SCALING_FACTOR, Settings.BORDER_WIDTH + Settings.MENU_HEIGHT + (40 * Settings.SCALING_FACTOR));
			panel2.Size = GetPlayPanelSize();
			panel2.BorderStyle = BorderStyle.None;
			panel2.SizeMode = PictureBoxSizeMode.Zoom;
			panel2.BackColor = Color.Silver;
			panel2.MouseClick += new MouseEventHandler(panel2_Click);
			this.Controls.Add(panel2);
			
			panel2.Image = _MANIPRATOR.DrawPlayPanel();

			panel3.Top = panel2.Top - (2 * Settings.SCALING_FACTOR);
			panel3.Left = panel2.Left - (2 * Settings.SCALING_FACTOR);
			panel3.Width = panel2.Width + (4 * Settings.SCALING_FACTOR);
			panel3.Height = panel2.Height + (4 * Settings.SCALING_FACTOR);
			panel3.BorderStyle = BorderStyle.Fixed3D;
			this.Controls.Add(panel3);
		}

		/// <summary>
		/// クリックイベント：スコアパネルをクリックしたときの処理。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panel1_Click(object sender, MouseEventArgs e) {

			_GAMES.InitGames();

			panel2.Image = _MANIPRATOR.DrawPlayPanel();
			panel1.Image = _MANIPRATOR.DrawScorePanel();
		}

		/// <summary>
		/// クリックイベント：プレイパネルをクリックしたときの処理。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panel2_Click(object sender, MouseEventArgs e) {

			if (_GAMES.Stopped()) {
				_TIMER.Enabled = false;
				return;
			}
			
			panel2.Image = _MANIPRATOR.Update1(e.X, e.Y, e.Button);
			panel1.Image = _MANIPRATOR.DrawScorePanel();

			int count = _GAMES.GetUnopenedCellCount();
			if (count == 0) {
				_TIMER.Enabled = false;
				_GAMES.Stop();
				panel1.Image = _MANIPRATOR.DrawScorePanel();
			}

			if (!_TIMER.Enabled)
				_TIMER.Enabled = true;
		}

		/// <summary>
		/// フォームサイズを計算する。
		/// </summary>
		/// <returns>計算結果をSizeで返却する。</returns>
		private Size GetFormSize() {

			Size cellSize = _GAMES.CellSize();

			int width = (Settings.CELL_SIZE * cellSize.Width) + (Settings.BORDER_WIDTH * 2);
			int height = Settings.SCORE_PANEL_HEIGHT + (Settings.CELL_SIZE * cellSize.Height) + (Settings.BORDER_WIDTH * 3);
			
			return new Size(width * Settings.SCALING_FACTOR, (height * Settings.SCALING_FACTOR) + Settings.MENU_HEIGHT);
		}

		/// <summary>
		/// スコアパネルのサイズを計算する。
		/// </summary>
		/// <returns>計算結果をSizeで返却する。</returns>
		private Size GetScorePanelSize() {

			Size cellSize = _GAMES.CellSize();

			int width = (Settings.CELL_SIZE * cellSize.Width);
			int height = 32;

			return new Size(width * Settings.SCALING_FACTOR, height * Settings.SCALING_FACTOR);
		}

		/// <summary>
		/// ゲームパネルのサイズを計算する。
		/// </summary>
		/// <returns>計算結果をSizeで返却する。</returns>
		private Size GetPlayPanelSize() {

			Size cellSize = _GAMES.CellSize();

			int width = (Settings.CELL_SIZE * cellSize.Width);
			int height = (Settings.CELL_SIZE * cellSize.Height);
			
			return new Size(width * Settings.SCALING_FACTOR, height * Settings.SCALING_FACTOR);
		}

		/// <summary>
		/// メニューイベント：新規にゲームを開始する。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NewGame(object sender, EventArgs e) {

			_GAMES.InitGames();

			panel2.Image = _MANIPRATOR.DrawPlayPanel();
			panel1.Image = _MANIPRATOR.DrawScorePanel();
        }

		/// <summary>
		/// メニューイベント：モードを変更する(EASY)。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChangeToEasyMode(object sender, EventArgs e) {

			ModeChange(Games.PlayLevel.Beginner);
		}

		/// <summary>
		/// メニューイベント：モードを変更する(MEDIUM)。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChangeToMediumMode(object sender, EventArgs e) {

			ModeChange(Games.PlayLevel.Intermediate);
		}

		/// <summary>
		/// メニューイベント：モードを変更する(EXPART)。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChangeToExpartMode(object sender, EventArgs e) {

			ModeChange(Games.PlayLevel.Expart);
		}

		/// <summary>
		/// モードを反映するための処理
		/// </summary>
		/// <param name="playLevel"></param>
		private void ModeChange(Games.PlayLevel playLevel) {

			_TIMER.Enabled = false;
			
			_GAMES.Stop();

			_GAMES.ChangePlayLevel(playLevel);
			_GAMES.InitGames();
			
			_MANIPRATOR.BitmapSizeChange();

			this.ClientSize = GetFormSize();
		}

		/// <summary>
		/// メニューイベント：アプリケーションを終了する。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExitGame(object sender, EventArgs e) {

			_TIMER.Enabled = false;
			
			this.Close();
		}

		/// <summary>
		/// フォームサイズを変更する。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NormalScreenSize(object sender, EventArgs e) {

			Settings.SCALING_FACTOR = 1;
			this.ClientSize = GetFormSize();
        }

		/// <summary>
		/// フォームサイズを変更する。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DoubleScreenSize(object sender, EventArgs e) {

			Settings.SCALING_FACTOR = 2;
			this.ClientSize = GetFormSize();
        }

		/// <summary>
		/// フォームサイズを変更する。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MaxScreenSize(object sender, EventArgs e) {

			Settings.SCALING_FACTOR = 4;
			this.ClientSize = GetFormSize();
        }

		/// <summary>
		/// メニュイベント：ダイアログを表示する。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ShowDialog(object sender, EventArgs e) {

			DialogForm dialog = new DialogForm();
			dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
			dialog.Dispose();
        }

		/// <summary>
		/// フォームリサイズイベント。フォーム内のコントロールのサイズを変更する。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form_Resize(object sender, EventArgs e) {

			panel1.Location = new Point(Settings.BORDER_WIDTH * Settings.SCALING_FACTOR, (Settings.BORDER_WIDTH * Settings.SCALING_FACTOR) + Settings.MENU_HEIGHT);
			panel1.Size = GetScorePanelSize();

			panel4.Top = panel1.Top - (2 * Settings.SCALING_FACTOR);
			panel4.Left = panel1.Left - (2 * Settings.SCALING_FACTOR);
			panel4.Width = panel1.Width + (4 * Settings.SCALING_FACTOR);
			panel4.Height = panel1.Height + (4 * Settings.SCALING_FACTOR);

			panel2.Location = new Point(Settings.BORDER_WIDTH * Settings.SCALING_FACTOR, Settings.BORDER_WIDTH + Settings.MENU_HEIGHT + (40 * Settings.SCALING_FACTOR));
			panel2.Size = GetPlayPanelSize();
			
			panel3.Top = panel2.Top - (2 * Settings.SCALING_FACTOR);
			panel3.Left = panel2.Left - (2 * Settings.SCALING_FACTOR);
			panel3.Width = panel2.Width + (4 * Settings.SCALING_FACTOR);
			panel3.Height = panel2.Height + (4 * Settings.SCALING_FACTOR);
			
			// 再描画
			panel2.Image = _MANIPRATOR.update2(-1, -1);
			panel1.Image = _MANIPRATOR.DrawScorePanel();
			Console.WriteLine("FORM-RESIZED");
        }

		/// <summary>
		/// タイマーイベント：スコアを更新する。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimerEventProcessor(Object sender, EventArgs e) {
        
			if (_GAMES.Stopped()) {
				_TIMER.Enabled = false;
				return;
			}
			
			_GAMES.SetScore();

			panel1.Image = _MANIPRATOR.DrawScorePanel();
    	}

	}

}
