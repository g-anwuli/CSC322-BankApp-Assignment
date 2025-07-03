using BankApp.Models;
using BankApp.Db;

namespace BankApp.Services
{
    public interface IBankService
    {
        Customer CreateCustomer(string firstname, string lastname, string email, string password, AccountType accountType);
        Account CreateAccount(Guid customerId, AccountType type);
        void Transfer(string from, string to, decimal amount);
        void Withdraw(string from, string to, decimal amount);
    }


    public class BankService : IBankService
    {
        BankDb Db = new();
        public Customer CreateCustomer(string firstname, string lastname, string email, string password, AccountType type)
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

            CreateAccount(customer.Id, type);

            return customer;
        }

        public Account CreateAccount(Guid customerId, AccountType type)
        {
            Account acc;
            if (type == AccountType.Savings)
            {
                Account? existingAccount = Db.Accounts.FindOne(c => c.CustomerId == customerId);

                if (existingAccount != null)
                {
                    throw new InvalidOperationException("Customer already has a savings account");
                }

                acc = new SavingsAccount
                {
                    CustomerId = customerId,
                    AccountNumber = Account.GenerateAccountNumber(),
                    InterestRate = 0.05m
                }
                ;
            }
            else
            {
                Account? existingAccount = Db.Accounts.FindOne(c => c.CustomerId == customerId);

                if (existingAccount != null)
                {
                    throw new InvalidOperationException("Customer already has a savings account");
                }

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

        public void Transfer(string from, string to, decimal amount)
        {
            Account? acc1 = Db.Accounts.FindOne(c => c.AccountNumber == from);
            Account? acc2 = Db.Accounts.FindOne(c => c.AccountNumber == from);
            if (acc1 == null || acc2 == null)
            {
                throw new InvalidOperationException("One or both accounts not found");
            }

            if (acc1.CustomerId != acc2.CustomerId)
            {
                throw new InvalidOperationException("Accounts belong to different customers");
            }

            FundTransfer(acc1, acc2, amount);
        }

        public void Withdraw(string from, string to, decimal amount)
        {
            Account? acc1 = Db.Accounts.FindOne(c => c.AccountNumber == from);
            Account? acc2 = Db.Accounts.FindOne(c => c.AccountNumber == from);
            if (acc1 == null || acc2 == null)
            {
                throw new InvalidOperationException("One or both accounts not found");
            }

            FundTransfer(acc1, acc2, amount);
        }

        private void FundTransfer(Account acc1, Account acc2, decimal amount)
        {
            if (acc1 is SavingsAccount acc)
            {
                Transaction interest1_tx = new()
                {
                    AccountId = acc.Id,
                    Amount = acc.Interest,
                    Type = "interest_before_withdraw",
                    Details = new InterestDetails
                    {
                        InterestRate = acc.InterestRate,
                    }
                };
                Db.Transactions.Add(interest1_tx);
                acc.ApplyInterest();
            }

            if (acc2 is SavingsAccount acc_2)
            {
                Transaction interest2_tx = new()
                {
                    AccountId = acc_2.Id,
                    Amount = acc_2.Interest,
                    Type = "interest_before_deposit",
                    Details = new InterestDetails
                    {
                        InterestRate = acc_2.InterestRate,
                    }
                };
                Db.Transactions.Add(interest2_tx);
                acc_2.ApplyInterest();
            }

            acc1.Withdraw(amount);
            Transaction withdraw_tx = new()
            {
                AccountId = acc1.Id,
                Amount = amount,
                Type = "withdraw",
                Details = new WithdrawDetails
                {
                    Reciever = acc2.Id
                }
            };
            Db.Transactions.Add(withdraw_tx);

            acc2.Deposit(amount);
            Transaction deposit_tx = new()
            {
                AccountId = acc2.Id,
                Amount = amount,
                Type = "desposit",
                Details = new DepositDetails
                {
                    Sender = acc1.Id
                }
            };

            Db.Transactions.Add(deposit_tx);
            Db.Transactions.Commit();
        }
    }
}