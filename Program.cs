using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace ChocoAutoInstall
{
    internal class Program
    {
        public static string chocoPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "choco_args.txt");
        static void Main(string[] args)
        {
            UserInput();
            
        }
        public static void UserInput()
        {
            List<object> answers = new List<object>();

            foreach (Question question in questions)
            {
                Console.Write(question.Text);
                string answer = Console.ReadLine();
                answers.Add(Convert.ChangeType(answer, question.ExpectedType));
            }
            
            if(answers[0].ToString().Length > 0)
            {
                if(!File.Exists(answers[0].ToString()))
                    Console.WriteLine("File not found. Using default choco_args.txt");
                else
                    chocoPath = answers[0].ToString();
            }
            else
                Console.WriteLine("Using default choco_args.txt");

            RunChoco();
        }
        
        public static List<string> ReadChocoArgs(string path)
        {
            List<string> chocoLines = new List<string>();
            var lines = File.ReadAllLines(path);

            Console.WriteLine($"\nThe following ({lines.Length}) packages will be installed:\n");

            foreach (var line in lines)
            {
                chocoLines.Add(line);
                Console.WriteLine(line);
            }
            Console.WriteLine();
            return chocoLines;
        }
        
        public static void RunChoco()
        {
            List<string> chocoArguements = ReadChocoArgs(chocoPath);
            
            foreach (string arg in chocoArguements)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c choco install " + arg
                    }
                };
                process.Start();
                process.WaitForExit();

            }
        }

        public static Question[] questions = 
        {
            new Question { Text = "Path to package list file (empty for default): ", ExpectedType = typeof(string) },
            //new Question { Text = "Example", ExpectedType = typeof(int) },
        };
    }
    
    public class Question
    {
        public string Text { get; set; }
        public Type ExpectedType { get; set; }
    }
}
