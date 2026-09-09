using System.Text.Json;
using LoanTracker.Core;

namespace LoanTracker.Console.Persistence
{
    public class JsonLoanRepository : ILoanRepository
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions() { WriteIndented = true };

        private readonly string _filePath;

        public JsonLoanRepository(string filePath)
        {
            _filePath = filePath;
        }

        public List<Loan> GetAll()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Loan>();
            }
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Loan>>(json, SerializerOptions) ?? new List<Loan>();
        }

        public void SaveAll(IEnumerable<Loan> loans)
        {
           var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory)) 
            {
                Directory.CreateDirectory(directory);
            }
            var json = JsonSerializer.Serialize(loans, SerializerOptions);
            File.WriteAllText(_filePath, json);
        }   


    }
}
