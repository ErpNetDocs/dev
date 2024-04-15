using System;
using System.Text;

namespace ConsoleApp1
{
    static class Program
    {
        const string ErpNetDatabaseUri = "https://demodb.my.erp.net";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("ERP.net Domain Api Samples.");
            Console.WriteLine($"Type an ERP.net database url (or use default {ErpNetDatabaseUri}) and hit ENTER:");

            var dbUrl = Console.ReadLine();
            if (!string.IsNullOrEmpty(dbUrl))
            {
                if (!dbUrl.Contains('.'))
                    dbUrl =  $"{dbUrl}.my.erp.net";
                if (!dbUrl.Contains("://"))
                    dbUrl = $"https://{dbUrl}";
            }
            else
            {
                dbUrl = ErpNetDatabaseUri;
            }

            Console.WriteLine($"Test Database: {dbUrl}");


            var service = Samples.CreateServiceAsync(dbUrl).Result;

            
            while (true)
            {
                Console.WriteLine("Type a sample number and hit Enter to execute it. Esc to quit.");

                int i = 0;
                foreach (var samp in Samples.List)
                    Console.WriteLine($"{++i} {samp.Name} - {samp.Description}");

                StringBuilder tok = new StringBuilder();
                while (true)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Enter)
                    {
                        if (int.TryParse(tok.ToString(), out var num) && num > 0 && num <= Samples.List.Length)
                        {
                            num--;
                            var samp = Samples.List[num];
                            Console.WriteLine();
                            Console.WriteLine("Executing " + samp.Name);
                            Console.WriteLine();
                            samp.ExecuteAsync(service).Wait();
                            Console.WriteLine();
                            Console.WriteLine("Done.");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Invalid sample number: " + tok);
                        }
                        break;
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        return;
                    }
                    else
                    {
                        tok.Append(key.KeyChar);
                    }
                }
            }
           
        }


        

    }
}
