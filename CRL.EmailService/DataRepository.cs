using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Notification ;


namespace CRL.EmailService
{
    public class ADOHealthMonitoringDataContext
    {
        private SqlConnection Healthconn ;
        private SqlConnection crlconn;
        private EventLog eventLogEmail;
        private const string HealthConn = "CBLHealthMonitoring";
        public static string GetConnectionString()
        {
            string mConnection = ConfigurationManager.ConnectionStrings[HealthConn].ConnectionString;
            //if (mConnection != null && mConnection.Length == 0)
            //{
            //    mConnection = ConfigurationManager.AppSettings[HealthConn];
            //}
            return mConnection;
        }
        public EventLog Log {
            get
            {
                if (eventLogEmail == null)
                {
                    eventLogEmail =  new EventLog();
                    return eventLogEmail;
                }
                else
                    return eventLogEmail;
            }
            private set { ;}
        }

        public SqlConnection HealthConnection
        {
            get
            {
                if (Healthconn == null)
                {
                    Healthconn = new SqlConnection(GetConnectionString());
                    return Healthconn;
                }
                else
                    return Healthconn;
            }
            private set { ;}
        }

        public SqlConnection CRLConnection
        {
            get
            {
                if (crlconn == null)
                {
                    crlconn = new SqlConnection(GetConnectionString());
                    return crlconn;
                }
                else
                    return crlconn;
            }
            private set { ;}
        }

//        public List<Email> GetPendingEmails()
//        {
//            List<Email> emails = new List<Email>();
//            SqlDataReader rdr = null;
           
//            SqlCommand cmd = null;

//            try
//            {
               
//                HealthConnection.Open();

//                // Set up a command with the given query and associate
//                // this with the current connection.
//                string CommandText = "SELECT dbo.Emails.Id, dbo.Emails.EmailBody, dbo.Emails.EmailSubject, dbo.Emails.EmailTo, dbo.Emails.EmailCc, dbo.Emails.EmailBcc, dbo.Emails.EmailFrom, dbo.Emails.IsSent, "+
//                  "dbo.Emails.NumRetries, dbo.Emails.CreatedBy, dbo.Emails.UpdatedBy, dbo.Emails.CreatedOn, dbo.Emails.UpdatedOn, dbo.Emails.IsActive, dbo.Emails.IsDeleted, "+
//                  "dbo.FileUploads.AttachedFile, dbo.FileUploads.AttachedFileName, dbo.FileUploads.AttachedFileType, dbo.FileUploads.AttachedFileSize "+
//"FROM     dbo.Emails INNER JOIN  dbo.FileUploads ON dbo.Emails.Id = dbo.FileUploads.EmailId WHERE  dbo.Emails.IsSent=0";
//                cmd = new SqlCommand(CommandText);
//                cmd.Connection = HealthConnection;

//                //// Add LastName to the above defined paramter @Find
//                //cmd.Parameters.Add(
//                //    new SqlParameter(
//                //    "@Find", // The name of the parameter to map
//                //    System.Data.SqlDbType.NVarChar, // SqlDbType values
//                //    20, // The width of the parameter
//                //    "LastName"));  // The name of the source column

//                //// Fill the parameter with the value retrieved 
//                //// from the text field
//                //cmd.Parameters["@Find"].Value = txtFind.Text;

//                // Execute the query
//                rdr = cmd.ExecuteReader();

//                // Fill the list box with the values retrieved
                
//                while (rdr.Read())
//                {
//                    Email = new Email {
//                         Id = (int)rdr["Id"] , EmailBody = rdr["EmailBody"].ToString (),    EmailSubject  = rdr["EmailSubject"].ToString (),
//                         EmailTo = rdr["EmailTo"].ToString () , EmailCc=rdr["EmailCc"].ToString (), EmailBcc =rdr["EmailBcc"].ToString (), EmailFrom =rdr["EmailFrom"].ToString () ,
//                     IsSent =  (bool)rdr["IsSent"], NumRetries= (int)rdr["NumRetries"] , CreatedBy = (int)rdr["CreatedBy"], UpdatedBy=(int)rdr["UpdatedBy"],CreatedOn=(DateTime)rdr["CreatedOn"],
//                     UpdatedOn= (DateTime)rdr["UpdatedOn"],  IsActive= (bool)rdr["IsActive"], IsDeleted = (bool)rdr["IsDeleted"]
//                    }
//            }
//            catch (Exception ex)
//            {
//                // Print error message
//                MessageBox.Show(ex.Message);
//            }
//            finally
//            {
//                // Close data reader object and database connection
//                if (rdr != null)
//                    rdr.Close();

//                if (con.State == ConnectionState.Open)
//                    con.Close();
//            }
//        }

        public void InsertError(string Message, string StackTrace, bool IsErrorOrSuccess, string Detail, EventLog eventLog=null)
        {

            try
            {
              
                //Create object of Command Class................
                SqlCommand cmd = new SqlCommand();

                //set Connection Property  of  Command object.............
                cmd.Connection = HealthConnection;
                //Set Command type of command object
                //1.StoredProcedure
                //2.TableDirect
                //3.Text   (By Default)

                cmd.CommandType = CommandType.Text;

                //Set Command text Property of command object.........

                cmd.CommandText = "Insert into EmailServiceNotification (Message, StackTrace, IsErrorOrSuccess, Detail, CreatedOn) values (@Message,@StackTrace,@IsErrorOrSuccess, @Detail,@CreatedOn)";

                //Assign values as `parameter`. It avoids `SQL Injection`
                cmd.Parameters.AddWithValue("Message", Message);
                cmd.Parameters.AddWithValue("StackTrace", StackTrace);
                cmd.Parameters.AddWithValue("IsErrorOrSuccess", IsErrorOrSuccess);
                cmd.Parameters.AddWithValue("Detail", Detail);
                cmd.Parameters.AddWithValue("CreatedOn", DateTime.Now);


                // Open Connection..................
                HealthConnection.Open();

                cmd.ExecuteNonQuery();
               


            }
            catch (Exception ex)
            {
                eventLog.WriteEntry(Message +  StackTrace,IsErrorOrSuccess==true? EventLogEntryType.Error : EventLogEntryType .Information );
                eventLog.WriteEntry(ex.Message + "\n Stack : " + ex.StackTrace, EventLogEntryType.Error );
               
            }
            finally
            {
                HealthConnection.Close();
            }


        }



    }
}
