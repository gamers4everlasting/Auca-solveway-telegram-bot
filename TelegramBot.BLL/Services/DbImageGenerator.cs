using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using TelegramBot.Common.Helper;

namespace TelegramBot.BLL.Services
{
    public static class DbImageGenerator
    {
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
            const int spaceBetweenColumn = 5;
            while (i < userRows.Length || j < correctRows.Length)
            {
                var rowData = new StringBuilder();
                if (i < userRows.Length)
                {
                    var userRowCells = userRows[i].Cells;
                    for (var cell = 0; cell < userRowCells.Length; cell++)
                    {
                        var spacesToAdd = maxUserDataLength[cell] - userRowCells[cell].Value.Length + spaceBetweenColumn;
                        rowData.Append(userRowCells[cell].Value + GenerateWhiteSpaces(spacesToAdd));
                    }

                    i++;
                }
                
                if (j < correctRows.Length)
                {
                    if (i == userRows.Length)
                    {
                        var spacesToAdd = maxUserDataLength.Sum(dict => dict.Value + spaceBetweenColumn);
                        rowData.Append(GenerateWhiteSpaces(spacesToAdd));
                    }

                    rowData.Append(GenerateWhiteSpaces(4)); //отступы до второй таблицы
                    var correctRowCells = correctRows[j].Cells;
                    for (var cell = 0; cell < correctRowCells.Length; cell++)
                    {
                        var spacesToAdd = maxCorrectDataLength[cell] - correctRowCells[cell].Value.Length + spaceBetweenColumn;
                        rowData.Append("| " + correctRowCells[cell].Value + GenerateWhiteSpaces(spacesToAdd));
                    }

                    j++;
                }

                text.AppendLine(rowData.ToString());
            }


            //generate emSize by the structure date lenght.
            var font = new Font("Arial", 20);


            //first, create a dummy bitmap just to get a graphics object
            SizeF textSize;
            using (Image img = new Bitmap(1, 1))
            {
                using (Graphics drawing = Graphics.FromImage(img))
                {
                    //measure the string to see how big the image needs to be
                    textSize = drawing.MeasureString(text.ToString(), font);
                    //if (!minSize.IsEmpty)
                    //{
                    textSize.Width = textSize.Width; //> minSize.Width ? textSize.Width : minSize.Width;
                        textSize.Height = textSize.Height; //> minSize.Height ? textSize.Height : minSize.Height;
                        //}
                }
            }

            //create a new image of the right size
            Image retImg = new Bitmap((int)textSize.Width + 200, (int)textSize.Height + 200);
            using (var drawing = Graphics.FromImage(retImg))
            {
                //paint the background
                drawing.Clear(Color.White);

                //create a brush for the text
                using (Brush textBrush = new SolidBrush(Color.Black))
                {
                    drawing.DrawString(text.ToString(), font, textBrush, 0, 0);
                    drawing.Save();
                }
            }
            
            //var ms = new MemoryStream();
            retImg.Save("C:\\Users\\OlenPC\\Desktop\\Auca\\TelegramBot\\test1.png", ImageFormat.Png);
            //retImg.Save(ms, ImageFormat.Png);
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
