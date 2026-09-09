using LoanTracker.Console;
using LoanTracker.Console.Persistence;  
using LoanTracker.Core;

var dataFilePath = Path.Combine(AppContext.BaseDirectory, "loans.json");
ILoanRepository loanRepository = new JsonLoanRepository(dataFilePath);
var loanService = new LoanService(loanRepository);

var app = new ConsoleApp(loanService);
app.Run();
