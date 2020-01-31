using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Victorina
{
    public class Player
    {

        public Player()
        {

        }

        public void NewGame()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\n\n\n");
            Console.WriteLine("                          П Р А В И Л А   И Г Р Ы :\n");
            Console.WriteLine("   - На каждый ответ дается 7 секунд.");
            Console.WriteLine("   - Правильный ответ +1 балл, неправильный ответ 0 баллов, нет ответа 0 баллов.\n\n");
            Console.WriteLine("   Если готовы, нажмите любую клавишу...");
            Console.ResetColor();
            Console.ReadKey();
            Console.Clear();
        }

        public int Results(int rightA, int wrongA, int raiting)
        {
            raiting = raiting + rightA;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(32, 3);
            Console.WriteLine("Ваши результаты:");
            Console.SetCursorPosition(28, 5);
            Console.WriteLine("Правильных ответов: " + rightA);
            Console.SetCursorPosition(28, 6);
            Console.WriteLine("Неправильных ответов: " + wrongA);
            Console.SetCursorPosition(28, 8);
            Console.WriteLine("Ваш рейтинг: " + raiting);
            return raiting;
        }

        public void Top20()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(35, 3);
            Console.WriteLine("ТОП - 20\n");
            Console.WriteLine("                     --------------------------------");
            Console.WriteLine("                     |    Рейтинг    |     Игрок    |");
            Console.WriteLine("                     --------------------------------");

        }


    }
}
