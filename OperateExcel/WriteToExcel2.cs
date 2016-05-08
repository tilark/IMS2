using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace OperateExcel
{
    class WriteToExcel2
    {
        /// <summary>
        /// 将值写入到Excel的指定的单元格中.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="text">The text.</param>
        public void WriteCellValueToExcel<T>(string fileName, uint rowIndex, string columnName, T text)
        {

        }
        /// <summary>
        /// 将项目值写入Excel中，分析表结构，Key为列名，Value为值，其中第一列特殊，是科室名称，为string类型
        /// 其余项为Decimal类型.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="itemValueDic">The item value dictionary.</param>
        public void WriteItemValueToExcel(string fileName, List<List<string>> itemValueListList, List<string>headerColumn)
        {

        }
        /// <summary>
        /// 将数值和字符串类型按行方式写入到Excel中.如果整个列表项是string，则数值也会当作字符串处理。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="ListText">The list text.</param>
        public void WriteToExcelColumnWithString<T>(string fileName, uint rowIndex, List<T> ListText)
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(fileName, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                WorksheetPart worksheetPart = CreateWorksheetPart(spreadsheetDocument);
                Worksheet worksheet = worksheetPart.Worksheet;
                SheetData sheetData = worksheet.GetFirstChild<SheetData>();
                var shareStringPart = spreadsheetDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();
                if (worksheetPart == null || ListText == null || shareStringPart == null)
                {
                    return;
                }
                Row row;
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
                char[] columnIndex = new char[] { ' ', ' ', 'A' };
                string cellReference = null;
                foreach (var Text in ListText)
                {

                    //将数据写入该列名中
                    //判断该数据是不是Text，如果是数值，可直接加入，但在用户信息处，都是Text
                    string columnName = String.Empty;
                    for (int i = 0; i < 3; i++)
                    {
                        columnName += columnIndex[i].ToString();
                    }
                    //已经是行数据了，可将值插入到Row中
                    //cellReference = columnName + columnIndex.ToString() + rowIndex;
                    cellReference = columnName + rowIndex;
                    Cell newCell = new Cell() { CellReference = cellReference };
                    row.AppendChild(newCell);
                    worksheet.Save();
                    //需对传入的Text进行判断，如果是数值型的直接填入，如果是字符串，再填入到SharedStringItem
                    var value = Text.ToString(); ;
                    if (String.Compare(Text.GetType().Name, "String", true) == 0)
                    {
                        var index = InsertSharedStringItem(value.ToString(), shareStringPart);
                        value = index.ToString();
                        newCell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                    }
                    newCell.CellValue = new CellValue(value.ToString());
                    worksheetPart.Worksheet.Save();
                    columnIndex[2]++;
                    if (columnIndex[2] > 'Z')
                    {
                        //A 之后为AA ,AZ然后是BA,ZZ后面是AAA
                        columnIndex[2] = 'A';
                        columnIndex[1] = (columnIndex[1] == ' ') ? 'A' : columnIndex[1]++;
                        if (columnIndex[1] > 'Z')
                        {
                            columnIndex[1] = 'A';

                            columnIndex[0] = (columnIndex[0] == ' ') ? 'A' : columnIndex[0]++;

                            if (columnIndex[0] > 'Z')
                            {
                                //如果出现这种情况，说明文本过大，从头开始写
                                columnIndex[0] = ' ';
                                columnIndex[1] = ' ';
                                columnIndex[2] = 'A';
                            }
                        }
                    }
                }
                rowIndex++;
            }
        }
        private WorksheetPart CreateWorksheetPart(SpreadsheetDocument spreadsheetDocument)
        {
            //WorksheetPart worksheetPart = null;
            #region ini spreadsheetDocument
            WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.
                GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Report"
            };
            sheets.Append(sheet);

            workbookPart.Workbook.Save();
            #endregion
            return worksheetPart;
        }

        private int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }
                i++;
            }
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

    }
}
