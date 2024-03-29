﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using TelegramBot.Common.Helper;

namespace TelegramBot.BLL.Services
{
    public static class DbImageGenerator
    {
        public static void GenerateImageFromSolutionQuery(DbValidationResult validationData)
        {
            var textData = new StringBuilder();
            var correctTable = validationData.SolutionResult;
            var userTable = validationData.UserResult.First();

            
            var correctRows = correctTable.Rows;
            var userRows = userTable.Rows;

            var maxUserDataLength = GetMaxRowsDataLength(userRows); //find max length of column data.
            var maxCorrectDataLength = GetMaxRowsDataLength(correctRows); //find max length of column data.
            const int spaceBetweenColumns = 5;

            textData.AppendLine();
            textData.AppendLine("Result");
            textData.AppendLine();
            for (var i = 0; i < userRows.Length; i++)
            {
                var rowData = new StringBuilder();
                var userRowCells = userRows[i].Cells;
                for (var cell = 0; cell < userRowCells.Length; cell++)
                {
                    var spacesToAdd = maxUserDataLength[cell] - userRowCells[cell].Value.Length + spaceBetweenColumns;
                    rowData.Append("  " + userRowCells[cell].Value + GenerateWhiteSpaces(spacesToAdd));
                }

                textData.AppendLine(rowData.ToString());
            }

            textData.AppendLine();
            textData.AppendLine();
            textData.AppendLine();
            textData.AppendLine("Correct Result");
            textData.AppendLine();
            for (var j = 0; j < correctRows.Length; j++)
            {
                var rowData = new StringBuilder();
                var correctRowCells = correctRows[j].Cells;
                for (var cell = 0; cell < correctRowCells.Length; cell++)
                {
                    var spacesToAdd = maxCorrectDataLength[cell] - correctRowCells[cell].Value.Length + spaceBetweenColumns;
                    rowData.Append("  " +correctRowCells[cell].Value + GenerateWhiteSpaces(spacesToAdd));
                }

                textData.AppendLine(rowData.ToString());
            }
            
            

            var image = GenerateImageFromText(textData.ToString());
            //var img2 = GenerateImageFromText2(text.ToString());
            image.Save("C:\\Users\\OlenPC\\Desktop\\Auca\\TelegramBot\\test.png", ImageFormat.Png);
            //img2.Save("C:\\Users\\OlenPC\\Desktop\\Auca\\TelegramBot\\test2.png", ImageFormat.Png);

        }

        public static void GenerateStream(DbValidationResult validationData)
        {
            var text = new StringBuilder();
            var correctTable = validationData.SolutionResult;
            var userTable = validationData.UserResult.First();

            var i = 0;
            var j = 0;
            var correctRows = correctTable.Rows;
            var userRows = userTable.Rows;

            var maxUserDataLength = GetMaxRowsDataLength(userRows); //find max length of column data.
            var maxCorrectDataLength = GetMaxRowsDataLength(correctRows); //find max length of column data.
            const int spaceBetweenTables = 20;
            const int spaceBetweenColumns = 5;
            while (i < userRows.Length || j < correctRows.Length)
            {
                var rowData = new StringBuilder();
                if (i < userRows.Length)
                {
                    var userRowCells = userRows[i].Cells;
                    for (var cell = 0; cell < userRowCells.Length; cell++)
                    {
                        var spacesToAdd = maxUserDataLength[cell] - userRowCells[cell].Value.Length + spaceBetweenColumns;
                        rowData.Append(userRowCells[cell].Value + GenerateWhiteSpaces(spacesToAdd));
                    }

                    i++;
                }
                
                if (j < correctRows.Length)
                {
                    if (rowData.Length == 0)
                    {
                        var spacesToAdd = maxUserDataLength.Sum(dict => dict.Value + spaceBetweenColumns);
                        rowData.Append(GenerateWhiteSpaces(spacesToAdd));
                    }

                    rowData.Append(GenerateWhiteSpaces(spaceBetweenTables)); //отступы до второй таблицы
                    var correctRowCells = correctRows[j].Cells;
                    for (var cell = 0; cell < correctRowCells.Length; cell++)
                    {
                        var spacesToAdd = maxCorrectDataLength[cell] - correctRowCells[cell].Value.Length + spaceBetweenColumns;
                        rowData.Append(correctRowCells[cell].Value + GenerateWhiteSpaces(spacesToAdd));
                    }

                    j++;
                }

                text.AppendLine(rowData.ToString());
            }

            var image = GenerateImageFromText(text.ToString());
            //var img2 = GenerateImageFromText2(text.ToString());
            image.Save("C:\\Users\\OlenPC\\Desktop\\Auca\\TelegramBot\\test.png", ImageFormat.Png);
            //img2.Save("C:\\Users\\OlenPC\\Desktop\\Auca\\TelegramBot\\test2.png", ImageFormat.Png);

        }

        private static Bitmap GenerateImageFromText2(string imageText)
        {
            Bitmap objBmpImage = new Bitmap(1, 1);
            int intWidth = 0;
            int intHeight = 0;

            // Create the Font object for the image text drawing.
            Font objFont = new Font("Arial", 20, FontStyle.Regular, GraphicsUnit.Pixel);

            // Create a graphics object to measure the text's width and height.
            Graphics objGraphics = Graphics.FromImage(objBmpImage);

            // This is where the bitmap size is determined.
            intWidth = (int)objGraphics.MeasureString(imageText, objFont).Width;
            intHeight = (int)objGraphics.MeasureString(imageText, objFont).Height;

            // Create the bmpImage again with the correct size for the text and font.
            objBmpImage = new Bitmap(objBmpImage, new Size(intWidth, intHeight));

            // Add the colors to the new bitmap.
            objGraphics = Graphics.FromImage(objBmpImage);

            // Set Background color
            objGraphics.Clear(Color.White);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            objGraphics.DrawString(imageText, objFont, new SolidBrush(Color.FromArgb(102, 102, 102)), 0, 0);
            objGraphics.Flush();
             
            return objBmpImage;

        }

        private static Image GenerateImageFromText(string imageText)
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
                    textSize = drawing.MeasureString(imageText, font);
                }
            }

            //create a new image of the right size
            Image retImg = new Bitmap((int)textSize.Width + 100, (int)textSize.Height + 50);
            using (var drawing = Graphics.FromImage(retImg))
            {
                //paint the background
                drawing.Clear(Color.White);

                //create a brush for the text
                using (Brush textBrush = new SolidBrush(Color.Black))
                {
                    drawing.DrawString(imageText, font, textBrush, 0, 0);
                    drawing.Save();
                }
            }
            return retImg;
        }

        private static Dictionary<int, int> GetMaxRowsDataLength(ResultRow[] userRows)
        {
            var dict = new Dictionary<int, int>();
            var columnCount = userRows.First().Cells.Length;
            for (var i = 0; i < columnCount; i++)
            {
                dict.Add(i, 0);
            }
            
            for (var i = 0; i < userRows.Length; i++)
            {
                for (var j = 0; j < columnCount; j++)
                {
                    var valueLength = userRows[i].Cells[j].Value.Length;
                    if (dict[j] < valueLength)
                    {
                        dict[j] = valueLength;
                    }
                }
            }

            return dict;
        }

        private static string GenerateWhiteSpaces(int spacesToAdd)
        {
            var spaces = "";
            for (var i = 0; i < spacesToAdd; i++)
            {
                spaces += " ";
            }

            return spaces;
        }

        private static int GetRowLength(ResultRow[] correctRows)
        {
            var maxL = 0;
            for (var i = 0; i < correctRows.Length; i++)
            {
                for(var j = 0; j < correctRows[i].Cells.Length; j++)
                {
                    var lenght = correctRows[i].Cells[j].Value.Length;
                    if (lenght > maxL) maxL = lenght;

                }
            }

            return maxL;
        }
    }
}
