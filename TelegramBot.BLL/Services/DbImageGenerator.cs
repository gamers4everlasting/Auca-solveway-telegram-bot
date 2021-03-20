using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TelegramBot.BLL.Services
{
    public static class DbImageGenerator
    {

        public static MemoryStream GenerateStream(string text)
        {
            //generate emSize by the structure date lenght.
            var font = new Font("Arial", 20);


            //first, create a dummy bitmap just to get a graphics object
            SizeF textSize;
            using (Image img = new Bitmap(1, 1))
            {
                using (Graphics drawing = Graphics.FromImage(img))
                {
                    //measure the string to see how big the image needs to be
                    textSize = drawing.MeasureString(text, font);
                    //if (!minSize.IsEmpty)
                    //{
                    textSize.Width = textSize.Width; //> minSize.Width ? textSize.Width : minSize.Width;
                        textSize.Height = textSize.Height; //> minSize.Height ? textSize.Height : minSize.Height;
                        //}
                }
            }

            //create a new image of the right size
            Image retImg = new Bitmap((int)textSize.Width, (int)textSize.Height);
            using (var drawing = Graphics.FromImage(retImg))
            {
                //paint the background
                drawing.Clear(Color.White);

                //create a brush for the text
                using (Brush textBrush = new SolidBrush(Color.Black))
                {
                    drawing.DrawString(text, font, textBrush, 0, 0);
                    drawing.Save();
                }
            }
            
            var ms = new MemoryStream();
            retImg.Save("C:\\Users\\OlenPC\\Desktop\\Auca\\TelegramBot\\test.png", ImageFormat.Png);
            retImg.Save(ms, ImageFormat.Png);
            return ms;
        }
    }
}
