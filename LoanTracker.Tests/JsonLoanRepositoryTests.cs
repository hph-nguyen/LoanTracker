using System;
using System.IO;
using System.Linq;
using Xunit;
using LoanTracker.Core;
using LoanTracker.Console.Persistence;

namespace LoanTracker.Tests
{
    public class JsonLoanRepositoryTests
    {
        [Fact]
        public void GetAll_WhenFileMissing_ReturnsEmpty()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
            if (File.Exists(path)) File.Delete(path);

            var repo = new JsonLoanRepository(path);
            var all = repo.GetAll();

            Assert.NotNull(all);
            Assert.Empty(all);
        }

        [Fact]
        public void SaveAll_Then_GetAll_PersistsLoans()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
            try
            {
                var repo = new JsonLoanRepository(path);
                var loan = new Loan(Guid.NewGuid(), "Kara", 75m, DateOnly.FromDateTime(DateTime.Today));
                repo.SaveAll(new[] { loan });

                var loaded = repo.GetAll();
                Assert.Single(loaded);
                Assert.Equal("Kara", loaded.First().Name);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}
