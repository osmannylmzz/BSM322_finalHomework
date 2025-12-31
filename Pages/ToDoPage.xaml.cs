using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using BusinessLayer.Todo;
using EntityLayer.Models;
using finalHomework.Services;

namespace finalHomework.Pages
{
    public partial class ToDoPage : ContentPage
    {
        private readonly ObservableCollection<TodoTask> _todos = new();
        private TodoTask? _editingTodo = null;

        public ToDoPage()
        {
            InitializeComponent();

            TodosCollectionView.ItemsSource = _todos;

            TodoDatePicker.Date = DateTime.Today;
            TodoTimePicker.Time = DateTime.Now.TimeOfDay;

            ExitEditMode(); // başlangıçta ekleme modu
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            try
            {
                var uid = Session.CurrentUid;

                var loggedIn = !string.IsNullOrWhiteSpace(uid);
                LoginWarningLabel.IsVisible = !loggedIn;
                AddPanel.IsVisible = loggedIn;
                TodosCollectionView.IsVisible = loggedIn;

                _todos.Clear();

                if (!loggedIn)
                    return;

                var list = await TodoBL.GetTodosAsync(uid!);
                foreach (var t in list)
                    _todos.Add(t);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }

        // --------- Edit Mode Helpers ----------
        private void EnterEditMode(TodoTask todo)
        {
            _editingTodo = todo;

            EditModeLabel.IsVisible = true;
            CancelEditButton.IsVisible = true;
            SaveButton.Text = "💾 Kaydet";

            TitleEntry.Text = todo.Title;
            DetailEntry.Text = todo.Detail;
            TodoDatePicker.Date = todo.Date;
            TodoTimePicker.Time = todo.Time;
        }

        private void ExitEditMode()
        {
            _editingTodo = null;

            EditModeLabel.IsVisible = false;
            CancelEditButton.IsVisible = false;
            SaveButton.Text = "➕ Ekle";

            TitleEntry.Text = "";
            DetailEntry.Text = "";
            TodoDatePicker.Date = DateTime.Today;
            TodoTimePicker.Time = DateTime.Now.TimeOfDay;
        }

        private void OnCancelEditClicked(object sender, EventArgs e)
        {
            ExitEditMode();
        }

        private void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.BindingContext is not TodoTask todo) return;

            EnterEditMode(todo);
        }

        // --------- Add / Save ----------
        private async void OnAddClicked(object sender, EventArgs e)
        {
            try
            {
                var uid = Session.CurrentUid;
                if (string.IsNullOrWhiteSpace(uid))
                {
                    await DisplayAlert("Uyarı", "Lütfen giriş yapın.", "Tamam");
                    return;
                }

                var title = TitleEntry.Text?.Trim() ?? "";
                var detail = DetailEntry.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(title))
                {
                    await DisplayAlert("Hata", "Başlık boş olamaz.", "Tamam");
                    return;
                }

                // ✅ DÜZENLEME MODU -> Kaydet
                if (_editingTodo != null)
                {
                    if (string.IsNullOrWhiteSpace(_editingTodo.Id))
                    {
                        await DisplayAlert("Hata", "Bu kaydın Id bilgisi yok. Düzenlenemiyor.", "Tamam");
                        return;
                    }

                    _editingTodo.Title = title;
                    _editingTodo.Detail = detail;
                    _editingTodo.Date = TodoDatePicker.Date;
                    _editingTodo.Time = TodoTimePicker.Time;

                    await TodoBL.UpdateTodoAsync(uid, _editingTodo);

                    ExitEditMode();
                    await LoadAsync();
                    return;
                }

                // ✅ EKLEME MODU
                var todo = new TodoTask
                {
                    Title = title,
                    Detail = detail,
                    Date = TodoDatePicker.Date,
                    Time = TodoTimePicker.Time,
                    IsDone = false
                };

                var id = await TodoBL.AddTodoAsync(uid, todo);
                if (!string.IsNullOrWhiteSpace(id))
                    todo.Id = id;

                _todos.Insert(0, todo);

                TitleEntry.Text = "";
                DetailEntry.Text = "";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }

        private async void OnDoneChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                var uid = Session.CurrentUid;
                if (string.IsNullOrWhiteSpace(uid))
                {
                    await DisplayAlert("Uyarı", "Lütfen giriş yapın.", "Tamam");
                    return;
                }

                if (sender is not CheckBox cb) return;
                if (cb.BindingContext is not TodoTask todo) return;

                if (string.IsNullOrWhiteSpace(todo.Id))
                    return;

                todo.IsDone = e.Value;
                await TodoBL.UpdateTodoAsync(uid, todo);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            try
            {
                var uid = Session.CurrentUid;
                if (string.IsNullOrWhiteSpace(uid))
                {
                    await DisplayAlert("Uyarı", "Lütfen giriş yapın.", "Tamam");
                    return;
                }

                if (sender is not Button btn) return;
                if (btn.BindingContext is not TodoTask todo) return;

                if (string.IsNullOrWhiteSpace(todo.Id))
                    return;

                var ok = await DisplayAlert("Onay", "Silmek istiyor musunuz?", "Evet", "Hayır");
                if (!ok) return;

                // Eğer düzenlenen todo siliniyorsa edit moddan çık
                if (_editingTodo != null && _editingTodo.Id == todo.Id)
                    ExitEditMode();

                await TodoBL.DeleteTodoAsync(uid, todo.Id);
                _todos.Remove(todo);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
    }
}
