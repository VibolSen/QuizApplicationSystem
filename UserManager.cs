using QuizApplicationSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class UserManager
{
    private const string UserFilePath = "users.txt";
    private readonly string adminUsername = "vibolsen";
    private readonly string adminPassword = "admin123";

    public UserManager()
    {
        // Ensure the user file exists
        if (!File.Exists(UserFilePath))
        {
            File.Create(UserFilePath).Close();
        }
    }

    public bool RegisterUser(string username, string password, DateTime dateOfBirth)
    {
        if (IsUsernameTaken(username))
        {
            return false;
        }

        var userLine = $"{username}|{password}|{dateOfBirth:yyyy-MM-dd}";
        File.AppendAllLines(UserFilePath, new[] { userLine });
        return true;
    }

    public User AuthenticateUser(string username, string password)
    {
        if (username == adminUsername && password == adminPassword)
        {
            return new User(username, password, DateTime.MinValue); // Admin doesn't need date of birth
        }

        var users = File.ReadAllLines(UserFilePath);
        foreach (var user in users)
        {
            var userParts = user.Split('|');
            if (userParts[0] == username && userParts[1] == password)
            {
                return new User(username, password, DateTime.Parse(userParts[2]));
            }
        }

        return null;
    }

    public bool EditUserSettings(string username, string newPassword, DateTime newDateOfBirth)
    {
        var users = File.ReadAllLines(UserFilePath).ToList();
        for (int i = 0; i < users.Count; i++)
        {
            var userParts = users[i].Split('|');
            if (userParts[0] == username)
            {
                users[i] = $"{username}|{newPassword}|{newDateOfBirth:yyyy-MM-dd}";
                File.WriteAllLines(UserFilePath, users);
                return true;
            }
        }

        return false;
    }

    public User GetUser(string username)
    {
        var users = File.ReadAllLines(UserFilePath);
        foreach (var user in users)
        {
            var userParts = user.Split('|');
            if (userParts[0] == username)
            {
                return new User(username, userParts[1], DateTime.Parse(userParts[2]));
            }
        }

        return null;
    }

    private bool IsUsernameTaken(string username)
    {
        var users = File.ReadAllLines(UserFilePath);
        return users.Any(user => user.Split('|')[0] == username);
    }

    public List<(string Username, int Score)> ListTop20Scores(Quiz quiz)
    {
        // Implement this method to list top 20 scores from a persistent storage
        return new List<(string Username, int Score)>();
    }
}
