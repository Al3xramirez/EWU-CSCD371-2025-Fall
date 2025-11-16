using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment
{
    public static class DataHelper
    {

        public static IEnumerable<string> CsvRows(string filePath)
        {

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CSV file was not found: {filePath}");
            }

            return File.ReadAllLines(filePath).Skip(1);
        }


        public static IEnumerable<string> ExtractStates(IEnumerable<string> csvRows)
        {
            return csvRows
                .Select(row =>
                {
                    var parts = row.Split(',');
                    return parts.Length > 6 ? parts[6].Trim() : string.Empty;
                })
                .Where(state => !string.IsNullOrWhiteSpace(state))
                .Distinct()
                .OrderBy(state => state);
        }



        public static IEnumerable<IPerson> ExtractPeople(IEnumerable<string> csvRows)
        {
            return csvRows
                .Select(row =>
                {
                    string[] parts = row.Split(',');
                    if (parts.Length < 8)
                    {
                        throw new InvalidDataException($"Invalid row: {row}");
                    }
                    IAddress address = new Address(
                        parts[4].Trim(),
                        parts[5].Trim(),
                        parts[6].Trim(),
                        parts[7].Trim()
                    );
                    return new Person(
                        parts[0].Trim(),
                        parts[1].Trim(),
                        address,
                        parts[3].Trim()
                    );
                });
        }


    }
}
