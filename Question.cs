using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplicationSystem
{
    public class Question
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public List<int> CorrectAnswers { get; set; }

        public Question(string questionText, List<string> options, List<int> correctAnswers)
        {
            QuestionText = questionText;
            Options = options;
            CorrectAnswers = correctAnswers;
        }
    }
}
