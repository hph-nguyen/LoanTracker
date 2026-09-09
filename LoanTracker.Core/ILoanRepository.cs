using System;
using System.Collections.Generic;
using System.Text;

namespace LoanTracker.Core
{
    /// <summary>
    /// Defines the contract for a repository that manages loan data.
    /// </summary>
    public interface ILoanRepository
    {
        List<Loan> GetAll();
        void SaveAll(IEnumerable<Loan> loans);
    }
}
