using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Views
{
    public partial class UncompletedTaskPage : ContentPage
    {
        private readonly DatabaseService _db;
        private readonly User _user;
        private ObservableCollection<TaskItem> _uncompletedTasks;
        private ObservableCollection<TaskItem> _filteredTasks; // Deklarasi _filteredTasks

        public UncompletedTaskPage(User user)
        {
            InitializeComponent();
            _db = new DatabaseService();
            _user = user;
            LoadTasks(); // Memuat tugas yang belum selesai saat halaman dibuka
        }

        // Memuat tugas yang belum selesai berdasarkan ID user
        private void LoadTasks()
        {
            var tasks = _db.TaskItems
                .Where(t => t.UserId == _user.Id && !t.IsCompleted && !t.IsDeleted)
                .ToList();
            _uncompletedTasks = new ObservableCollection<TaskItem>(tasks);

            // Inisialisasi _filteredTasks dengan koleksi awal yang sama seperti _uncompletedTasks
            _filteredTasks = new ObservableCollection<TaskItem>(_uncompletedTasks);

            // Tetapkan sumber data untuk ListView menggunakan _filteredTasks
            UncompletedTasksListView.ItemsSource = _filteredTasks;
        }

        // Ketika tombol Complete ditekan
        private async void OnCompleteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var task = button?.BindingContext as TaskItem;

            if (task != null)
            {
                task.IsCompleted = true;
                task.Status = "uncompleted";

                _db.TaskItems.Update(task);
                _db.SaveChanges();
                LoadTasks(); // Reload tugas setelah update
            }
        }

        // Ketika tombol Edit ditekan
        private async void OnEditClicked(TaskItem task)
        {
            if (task != null)
            {
                await Navigation.PushAsync(new EditTaskPage(task));
            }
        }

        // Ketika tombol Delete ditekan
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
                    LoadTasks(); // Reload tugas setelah delete
                }
            }
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var tappedTask = e.Item as TaskItem;
            OnEditClicked(tappedTask);

            ((ListView)sender).SelectedItem = null;
        }

        // Fungsi untuk menangani perubahan teks di SearchBar
        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue?.ToLower() ?? string.Empty; // Cek jika searchText null

            // Filter tugas yang sesuai dengan teks pencarian
            var filtered = _uncompletedTasks
                .Where(t => t.Title.ToLower().Contains(searchText) || t.Description.ToLower().Contains(searchText))
                .ToList();

            // Perbarui _filteredTasks dengan hasil pencarian
            _filteredTasks.Clear();
            foreach (var task in filtered)
            {
                _filteredTasks.Add(task);
            }

            // Jika searchText kosong, tampilkan semua tugas
            if (string.IsNullOrEmpty(searchText))
            {
                foreach (var task in _uncompletedTasks)
                {
                    if (!_filteredTasks.Contains(task))
                    {
                        _filteredTasks.Add(task);
                    }
                }
            }
        }

        // Opsional: Fungsi untuk menangani item yang dipilih dalam ListView
        private void OnUncompletedTaskSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Implementasikan logika jika diperlukan saat item dipilih
        }
    }
}
