namespace BankApp.Models
{
    /// <summary>
    /// Represents the currency of a bank account.
    /// </summary>
    public enum CurrencyType
    {
        /// <summary>
        /// A naira currency.
        /// </summary>
        NGN,

        /// <summary>
        /// A usd currency.
        /// </summary>
        USD
    }

    /// <summary>
    /// Represents an interest rate for a bank account.
    /// </summary>
    public class InterestRate
    {
        /// <summary>
        /// Unique identifier for the interest rate record.
        /// This is automatically generated when a new interest rate is created.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// The unique identifier for the interest rate record.
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        /// The interest rate for the account.
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// The date when the interest rate was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// The date when the interest was collected.
        /// </summary>
        public DateTime LastCollected { get; set; } = DateTime.Now;

        /// <summary>
        /// The date when the interest rate was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Updates the interest rate for the account.
        /// </summary>
        public void UpdateRate(decimal newRate)
        {
            if (newRate < 0)
            {
                throw new ArgumentException("Interest rate must be non-negative.");
            }
            Rate = newRate;
            LastUpdated = DateTime.Now;
        }
    }

    /// <summary>
    /// base class representing a bank account.
    /// Contains shared properties and methods for different account types.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Unique identifier for the account.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Identifier for the customer who owns the account.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Current account balance (protected from external modification).
        /// </summary>
        private decimal Balance { get; set; }; //this is just for test case so we can have something to work with

        /// <summary>
        /// Human-readable account number.
        /// </summary>
        public string AccountNumber { get; set; };

        /// <summary>
        /// Account type (e.g., "current", "savings").
        /// Default is "unknown" to ensure all accounts have a type.
        /// </summary>
        public string AccountType { get; set; } = "unknown";

        /// <summary>
        /// The currency type of the account.
        /// Default is NGN (Nigerian Naira).
        /// </summary>
        public CurrencyType Currency { get; set; } = CurrencyType.NGN;

        /// <summary>
        /// The date and time the customer record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// The date and time the account was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Generates a random 10-digit account number.
        /// </summary>
        /// <returns>A string representing the account number.</returns>
        public static string GenerateAccountNumber()
        {
            var random = new Random();
            return random.Next(1000000000, int.MaxValue).ToString();
        }

        /// <summary>
        /// Retrieves the current balance of the account.
        /// This method is used to access the balance without allowing external modification.
        /// </summary>
        public decimal GetBalance()
        {
            return Balance;
        }

        /// <summary>
        /// Deposits a specified amount into the current account.
        /// </summary>
        /// <param name="amount">The amount to deposit.</param>
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive.");
            }

            Balance += amount;
        }

        /// <summary>
        /// Withdraws a specified amount from the current account.
        /// </summary>
        /// <param name="amount">The amount to withdraw.</param>
        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be positive.");
            }

            if (amount > Balance)
            {
                throw new InvalidOperationException("Insufficient funds for withdrawal.");
            }

            Balance -= amount;
        }
    }


    /// <summary>
    /// Represents a current (checking) account with standard deposit and withdrawal operations.
    /// </summary>
    public class CurrentAccount : Account
    {
        public CurrentAccount()
        {
            AccountType = "current";
        }
    }


    /// <summary>
    /// Represents a savings account with interest accrual.
    /// </summary>
    public class SavingsAccount : Account
    {
        public SavingsAccount()
        {
            AccountType = "savings";

        }

        public void ApplyInterest(InterestRate interest)
        {
            if (interest.AccountId != Id)
            {
                throw new InvalidOperationException("Interest rate does not belong to this account.");
            }
            ;
            if (account.InterestRate < 0 || Balance < 0)
            {
                throw new ArgumentException("Interest rate and balance must be non-negative.");
            }
            ;

            time_diff = (DateTime.Now - interest.LastCollected) / 365.0;

            Balance += Balance * interest.Rate * time_diff;
        }
    }
}