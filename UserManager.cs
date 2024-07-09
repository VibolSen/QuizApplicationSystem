using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuizApplicationSystem
{

    public class UserManager
    {
        private List<User> users = new List<User>();

        //test
        private readonly string usersFilePath = "users.txt";

        public UserManager()
        {
            LoadUsersFromFile();
        }

        public bool RegisterUser(string username, string password, DateTime dateOfBirth)
        {
            if (users.Any(u => u.Username == username))
                return false;

            var newUser = new User(username, password, dateOfBirth);
            users.Add(newUser);
            //add test
            SaveUsersToFile();
            return true;
        }

        public User LoginUser(string username, string password)
        {
            return users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public List<User> GetUsers()
        {
            return users;
        }

        public void ChangePassword(User user, string newPassword)
        {
            user.Password = newPassword;
            SaveUsersToFile();
        }

        public void ChangeDateOfBirth(User user, DateTime newDateOfBirth)
        {
            user.DateOfBirth = newDateOfBirth;
            SaveUsersToFile();
        }

        private void SaveUsersToFile()
        {
            using (StreamWriter writer = new StreamWriter(usersFilePath))
            {
                foreach (var user in users)
                {
                    writer.WriteLine($"{user.Username},{user.Password},{user.DateOfBirth:yyyy-MM-dd}");
                }
            }
        }

        private void LoadUsersFromFile()
        {
            if (!File.Exists(usersFilePath))
                return;

            using (StreamReader reader = new StreamReader(usersFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 3 &&
                        DateTime.TryParse(parts[2], out DateTime dateOfBirth))
                    {
                        var user = new User(parts[0], parts[1], dateOfBirth);
                        users.Add(user);
                    }
                }
            }
        }
    }
}
