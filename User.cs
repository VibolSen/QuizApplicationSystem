using System;
using System.Collections.Generic;

namespace QuizApplicationSystem
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<QuizResult> QuizResults { get; set; }

        public User(string username, string password, DateTime dateOfBirth)
        {
            Username = username;
            Password = password;
            DateOfBirth = dateOfBirth;
            QuizResults = new List<QuizResult>();
        }
    }
}
