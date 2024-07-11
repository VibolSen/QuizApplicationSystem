using System;
using System.Collections.Generic;

namespace QuizApplicationSystem
{
    public class Quiz
    {
        public string Title { get; set; }
        public List<Question> Questions { get; set; }

        public Quiz(string title)
        {
            Title = title;
            Questions = new List<Question>();
        }
    }
}