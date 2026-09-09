using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using LoanTracker.Core;

namespace LoanTracker.Tests
{
    internal class InMemoryLoanRepository : ILoanRepository
    {
        private List<Loan> _store = new List<Loan>();
        public List<Loan> GetAll() => new List<Loan>(_store);
        public List<Loan>? LastSaved { get; private set; }
        public void SaveAll(IEnumerable<Loan> loans)
        {
            LastSaved = loans.Select(l => l).ToList();
            _store = LastSaved.ToList();
        }
    }

    public class LoanServiceTests
    {
        [Fact]
        public void AddLoan_AddsAndSaves()
        {
            var repo = new InMemoryLoanRepository();
            var svc = new LoanService(repo);

            var loan = svc.AddLoan("Eve", 150m, DateOnly.FromDateTime(DateTime.Today));

            Assert.NotNull(loan);
            Assert.Single(svc.GetAllLoans());
            Assert.NotNull(repo.LastSaved);
            Assert.Single(repo.LastSaved);
            Assert.Equal("Eve", repo.LastSaved[0].Name);
        }

        [Fact]
        public void EditLoan_UpdatesLoanAndSaves()
        {
            var repo = new InMemoryLoanRepository();
            var svc = new LoanService(repo);
            var loan = svc.AddLoan("Frank", 100m, DateOnly.FromDateTime(DateTime.Today));

            svc.EditLoan(loan.Id, "Frank Updated", 120m, DateOnly.FromDateTime(DateTime.Today));

            var stored = svc.GetAllLoans().First(l => l.Id == loan.Id);
            Assert.Equal("Frank Updated", stored.Name);
            Assert.Equal(120m, stored.Amount);
            Assert.NotNull(repo.LastSaved);
        }

        [Fact]
        public void DeleteLoan_RemovesAndSaves()
        {
            var repo = new InMemoryLoanRepository();
            var svc = new LoanService(repo);
            var loan = svc.AddLoan("Gail", 80m, DateOnly.FromDateTime(DateTime.Today));

            svc.DeleteLoan(loan.Id);

            Assert.Empty(svc.GetAllLoans());
            Assert.NotNull(repo.LastSaved);
            Assert.Empty(repo.LastSaved);
        }

        [Fact]
        public void UpdateRepayment_UpdatesLoanAndSaves()
        {
            var repo = new InMemoryLoanRepository();
            var svc = new LoanService(repo);
            var loan = svc.AddLoan("Heidi", 200m, DateOnly.FromDateTime(DateTime.Today));

            svc.UpdateRepayment(loan.Id, 50m);

            var stored = svc.GetAllLoans().First(l => l.Id == loan.Id);
            Assert.Equal(150m, stored.OutstandingAmount);
            Assert.NotNull(repo.LastSaved);
        }

        [Fact]
        public void GetOutstandingBalancedByName_ReturnsCorrectSums()
        {
            var repo = new InMemoryLoanRepository();
            var svc = new LoanService(repo);
            svc.AddLoan("Ivy", 100m, DateOnly.FromDateTime(DateTime.Today));
            svc.AddLoan("Ivy", 50m, DateOnly.FromDateTime(DateTime.Today));
            svc.AddLoan("Jack", 20m, DateOnly.FromDateTime(DateTime.Today));

            var result = svc.GetOutstandingBalancesByName();

            Assert.Equal(2, result.Count);
            Assert.Equal(150m, result["Ivy"]);
            Assert.Equal(20m, result["Jack"]);
        }

        /// <summary>
        /// Test for EditLoan to ensure that editing a non-existent loan throws an ArgumentException.
        /// </summary>
        [Fact]
        public void EditLoan_WhenLoanDoesNotExist_Throws()
        {
            var repo = new InMemoryLoanRepository();
            var svc = new LoanService(repo);

            Assert.Throws<ArgumentException>(() =>
                svc.EditLoan(
                    Guid.NewGuid(),
                    "Unknown",
                    100m,
                    DateOnly.FromDateTime(DateTime.Today)));
        }

        /// <summary>
        /// Test for GetOutstandingBalancesByName to ensure that fully repaid loans are excluded from the result.
        /// </summary>
        [Fact]
        public void GetOutstandingBalancesByName_ExcludesFullyRepaidLoan()
        {
            var repo = new InMemoryLoanRepository();
            var svc = new LoanService(repo);

            var loan = svc.AddLoan(
                "Ivy",
                100m,
                DateOnly.FromDateTime(DateTime.Today));

            svc.UpdateRepayment(loan.Id, 100m);

            var result = svc.GetOutstandingBalancesByName();

            Assert.DoesNotContain("Ivy", result.Keys);
        }
    }
}
