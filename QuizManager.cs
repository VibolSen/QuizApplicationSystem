using QuizApplicationSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class QuizManager
{
    private const string QuizFilePath = "quizzes.txt";
    private List<Quiz> quizzes = new List<Quiz>();

    public QuizManager()
    {
        LoadQuizzesFromFile();
    }

    public void CreateQuiz(Quiz quiz)
    {
        quizzes.Add(quiz);
        SaveQuizzesToFile();
    }

    public void EditQuiz(Quiz quiz)
    {
        var existingQuiz = quizzes.Find(q => q.QuizName == quiz.QuizName);
        if (existingQuiz != null)
        {
            existingQuiz.Questions = quiz.Questions;
            SaveQuizzesToFile();
        }
    }

    public void DeleteQuiz(string quizName)
    {
        var quiz = quizzes.Find(q => q.QuizName == quizName);
        if (quiz != null)
        {
            quizzes.Remove(quiz);
            SaveQuizzesToFile();
        }
    }

    public Quiz GetQuiz(string quizName)
    {
        return quizzes.Find(q => q.QuizName == quizName);
    }

    public List<Quiz> ListAllQuizzes()
    {
        return quizzes;
    }

    public Quiz GenerateMixedQuiz()
    {
        var mixedQuiz = new Quiz("Mixed Quiz");
        var allQuestions = new List<Question>();
        foreach (var quiz in quizzes)
        {
            allQuestions.AddRange(quiz.Questions);
        }

        var random = new Random();
        var questionCount = Math.Min(20, allQuestions.Count);
        var usedIndices = new HashSet<int>();

        while (mixedQuiz.Questions.Count < questionCount)
        {
            int index;
            do
            {
                index = random.Next(allQuestions.Count);
            } while (usedIndices.Contains(index));

            usedIndices.Add(index);
            mixedQuiz.AddQuestion(allQuestions[index]);
        }

        return mixedQuiz;
    }

    private void SaveQuizzesToFile()
    {
        using (var writer = new StreamWriter(QuizFilePath))
        {
            foreach (var quiz in quizzes)
            {
                writer.WriteLine(quiz.QuizName);
                foreach (var question in quiz.Questions)
                {
                    writer.WriteLine(question.QuestionText);
                    writer.WriteLine(string.Join("|", question.Options));
                    writer.WriteLine(string.Join(",", question.CorrectAnswers));
                }
                writer.WriteLine("ENDQUIZ");
            }
        }
    }

    private void LoadQuizzesFromFile()
    {
        if (File.Exists(QuizFilePath))
        {
            using (var reader = new StreamReader(QuizFilePath))
            {
                Quiz quiz = null;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == "ENDQUIZ")
                    {
                        quizzes.Add(quiz);
                        quiz = null;
                    }
                    else if (quiz == null)
                    {
                        quiz = new Quiz(line);
                    }
                    else
                    {
                        var questionText = line;
                        var options = reader.ReadLine().Split('|').ToList();
                        var correctAnswers = reader.ReadLine().Split(',').Select(int.Parse).ToList();
                        quiz.AddQuestion(new Question(questionText, options, correctAnswers));
                    }
                }
            }
        }
    }
}
