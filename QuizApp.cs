using System;
using System.Collections.Generic;
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
                return;
            }

            if (userManager.RegisterUser(username, password, dateOfBirth))
                Console.WriteLine("Registration successful.");
            else
                Console.WriteLine("Username already exists.");
        }

        public void Login()
        {
            Console.Clear();
            Console.WriteLine("1. Login");
            Console.WriteLine("==============================================");
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
                    UserMenu();
                else
                    Console.WriteLine("Invalid username or password.");
            }
        }

        public void Logout()
        {
            Console.Clear();
            currentUser = null;
            Console.WriteLine("Logged out successfully.");
        }

        //test staty quiz
        public void StartQuiz()
        {
            Console.Clear();
            Console.WriteLine("1. Start Quiz");
            Console.WriteLine("==============================================");
            Console.Write("Select quiz title: ");
            foreach (var quiz in quizManager.GetQuizzes())
                Console.WriteLine(quiz.Title);

            string selectedQuizTitle = Console.ReadLine();
            var selectedQuiz = quizManager.GetQuizByTitle(selectedQuizTitle);

            if (selectedQuiz != null)
            {
                int correctAnswers = 0;
                foreach (var question in selectedQuiz.Questions)
                {
                    Console.WriteLine(question.Text);
                    for (int i = 0; i < question.Answers.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {question.Answers[i].Text}");
                    }

                    Console.Write("Enter your answer(s) (comma separated):  ");
                    var userAnswers = Console.ReadLine().Split(',').Select(answer =>
                    {
                        return int.TryParse(answer.Trim(), out int parsedAnswer) ? parsedAnswer : -1;
                    }).ToList();

                    if (question.CheckAnswer(userAnswers))
                        correctAnswers++;
                }

                var result = new QuizResult(selectedQuiz, correctAnswers, DateTime.Now);
                currentUser.QuizResults.Add(result);
                Console.WriteLine($"You answered {correctAnswers} out of {selectedQuiz.Questions.Count} correctly.");

                // Save the result to file
                SaveResultToFile(result);
            }
            else
            {
                Console.WriteLine("Quiz not found.");
            }
        }

        // Update
        public void ViewResults()
        {
            Console.Clear();
            Console.WriteLine("2. View Result");
            Console.WriteLine("==============================================");
            Console.WriteLine();
            var results = LoadResultsFromFile();

            if (results.Any())
            {
                foreach (var result in results)
                {
                    Console.WriteLine($"Quiz: {result.Quiz.Title}, Correct Answers: {result.CorrectAnswers}, Date: {result.DateTaken}");
                }
            }
            else
            {
                Console.WriteLine("No results found.");
            }
        }
        public void ViewTop10()
        {
            Console.WriteLine("Select quiz title to view top 10 results:");
            foreach (var quiz in quizManager.GetQuizzes())
                Console.WriteLine(quiz.Title);

            string selectedQuizTitle = Console.ReadLine();
            var selectedQuiz = quizManager.GetQuizByTitle(selectedQuizTitle);

            if (selectedQuiz != null)
            {
                var topResults = userManager.GetUsers()
                    .SelectMany(u => u.QuizResults)
                    .Where(r => r.Quiz.Title == selectedQuiz.Title)
                    .OrderByDescending(r => r.CorrectAnswers)
                    .Take(10);

                foreach (var result in topResults)
                {
                    Console.WriteLine($"{result.CorrectAnswers} correct answers by {result.Quiz.Title} on {result.DateTaken}");
                }
            }
            else
            {
                Console.WriteLine("Quiz not found.");
            }
        }

        // Add view all result
        public void ViewAllResults()
        {
            Console.Clear();
            Console.WriteLine("View All Results");
            Console.WriteLine("==============================================");

            var resultFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*_results.txt");

            if (!resultFiles.Any())
            {
                Console.WriteLine("No results found.");
                return;
            }

            foreach (var file in resultFiles)
            {
                var username = Path.GetFileNameWithoutExtension(file).Replace("_results", "");
                Console.WriteLine($"Results for user: {username}");
                Console.WriteLine("----------------------------");

                try
                {
                    var lines = File.ReadAllLines(file);
                    for (int i = 0; i < lines.Length; i += 4)
                    {
                        if (i + 3 < lines.Length &&
                            lines[i].StartsWith("Quiz:") &&
                            lines[i + 1].StartsWith("Correct Answers:") &&
                            lines[i + 2].StartsWith("Date Taken:"))
                        {
                            var quizTitle = lines[i].Substring(lines[i].IndexOf(":") + 2).Trim();
                            var correctAnswers = int.Parse(lines[i + 1].Substring(lines[i + 1].IndexOf(":") + 2).Trim());
                            var dateTaken = DateTime.Parse(lines[i + 2].Substring(lines[i + 2].IndexOf(":") + 2).Trim());
                            var quiz = quizManager.GetQuizByTitle(quizTitle);

                            if (quiz != null)
                            {
                                Console.WriteLine($"Quiz: {quizTitle}");
                                Console.WriteLine($"Correct Answers: {correctAnswers}");
                                Console.WriteLine($"Date Taken: {dateTaken}");
                                Console.WriteLine();
                            }
                            else
                            {
                                Console.WriteLine($"Quiz '{quizTitle}' not found in quiz manager.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Unexpected format in file: {file} at line: {i}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading results for user '{username}': {ex.Message}");
                }
            }
        }

        //test
        public void ChangeSettings()
        {
            // Display current user data in a table format
            Console.Clear();
            Console.WriteLine("4. Change Setting");
            Console.WriteLine("==============================================");
            Console.WriteLine("Current User Data:");
            Console.WriteLine("------------------");
            Console.WriteLine($"Username: {currentUser.Username}");
            Console.WriteLine($"Password: {currentUser.Password}");
            Console.WriteLine($"Date of Birth: {currentUser.DateOfBirth:yyyy-MM-dd}");
            Console.WriteLine("------------------");

            // Ask the user which data they want to change
            Console.Write("Which one do you want to change? (username/password/date of birth): ");
            string changeChoice = Console.ReadLine().ToLower();

            switch (changeChoice)
            {
                case "username":
                    Console.Write("Enter new username: ");
                    string newUsername = Console.ReadLine();
                    Console.Write($"Are you sure you want to change your username to {newUsername}? (Y/N) ");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        currentUser.Username = newUsername;
                        Console.WriteLine("Username changed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Username change canceled.");
                    }
                    break;

                case "password":
                    Console.Write("Enter new password: ");
                    string newPassword = Console.ReadLine();
                    Console.Write($"Are you sure you want to change your password to {newPassword}? (Y/N) ");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        userManager.ChangePassword(currentUser, newPassword);
                        Console.WriteLine("Password changed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Password change canceled.");
                    }
                    break;

                case "date of birth":
                    Console.Write("Enter new date of birth (yyyy-mm-dd): ");
                    DateTime newDateOfBirth;
                    if (DateTime.TryParse(Console.ReadLine(), out newDateOfBirth))
                    {
                        Console.Write($"Are you sure you want to change your date of birth to {newDateOfBirth:yyyy-MM-dd}? (Y/N) ");
                        if (Console.ReadLine().ToLower() == "y")
                        {
                            userManager.ChangeDateOfBirth(currentUser, newDateOfBirth);
                            Console.WriteLine("Date of birth changed successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Date of birth change canceled.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format.");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        public void CreateQuiz()
        {
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

                // Set correct answer index (1-4)
                Console.Write("Enter correct answer index (1-3): ");
                int correctIndex;
                while (!int.TryParse(Console.ReadLine(), out correctIndex) || correctIndex < 1 || correctIndex > 4)
                {
                    Console.WriteLine("Invalid input. Enter a number between 1 and 4.");
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
        }

        //test
        public void LoadQuizzesFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var quizData = File.ReadAllText(filePath);
                    var loadedQuizzes = JsonSerializer.Deserialize<List<Quiz>>(quizData);
                    quizManager = new QuizManager();
                    foreach (var quiz in loadedQuizzes)
                    {
                        quizManager.AddQuiz(quiz);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading quizzes: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("No quiz data file found.");
            }
        }

        //update view quiz
        public void ViewQuiz()
        {
            Console.WriteLine("Select quiz title to view:");
            foreach (var quiz in quizManager.GetQuizzes())
            {
                Console.WriteLine(quiz.Title);
            }

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
        }

        public void EditQuiz()
        {
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
        }

        public void DeleteQuiz()
        {
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
        }

        public void AdminMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Admin Menu");
                Console.WriteLine("==============================================");
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

        public void UserMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("User Menu");
                Console.WriteLine("==============================================");
                Console.WriteLine("1. Start Quiz");
                Console.WriteLine("2. View Results");
                Console.WriteLine("3. View Top Quiz");
                Console.WriteLine("4. View All Results");
                Console.WriteLine("5. Edit Settings");
                Console.WriteLine("6. Logout");
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
                        ViewTop10();
                        break;
                    case 4:
                        ViewAllResults();
                        break;
                    case 5:
                        ChangeSettings();
                        break;
                    case 6:
                        Logout();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.");
                        break;
                }
            }
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to Quiz Application");
                Console.WriteLine("===========================");
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

        private void SaveQuizzesToFile(string filePath)
        {
            try
            {
                var quizData = JsonSerializer.Serialize(quizManager.GetQuizzes());
                File.WriteAllText(filePath, quizData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving quizzes: " + ex.Message);
            }
        }

        private void SaveResultToFile(QuizResult result)
        {
            string resultFile = $"{currentUser.Username}_results.txt";
            using (StreamWriter writer = new StreamWriter(resultFile, true))
            {
                writer.WriteLine($"Quiz: {result.Quiz.Title}");
                writer.WriteLine($"Correct Answers: {result.CorrectAnswers}");
                writer.WriteLine($"Date Taken: {result.DateTaken}");
                writer.WriteLine();
            }
        }

        private List<QuizResult> LoadResultsFromFile()
        {
            string resultFile = $"{currentUser.Username}_results.txt";
            List<QuizResult> results = new List<QuizResult>();

            if (File.Exists(resultFile))
            {
                using (StreamReader reader = new StreamReader(resultFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("Quiz:"))
                        {
                            string quizTitle = line.Substring(line.IndexOf(":") + 2).Trim();
                            int correctAnswers = int.Parse(reader.ReadLine().Substring(line.IndexOf(":") + 2).Trim());
                            DateTime dateTaken = DateTime.Parse(reader.ReadLine().Substring(line.IndexOf(":") + 2).Trim());
                            Quiz quiz = quizManager.GetQuizByTitle(quizTitle);
                            results.Add(new QuizResult(quiz, correctAnswers, dateTaken));
                        }
                    }
                }
            }

            return results;
        }
    }
}