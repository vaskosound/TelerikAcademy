using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Data.OleDb;
using Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace Supermarket.Client
{
    public static class VendorsReports
    {
        public static void CreateExcel()
        {
            string conString = @"Data Source=../../../supermarket.db;Version=3;";
            var dbSqLiteConnection = new SQLiteConnection(conString);

            string conectionExcel = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\..\..\Products-Total-Reports.xlsx;Extended Properties=""Excel 12.0 Xml;HDR=Yes""";
            OleDbConnection con = new OleDbConnection(conectionExcel);
            con.Open();
            try
            {
                dbSqLiteConnection.Open();

                string query = @"SELECT  te.[VandorName], SUM(tr.[TotalIncomes]) as Incomes, te.[Expense]," +
                    " SUM(tr.[TotalIncomes] * t.[Tax]) as Taxes," +
                    " SUM(tr.[TotalIncomes]) - te.[Expense] - SUM(tr.[TotalIncomes] * t.[Tax]) as FinancialResult " +
                    "from  Taxes t Join TotalReports tr on t.[ProductName] = tr.[ProductName] " +
                    "Join TotalExpenses te on tr.[VendorName] = te.[VandorName] " +
                    "Group By te.[VandorName], te.[Expense]";

                SQLiteCommand cmd = new SQLiteCommand(query, dbSqLiteConnection);

                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    string row = string.Format(@"INSERT INTO [Sheet1$] " +
                        @"(VandorName, Incomes,Expense, Taxes, FinancialResult) VALUES (""{0}"", {1}, {2}, {3}, {4})",
                        result["VandorName"].ToString(), result["Incomes"].ToString(), result["Expense"].ToString(),
                        result["Taxes"].ToString(), result["FinancialResult"].ToString());
                    OleDbCommand cmdExcel = new OleDbCommand(row, con);
                    cmdExcel.ExecuteNonQuery();
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                dbSqLiteConnection.Close();
            }
        }

        //private static void DoThatExcelThing()
        //{

        //    ApplicationClass myExcel;
        //    try
        //    {
        //        myExcel = GetObject("Excel.Application");
        //    }
        //    catch (Exception ex)
        //    {
        //        myExcel = new ApplicationClass();
        //    }

        //    myExcel.Visible = true;
        //    Workbook wb1 = myExcel.Workbooks.Add("");
        //    Worksheet ws1 = (Worksheet)wb1.Worksheets[1];


        //    //Add some formatting
        //    Range rng1 = ws1.get_Range("A1", "E1");
        //    rng1.Font.Bold = true;
        //    rng1.Font.ColorIndex = 3;
        //    rng1.HorizontalAlignment = XlHAlign.xlHAlignCenter;

        //    Range rng2 = ws1.get_Range("A2", "H50");
        //    rng2.WrapText = false;
        //    rng2.EntireColumn.AutoFit();

        //    //Add a header row
        //    ws1.get_Range("A1", "E1").EntireRow.Insert(XlInsertShiftDirection.xlShiftDown, Missing.Value);
        //    ws1.Cells[1, 1] = "Employee Contact List";
        //    Range rng3 = ws1.get_Range("A1", "H1");
        //    rng3.Merge(Missing.Value);
        //    rng3.Font.Size = 16;
        //    rng3.Font.ColorIndex = 3;
        //    rng3.Font.Underline = true;
        //    rng3.Font.Bold = true;
        //    rng3.VerticalAlignment = XlVAlign.xlVAlignCenter;

        //    //Save and close
        //    string strFileName = String.Format("Employees{0}.xlsx", DateTime.Now.ToString("HHmmss"));
        //    System.IO.File.Delete(strFileName);
        //    wb1.SaveAs(strFileName, XlFileFormat.xlWorkbookDefault, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
        //        XlSaveAsAccessMode.xlExclusive, Missing.Value, false, Missing.Value, Missing.Value, Missing.Value);
        //    myExcel.Quit();

        //}

    }

}
