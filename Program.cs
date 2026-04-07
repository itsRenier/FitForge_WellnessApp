using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitForge_WellnessApp
{
    // Structure to store data for each fitness member
    public struct Member
    {
        public string Name;
        public int Age;
        public DateTime JoinDate;
        public int MembershipRank;
        public double TreadmillDistance;
        public bool Employed;
        public bool GuardianEmployed;
        public int SmoothiesConsumed;
        public string FavouriteSmoothie;
    }

    internal class Program
    {
        // Enum to represent user menu choices
        enum Menu
        {
            CaptureMemberDetails = 1,
            CheckWellnessRewardQualification,
            ShowFitForgeStats,
            SearchMember,
            SmoothieSuggestion,
            Exit
        }

        // Lists to store all members and their eligibility status
        static List<Member> allMembers = new List<Member>();
        static List<Member> qualifiedMembers = new List<Member>();
        static List<Member> unqualifiedMembers = new List<Member>();

        // Entry point of the application, handles  the menu interaction
        static void Main(string[] args)
        {
            Menu choice;

            do
            {
                Console.Clear();
                Console.WriteLine("=== FitForge Wellness App ===");
                Console.WriteLine("1. Capture Member Details");
                Console.WriteLine("2. Check Wellness Reward Qualification");
                Console.WriteLine("3. Show FitForge Stats");
                Console.WriteLine("4. Search Member");
                Console.WriteLine("5. Get Smoothie Suggestion Based on Weather");
                Console.WriteLine("6. Exit");
                Console.Write("Enter your choice (1-6): ");

                string userInput = Console.ReadLine();

                // Safely parse user input into a valid enum value
                if (Enum.TryParse(userInput, out choice) && Enum.IsDefined(typeof(Menu), choice))
                {
                    // Perform selected operation based on user input
                    switch (choice)
                    {
                        case Menu.CaptureMemberDetails:
                            CaptureMemberDetails();
                            break;

                        case Menu.CheckWellnessRewardQualification:
                            // Check members' eligibility and update respective lists
                            AssessEligibility(allMembers, out qualifiedMembers, out unqualifiedMembers);
                            Console.WriteLine("Eligibility check completed.");
                            Console.ReadKey();
                            break;

                        case Menu.ShowFitForgeStats:
                            // Display statistics of qualified vs unqualified members
                            ShowFitForgeStats();
                            Console.ReadKey();
                            break;

                        case Menu.SearchMember:
                            // Search for a member by name
                            SearchMember();
                            Console.ReadKey();
                            break;

                        case Menu.SmoothieSuggestion:
                            // Suggest smoothie based on current temperature
                            SmoothieSuggestion();
                            Console.ReadKey();
                            break;

                        case Menu.Exit:
                            Console.WriteLine("Exiting...");
                            break;
                    }
                }
                else
                {
                    // Handles invalid input
                    Console.WriteLine("Invalid input. Enter a number between 1 and 6.");
                    Console.ReadKey();
                }

            } while (choice != Menu.Exit);
        }

        // Method to capture new member details from user input
        public static void CaptureMemberDetails()
        {
            string another = "Y";

            // Allow user to add multiple members
            while (another.ToUpper() == "Y")
            {
                Member m = new Member();

                Console.Clear();
                Console.WriteLine("=== Enter Member Details ===");

                // Name input
                Console.Write("Name: ");
                m.Name = Console.ReadLine();

                // Age input with validation
                while (true)
                {
                    Console.Write("Age: ");

                    if (int.TryParse(Console.ReadLine(), out m.Age) && m.Age > 0)
                        break;
                    Console.WriteLine("Invalid age. Please enter a valid positive number.");
                }

                // Date joined input with format validation
                while (true)
                {
                    Console.Write("Join Date (dd/MM/yyyy): ");

                    if (DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out m.JoinDate))
                        break;
                    Console.WriteLine("Invalid date. Use format dd/MM/yyyy.");
                }

                // Membership rank input with validation
                while (true)
                {
                    Console.Write("Membership Rank: ");

                    if (int.TryParse(Console.ReadLine(), out m.MembershipRank) && m.MembershipRank >= 0)
                        break;
                    Console.WriteLine("Invalid rank. Please enter a non-negative number.");
                }

                // Treadmill distance input with validation
                while (true)
                {
                    Console.Write("Personal Best Treadmill Distance (km): ");

                    if (double.TryParse(Console.ReadLine(), out m.TreadmillDistance) && m.TreadmillDistance >= 0)
                        break;
                    Console.WriteLine("Invalid distance. Please enter a non-negative number.");
                }

                // Employment status input
                while (true)
                {
                    Console.Write("Are you currently employed? (Y/N): ");

                    string input = Console.ReadLine().ToUpper();

                    if (input == "Y") { m.Employed = true; break; }
                    if (input == "N") { m.Employed = false; break; }

                    Console.WriteLine("Please enter Y or N.");
                }

                // Guardian employment for members under 18
                if (m.Age < 18)
                {
                    while (true)
                    {
                        Console.Write("Is your guardian employed? (Y/N): ");

                        string input = Console.ReadLine().ToUpper();

                        if (input == "Y") { m.GuardianEmployed = true; break; }
                        if (input == "N") { m.GuardianEmployed = false; break; }

                        Console.WriteLine("Please enter Y or N.");
                    }
                }

                // Smoothies consumed input with validation
                while (true)
                {
                    Console.Write("How many smoothies have you consumed since joining? ");

                    if (int.TryParse(Console.ReadLine(), out m.SmoothiesConsumed) && m.SmoothiesConsumed >= 0)
                        break;
                    Console.WriteLine("Invalid number. Please enter a valid non-negative amount.");
                }

                // Favorite smoothie input
                Console.Write("Favourite Smoothie Flavour: ");
                m.FavouriteSmoothie = Console.ReadLine();

                // Add member to the master list
                allMembers.Add(m);
                Console.WriteLine("Member added successfully!");

                // Ask if user wants to add another member
                Console.Write("Do you want to enter another member? (Y/N): ");
                another = Console.ReadLine().ToUpper();
            }
        }

        // Evaluate each member and determine wellness reward eligibility
        public static void AssessEligibility(List<Member> members, out List<Member> qualified, out List<Member> unqualified)
        {
            qualified = new List<Member>();
            unqualified = new List<Member>();

            foreach (var member in members)
            {
                bool isQualified = true;

                // Check employment based on age
                if (member.Age >= 18)
                {
                    if (!member.Employed) isQualified = false;
                }
                else
                {
                    if (!member.GuardianEmployed) isQualified = false;
                }

                // Check if member has been active for at least 24 months
                int months = ((DateTime.Now.Year - member.JoinDate.Year) * 12) + (DateTime.Now.Month - member.JoinDate.Month);
                if (months < 24) isQualified = false;

                // Evaluate fitness score
                double avgFitness = (member.MembershipRank + member.TreadmillDistance) / 2.0;
                if (!(member.MembershipRank > 2000 || member.TreadmillDistance > 20 || avgFitness > 2000))
                    isQualified = false;

                // Check smoothie consumption rate (should be between 4 and 20 per month)
                if (months > 0)
                {
                    double smoothiesPerMonth = (double)member.SmoothiesConsumed / months;
                    if (smoothiesPerMonth < 4 || smoothiesPerMonth > 20) isQualified = false;
                }
                else
                {
                    isQualified = false;
                }

                // Special disqualification for specific smoothie preference
                if (member.FavouriteSmoothie.Trim().ToUpper() == "CHOCOCHURNED CHAOS")
                    isQualified = false;

                // Add member to respective list and print result
                if (isQualified)
                {
                    qualified.Add(member);
                    Console.WriteLine($"{member.Name} has qualified for the wellness reward.");
                }
                else
                {
                    unqualified.Add(member);
                    Console.WriteLine($"{member.Name} did not qualify for the wellness reward.");
                }
            }
        }

        // Display stats summary for all processed members
        public static void ShowFitForgeStats()
        {
            int total = qualifiedMembers.Count + unqualifiedMembers.Count;

            if (total == 0)
            {
                Console.WriteLine("No eligibility results available. Please assess qualifications first.");
                return;
            }

            Console.WriteLine("=== FitForge Wellness Stats ===");
            Console.WriteLine($"Total Applicants: {total}");
            Console.WriteLine($"Qualified Members: {qualifiedMembers.Count}");
            Console.WriteLine($"Unqualified Members: {unqualifiedMembers.Count}");

            // Calculate percentage values
            double qualifiedPercentage = Math.Round((double)qualifiedMembers.Count / total * 100, 2);
            double unqualifiedPercentage = Math.Round((double)unqualifiedMembers.Count / total * 100, 2);

            Console.WriteLine($"Qualified Percentage: {qualifiedPercentage}%");
            Console.WriteLine($"Unqualified Percentage: {unqualifiedPercentage}%");
        }

        // Search for members by name and show their profile
        public static void SearchMember()
        {
            Console.Clear();
            Console.Write("Enter name or part of name to search: ");

            string search = Console.ReadLine().ToLower();

            // Find all members whose name includes the search term
            var results = allMembers.FindAll(m => m.Name.ToLower().Contains(search));

            if (results.Count == 0)
            {
                Console.WriteLine("No members found with that name.");
                return;
            }

            // Display detailed info about matched members
            foreach (var m in results)
            {
                int months = ((DateTime.Now.Year - m.JoinDate.Year) * 12) + (DateTime.Now.Month - m.JoinDate.Month);
                double smoothiesPerMonth = months > 0 ? Math.Round((double)m.SmoothiesConsumed / months, 2) : 0;

                Console.WriteLine($"\n--- Profile for {m.Name} ---");
                Console.WriteLine($"Age: {m.Age}");
                Console.WriteLine($"Join Date: {m.JoinDate:dd/MM/yyyy} ({months} months ago)");
                Console.WriteLine($"Rank: {m.MembershipRank}");
                Console.WriteLine($"Treadmill PB: {m.TreadmillDistance} km");
                Console.WriteLine($"Employed: {(m.Employed ? "Yes" : "No")}");
                if (m.Age < 18)
                    Console.WriteLine($"Guardian Employed: {(m.GuardianEmployed ? "Yes" : "No")}");
                Console.WriteLine($"Smoothies Consumed: {m.SmoothiesConsumed}");
                Console.WriteLine($"Smoothies/Month: {smoothiesPerMonth}");
                Console.WriteLine($"Favourite Smoothie: {m.FavouriteSmoothie}");

                // Show wellness tips based on smoothie habits
                if (smoothiesPerMonth > 15)
                    Console.WriteLine("Wellness Tip: Consider reducing sugar intake from smoothies.");
                else if (smoothiesPerMonth < 4)
                    Console.WriteLine("Wellness Tip: Try to stay more consistent with your smoothie nutrition!");
            }
        }

        // Suggest a smoothie based on the temperature entered by user
        public static void SmoothieSuggestion()
        {
            Console.Clear();
            Console.WriteLine("=== Smoothie Suggestion Generator ===");

            int temp;

            // Validate temperature input
            while (true)
            {
                Console.Write("Enter the current temperature in °C: ");
                if (int.TryParse(Console.ReadLine(), out temp))
                    break;
                Console.WriteLine("Please enter a valid number.");
            }

            // Suggest smoothies based on weather
            if (temp > 25)
            {
                Console.WriteLine("It's a hot day! We recommend the Minty Melon Refresher.");
            }
            else if (temp < 15)
            {
                Console.WriteLine("Chilly today! Try a warm Spiced Banana Boost.");
            }
            else
            {
                Console.WriteLine("Mild weather! A Berry Balance Blend is perfect.");
            }
        }
    }
}
