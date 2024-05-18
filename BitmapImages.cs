using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace MinesweeperGame {

    public class BitmapImages {

        // インスタンス
        private static BitmapImages singleton = null;

        // 各種ビットマップ情報
        public Bitmap[] DIGIT = new Bitmap[11];
        public Bitmap[] SMILE = new Bitmap[4];
        public Bitmap[] MINE = new Bitmap[3];
        public Bitmap[] NUMBER = new Bitmap[8];
        public Bitmap[] FLAG = new Bitmap[2];
        public Bitmap[] CELL = new Bitmap[2];
        public Bitmap[] CHECK = new Bitmap[2];

        private string[] bitmap_files = {
                "digit0.png", 
                "digit1.png",
                "digit2.png",
                "digit3.png",
                "digit4.png",
                "digit5.png",
                "digit6.png",
                "digit7.png",
                "digit8.png",
                "digit9.png",
                "digit-.png",
                "smile.png",
                "ohh.png",
                "win.png",
                "dead.png",
                "mine-ceil.png",
                "mine-death.png",
                "misflagged.png",
                "open1.png",
                "open2.png",
                "open3.png",
                "open4.png",
                "open5.png",
                "open6.png",
                "open7.png",
                "open8.png",
                "flag.png",
                "question.png",
                "cell-close.png",
                "cell-open.png",
                "checked.png",
                "empty.png"
        };

        /// <summary>
        /// インスタンスを作成
        /// </summary>
        /// <returns>当クラスのインスタンスを返却する。シングルトン構成です。</returns>
        public static BitmapImages GetInstance() {

            if (singleton == null) {
                singleton = new BitmapImages();
            }
            return singleton;
        }

        /// <summary>
        /// BitmapImagesクラスのコンストラクタ。
        /// </summary>
        private BitmapImages() {

            try {
                // カウンター０～９とマイナス記号
                DIGIT[0] = GetBitmapFromAssembly(bitmap_files[0]);
                DIGIT[1] = GetBitmapFromAssembly(bitmap_files[1]);
                DIGIT[2] = GetBitmapFromAssembly(bitmap_files[2]);
                DIGIT[3] = GetBitmapFromAssembly(bitmap_files[3]);
                DIGIT[4] = GetBitmapFromAssembly(bitmap_files[4]);
                DIGIT[5] = GetBitmapFromAssembly(bitmap_files[5]);
                DIGIT[6] = GetBitmapFromAssembly(bitmap_files[6]);
                DIGIT[7] = GetBitmapFromAssembly(bitmap_files[7]);
                DIGIT[8] = GetBitmapFromAssembly(bitmap_files[8]);
                DIGIT[9] = GetBitmapFromAssembly(bitmap_files[9]);
                DIGIT[10] = GetBitmapFromAssembly(bitmap_files[10]);

                // フェイス
                SMILE[0] = GetBitmapFromAssembly(bitmap_files[11]);
                SMILE[1] = GetBitmapFromAssembly(bitmap_files[12]);
                SMILE[2] = GetBitmapFromAssembly(bitmap_files[13]);
                SMILE[3] = GetBitmapFromAssembly(bitmap_files[14]);

                // マインボム
                MINE[0] = GetBitmapFromAssembly(bitmap_files[15]);
                MINE[1] = GetBitmapFromAssembly(bitmap_files[16]);
                MINE[2] = GetBitmapFromAssembly(bitmap_files[17]);

                // ヒントナンバー
                NUMBER[0] = GetBitmapFromAssembly(bitmap_files[18]);
                NUMBER[1] = GetBitmapFromAssembly(bitmap_files[19]);
                NUMBER[2] = GetBitmapFromAssembly(bitmap_files[20]);
                NUMBER[3] = GetBitmapFromAssembly(bitmap_files[21]);
                NUMBER[4] = GetBitmapFromAssembly(bitmap_files[22]);
                NUMBER[5] = GetBitmapFromAssembly(bitmap_files[23]);
                NUMBER[6] = GetBitmapFromAssembly(bitmap_files[24]);
                NUMBER[7] = GetBitmapFromAssembly(bitmap_files[25]);

                FLAG[0] = GetBitmapFromAssembly(bitmap_files[26]);
                FLAG[1] = GetBitmapFromAssembly(bitmap_files[27]);

                CELL[0] = GetBitmapFromAssembly(bitmap_files[28]);
                CELL[1] = GetBitmapFromAssembly(bitmap_files[29]);

                CHECK[0] = GetBitmapFromAssembly(bitmap_files[30]);
                CHECK[1] = GetBitmapFromAssembly(bitmap_files[31]);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                Environment.Exit(0x8020);
            }
        }

        /// <summary>
        /// 指定したビットマップをアセンブリ情報から取得する。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Bitmap形式で返却する。読み込めない場合はアプリケーションを終了する。</returns>
        private Bitmap GetBitmapFromAssembly(string filePath) {

            try {
                Assembly assem = Assembly.GetExecutingAssembly();
                using (Stream reader = assem.GetManifestResourceStream(filePath))
                    return new Bitmap(Image.FromStream(reader));
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                Environment.Exit(0x8020);
            }
            return null;
        }
    }
}
