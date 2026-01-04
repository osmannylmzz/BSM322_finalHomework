using System;

namespace EntityLayer.Models
{
    public class TodoTask
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Detail { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Today;
        public TimeSpan Time { get; set; } = DateTime.Now.TimeOfDay;
        public bool IsDone { get; set; }
    }
}
