using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace TEC_MegaOpgave.Helpers
{
    class SQLreaders
    {
        /// <summary>
        /// ConnectionString for at få adgang til databasen
        /// </summary>
        private static string ConnectionString = "Data Source=db.accradio.dk,1433; " +
            "Initial Catalog = helpservicedb; User ID = **********; Password= **********;";

        /// <summary>
        /// De forskellige variabler
        /// </summary>
        public int ChecklistID;
        public string ChecklistName;
        public bool CustomerApproval;
        public string PartsList;
        public string EmployeeName;
        public int TicketID;
        public ObservableCollection<MainWindow.AllTickets> tickets = new ObservableCollection<MainWindow.AllTickets>();
        public ObservableCollection<TicketWindow.Checkpoints> checkpoints = new ObservableCollection<TicketWindow.Checkpoints>();
        public string CustomerName;
        public string CustomerSurname;
        public string CustomerAddress;
        public string CustomerNummber;
        public string CustomerEmail;
        public string HardwareName;
        public string HardwareModel;
        public string HardwareProblem;
        public string StatusInfo;
        public string TestInfo;
        public string TechnicalInfo;
        public string PickupInfo;
        public string ProblemSolution;        

        /// <summary>
        /// Finder Parts liste og CustomerApproval i databasen
        /// </summary>
        public void ShowParts()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from Parts inner join Ticket on Ticket.PartsID = Parts.PartsID where TicketID = " + MainWindow.id + ";");
                cmd.Connection = con;
                con.Open();

                SqlDataReader rdr1 = null;
                try
                {
                    cmd.Connection = con;
                    rdr1 = cmd.ExecuteReader();
                    while (rdr1.Read())
                    {
                        PartsList = rdr1["PartsList"].ToString();
                        CustomerApproval = Convert.ToBoolean(rdr1["CustomerApproval"]);
                    }
                }
                finally
                {
                    if (rdr1 != null)
                    {
                        rdr1.Close();
                    }
                    if (con != null)
                    {
                        con.Close();
                    }
                }
            }
        }     

        /// <summary>
        /// Finder, aflæser og gemmer ChecklistName for denne ticket i en variable fra databasen hvor
        /// </summary>
        public void ShowChecklistName()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from Checklist inner join Ticket on Ticket.ChecklistID = Checklist.ChecklistID where TicketID = " + MainWindow.id + ";");
                cmd.Connection = con;
                con.Open();

                SqlDataReader rdr1 = null;
                try
                {
                    cmd.Connection = con;
                    rdr1 = cmd.ExecuteReader();
                    while (rdr1.Read())
                    {

                        ChecklistID = Convert.ToInt32(rdr1["ChecklistID"]);
                        System.Windows.Application.Current.Resources["checklistID"] = ChecklistID;
                        //int checklistid = Convert.ToInt32(System.Windows.Application.Current.Resources["checklistID"]);

                        ChecklistName = rdr1["ChecklistName"].ToString();
                    }
                }
                finally
                {
                    if (rdr1 != null)
                    {
                        rdr1.Close();
                    }
                    if (con != null)
                    {
                        con.Close();
                    }
                }
            }
        }         

        /// <summary>
        /// Finder CheckID, CheckpointName (checkpoint instruktion) og boolean om IsComplete
        /// </summary>
        public void ShowChecklist()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from ChecklistPoints inner join Ticket on Ticket.ChecklistID = ChecklistPoints.ChecklistID where TicketID = " + MainWindow.id + ";");
                cmd.Connection = con;
                con.Open();

                SqlDataReader rdr1 = null;
                try
                {
                    cmd.Connection = con;
                    rdr1 = cmd.ExecuteReader();
                    while (rdr1.Read())
                    {
                        TicketWindow.Checkpoints c = new TicketWindow.Checkpoints();
                        c.ChecklistID = Convert.ToInt32(rdr1["ChecklistID"]);
                        c.CheckID = Convert.ToInt32(rdr1["CheckID"]);
                        c.CheckpointName = rdr1["CheckInstruction"].ToString();
                        c.IsComplete = Convert.ToBoolean(rdr1["CheckDone"]);      
                        checkpoints.Add(c);
                    }
                }
                finally
                {
                    if (rdr1 != null)
                    {
                        rdr1.Close();
                    }
                    if (con != null)
                    {
                        con.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Finder medarbejderens navn fra database på grund at deres ID og gemmer den i en variable
        /// </summary>
        public void ShowEmployeeName()
        {
            string medarbejder = System.Windows.Application.Current.Resources["employeeID"].ToString();

            string sql = "Select EmpName from Employee where EmployeeID = @empID";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@empID", medarbejder);
                con.Open();

                SqlDataReader rdr2 = null;
                try
                {
                    cmd.Connection = con;
                    rdr2 = cmd.ExecuteReader();
                    while (rdr2.Read())
                    {
                        EmployeeName = rdr2[0].ToString();
                    }
                }
                finally
                {
                    if (rdr2 != null)
                    {
                        rdr2.Close();
                    }
                    if (con != null)
                    {
                        con.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Metode som finder information om en ticket og sætter den i en liste for at vise når den bliver kald
        /// </summary>

        public void ShowAllTickets()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {          
                SqlCommand cmd1 = new SqlCommand("Select * from Ticket inner join Customer on Ticket.CustomerID = Customer.CustomerID inner join Hardware on Ticket.HardwareID = Hardware.HardwareID left join Employee on Ticket.EmployeeID = Employee.EmployeeID order by TicketStatus DESC");
                cmd1.Connection = con;
                con.Open();

                SqlDataReader rdr1 = null;
                try
                {
                    cmd1.Connection = con;
                    rdr1 = cmd1.ExecuteReader();
                    while (rdr1.Read())
                    {
                        MainWindow.AllTickets t = new MainWindow.AllTickets();
                        t.TicketID = Convert.ToInt32(rdr1["TicketID"]);
                        t.Subject = rdr1["Problem"].ToString();
                        t.Date = Convert.ToDateTime(rdr1["TicketDate"]).ToString("yyy-MM-dd");
                        t.Status = rdr1["TicketStatus"].ToString();
                        t.Technical = rdr1["EmpInitials"].ToString();
                        string Pickupinfo = rdr1["Pickup"].ToString();
                        if (Pickupinfo != "")
                        {
                            t.PickupTime = Convert.ToDateTime(rdr1["Pickup"]).ToString("yyy-MM-dd");
                        }
                        else
                        {
                            t.PickupTime = "";
                        }
                        tickets.Add(t);
                    }
                }
                finally
                {
                    if (rdr1 != null)
                    {
                        rdr1.Close();
                    }
                    if (con != null)
                    {
                        con.Close();
                    }
                }
            }
        }
         
        /// <summary>
        /// Finder og viser alle information om denne ticket
        /// </summary>
        public void ShowThisTicket()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd2 = new SqlCommand("Select * from Ticket inner join Customer on Ticket.CustomerID = Customer.CustomerID inner join Hardware on Ticket.HardwareID = Hardware.HardwareID left join Employee on Ticket.EmployeeID = Employee.EmployeeID where TicketID = " + MainWindow.id+";");
                cmd2.Connection = con;
                con.Open();

                SqlDataReader rdr2 = null;
                try
                {
                    cmd2.Connection = con;
                    rdr2 = cmd2.ExecuteReader();
                    while (rdr2.Read())
                    {
                        TicketID = Convert.ToInt32(rdr2["TicketID"]);
                        System.Windows.Application.Current.Resources["thisticketID"] = TicketID;
                        //int thisticketid = Convert.ToInt32(System.Windows.Application.Current.Resources["thisticketID"]);
                        StatusInfo = rdr2["TicketStatus"].ToString();
                        TestInfo = rdr2["TicketTest"].ToString();
                        ProblemSolution = rdr2["Solution"].ToString();
                        PickupInfo = rdr2["Pickup"].ToString();
                        CustomerName = rdr2["CustomerName"].ToString();
                        CustomerSurname = rdr2["CustomerSurname"].ToString();
                        CustomerAddress = rdr2["CustomerAddress"].ToString();
                        CustomerNummber = rdr2["CustomerMobile"].ToString();
                        CustomerEmail = rdr2["CustomerEmail"].ToString();
                        HardwareName = rdr2["Fabricate"].ToString();
                        HardwareModel = rdr2["Model"].ToString();
                        HardwareProblem = rdr2["Problem"].ToString();
                        TechnicalInfo = rdr2["EmpInitials"].ToString();
                    }
                }
                finally
                {
                    if (rdr2 != null)
                    {
                        rdr2.Close();
                    }
                    if (con != null)
                    {
                        con.Close();
                    }
                }
            }
        }
    }
}
