namespace TelegramBot.BLL.Models.Submissions
{
    public class ResultTableStructure
    {
        public string DbName { get; set; }
        public ResultRow[] Rows { get; set; }

        public ResultTableStructure()
        {
            
        }
        public ResultTableStructure(string name, int columnNumber, int rowsNumber, string[] headers)
        {
            DbName = name;
            Rows = new ResultRow[rowsNumber];
            for (int i = 0; i < rowsNumber; i++)
            {
                Rows[i] = new ResultRow(columnNumber, i == 0, i == 0 ? headers : null);
            }
        }
    }

    public class ResultRow
    {
        public ResultRow()
        {
        }
        public ResultCell[] Cells { get; set; }
        public bool IsHeader { get; set; }

        public ResultRow(int cellsNumber, bool isHeader, string[] headers)
        {
            IsHeader = isHeader;
            Cells = new ResultCell[cellsNumber];
            for (int i = 0; i < cellsNumber; i++)
            {
                Cells[i] = headers != null ? new ResultCell(headers[i]) : new ResultCell();
            }
        }
    }

    public class ResultCell
    {
        public string Value { get; set; }
        public bool IsValid { get; set; }

        public ResultCell()
        {
            IsValid = true;
        }

        public ResultCell(string value)
        {
            Value = value;
            IsValid = true;
        }
    }
}
