using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Threading;

namespace Victorina
{
    class Program
    {
        static Timer time;
        static int count;

        //метод таймера
        static void Count(object sender)
        {
            count--;

            Console.SetCursorPosition(1, 1);
            Console.WriteLine(count.ToString());
            Console.SetCursorPosition(13, 10);

            if (count <= 0)
            {
                time.Dispose();
                Console.SetCursorPosition(1, 1);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Время истекло!");
                Console.ResetColor();
                Console.SetCursorPosition(13, 10);
            }

            GC.Collect();
        }


        static void Main(string[] args)
        {
            FileStream fsUsersWrite;
            FileStream fsUsersRead;
            Player player = new Player();
            List<UsersData> user = new List<UsersData>();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetWindowSize(80, 30);
            Console.SetCursorPosition(35, 12);
            Console.Write("Ваше имя: ");
            Console.ForegroundColor = ConsoleColor.Blue;
            string username = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Yellow;

            //считываем данные из файла об игроках
            fsUsersRead = File.OpenRead("Users.json");
            DataContractJsonSerializer formatterUsers = new DataContractJsonSerializer(user.GetType());
            user = formatterUsers.ReadObject(fsUsersRead) as List<UsersData>;
            
            //Определяем существует ли уже такой игрок, если да - определяем его номер, 
            //если нет - записываем его в файл Users.json
            int n = 0;
            int usernumber = -1;
            int rating = 0;
            int lastQuestion = 0;

            foreach (UsersData i in user)
            {
                if(i.username.IndexOf(username) != -1)
                {
                    usernumber = n;
                    rating = i.rating;
                    lastQuestion = i.lastQuestion;
                    break;
                }
                n++;
            }           

            if(usernumber == -1)
            {
                Console.SetCursorPosition(22, 12);
                Console.WriteLine("Вы новенький. Рады вас видеть!");
                Thread.Sleep(1000);
                user.Add(new UsersData() { rating = 0, username = username, lastQuestion = 0 });
                usernumber = user.Count - 1;
            }

            Console.Clear();
            Console.SetCursorPosition(20, 12);
            Console.WriteLine(username + ", добро пожаловать на викторину!");
            Thread.Sleep(1000);

            string nMenu;
            int rightAnswer = 0;
            int wrongAnswer = 0;

            do
            {
                Console.Clear();
                Console.SetCursorPosition(35, 5);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("   М Е Н Ю\n");
                Console.WriteLine("                                  1. Новая игра");
                Console.WriteLine("                                  2. Ваши результаты");
                Console.WriteLine("                                  3. Топ-20");
                Console.WriteLine("                                  4. Выход\n\n\n");
                Console.Write("                                 : ");
                nMenu = Console.ReadLine();

                switch (nMenu)
                {
                    case "1":
                        
                        rightAnswer = 0;
                        wrongAnswer = 0;

                        Task[] tasks = new Task[10];
                        for (int i = 0; i < tasks.Length; i++)
                            tasks[i] = new Task();

                        if (user[usernumber].lastQuestion == tasks.Length)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.SetCursorPosition(22, 14);
                            Console.WriteLine("Вы ответили на все вопросы викторины.");
                            Console.ResetColor();
                            Console.ReadKey();
                            break;
                        }
                        
                        //считываем данные из файла с вопросами для викторины
                        FileStream fsTasks = File.OpenRead("Tasks.json");
                        DataContractJsonSerializer formatterTasks = new DataContractJsonSerializer(tasks.GetType());
                        tasks = formatterTasks.ReadObject(fsTasks) as Task[];

                        player.NewGame();

                        int num = 0;

                        for (int i = user[usernumber].lastQuestion; i < tasks.Length; i++)
                        {
                            if(num > 2)
                            {
                                continue;
                            }

                            Console.SetCursorPosition(36, 1);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Вопрос " + ++num + ":\n");
                            Console.WriteLine(tasks[i].question + "\n");
                            Console.WriteLine("             " + tasks[i].var1);
                            Console.WriteLine("             " + tasks[i].var2);
                            Console.WriteLine("             " + tasks[i].var3);
                            Console.WriteLine("             " + tasks[i].var4);

                            //сохраняем номер заданного вопроса
                            lastQuestion = tasks[i].n;

                            //включаем таймер
                            count = 8;
                            TimerCallback tm = new TimerCallback(Count);
                            time = new Timer(tm, null, 0, 1000);

                            string str;
                            int playerAnswer;

                            do
                            {
                                str = Console.ReadLine();
                                if (count < 1)
                                {
                                    playerAnswer = -1;
                                    break;
                                }
                            } while (!int.TryParse(str, out playerAnswer) | playerAnswer < 1 | playerAnswer > 4);


                            if (playerAnswer == tasks[i].answer)
                            {
                                rightAnswer++;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.SetCursorPosition(39, 12);
                                Console.WriteLine("Верно!");
                                Thread.Sleep(1000);
                                Console.Clear();
                            }
                            else if (playerAnswer == -1)
                            {
                                wrongAnswer++;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.SetCursorPosition(34, 12);
                                Console.WriteLine("Время истекло!");
                                Thread.Sleep(1000);
                                Console.Clear();
                            }
                            else
                            {
                                wrongAnswer++;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.SetCursorPosition(38, 12);
                                Console.WriteLine("Неверно!");
                                Console.ResetColor();
                                Thread.Sleep(1000);
                                Console.Clear();
                            }
                        }
                        time.Dispose();
                        
                        //изменение рейтинга с учетом результатов последней игры
                        rating = player.Results(rightAnswer, wrongAnswer, rating);
                        user[usernumber].rating = rating;
                        user[usernumber].lastQuestion = lastQuestion;
                        user.Sort((x, y) => -x.rating.CompareTo(y.rating));
                        fsTasks.Close();

                        //сохраняем результаты игры в файл
                        fsUsersRead.Close();
                        fsUsersWrite = File.Create("Users.json");
                        formatterUsers.WriteObject(fsUsersWrite, user);
                        fsUsersWrite.Close();

                        Console.ReadKey();
                        break;

                    case "2":
                        player.Results(rightAnswer, wrongAnswer, rating - rightAnswer);
                        Console.ReadKey();
                        break;

                    case "3":
                        player.Top20();
                        int top = user.Count;

                        if (top > 20)
                        {
                            top = 20;
                        }

                        Console.SetCursorPosition(0, 8);
                        Console.ForegroundColor = ConsoleColor.Yellow;

                        for (int i = 0; i < top; i++)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            UsersData item = user[i];
                            if(item.username == username)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                            }
                            Console.WriteLine("                             " + item.rating + "\t\t" + item.username);
                        }

                        Console.WriteLine();
                        Console.ReadKey();
                        break;

                    default:
                        Console.Clear();
                        Console.SetCursorPosition(25, 13);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Викторина закончена! Пока!\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
                        break;
                }
            } while (nMenu != "4");
        }
    }
}
