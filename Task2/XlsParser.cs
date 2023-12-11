using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text.RegularExpressions;

namespace Task2
{
    /// <summary>
    /// Class for xls parsing
    /// </summary>
    public class XlsParser
    {
        /// <summary>
        /// String containing input directory where files are stored
        /// </summary>
        public string InputDirectory { get; set; } = "..\\..\\data";

        /// <summary>
        /// Array, where possible file extnsions are stored
        /// </summary>
        public string[] FileExtensions { get; set; } = { ".xls", ".xlsx" };
        
        /// <summary>
        /// Connection string for database
        /// </summary>
        public string ConnectionString { get; set; } = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=NO';";
       
        /// <summary>
        /// List where found file names are stored
        /// </summary>
        public List<string> FoundFiles { get; set; } = new List<string>();

        public XlsParser() { } 

        /// <summary>
        /// This method finds all xls files in input directory
        /// </summary>
        public void FindXls()
        {
            string[] inputFiles = Directory.GetFiles(InputDirectory);
            foreach (string inputFile in inputFiles)
            {
                string fileExtension = Path.GetExtension(inputFile);
                if (Array.IndexOf(FileExtensions, fileExtension) >= 0)
                {
                    FoundFiles.Add(inputFile);
                    Console.WriteLine(inputFile);
                }
            }
        }

        /// <summary>
        /// This method converts xls table with specific structure into corresponding DataTable object
        /// </summary>
        /// <param name="filename">string containing file name</param>
        /// <returns>DataTable object of the xls table</returns>
        public DataTable ParseFile(string filename)
        {
            DataTable dataTable = new DataTable();
            string connectionString = string.Format(ConnectionString, filename);
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try {
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select F1,F2,F3,F4,F5,F6,F7 from [Sheet1$]", connectionString); //here we read data from sheet1                   
                    DataTable res = new DataTable();
                    oleAdpt.Fill(res); //fill excel data into dataTable
                    res.Columns["F1"].ColumnName = "ClassNumber";
                    res.Columns["F2"].ColumnName = "IncomingBalanceAsset";
                    res.Columns["F3"].ColumnName = "IncomingBalanceLiability";
                    res.Columns["F4"].ColumnName = "TurnoverDebit";
                    res.Columns["F5"].ColumnName = "TurnoverCredit";
                    res.Columns["F6"].ColumnName = "OutgoingBalanceAsset";
                    res.Columns["F7"].ColumnName = "OutgoingBalanceLiability";
                    res.Columns.Add("FileId", typeof(string));
                    res.Columns.Add("FileName", typeof(string));
                    foreach (DataRow row in res.Rows)
                    {
                        row["FileId"] = filename.GetHashCode().ToString();
                        row["FileName"] = filename;
                    }

                    IEnumerable<DataRow> query =
                        from r in res.AsEnumerable()
                        where r.Field<String>("ClassNumber") != null && 
                        (Regex.IsMatch(r.Field<String>("ClassNumber"), @"^\d{2,4}$") || r.Field<String>("ClassNumber") == "ПО КЛАССУ")
                        select r;

                    dataTable = query.CopyToDataTable<DataRow>();

                    int columnIndex = dataTable.Columns.IndexOf("ClassNumber");

                    for (int i = 1; i < dataTable.Rows.Count; i++)
                    {
                        string currentValue = dataTable.Rows[i][columnIndex].ToString();

                        if (currentValue == "ПО КЛАССУ")
                        {
                            string previousValue = dataTable.Rows[i - 1][columnIndex].ToString();

                            if (!string.IsNullOrEmpty(previousValue) && char.IsDigit(previousValue[0]))
                            {
                                char firstDigit = previousValue[0];
                                dataTable.Rows[i][columnIndex] = firstDigit;
                            }
                        }
                    }

                } catch {}
            }

            return dataTable;
        }
    }
}