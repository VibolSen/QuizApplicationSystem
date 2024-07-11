using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace QuizApplicationSystem
{
    public class QuizApp
    {
        private UserManager userManager;
        private QuizManager quizManager;
        private User currentUser;
        private readonly string adminUsername = "vibolsen";
        private readonly string adminPassword = "vibol123";

        public QuizApp()
        {
            userManager = new UserManager();
            quizManager = new QuizManager();
        }

        public void LoadAllResultsForCurrentUser()
        {
            if (currentUser != null)
            {
                var allResults = LoadResultsFromFile();
                currentUser.QuizResults = allResults.Where(r => r.UserName == currentUser.Username).ToList();
            }
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to Quiz Application");
                Console.WriteLine("===========================");
                Console.WriteLine();
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("0. Exit program");
                Console.WriteLine();
                Console.Write("Please Enter Your Choice: ");

                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid choice. Please enter a number between 0 and 2.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Login();
                        break;
                    case 2:
                        Register();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 0 and 2.");
                        break;
                }
            }
        }

        public void Login()
        {
            Console.Clear();
            Console.WriteLine("1. Login");
            Console.WriteLine("==============================================");
            Console.WriteLine();
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            if (username == adminUsername && password == adminPassword)
            {
                currentUser = new User(adminUsername, adminPassword, DateTime.Now); // Admin user
                AdminMenu();
            }
            else
            {
                currentUser = userManager.LoginUser(username, password);

                if (currentUser != null)
                {
                    LoadAllResultsForCurrentUser(); // Load results after login
                    UserMenu();
                }
                else
                {
                    Console.WriteLine("Invalid username or password.");
                    Console.Write("Press any key to continue... ");
                    Console.ReadKey();
                }
            }
        }

        public void Register()
        {
            Console.Clear();
            Console.WriteLine("2. Register");
            Console.WriteLine("==============================================");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            Console.Write("Enter date of birth (yyyy-mm-dd): ");
            DateTime dateOfBirth;
            if (!DateTime.TryParse(Console.ReadLine(), out dateOfBirth))
            {
                Console.WriteLine("Invalid date format.");
                Console.Write("Press any key to continue... ");
                Console.ReadKey();
                return;
            }

            if (userManager.RegisterUser(username, password, dateOfBirth))
                Console.WriteLine("Registration successful.");
            else
                Console.WriteLine("Username already exists.");

            Console.Write("Press any key to continue... ");
            Console.ReadKey();
        }

        public void Logout()
        {
            Console.Clear();
            currentUser = null;
            Console.WriteLine("Logged out successfully.");
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }

        public void AdminMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Admin Menu");
                Console.WriteLine("==============================================");
                Console.WriteLine();
                Console.WriteLine("1. Create Quiz");
                Console.WriteLine("2. View Quiz");
                Console.WriteLine("3. Edit Quiz");
                Console.WriteLine("4. Delete Quiz");
                Console.WriteLine("5. Logout");
                Console.WriteLine();
                Console.Write("Enter your choice: ");

                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        CreateQuiz();
                        break;
                    case 2:
                        ViewQuiz();
                        break;
                    case 3:
                        EditQuiz();
                        break;
                    case 4:
                        DeleteQuiz();
                        break;
                    case 5:
                        Logout();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.");
                        break;
                }
            }
        }

        public void CreateQuiz()
        {
            Console.Clear();
            Console.WriteLine("Create Quiz");
            Console.WriteLine("==============================================");
            Console.Write("Enter quiz title: ");
            string title = Console.ReadLine();
            var quiz = new Quiz(title);

            // Add questions to the quiz
            for (int i = 0; i < 5; i++)
            {
                Console.Write($"Enter question {i + 1}: ");
                string questionText = Console.ReadLine();
                var question = new Question(questionText);

                // Add answers to the question
                for (int j = 0; j < 3; j++)
                {
                    Console.Write($"Enter answer {j + 1}: ");
                    string answerText = Console.ReadLine();
                    var answer = new Answer(answerText);
                    question.Answers.Add(answer);
                }

                // Set correct answer index (1-3)
                Console.Write("Enter correct answer index (1-3): ");
                int correctIndex;
                while (!int.TryParse(Console.ReadLine(), out correctIndex) || correctIndex < 1 || correctIndex > 3)
                {
                    Console.WriteLine("Invalid input. Enter a number between 1 and 3.");
                }
                question.CorrectAnswerIndexes.Add(correctIndex - 1);

                // Add the question to the quiz
                quiz.Questions.Add(question);
            }

            // Save the quiz using QuizManager
            quizManager.AddQuiz(quiz);

            // Save quizzes to file after adding new quiz
            SaveQuizzesToFile("quizzes.txt");

            Console.WriteLine("Quiz created successfully.");
            Console.Write("Press any key to continue... ");
            Console.ReadKey();
        }

        public void ViewQuiz()
        {
            Console.Clear();
            Console.WriteLine("View Quiz");
            Console.WriteLine("==============================================");
            Console.WriteLine();
            foreach (var quiz in quizManager.GetQuizzes())
            {
                Console.WriteLine(quiz.Title);
            }
            Console.WriteLine();
            Console.Write("Select quiz title to view: ");

            string selectedQuizTitle = Console.ReadLine();
            var selectedQuiz = quizManager.GetQuizByTitle(selectedQuizTitle);

            if (selectedQuiz != null)
            {
                foreach (var question in selectedQuiz.Questions)
                {
                    Console.WriteLine(question.Text);
                    for (int i = 0; i < question.Answers.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {question.Answers[i].Text}");
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Quiz not found.");
            }

            Console.Write("Press any key to continue... ");
            Console.ReadKey();
        }

        public void EditQuiz()
        {
            Console.Clear();
            Console.WriteLine("Edit Quiz");
            Console.WriteLine("==============================================");
            Console.WriteLine("Enter quiz title to edit:");
            string title = Console.ReadLine();
            var quiz = quizManager.GetQuizByTitle(title);
            if (quiz != null)
            {
                // For simplicity, we allow editing only the title here. Expand as needed.
                Console.WriteLine("Enter new quiz title:");
                quiz.Title = Console.ReadLine();
                Console.WriteLine("Quiz edited successfully.");
            }
            else
            {
                Console.WriteLine("Quiz not found.");
            }

            Console.Write("Press any key to continue... ");
            Console.ReadKey();
        }

        public void DeleteQuiz()
        {
            Console.Clear();
            Console.WriteLine("Delete Quiz");
            Console.WriteLine("==============================================");
            Console.WriteLine("Enter quiz title to delete:");
            string title = Console.ReadLine();
            var quiz = quizManager.GetQuizByTitle(title);
            if (quiz != null)
            {
                quizManager.RemoveQuiz(quiz);
                Console.WriteLine("Quiz deleted successfully.");
            }
            else
            {
                Console.WriteLine("Quiz not found.");
            }

            Console.Write("Press any key to continue... ");
            Console.ReadKey();
        }

        public void UserMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("User Menu");
                Console.WriteLine("==============================================");
                Console.WriteLine();
                Console.WriteLine("1. Start Quiz");
                Console.WriteLine("2. View Results");
                Console.WriteLine("3. View Top Quiz");
                Console.WriteLine("4. Edit Settings");
                Console.WriteLine("5. Logout");
                Console.WriteLine();
                Console.Write("Enter your choice: ");

                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        StartQuiz();
                        break;
                    case 2:
                        ViewResults();
                        break;
                    case 3:
                        ViewTopQuiz();
                        break;
                    case 4:
                        EditSettings();
                        break;
                    case 5:
                        Logout();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.");
                        break;
                }
            }
        }

        public void StartQuiz()
        {
            Console.Clear();
            Console.WriteLine("Start Quiz");
            Console.WriteLine("==============================================");
            Console.WriteLine();

            foreach (var quiz in quizManager.GetQuizzes())
            {
                Console.WriteLine(quiz.Title);
            }

            Console.WriteLine();
            Console.Write("Please Select the subject quiz title to start: ");

            string selectedQuizTitle = Console.ReadLine();
            var selectedQuiz = quizManager.GetQuizByTitle(selectedQuizTitle);

            if (selectedQuiz != null)
            {
                var result = new QuizResult(currentUser.Username, selectedQuiz.Title);

                foreach (var question in selectedQuiz.Questions)
                {
                    Console.Clear();
                    Console.WriteLine("Sart Quiz");
                    Console.WriteLine("==============================================");
                    Console.WriteLine();
                    Console.WriteLine(selectedQuiz.Title);
                    Console.WriteLine();
                    Console.WriteLine(question.Text);
                    Console.WriteLine();
                    for (int i = 0; i < question.Answers.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {question.Answers[i].Text}");
                    }
                    Console.WriteLine();
                    Console.Write("Please enter your correct number: ");

                    int selectedAnswerIndex;
                    while (!int.TryParse(Console.ReadLine(), out selectedAnswerIndex) || selectedAnswerIndex < 1 || selectedAnswerIndex > question.Answers.Count)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid answer number.");
                    }

                    if (question.CorrectAnswerIndexes.Contains(selectedAnswerIndex - 1))
                    {
                        result.Score++;
                    }
                }

                result.TimeTaken = DateTime.Now.Subtract(result.Timestamp).Seconds;
                currentUser.QuizResults.Add(result);

                // Save results to file after completing quiz
                SaveResultsToFile("results.txt");

                Console.WriteLine($"Quiz completed. Your score: {result.Score}");
            }
            else
            {
                Console.WriteLine("Quiz not found.");
            }

            Console.WriteLine();
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }

        public void ViewResults()
        {
            Console.Clear();
            Console.WriteLine("View Results");
            Console.WriteLine("==============================================");
            if (currentUser.QuizResults.Count == 0)
            {
                Console.WriteLine("No results to display.");
            }
            else
            {
                foreach (var result in currentUser.QuizResults)
                {
                    Console.WriteLine($"Quiz: {result.QuizTitle}, Score: {result.Score}, Time Taken: {result.TimeTaken} seconds");
                }
            }

            Console.Write("Press any key to continue... ");
            Console.ReadKey();
        }

        public void ViewTopQuiz()
        {
            Console.Clear();
            Console.WriteLine("Top Quizzes");
            Console.WriteLine("==============================================");

            var allResults = LoadResultsFromFile();
            var topResults = allResults
                .GroupBy(r => r.QuizTitle)
                .Select(g => new { QuizTitle = g.Key, AverageScore = g.Average(r => r.Score) })
                .OrderByDescending(x => x.AverageScore)
                .Take(20)
                .ToList();

            if (topResults.Count == 0)
            {
                Console.WriteLine("No quiz results found.");
            }
            else
            {
                foreach (var topResult in topResults)
                {
                    Console.WriteLine($"Quiz: {topResult.QuizTitle}, Average Score: {topResult.AverageScore}");
                }
            }

            Console.Write("Press any key to continue... ");
            Console.ReadKey();
        }

        /*public void EditSettings()
        {
            Console.Clear();
            Console.WriteLine("Edit Settings");
            Console.WriteLine("==============================================");
            Console.Write("Enter new password: ");
            string newPassword = Console.ReadLine();
            currentUser.Password = newPassword;

            // Update user information in UserManager
            userManager.UpdateUser(currentUser);

            Console.WriteLine("Password updated successfully.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }*/

        public void EditSettings()
        {
            Console.Clear();
            Console.WriteLine("Edit Settings");
            Console.WriteLine("==============================================");

            // Verify current password
            Console.Write("Please enter your password to continue editing: ");
            string enteredPassword = Console.ReadLine();

            if (enteredPassword != currentUser.Password)
            {
                Console.WriteLine("Your password is incorrect.");
                Console.Write("Press any key to continue... ");
                Console.ReadKey();
                return;
            }

            // Provide options for editing user information
            Console.WriteLine("Which part do you want to update?");
            Console.WriteLine("1. Password");
            Console.WriteLine("2. Username");
            Console.WriteLine("3. Date of birth");
            Console.Write("Please enter the number to update: ");

            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
            {
                Console.WriteLine("Invalid choice. Please enter a number between 1 and 3.");
                Console.Write("Press any key to continue... ");
                Console.ReadKey();
                return;
            }

            switch (choice)
            {
                case 1:
                    Console.Write("Enter new password: ");
                    string newPassword = Console.ReadLine();
                    currentUser.Password = newPassword;
                    break;
                case 2:
                    Console.Write("Enter new username: ");
                    string newUsername = Console.ReadLine();
                    currentUser.Username = newUsername;
                    break;
                case 3:
                    Console.Write("Enter new date of birth (yyyy-mm-dd): ");
                    DateTime newDateOfBirth;
                    if (DateTime.TryParse(Console.ReadLine(), out newDateOfBirth))
                    {
                        currentUser.DateOfBirth = newDateOfBirth;
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format.");
                        Console.Write("Press any key to continue... ");
                        Console.ReadKey();
                        return;
                    }
                    break;
            }

            // Update user information in UserManager
            userManager.UpdateUser(currentUser);

            Console.WriteLine("Information updated successfully.");
            Console.Write("Press any key to continue... ");
            Console.ReadKey();
        }


        private void SaveQuizzesToFile(string fileName)
        {
            try
            {
                string json = JsonSerializer.Serialize(quizManager.GetQuizzes());
                File.WriteAllText(fileName, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving quizzes to file: {ex.Message}");
            }
        }

        private void SaveResultsToFile(string fileName)
        {
            try
            {
                var allResults = LoadResultsFromFile();
                allResults.AddRange(currentUser.QuizResults);

                string json = JsonSerializer.Serialize(allResults);
                File.WriteAllText(fileName, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving results to file: {ex.Message}");
            }
        }

        private List<QuizResult> LoadResultsFromFile()
        {
            try
            {
                if (File.Exists("results.txt"))
                {
                    string json = File.ReadAllText("results.txt");
                    return JsonSerializer.Deserialize<List<QuizResult>>(json) ?? new List<QuizResult>();
                }
                return new List<QuizResult>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading results from file: {ex.Message}");
                return new List<QuizResult>();
            }
        }
    }
}
