using BankApp.Models;
using BankApp.Db;

namespace BankApp.Services
{
    public interface IBankService
    {
        (Customer, Account) CreateCustomer(string firstname, string lastname, string email, string password, string accountType);
        Account CreateAccount(Guid customerId, string type);
        Account CreateAccountByEmail(string Email, string type);
        void Withdraw(string from, string to, decimal amount);

        List<Transaction> GetTransactions(string accountNumber);
        List<Account> GetAccountsByCustomerEmail(string email);
        List<Account> GetAccountsByCustomerId(Guid customerId);
        Customer? GetCustomerByEmail(string email);
    }


    public class BankService : IBankService
    {
        private BankDb Db;

        public BankService(BankDb db)
        {
            Db = db ?? throw new ArgumentNullException(nameof(db), "Database cannot be null");
        }

        public List<Account> GetAccountsByCustomerId(Guid customerId)
        {
            return Db.Accounts.Find(acc => acc.CustomerId == customerId);
        }

        public List<Account> GetAccountsByCustomerEmail(string email)
        {
            Customer? customer = Db.Customers.FindOne(c => c.Email == email);
            if (customer == null)
            {
                throw new InvalidOperationException("Customer not found");
            }

            return Db.Accounts.Find(acc => acc.CustomerId == customer.Id);
        }

        public Customer? GetCustomerByEmail(string email)
        {
            return Db.Customers.FindOne(c => c.Email == email);
        }


        public List<Transaction> GetTransactions(string accountNumber)
        {
            return Db.Transactions.Find(t => t.AccountNumber == accountNumber);
        }

        public (Customer, Account) CreateCustomer(string firstname, string lastname, string email, string password, string type)
        {
            Customer? existingCustomer = Db.Customers.FindOne(c => c.Email == email);
            if (existingCustomer != null)
            {
                throw new InvalidOperationException("Customer with this email already exists");
            }

            Customer customer = new Customer
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                Password = password
            };
            Db.Customers.Add(customer);
            Db.Customers.Commit();

            Account acc = CreateAccount(customer.Id, type);

            return (customer, acc);
        }

        public Account CreateAccountByEmail(string email, string type)
        {
            Customer? customer = Db.Customers.FindOne(c => c.Email == email);
            if (customer == null)
            {
                throw new InvalidOperationException("Customer with this email does not exist");
            }

            return CreateAccount(customer.Id, type);
        }

        public Account CreateAccount(Guid customerId, string type)
        {
            Account? existingAccount = Db.Accounts.FindOne(c => c.CustomerId == customerId && c.AccountType == type);
            if (existingAccount != null)
            {
                throw new InvalidOperationException($"Customer already has a {type} account");
            }

            Account acc;
            if (type == "savings")
            {
                acc = new SavingsAccount
                {
                    CustomerId = customerId,
                    AccountNumber = Account.GenerateAccountNumber()
                }
                ;
            }
            else
            {
                acc = new CurrentAccount
                {
                    CustomerId = customerId,
                    AccountNumber = Account.GenerateAccountNumber()
                };
            }

            Db.Accounts.Add(acc);
            Db.Accounts.Commit();

            return acc;
        }

        public void Withdraw(string from, string to, decimal amount)
        {
            Account? acc1 = Db.Accounts.FindOne(c => c.AccountNumber == from);
            Account? acc2 = Db.Accounts.FindOne(c => c.AccountNumber == to);
            if (acc1 == null || acc2 == null)
            {
                throw new InvalidOperationException("One or both accounts not found");
            }

            if (amount > acc1.Balance)
            {
                throw new InvalidOperationException("Insufficient funds in the source account");
            }

            FundTransfer(acc1, acc2, amount);
        }

        private void FundTransfer(Account acc1, Account acc2, decimal amount)
        {
            if (acc1.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient balance.");
            }

            if (acc1 is SavingsAccount savings1)
            {
                Db.Transactions.Add(new Transaction
                {
                    AccountNumber = savings1.AccountNumber!,
                    Amount = savings1.Interest,
                    Type = "interest_applied_before_withdraw",
                    Details = new InterestDetails { InterestRate = savings1.InterestRate }
                });
                savings1.ApplyInterest();
                Db.Accounts.Update(savings1);

            }

            if (acc2 is SavingsAccount savings2)
            {
                Db.Transactions.Add(new Transaction
                {
                    AccountNumber = savings2.AccountNumber!,
                    Amount = savings2.Interest,
                    Type = "interest_applied_before_deposit",
                    Details = new InterestDetails { InterestRate = savings2.InterestRate }
                });

                savings2.ApplyInterest();
                Db.Accounts.Update(savings2);

            }

            acc1.Withdraw(amount);
            Db.Accounts.Update(acc1);
            Db.Transactions.Add(new Transaction
            {
                AccountNumber = acc1.AccountNumber!,
                Amount = amount,
                Type = "withdraw",
                Details = new WithdrawDetails { Reciever = acc2.Id }
            });

            acc2.Deposit(amount);
            Db.Accounts.Update(acc2);
            Db.Transactions.Add(new Transaction
            {
                AccountNumber = acc2.AccountNumber!,
                Amount = amount,
                Type = "deposit",
                Details = new DepositDetails { Sender = acc1.Id }
            });

            Db.Transactions.Commit();
            Db.Accounts.Commit();
        }
    }
}