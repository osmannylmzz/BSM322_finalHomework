using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Config;
using EntityLayer.Models;
using Firebase.Database;
using Firebase.Database.Query;

namespace DataLayer.Todo
{
    public static class TodoDL
    {
        private static FirebaseClient CreateClient()
        {
            // FirebaseDatabase.net base URL sonu / olmalý
            var baseUrl = FirebaseSettings.RealtimeDbUrl?.Trim();
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("FirebaseSettings.RealtimeDbUrl boþ olamaz.");

            if (!baseUrl.EndsWith("/"))
                baseUrl += "/";

            return new FirebaseClient(baseUrl);
        }

        public static async Task<List<TodoTask>> GetTodosAsync(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
                throw new ArgumentException("uid boþ olamaz.", nameof(uid));

            var client = CreateClient();

            var items = await client
                .Child("users")
                .Child(uid)
                .Child("todos")
                .OnceAsync<TodoTask>();

            return items
                .Select(x =>
                {
                    var t = x.Object ?? new TodoTask();
                    t.Id = x.Key; // key'i Id'ye yaz
                    return t;
                })
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.Time)
                .ToList();
        }

        public static async Task<string> AddTodoAsync(string uid, TodoTask todo)
        {
            if (string.IsNullOrWhiteSpace(uid))
                throw new ArgumentException("uid boþ olamaz.", nameof(uid));
            if (todo is null)
                throw new ArgumentNullException(nameof(todo));

            var client = CreateClient();

            // Push -> key üret
            var pushResult = await client
                .Child("users")
                .Child(uid)
                .Child("todos")
                .PostAsync(new TodoTask
                {
                    // Id'yi ilk etapta boþ býrakýyoruz, key'le set edilecek
                    Id = "",
                    Title = todo.Title ?? "",
                    Detail = todo.Detail ?? "",
                    Date = todo.Date,
                    Time = todo.Time,
                    IsDone = todo.IsDone
                });

            var key = pushResult.Key;
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Firebase todoId üretmedi.");

            todo.Id = key;

            // todo.Id = key olacak þekilde kaydý güncelle (istenen þart)
            await client
                .Child("users")
                .Child(uid)
                .Child("todos")
                .Child(key)
                .PutAsync(todo);

            return key;
        }

        public static async Task UpdateTodoAsync(string uid, TodoTask todo)
        {
            if (string.IsNullOrWhiteSpace(uid))
                throw new ArgumentException("uid boþ olamaz.", nameof(uid));
            if (todo is null)
                throw new ArgumentNullException(nameof(todo));
            if (string.IsNullOrWhiteSpace(todo.Id))
                throw new ArgumentException("todo.Id boþ olamaz.", nameof(todo));

            var client = CreateClient();

            await client
                .Child("users")
                .Child(uid)
                .Child("todos")
                .Child(todo.Id)
                .PutAsync(todo);
        }

        public static async Task DeleteTodoAsync(string uid, string todoId)
        {
            if (string.IsNullOrWhiteSpace(uid))
                throw new ArgumentException("uid boþ olamaz.", nameof(uid));
            if (string.IsNullOrWhiteSpace(todoId))
                throw new ArgumentException("todoId boþ olamaz.", nameof(todoId));

            var client = CreateClient();

            await client
                .Child("users")
                .Child(uid)
                .Child("todos")
                .Child(todoId)
                .DeleteAsync();
        }
    }
}       