namespace BankApp.Models
{
    /// <summary>
    /// Represents the type of a bank account.
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// A savings account.
        /// </summary>
        Savings,

        /// <summary>
        /// A current (checking) account.
        /// </summary>
        Current
    }

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
    /// Abstract base class representing a bank account.
    /// Contains shared properties and methods for different account types.
    /// </summary>
    public abstract class Account
    {
        /// <summary>
        /// Unique identifier for the account.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Current account balance (protected from external modification).
        /// </summary>
        protected decimal Balance { get; set; } = 0;

        /// <summary>
        /// Identifier for the customer who owns the account.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Human-readable account number.
        /// </summary>
        public string? AccountNumber { get; set; }

        /// <summary>
        /// Returns the current balance of the account.
        /// </summary>
        public decimal GetBalance()
        {
            return Balance;
        }

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
        public abstract void Deposit(decimal amount);

        /// <summary>
        /// Withdraws a specified amount from the account.
        /// </summary>
        /// <param name="amount">The amount to withdraw.</param>
        public abstract void Withdraw(decimal amount);
    }


    /// <summary>
    /// Represents a current (checking) account with standard deposit and withdrawal operations.
    /// </summary>
    public class CurrentAccount : Account
    {
        /// <inheritdoc />
        public override void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive");
            Balance += amount;
        }

        /// <inheritdoc />
        public override void Withdraw(decimal amount)
        {
            if (amount > Balance)
                throw new InvalidOperationException("Insufficient funds");
            Balance -= amount;
        }
    }


    /// <summary>
    /// Represents a savings account with interest accrual.
    /// </summary>
    public class SavingsAccount : Account
    {
        /// <summary>
        /// Annual interest rate (e.g., 0.05 for 5%).
        /// </summary>
        public decimal InterestRate { get; set; }

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
            }
        }

        /// <inheritdoc />
        public override void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive");
            Balance += amount;
        }

        /// <inheritdoc />
        public override void Withdraw(decimal amount)
        {
            if (amount > Balance)
                throw new InvalidOperationException("Insufficient funds");
            Balance -= amount;
        }
    }
}