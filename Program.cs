// Program.cs
using System.Reflection.Metadata.Ecma335;
using BankApp.CLI;

namespace BankApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var cli = new BaseCli("BankApp");
            cli.RegisterCommand("account", "account --balance $accountNumber This shows the balance for that account", args =>
            {
                BaseCli.PrintColoredText($"Account Number is: {args[0]}", ConsoleColor.Yellow);

                BaseCli.PrintTable([
                    new { Name= "Godswill", Age=12, CreatedAt=DateTime.Now },
                    new { Name= "John", Age=25, CreatedAt=DateTime.Now.AddDays(-1) },
                    new { Name= "Jane", Age=30, CreatedAt=DateTime.Now.AddDays(-2) }
                ]);
            });
            cli.Run();
        }
    }
}
