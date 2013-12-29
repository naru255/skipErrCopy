using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace skipErrCopy2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);
            foreach (string arg in args)
            {
                Console.WriteLine(arg);
                copy(arg);
            }
            Console.WriteLine("fin");
            string line = Console.ReadLine();
        }

        static void copy(string rf)
        {
            string wf;
            int fileCount = 0;//ファイル名添字
            Int64 seekPoint = 0;//既に読み込んだバイト数
            const int BUFSIZE = 1024 * 1024; // 1度に処理するサイズ
            const int SKIPSIZE = 1024 * 1024;//エラー時に読み飛ばすサイズ
            byte[] buf = new byte[BUFSIZE]; // 読み込み用バッファ

            int readSize; // Readメソッドで読み込んだバイト数

            System.IO.FileInfo fi = new System.IO.FileInfo(rf);
            Int64 rf_length = fi.Length;
            Console.WriteLine( rf_length.ToString() + "Byte" );

            while (true)
            {
                if (rf_length <= seekPoint)
                {
                    break; // コピー完了
                }
                wf =System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ),
                        System.IO.Path.GetFileNameWithoutExtension(rf) + fileCount.ToString()
                            + System.IO.Path.GetExtension(rf)) ;
                using (FileStream rfs = new FileStream(rf, FileMode.Open, FileAccess.Read))
                using (FileStream wfs = new FileStream(wf, FileMode.Create, FileAccess.Write))
                {
                    rfs.Seek(seekPoint, SeekOrigin.Begin);

                    while (true)
                    {
                        try
                        {
                            readSize = rfs.Read(buf, 0, BUFSIZE); // 読み込み
                        }
                        catch(IOException e)
                        {
                            // 読み込みに失敗した場合
                            Console.WriteLine("error at " + rfs.Position + "Byte");
                            Console.WriteLine(e.ToString());
                            //rfs.Positionでどこまで読めたか取得できる？？
                            seekPoint += SKIPSIZE;
                            fileCount++;
                            break;
                        }//catch
                        seekPoint += readSize;
                        if (readSize == 0)
                        {
                            break; // コピー完了
                        }
                        wfs.Write(buf, 0, readSize); // 書き込み
                    }
                }//using
            }//while
        }//void
    }
}
