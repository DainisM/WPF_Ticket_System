using System.Windows;
using System.Windows.Controls;
using TEC_MegaOpgave.Helpers;

namespace TEC_MegaOpgave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SQLreaders sqlreader = new SQLreaders();
        SQLwriter sqlwriter = new SQLwriter();
        SQLvalidater sqlvalidater = new SQLvalidater();

        public static int id;

        public MainWindow()
        {

            InitializeComponent();

            /// <summary>
            /// Starter ShowEmployeeName metode fra SQLreader classe
            /// og bagefter viser dens medarbejderens navne i EmployeeNameBox på 
            /// grund af hvem der har logged in
            /// </summary>
            sqlreader.ShowEmployeeName();
            EmployeeNameBox.Text = sqlreader.EmployeeName;

            /// <summary>
            /// Kalder på ShowAllTickets metode fra SQLreader classe
            /// og sætter alle tickets som der er i listen i en ItemsSource
            /// vindue i horizontale view
            /// </summary>
            sqlreader.ShowAllTickets();
            DisplayTickets.ItemsSource = sqlreader.tickets;
        }

        /// <summary>
        /// Knappen som vil lukke ned denne Window og kom tilbage til LoginWindow
        /// </summary>
        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            Window loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Knappen som vil åbne TicketWindow som indeholder de information de ticket
        /// </summary>
        public void ShowTicket_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            AllTickets ticket = btn.DataContext as AllTickets;
            id = ticket.TicketID;
            
            Window ticketWindow = new TicketWindow();
            ticketWindow.Show();
        }

        /// <summary>
        /// Knappen som refresher vores ItemsControl og viser alle tickets og hvis der er nye tickets i vores database 
        /// mens medarbejderen var igang vil de bliver vist
        /// </summary>
        private void RefreshView_Click(object sender, RoutedEventArgs e)
        {
            sqlreader.tickets.Clear();
            sqlreader.ShowAllTickets();
        }

        public class AllTickets
        {
            public int TicketID { get; set; }
            public string Subject { get; set; }
            public string Date { get; set; }
            public string Status { get; set; }
            public string Technical { get; set; }
            public string PickupTime { get; set; }
        }
    }
}
