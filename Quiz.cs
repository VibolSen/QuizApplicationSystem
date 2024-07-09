using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplicationSystem
{
    public class Quiz
    {
        public string QuizName { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();

        public Quiz(string quizName)
        {
            QuizName = quizName;
        }

        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }

        public void RemoveQuestion(Question question)
        {
            Questions.Remove(question);
        }


    }

}
