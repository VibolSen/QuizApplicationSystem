using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplicationSystem
{
    public class QuizResult
    {
        public Quiz Quiz { get; set; }
        public int CorrectAnswers { get; set; }
        public DateTime DateTaken { get; set; }

        public QuizResult(Quiz quiz, int correctAnswers, DateTime dateTaken)
        {
            Quiz = quiz;
            CorrectAnswers = correctAnswers;
            DateTaken = dateTaken;
        }
    }
}
