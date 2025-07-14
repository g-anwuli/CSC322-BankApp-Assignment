// Program.cs
using BankApp.CLI;
using BankApp.Db;
using BankApp.Services;
using BankApp.Models;

namespace BankApp
{
    class Program
    {
        static void Main(string[] args)
        {
            BankDb db = new BankDb();
            BankService bankService = new BankService(db);
            AuthService authService = new AuthService();
            BaseCli cli = new BaseCli("My BankApp");

            cli.RegisterCommand("register", "This is create a bank profile that can be used to create bank accounts", arg =>
            {
                Console.Write("Enter your first name: ");
                var firstname = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(firstname))
                {
                    BaseCli.PrintColoredText("First Name cannot be empty.", ConsoleColor.Red);
                    return;
                }

                Console.Write("Enter your last name: ");
                var lastname = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(lastname))
                {
                    BaseCli.PrintColoredText("Last Name cannot be empty.", ConsoleColor.Red);
                    return;
                }

                Console.Write("Enter your email: ");
                var email = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                {
                    BaseCli.PrintColoredText("Invalid email format.", ConsoleColor.Red);
                    return;
                }

                Console.Write("Enter your password: ");
                var password = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    BaseCli.PrintColoredText("Password must be at least 6 characters long.", ConsoleColor.Red);
                    return;
                }

                Console.Write("Select an account type -> current or savings: ");
                var type = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(type) || (type != "current" && type != "savings"))
                {
                    BaseCli.PrintColoredText("Invalid account type. Please choose 'current' or 'savings'.", ConsoleColor.Red);
                    return;
                }

                try
                {
                    var (customer, acc) = bankService.CreateCustomer(firstname, lastname, email, password, type);
                    BaseCli.PrintColoredText($"\nAccount created successfully", ConsoleColor.Green);

                    BaseCli.PrintHeader("Your Profile");
                    BaseCli.PrintColoredText($"FirstName: {customer.FirstName}");
                    BaseCli.PrintColoredText($"LastName: {customer.LastName}");
                    BaseCli.PrintColoredText($"Email: {customer.Email}\n");
                    BaseCli.PrintColoredText($"Login to your account using the 'login' command to view your accounts.", ConsoleColor.DarkYellow);
                }
                catch (System.Exception ex)
                {

                    BaseCli.PrintColoredText($"Error creating account: {ex.Message}", ConsoleColor.Red);
                }
            });

            cli.RegisterCommand("login", "Login to your bank account", arg =>
            {
                Console.Write("Enter your email: ");
                var email = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                {
                    BaseCli.PrintColoredText("Invalid email format.", ConsoleColor.Red);
                    return;
                }
                Console.Write("Enter your password: ");
                var password = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    BaseCli.PrintColoredText("Password must be at least 6 characters long.", ConsoleColor.Red);
                    return;
                }

                Customer? customer = bankService.GetCustomerByEmail(email);

                if (customer == null)
                {
                    BaseCli.PrintColoredText("No account found with this email.", ConsoleColor.Red);
                    return;
                }

                var (success, message) = authService.Login(email, password, customer.Password);

                if (success)
                {
                    BaseCli.PrintColoredText(message, ConsoleColor.Green);
                    BaseCli.PrintHeader("Bank Accounts");
                    if (customer != null)
                    {
                        List<Account> accs = bankService.GetAccountsByCustomerId(customer.Id);
                        BaseCli.PrintTableWithHeaders(accs, ["Account Number", "Balance", "Type"], a => a.AccountNumber!, a => $"₦{a.GetBalance()}", a => a.AccountType);
                    }
                }
                else
                {
                    BaseCli.PrintColoredText(message, ConsoleColor.Red);
                }
            });

            cli.RegisterCommand("create-account", "[Authenticated] Create a new bank account", arg =>
            {
                if (!authService.isAuthenticated())
                {
                    BaseCli.PrintColoredText("You must be logged in to create an account.", ConsoleColor.Red);
                    return;
                }

                Console.Write("Select an account type -> current or savings: ");
                var type = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(type) || (type != "current" && type != "savings"))
                {
                    BaseCli.PrintColoredText("Invalid account type. Please choose 'current' or 'savings'.", ConsoleColor.Red);
                    return;
                }

                try
                {
                    Account acc = bankService.CreateAccountByEmail(authService._currentEmail, type);
                    BaseCli.PrintColoredText($"\nAccount created successfully", ConsoleColor.Green);
                    List<Account> accs = bankService.GetAccountsByCustomerId(acc.CustomerId);
                    BaseCli.PrintTableWithHeaders(accs, ["Account Number", "Balance", "Type"], a => a.AccountNumber!, a => $"₦{a.GetBalance()}", a => a.AccountType);

                }
                catch (System.Exception ex)
                {
                    BaseCli.PrintColoredText($"Error creating account: {ex.Message}", ConsoleColor.Red);
                }
            });

            cli.RegisterCommand("update-name", "[Authenticated] Update your profile name.", arg =>
            {
                if (!authService.isAuthenticated())
                {
                    BaseCli.PrintColoredText("You must be logged in to create an account.", ConsoleColor.Red);
                    return;
                }

                Console.Write("Enter new first name: ");
                var first = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(first))
                {
                    BaseCli.PrintColoredText("first name cannot be null or empty", ConsoleColor.Red);
                    return;
                }

                Console.Write("Enter new last name: ");
                var last = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(last))
                {
                    BaseCli.PrintColoredText("last name cannot be null or empty", ConsoleColor.Red);
                    return;
                }

                try
                {
                    Customer customer = bankService.GetCustomerByEmail(authService._currentEmail)!;
                    bankService.UpdateDetails(customer, first, last, customer.Email);
                    BaseCli.PrintColoredText($"\nProfile updated successfully", ConsoleColor.Green);
                }
                catch (System.Exception ex)
                {
                    BaseCli.PrintColoredText($"Error changing name: {ex.Message}", ConsoleColor.Red);
                }
            });

            cli.RegisterCommand("update-email", "[Authenticated] Update your profile email.", arg =>
            {
                if (!authService.isAuthenticated())
                {
                    BaseCli.PrintColoredText("You must be logged in to create an account.", ConsoleColor.Red);
                    return;
                }

                Console.Write("Enter new email: ");
                var email = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(email))
                {
                    BaseCli.PrintColoredText("New email cannot be null", ConsoleColor.Red);
                    return;
                }

                Customer? existing = bankService.GetCustomerByEmail(email);
                if (existing != null && authService._currentEmail != email)
                {
                    BaseCli.PrintColoredText("Email already in use", ConsoleColor.Red);
                    return;
                }

                try
                {
                    Customer customer = bankService.GetCustomerByEmail(authService._currentEmail)!;
                    bankService.UpdateDetails(customer, customer.FirstName, customer.LastName, email);
                    BaseCli.PrintColoredText($"\nProfile updated successfully", ConsoleColor.Green);
                }
                catch (System.Exception ex)
                {
                    BaseCli.PrintColoredText($"Error changing name: {ex.Message}", ConsoleColor.Red);
                }
            });

            cli.RegisterCommand("transfer", "[Authenticated] Transfer money between accounts", arg =>
            {
                if (!authService.isAuthenticated())
                {
                    BaseCli.PrintColoredText("You must be logged in to transfer money.", ConsoleColor.Red);
                    return;
                }

                Console.Write("Enter the account number to transfer from: ");
                var from = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(from))
                {
                    BaseCli.PrintColoredText("Account number cannot be empty.", ConsoleColor.Red);
                    return;
                }

                Console.Write("Enter the account number to transfer to: ");
                var to = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(to))
                {
                    BaseCli.PrintColoredText("Account number cannot be empty.", ConsoleColor.Red);
                    return;
                }

                Console.Write("Enter the amount to transfer: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
                {
                    BaseCli.PrintColoredText("Invalid amount. Please enter a positive number.", ConsoleColor.Red);
                    return;
                }

                try
                {
                    bankService.Withdraw(from, to, amount);
                    BaseCli.PrintColoredText($"Transfer of ₦{amount} from {from} to {to} was successful.", ConsoleColor.Green);
                }
                catch (System.Exception ex)
                {
                    BaseCli.PrintColoredText($"Error during transfer: {ex.Message}", ConsoleColor.Red);
                }
            });

            cli.RegisterCommand("transactions", "[Authenticated] See all your account transactions, optional arg: transactions $account. eg transactions 1234567890", arg =>
            {
                if (!authService.isAuthenticated())
                {
                    BaseCli.PrintColoredText("You must be logged in to view transactions.", ConsoleColor.Red);
                    return;
                }

                List<Account> accounts = bankService.GetAccountsByCustomerEmail(authService._currentEmail);

                if (arg == null || arg.Length == 0)
                {
                    List<Transaction> transactions = bankService.GetTransactions(accounts[0].AccountNumber!);

                    if (accounts.Count == 2)
                    {
                        transactions.AddRange(bankService.GetTransactions(accounts[1].AccountNumber!));
                    }

                    if (transactions.Count == 0)
                    {
                        BaseCli.PrintColoredText("No transactions found for this account.", ConsoleColor.Yellow);
                        return;
                    }

                    BaseCli.PrintTableWithHeaders(transactions, ["AccountNumber", "Type", "Amount", "Date"],
                        t => t.AccountNumber, t => t.Type, t => $"₦{t.Amount}", t => t.Timestamp.ToString("g"));
                    return;
                }

                string accountNumber = arg[0];

                if (accountNumber.Length != 10)
                {
                    BaseCli.PrintColoredText("Invalid account number format. Please enter a valid 10-digit account number.", ConsoleColor.Red);
                    return;
                }

                if (!accounts.Any(a => a.AccountNumber == accountNumber))
                {
                    BaseCli.PrintColoredText("This account does not belong to current customer", ConsoleColor.Red);
                    return;
                }

                List<Transaction> txs = bankService.GetTransactions(accountNumber);

                if (txs.Count == 0)
                {
                    BaseCli.PrintColoredText($"No transactions found for {accountNumber}.", ConsoleColor.Yellow);
                    return;
                }

                BaseCli.PrintTableWithHeaders(txs, ["Type", "Amount", "Date"],
                    t => t.Type, t => $"₦{t.Amount}", t => t.Timestamp.ToString("g"));
            });

            cli.Run();
        }
    }
}
