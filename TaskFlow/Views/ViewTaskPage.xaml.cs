using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Views;

public partial class ViewTaskPage : ContentPage
{
    private readonly DatabaseService _db;
    private readonly User _user;

    // Koleksi observasi untuk tugas-tugas
    private ObservableCollection<TaskItem> _allTasks;

    public ViewTaskPage(User user)
    {
        InitializeComponent();
        _db = new DatabaseService();
        _user = user;
    }

    // Ketika halaman muncul
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadTasks(); // Memuat tugas dari database

    }



    // Memuat tugas berdasarkan ID user
    private void LoadTasks()
    {
        var tasks = _db.TaskItems
                       .Where(t => t.UserId == _user.Id && !t.IsDeleted) // Hanya ambil tugas yang belum dihapus
                       .ToList();
        _allTasks = new ObservableCollection<TaskItem>(tasks);
        TasksListView.ItemsSource = _allTasks;
    }


    private async void ViewClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ViewTaskPage(_user));
    }

    private async void ViewCompleted(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CompletedTasksPage(_user));
    }

    private async void ViewUncompleted(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UncompletedTaskPage(_user));
    }

    private async void ViewTrash(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TrashPage());
    }






    // Ketika tombol "Create New Task" ditekan
    private async void OnCreateTaskClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreateTaskPage(_user));
    }



    // Ketika tombol Complete/Uncomplete ditekan
    private async void OnCompleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var task = button?.BindingContext as TaskItem;

        if (task != null)
        {
            task.IsCompleted = !task.IsCompleted;
            task.Status = task.IsCompleted ? "completed" : "uncompleted";

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
            bool confirm = await DisplayAlert("Confirm", $"Move task '{task.Title}' to trash?", "Yes", "No");
            if (confirm)
            {
                task.IsDeleted = true;
                _db.TaskItems.Update(task);
                _db.SaveChanges();  // Pastikan perubahan disimpan ke database

                LoadTasks();  // Perbarui tampilan tugas
            }
        }
    }

    // Ketika teks di SearchBar berubah
    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.ToLower();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            TasksListView.ItemsSource = _allTasks; // Tampilkan semua tugas
        }
        else
        {
            var filteredTasks = _allTasks
                .Where(task => task.Title.ToLower().Contains(searchText))
                .ToList();

            TasksListView.ItemsSource = new ObservableCollection<TaskItem>(filteredTasks);
        }
    }

    // Opsional: Fungsi untuk menangani item yang dipilih dalam ListView
    private void OnTaskSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // Implementasi opsional jika diperlukan
    }

    private void OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item == null)
            return;

    
        var tappedTask = e.Item as TaskItem; 
        OnEditClicked(tappedTask); 


        ((ListView)sender).SelectedItem = null;
    }

}
