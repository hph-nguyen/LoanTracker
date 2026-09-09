using System;
using Xunit;
using LoanTracker.Core;

namespace LoanTracker.Tests
{

    public class LoanTests
    {
        [Fact]
        public void Constructor_ValidInputs_CreatesLoan()
        {
            var id = Guid.NewGuid();
            var loan = new Loan(id, "Alice", 100m, DateOnly.FromDateTime(new DateTime(2024,1,1)));

            Assert.Equal(id, loan.Id);
            Assert.Equal("Alice", loan.Name);
            Assert.Equal(100m, loan.Amount);
            Assert.Equal(100m, loan.OutstandingAmount);
            Assert.False(loan.IsFullyRepaid);
        }

        [Fact]
        public void Constructor_InvalidName_Throws()
        {
            Assert.Throws<ArgumentException>(() => new Loan(Guid.NewGuid(), "", 100m, DateOnly.FromDateTime(DateTime.Today)));
        }

        [Fact]
        public void UpdateRepayment_Works_And_PreventsOverpay()
        {
            var loan = new Loan(Guid.NewGuid(), "Bob", 100m, DateOnly.FromDateTime(DateTime.Today));
            loan.UpdateRepayment(40m);
            Assert.Equal(60m, loan.OutstandingAmount);

            Assert.Throws<InvalidOperationException>(() => loan.UpdateRepayment(70m));
        }

        [Fact]
        public void Edit_ValidAndInvalidBehavior()
        {
            var loan = new Loan(Guid.NewGuid(), "Carol", 200m, DateOnly.FromDateTime(DateTime.Today));
            loan.UpdateRepayment(50m);

            // Editing to a larger amount is allowed
            loan.Edit("Carol Updated", 300m, DateOnly.FromDateTime(DateTime.Today), "note");
            Assert.Equal("Carol Updated", loan.Name);
            Assert.Equal(300m, loan.Amount);

            // Editing to an amount less than already repaid should throw
            Assert.Throws<InvalidOperationException>(() => loan.Edit("Carol", 40m, DateOnly.FromDateTime(DateTime.Today)));
        }

        [Fact]
        public void FullyRepaidFlag_IsTrueWhenRepaid()
        {
            var loan = new Loan(Guid.NewGuid(), "Dan", 50m, DateOnly.FromDateTime(DateTime.Today));
            loan.UpdateRepayment(50m);
            Assert.True(loan.IsFullyRepaid);
            Assert.Equal(0m, loan.OutstandingAmount);
        }
    }
}
