using System;
using System.IO;
using System.Text.RegularExpressions;



namespace InterviewTest {
    class Program {
        static bool ValidateInput(string line) {
            string pattern = @"[A-Z][A-Z][0-9]+\b";
            Match m = Regex.Match(line, pattern, RegexOptions.IgnoreCase);
            
            return m.Success;
        }

        static void Main(string[] args) {
            string line;

            StreamReader file = new StreamReader(System.IO.Path.Combine(Environment.CurrentDirectory, "input.txt"));
            while((line = file.ReadLine()) != null){  
                //System.Console.WriteLine(line);
                if (ValidateInput(line) == true) {
                    System.Console.WriteLine(@"true"); 
                }
                else {
                    System.Console.WriteLine(@"false"); 
                }
            }  
            
            file.Close();  
        }
    }
}
