using System.Windows;
using TEC_MegaOpgave.Helpers;

namespace TEC_MegaOpgave
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        SQLreaders sqlreader = new SQLreaders();
        SQLwriter sqlwriter = new SQLwriter();
        SQLvalidater sqlvalidater = new SQLvalidater();

        public LoginWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 2 variabler som vil gemme brugerens indtastning
        /// </summary>
        public static string UsernameInput { get; set; }
        public static string PasswordInput { get; set; }

        /// <summary>
        /// Metode som vil sørge for at der er en bruger i database med de brugernavn og adgangskode
        /// som brugeren har indtastede
        /// </summary>
        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            UsernameInput = UsernameInputBox.Text;
            PasswordInput = PasswordInputBox.Password;

            if (UsernameInput != null && PasswordInput != null)
            {
                sqlvalidater.LoginValidate();

                if (sqlvalidater.loginSuccessful)
                {
                    Window mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password");
                }
            }
        }
    }
}
