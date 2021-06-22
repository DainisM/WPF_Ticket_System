using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace TEC_MegaOpgave.Helpers
{
    class SQLvalidater
    {
        /// <summary>
        /// ConnectionString for at få adgang til databasen
        /// </summary>
        private static string ConnectionString = "Data Source=db.accradio.dk,1433; " +
            "Initial Catalog = helpservicedb; User ID = **********; Password= **********;";

        /// <summary>
        /// De forskellige variabler
        /// </summary>
        public bool PartsAvailable;
        public bool loginSuccessful;
        public bool ChecklistAvailable;
        public bool ChecklistNameValid;
        public bool EmpInitialsAvailable;
        public string EmployeeID;

        /// <summary>
        /// Metode som vil kigge i database for at finde ud om at der er en bruger med den brugernavn og adgangskode
        /// som brugeren har indtastet og vil returnere en boolean value
        /// </summary>
        public void LoginValidate()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from Employee where EmpUsername = @username and EmpPassword = @password;");
                cmd.Parameters.AddWithValue("@username", LoginWindow.UsernameInput);
                cmd.Parameters.AddWithValue("@password", LoginWindow.PasswordInput);
                cmd.Connection = con;
                con.Open();

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                loginSuccessful = ((ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0));
            }

            /// <summary>
            /// Vil tage medarbejderens ID fra database på grund af den brugernavn som der er blevet logged ind med
            /// og vil gemme i en variable som kan og vil bliver brugt bagefter i denne app
            /// </summary>
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("select EmployeeID from Employee where EmpUsername = '" + LoginWindow.UsernameInput + "';");
                cmd1.Connection = con;
                con.Open();

                SqlDataReader rdr1 = null;
                try
                {
                    cmd1.Connection = con;
                    rdr1 = cmd1.ExecuteReader();
                    while (rdr1.Read())
                    {
                        EmployeeID = rdr1[0].ToString();
                        System.Windows.Application.Current.Resources["employeeID"] = EmployeeID;
                        //string medarbejder = System.Windows.Application.Current.Resources["employeeID"].ToString();
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
        /// Metode som vil returnere en boolean value, når den bliver kaldt og har kigget i database for at se om findes
        /// der i forvejen en record i Parts tabbellen
        /// </summary>
        public void PartsValidate()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                SqlCommand cmd = new SqlCommand("select * from Parts inner join Ticket on Ticket.PartsID = Parts.PartsID where TicketID = " + MainWindow.id + ";");
                cmd.Connection = con;
                con.Open();

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                PartsAvailable = ((ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0));
            }
        }

        /// <summary>
        /// Metode som finder ud om er der en ChecklistName i Checklist tabbellen for denne ticket som vil returnere en boolean value
        /// </summary>
        public void ChecklistNameValidate()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                SqlCommand cmd = new SqlCommand("select * from Checklist inner join Ticket on Ticket.ChecklistID = Checklist.ChecklistID where TicketID = " + MainWindow.id + ";");
                cmd.Connection = con;
                con.Open();

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                ChecklistNameValid = ((ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0));
            }
        }

        /// <summary>
        /// Metode som vil returnere en boolean value, når den bliver kaldt og har kigget i database for at se om findes
        /// der i forvejen en record i ChecklistPoints tabbellen
        /// </summary>
        public void ChecklistValidate()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                SqlCommand cmd = new SqlCommand("select * from ChecklistPoints inner join Ticket on Ticket.ChecklistID = ChecklistPoints.ChecklistID where TicketID = " + MainWindow.id + ";");
                cmd.Connection = con;
                con.Open();

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                ChecklistAvailable = ((ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0));
            }
        }

        /// <summary>
        /// Metode som vil returnere en boolean value, når den bliver kaldt og har kigget i database for at se om findes
        /// der i forvejen en record med specifik EmpInitials i Employee tabel 
        /// </summary>
        public void EmpInitialsValidate()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from Employee where EmpInitials = '"+ TicketWindow.EmpName +"';");
                cmd.Connection = con;
                con.Open();

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                EmpInitialsAvailable = ((ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0));
            } 
        }
    }
}
