using System;
using System.Collections.Generic;
using System.Drawing;

namespace MinesweeperGame {

	public class Games {

        // インスタンス
        private static Games singleton = null;

		public enum PlayLevel {
			Beginner,
			Intermediate,
			Expart,
			Custom,
		}
		public PlayLevel PLAY_LEVEL = PlayLevel.Beginner;

		private int _ARRAY_WIDTH = 0;
		private int _ARRAY_HEIGHT = 0;
		private int _ARRAY_MINES = 0;

        private int _DISP_SCORE = 0;
        private int _DISP_MINE = 0;

        public int[,] _MAP = { { } };
        public bool[,] _IS_MAP_OPEN = { { } };
 
        private List<Position> positions;
        private List<Position> mines;
        private List<PositionState> _FLAGSTATE;

        private static Random _RANDOM = new Random();

        private bool _GAME_STOP = false;

        /// <summary>
        /// インスタンスを作成する。
        /// </summary>
        /// <returns>当クラスのインスタンスを返却する。</returns>
        public static Games GetInstance() {

            if (singleton == null) {
                singleton = new Games();
            }
            return singleton;
        }

        /// <summary>
        /// 当クラスのコンストラクター。
        /// </summary>
        private Games() {

            ChangePlayLevel(PlayLevel.Beginner);
            InitGames();
        }

        /// <summary>
        /// ゲームのプレイレベルを設定する。
        /// 設定できるレベルは３段階。
        /// レベルに合わせてセル数、地雷数が変更になる。
        /// </summary>
        /// <param name="play_lavel"></param>
        public void ChangePlayLevel(PlayLevel play_lavel) {

            PLAY_LEVEL = play_lavel;

            // セル数、地雷数をレベル毎に設定する。
            if (PLAY_LEVEL == PlayLevel.Beginner) {
                _ARRAY_WIDTH = Settings.EASY_CELL_WIDTH;
                _ARRAY_HEIGHT = Settings.EASY_CELL_HEIGHT;
                _ARRAY_MINES = Settings.EASY_NUMBER_OF_MINES;
            } else if (PLAY_LEVEL == PlayLevel.Intermediate){
                _ARRAY_WIDTH = Settings.MEDIUM_CELL_WIDTH;
                _ARRAY_HEIGHT = Settings.MEDIUM_CELL_HEIGHT;
                _ARRAY_MINES = Settings.MEDIUM_NUMBER_OF_MINES;
            } else {
                _ARRAY_WIDTH = Settings.EXPART_CELL_WIDTH;
                _ARRAY_HEIGHT = Settings.EXPART_CELL_HEIGHT;
                _ARRAY_MINES = Settings.EXPART_NUMBER_OF_MINES;
            }
        }

        /// <summary>
        /// ゲームを初期化する。
        /// </summary>
        public void InitGames() {

            _DISP_SCORE = 0;
            _DISP_MINE = _ARRAY_MINES;

            _GAME_STOP = false;
        
            // 最初はMapの各要素を0に、IsMapOpenの各要素をfalseにする
            _MAP = new int[_ARRAY_HEIGHT, _ARRAY_WIDTH];
            _IS_MAP_OPEN = new bool[_ARRAY_HEIGHT, _ARRAY_WIDTH];

            positions = new List<Position>();
            mines = new List<Position>();
            _FLAGSTATE = new List<PositionState>();

            // 二次元配列をリストに変換、Positionにcol, rowを格納する
            for (int row = 0; row < _ARRAY_HEIGHT; row++)
                for (int col = 0; col < _ARRAY_WIDTH; col++)
                    positions.Add(new Position(col, row));

            // リストのなかから地雷をセットするPositionをランダムに選ぶ
            for (int i = 0; i < _ARRAY_MINES; i++) {
                int r = _RANDOM.Next(positions.Count);
                mines.Add(positions[r]);
                positions.RemoveAt(r);
            }

            // 地雷をセットしたPositionの周囲に地雷の数をセットする
            int[] dx = { 0, 0, 1, -1, 1, -1, 1, -1 };
            int[] dy = { 1, -1, 0, 0, 1, -1, -1, 1 };
            foreach (Position mine in mines) {
                for (int k = 0; k < dx.Length; k++) {
                    int ncol = mine.Column + dx[k];
                    int nrow = mine.Row + dy[k];
                    if (ncol < 0 || ncol >= _ARRAY_WIDTH || nrow < 0 || nrow >= _ARRAY_HEIGHT)
                        continue;
                    _MAP[nrow, ncol] += 1;
                }
            }
        
            // 地雷がセットされているPositionには-1を代入する
            foreach (Position mine in mines)
                _MAP[mine.Row, mine.Column] = -1;
        }


        /// <summary>
        /// 地雷が埋まっているセルを全部開く。
        /// </summary>
        public void ShowAllMines() {

            foreach (Position mine in mines)
                //if (GetFlagState(mine.Row, mine.Column) != 1)
                _IS_MAP_OPEN[mine.Row, mine.Column] = true;
            
            foreach (PositionState flag in _FLAGSTATE)
                if (_MAP[flag.Row, flag.Column] != -1) {
                    _MAP[flag.Row, flag.Column] = -2;
                    _IS_MAP_OPEN[flag.Row, flag.Column] = true;
                }
        }

        /// <summary>
        /// 自セル周辺のセルを開く。
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public void OpenAroundCell(int rows, int cols) {

            int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
            int[] dy = { -1, -1, 0, 1, 1, 1, 0, -1 };

            for (int i = 0; i < 8; i++) {

                // 隣接するセルの位置を取得
                int nrow = rows + dy[i];
                int ncol = cols + dx[i];
 
                // 配列の範囲外である場合はなにもしない
                if (nrow < 0 || ncol < 0 || nrow >= _ARRAY_HEIGHT || ncol >= _ARRAY_WIDTH)
                    continue;
 
                // 地雷があるセルは無視
                if (_MAP[nrow, ncol] == -1)
                    continue;
 
                // すでに開かれているセルは無視
                if (_IS_MAP_OPEN[nrow, ncol])
                    continue;
 
                // 旗を立てているセルは無視
                if (GetFlagState(nrow, ncol) == 1)
                    continue;
                
                _IS_MAP_OPEN[nrow, ncol] = true;
                if (_MAP[nrow, ncol] == 0)
                    OpenAroundCell(nrow, ncol);
            }
        }

        /// <summary>
        /// ゲームを中断する。
        /// </summary>
        public void Stop() {

            _GAME_STOP = true;
        }

        /// <summary>
        /// ゲームが中断されているかを問い合わせる。
        /// </summary>
        /// <returns></returns>
        public bool Stopped() {

            return _GAME_STOP;
        }

        /// <summary>
        /// 現在のスコア更新する。
        /// ゲームを開始してからの経過秒になる、
        /// </summary>
        public void SetScore() {

            _DISP_SCORE = (_DISP_SCORE < 999) ? ++_DISP_SCORE : 999;
        }

        /// <summary>
        /// 現在のスコアを問い合わせる。
        /// </summary>
        public int Score() {

            return _DISP_SCORE;
        }

        /// <summary>
        /// セル数の幅数を返却する。
        /// </summary>
        /// <returns>セル数をSize型で返却する。</returns>
        public Size CellSize() {

            return new Size(_ARRAY_WIDTH, _ARRAY_HEIGHT);
        }

        /// <summary>
        /// 指定した位置のマップ状態を取得し返却する。
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns>0:なにもなし、1～8:まわりの地雷数、-1:地雷</returns>
        public int Map(int rows, int cols) {

            return _MAP[rows, cols];
        }

        /// <summary>
        /// 指定した位置のマップの開閉状態を返却する。
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns>true:セルは開かれている、false:セルは閉じている。</returns>
        public bool IsMapOpened(int rows, int cols) {

            return _IS_MAP_OPEN[rows, cols];
        }

        /// <summary>
        /// 指定した位置のマップを開く。
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public void MapOpen(int rows, int cols) {

            _IS_MAP_OPEN[rows, cols] = true;
        }

        /// <summary>
        /// 指定した位置のフラグの状態を取得する。
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns>0:旗を立てている、1:疑問符を立てている。</returns>
        public int GetFlagState(int rows, int cols) {

            foreach (PositionState flag in _FLAGSTATE)
                if (flag.Row == rows && flag.Column == cols)
                    return flag.State;
            return 0;
        }

        /// <summary>
        /// 指定した位置にフラグを設定する。
        /// フラグなし→フラグ→疑問符フラグ→フラグなしで回帰する。
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public void SetFlagState(int rows, int cols) {

            foreach (PositionState flag in _FLAGSTATE)
                if (flag.Row == rows && flag.Column == cols) {
                    flag.State++;
                    if (flag.State == 2)
                        _DISP_MINE++;
                    if (flag.State > 2)
                        _FLAGSTATE.Remove(flag);
                    return;
                }
            _FLAGSTATE.Add(new PositionState(cols, rows, 1));
            _DISP_MINE--;
        }

        /// <summary>
        /// 未開のセル数を求めて返却する。
        /// </summary>
        /// <returns>未開のセル数を返却する。ただし、地雷は除く。</returns>
        public int GetUnopenedCellCount() {

            int openCell = 0;
            for (int row = 0; row < _ARRAY_HEIGHT; row++)
                for (int col = 0; col < _ARRAY_WIDTH; col++)
                    if (_MAP[row, col] != -1)
                        if (!_IS_MAP_OPEN[row, col])
                            openCell++;
            return openCell;
        }

        /// <summary>
        /// 地雷の残数を返却する。
        /// </summary>
        /// <returns>地雷の残数を返却する。</returns>
        public int GetMinerCount() {

            return _DISP_MINE;
        }





    }



    public class Position {

        public Position(int col, int row) {

            Column = col;
            Row = row;
        }
        public int Column { get; set; }
        public int Row { get; set; }
    }
    
    public class PositionState {

        public PositionState(int col, int row, int state) {

            Column = col;
            Row = row;
            State = state;
        }
        public int Column { get; set; }
        public int Row { get; set; }
        public int State { get; set; }
    }
    
}