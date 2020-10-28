using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            TextStream processor = new TextStream();
            char choice = ' ';
            String response;
            bool success = false;
            do
            {
                Console.Clear();
                Console.WriteLine("*Text Processing And Concordance*");
                Console.WriteLine("\nВыберите пункт меню:\n" +
                    "1) Вывести все предложения текста в порядке возрастания количества слов в каждом из них.\n" +
                    "2) Во всех вопросительных предложениях текста найти и напечатать без повторений слова заданной длины.\n" +
                    "3) Из текста удалить все слова заданной длины, начинающиеся на согласную букву.\n" +
                    "4) В некотором предложении текста слова заданной длины заменить указанной подстрокой.\n" +
                    "5) Сформировать конкорданс текста.\n" +
                    "6) Выход.\n");
                if (success)
                {
                    Console.WriteLine("Операция завершена успешно!\n");
                }
                Console.Write("//> ");
                choice = Console.ReadLine().First();

                switch (choice)
                {
                    case '1':
                        {
                            Console.Write("Введите имя файла с исходным текстом: ");
                            response = Console.ReadLine();
                            processor.TextFileName = response;
                            Console.Write("Введите имя нового файла с результирующим текстом: ");
                            response = Console.ReadLine();
                            processor.WriteSortedByWordsAmountSentences(response);
                            success = true;
                            break;
                        }
                    case '2':
                        {
                            Console.Write("Введите имя файла с исходным текстом: ");
                            response = Console.ReadLine();
                            processor.TextFileName = response;
                            int fixedLength;
                            Console.Write("Введите длину слов: ");
                            while (!int.TryParse(Console.ReadLine(), out fixedLength)) ;
                            Console.Write("Введите имя нового файла с результирующим текстом: ");
                            response = Console.ReadLine();
                            processor.WriteFixedLengthWordsFromQuestions(fixedLength, response);
                            success = true;
                            break;
                        }
                    case '3':
                        {
                            Console.Write("Введите имя файла с исходным текстом: ");
                            response = Console.ReadLine();
                            processor.TextFileName = response;
                            int fixedLength;
                            Console.Write("Введите длину слов: ");
                            while (!int.TryParse(Console.ReadLine(), out fixedLength)) ;
                            Console.Write("Введите имя нового файла с результирующим текстом: ");
                            response = Console.ReadLine();
                            processor.DeleteFixedLengthWordsWithFirstConsonant(fixedLength, response);
                            success = true;
                            break;
                        }
                    case '4':
                        {
                            Console.Write("Введите имя файла с исходным текстом: ");
                            response = Console.ReadLine();
                            processor.TextFileName = response;
                            int fixedLength;
                            long targetSentenceNum;
                            Console.Write("Введите номер предложения в тексте (с 0): ");
                            while (!long.TryParse(Console.ReadLine(), out targetSentenceNum)) ;
                            Console.Write("Введите длину слов: ");
                            while (!int.TryParse(Console.ReadLine(), out fixedLength)) ;
                            Console.Write("Введите подстроку для замены: ");
                            String replacer = Console.ReadLine();
                            Console.Write("Введите имя нового файла с результирующим текстом: ");
                            response = Console.ReadLine();
                            processor.ReplaceFixedLengthWords(targetSentenceNum, fixedLength, replacer, response);
                            success = true;
                            break;
                        }
                    case '5':
                        {
                            int linesInPage;
                            Concordance concordanceProcessor = new Concordance();
                            Console.Write("Введите имя файла с исходным текстом: ");
                            response = Console.ReadLine();
                            concordanceProcessor.TextFileName = response;
                            Console.Write("Введите кол-во строк в одной странице: ");
                            while (!int.TryParse(Console.ReadLine(), out linesInPage)) ;
                            concordanceProcessor.LinesInPage = linesInPage;
                            Console.Write("Введите имя нового файла с конкордансом текста: ");
                            response = Console.ReadLine();
                            concordanceProcessor.WriteConcordance(response);
                            success = true;
                            break;
                        }
                    default:
                        break;
                }
            } while (choice != '6');
        }
    }
}
