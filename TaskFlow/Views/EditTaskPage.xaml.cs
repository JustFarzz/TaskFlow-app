using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Views;

public partial class EditTaskPage : ContentPage
{
	private readonly DatabaseService _db;
	private TaskItem _task;

	public EditTaskPage(TaskItem task)
	{
		InitializeComponent();

		_db = new DatabaseService();
		_task = task;

		TitleEntry.Text = _task.Title;
		DescriptionEditor.Text = _task.Description;

        CategoryPicker.Items.Add("Personal");
        CategoryPicker.Items.Add("Work");
        CategoryPicker.Items.Add("Optional");

    }

	private async void OnSaveClicked(object sender, EventArgs e)
	{
		var newTitle = TitleEntry.Text;
		var newDescription = DescriptionEditor.Text;

        if (string.IsNullOrWhiteSpace(newTitle) )
		{
			await DisplayAlert("Error", "Title is required", "OK");
			return;
		}

		_task.Title = newTitle;
		_task.Description = newDescription;

		_db.TaskItems.Update(_task);
		_db.SaveChanges();

        await DisplayAlert("Success", "Task updated successfully", "OK");
        await Navigation.PopAsync();
    }

	private async void OnCancelClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}