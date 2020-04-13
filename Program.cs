using System;
using System.IO;

namespace whitespace {
    class Program {
        static void Main(string[] args) {
            string line;  
            
            // Read the file and display it line by line.  
            StreamReader file = new StreamReader(System.IO.Path.Combine(Environment.CurrentDirectory, "input.txt"));
            while((line = file.ReadLine()) != null){  
                System.Console.WriteLine(line);  
            }  
            
            file.Close();  
        }
    }
}
