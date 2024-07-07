using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplicationSystem
{
    public class QuizApplication
    {
        private UserManager userManager = new UserManager();
        private QuizManager quizManager = new QuizManager();
        private User currentUser;

        public void Start()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Welcome to the Quiz Application!");
            Console.WriteLine("==============================================");
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1. Login\n2. Register\n0. Exit");
                Console.WriteLine();
                Console.Write("Please Enter your choice: ");
                var choice = Console.ReadLine();
                if (choice == "1") Login();
                else if (choice == "2") Register();
                else if (choice == "0") break;
            }
        }

        private void Login()
        {
            Console.Clear();
            Console.WriteLine("1. Login");
            Console.WriteLine("==============================================");
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();
            Console.WriteLine();

            var user = userManager.AuthenticateUser(username, password);
            if (user != null)
            {
                currentUser = user;
                Console.WriteLine("Login successful!");
                Console.WriteLine();
                if (username == "vibolsen")
                {
                    AdminMenu();
                }
                else
                {
                    UserMenu();
                }
            }
            else
            {
                Console.WriteLine("Login failed your username or password incorrect! try again.");
                Console.WriteLine();
            }
        }

        private void Register()
        {
            Console.Clear();
            Console.WriteLine("2. Register");
            Console.WriteLine("==============================================");
            Console.WriteLine();
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();
            Console.Write("Date of Birth (yyyy-mm-dd): ");
            var dateOfBirth = DateTime.Parse(Console.ReadLine());

            if (userManager.RegisterUser(username, password, dateOfBirth))
            {
                Console.WriteLine("Registration successful! You can now login.");
            }
            else
            {
                Console.WriteLine("Username already exists.");
            }
        }

        private void AdminMenu()
        {
            //Console.Clear ();
            while (true)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("Admin Menu");
                Console.WriteLine("==============================================");
                Console.WriteLine();
                Console.WriteLine("1. Create Quiz\n2. Edit Quiz\n3. Delete Quiz\n4. View Quiz\n5. Logout");
                Console.WriteLine();
                Console.Write("Please Enter your choice: ");
                var choice = Console.ReadLine();
                if (choice == "1") CreateQuiz();
                else if (choice == "2") EditQuiz();
                else if (choice == "3") DeleteQuiz();
                else if (choice == "4") ViewQuiz();
                else if (choice == "5") { currentUser = null; break; }
            }
        }

        private void CreateQuiz()
        {
            var quizUtility = new QuizUtility();
            quizUtility.CreateQuiz("vibolsen", "admin123");
        }

        private void ViewQuiz()
        {
            var quizUtility = new QuizUtility();
            quizUtility.ViewQuiz("vibolsen", "admin123");
        }

        private void EditQuiz()
        {
            var quizUtility = new QuizUtility();
            quizUtility.EditQuiz("vibolsen", "admin123");
        }

        private void DeleteQuiz()
        {
            var quizUtility = new QuizUtility();
            quizUtility.DeleteQuiz("vibolsen", "admin123");
        }

        private void PrintQuiz(Quiz quiz)
        {
            Console.WriteLine($"Quiz Name: {quiz.QuizName}");
            foreach (var question in quiz.Questions)
            {
                Console.WriteLine($"Question: {question.QuestionText}");
                for (int i = 0; i < question.Options.Count; i++)
                {
                    Console.WriteLine($"Option {i + 1}: {question.Options[i]}");
                }
            }
        }

        private void UserMenu()
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("User Menu");
                Console.WriteLine("==============================================");
                Console.WriteLine();
                Console.WriteLine("1. Start Quiz\n2. View Results\n3. View Top 20\n4. Edit Settings\n5. Logout");
                Console.WriteLine();
                Console.Write("Please Enter your choice: ");
                var choice = Console.ReadLine();
                if (choice == "1") StartQuiz();
                else if (choice == "2") ViewResults();
                else if (choice == "3") ViewTop20();
                else if (choice == "4") EditSettings();
                else if (choice == "5") { currentUser = null; break; }
            }
        }

        private void StartQuiz()
        {
            Console.WriteLine("Select a quiz:");
            var quizzes = quizManager.ListAllQuizzes();
            if (quizzes.Count == 0)
            {
                Console.WriteLine("No quizzes available.");
                return;
            }

            for (int i = 0; i < quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {quizzes[i].QuizName}");
            }
            Console.WriteLine($"{quizzes.Count + 1}. Mixed Quiz");

            int choice;
            while (true)
            {
                Console.Write("Enter your choice: ");
                if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= quizzes.Count + 1)
                {
                    choice -= 1;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }

            Quiz selectedQuiz;
            if (choice == quizzes.Count)
            {
                selectedQuiz = quizManager.GenerateMixedQuiz();
            }
            else
            {
                selectedQuiz = quizzes[choice];
            }

            int correctAnswers = 0;
            foreach (var question in selectedQuiz.Questions)
            {
                Console.WriteLine(question.QuestionText);
                for (int i = 0; i < question.Options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {question.Options[i]}");
                }

                Console.Write("Enter your answers (comma-separated): ");
                List<int> answer;
                while (true)
                {
                    try
                    {
                        answer = Console.ReadLine().Split(',').Select(int.Parse).ToList();
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Invalid input. Please enter numbers separated by commas.");
                    }
                }

                if (answer.OrderBy(a => a).SequenceEqual(question.CorrectAnswers.OrderBy(a => a)))
                {
                    correctAnswers++;
                    Console.WriteLine("Correct!");
                }
                else
                {
                    Console.WriteLine("Incorrect.");
                }
                Console.WriteLine();
            }

            Console.WriteLine($"You answered {correctAnswers} out of {selectedQuiz.Questions.Count} questions correctly.");
            SaveQuizResult(currentUser.Username, selectedQuiz.QuizName, correctAnswers);
        }


        private Dictionary<string, List<(string quizName, int score)>> userQuizResults = new Dictionary<string, List<(string quizName, int score)>>();

        private void SaveQuizResult(string username, string quizName, int score)
        {
            if (!userQuizResults.ContainsKey(username))
            {
                userQuizResults[username] = new List<(string quizName, int score)>();
            }
            userQuizResults[username].Add((quizName, score));

            Console.WriteLine($"Quiz result saved for {username}: Quiz - {quizName}, Score - {score}");
        }

        private void ViewResults()
        {
            var results = GetUserQuizResults(currentUser.Username);
            Console.WriteLine("Your Previous Quiz Results:");
            foreach (var result in results)
            {
                Console.WriteLine($"Quiz: {result.quizName}, Score: {result.score}");
            }
        }

        private List<(string quizName, int score)> GetUserQuizResults(string username)
        {
            if (userQuizResults.ContainsKey(username))
            {
                return userQuizResults[username];
            }
            else
            {
                return new List<(string quizName, int score)>();
            }
        }

        private void ViewTop20()
        {
            Console.WriteLine("Select a quiz:");
            var quizzes = quizManager.ListAllQuizzes();
            for (int i = 0; i < quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {quizzes[i].QuizName}");
            }

            var choice = int.Parse(Console.ReadLine()) - 1;
            var selectedQuiz = quizzes[choice];

            var topScores = userManager.ListTop20Scores(selectedQuiz);
            Console.WriteLine("Top 20 Scores:");
            foreach (var (username, score) in topScores)
            {
                Console.WriteLine($"{username}: {score}");
            }
        }

        private void EditSettings()
        {
            Console.Write("New Password: ");
            var newPassword = Console.ReadLine();
            Console.Write("New Date of Birth (yyyy-mm-dd): ");
            var newDateOfBirth = DateTime.Parse(Console.ReadLine());

            if (userManager.EditUserSettings(currentUser.Username, newPassword, newDateOfBirth))
            {
                Console.WriteLine("Settings updated successfully.");
            }
            else
            {
                Console.WriteLine("Error updating settings.");
            }
        }
    }
}