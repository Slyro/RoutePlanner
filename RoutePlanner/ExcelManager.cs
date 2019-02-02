using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RoutePlanner
{
    class ExcelManager : IDisposable
    {
        ExcelWorksheet worksheet;
        ExcelPackage excelBook;
        public int MaxRows => worksheet.Dimension.Rows;
        public int MaxColumns => worksheet.Dimension.Columns;
        public bool OpenExcelFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Книга Excel (*.xlsx)|*.xlsx", Title = "Выберите файл..." })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    excelBook = new ExcelPackage(new FileInfo(openFileDialog.FileName));
                    return true;
                }
                //MessageBox.Show("Нужно выбрать файл Excel", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
        public bool FindWorksheet(string sheet_name)
        {
            if (excelBook != null)
            {
                if ((worksheet = excelBook.Workbook.Worksheets[sheet_name]) is null)
                {
                    MessageBox.Show("В книге не найден лист " + sheet_name + "\"", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return true;
            }
            else
            {
                MessageBox.Show("Нужно выбрать файл Excel", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }


        }
        public Dictionary<string, int> GetCouriers(Dictionary<string, int> couriers)
        {
            for (int i = 1; i <= MaxColumns; i++)
            {
                if (worksheet.Cells[1, i].Value != null && worksheet.Cells[2, i].Value != null)
                    couriers.Add(worksheet.Cells[1, i].Value.ToString(), i);
                else
                    continue;
            }
            return couriers;
        }
        public List<string> GetOrders(int courier_column)
        {
            List<string> list = new List<string>();
            for (int i = 2; i <= MaxRows; i++)
            {
                if (worksheet.Cells[i, courier_column].Value == null)
                    continue;
                list.Add(worksheet.Cells[i, courier_column].Text);
            }
            return list;
        }
        public void DestroyObject()
        {
            if (excelBook != null)
                excelBook.Dispose();
        }
        public void Dispose()
        {
            DestroyObject();
        }
    }
}