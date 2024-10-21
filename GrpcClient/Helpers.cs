
namespace EpcDataApp.GrpcClient
{
    internal static class Helpers
    {
        internal static void PrintMenu()
        {
            var actions = new string[] 
            {
                "1) Получение списка вагонов с данными",
                "2) Вывести информацию о всех путях, на которых находился вагон за период прибывания на станции",
                "3) Выйти из приложения"
            };
            Console.WriteLine("Выбрите действие:");
            foreach (var action in actions)
            {
                Console.WriteLine(action);

            }
            Console.WriteLine("");
        }
        internal static void DisplayError(string messageError)
        {
            SetConsoleDisplayError();
            Console.WriteLine(messageError);
            ResetConsoleDisplayError();
        }
        private static void SetConsoleDisplayError()=> Console.ForegroundColor = ConsoleColor.DarkRed;
        private static void ResetConsoleDisplayError()=> Console.ForegroundColor = ConsoleColor.White;

        internal static void DisplayInvite(string messageInvite)
        {
            SetConsoleDisplayInvite();
            Console.WriteLine(messageInvite);
            ResetConsoleDisplayInvite();
        }
        private static void SetConsoleDisplayInvite() => Console.ForegroundColor = ConsoleColor.DarkGreen;
        private static void ResetConsoleDisplayInvite() => Console.ForegroundColor = ConsoleColor.White;

    }
}
