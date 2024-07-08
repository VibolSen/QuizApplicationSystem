using System;
using System.Collections.Generic;

namespace QuizApplicationSystem
{
    public class QuizUtility
    {
        private QuizManager quizManager = new QuizManager(); // Manages quizzes

        /*public void CreateQuiz(string adminUsername, string adminPassword)
        {
            // Admin authentication logic can be added here
            Console.Clear();
            Console.WriteLine("Create New Quiz");
            Console.WriteLine("==============================================");
            Console.Write("Enter Quiz Name: ");
            var quizName = Console.ReadLine();

            var quiz = new Quiz(quizName);

            while (true)
            {
                Console.WriteLine("Add a new question:");
                Console.Write("Question Text: ");
                var question = new Question();
                //var questionText = Console.ReadLine();

                Console.Write("Question Text: ");
                question.QuestionText = Console.ReadLine();

                Console.WriteLine("Enter the options (separated by commas):");
                var options = Console.ReadLine().Split(',');
                question.Options = new List<string>(options);

                Console.WriteLine("Enter the correct answers (separated by commas, by option number):");
                var correctAnswers = Console.ReadLine().Split(',');
                question.CorrectAnswers = new List<int>();
                foreach (var answer in correctAnswers)
                {
                    if (int.TryParse(answer, out int result))
                    {
                        question.CorrectAnswers.Add(result);
                    }
                }

                quiz.Questions.Add(question);

                Console.Write("Do you want to add another question? (yes/no): ");
                var continueAdding = Console.ReadLine();
                if (continueAdding.ToLower() != "yes")
                {
                    break;
                }
            }

            quizManager.AddQuiz(quiz);
            Console.WriteLine("Quiz created successfully!");
        }*/

        public void CreateQuiz(string adminUsername, string adminPassword)
        {
            // Admin authentication logic can be added here
            Console.Clear();
            Console.WriteLine("Create New Quiz");
            Console.WriteLine("==============================================");
            Console.Write("Enter Quiz Name: ");
            var quizName = Console.ReadLine();

            var quiz = new Quiz(quizName);

            while (true)
            {
                Console.WriteLine("Add a new question:");

                // Prompt for question text
                Console.Write("Question Text: ");
                var questionText = Console.ReadLine();

                // Prompt for options
                Console.WriteLine("Enter the options (separated by commas):");
                var optionsInput = Console.ReadLine();
                var options = optionsInput.Split(',');
                var questionOptions = new List<string>(options);

                // Prompt for correct answers
                Console.WriteLine("Enter the correct answers (separated by commas, by option number):");
                var correctAnswersInput = Console.ReadLine();
                var correctAnswers = new List<int>();
                var correctAnswersArray = correctAnswersInput.Split(',');
                foreach (var answer in correctAnswersArray)
                {
                    if (int.TryParse(answer, out int result))
                    {
                        correctAnswers.Add(result);
                    }
                }

                // Create the Question object
                var question = new Question(questionText, questionOptions, correctAnswers);

                // Add the question to the quiz
                quiz.Questions.Add(question);

                // Ask if user wants to add another question
                Console.Write("Do you want to add another question? (yes/no): ");
                var continueAdding = Console.ReadLine();
                if (continueAdding.ToLower() != "yes")
                {
                    break;
                }
            }

            // Add the quiz to the quiz manager
            quizManager.AddQuiz(quiz);
            Console.WriteLine("Quiz created successfully!");
        }


        public void ViewQuiz(string adminUsername, string adminPassword)
        {
            // Admin authentication logic can be added here
            Console.Clear();
            var quizzes = quizManager.ListAllQuizzes();
            foreach (var quiz in quizzes)
            {
                Console.WriteLine($"Quiz: {quiz.QuizName}");
                foreach (var question in quiz.Questions)
                {
                    Console.WriteLine($"Question: {question.QuestionText}");
                    for (int i = 0; i < question.Options.Count; i++)
                    {
                        Console.WriteLine($"Option {i + 1}: {question.Options[i]}");
                    }
                    Console.WriteLine($"Correct Answers: {string.Join(", ", question.CorrectAnswers)}");
                }
                Console.WriteLine("==============================================");
            }
        }

        public void EditQuiz(string adminUsername, string adminPassword)
        {
            // Admin authentication logic can be added here
            Console.Clear();
            var quizzes = quizManager.ListAllQuizzes();
            Console.WriteLine("Select a quiz to edit:");
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

            var quiz = quizzes[choice];

            Console.WriteLine($"Editing quiz: {quiz.QuizName}");
            Console.Write("Enter new quiz name (or press Enter to keep current name): ");
            var newQuizName = Console.ReadLine();
            if (!string.IsNullOrEmpty(newQuizName))
            {
                quiz.QuizName = newQuizName;
            }

            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                var question = quiz.Questions[i];
                Console.WriteLine($"Editing question {i + 1}: {question.QuestionText}");
                Console.Write("Enter new question text (or press Enter to keep current text): ");
                var newQuestionText = Console.ReadLine();
                if (!string.IsNullOrEmpty(newQuestionText))
                {
                    question.QuestionText = newQuestionText;
                }

                Console.WriteLine("Enter new options (separated by commas) or press Enter to keep current options:");
                var newOptions = Console.ReadLine();
                if (!string.IsNullOrEmpty(newOptions))
                {
                    question.Options = new List<string>(newOptions.Split(','));
                }

                Console.WriteLine("Enter new correct answers (separated by commas, by option number) or press Enter to keep current correct answers:");
                var newCorrectAnswers = Console.ReadLine();
                if (!string.IsNullOrEmpty(newCorrectAnswers))
                {
                    question.CorrectAnswers = new List<int>();
                    foreach (var answer in newCorrectAnswers.Split(','))
                    {
                        if (int.TryParse(answer, out int result))
                        {
                            question.CorrectAnswers.Add(result);
                        }
                    }
                }
            }

            quizManager.EditQuiz(quiz.QuizName, quiz);
            Console.WriteLine("Quiz edited successfully!");
        }

        public void DeleteQuiz(string adminUsername, string adminPassword)
        {
            // Admin authentication logic can be added here
            Console.Clear();
            var quizzes = quizManager.ListAllQuizzes();
            Console.WriteLine("Select a quiz to delete:");
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
            quizManager.RemoveQuiz(quizName);
            Console.WriteLine("Quiz deleted successfully!");
        }
    }
}
