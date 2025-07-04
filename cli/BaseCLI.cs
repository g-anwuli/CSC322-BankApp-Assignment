using System.Diagnostics;

namespace BankApp.CLI
{
    public class CliCommand
    {
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required Action<string[]> Action { get; init; }
    }

    public class BaseCli
    {
        private readonly Dictionary<string, CliCommand> _commands = new();
        private readonly string _appName;
        private bool _running = true;

        public BaseCli(string appName)
        {
            _appName = appName;
            RegisterBuiltIns();
        }

        private void RegisterBuiltIns()
        {
            RegisterCommand("--help", "Displays all available commands.", args => ShowHelp());
            RegisterCommand("-h", "Alias for --help", args => ShowHelp());
            RegisterCommand("exit", "Exits the application.", args => Quit());
            RegisterCommand("clear", "Clears the console.", args => Console.Clear());
        }

        public void RegisterCommand(string name, string description, Action<string[]> action)
        {
            _commands[name] = new CliCommand { Name = name, Description = description, Action = action };
        }

        public void Run()
        {
            Console.WriteLine($"Welcome to {_appName} CLI.");
            ShowHelp();

            while (_running)
            {
                Console.Write($"{_appName}> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;

                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var command = parts[0];
                var args = parts.Skip(1).ToArray();

                if (_commands.TryGetValue(command, out var cliCommand))
                {
                    cliCommand.Action.Invoke(args);
                }
                else
                {
                    PrintColoredText($"Unknown command: {command}", ConsoleColor.Red);
                }
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine("\nAvailable Commands:");
            PrintTable(_commands.Values.ToList(), c => c.Name, c => c.Description);
        }

        private void Quit()
        {
            _running = false;
            Console.WriteLine("Exiting...");
        }

        public static void PrintColoredText(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void PrintHeader(string text)
        {
            PrintColoredText($"\n=== {text.ToUpper()} ===\n", ConsoleColor.DarkBlue);
        }

        public static void PrintInfo(string label, string value, ConsoleColor labelColor = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = labelColor;
            Console.Write(label.PadRight(20));
            Console.ResetColor();
            Console.WriteLine(value);
        }


        public static void PrintTable<T>(IEnumerable<T> items, params Func<T, string>[] columnSelectors)
        {
            var itemsList = items.ToList();
            if (!itemsList.Any() || columnSelectors.Length == 0) return;

            // Calculate column widths
            var columnWidths = new int[columnSelectors.Length];
            for (int i = 0; i < columnSelectors.Length; i++)
            {
                columnWidths[i] = itemsList.Select(columnSelectors[i]).Max(s => s?.Length ?? 0) + 2;
            }

            // Create border line
            var borderLine = "+" + string.Join("+", columnWidths.Select(w => new string('-', w + 2))) + "+";

            // Top border
            Console.WriteLine(borderLine);

            // Print each row
            foreach (var item in itemsList)
            {
                var row = "|";
                for (int i = 0; i < columnSelectors.Length; i++)
                {
                    var cellValue = columnSelectors[i](item) ?? "";
                    row += $" {cellValue.PadRight(columnWidths[i])} |";
                }
                Console.WriteLine(row);
                Console.WriteLine(borderLine);
            }
        }

        // Version with headers
        public static void PrintTableWithHeaders<T>(IEnumerable<T> items, string[] headers, params Func<T, string>[] columnSelectors)
        {
            var itemsList = items.ToList();
            if (!itemsList.Any() || columnSelectors.Length == 0 || headers.Length != columnSelectors.Length) return;

            // Calculate column widths (including headers)
            var columnWidths = new int[columnSelectors.Length];
            for (int i = 0; i < columnSelectors.Length; i++)
            {
                var dataWidth = itemsList.Select(columnSelectors[i]).Max(s => s?.Length ?? 0);
                var headerWidth = headers[i]?.Length ?? 0;
                columnWidths[i] = Math.Max(dataWidth, headerWidth) + 2;
            }

            // Create border line
            var borderLine = "+" + string.Join("+", columnWidths.Select(w => new string('-', w + 2))) + "+";

            // Top border
            Console.WriteLine(borderLine);

            // Headers
            var headerRow = "|";
            for (int i = 0; i < headers.Length; i++)
            {
                headerRow += $" {headers[i].PadRight(columnWidths[i])} |";
            }
            Console.WriteLine(headerRow);
            Console.WriteLine(borderLine);

            // Data rows
            foreach (var item in itemsList)
            {
                var row = "|";
                for (int i = 0; i < columnSelectors.Length; i++)
                {
                    var cellValue = columnSelectors[i](item) ?? "";
                    row += $" {cellValue.PadRight(columnWidths[i])} |";
                }
                Console.WriteLine(row);
                Console.WriteLine(borderLine);
            }
        }

    }

}
