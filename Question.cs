using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool CheckAnswer(List<int> userAnswers)
        {
            if (userAnswers.Count != CorrectAnswerIndexes.Count)
                return false;

            foreach (var index in CorrectAnswerIndexes)
            {
                if (!userAnswers.Contains(index))
                    return false;
            }

            return true;
        }
    }


}
