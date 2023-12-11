using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Task1
{
    /// <summary>
    /// Class which implements operations with database
    /// </summary>
    class DbOperator
    {
        /// <summary>
        /// Connection string to database.
        /// </summary>
        public string ConnectionString { get; set; } = "Server=(localdb)\\MSSQLLocalDB;Database=TestTask";
        /// <summary>
        /// Directory, where merged files are stored.
        /// </summary>
        public string InputDirectory { get; set; } = "..\\..\\files\\merged\\";  
        /// <summary>
        /// Name of the table in database. 
        /// </summary>
        public string TableName { get; set; } = "Task1";  
        /// <summary>
        /// Extension of files to be seeked.
        /// </summary>
        public string[] FileExtensions { get; set; } = { ".txt" };  
        
        /// <summary>
        /// This method imports all the files found in directory specified in InputDirectory property into database. 
        /// </summary>
        public void ImportFiles()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string[] inputFiles = Directory.GetFiles(InputDirectory);
                Console.WriteLine($"Найдено {inputFiles.Length} файлов для импорта.");
                int fileNumber = 0;
                foreach (string inputFile in inputFiles)
                {
                    string fileExtension = Path.GetExtension(inputFile);
                    fileNumber++;
                    if (Array.IndexOf(FileExtensions, fileExtension) >= 0)
                    {
                        ImportFile(inputFile, TableName, connection, fileNumber);
                    }
                }

                connection.Close();
            }

            Console.WriteLine("Импорт завершен.");
        }
        
        /// <summary>
        /// This method imports a specific file into database
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="tableName">Name of the table in database</param>
        /// <param name="connection">Connection string</param>
        /// <param name="fileNumber">Number of file in the list of found files</param>
        static void ImportFile(string filePath, string tableName, SqlConnection connection, int fileNumber)
        {
            int importedRowCount = 0;

            using (StreamReader reader = new StreamReader(filePath))
            {

                var linesCount = File.ReadLines(filePath).Count();

                bool passed = false;
                string ans;
                int rowsToImport = int.MaxValue;
                while (!passed)
                {
                    Console.WriteLine($"В файле №{fileNumber} {linesCount} строк. Сколько из них Вы хотите импортировать? (-1 = все)");
                    ans = Console.ReadLine();
                    passed = int.TryParse(ans, out rowsToImport);
                    if (rowsToImport < 0)
                    {
                        rowsToImport = linesCount;
                    }
                    if (rowsToImport > linesCount)
                    {
                        passed = false;
                    }
                }
                
                
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = tableName;

                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("TDate", typeof(DateTime));
                    dataTable.Columns.Add("TLatin", typeof(string));
                    dataTable.Columns.Add("TRussian", typeof(string));
                    dataTable.Columns.Add("TInteger", typeof(int));
                    dataTable.Columns.Add("TDouble", typeof(double));

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] fields = line.Split(new []{"||"}, StringSplitOptions.RemoveEmptyEntries);
                        if (fields.Length == 5)
                        {
                            DataRow row = dataTable.NewRow();
                            row["TDate"] = DateTime.Parse(fields[0]);
                            row["TLatin"] = fields[1];
                            row["TRussian"] = fields[2];
                            row["TInteger"] = int.Parse(fields[3]);
                            row["TDouble"] = double.Parse(fields[4]);

                            dataTable.Rows.Add(row);

                            importedRowCount++;
                            
                            if (rowsToImport == Int32.MaxValue)
                            {
                                Console.WriteLine($"Импортировано строк: {importedRowCount}. Осталось {linesCount - importedRowCount}");
                            }
                            else
                            {
                                Console.WriteLine($"Импортировано строк: {importedRowCount}. Осталось {rowsToImport - importedRowCount}");
                                if (importedRowCount == rowsToImport)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    bulkCopy.WriteToServer(dataTable);
                }
            }
        }
        
        /// <summary>
        /// This method clears the table specified in TableName property.
        /// </summary>
        public void ClearTable()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand($"DELETE FROM {TableName}", connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
                
                Console.WriteLine($"Таблица {TableName} очищена.");
            }
        }
        
        /// <summary>
        /// This method executes stored procedure "CountMedianAndSum".
        /// </summary>
        public void ExecuteCountMedianAndSumProcedure()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string median = "";
                string average = "";
                   
                using (SqlCommand command = new SqlCommand("CountMedianAndSum", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            median += dataReader.GetValue(0);
                        }

                        dataReader.NextResult();
                        
                        while (dataReader.Read())
                        {
                            average += dataReader.GetValue(0);
                        }
                    }
                }
                
                connection.Close();
                
                Console.WriteLine($"Медиана: {median}\nСумма: {average}");
            }
        }
        
        /// <summary>
        /// This method prints the number of rows in the table specified in TableName property. 
        /// </summary>
        public void GetRowNumber()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string rowNumber = "";
                using (SqlCommand command = new SqlCommand($"SELECT COUNT(*) FROM {TableName}", connection))
                {
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            rowNumber += dataReader.GetValue(0);
                        }
                    }
                }

                connection.Close();
                
                Console.WriteLine($"В таблице {TableName} {rowNumber} строк.");
            }
        }
    }
}

