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
            Console.WriteLine("Enter username:");
            string username = Console.ReadLine();
            Console.WriteLine("Enter password:");
            string password = Console.ReadLine();
            Console.WriteLine("Enter date of birth (yyyy-mm-dd):");
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
            Console.WriteLine("Enter username:");
            string username = Console.ReadLine();
            Console.WriteLine("Enter password:");
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
            currentUser = null;
            Console.WriteLine("Logged out successfully.");
        }

        //test staty quiz
        public void StartQuiz()
        {
            Console.WriteLine("Select quiz title:");
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

                    Console.WriteLine("Enter your answer(s) (comma separated):");
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
            }
            else
            {
                Console.WriteLine("Quiz not found.");
            }
        }

        public void ViewResults()
        {
            foreach (var result in currentUser.QuizResults)
            {
                Console.WriteLine($"Quiz: {result.Quiz.Title}, Correct Answers: {result.CorrectAnswers}, Date: {result.DateTaken}");
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

        public void ChangeSettings()
        {
            Console.WriteLine("Change password (enter new password or leave blank to skip):");
            string newPassword = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                userManager.ChangePassword(currentUser, newPassword);
                Console.WriteLine("Password changed successfully.");
            }

            Console.WriteLine("Change date of birth (enter new date of birth (yyyy-mm-dd) or leave blank to skip):");
            string newDateOfBirth = Console.ReadLine();
            DateTime dateOfBirth;
            if (!string.IsNullOrWhiteSpace(newDateOfBirth) && DateTime.TryParse(newDateOfBirth, out dateOfBirth))
            {
                userManager.ChangeDateOfBirth(currentUser, dateOfBirth);
                Console.WriteLine("Date of birth changed successfully.");
            }
        }

        public void CreateQuiz()
        {
            Console.WriteLine("Enter quiz title:");
            string title = Console.ReadLine();
            var quiz = new Quiz(title);

            // Add questions to the quiz
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"Enter question {i + 1}:");
                string questionText = Console.ReadLine();
                var question = new Question(questionText);

                // Add answers to the question
                for (int j = 0; j < 4; j++)
                {
                    Console.WriteLine($"Enter answer {j + 1}:");
                    string answerText = Console.ReadLine();
                    var answer = new Answer(answerText);
                    question.Answers.Add(answer);
                }

                // Set correct answer index (1-4)
                Console.WriteLine("Enter correct answer index (1-4):");
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
                Console.WriteLine("1. Create Quiz");
                Console.WriteLine("2. View Quiz");
                Console.WriteLine("3. Edit Quiz");
                Console.WriteLine("4. Delete Quiz");
                Console.WriteLine("5. Logout");

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
                Console.WriteLine("1. Start Quiz");
                Console.WriteLine("2. View Results");
                Console.WriteLine("3. View Top Quiz");
                Console.WriteLine("4. Edit Settings");
                Console.WriteLine("5. Logout");

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
                        ChangeSettings();
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

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("Welcome to Quiz Application");
                Console.WriteLine("====================");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("0. Exit program");

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
    }
}