using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Globalization;
using Sys = Cosmos.System;
using System.Reflection.Metadata;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace CosmosKernel1
{
    public class Kernel : Sys.Kernel
    {
        Sys.FileSystem.CosmosVFS fs = new Sys.FileSystem.CosmosVFS();
        bool check = false;
        bool pwCheck = false;
        string userName, password, unInput, pwInput;
        protected override void BeforeRun()
        {
           
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs); 
            Console.Clear();
            Console.Beep();
            Console.WriteLine("Cosmos booted successfully.\n\n");
            if (!File.Exists(@"0:\password.txt") && !File.Exists(@"0:\password.txt"))
            {
                string introduction = "\t\t\t\tWe haven't seen you around here....\n\n\t\t\t\tWhy don't you try to create an account?\n\nPlease Enter Username: ";

                Console.Write(introduction);
                userName = Console.ReadLine();
                while (!pwCheck)
                {
                    Console.Write("Please Enter Password\n(Password must contain at least 8 characters, a lowercase and uppercase,\nand a special character): ");
                    password = Console.ReadLine();

                    pwCheck = PasswordStrength(password);
                }


                SetCredentials(userName, password);
                Console.WriteLine("\nUsername: " + File.ReadAllText(@"0:\username.txt"));
                Console.WriteLine("Password: " + File.ReadAllText(@"0:\password.txt"));
                Console.WriteLine("\n\t\tAccount creation success!\n\n\t\tWelcome " + userName + "!");
                Console.WriteLine("Press Any Key to Continue...");
                Console.ReadKey(true);
                Console.Clear();
            }
            else
            {
                while (!check)
                {

                    bool yonCheck = false;
                    Console.Clear();
                    Console.Beep();
                    Console.Write("\t\t\tIt seems like I've seen you here before...\n");
                    Console.Write("Please Enter UserName: ");
                    unInput = Console.ReadLine();
                    Console.Write("Please Enter Password: ");
                    pwInput = Console.ReadLine();
                    
                    if (unInput == File.ReadAllText(@"0:\username.txt") && pwInput == File.ReadAllText(@"0:\password.txt"))
                    {
                        check = true;
                        
                        Console.Clear(); 
                        Console.WriteLine("\t\t\t\tWe missed you! Welcome Back!\n");
                    }
                    else
                    {
                        while (!yonCheck)
                        {
                            Console.Write("Invalid Credentials. Would you like to Try Again? Y/N\n> ");
                            string yon = Console.ReadLine().ToLower();
                            if (yon.Equals("y", StringComparison.InvariantCultureIgnoreCase))
                            {
                                check = false;
                                break;
                            }
                            else if (yon.Equals("n", StringComparison.InvariantCultureIgnoreCase))
                            {
                                Console.WriteLine("System Shutting Down.\nPress Any key to Continue...");
                                Console.ReadKey(true);
                                Cosmos.System.Power.Shutdown();
                            }
                            else
                            {
                                Console.WriteLine("Invalid Input. Please Try Again!\n");
                                yonCheck = false;
                            }
                        }
                        Thread.Sleep(500);
                        check = false;

                    }
                }
                         
            }
           
           
        }

        protected override void Run()
        {
          
            Console.WriteLine("Please Enter a Command.\nEnter \"Help\" for a list of commands.");
            Console.Write("> ");
            string input = Console.ReadLine();
            switch (input.ToLower())
            {
                case "fd":
                    {
                        File.Delete(@"0:\password.txt");
                        File.Delete(@"0:\username.txt");

                        break;
                    }

                case "help":
                    {
                        Console.WriteLine("\t[HE] - Simple OS Commands");
                        Console.WriteLine("\t[AB] - Kernel Information");
                        Console.WriteLine("\t[SE] - Change Login Credentials");
                        Console.WriteLine("\t[CL] - Clear screen");
                        Console.WriteLine("\t[DA] - Current Day");
                        Console.WriteLine("\t[TI] - Current Time");
                        Console.WriteLine("\t[CA] - Use Calculator");
                        Console.WriteLine("\t[RE] - Restart System");
                        Console.WriteLine("\t[SD] - Power Off");
                        break;
                    }
                case "se":
                    {
                        bool valid = false;
                        while(!valid)
                        {
                            Console.Write("\n\t[a] - Change Username\n\t[b] - Change Password\n\t[c] - Change Both\n\t[d] - Back\n> ");
                            string inputMode = Console.ReadLine().ToLower();
                            if (inputMode == "a" || inputMode == "b" || inputMode == "c" || inputMode == "d")
                            {
                                ChangeCredentials(inputMode);
                                valid = true;
                            }
                            else
                                Console.WriteLine("Invalid Input. Please Try Again!");
                        }
                        break;
                    }
                case "he":
                    {
                        Console.WriteLine("\t\t\t\tHello " + File.ReadAllText(@"0:\username.txt") + "!");
                        break;
                    }
                case "ab":
                    {
                        static void WriteFullLine(string value)
                        {

                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine(value.PadRight(Console.WindowWidth - 1));

                            Console.ResetColor();
                        }
                        WriteFullLine("This line is green.");
                        Console.WriteLine();
                        Console.WriteLine("                                 Group 1 OS 1.0");
                        WriteFullLine("This line is green.");
                        Console.WriteLine();
                        break;
                    }

                case "sd":
                    {
                        Sys.Power.Shutdown();
                        break;
                    }
                case "re":
                    {
                        Sys.Power.Reboot();
                        break;
                    }
                case "da":
                    {
                        Console.WriteLine(Date());
                        break;
                    }
                case "ti":
                    {
                        Console.WriteLine(Time());
                        break;
                    }
                case "ca":
                    {
                        Calculator();
                        break;
                    }
                case "cl":
                    {
                        Console.Clear();
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Command not Recognized. Please Try Again!");
                        break;
                    }
            }
            Console.Write('\n');
        }

        public void ChangeCredentials(string mode)
        {
            switch (mode)
            {
                case "a":
                    {
                        Console.Clear();
                        Console.WriteLine("Current Username: " + File.ReadAllText(@"0:\username.txt"));
                        Console.Write("Please Enter New Username: ");
                        string newName = Console.ReadLine();
                        File.WriteAllText(@"0:\username.txt", newName);
                        Console.WriteLine("New Username: " + File.ReadAllText(@"0:\username.txt"));
                        Console.WriteLine("Press Any Key to Continue...");
                        Console.ReadKey(true);
                        break;
                    }
                case "b":
                    {
                        Console.Clear();
                        bool pwCheck = false;
                        string newPassword = "";
                        bool passwordvalid = false;
                        while (!passwordvalid)
                        {
                            Console.WriteLine("Current Password: ");
                            string checkpass = Console.ReadLine();
                            if (!checkpass.Equals(File.ReadAllText(@"0:\password.txt")))
                            {
                                Console.WriteLine("Enter Current Password");
                            }
                            else
                            {
                                passwordvalid = true;
                            }
                        }

                        while (!pwCheck)
                        {
                            Console.Write("Please Enter New Password: ");
                            newPassword = Console.ReadLine();
                            if (!newPassword.Equals(File.ReadAllText(@"0:\password.txt")))
                                pwCheck = PasswordStrength(newPassword);
                            else
                            {
                                Console.WriteLine("New Password and Current Password cannot be same\nPlease Try Again.");
                                pwCheck = false;
                            }
                        }
                        File.WriteAllText(@"0:\password.txt", newPassword, Encoding.UTF8);
                        Console.WriteLine("New Password: " + File.ReadAllText(@"0:\password.txt"));
                        Console.WriteLine("Press Any Key to Continue...");
                        Console.ReadKey(true);
                        break;
                    }
                case "c":
                    {
                        Console.Clear();
                        bool pwCheck = false;
                        string newPassword = "";
                        bool passwordvalid = false;
                        while (!passwordvalid)
                        {
                            Console.WriteLine("Current Password: ");
                            string checkpass = Console.ReadLine();
                            if (!checkpass.Equals(File.ReadAllText(@"0:\password.txt")))
                            {
                                Console.WriteLine("Enter Current Password");
                            }
                            else
                            {
                                passwordvalid = true;
                            }
                        }
                        Console.WriteLine("Current Username: " + File.ReadAllText(@"0:\username.txt"));
                        Console.Write("Please Enter New Username: ");
                        string newName = Console.ReadLine();
                        while (!pwCheck)
                        {
                            Console.Write("Please Enter New Password\n(Password must contain at least 8 characters, a lowercase and uppercase, \nand a special character): ");
                            newPassword = Console.ReadLine();
                            if (!newPassword.Equals(File.ReadAllText(@"0:\password.txt")))
                                pwCheck = PasswordStrength(newPassword);
                            else
                            {
                                Console.WriteLine("New Password and Current Password cannot be same\nPlease Try Again.");
                                pwCheck = false;
                            }
                        }
                        File.WriteAllText(@"0:\username.txt", newName);
                        File.WriteAllText(@"0:\password.txt", newPassword);
                        Console.WriteLine("New Username: " + File.ReadAllText(@"0:\username.txt"));
                        Console.WriteLine("New Password: " + File.ReadAllText(@"0:\password.txt"));
                        Console.WriteLine("Press Any Key to Continue...");
                        Console.ReadKey(true);
                        break;
                    }
                case "d":
                    {
                        Console.Clear();
                        Console.WriteLine("Going Back...");
                        break;
                    }
            }
        }
        public static void SetCredentials(string newName, string newPassword)
        {
            try
            {
                var file_stream = File.Create(@"0:\username.txt");
                File.WriteAllText(@"0:\username.txt", newName);
                Console.WriteLine("Username Set Successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            try
            {
                var file_stream = File.Create(@"0:\password.txt");
                File.WriteAllText(@"0:\password.txt", newPassword);
                Console.WriteLine("Password Set Successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static string Date()
        {
            string day = Cosmos.HAL.RTC.DayOfTheMonth.ToString();
            string month = Cosmos.HAL.RTC.Month.ToString();
            int monthInt = int.Parse(month);
            month = DateTimeFormatInfo.CurrentInfo.GetMonthName(monthInt).ToString();
            string year = Cosmos.HAL.RTC.Year.ToString();
            string message = "Date today: " + month + " " + day + ", 20" + year;
            return message;
        }
        public static string Time()
        {
            string hour = Cosmos.HAL.RTC.Hour.ToString();
            string minute = Cosmos.HAL.RTC.Minute.ToString();
            string second = Cosmos.HAL.RTC.Second.ToString();
            string message = "Current time: " + hour + ":" + minute + ":" + second;
            return message;
        }
        public static void Calculator()
        {
            double aDbl = 0, bDbl = 0;
            bool validA = false, validB = false, validC = false;
            string a,b,c;
            while(!validA)
            {
                Console.Write("Enter First Number: ");
                a = Console.ReadLine();
                if (Double.TryParse(a, out aDbl) )
                {
                    validA = true;
                }
                else
                {
                    Console.WriteLine("Input a valid number.\nPlease Try Again!\n");
                }
            }
            while (!validB)
            {
                Console.Write("Enter Second Number: ");
                b = Console.ReadLine();
                if (Double.TryParse(b, out bDbl))
                {
                    validB = true;
                }
                else
                {
                    Console.WriteLine("Input a valid number.\nPlease Try Again!\n");
                }
            }
            while (!validC)
            {
                Console.Write("Enter Operation:\n\t[a] - Addition\n\t[b] - Subtraction\n\t[c] - Multiplication\n\t[d] - Division\n> ");
                c = Console.ReadLine();
                switch (c.ToLower())
                {
                    case "a":
                        Console.WriteLine("\t{0} + {1} = {2}", aDbl, bDbl, aDbl + bDbl);
                        validC = true;
                        break;
                    case "b":
                        Console.WriteLine("\t{0} - {1} = {2}", aDbl, bDbl, aDbl - bDbl);
                        validC = true;
                        break;
                    case "c":
                        Console.WriteLine("\t{0} * {1} = {2}", aDbl, bDbl, aDbl * bDbl);
                        validC = true;
                        break;
                    case "d":
                        Console.WriteLine("\t{0} / {1} = {2}", aDbl, bDbl, aDbl / bDbl);
                        validC = true;
                        break;
                    default:
                        Console.WriteLine("Invalid Input.\nPlease Try Again!\n");
                        break;
                }
            }
        }
        public static bool PasswordStrength(string password)
        {
            bool check = true;
            if (password.Length < 8)
            {
                Console.WriteLine("Password must be at least 8 characters\nPlease Try Again!");
                check = false;
            }
            else {
                if (!password.Any(char.IsLower))
                {
                    Console.WriteLine("Password must contain at least 1 small character\nPlease Try Again!\n");
                    check = false;
                }
                else if (!password.Any(char.IsUpper))
                {
                    Console.WriteLine("Password must contain at least 1 capital character\nPlease Try Again!\n");
                    check = false;
                }
                else if (!password.Any(char.IsDigit))
                {
                    Console.WriteLine("Password must contain at least 1 number\nPlease Try Again!\n");
                    check = false;
                }
                else if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                {
                    Console.WriteLine("Password must contain at least 1 special character\nPlease Try Again!\n");
                    check =  false;
                }
            }
            return check;
        }
    }
}
