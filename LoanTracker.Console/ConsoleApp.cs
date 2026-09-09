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
                PrintMenu();
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
                        ListAllLoans();
                        break;
                    case "5":
                        RecordRepayment();
                        break;
                    case "6": 
                        ShowOutstandingBalances();
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

        private static void  PrintMenu()
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
        private static string FormatCurrency(decimal amount)
        {
            return amount.ToString("0.00", CultureInfo.InvariantCulture);
        }   

        private static string FormatDate(DateOnly date)
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

            decimal loanAmount;

            while (true)
            {
                System.Console.Write("Amount: ");
                var amountInput = System.Console.ReadLine();

                if (decimal.TryParse(amountInput, out loanAmount) && loanAmount > 0)
                    break;

                System.Console.Write("Invalid amount. Please enter a positive number. Re-enter? (y/n): ");
                var retry = System.Console.ReadLine();

                if (retry?.Trim().ToLower() != "y")
                    return;
            }

            System.Console.Write("Note (optional): ");
            var note = System.Console.ReadLine();

            var loan = _loanService.AddLoan(name, loanAmount, DateOnly.FromDateTime(DateTime.Now), note);
            System.Console.WriteLine($"Added: {loan.Name} borrowed {FormatCurrency(loanAmount)} on {FormatDate(loan.DateLent)}");
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
            System.Console.WriteLine($"Are you sure you want to delete the loan for {loanToDelete.Name} borrowed {FormatCurrency(loanToDelete.Amount)} on {FormatDate(loanToDelete.DateLent)}? (y/n)");
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
            var newName = string.IsNullOrEmpty(newNameInput) ? loanToEdit.Name : newNameInput.Trim();

            decimal newAmount;
            while (true)
            {
                System.Console.Write($"Enter new amount (current: {FormatCurrency(loanToEdit.Amount)}): ");
                var newAmountInput = System.Console.ReadLine();

                if (string.IsNullOrEmpty(newAmountInput))
                {
                    newAmount = loanToEdit.Amount;
                    break; // Keep old amount
                }
                if(decimal.TryParse(newAmountInput, out newAmount) && newAmount > 0)
                {
                    break; // Valid new amount
                }
                
                System.Console.WriteLine("Invalid amount. Re-enter? (y/n): ");
                var retry = System.Console.ReadLine();
                if (retry?.Trim().ToLower() != "y")
                {
                    newAmount = loanToEdit.Amount;
                    break;
                }

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

        private void ShowOutstandingBalances() 
        { 
            var balances = _loanService.GetOutstandingBalancesByName();
            if(balances.Count == 0)
            {
                System.Console.WriteLine("No loans recorded yet.");
                return;
            }
            ConsoleTableWriter.Write(new string[] { "Name", "Outstanding" }, balances.Select(kvp => new string[] { kvp.Key, FormatCurrency(kvp.Value) }).ToList());
        }
        private void RecordRepayment() 
        { 
            var loans = _loanService.GetAllLoans();
            if (loans.Count == 0)
            {
                System.Console.WriteLine("No loans recorded yet.");
                return;
            }
            PrintLoansTable(loans);
            System.Console.Write("Select a loan to record repayment by Id: ");
            var selectionInput = System.Console.ReadLine();
            if (!int.TryParse(selectionInput, out var selection) || selection < 1 || selection > loans.Count)
            {
                System.Console.Write("Please enter a valid loan number.");
                return;
            }
            var loan = loans[selection - 1];
            System.Console.Write($"Enter repayment amount for {loan.Name} (Outstanding: {FormatCurrency(loan.OutstandingAmount)}): ");
            var amountInput = System.Console.ReadLine();
            if (!decimal.TryParse(amountInput, out var repaymentAmount) || repaymentAmount <= 0)
            {
                System.Console.Write("Please enter a valid repayment amount.");
                return;
            }
            try
            {
                _loanService.UpdateRepayment(loan.Id, repaymentAmount);
                System.Console.WriteLine($"Recorded repayment of {FormatCurrency(repaymentAmount)} for {loan.Name}. New outstanding balance: {FormatCurrency(loan.OutstandingAmount)}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error recording repayment: {ex.Message}");
            }

        }

        private void ListAllLoans()
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
                    FormatDate(loan.DateLent),
                    FormatCurrency(loan.Amount),
                    FormatCurrency(loan.AmountRepaid),
                    FormatCurrency(loan.OutstandingAmount),
                    loan.IsFullyRepaid ? "Repaid" : "Open",
                    loan.Note ?? ""
                };
                rowsList.Add(rowData);
            }
            ConsoleTableWriter.Write(headers, rowsList);
        }

    }
}
