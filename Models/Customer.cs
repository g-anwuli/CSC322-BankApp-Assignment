namespace BankApp.Models
{
    /// <summary>
    /// Represents a customer in the bank system.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Unique identifier for the customer.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Customer's first name.
        /// </summary>
        public required string FirstName { get; set; }

        /// <summary>
        /// Customer's last name.
        /// </summary>
        public required string LastName { get; set; }

        /// <summary>
        /// Customer's email address.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Customer's password (should be stored hashed in production).
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// The date and time the customer record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// The date and time the customer record was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
