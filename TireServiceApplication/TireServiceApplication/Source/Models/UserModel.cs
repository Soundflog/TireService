using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Models;

public class UserModel
{
    private static List<User> _users = new List<User>();

    // Метод для получения всех пользователей
    public static async Task<List<User>?> GetUsers()
    {
        var users = await UserData.GetUsers();
        if (users != null)
        {
            _users = users;
            TextForListView(users);
        }
        return _users;
    }

    // Метод для добавления пользователя
    public static async Task<bool> AddUser(User user)
    {
        var newUser = await UserData.AddUser(user);
        if (newUser != null) _users.Add(newUser);
        return newUser != null;
    }

    // Метод для изменения пользователя
    public static async Task<bool> UpdateUser(User user)
    {
        var newUser = await UserData.UpdateUser(user);
        if (newUser != null)
        {
            var index = _users.FindIndex(x => x.Id == user.Id);
            _users[index] = newUser;
        }

        return newUser != null;
    }

    // Метод для удаления пользователя
    public static async Task<bool> DeleteUser(User user)
    {
        if (user.Id == null) return false;
        var result = await UserData.DeleteUser(user.Id);
        if (result)
        {
            var index = _users.FindIndex(x => x.Id == user.Id);
            _users.RemoveAt(index);
        }

        return result;
    }
    
    // Метод для получения пользователя по Id
    public static async Task<User?> GetUser(string id)
    {
        var user = await UserData.GetUserById(id);
        return user;
    }
    
    // Метод для составления текста для отображения в ListView
    public static void TextForListView(List<User> users)
    {
        var roles = new List<Role>()
        {
            new Role { Name = "Менеджер", Key = "manager" },
            new Role { Name = "Администратор", Key = "admin" }
        };
        // Пример "Роль: Администратор"
        foreach (var user in users)
        {
            var role = roles.Find(x => x.Key == user.Role);
            user.RoleView = role != null ? $"Роль: {role.Name}" : "Роль: Неизвестно";
        }
    }
}