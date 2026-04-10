using EasyShoeDemo.Lib.Services;
using System.Windows;
using System.Windows.Controls;

namespace EasyShoeDemo.Wpf
{
    public partial class LoginWindow : Window
    {
        private IMainApp _mainApp;
        public LoginWindow()
        {
            InitializeComponent();
            _mainApp = new MainApp();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = _mainApp.Authorize(LoginTextBox.Text.Trim(), PasswordBox.Password);
                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль!",
                        "Ошибка входа", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                var roleName = _mainApp.GetRoleName(user.RoleId);
                var window = new MainWindow(user, roleName);
                window.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow(null, "Гость");
            window.Show();
            Close();
        }
    }
}
