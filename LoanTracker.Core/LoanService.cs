using System;
using System.Collections.Generic;
using System.Text;

namespace LoanTracker.Core
{
    /// <summary>
    /// Represents a service that manages loans, providing methods to:
    /// 1. Add, edit, delete, and retrieve loans, 
    /// 2. Update repayments and calculate outstanding balances.
    /// </summary>
    public class LoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly List<Loan> _loanList;

        public LoanService(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
            _loanList = _loanRepository.GetAll();
        }

        public Loan AddLoan(string name, decimal amount, DateOnly dateLent, string? note = null)
        {
            var newLoan = new Loan(Guid.NewGuid(), name, amount, dateLent, 0m, note);
            _loanList.Add(newLoan);
            _loanRepository.SaveAll(_loanList);
            return newLoan;
        }

        public void EditLoan(Guid loanId, string name, decimal amount, DateOnly dateLent, string? note = null)
        {
            var loan = _loanList.Find(l => l.Id == loanId);
            if (loan == null)
            {
                throw new ArgumentException($"No loan found with ID {loanId}", nameof(loanId));
            }

            loan.Edit(name, amount, dateLent, note);
            _loanRepository.SaveAll(_loanList);
        }

        public void DeleteLoan(Guid loanId)
        {
            var loan = _loanList.Find(l => l.Id == loanId);
            if (loan == null)
            {
                throw new ArgumentException($"No loan found with ID {loanId}", nameof(loanId));
            }
            _loanList.Remove(loan);
            _loanRepository.SaveAll(_loanList);
        }
        public IReadOnlyList<Loan> GetAllLoans()
        {
            return _loanList.AsReadOnly();
        }

        public void UpdateRepayment(Guid loanId, decimal amountRepaid)
        {
            var loan = _loanList.Find(l => l.Id == loanId);
            if (loan == null)
            {
                throw new ArgumentException($"No loan found with ID {loanId}", nameof(loanId));
            }
            loan.UpdateRepayment(amountRepaid);
            _loanRepository.SaveAll(_loanList);
        }

        /// <summary>
        /// Gets the outstanding balance for each unique name in the loan list.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, decimal> GetOutstandingBalancesByName()
        {
            return _loanList.GroupBy(loan => loan.Name)
                            .Select(group => new {group.Key, Outstanding = group.Sum(loan => loan.OutstandingAmount)})
                            .Where(x => x.Outstanding > 0m)
                            .ToDictionary(x => x.Key, x => x.Outstanding);
        }


    }
}
