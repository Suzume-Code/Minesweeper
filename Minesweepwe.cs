using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace MinesweeperGame {

	class Program {
 
		[STAThread]
		public static void Main(string[] args) {

			if (StartupProcessCheck())
				return;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

        /// <summary>
		/// アプリケーション開始時のチェック
		/// </summary>
        /// <returns>自分自身を含んで同名のプロセスが２つ以上起動していればtrue、以外はfalse。</returns>
        static private bool StartupProcessCheck() {

            return (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1);
        }
	}

	/// <summary>
	/// プログラム全体の設定値クラス
	/// </summary>
	public class Settings {

        public static readonly string WINDOW_TITLE = "Minesweeper";

		public static int SCALING_FACTOR = 1;

		public static readonly int MENU_HEIGHT = 24;
		public static readonly int SCORE_PANEL_HEIGHT = 32;
		public static readonly int BORDER_WIDTH = 6;
		public static readonly int CELL_SIZE = 16;

        public static readonly string FONT_FACE = "メイリオ";
        public static readonly int FONT_SIZE = 9;

		public static readonly string DIALOG_TITLE = "Minesweeperについて...";
		public static readonly string DIALOG_TEXT = "Minesweeperへようこそ。\n\nWindows標準の.NET Framework v4.0.30319で作成しました。\nサウンド機能はありません。\n\nHave fun.";
    
		// ゲームメニュー
		public static readonly string GAME_MENU = "Game";
		// 新規ゲーム
		public static readonly string NEW_GAME = "New Game";
		// セルサイズ
		public static readonly string EASY_MENU = "Easy 9x9";
		public static readonly string MEDIUM_MENU = "Medium 16x16";
		public static readonly string EXPART_MENU = "Expart 30x16";
		//
		public static readonly string EXIT_MENU = "Exit";

		// ヘルプメニュー
		public static readonly string HELP_MENU = "Help";
		// アバウトメニュー
		public static readonly string ABOUT_MENU = "Minesweeperについて...";
		// フォーム倍率
		public static readonly string SINGLE_SCALE = "1x Normal";
		public static readonly string DOUBLE_SCALE = "2x Enlarge";
		public static readonly string QUAD_SCALE = "4x Enlarge";

		// EASY設定
		public static readonly int EASY_CELL_WIDTH = 9;
		public static readonly int EASY_CELL_HEIGHT = 9;
		public static readonly int EASY_NUMBER_OF_MINES = 10;
		// MEDIUM設定
		public static readonly int MEDIUM_CELL_WIDTH = 16;
		public static readonly int MEDIUM_CELL_HEIGHT = 16;
		public static readonly int MEDIUM_NUMBER_OF_MINES = 40;
		// EXPART設定
		public static readonly int EXPART_CELL_WIDTH = 30;
		public static readonly int EXPART_CELL_HEIGHT = 16;
		public static readonly int EXPART_NUMBER_OF_MINES = 99;
	}

}
