using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using LoanTracker.Core;

namespace LoanTracker.Console
{
    internal class ConsoleApp
    {
        private readonly LoanService _loanService;
        public ConsoleApp(LoanService loanService)
        {
            _loanService = loanService;
        }

        public void Run()
        {
            var running = true;
            while (running)
            {
                printMenu();
                var choice = System.Console.ReadLine();
                System.Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        AddLoan();
                        break;
                    case "2":
                        EditLoan(); 
                        break;
                    case "3":
                        DeleteLoan();
                        break;
                    case "4":
                        ListAllloans();
                        break;
                    case "5":
                        RecordRepaymernt();
                        break;
                    case "6": 
                        ShowOutStandingBalances();
                        break;
                    case "7":
                        running = false;
                        break;
                    default:
                        System.Console.WriteLine("Please choose a number from 1 to 7.");
                        break;

                }
                System.Console.WriteLine();
            }

        }

        private static void printMenu()
        {
            var rule = new string('=', 42);
            System.Console.WriteLine(rule);
            System.Console.WriteLine("Loan Tracker");
            System.Console.WriteLine(rule);
            System.Console.WriteLine("1. Add a loan");
            System.Console.WriteLine("2. Edit a loan");
            System.Console.WriteLine("3. Delete a loan");
            System.Console.WriteLine("4. List all loans");
            System.Console.WriteLine("5. Record a repayment");
            System.Console.WriteLine("6. Show outstanding balances by borrowers");
            System.Console.WriteLine("7. Exit");
            System.Console.WriteLine(rule);
            System.Console.Write("Choice: "); 
        }
        private static string formatCurrency(decimal amount)
        {
            return amount.ToString("0.00", CultureInfo.InvariantCulture);
        }   

        private static string formatDate(DateOnly date)
        {
            return date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
        }
        private void AddLoan()
        {
            // Implementation for adding a loan
            System.Console.Write("Name: ");
            var name = System.Console.ReadLine();
            if(string.IsNullOrWhiteSpace(name))
            {
                System.Console.WriteLine("Name cannot be empty.");
                return;
            }
            System.Console.Write("Amount: ");
            var amount = System.Console.ReadLine();
            if(!decimal.TryParse(amount, out var loanAmount))
            {
                System.Console.WriteLine("Invalid amount.");
                return;
            }
            System.Console.Write("Note (optional): ");
            var note = System.Console.ReadLine();

            var loan = _loanService.AddLoan(name, loanAmount, DateOnly.FromDateTime(DateTime.Now), note);
            System.Console.WriteLine($"Added: {loan.Name} borrowed {formatCurrency(loanAmount)} on {formatDate(loan.DateLent)}");
        }
        private void DeleteLoan() 
        {
            var loans = _loanService.GetAllLoans();
            if (loans.Count == 0)
            {
                System.Console.WriteLine("No loans recorded yet.");
                return;
            }
            PrintLoansTable(loans);
            System.Console.Write("Enter the Id of the loan to delete: ");
            var selectionInput = System.Console.ReadLine();
            if (!int.TryParse(selectionInput, out var selection) || selection < 1 || selection > loans.Count)
            {
                System.Console.Write("Please enter a valid loan number.");
                return;
            }
            var loanToDelete = loans[selection - 1];
            System.Console.WriteLine($"Are you sure you want to delete the loan for {loanToDelete.Name} borrowed {formatCurrency(loanToDelete.Amount)} on {formatDate(loanToDelete.DateLent)}? (y/n)");
            var confirmation = System.Console.ReadLine();
            if (confirmation?.Trim().ToLower() == "y")
            {
                _loanService.DeleteLoan(loanToDelete.Id);
                System.Console.WriteLine("Loan deleted successfully.");
            }
            else
            {
                System.Console.WriteLine("Deletion cancelled.");
                return;
            }
        }
        private void EditLoan()
        {
            var loans = _loanService.GetAllLoans();
            if (loans.Count == 0)
            {
                System.Console.WriteLine("No loans recorded yet.");
                return;
            }
            PrintLoansTable(loans);
            System.Console.Write("Enter the Id of the loan to edit: ");
            var selectionInput = System.Console.ReadLine();
            if (!int.TryParse(selectionInput, out var selection) || selection < 1 || selection > loans.Count)
            {
                System.Console.Write("Please enter a valid loan number.");
                return;
            }
            var loanToEdit = loans[selection - 1];
            System.Console.Write($"Enter new name (current: {loanToEdit.Name}): ");
            var newNameInput = System.Console.ReadLine();
            var newName = string.IsNullOrEmpty(newNameInput) ? loanToEdit.Name : newNameInput;
            System.Console.Write($"Enter new amount (current: {formatCurrency(loanToEdit.Amount)}): ");
            var newAmountInput = System.Console.ReadLine();
            var newAmount = string.IsNullOrEmpty(newAmountInput) ? loanToEdit.Amount : decimal.Parse(newAmountInput);   
            if(newAmount <=0)
            {
                System.Console.WriteLine("Amount must be greater than zero.");
                return;
            }
            System.Console.Write($"Enter new note (current: {loanToEdit.Note ?? "none"}): ");
            var newNoteInput = System.Console.ReadLine();
            var newNote = string.IsNullOrEmpty(newNoteInput) ? loanToEdit.Note : newNoteInput;
            try
            {
                _loanService.EditLoan(loanToEdit.Id, newName, newAmount, loanToEdit.DateLent, newNote);
                System.Console.WriteLine("Loan updated successfully.");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error updating loan: {ex.Message}");
            }
        }

        private void ShowOutStandingBalances() 
        { 
            var loans = _loanService.GetAllLoans();
            if (loans.Count == 0)
            {
                System.Console.WriteLine("No loans recorded yet.");
                return;
            }
            var outstandingBalances = new Dictionary<string, decimal>();
            foreach (var loan in loans)
            {
                if (!outstandingBalances.ContainsKey(loan.Name))
                {
                    outstandingBalances[loan.Name] = 0m;
                }
                outstandingBalances[loan.Name] += loan.OutStandingAmount;
            }
            System.Console.WriteLine("Outstanding Balances:");
            ConsoleTableWriter.Write(new string[] { "Borrower", "Outstanding" }, outstandingBalances.Select(kvp => new string[] { kvp.Key, formatCurrency(kvp.Value) }).ToList());
        }
        private void RecordRepaymernt() 
        { 
            var loans = _loanService.GetAllLoans();
            if (loans.Count == 0)
            {
                System.Console.WriteLine("No loans recorded yet.");
                return;
            }
            PrintLoansTable(loans);
            System.Console.WriteLine("Select a loan to record repayment by Id: ");
            var selectionInput = System.Console.ReadLine();
            if (!int.TryParse(selectionInput, out var selection) || selection < 1 || selection > loans.Count)
            {
                System.Console.Write("Please enter a valid loan number.");
                return;
            }
            var loan = loans[selection - 1];
            System.Console.WriteLine($"Enter repayment amount for {loan.Name} (Outstanding: {formatCurrency(loan.OutStandingAmount)}): ");
            var amountInput = System.Console.ReadLine();
            if (!decimal.TryParse(amountInput, out var repaymentAmount) || repaymentAmount <= 0)
            {
                System.Console.Write("Please enter a valid repayment amount.");
                return;
            }
            try
            {
                _loanService.UpdateRepayment(loan.Id, repaymentAmount);
                System.Console.WriteLine($"Recorded repayment of {formatCurrency(repaymentAmount)} for {loan.Name}. New outstanding balance: {formatCurrency(loan.OutStandingAmount - repaymentAmount)}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error recording repayment: {ex.Message}");
            }

        }

        private void ListAllloans()
        {
            var loans = _loanService.GetAllLoans();
            PrintLoansTable(loans);
        }

        private void PrintLoansTable(IReadOnlyList<Loan> rows)
        {
            var headers = new string[] { "Id", "Name", "Date Lent", "Amount", "Repaid", "Outstanding", "Status" , "Note" };
            var rowsList = new List<string[]>();
            for(int i = 0, len = rows.Count; i < len; i++)
            {
                var loan = rows[i];
                var rowData = new string[]
                {
                    (i + 1).ToString(),
                    loan.Name,
                    formatDate(loan.DateLent),
                    formatCurrency(loan.Amount),
                    formatCurrency(loan.AmountRePaid),
                    formatCurrency(loan.OutStandingAmount),
                    loan.IsFullyRepaid ? "Repaid" : "Open",
                    loan.Note ?? ""
                };
                rowsList.Add(rowData);
            }
            ConsoleTableWriter.Write(headers, rowsList);
        }

    }
}
