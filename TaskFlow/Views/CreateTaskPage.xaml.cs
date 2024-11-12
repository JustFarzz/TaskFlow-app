using Microsoft.Maui.Controls;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Views;

public partial class CreateTaskPage : ContentPage
{
    private readonly DatabaseService _db;
    private readonly User _user;

    public CreateTaskPage(User user)
	{
		InitializeComponent();
        _db = new DatabaseService();
        _user = user;

    }

    private void OnCreateClicked(object sender, EventArgs e)
    {
        var title = TitleEntry.Text;
        var description = DescriptionEditor.Text;

        // Validasi title
        if (string.IsNullOrWhiteSpace(title))
        {
            DisplayAlert("Error", "Title is required", "OK");
            return;
        }



        var task = new TaskItem
        {
            Title = title,
            Description = description,
            IsCompleted = false,
            UserId = _user.Id,
        };

        _db.TaskItems.Add(task);
        _db.SaveChanges();

        DisplayAlert("Success", "Task created successfully", "OK");
        Navigation.PopAsync();
    }
}