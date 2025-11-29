//using System;
//using System.IO;
//using OfficeOpenXml;

//namespace ReleaseNoteCreatorXlsx
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            // IMPORTANT: Required for EPPlus 8+ if you're using it personally/non-commercially
//            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//            string inputPath = @"C:\Users\user\Desktop\Winsoft Technologies\My projects\RNBuilder\RN_Release_4_0_4_10.xls";
//            string outputPath = @"C:\Users\user\Desktop\Winsoft Technologies\My projects\RNBuilder\RN_Release_4_0_4_10.xlsx";

//            try
//            {
//                // EPPlus cannot read .xls (BIFF format), only .xlsx
//                // So we must load it via Interop or a converter first.
//                // Quick workaround: use Microsoft.Office.Interop.Excel for conversion.

//                Console.WriteLine("Converting .xls to .xlsx...");

//                ConvertXlsToXlsx(inputPath, outputPath);

//                Console.WriteLine("Conversion completed successfully!");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error: " + ex.Message);
//            }
//        }

//        static void ConvertXlsToXlsx(string inputPath, string outputPath)
//        {
//            var excelApp = new Microsoft.Office.Interop.Excel.Application();
//            excelApp.Visible = false;
//            excelApp.DisplayAlerts = false;

//            var workbooks = excelApp.Workbooks;
//            var workbook = workbooks.Open(inputPath);

//            workbook.SaveAs(outputPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);

//            workbook.Close();
//            excelApp.Quit();

//            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
//            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbooks);
//            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
//        }
//    }
//}
