using System;
using System.Windows;
using TEC_MegaOpgave.Helpers;

namespace TEC_MegaOpgave
{
    /// <summary>
    /// Interaction logic for TicketWindow.xaml
    /// </summary>
    public partial class TicketWindow : Window
    {
        SQLreaders sqlreader = new SQLreaders();
        SQLwriter sqlwriter = new SQLwriter();
        SQLvalidater sqlvalidater = new SQLvalidater();

        /// <summary>
        /// Variabler
        /// </summary>
        public static string ChecklistToDO;
        public static int checklstID;
        public static string Checklistname;
        public static string TicketStatus;
        public static string TestStatus;
        public static string ClistName;
        public static string EmpName;
        public static DateTime PickupDate;
        public string Datepicker;
        public static string TicketSolution;
        public static string TicketParts;
        public static bool CustomerPartsApproval;
        public string CustomerFullName;

        public TicketWindow()
        {
            InitializeComponent();
            InitializeTicketData();
            InitializeParts();
            InitializeChecklist();
            DataContext = this;
        }

        public class Checkpoints
        {
            public int ChecklistID { get; set; }
            public int CheckID { get; set; }
            public bool IsComplete { get; set; }
            public string CheckpointName { get; set; }
        }

        /// <summary>
        /// Metode som sætter og viser alle de info om ticket i de nødvendige steder
        /// </summary>
        public void InitializeTicketData()
        {
            // Kalder på de forskellige metoder for at få data fra databasen
            sqlreader.ShowThisTicket();
            sqlvalidater.PartsValidate();
            sqlvalidater.ChecklistNameValidate();

            // Vi sætter kundens fornavn og efternavn sammen
            CustomerFullName = sqlreader.CustomerName + " " + sqlreader.CustomerSurname;

            // Sætter data i de forskellige text boxes
            CustomerNameBox.Text = CustomerFullName;
            CustomerAddressBox.Text = sqlreader.CustomerAddress;
            CustomerMobilenummberBox.Text = sqlreader.CustomerNummber;
            CustomerEmailBox.Text = sqlreader.CustomerEmail;
            FabricateBox.Text = sqlreader.HardwareName;
            ModelBox.Text = sqlreader.HardwareModel;
            ProblemBox.Text = sqlreader.HardwareProblem;
            TechnicalIdBox.Text = sqlreader.TechnicalInfo;
            StatusBox.Text = sqlreader.StatusInfo;
            TestBox.Text = sqlreader.TestInfo;
            PickUpDate.Text = sqlreader.PickupInfo;
            SolutionBox.Text = sqlreader.ProblemSolution;
        }

        /// <summary>
        /// Metode som fremviser data fra Parts tabellen og sætter den ind i textbox og
        /// viser 2 radiobuttons om kundens godkendelse eller nej til dette list over dele
        /// </summary>
        public void InitializeParts()
        {
            if (sqlvalidater.PartsAvailable)
            {
                sqlreader.ShowParts();
                PartsBox.Text = sqlreader.PartsList;

                if (sqlreader.CustomerApproval == false)
                {
                    Declined.IsChecked = true;
                }
                else if (sqlreader.CustomerApproval == true)
                {
                    Approved.IsChecked = true;
                }
                else
                {
                    Declined.IsChecked = false;
                    Approved.IsChecked = false;
                }
            }
        }
        
        /// <summary>
        /// Metode som viser Checklisten navn og de forskellige checkpoints
        /// </summary>
        public void InitializeChecklist()
        {
            if (sqlvalidater.ChecklistNameValid)
            {
                sqlreader.ShowChecklistName();
                ChecklistNameBox.Text = sqlreader.ChecklistName;
            }

            sqlreader.ShowChecklist();
            SeeCheckponts.ItemsSource = sqlreader.checkpoints;
        }

        /// <summary>
        /// Knappen som vil lukke denne vidue
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Knappen som vil tilføj checkpoints til databasen
        /// </summary>
        private void AddCheckpointButton_Click(object sender, RoutedEventArgs e)
        {
            Checklistname = ChecklistNameBox.Text;
            if (sqlvalidater.ChecklistNameValid)
            {
                ChecklistToDO = TypeCheckpointBox.Text;
                sqlwriter.InsertChecklist();
                sqlreader.checkpoints.Clear();
                sqlreader.ShowChecklist();
            }
            else
            {
                MessageBox.Show("Please first insert and add a checklist name!");
            }
            
        }

        /// <summary>
        /// Knappen som vil tilføj checklist navn til databasen
        /// </summary>
        private void AddChecklistName_Click(object sender, RoutedEventArgs e)
        {
            Checklistname = ChecklistNameBox.Text;

            if (sqlvalidater.ChecklistNameValid)
            {
                sqlwriter.UpdateChecklistName();
                sqlreader.ShowChecklistName();
                ChecklistNameBox.Text = sqlreader.ChecklistName;
                MessageBox.Show("Checklist name changed!");
            }
            else
            {
                sqlwriter.InsertChecklistName();
                MessageBox.Show("Checklist name added!");
            }
        }

        /// <summary>
        /// Knappen som vil gemme alle de indtastede data i databasen
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Vi vil gå igennem alle de checkpoints som er checked og gemme ændring i database
            foreach (var item in sqlreader.checkpoints)
            {
                sqlwriter.UpdateCheckpoints(Convert.ToInt16(item.IsComplete), item.CheckID);
            }

            // Gemmer/opdatere data fra datetimepicker i databasen hvis den er ikke tomt
            Datepicker = PickUpDate.Text;
            if (Datepicker != "")
            {
                PickupDate = DateTime.Parse(Datepicker);
                sqlwriter.UpdatePickup();
            }

            // Gemmer/opdatere ticket status fra dropdown list i database
            TicketStatus = StatusBox.Text;

            // Gemmer/opdatere ticket test status fra dropdown list i database
            TestStatus = TestBox.Text;

            // Hvis feltet (hvor der skal stå med.arb. initialer) er ikke tomt gemmer/opdatere vi medarbejderens initialer på denne ticket tabel i database
            if (TechnicalIdBox.Text != "")
            {
                EmpName = TechnicalIdBox.Text;
                sqlvalidater.EmpInitialsValidate();
                if (sqlvalidater.EmpInitialsAvailable)
                {
                    sqlwriter.UpdateEmployee();
                }
                else
                {
                    throw new Exception();
                }
            }

            // Gemmer/opdatere information om solutin på denne ticket ud fra hvad er srevet i textbox
            TicketSolution = SolutionBox.Text;

            // Gemmer/opdatere information om de dele som skal skiftes og om kundens godkendelse til de dele
            sqlvalidater.PartsValidate();
            if(sqlvalidater.PartsAvailable)
            {
                TicketParts = PartsBox.Text;
                if (Approved.IsChecked == true)
                {
                    CustomerPartsApproval = true;
                }
                else if (Declined.IsChecked == true)
                {
                    CustomerPartsApproval = false;
                }

                sqlwriter.UpdateParts();
            }
            else
            {
                TicketParts = PartsBox.Text;
                if (Approved.IsChecked == true)
                {
                    CustomerPartsApproval = true;
                }
                else if (Declined.IsChecked == true)
                {
                    CustomerPartsApproval = false;
                }
                sqlwriter.InsertParts();
            }

            // Kalder på de metoder som vil tager de data og opdatere dem i database
            sqlwriter.UpdateTicketStatus();
            sqlwriter.UpdateTicketTest();
            sqlwriter.UpdateSolution();      
            sqlwriter.UpdateCustomerApproval();

            // MessageBox som vil pop op når man trykker på "Save" knappen
            MessageBox.Show("Changes saved!");
        }
    }
}
