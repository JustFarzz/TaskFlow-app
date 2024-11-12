using Microsoft.Maui.Controls;
using TaskFlow.Services;

namespace TaskFlow.Views;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseService _db;
    private bool isPasswordVisible = false;
    public LoginPage()
    {
        InitializeComponent();
        _db = new DatabaseService();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {

        var username = UsernameEntry.Text;
        var password = PasswordEntry.Text;

        var user = _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user != null)
        {
            await Navigation.PushAsync(new ViewTaskPage(user));
        }
        else
        {
            await DisplayAlert("Error", "Invalid Credentials", "OK");
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    private void OnShowPasswordClicked(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible; // Toggle visibility
        PasswordEntry.IsPassword = !isPasswordVisible; // Toggle IsPassword property

        // Ganti ikon berdasarkan visibilitas password
        ShowPasswordButton.Source = isPasswordVisible ? "eye_open.png" : "eye_close.png";
    }

    private async void ForgotPasswordTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ForgotPasswordPage());
    }

    void OnEntryFocused(object sender, EventArgs e)
    {
        var entry = (Entry)sender;
        entry.BackgroundColor = Colors.Transparent;
    }






}