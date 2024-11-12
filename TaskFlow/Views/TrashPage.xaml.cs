using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using TaskFlow.Models;
using TaskFlow.Services;
using System.Diagnostics;

namespace TaskFlow.Views
{
    public partial class TrashPage : ContentPage
    {
        private readonly DatabaseService _db;
        private ObservableCollection<TaskItem> _deletedTasks;

        public TrashPage()
        {
            InitializeComponent();
            _db = new DatabaseService();
            LoadDeletedTasks(); // Muat tugas yang sudah dihapus
        }

        // Memuat tugas yang sudah dihapus (tetapi belum dihapus permanen)
        private void LoadDeletedTasks()
        {
            var tasks = _db.TaskItems
                .Where(t => t.IsDeleted) // Mengambil tugas yang sudah dihapus
                .ToList();

            if (tasks.Count == 0)
            {
                Debug.WriteLine("$ Found {tasks.Count} deleted tasks. ");
            }

            _deletedTasks = new ObservableCollection<TaskItem>(tasks);
            TrashTasksListView.ItemsSource = _deletedTasks; // Pastikan ini ada di XAML
        }

        // Fungsi untuk merestore task yang dihapus
        private async void OnRestoreClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var task = button?.BindingContext as TaskItem;

            if (task != null)
            {
                task.IsDeleted = false; // Mengembalikan status task
                _db.TaskItems.Update(task);
                _db.SaveChanges();
                LoadDeletedTasks(); // Reload setelah restore
            }
        }

        // Fungsi untuk menghapus secara permanen
        private async void OnDeletePermanentlyClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var task = button?.BindingContext as TaskItem;

            if (task != null)
            {
                bool confirm = await DisplayAlert("Confirm", $"Permanently delete task '{task.Title}'?", "Yes", "No");
                if (confirm)
                {
                    _db.TaskItems.Remove(task); // Hapus dari database secara permanen
                    _db.SaveChanges();
                    LoadDeletedTasks(); // Reload setelah delete permanen
                }
            }
        }

        // Fungsi pencarian dalam trash
        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue?.ToLower() ?? string.Empty;

            var filtered = _deletedTasks
                .Where(t => t.Title.ToLower().Contains(searchText) ||
                             t.Description?.ToLower().Contains(searchText) == true)
                .ToList();

            TrashTasksListView.ItemsSource = new ObservableCollection<TaskItem>(filtered);
        }

        // Fungsi opsional saat item dipilih dalam ListView
        private void OnTaskSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Logika opsional saat task dipilih
        }
    }
}
