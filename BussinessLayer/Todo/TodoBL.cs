using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Todo;
using EntityLayer.Models;

namespace BusinessLayer.Todo
{
    public static class TodoBL
    {
        public static Task<List<TodoTask>> GetTodosAsync(string uid)
            => TodoDL.GetTodosAsync(uid);

        public static Task<string> AddTodoAsync(string uid, TodoTask todo)
            => TodoDL.AddTodoAsync(uid, todo);

        public static Task UpdateTodoAsync(string uid, TodoTask todo)
            => TodoDL.UpdateTodoAsync(uid, todo);

        public static Task DeleteTodoAsync(string uid, string todoId)
            => TodoDL.DeleteTodoAsync(uid, todoId);
    }
}