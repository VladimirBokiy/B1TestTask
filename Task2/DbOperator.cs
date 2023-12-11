using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace Task2
{
    /// <summary>
    /// Class for database operations
    /// </summary>
    public class DbOperator
    {
        /// <summary>
        /// Connection string ti database
        /// </summary>
        public string ConnectionString { get; set; } = "Server=(localdb)\\MSSQLLocalDB;Database=TestTask";

        /// <summary>
        /// Databases table names 
        /// </summary>
        public string[] TableNames { get; set; } = { "[Task2.ShallowAccounts]", "[Task2.GeneralAccounts]", "[Task2.LoadedFiles]" };

        /// <summary>
        /// XlsParser object
        /// </summary>
        public XlsParser XlsParser { get; set; } = new XlsParser();

        public DbOperator() { }

        /// <summary>
        /// This method stores xls table in the database
        /// </summary>
        /// <param name="fileName"></param>
        public void ImportTable(string fileName)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                DataTable table = XlsParser.ParseFile(fileName);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = TableNames[0];

                    bulkCopy.WriteToServer(ConvertToShallowAccount(table));
                }

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = TableNames[1];
                
                    bulkCopy.WriteToServer(ConvertToGeneralAccount(table));
                }
                
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = TableNames[2];
                
                    bulkCopy.WriteToServer(ConvertToLoadedFiles(table));
                }
                
                connection.Close();
            }
        }

        /// <summary>
        /// This method converts initial table into table with shallow accounts only
        /// </summary>
        /// <param name="dataTable">Initial table</param>
        /// <returns>Table with shallow account only</returns>
        public DataTable ConvertToShallowAccount(DataTable dataTable)
        {
            IEnumerable<DataRow> query =
                from r in dataTable.AsEnumerable()
                where Regex.IsMatch(r.Field<String>("ClassNumber"), @"^\d{4}$")
                      && r.Field<String>("ClassNumber") != null
                select r;

            DataTable rowTable = query.CopyToDataTable<DataRow>();

            rowTable.Columns.Add("ClassNumberBase");
            rowTable.Columns.Add("ClassNumberSerial");
            int columnIndex = rowTable.Columns.IndexOf("ClassNumber");
            for (int i = 0; i < rowTable.Rows.Count; i++)
            {
                string currentValue = rowTable.Rows[i][columnIndex].ToString();

                rowTable.Rows[i]["ClassNumberBase"] = currentValue.Substring(0,2);
                rowTable.Rows[i]["ClassNumberSerial"] = currentValue.Substring(2,2);
            }
            rowTable.Columns.Remove("ClassNumber");
            rowTable.Columns.Remove("FileName");
            
            return rowTable;
        }

        /// <summary>
        /// This method converts initial table into table with general accounts only
        /// </summary>
        /// <param name="dataTable">Initial table</param>
        /// <returns>Table with general account only</returns>
        public DataTable ConvertToGeneralAccount(DataTable dataTable)
        {
            IEnumerable<DataRow> query =
                        from r in dataTable.AsEnumerable()
                        where Regex.IsMatch(r.Field<String>("ClassNumber"), @"^\d{1,2}$")
                        select r;

            DataTable table = query.CopyToDataTable<DataRow>();

            table.Columns["ClassNumber"].ColumnName = "ClassNumberBase";
            table.Columns.Remove("FileName");
            return table;
        }

        /// <summary>
        /// This method converts initial table into table with information about stored file
        /// </summary>
        /// <param name="dataTable">Initial table</param>
        /// <returns>Table with information about stored file</returns>
        public DataTable ConvertToLoadedFiles(DataTable dataTable)
        {
            IEnumerable<DataRow> query =
                (from r in dataTable.AsEnumerable()
                    select r).Take(1);
            
            DataTable table = query.CopyToDataTable<DataRow>();
            table.Columns.Remove("ClassNumber");
            table.Columns.Remove("IncomingBalanceAsset");
            table.Columns.Remove("IncomingBalanceLiability");
            table.Columns.Remove("TurnoverDebit");
            table.Columns.Remove("TurnoverCredit");
            table.Columns.Remove("OutgoingBalanceAsset");
            table.Columns.Remove("OutgoingBalanceLiability");

            return table;
        }

        /// <summary>
        /// This method returns the list of stored files in database
        /// </summary>
        /// <returns>List of stored files</returns>
        public List<String> GetLoadedFiles()
        {
            List<String> files = new List<String>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT FileName FROM {TableNames[2]}", connection))
                {
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            files.Add(dataReader.GetValue(0).ToString());
                        }
                    }
                }

                connection.Close();
            }

            return files;
        }
    }
}