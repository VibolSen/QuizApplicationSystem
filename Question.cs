using System;
using System.Collections.Generic;

namespace QuizApplicationSystem
{
    public class Question
    {
        public string Text { get; set; }
        public List<Answer> Answers { get; set; }
        public List<int> CorrectAnswerIndexes { get; set; }

        public Question(string text)
        {
            Text = text;
            Answers = new List<Answer>();
            CorrectAnswerIndexes = new List<int>();
        }
    }
}
