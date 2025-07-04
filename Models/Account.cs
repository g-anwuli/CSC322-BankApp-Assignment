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
        /// Current account balance (protected from external modification).
        /// </summary>
        public decimal Balance { get; set; } = 10_000.00m; //this is just for test case so we can have something to work with

        /// <summary>
        /// Identifier for the customer who owns the account.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Human-readable account number.
        /// </summary>
        public string? AccountNumber { get; set; }

        public string AccountType { get; set; } = "unknown";

        /// <summary>
        /// Returns the current balance of the account.
        /// </summary>
        public decimal GetBalance()
        {
            return Balance;
        }

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
        /// Deposits a specified amount into the account.
        /// </summary>
        /// <param name="amount">The amount to deposit.</param>
        public virtual void Deposit(decimal amount) { }

        /// <summary>
        /// Withdraws a specified amount from the account.
        /// </summary>
        /// <param name="amount">The amount to withdraw.</param>
        public virtual void  Withdraw(decimal amount) { }
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

        /// <inheritdoc />
        public override void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive");
            Balance += amount;
            UpdatedAt = DateTime.Now;
        }

        /// <inheritdoc />
        public override void Withdraw(decimal amount)
        {
            if (amount > Balance)
                throw new InvalidOperationException("Insufficient funds");
            Balance -= amount;
            UpdatedAt = DateTime.Now;
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
        /// <summary>
        /// Annual interest rate (e.g., 0.05 for 5%).
        /// </summary>
        public decimal InterestRate { get; set; } = 0.05m;

        /// <summary>
        /// The last date interest was applied to the account.
        /// </summary>
        public DateTime LastInterestDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Calculates the interest accumulated since the last interest date.
        /// </summary>
        public decimal Interest
        {
            get
            {
                var days = (decimal)(DateTime.Now - LastInterestDate).TotalDays;
                return Balance * InterestRate * (days / 365);
            }
        }

        /// <summary>
        /// Applies any accumulated interest to the account balance.
        /// </summary>
        public void ApplyInterest()
        {
            if (Interest > 0)
            {
                Balance += Interest;
                LastInterestDate = DateTime.Now;
                UpdatedAt = DateTime.Now;
            }
        }

        /// <inheritdoc />
        public override void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive");
            Balance += amount;
            UpdatedAt = DateTime.Now;
        }

        /// <inheritdoc />
        public override void Withdraw(decimal amount)
        {
            if (amount > Balance)
                throw new InvalidOperationException("Insufficient funds");
            Balance -= amount;
            UpdatedAt = DateTime.Now;
        }
    }
}