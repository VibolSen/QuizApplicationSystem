using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace QuizApplicationSystem
{
    public class QuizManager
    {
        private List<Quiz> quizzes = new List<Quiz>();

        //test
        private readonly string quizFilePath = "quizzes.txt";

        public QuizManager()
        {
            LoadQuizzesFromFile();
        }

        public void AddQuiz(Quiz quiz)
        {
            quizzes.Add(quiz);

            //add test
            SaveQuizzesToFile();
        }

        public List<Quiz> GetQuizzes()
        {
            return quizzes;
        }

        public Quiz GetQuizByTitle(string title)
        {
            return quizzes.FirstOrDefault(q => q.Title == title);
        }

        public void RemoveQuiz(Quiz quiz)
        {
            quizzes.Remove(quiz);

            //test
            SaveQuizzesToFile();

        }

        private void SaveQuizzesToFile()
        {
            try
            {
                var quizData = JsonSerializer.Serialize(quizzes);
                File.WriteAllText(quizFilePath, quizData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving quizzes: " + ex.Message);
            }
        }

        private void LoadQuizzesFromFile()
        {
            if (!File.Exists(quizFilePath))
                return;

            try
            {
                var quizData = File.ReadAllText(quizFilePath);
                quizzes = JsonSerializer.Deserialize<List<Quiz>>(quizData) ?? new List<Quiz>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading quizzes: " + ex.Message);
            }
        }
    }
}