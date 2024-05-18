using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace MinesweeperGame {

	public class BitmapManiprator {

		private static Bitmap _SCORE_PANEL_BMP = null;
		private static Bitmap _PLAY_PANEL_BMP = null;

		private BitmapImages _BITMAP_IMAGES = BitmapImages.GetInstance();
		private Games _GAMES = Games.GetInstance();

		public BitmapManiprator() {

			BitmapSizeChange();
		}

		public void BitmapSizeChange() {

			Size cellSize = _GAMES.CellSize();

			_SCORE_PANEL_BMP = new Bitmap(Settings.CELL_SIZE * cellSize.Width, 32);
			_PLAY_PANEL_BMP = new Bitmap(Settings.CELL_SIZE * cellSize.Width, Settings.CELL_SIZE * cellSize.Height);
		}


		public Bitmap DrawScorePanel() {

			int top_margin = 4;
			int width_margin = 4;
			int digit_width = 13;
			int digit_height = 23;
			int smile_box_width = 23;
			int smile_box_height = 23;
			int smile_width = 17;
			int smile_height = 17;

			using (Graphics g = Graphics.FromImage(_SCORE_PANEL_BMP)) {

				g.FillRectangle(Brushes.Silver, 0, 0, _SCORE_PANEL_BMP.Width, _SCORE_PANEL_BMP.Height);

				
				int nnn = _GAMES.GetMinerCount();
				string NNN = nnn.ToString("000;-00;");

				int[] digit = new int[3];
				for (int i = 0; i < 3; i++) {
					if (NNN.Substring(i, 1).Equals("-"))
						digit[i] = 10;
					else
						digit[i] = Int32.Parse(NNN.Substring(i, 1));
				}

				int xpos = width_margin;
				int ypos = top_margin;
				
				g.DrawImage(_BITMAP_IMAGES.DIGIT[digit[0]], xpos, ypos, digit_width, digit_height);
				g.DrawImage(_BITMAP_IMAGES.DIGIT[digit[1]], xpos + digit_width, ypos, digit_width, digit_height);
				g.DrawImage(_BITMAP_IMAGES.DIGIT[digit[2]], xpos + (digit_width * 2), ypos, digit_width, digit_height);

				nnn = _GAMES.Score();
				NNN = nnn.ToString("000;000;");
				for (int i = 0; i < 3; i++) {
					digit[i] = Int32.Parse(NNN.Substring(i, 1));
				}

				xpos = _SCORE_PANEL_BMP.Width - width_margin - (digit_width * 3);
				ypos = top_margin;
				
				g.DrawImage(_BITMAP_IMAGES.DIGIT[digit[0]], xpos, ypos, digit_width, digit_height);
				g.DrawImage(_BITMAP_IMAGES.DIGIT[digit[1]], xpos + digit_width, ypos, digit_width, digit_height);
				g.DrawImage(_BITMAP_IMAGES.DIGIT[digit[2]], xpos + (digit_width * 2), ypos, digit_width, digit_height);

				xpos = (_SCORE_PANEL_BMP.Width / 2) - (smile_box_width / 2);
				ypos = top_margin;
				ControlPaint.DrawBorder(g, new Rectangle(xpos, ypos, smile_box_width, smile_box_height), Color.Silver, ButtonBorderStyle.Outset);

				xpos = (_SCORE_PANEL_BMP.Width / 2) - (smile_width / 2);
				ypos = (_SCORE_PANEL_BMP.Height / 2) - (smile_height / 2) - 1;
				if (_GAMES.Stopped()) {
					if (_GAMES.GetUnopenedCellCount() == 0)
						g.DrawImage(_BITMAP_IMAGES.SMILE[2], xpos, ypos, smile_width, smile_height);
					else
						g.DrawImage(_BITMAP_IMAGES.SMILE[3], xpos, ypos, smile_width, smile_height);	
				} else
					g.DrawImage(_BITMAP_IMAGES.SMILE[0], xpos, ypos, smile_width, smile_height);
			}
			return _SCORE_PANEL_BMP;
		}



		public Bitmap DrawPlayPanel() {

			Size cellSize = _GAMES.CellSize();

			using (Graphics g = Graphics.FromImage(_PLAY_PANEL_BMP)) {

				g.FillRectangle(Brushes.Silver, 0, 0, _PLAY_PANEL_BMP.Width, _PLAY_PANEL_BMP.Height);

				int xpos = 0;
				int ypos = 0;
				for (int x = 0; x < cellSize.Width; x++) {
					xpos = Settings.CELL_SIZE * x;
					for (int y = 0; y < cellSize.Height; y++) {
						ypos = Settings.CELL_SIZE * y;
						g.DrawImage(_BITMAP_IMAGES.CELL[0], xpos, ypos, Settings.CELL_SIZE, Settings.CELL_SIZE);
					}
				}
			}
			return _PLAY_PANEL_BMP;
		}


		public Bitmap Update1(int X, int Y, MouseButtons state) {

			int rows = (Y / Settings.SCALING_FACTOR) / Settings.CELL_SIZE;
			int cols = (X / Settings.SCALING_FACTOR) / Settings.CELL_SIZE;
				
			// 左ボタンの処理
			if (state == MouseButtons.Left) {
				if (!_GAMES.IsMapOpened(rows, cols) && _GAMES.GetFlagState(rows, cols) != 1) {
					_GAMES.MapOpen(rows, cols);
					int o = _GAMES.Map(rows, cols);

					// 爆弾を引いたらゲームオーバー
					if (o == -1) {
						_GAMES.ShowAllMines();
						_GAMES.Stop();
					} else if (o == 0) {
						_GAMES.OpenAroundCell(rows, cols);
					}
				}
			// 右ボタンの処理
			} else {

				if (!_GAMES.IsMapOpened(rows, cols)) {
					_GAMES.SetFlagState(rows, cols);
				}
			}

            return update2(rows, cols);
		}

		/// <summary>
		/// マップの状況に合わせてGraphicsにビットマップイメージを展開する。
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="cols"></param>
		/// <returns>描画後のBitmapを返却する。</returns>
        public Bitmap update2(int rows, int cols) {

			Size cellSize = _GAMES.CellSize();

			using (Graphics g = Graphics.FromImage(_PLAY_PANEL_BMP)) {

				// 背景を埋める
				g.FillRectangle(Brushes.Silver, 0, 0, _PLAY_PANEL_BMP.Width, _PLAY_PANEL_BMP.Height);

				for (int x = 0; x < cellSize.Width; x++) {
					for (int y = 0; y < cellSize.Height; y++) {
						
						// 閉じているマップ
						if (!_GAMES.IsMapOpened(y, x)) {
							DrawCellImage(g, _BITMAP_IMAGES.CELL[0], y, x);
							int flagState = _GAMES.GetFlagState(y, x);
							if (flagState > 0)
								DrawCellImage(g, _BITMAP_IMAGES.FLAG[--flagState], y, x);
							continue;
						}

						int o = _GAMES.Map(y, x);

						if (o >= 0) {
							DrawCellImage(g, _BITMAP_IMAGES.CELL[1], y, x);
							if (o >= 1)
								DrawCellImage(g, _BITMAP_IMAGES.NUMBER[--o], y, x);
						} else if (o == -1) {
							DrawCellImage(g, _BITMAP_IMAGES.CELL[1], y, x);
							DrawCellImage(g, _BITMAP_IMAGES.MINE[0], y, x);
						} else if (o == -2) {
							DrawCellImage(g, _BITMAP_IMAGES.CELL[1], y, x);
							DrawCellImage(g, _BITMAP_IMAGES.MINE[2], y, x);
						} else
							DrawCellImage(g, _BITMAP_IMAGES.CELL[0], y, x);
						if (x == cols && y == rows && o == -1)
							DrawCellImage(g, _BITMAP_IMAGES.MINE[1], y, x);
					}
				}

			}
			return _PLAY_PANEL_BMP;
        }

		/// <summary>
		/// ビットマップにセルイメージを描画する。
		/// </summary>
		/// <param name="g"></param>
		/// <param name="bmp"></param>
		/// <param name="rows"></param>
		/// <param name="cols"></param>
		private void DrawCellImage(Graphics g, Bitmap bmp, int rows, int cols) {

			int xpos = Settings.CELL_SIZE * cols;
			int ypos = Settings.CELL_SIZE * rows;

			g.DrawImage(bmp, xpos, ypos, Settings.CELL_SIZE, Settings.CELL_SIZE);
		}

	}
}
