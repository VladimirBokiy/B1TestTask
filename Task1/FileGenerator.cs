using System;
using System.IO;

namespace Task1
{ 
    /// <summary>
    /// Class which implements file generation.
    /// </summary>
    class FileGenerator
    {
        /// <summary>
        /// This method generates 100 files with 10000 rows in each file.
        /// Output directory is fixed.
        /// Files are named according to template file{number}.txt.
        /// File rows are generated according to next template:
        /// {random date}||{random 10-symbol latin string}||{random 10-symbol russian string}||{
        /// random int from 1 to 100000000}||{a random positive number with 8 decimal places in the range from 1 to 20}||
        /// </summary>
        public void GenerateFiles()
        {
            string outputDirectory = "..\\..\\files";  // Замените на путь к желаемой папке вывода
            Random random = new Random();

            for (int i = 0; i < 100; i++)
            {
                string filePath = Path.Combine(outputDirectory, $"file{i + 1}.txt");

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    for (int j = 0; j < 100000; j++)
                    {
                        DateTime randomDate = DateTime.Now.AddDays(-random.Next(1, 1826));  // Отнимаем случайное количество дней от текущей даты за последние 5 лет
                        string randomLatin = GenerateRandomString(10, random);
                        string randomRussian = GenerateRandomRussianString(10, random);
                        int randomInt = random.Next(1, 50000001) * 2;  // Генерируем случайное четное число от 1 до 100000000
                        double randomDouble = random.NextDouble() * 19 + 1;  // Генерируем случайное число с 8 знаками после запятой от 1 до 20

                        writer.WriteLine($"{randomDate:dd.MM.yyyy}||{randomLatin}||{randomRussian}||{randomInt}||{randomDouble:F8}||");
                    }
                }
            }

            Console.WriteLine("Генерация завершена.");
        }
        
        /// <summary>
        /// This method generates a random latin string to be pasted in a file row.
        /// </summary>
        /// <param name="length">String lenght</param>
        /// <param name="random">Random instance</param>
        /// <returns>Random latin string</returns>
        static string GenerateRandomString(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }
        
        /// <summary>
        /// This method generates a random russian string to be pasted in a file row.
        /// </summary>
        /// <param name="length">String lenght</param>
        /// <param name="random">Random instance</param>
        /// <returns>Random russian string</returns>
        static string GenerateRandomRussianString(int length, Random random)
        {
            const string chars = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }
    }
}
