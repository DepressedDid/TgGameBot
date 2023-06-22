using TgMyGameInfoBot;
namespace TgMyGameInfoBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MyGameInfoBot Bot = new MyGameInfoBot();
            Bot.Start();
            Console.ReadKey();
        }
    }
}