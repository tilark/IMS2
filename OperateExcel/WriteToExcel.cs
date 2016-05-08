using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace OperateExcel
{
    public class WriteToExcel
    {
        /// <summary>
        /// 将项目值写入Excel中，分析表结构，Key为列名，Value为值，其中第一列特殊，是科室名称，为string类型
        /// 其余项为Decimal类型.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="itemValueDic">The item value dictionary.</param>
        public void WriteItemValueToExcel(string fileName, List<string> headTitleList, List<string> headerColumn, List<List<string>> itemValueListList)
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(fileName, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                WorksheetPart worksheetPart = CreateWorksheetPart(spreadsheetDocument);
                Worksheet worksheet = worksheetPart.Worksheet;
                SheetData sheetData = worksheet.GetFirstChild<SheetData>();
                var shareStringPart = spreadsheetDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();
                if (worksheetPart == null || headerColumn == null || shareStringPart == null)
                {
                    return;
                }
                //将列标题写入到Excel的第一行中
                uint rowIndex = 1;
                WriteToExcelColumnWithString(worksheetPart, shareStringPart, rowIndex, headTitleList);

                //将列名写入到Excel的第二行中
                rowIndex = 2;
                WriteToExcelColumnWithString(worksheetPart, shareStringPart, rowIndex, headerColumn);
                //再按行编写到itemValueList中，
                //再写入到Excel文件中
                foreach(var itemValueList in itemValueListList)
                {
                    rowIndex++;
                    WriteToExcelColumnWithString(worksheetPart, shareStringPart, rowIndex, itemValueList);
                }
    

            }
        }

        /// <summary>
        /// 将数值和字符串类型按行方式写入到Excel中.如果整个列表项是string，则数值也会当作字符串处理。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="ListText">The list text.</param>
        public void WriteToExcelColumnWithString<T>(WorksheetPart worksheetPart, SharedStringTablePart shareStringPart, uint rowIndex, List<T> ListText)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

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
                var value = Text.ToString();
                Decimal itemValue;
                //对数值进行Decimal判断，如果解析成功，则直接存入，未成功，则为string类型
                if (! Decimal.TryParse(value, out itemValue))
                {
                    var index = InsertSharedStringItem(value, shareStringPart);
                    value = index.ToString();
                    newCell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                    newCell.CellValue = new CellValue(value.ToString());

                }
                else
                {
                    newCell.CellValue = new CellValue(itemValue.ToString());

                }
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
