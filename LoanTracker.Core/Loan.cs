namespace LoanTracker.Core
{
    /// <summary>
    /// Represents a loan with details such as the borrower's name, loan amount, date lent, amount repaid, and an optional note.
    /// </summary>
    public class Loan
    {
        public Guid Id { get; set; }
        public string Name { get; private set; }
        public decimal Amount { get; private set; }
        public DateOnly DateLent { get; private set; }
        public decimal AmountRepaid { get; private set; }
        public string? Note { get; private set; }
        public decimal OutstandingAmount => Amount - AmountRepaid;
        public bool IsFullyRepaid => OutstandingAmount <= 0;

        public Loan(Guid id,string name, decimal amount, DateOnly dateLent, decimal amountRePaid = 0m,string? note = null)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
            }
            if(amount <= 0)
            {
                throw new ArgumentException("Amount must be a positive value.", nameof(amount));
            }
            if(amountRePaid < 0m || amountRePaid > amount)
            {
                throw new ArgumentException("Amount repaid must be a non-negative value and not exceed the loan amount.", nameof(amountRePaid));
            }
            Id = id;
            Name = name;
            Amount = amount;
            DateLent = dateLent;
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
            AmountRepaid = amountRePaid;
        }

        public void UpdateRepayment(decimal amountRePaid)
        {
            if (amountRePaid < 0m || amountRePaid > Amount)
            {
                throw new ArgumentException("Amount repaid must be a non-negative value and not exceed the loan amount.", nameof(amountRePaid));
            }
            
            if(amountRePaid > OutstandingAmount)
            {
                throw new InvalidOperationException($"Repayment amount {amountRePaid} exceeds the outstanding loan amount {OutstandingAmount}");
            }

            AmountRepaid += amountRePaid;
        }

        /// <summary>
        /// Edits the details of the loan.
        /// </summary>
        /// <param name="name">The name of the borrower.</param>
        /// <param name="amount">The amount of the loan.</param>
        /// <param name="dateLent">The date the loan was lent.</param>
        /// <param name="note">An optional note about the loan.</param>
        public void Edit(string name, decimal amount, DateOnly dateLent, string? note = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
            }
            if (amount <= 0m)
            {
                throw new ArgumentException("Amount must be a positive value.", nameof(amount));
            }
            if (AmountRepaid > amount)
            {
                throw new InvalidOperationException($"Cannot set loan amount to {amount} as it is less than the amount already repaid {AmountRepaid}");
            }
            Name = name;
            Amount = amount;
            DateLent = dateLent;
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        }
    }
}
