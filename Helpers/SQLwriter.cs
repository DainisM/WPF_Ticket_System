using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace TEC_MegaOpgave.Helpers
{
    class SQLwriter
    {
        SQLreaders sqlreader = new SQLreaders();

        /// <summary>
        /// ConnectionString for at få adgang til databasen
        /// </summary>
        private static string ConnectionString = "Data Source=db.accradio.dk,1433; " +
            "Initial Catalog = helpservicedb; User ID = **********; Password= **********5;";

        /// <summary>
        /// De forskellige variabler
        /// </summary>
        public int Checklistid;
        public int PartsID;
        public string EmpID;

        /// <summary>
        /// Metode som sætter checkpoints ind i databsen
        /// </summary>
        public void InsertChecklist()
        {
            int checklistid = Convert.ToInt32(System.Windows.Application.Current.Resources["checklistID"]);

            string statement = "insert into ChecklistPoints values (@instruction, 0, @checklistID);";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(statement, con))
            {
                cmd.Parameters.AddWithValue("@instruction", TicketWindow.ChecklistToDO);
                cmd.Parameters.AddWithValue("@checklistID", checklistid);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Metode som sætter ChecklistName i database, samt update Ticket tabellen og sætter ChecklistName ID ind i Ticket´en
        /// </summary>
        public void InsertChecklistName()
        {
            // Her vi sætter ind data (checklist navn) i databsen - Checklist tabellen
            string statement = "insert into Checklist values (@checkname);";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(statement, con))
            {
                cmd.Parameters.AddWithValue("@checkname", TicketWindow.Checklistname);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            // Vi hæver alle data fra databsen - Checklist tabellen med det navne som vi har lige sæt ind
            string sql = "Select * from Checklist where ChecklistName = @checlistkname";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd2 = new SqlCommand(sql, con))
            {
                cmd2.Parameters.AddWithValue("@checlistkname", TicketWindow.Checklistname);
                con.Open();

                // Aflæset dens ID og sætter ind i variablen
                SqlDataReader rdr2 = null;
                try
                {
                    cmd2.Connection = con;
                    rdr2 = cmd2.ExecuteReader();
                    while (rdr2.Read())
                    {
                        Checklistid = Convert.ToInt32(rdr2["ChecklistID"]);
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
            // Opdatere Ticket tabellens ChecklistID med den ID vi fik fra reader
            string updateStatement = "update Ticket set ChecklistID = @checkID where TicketID = " + MainWindow.id + ";";
            using (var con2 = new SqlConnection(ConnectionString))
            using (var cmd3 = new SqlCommand(updateStatement, con2))
            {
                cmd3.Parameters.AddWithValue("@checkID", Checklistid);
                con2.Open();
                cmd3.ExecuteNonQuery();
            }
        }
        
        /// <summary>
        /// Opdatere Ticket status for den ticket vi er igang nu
        /// </summary>
        public void UpdateTicketStatus()
        {
            string statement = "Update Ticket set TicketStatus = @tstatus where TicketID = " + MainWindow.id + ";";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(statement, con))
            {
                cmd.Parameters.AddWithValue("@tstatus", TicketWindow.TicketStatus);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Opdatere Ticket test status for den ticket vi er igang nu
        /// </summary>
        public void UpdateTicketTest()
        {
            string statement = "Update Ticket set TicketTest = @teststatus where TicketID = " + MainWindow.id + ";";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(statement, con))
            {
                cmd.Parameters.AddWithValue("@teststatus", TicketWindow.TestStatus);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Opdatere checklist navn på tabellen Checklist for den ticket som har det samme ChecklistID som tabellen Cheacklist.ChecklistID
        /// </summary>
        public void UpdateChecklistName()
        {
            string statement = "UPDATE Checklist SET ChecklistName = @clistname from Checklist INNER JOIN Ticket on Ticket.ChecklistID = Checklist.ChecklistID where TicketID = " + MainWindow.id + ";";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(statement, con))
            {
                cmd.Parameters.AddWithValue("@clistname", TicketWindow.Checklistname);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Opdatere EmployeeID for den ticket vi er igang nu på grund af den indtastede EmpInitials
        /// </summary>
        public void UpdateEmployee()
        {
            string sql = "select EmployeeID from Employee where EmpInitials = '" + TicketWindow.EmpName + "';";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                con.Open();
                SqlDataReader rdr = null;
                try
                {
                    cmd.Connection = con;
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        EmpID = rdr["EmployeeID"].ToString();
                    }
                }
                finally
                {
                    if (rdr != null)
                    {
                        rdr.Close();
                    }
                    if (con != null)
                    {
                        con.Close();
                    }
                }
            }

            string sql2 = "UPDATE Ticket Set EmployeeID = @empid where TicketID = " + MainWindow.id + ";";
            using (var con2 = new SqlConnection(ConnectionString))
            using (var cmd2 = new SqlCommand(sql2, con2))
            {
                cmd2.Parameters.AddWithValue("@empid", EmpID);
                con2.Open();
                cmd2.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Opdatere Pickup tid for den ticket vi er igang nu
        /// </summary>
        public void UpdatePickup()
        {
            string sql = "UPDATE Ticket Set Pickup = @pickupDate where TicketID = " + MainWindow.id + ";";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@pickupDate", TicketWindow.PickupDate);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Opdatere Solutin for den ticket vi er igang nu
        /// </summary>
        public void UpdateSolution()
        {
            string sql = "UPDATE Ticket Set Solution = @solution where TicketID = " + MainWindow.id + ";";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@solution", TicketWindow.TicketSolution);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Opdatere tabellen Parts, hvor vi sætter PartsList (navne) for den ticket vi er igang nu med det samme PartsID som tabellen Parts.PartsID
        /// </summary>
        public void UpdateParts()
        {
            string sql = "UPDATE Parts SET PartsList = @updateParts from Parts INNER JOIN Ticket on Ticket.PartsID = Parts.PartsID where TicketID = " + MainWindow.id + ";";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@updateParts", TicketWindow.TicketParts);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Opdatere CustomerApproval i tabellen Parts, for den ticket vi er igang nu med det samme PartsID som i tabellen Parts.PartsID
        /// </summary>
        public void UpdateCustomerApproval()
        {
            string sql = "UPDATE Parts SET CustomerApproval = @custApproval from Parts INNER JOIN Ticket on Ticket.PartsID = Parts.PartsID where TicketID = " + MainWindow.id + ";";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@custApproval", TicketWindow.CustomerPartsApproval);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Sætter data(PartsList - navne) i tabellen Parts
        /// </summary>
        public void InsertParts()
        {
            // Her sætter vi data ind i tabellen
            string statement = "insert into Parts (PartsList) values (@partsList);";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(statement, con))
            {
                cmd.Parameters.AddWithValue("@partsList", TicketWindow.TicketParts);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            // Hæver vi data fra tabellen Parts med PartsList som vi har lige sæt ind
            string sql = "Select * from Parts where PartsList = @partsl";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd2 = new SqlCommand(sql, con))
            {
                cmd2.Parameters.AddWithValue("@partsl", TicketWindow.TicketParts);
                con.Open();
                // Aflæser den data og sætter i variablen (PartsID)
                SqlDataReader rdr2 = null;
                try
                {
                    cmd2.Connection = con;
                    rdr2 = cmd2.ExecuteReader();
                    while (rdr2.Read())
                    {
                        PartsID = Convert.ToInt32(rdr2["PartsID"]);
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
            // Opdatere tabellen Ticket, sætter PartsID fra den variable som vi har sæt aflæste data ind, så Ticket.PartsID er lige med Parts.PartsID for denne ticket
            string updateStatement = "update Ticket set PartsID = @partsid where TicketID = " + MainWindow.id + ";";
            using (var con2 = new SqlConnection(ConnectionString))
            using (var cmd3 = new SqlCommand(updateStatement, con2))
            {
                cmd3.Parameters.AddWithValue("@partsid", PartsID);
                con2.Open();
                cmd3.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Opdatere status på en checkpoint for den ticket som vi er igang nu
        /// </summary>
        public void UpdateCheckpoints(int checkDone, int checkID)
        {            
            string sql = "update ChecklistPoints set CheckDone = @checkDone from ChecklistPoints inner join Ticket on Ticket.ChecklistID = ChecklistPoints.ChecklistID where TicketID = " + MainWindow.id + " and CheckID = @checkID";
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@checkDone", checkDone);
                cmd.Parameters.AddWithValue("@checkID", checkID);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
