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
                
                if (question.Name == "PackageList")
                    PackageList(answer);
                
                if (question.Name == "Upgrade")
                {
                    if (answer == "y")
                        RunChoco(new List<string> { "all -y" });
                    else
                        continue;
                }
            }
        }
        
        public static void PackageList(string answer)
        {
            if (answer.Length > 0)
            {
                if (!File.Exists(answer.ToString()))
                    Console.WriteLine("File not found. Using default choco_args.txt");
                else
                    chocoPath = answer.ToString();
            }
            else
                Console.WriteLine("Using default choco_args.txt");

            List<string> chocoLines = new List<string>();
            string[] lines = File.ReadAllLines(chocoPath);
            Console.WriteLine("\n-- List of Packages --\n");
            
            foreach (var line in lines)
            {
                chocoLines.Add(line);
                Console.WriteLine(line);
            }
            
            Console.Write($"\nInstall ({lines.Length}) packages? (y/n): ");
            
            if (Console.ReadLine().ToLower() == "y")
                RunChoco(chocoLines);
            else
                Environment.Exit(0);
        }
        
        public static void RunChoco(List<string> chocoPackages)
        {
            foreach (string package in chocoPackages)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c choco upgrade " + package
                    }
                };
                process.Start();
                process.WaitForExit();
            }
        }

        public static Question[] questions = 
        {
            new Question { Text = "Upgrade installed Choco packages not in list? (y/n): ", Name = "Upgrade", ExpectedType = typeof(string) },
            new Question { Text = "Path to package list file (empty for default): ", Name = "PackageList", ExpectedType = typeof(string) },
        };
    }
    
    public class Question
    {
        public string Text { get; set; }
        public string Name { get; set; }
        public Type ExpectedType { get; set; }
    }
}
