namespace TaskFlow.Views
{
    public partial class ForgotPasswordPage : ContentPage
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        private async void SignUp(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            // Mengambil teks dari EmailEntry (yang terdefinisi di XAML)
            string email = EmailEntry.Text;

            if (!string.IsNullOrEmpty(email))
            {
                // Tambahkan logika pengiriman email reset password di sini
                await DisplayAlert("Success", "A password reset link has been sent to your email.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Please enter a valid email address.", "OK");
            }
        }
    }
}
