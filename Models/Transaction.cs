namespace BankApp.Models
{
    /// <summary>
    /// Represents the details of a deposit transaction.
    /// </summary>
    public class DepositDetails
    {
        public Guid Sender { get; set; }
    }

    /// <summary>
    /// Represents the details of a withdraw transaction.
    /// </summary>
    public class WithdrawDetails
    {
        public Guid Reciever { get; set; }
    }

    /// <summary>
    /// Represents the details of a transfer transaction.
    /// </summary>
    public class TransferDetails : WithdrawDetails
    {
    }

    /// <summary>
    /// Represents the details of a interest transaction.
    /// </summary>
    public class InterestDetails
    {
        public decimal InterestRate { get; set; }
    }


    /// <summary>
    /// Represents a transaction associated with a bank account.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Unique identifier for the transaction.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The ID of the account associated with this transaction.
        /// </summary>
        public required string AccountNumber { get; set; }

        /// <summary>
        /// The amount of money involved in the transaction.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Type of transaction ("deposit" or "transfer" or "withdraw" or "interest_before_withdraw" or "interest_before_deposit").
        /// </summary>
        public required string Type { get; set; }

        /// <summary>
        /// Additional data relevant to the transaction. 
        /// This can vary depending on the transaction type.
        /// </summary>
        public object? Details { get; set; }

        /// <summary>
        /// Timestamp of when the transaction occurred (in UTC).
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
