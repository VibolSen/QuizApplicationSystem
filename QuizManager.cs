using System;
using System.Collections.Generic;
using System.Linq;

namespace QuizApplicationSystem
{
    public class QuizManager
    {
        // Stores all quizzes
        private List<Quiz> quizzes = new List<Quiz>();

        public List<Quiz> ListAllQuizzes()
        {
            // Returns a list of all quizzes
            return quizzes;
        }

        public Quiz GenerateMixedQuiz()
        {
            // Generates a mixed quiz by selecting random questions from all quizzes
            var mixedQuiz = new Quiz ("Mixed Quiz");
            var allQuestions = quizzes.SelectMany(q => q.Questions).ToList();
            var random = new Random();
            for (int i = 0; i < 20 && allQuestions.Count > 0; i++)
            {
                var index = random.Next(allQuestions.Count);
                mixedQuiz.Questions.Add(allQuestions[index]);
                allQuestions.RemoveAt(index);
            }
            return mixedQuiz;
        }

        public void AddQuiz(Quiz quiz)
        {
            // Adds a new quiz to the list
            quizzes.Add(quiz);
        }

        public void RemoveQuiz(string quizName)
        {
            // Removes a quiz by name
            quizzes.RemoveAll(q => q.QuizName == quizName);
        }

        public void EditQuiz(string quizName, Quiz newQuiz)
        {
            // Edits an existing quiz
            var index = quizzes.FindIndex(q => q.QuizName == quizName);
            if (index != -1)
            {
                quizzes[index] = newQuiz;
            }
        }
    }
}
