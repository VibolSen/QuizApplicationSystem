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

        public void EditQuiz(string username, string password)
        {
            // Similar to CreateQuiz but modifying an existing quiz
        }

        public void DeleteQuiz(string username, string password)
        {
            // Similar to CreateQuiz but deleting an existing quiz
        }
    }


}
