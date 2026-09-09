using System;
using System.Collections.Generic;
using System.Text;

namespace LoanTracker.Console
{
    public class ConsoleTableWriter
    {
        private const int MaxWidth = 30;
        public static void Write(string[] headers, IReadOnlyList<string[]> rows)
        {
            // Calculate column widths
            var widths = new int[headers.Length];

            for(int i = 0; i < headers.Length; i++)
            {
                widths[i] = headers[i].Length;

                foreach (var row in rows)
                {
                    widths[i] = Math.Max(widths[i], row[i].Length);
                }
                widths[i] = Math.Min(widths[i], MaxWidth);
            }

            // Write headers
            PrintSeparator(widths);
            PrintRows(headers, widths);
            PrintSeparator(widths);
            
            // Write rows
            for(int i = 0; i < rows.Count; i++)
            {
                PrintRows(rows[i], widths);
            }
            PrintSeparator(widths);
        }
        public static void PrintRows(string[] cells, int[] columnWidths)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                string cell = cells[i];
                int width = columnWidths[i];
                if (cell.Length > width)
                {
                    cell = cell.Substring(0, width - 3) + "...";
                }
                System.Console.Write($"| {cell.PadRight(width)} ");
            }
            System.Console.WriteLine("|");
        }

        public static void PrintSeparator(int[] columnWidths)
        {
            foreach (var width in columnWidths)
            {
                System.Console.Write("+");
                System.Console.Write(new string('-', width + 2));
            }
            System.Console.WriteLine("+");
        }
    }
}
