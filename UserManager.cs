using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace QuizApplicationSystem
{
    public class UserManager
    {
        private List<User> users;

        public UserManager()
        {
            users = LoadUsersFromFile();
        }

        public bool RegisterUser(string username, string password, DateTime dateOfBirth)
        {
            if (users.Any(u => u.Username == username))
            {
                return false; // Username already exists
            }

            var newUser = new User(username, password, dateOfBirth);
            users.Add(newUser);
            SaveUsersToFile();
            return true;
        }

        public User LoginUser(string username, string password)
        {
            return users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public void UpdateUser(User user)
        {
            var existingUser = users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                existingUser.Password = user.Password;
                SaveUsersToFile();
            }
        }

        private void SaveUsersToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(users);
                File.WriteAllText("users.txt", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving users to file: {ex.Message}");
            }
        }

        private List<User> LoadUsersFromFile()
        {
            try
            {
                if (File.Exists("users.txt"))
                {
                    string json = File.ReadAllText("users.txt");
                    return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                }
                return new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users from file: {ex.Message}");
                return new List<User>();
            }
        }
    }
}
