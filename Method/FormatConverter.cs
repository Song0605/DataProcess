using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace DataProcess.Method
{
    public static class FormatConverter
    {
        public static string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public static void ExcelToPdf()
        {
            var oriPath = Path.Combine(DesktopPath, "导入线路模板.xlsx");
            var pdfFile = oriPath.Replace(".xlsx", ".pdf");

            var app = new Excel.Application();
            var book = app.Workbooks.Open(oriPath, 2, true, Type.Missing, "", "", false, Type.Missing, Type.Missing, false, false, Type.Missing, false, true, Excel.XlCorruptLoad.xlNormalLoad);
            try
            {
                book.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, pdfFile, Excel.XlFixedFormatQuality.xlQualityStandard, true, false);
            }
            finally
            {
                (book as Excel._Workbook).Close(false);
                Marshal.ReleaseComObject(book);
            }
        }
    }
}
