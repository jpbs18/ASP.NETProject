using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace Exercise1
{  
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Initializing redimension...");

            Thread thread = new (Redimension);
            thread.Start();

            Console.WriteLine("End of task!");
        }

        static void Redimension()
        {
            #region "Directories"
            string entry_directory = "Folders_Entry";
            string redimensioned_directory = "Folders_Redimensioned";
            string finalized_directory = "Folders_Finalized";

            string[] directories = { entry_directory, redimensioned_directory, finalized_directory };

            foreach (string directory in directories)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            #endregion

            FileStream fileStream;
            FileInfo fileInfo;

            while (true)
            {
               var entryFiles = Directory.EnumerateFiles(entry_directory);
               int imageSize = 200;
               
               foreach (var file in entryFiles)
               {
                    fileStream = new (file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fileInfo = new (file);

                    string path = Environment.CurrentDirectory + @"\" + redimensioned_directory 
                        + @"\" + DateTime.Now.Millisecond.ToString() + "_" + fileInfo.Name;

                    ChangeDimension(Image.FromStream(fileStream), imageSize, path);
                    fileStream.Close();

                    string finalPath = Environment.CurrentDirectory + @"\" + finalized_directory + @"\" + fileInfo.Name;
                    fileInfo.MoveTo(finalPath);
               }

               Thread.Sleep(new TimeSpan(0, 0, 3));
            }
        }

        static void ChangeDimension(Image image, int size, string path)
        {
            double ratio = (double) size / image.Height;
            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            Bitmap finalImage = new (newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(finalImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            finalImage.Save(path);
            image.Dispose();
        }
    }
}