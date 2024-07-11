using System;

namespace QuizApplicationSystem
{
    public class QuizResult
    {
        public string UserName { get; set; }
        public string QuizTitle { get; set; }
        public int Score { get; set; }
        public int TimeTaken { get; set; } // in seconds
        public DateTime Timestamp { get; set; }

        public QuizResult(string userName, string quizTitle)
        {
            UserName = userName;
            QuizTitle = quizTitle;
            Score = 0;
            TimeTaken = 0;
            Timestamp = DateTime.Now;
        }
    }
}
