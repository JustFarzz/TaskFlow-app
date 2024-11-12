using Microsoft.Maui.Controls;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Views
{
    public partial class RegisterPage : ContentPage
    {
        private bool isPasswordVisible = false;
        private readonly DatabaseService _db;

        public RegisterPage()
        {
            InitializeComponent();
            _db = new DatabaseService();
        }

        private async void OnKeLogin(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

        private void OnShowPasswordClick(object sender, EventArgs e)
        {
            isPasswordVisible = !isPasswordVisible; // Toggle visibility
            PasswordEntry.IsPassword = !isPasswordVisible; // Toggle IsPassword property

            // Change icon based on password visibility
            ShowPasswordButton.Source = isPasswordVisible ? "eye_open.png" : "eye_close.png";
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text;
            var email = EmailEntry.Text; // Ambil nilai dari EmailEntry
            var password = PasswordEntry.Text;
            //var confirmPassword = ConfirmPasswordEntry.Text;

            // Validasi email
            if (string.IsNullOrWhiteSpace(email))
            {
                await DisplayAlert("Error", "Email is required.", "OK");
                return; // Hentikan proses jika email tidak diisi
            }

            // Validasi username dan password
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please enter both username and password", "OK");
                return;
            }

            // Validasi password
            /*if (password != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }*/

            // Cek apakah username sudah ada
            var existingUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                await DisplayAlert("Error", "Username already exists", "OK");
                return;
            }

            // Tambahkan pengguna baru ke database
            var newUser = new User
            {
                Username = username,
                Email = email, // Simpan email juga
                Password = password
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();

            await DisplayAlert("Success", "User registered successfully", "OK");
            await Navigation.PopAsync();
        }

    }
}
