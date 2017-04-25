using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using System.IO;
using System.Data;
using System.ComponentModel;

namespace ExcelEntityOperation
{
    public class WriteToExcel : IWriteToExcel
    {

        #region Excel2007版本类型
        public string ExcelContentType
        {
            get
            {
                return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
        }

        #endregion
        #region 将数据写入到指定Excel模版中
        /// <summary>
        /// 对应的模版为第一行为大标题，第1列为各关键列，第二行为关键列对应的各属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceData"><标题,T>key为Excel模版中的keyCell对应列的内容，如A1列中为"部门1，部门2，部门3",<部门1, T>，可将部门1等内容写在属性的Display(Name="部门1")上</param>
        /// <param name="templateFilePath">Excel模版路径</param>
        /// <param name="Message">输出错误信息</param>
        /// <param name="dataRangeStartCell">数据区域开始的单元格，如第1行为标题，A列为关键字，数据区域从B2开始写</param>
        /// <param name="keyCell">如A1列中为"部门1，部门2，部门3"，标题为“姓名、年龄、生日”等</param>
        /// <param name="changeTitleCell">需更改的标题所在单元格</param>
        /// <param name="changeContent">将标题所在单元格替换为该内容</param>
        /// <param name="columnData">Key为对应单元格，Value为T的属性名，将属性值写入到对应单元格内，如//<C5,Name> <D5, Age> Name与Age均为T中的属性，表示将Name中的值写入到C5</param>
        /// <param name="sheetName">工作表名称，默认为第一个</param>
        /// <returns>写入成功，返回True，否则返回false</returns>
        public bool ExportEntityToExcelFile<T>(Dictionary<string, T> sourceData, string templateFilePath, out StringBuilder Message, string dataRangeStartCell, string keyCell, string changeTitleCell, string changeContent, Dictionary<string, string> columnData, string sheetName) where T : class
        {
            bool result = false;
            var message = new StringBuilder(100);
            try
            {
                FileInfo existingFile = new FileInfo(templateFilePath);
                using (ExcelPackage package = new ExcelPackage(existingFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    if (!String.IsNullOrEmpty(sheetName))
                    {
                        //如果没有获取该名称的sheet，获取第一个
                        worksheet = package.Workbook.Worksheets[sheetName];
                    }
                    if (worksheet.Dimension == null)
                    {
                        message.Append("EmptyError:File is Empty");
                        Message = message;
                        return result;
                    }
                    else
                    {
                        var keyRowStart = GetRowIndex(keyCell);
                        var keyColumn = GetColumnName(keyCell)[0] - 'A' + 1;
                        var dataRowStart = GetRowIndex(dataRangeStartCell);
                        var dataColumnStart = GetColumnName(dataRangeStartCell);
                        //需获得keyColumn中的Rows行
                        var keyHeader = new Dictionary<int, string>();
                        int colEnd = worksheet.Dimension.End.Column;
                        int rowEnd = worksheet.Dimension.End.Row;
                        for (int i = (int)keyRowStart; i <= rowEnd; i++)
                        {
                            if (worksheet.Cells[i, keyColumn].Value != null)
                            {
                                keyHeader[i] = worksheet.Cells[i, keyColumn].Value.ToString();

                            }
                        }
                        //
                        //更改标题栏的年与月
                        worksheet.Cells[changeTitleCell].Value = changeContent;

                        //将sourceData转换成Dictionary<row, T>形式
                        var destData = ConvertDataSourceToRowDataDictionary<T>(sourceData, keyHeader);
                        //制作原值-固定表和制作原值-无形表
                        for (int dataRow = (int)dataRowStart; dataRow <= rowEnd; dataRow++)
                        {
                            if (worksheet.Cells[dataRow, keyColumn].Value == null)
                            {
                                break;
                            }
                            //
                            /// <param name="columnData">Key为对应单元格，Value为T的属性名，将属性值写入到对应单元格内，如//<C5,Name> <D5, Age> Name与Age均为T中的属性，表示将Name中的值写入到C5</param>
                            foreach (var columnName in columnData)
                            {
                                var cellName = columnName.Key + dataRow.ToString();
                                var propertiInfo = typeof(T).GetProperty(columnName.Value);
                                var pointData = propertiInfo.GetValue(destData[dataRow]);
                                worksheet.Cells[cellName].Value = pointData;
                            }

                        }
                    }
                    package.Save();
                    result = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            Message = message;
            return result;
        }


        #endregion

        #region 将数据写入到byte[]中
        public byte[] ExportListToExcel<T>(List<T> data, List<string> heading, bool isShowSlNo = false, params string[] ColumnsToTake)
        {
            return ExportExcel(ListToDataTable<T>(data), heading, isShowSlNo, ColumnsToTake);
        }
        /// <summary>
        /// 导出Excel.
        /// </summary>
        /// <param name="dataTable">数据源.</param>
        /// <param name="heading">第一行为工作簿Worksheet名称及标题（必填），第二行为其他备注消息（可选）.</param>
        /// <param name="showSrNo">是否显示行编号</c> [show sr no].</param>
        /// <param name="columnsToTake">要导出的列.</param>
        /// <returns>System.Byte[].</returns>
        public byte[] ExportExcel(DataTable dataTable, List<string> heading, bool showSrNo = false, params string[] columnsToTake)
        {
            byte[] results = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                var worksheetName = heading.Count > 0 ? heading[0] : String.Empty;
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(string.Format("{0} Data", worksheetName));

                //从哪一行开始填数据
                int startRowFrom = heading.Count + 1;

                //是否显示行编号
                if (showSrNo)
                {
                    DataColumn dataColumn = dataTable.Columns.Add("#", typeof(int));
                    dataColumn.SetOrdinal(0);
                    int index = 1;
                    foreach (DataRow item in dataTable.Rows)
                    {
                        item[0] = index;
                        index++;
                    }
                }

                //Add Content Into the Excel File
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);
                int columnIndex = 1;
                foreach (DataColumn item in dataTable.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    int maxLength = columnCells.Max(cell => cell.Value != null ? cell.Value.ToString().Count() : 140);

                    if (maxLength < 150)
                    {
                        workSheet.Column(columnIndex).AutoFit();
                    }

                    columnIndex++;
                }



                //removed ignored columns
                if (columnsToTake.Count() > 0)
                {
                    for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                    {
                        if (i == 0 && showSrNo)
                        {
                            continue;
                        }

                        if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                        {
                            workSheet.DeleteColumn(i + 1);
                        }
                    }
                }


                //创建表标题
                if (heading.Count > 0)
                {
                    var titleColumn = "A";

                    for (int titleRow = 0; titleRow < heading.Count; titleRow++)
                    {
                        var titleCell = titleColumn + (titleRow + 1).ToString();
                        workSheet.Cells[titleCell].Value = heading[titleRow];

                    }
                    workSheet.Cells["A1"].Style.Font.Size = 20;
                    //此处需将标题能够跨行排列最好

                    //新插入一行一列
                    workSheet.InsertColumn(1, 1);
                    workSheet.InsertRow(1, 1);
                    workSheet.Column(1).Width = 2;
                    workSheet.Row(1).Height = 10;
                }

                results = package.GetAsByteArray();
            }

            return results;
        }

        /// <summary>
        /// Lists 转成 data table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns>DataTable.</returns>
        /// <remarks></remarks>
        public DataTable ListToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            DataTable dataTable = new DataTable();
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];

                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceData"></param>
        /// <param name="keyHeader"></param>
        /// <returns></returns>
        private Dictionary<int, T> ConvertDataSourceToRowDataDictionary<T>(Dictionary<string, T> sourceData, Dictionary<int, string> keyHeader) where T : class
        {
            var result = new Dictionary<int, T>();
            foreach (var temp in keyHeader)
            {
                try
                {
                    if (sourceData.Keys.Contains(temp.Value))
                    {
                        var colNumber = sourceData[temp.Value];
                        result.Add(temp.Key, colNumber);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return result;
        }

        internal uint GetRowIndex(string cellName)
        {
            // Create a regular expression to match the row index portion the cell name.
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(cellName);

            return uint.Parse(match.Value);
        }
        // Given a cell name, parses the specified cell to get the column name.
        internal string GetColumnName(string cellName)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellName);

            return match.Value;
        }
    }
}
