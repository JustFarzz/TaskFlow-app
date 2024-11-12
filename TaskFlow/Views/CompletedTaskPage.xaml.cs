using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Views
{
    public partial class CompletedTasksPage : ContentPage
    {
        private readonly DatabaseService _db;
        private readonly User _user;

        // Koleksi observasi untuk tugas yang selesai
        private ObservableCollection<TaskItem> _completedTasks;

        // Koleksi observasi untuk tugas yang difilter
        private ObservableCollection<TaskItem> _filteredTasks;

        public CompletedTasksPage(User user)
        {
            InitializeComponent();
            _db = new DatabaseService();
            _user = user;
            LoadCompletedTasks(); // Muat tugas yang selesai saat halaman dibuka
        }

        private async void OnUncompleteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var task = button?.BindingContext as TaskItem;

            if (task != null)
            {
                task.IsCompleted = false;
                task.Status = "completed";

                _db.TaskItems.Update(task);
                _db.SaveChanges();
                LoadCompletedTasks(); // Reload tugas setelah update
            }
        }

        private async void OnEditClicked(TaskItem task)
        {
            if (task != null)
            {
                await Navigation.PushAsync(new EditTaskPage(task));
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var task = button?.BindingContext as TaskItem;

            if (task != null)
            {
                bool confirm = await DisplayAlert("Confirm", $"Delete task '{task.Title}'?", "Yes", "No");
                if (confirm)
                {
                    task.IsDeleted = true;
                    _db.TaskItems.Update(task);
                    _db.SaveChanges();
                    LoadCompletedTasks(); // Reload tugas setelah delete
                }
            }
        }

        // Memuat tugas yang sudah selesai berdasarkan ID user
        private void LoadCompletedTasks()
        {
            var tasks = _db.TaskItems
                .Where(t => t.UserId == _user.Id && t.IsCompleted && !t.IsDeleted)
                .ToList();

            // Inisialisasi koleksi observasi
            _completedTasks = new ObservableCollection<TaskItem>(tasks);
            _filteredTasks = new ObservableCollection<TaskItem>(_completedTasks);

            CompletedTasksListView.ItemsSource = _filteredTasks; // Menetapkan sumber data untuk ListView
        }

        // Ketika item dalam ListView dipilih
        private void OnCompletedTaskSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Implementasikan logika jika diperlukan saat item dipilih
        }

        // Event handler untuk perubahan teks di SearchBar
        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue?.ToLower() ?? string.Empty;

            // Filter koleksi tugas yang sesuai dengan teks pencarian
            var filtered = _completedTasks
                .Where(t => t.Title.ToLower().Contains(searchText) || t.Description.ToLower().Contains(searchText))
                .ToList();

            // Perbarui _filteredTasks dengan hasil filter
            _filteredTasks.Clear();
            foreach (var task in filtered)
            {
                _filteredTasks.Add(task);
            }

            // Jika searchText kosong, tampilkan semua tugas yang sudah selesai
            if (string.IsNullOrEmpty(searchText))
            {
                foreach (var task in _completedTasks)
                {
                    if (!_filteredTasks.Contains(task))
                    {
                        _filteredTasks.Add(task);
                    }
                }
            }
        }

        // Menangani item yang dipilih
        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var tappedTask = e.Item as TaskItem;
            OnEditClicked(tappedTask);

            ((ListView)sender).SelectedItem = null;
        }
    }
}
