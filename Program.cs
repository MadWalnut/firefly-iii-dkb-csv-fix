using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FireflyFixTransactionReportDKB
{
    internal static class Program
    {
        /// <summary>
        /// Checks, if a CSV file is a DKB transaction report (checking account / credit card). Converts the file from ANSI to UTF-8, after removing unwanted lines.
        /// Prepares the file for the import with Firefly III (https://www.firefly-iii.org/) and the DKB profile (https://github.com/firefly-iii/import-configurations/tree/main/de/dkb).
        /// Does not remove the header line, as this can be done in the CSV importer directly.
        /// Code: https://github.com/MadWalnut/firefly-iii-dkb-csv-fix
        /// Icon made by Freepik (https://www.flaticon.com/authors/freepik) from https://www.flaticon.com
        /// </summary>
        /// <param name="args">First parameter: Path to CSV file.</param>
        private static void Main(string[] args)
        {
            // Check if a potential path has been given. 
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments. Specify CSV path in first parameter.");
                Console.ReadLine();
                return;
            }

            // Display help / version text if requested.
            if (args[0] == "help" || args[0] == "-h" || args[0] == "--help" || args[0] == "version" || args[0] == "-v" || args[0] == "--version")
            {
                Console.WriteLine("This application checks if a given file (first parameter) is a valid DKB transaction report (checking account or credit card). "
                    + "It then converts the file from ANSI to UTF-8, after removing several unwanted lines in order to prepare it for the Firefly III CSV importer.");
                Console.WriteLine(Environment.NewLine + "Usage: FireflyFixTransactionReportDKB [FILEPATH TO CSV EXPORT FROM DKB]");
                Console.WriteLine(Environment.NewLine + "Repo and more information: https://github.com/MadWalnut/firefly-iii-dkb-csv-fix" + Environment.NewLine
                    + "Icon made by Freepik (https://www.flaticon.com/authors/freepik) from https://www.flaticon.com" + Environment.NewLine
                    + "Firefly III DKB CSV Fix v1.0.4.0" + " Copyright © " + DateTime.Now.Year + " MadWalnut");
                return;
            }
             
            // Set the CSV path from the first program argument.
            var importFilePath = args[0];

            // Check, if the given file exists.
            if (!File.Exists(importFilePath))
            {
                Console.WriteLine("Invalid path. The CSV file does not exist.");
                Console.ReadLine();
                return;
            }

            // Check, if the given file is a CSV file.
            if (!string.Equals(Path.GetExtension(importFilePath), ".csv", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Invalid path. The given file is not a CSV file.");
                Console.ReadLine();
                return;
            }

            // Variable, to which the content of the CSV file will be parsed.
            List<string> importFileContent;

            try
            {
                // Register additional codepages (needed for reading the ANSI formatted CSV file).
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                // Read the given CSV file with Windows-1252-Coding (works for the codepage (ANSI) used by DKB).
                importFileContent = File.ReadAllLines(importFilePath, Encoding.GetEncoding(1252)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while reading the CSV file: " + ex.Message);
                Console.ReadLine();
                return;
            }

            // Check, if the contents-variable is still null or empty after reading.
            if (importFileContent == null || importFileContent.Count == 0)
            {
                Console.WriteLine("Imported list is null or empty.");
                Console.ReadLine();
                return;
            }

            // Check, if the first line looks like an already converted DKB export. This happens, when the user uses this application twice on the same file.
            if (importFileContent[0].StartsWith("\"Buchungstag\";\"Wertstellung\";\"Buchungstext\";\"")
                || importFileContent[0].StartsWith("\"Umsatz abgerechnet und nicht im Saldo enthalten\";\"Wertstellung\";\"Belegdatum\";\"Beschreibung\";\""))
            {
                Console.WriteLine("The given file seems to have already been converted.");
                Console.ReadLine();
                return;
            }
            // Check, if the file is an untouched DKB export. If not, exit the application so no non-DKB files are attempted to be converted.
            else if (!importFileContent[0].StartsWith("\"Kontonummer:\";\"DE") && !importFileContent[0].StartsWith("\"Kreditkarte:\";\""))
            {
                Console.WriteLine("Unexpected start of given file. Is this file a DKB export?");
                Console.ReadLine();
                return;
            }

            // Remove unwanted lines by iterating through all lines. Running backwards, so we can directly remove unwanted lines without messing up the for-counter. 
            for (var i = importFileContent.Count - 1; i >= 0; i--)
            {
                // When a line is empty, a credit card bill, a monthly closing or starts with unwanted content; remove it.
                if (string.IsNullOrWhiteSpace(importFileContent[i]) || importFileContent[i].StartsWith("\"Kontonummer:\";\"DE") || importFileContent[i].StartsWith("\"Von:\";\"")
                    || importFileContent[i].StartsWith("\"Bis:\";\"") || importFileContent[i].StartsWith("\"Kontostand vom ") || importFileContent[i].StartsWith("\"Kreditkarte:\";\"")
                    || importFileContent[i].StartsWith("\"Zeitraum:\";\"") || importFileContent[i].StartsWith("\"Saldo:\";\"") || importFileContent[i].StartsWith("\"Datum:\";\"")
                    || importFileContent[i].Contains("\"KREDITKARTENABRECHNUNG") || importFileContent[i].Contains(";\"Abschluss\";"))
                {
                    // Remove the unwanted line from the list.
                    importFileContent.RemoveAt(i);
                }
            }

            // Overwrite the existing CSV file with the new version, where unwanted lines have been removed. Save as UTF-8 instead of ANSI to ensure the file works with the Firefly III CSV importer.
            try
            {
                File.WriteAllLines(importFilePath, importFileContent, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while writing new CSV back to file: " + ex.Message);
                Console.ReadLine();
                return;
            }
        }
    }
}