using BankApp.Models;

namespace BankApp.Db
{
    /// <summary>
    /// Central database context for the Bank application.
    /// Provides access to persistent data tables for customers, accounts, and transactions.
    /// </summary>
    public class BankDb
    {
        /// <summary>
        /// Table for managing customer records.
        /// Uses "Id" as the unique identifier.
        /// </summary>
        public DbTable<Customer> Customers { get; private set; }

        /// <summary>
        /// Table for managing bank account records.
        /// Uses "AccountNumber" as the unique identifier.
        /// </summary>
        public DbTable<Account> Accounts { get; private set; }

        /// <summary>
        /// Table for managing transaction records.
        /// Uses "TransactionId" as the unique identifier.
        /// </summary>
        public DbTable<Transaction> Transactions { get; private set; }

        /// <summary>
        /// Initializes the bank database context and binds each table
        /// to its corresponding JSON storage with appropriate identifier keys.
        /// </summary>
        public BankDb()
        {
            Customers = new DbTable<Customer>("users", "Id");
            Accounts = new DbTable<Account>("accounts", "AccountNumber");
            Transactions = new DbTable<Transaction>("transactions", "TransactionId");
        }
    }
}
