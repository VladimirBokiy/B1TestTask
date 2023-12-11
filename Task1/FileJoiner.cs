using System;
using System.IO;

namespace Task1
{
    /// <summary>
    /// Class which implements files join.
    /// </summary>
    class FileJoiner
    {
    
        /// <summary>
        /// This method joins all files found in directory specified in inputDirectory variable
        /// Output directory specified in outputFilePath variable.
        /// New file is name according to next template: "MergedExclude{stringToRemove}.txt"
        /// </summary>
        /// <param name="stringToRemove">Rows containing this string will not be presented in the new file</param>
        public void JoinFiles(string stringToRemove)
        {
            string inputDirectory = "..\\..\\files";  // 
            string outputFilePath = Path.Combine("..\\..\\files\\merged", $"MergedExclude{stringToRemove}.txt"); 

            int removedLinesCount = 0;
            int processedFiles = 0;

            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                string[] inputFiles = Directory.GetFiles(inputDirectory);

                foreach (string inputFile in inputFiles)
                {
                    using (StreamReader reader = new StreamReader(inputFile))
                    {
                        processedFiles++;
                        string line;
                        Console.Write($"Начата обработка файла {processedFiles}. ");
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (!line.Contains(stringToRemove))
                            {
                                writer.WriteLine(line);
                            }
                            else
                            {
                                removedLinesCount++;
                            }
                        }
                        Console.WriteLine($"Файл {processedFiles} прошел обработку.");
                    }
                }
            }

            Console.WriteLine($"Объединение завершено. Удалено строк: {removedLinesCount}");
        }
    }
}

