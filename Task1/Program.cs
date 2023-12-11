using System;

namespace Task1
{
    public class Program
    {
        public static void Main()
        {
            FileGenerator generator = new FileGenerator();
            FileJoiner joiner = new FileJoiner();
            DbOperator dbOperator = new DbOperator();
            
            //генерация файлов
            //generator.GenerateFiles();
            
            while (true)
            {
                Console.WriteLine("Хотите ли Вы объединить файлы (Да\\Нет)?");
                if (Console.ReadLine().ToLower() == "да")
                {
                    Console.WriteLine("Введите сочетание символов, которое не должны содержать строки в объединенном файле:");
                    string stringToRemove = Console.ReadLine();
                    joiner.JoinFiles(stringToRemove);
                }
                dbOperator.GetRowNumber();
                Console.WriteLine("Хотите ли Вы перед импортом предварительно очистить таблицу (Да\\Нет)?");
                if (Console.ReadLine().ToLower() == "да")
                {
                    dbOperator.ClearTable();
                }

                dbOperator.ImportFiles();
                dbOperator.ExecuteCountMedianAndSumProcedure();
            
                Console.WriteLine("Для завершения введите \"выход\"");
                if (Console.ReadLine().ToLower() == "выход")
                {
                    break;
                }
            }
        }
    }
}

