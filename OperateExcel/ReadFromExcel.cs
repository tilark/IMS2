using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
namespace OperateExcel
{
    public class ReadFromExcel
    {
        /// <summary>
        /// 获取文件第一个工作表的行总数.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>System.UInt32.</returns>
        public int GetRowCount(string fileName)
        {
            int rowCount = 0;
            try
            {
                using (SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Open(fileName, false))
                {
                    WorkbookPart workbookPart = spreadsheetDoc.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                    rowCount = sheetData.Elements<Row>().Count();
                }
            }
            catch (Exception ex)
            {
                string errorMess = ex.Message;
            }
            return rowCount;

        }
        /// <summary>
        /// 读取指定Excel文件名的第一个工作簿，根据指定的Row读取该行所有数据，并返回列表.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> ReadRowFromExcel(uint rowIndex, string fileName)
        {
            List<string> ListData = new List<string>();
            try
            {
                using (SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Open(fileName, false))
                {
                    WorkbookPart workbookPart = spreadsheetDoc.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                    string text;
                    Row row = sheetData.Elements<Row>().Where(r => r.RowIndex.Value == rowIndex).FirstOrDefault();
                    if (row == null)
                    {
                        return ListData;
                    }
                    foreach (Cell cell in row.Elements<Cell>())
                    {
                        //根据DataType读取数据
                        text = GetCellValue(workbookPart, cell);
                        ListData.Add(text);
                    }
                }
            }
            catch (Exception ex)
            {

                //如果文件不存在，会引发该异常，不处理，直接返回NULL值
                string errorMess = ex.Message;

            }
            return ListData;
        }
        /// <summary>
        /// 按行读取第一个工作簿中的所有数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        public List<string> ReadExcelFileDOM(string fileName)
        {
            List<string> ListData = new List<string>();
            try
            {
                using (SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Open(fileName, false))
                {
                    WorkbookPart workbookPart = spreadsheetDoc.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                    string text;
                    foreach (Row row in sheetData.Elements<Row>())
                    {
                        foreach (Cell cell in row.Elements<Cell>())
                        {
                            //根据DataType读取数据
                            //text = cell.CellValue.Text;
                            text = GetCellValue(workbookPart, cell);
                            ListData.Add(text);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //如果文件不存在，会引发该异常，不处理，直接返回NULL值
                string errorMess = ex.Message;

            }
            return ListData;

        }
        /// <summary>
        /// 根据指定表格范围从Excel中读取数据，写入到List<string>中
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="firstCellName">First name of the cell.</param>
        /// <param name="lastCellName">Last name of the cell.</param>
        /// <returns>数据列表;.</returns>
        public List<string> ReadCellRangeFromExcel(string fileName, string firstCellName, string lastCellName)
        {
            List<string> ListData = new List<string>();
            try
            {
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fileName, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                    // Get the row number and column name for the first and last cells in the range.
                    uint firstrowIndex = GetRowIndex(firstCellName);
                    uint lastrowIndex = GetRowIndex(lastCellName);
                    string firstColumn = GetColumnName(firstCellName);
                    string lastColumn = GetColumnName(lastCellName);
                    string text = null;
                    //加.Skip<Row>(1)可过滤第一行标题。
                    foreach (Row row in worksheetPart.Worksheet.Descendants<Row>().Where(r => r.RowIndex.Value >= firstrowIndex && r.RowIndex.Value <= lastrowIndex))
                    {
                        foreach (Cell cell in row)
                        {
                            //某些Cell中并没有数据
                            string columnName = GetColumnName(cell.CellReference.Value);
                            if (CompareColumn(columnName, firstColumn) >= 0 && CompareColumn(columnName, lastColumn) <= 0)
                            {
                                //在选定的范围内，可以读取数据
                                text = GetCellValue(workbookPart, cell);
                                ListData.Add(text);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //如果文件不存在，会引发该异常，不处理，直接返回NULL值
                string errorMess = ex.Message;


            }
            return ListData;


        }

        /// <summary>
        /// 获取单元格数据
        /// </summary>
        /// <param name="workbookPart"></param>
        /// <param name="cell">单元格名</param>
        /// <returns></returns>
        private string GetCellValue(WorkbookPart workbookPart, Cell cell)
        {
            string value = null;
            if (cell != null && cell.HasChildren)
            {
                //value = cell.InnerText;
                value = cell.CellValue.Text;

                if (cell.DataType != null)
                {
                    switch (cell.DataType.Value)
                    {
                        case CellValues.SharedString:
                            var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>()
                                .FirstOrDefault();
                            if (stringTable != null)
                            {
                                value = stringTable.SharedStringTable
                                    .ElementAt(int.Parse(value)).InnerText;
                            }
                            break;
                        case CellValues.Boolean:
                            switch (value)
                            {
                                case "0":
                                    value = "FALSE";
                                    break;
                                default:
                                    value = "TRUE";
                                    break;
                            }
                            break;
                    }
                }
            }
            return value;
        }

        // Given a cell name, parses the specified cell to get the row index.
        public uint GetRowIndex(string cellName)
        {
            // Create a regular expression to match the row index portion the cell name.
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(cellName);

            return uint.Parse(match.Value);
        }
        // Given a cell name, parses the specified cell to get the column name.
        public string GetColumnName(string cellName)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellName);

            return match.Value;
        }
        // Given two columns, compares the columns.
        public int CompareColumn(string column1, string column2)
        {
            if (column1.Length > column2.Length)
            {
                return 1;
            }
            else if (column1.Length < column2.Length)
            {
                return -1;
            }
            else
            {
                return string.Compare(column1, column2, true);
            }
        }

    }
}
