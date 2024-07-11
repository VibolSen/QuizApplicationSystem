using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace QuizApplicationSystem
{
    public class QuizManager
    {
        private List<Quiz> quizzes;

        public QuizManager()
        {
            quizzes = LoadQuizzesFromFile();
        }

        public void AddQuiz(Quiz quiz)
        {
            quizzes.Add(quiz);
            SaveQuizzesToFile();
        }

        public void RemoveQuiz(Quiz quiz)
        {
            quizzes.Remove(quiz);
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

        private void SaveQuizzesToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(quizzes);
                File.WriteAllText("quizzes.txt", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving quizzes to file: {ex.Message}");
            }
        }

        private List<Quiz> LoadQuizzesFromFile()
        {
            try
            {
                if (File.Exists("quizzes.txt"))
                {
                    string json = File.ReadAllText("quizzes.txt");
                    return JsonSerializer.Deserialize<List<Quiz>>(json) ?? new List<Quiz>();
                }
                return new List<Quiz>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading quizzes from file: {ex.Message}");
                return new List<Quiz>();
            }
        }
    }
}
