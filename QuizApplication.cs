using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplicationSystem
{
    public class QuizApplication
    {
        private UserManager userManager = new UserManager(); // Manages user authentication and registration
        private QuizManager quizManager = new QuizManager(); // Manages quizzes
        private User currentUser; // Stores the currently logged in user

        public void Start()
        {
            // Clears the console and displays the welcome message
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Welcome to the Quiz Application!");
            Console.WriteLine("==============================================");

            while (true)
            {
                // Display login and registration options
                Console.WriteLine();
                Console.WriteLine("1. Login\n2. Register\n0. Exit");
                Console.WriteLine();
                Console.Write("Please Enter your choice: ");
                var choice = Console.ReadLine();

                // Perform actions based on user choice
                if (choice == "1") Login();
                else if (choice == "2") Register();
                else if (choice == "0") break;
            }
        }

        private void Login()
        {
            // Clears the console and prompts the user for login details
            Console.Clear();
            Console.WriteLine("1. Login");
            Console.WriteLine("==============================================");
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();
            Console.WriteLine();

            // Authenticates the user
            var user = userManager.AuthenticateUser(username, password);
            if (user != null)
            {
                currentUser = user;
                Console.WriteLine("Login successful!");
                Console.WriteLine();

                // Redirect to admin or user menu based on username
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
                Console.WriteLine("Login failed. Your username or password is incorrect! Try again.");
                Console.WriteLine();
            }
        }

        private void Register()
        {
            // Clears the console and prompts the user for registration details
            Console.Clear();
            Console.WriteLine("2. Register");
            Console.WriteLine("==============================================");
            Console.WriteLine();
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();
            Console.Write("Date of Birth (yyyy-mm-dd): ");
            DateTime dateOfBirth;

            // Ensures the date format is correct
            while (!DateTime.TryParse(Console.ReadLine(), out dateOfBirth))
            {
                Console.WriteLine("Invalid date format. Please enter again (yyyy-mm-dd): ");
            }

            // Registers the user
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
            while (true)
            {
                // Displays the admin menu options
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("Admin Menu");
                Console.WriteLine("==============================================");
                Console.WriteLine();
                Console.WriteLine("1. Create Quiz\n2. Edit Quiz\n3. Delete Quiz\n4. View Quiz\n5. Logout");
                Console.WriteLine();
                Console.Write("Please Enter your choice: ");
                var choice = Console.ReadLine();

                // Perform actions based on admin's choice
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
            quizUtility.CreateQuiz("vibolsen", "admin123"); // Calls QuizUtility to create a quiz
        }

        private void ViewQuiz()
        {
            var quizUtility = new QuizUtility();
            quizUtility.ViewQuiz("vibolsen", "admin123"); // Calls QuizUtility to view quizzes
        }

        private void EditQuiz()
        {
            var quizUtility = new QuizUtility();
            quizUtility.EditQuiz("vibolsen", "admin123"); // Calls QuizUtility to edit a quiz
        }

        private void DeleteQuiz()
        {
            var quizUtility = new QuizUtility();
            quizUtility.DeleteQuiz("vibolsen", "admin123"); // Calls QuizUtility to delete a quiz
        }

        private void PrintQuiz(Quiz quiz)
        {
            // Prints quiz details
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
                // Displays the user menu options
                Console.WriteLine("User Menu");
                Console.WriteLine("==============================================");
                Console.WriteLine();
                Console.WriteLine("1. Start Quiz\n2. View Results\n3. View Top 20\n4. Edit Settings\n5. Logout");
                Console.WriteLine();
                Console.Write("Please Enter your choice: ");
                var choice = Console.ReadLine();

                // Perform actions based on user's choice
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
            var quizzes = quizManager.ListAllQuizzes(); // Lists all available quizzes
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
                // Prompts user to select a quiz
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
                selectedQuiz = quizManager.GenerateMixedQuiz(); // Generates a mixed quiz
            }
            else
            {
                selectedQuiz = quizzes[choice];
            }

            int correctAnswers = 0;
            foreach (var question in selectedQuiz.Questions)
            {
                // Displays quiz questions and options
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
                        answer = Console.ReadLine().Split(',').Select(int.Parse).ToList(); // Reads user answers
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Invalid input. Please enter numbers separated by commas.");
                    }
                }

                // Checks if answers are correct
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

            // Displays user's score
            Console.WriteLine($"You answered {correctAnswers} out of {selectedQuiz.Questions.Count} questions correctly.");
            SaveQuizResult(currentUser.Username, selectedQuiz.QuizName, correctAnswers);
        }

        // Stores quiz results
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
            // Retrieves and displays user's previous quiz results
            var results = GetUserQuizResults(currentUser.Username);
            Console.WriteLine("Your Previous Quiz Results:");
            foreach (var result in results)
            {
                Console.WriteLine($"Quiz: {result.quizName}, Score: {result.score}");
            }
        }

        private List<(string quizName, int score)> GetUserQuizResults(string username)
        {
            // Returns the quiz results for the specified user
            return userQuizResults.ContainsKey(username) ? userQuizResults[username] : new List<(string quizName, int score)>();
        }

        private void ViewTop20()
        {
            Console.Clear();
            // Prompts user to select a quiz to view top 20 scores
            Console.WriteLine("Select a quiz to view top 20 scores:");
            var quizzes = quizManager.ListAllQuizzes();
            for (int i = 0; i < quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {quizzes[i].QuizName}");
            }
            int choice;
            while (true)
            {
                Console.Write("Enter your choice: ");
                if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= quizzes.Count)
                {
                    choice -= 1;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
            var quizName = quizzes[choice].QuizName;
            var topScores = userManager.ListTop20Scores(quizName);

            // Displays top 20 scores for the selected quiz
            Console.WriteLine($"Top 20 scores for quiz: {quizName}");
            foreach (var score in topScores)
            {
                Console.WriteLine($"Username: {score.username}, Score: {score.score}");
            }
        }

        private void EditSettings()
        {
            Console.Clear();
            // Prompts user to edit their account settings
            Console.WriteLine("Edit Settings");
            Console.WriteLine("==============================================");
            Console.Write("New Username: ");
            var newUsername = Console.ReadLine();
            Console.Write("New Password: ");
            var newPassword = Console.ReadLine();
            Console.Write("New Date of Birth (yyyy-mm-dd): ");
            DateTime newDateOfBirth;
            while (!DateTime.TryParse(Console.ReadLine(), out newDateOfBirth))
            {
                Console.WriteLine("Invalid date format. Please enter again (yyyy-mm-dd): ");
            }

            // Updates user settings
            userManager.EditUserSettings(currentUser.Username, newUsername, newPassword, newDateOfBirth);
            currentUser.Username = newUsername;
            Console.WriteLine("Settings updated successfully.");
        }
    }
}
