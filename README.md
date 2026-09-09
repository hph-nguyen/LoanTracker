LoanTracker
===========

Overview
--------
LoanTracker is a small console application for recording and managing simple person-to-person loans. It supports adding, editing, deleting loans, recording repayments, listing loans, and viewing outstanding balances grouped by borrower.

Features
--------
- Add a loan (name, amount, date, optional note)
- Edit and delete existing loans
- Record repayments against a loan
- List all loans with outstanding amounts
- Show outstanding balances aggregated by borrower

Architecture
------------
This solution contains three projects:
- LoanTracker.Core: Domain types and business logic (Loan, LoanService, ILoanRepository).
- LoanTracker.Console: Console UI and program entry point (ConsoleApp and Program).
- LoanTracker.Tests: xUnit tests covering domain and repository behavior.

The code uses a simple repository pattern (ILoanRepository) and dependency inversion so the LoanService is testable and does not depend on a concrete persistence implementation.

Data persistence
----------------
Loans are persisted locally as JSON. The console application constructs a JsonLoanRepository using a file named loans.json located in the application's base directory (AppContext.BaseDirectory). The repository serializes and deserializes a List<Loan> to this file.

How to run
----------
Requirements
- .NET 10 SDK
- Visual Studio 2026 or the dotnet CLI

Open the solution
- Open LoanTracker.slnx in Visual Studio, or from a terminal run: dotnet sln LoanTracker.slnx

Build and run (console app)
- In Visual Studio: set LoanTracker.Console as the startup project and run.
- With the CLI: cd LoanTracker.Console then dotnet run

Tests
-----
The tests use xUnit. To run them:
- In Visual Studio: run Test Explorer.
- With the CLI: from the solution root run dotnet test

What the tests cover
- LoanTests: constructor validation, Edit and UpdateRepayment behavior, outstanding/fully-repaid flags.
- LoanServiceTests: Add/Edit/Delete loans, UpdateRepayment, GetAllLoans, and outstanding-balance aggregation using an in-memory repository.
- JsonLoanRepositoryTests: persistence behavior — saving and loading loans and behavior when the data file is missing.

Design decisions
----------------
- Separation of concerns: core business logic is in LoanTracker.Core while the Console project only handles input/output.
- Testability: LoanService depends on ILoanRepository which allows in-memory fakes for unit tests.
- Keep it small and pragmatic: minimal dependencies and a straightforward JSON file for local persistence.

AI-assisted development
----------------------
Parts of the unit tests were generated with assistance from GitHub Copilot. All generated code was reviewed, understood, and validated by the developer.

License
-------
This repository is provided as-is for the coding challenge; adapt or relicense as needed.
