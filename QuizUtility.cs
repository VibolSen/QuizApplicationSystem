using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplicationSystem
{
    public class QuizUtility
    {
        private QuizManager quizManager = new QuizManager();
        private UserManager userManager = new UserManager();

        public void CreateQuiz(string username, string password)
        {
            Console.Clear();
            Console.WriteLine("1. Create Quiz");
            Console.WriteLine("==============================================");
            var user = userManager.AuthenticateUser(username, password);
            if (user != null)
            {
                Console.Write("Quiz Name: ");
                var quizName = Console.ReadLine();
                var quiz = new Quiz(quizName);

                for (int i = 0; i < 20; i++)
                {
                    Console.Write($"Question {i + 1}: ");
                    var questionText = Console.ReadLine();
                    var options = new List<string>();
                    for (int j = 0; j < 4; j++)
                    {
                        Console.Write($"Option {j + 1}: ");
                        options.Add(Console.ReadLine());
                    }
                    Console.Write("Correct Answers (comma separated indices): ");
                    var correctAnswers = Console.ReadLine().Split(',').Select(int.Parse).ToList();

                    quiz.AddQuestion(new Question(questionText, options, correctAnswers));
                }

                quizManager.CreateQuiz(quiz);
                Console.WriteLine("Quiz created successfully.");
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
            }
        }

        /*public void ViewQuiz(string username, string password)
        {
            Console.Clear();
            Console.WriteLine("4. View Quiz");
            Console.WriteLine("==============================================");
            var user = userManager.AuthenticateUser(username, password);
            if (user != null)
            {
                Console.Write("Enter Quiz Name to View: ");
                var quizName = Console.ReadLine();
                var quiz = quizManager.GetQuiz(quizName);

                if (quiz != null)
                {
                    Console.WriteLine($"Quiz Name: {quiz.QuizName}");
                    for (int i = 0; i < quiz.Questions.Count; i++)
                    {
                        var question = quiz.Questions[i];
                        Console.WriteLine($"Question {i + 1}: {question.QuestionText}");
                        for (int j = 0; j < question.Options.Count; j++)
                        {
                            Console.WriteLine($"Option {j + 1}: {question.Options[j]}");
                        }
                        Console.WriteLine($"Correct Answers: {string.Join(", ", question.CorrectAnswers)}");
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.Write("Quiz not found. Press any key to return to the admin menu. ");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
            }
        }*/

        public void ViewQuiz(string username, string password)
        {
            Console.Clear();
            Console.WriteLine("4. View Quiz");
            Console.WriteLine("==============================================");
            var user = userManager.AuthenticateUser(username, password);
            if (user != null)
            {
                var quizzes = quizManager.ListAllQuizzes();
                if (quizzes.Count == 0)
                {
                    Console.Write("No quizzes available. Press any key to return to the admin menu.");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("Available Quizzes:");
                for (int i = 0; i < quizzes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {quizzes[i].QuizName}");
                }

                Console.Write("Which quiz do you want to view? Enter the number: ");
                if (int.TryParse(Console.ReadLine(), out int quizIndex) && quizIndex > 0 && quizIndex <= quizzes.Count)
                {
                    var quiz = quizzes[quizIndex - 1];
                    Console.WriteLine($"Quiz Name: {quiz.QuizName}");
                    for (int i = 0; i < quiz.Questions.Count; i++)
                    {
                        var question = quiz.Questions[i];
                        Console.WriteLine($"Question {i + 1}: {question.QuestionText}");
                        for (int j = 0; j < question.Options.Count; j++)
                        {
                            Console.WriteLine($"Option {j + 1}: {question.Options[j]}");
                        }
                        Console.WriteLine($"Correct Answers: {string.Join(", ", question.CorrectAnswers)}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection. Press any key to return to the admin menu.");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
            }
        }

        public void EditQuiz(string username, string password)
        {
            Console.Clear();
            Console.WriteLine("2. Edit Quiz");
            Console.WriteLine("==============================================");
            var user = userManager.AuthenticateUser(username, password);
            if (user != null)
            {
                Console.Write("Enter Quiz Name to Edit: ");
                var quizName = Console.ReadLine();
                var quiz = quizManager.GetQuiz(quizName);

                if (quiz != null)
                {
                    for (int i = 0; i < quiz.Questions.Count; i++)
                    {
                        var question = quiz.Questions[i];
                        Console.WriteLine($"Current Question {i + 1}: {question.QuestionText}");
                        Console.Write("New Question Text (leave empty to keep current): ");
                        var newQuestionText = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newQuestionText))
                        {
                            question.QuestionText = newQuestionText;
                        }

                        for (int j = 0; j < question.Options.Count; j++)
                        {
                            Console.WriteLine($"Current Option {j + 1}: {question.Options[j]}");
                            Console.Write("New Option Text (leave empty to keep current): ");
                            var newOptionText = Console.ReadLine();
                            if (!string.IsNullOrEmpty(newOptionText))
                            {
                                question.Options[j] = newOptionText;
                            }
                        }
                        Console.Write("New Correct Answers (comma separated indices, leave empty to keep current): ");
                        var newCorrectAnswers = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newCorrectAnswers))
                        {
                            question.CorrectAnswers = newCorrectAnswers.Split(',').Select(int.Parse).ToList();
                        }
                    }
                    quizManager.EditQuiz(quiz);
                    Console.WriteLine("Quiz updated successfully.");
                }
                else
                {
                    Console.WriteLine();
                    Console.Write("Quiz not found. Press any key to return to the admin menu. ");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
            }
        }

        public void DeleteQuiz(string username, string password)
        {
            Console.Clear();
            Console.WriteLine("3. Delete Quiz");
            Console.WriteLine("==============================================");
            var user = userManager.AuthenticateUser(username, password);
            if (user != null)
            {
                Console.Write("Enter Quiz Name to Delete: ");
                var quizName = Console.ReadLine();
                var quiz = quizManager.GetQuiz(quizName);

                if (quiz != null)
                {
                    quizManager.DeleteQuiz(quizName);
                    Console.WriteLine("Quiz deleted successfully.");
                }
                else
                {
                    Console.WriteLine();
                    Console.Write("Quiz not found. Press any key to return to the admin menu. ");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
            }
        }
    }
}