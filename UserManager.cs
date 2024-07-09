using System;
using System.Collections.Generic;

namespace QuizApplicationSystem
{
    public class UserManager
    {
        // Stores users' credentials and information
        private Dictionary<string, (string Password, DateTime DateOfBirth)> users = new Dictionary<string, (string Password, DateTime DateOfBirth)>();
        // Stores users' quiz scores
        private Dictionary<string, List<(string quizName, int score)>> userScores = new Dictionary<string, List<(string quizName, int score)>>();

        public User AuthenticateUser(string username, string password)
        {
            // Authenticates the user by checking if the username and password match
            if (username == "vibolsen" && password == "admin123")
            {
                // If the username and password match admin credentials
                return new User(username, password, DateTime.Now); // You should fetch actual date of birth from stored user data.
            }

            if (users.ContainsKey(username) && users[username].Password == password)
            {
                return new User(username, password, users[username].DateOfBirth);
            }
            return null;
        }

        public bool RegisterUser(string username, string password, DateTime dateOfBirth)
        {
            // Registers a new user if the username doesn't already exist
            if (users.ContainsKey(username))
            {
                return false;
            }
            users[username] = (password, dateOfBirth);
            return true;
        }

        public bool EditUserSettings(string username, string newUsername, string newPassword, DateTime newDateOfBirth)
        {
            // Check if the newUsername already exists and ensure it's not the same as the current username
            if (!users.ContainsKey(username) || (users.ContainsKey(newUsername) && newUsername != username))
            {
                return false;
            }

            var userInfo = users[username];
            users.Remove(username);
            users[newUsername] = (newPassword, newDateOfBirth);

            // Update the user's quiz scores if the username has been changed
            if (userScores.ContainsKey(username))
            {
                var scores = userScores[username];
                userScores.Remove(username);
                userScores[newUsername] = scores;
            }

            return true;
        }

        public List<(string username, int score)> ListTop20Scores(string quizName)
        {
            // Lists the top 20 scores for a specific quiz
            var allScores = new List<(string username, int score)>();
            foreach (var user in userScores)
            {
                foreach (var score in user.Value)
                {
                    if (score.quizName == quizName)
                    {
                        allScores.Add((user.Key, score.score));
                    }
                }
            }
            allScores.Sort((a, b) => b.score.CompareTo(a.score));
            return allScores.GetRange(0, Math.Min(20, allScores.Count));
        }

        public void SaveUserScore(string username, string quizName, int score)
        {
            // Saves the user's score for a specific quiz
            if (!userScores.ContainsKey(username))
            {
                userScores[username] = new List<(string quizName, int score)>();
            }
            userScores[username].Add((quizName, score));
        }
    }
}
