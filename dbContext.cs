using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;


namespace MyOrgs2
{
    public class dbcontext
    {
        public string thisdomain = HttpContext.Current.Request.Url.Host;
        public static string thisCS(string domain)
        {
            if (findmatch(domain, "myorgs.carrollu.edu"))
            {
                return ConfigurationManager.ConnectionStrings["StudentOrgs"].ConnectionString;
            }
            else
            {
                return ConfigurationManager.ConnectionStrings["StudentOrgsDEV"].ConnectionString;
            }
        }

        private static bool findmatch(string thisxml, string thistext)
        {
            bool match = false;
            if (System.Text.RegularExpressions.Regex.IsMatch(thisxml, thistext))
            {
                match = true;
            }
            return match;
        }

        //Correct
        public static DataTable sp_GetRoleOfUser(string user, string password, string dom)
        {
            string CS = dom;
            string query = "SELECT Roles.Role_Name, Users.Is_Active FROM Roles INNER JOIN Users ON Roles.Role_ID = Users.Role_ID WHERE Users.User_ID = @ID AND Users.Password = @PW";
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", user);
                cmd.Parameters.AddWithValue("@PW", password);
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;

        }

        //Correct
        public static DataTable sp_GetUserOrgsByID(string user, string dom)
        {
            string CS = dom;
            string query = "SELECT UserOrgs.*, Organization.OrganizationName FROM UserOrgs INNER JOIN Users on Users.User_ID = UserOrgs.User_ID INNER JOIN Organization on Organization.Organization_ID = UserOrgs.Organization_ID WHERE UserOrgs.User_ID = @UserID AND inactive_date is NULL";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserID", user);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_GetActiveMembersForAllOrganization(string startdate, string enddate, string dom)
        {
            string CS = dom;
            DateTime start = Convert.ToDateTime(startdate);  
            DateTime end = Convert.ToDateTime(enddate);

            //if-else next line question
            string query = "IF Convert(Date,@EndDate) >= CONVERT(VARCHAR(10),GETDATE(),111) " + // "\n" +
                           "SELECT Organization.OrganizationName, Count(Distinct StudentRoster.Student_ID) as \"Number of Members\" FROM Student INNER JOIN StudentRoster ON Student.Student_ID = StudentRoster.Student_ID INNER JOIN Organization ON StudentRoster.Organization_ID = Organization.Organization_ID WHERE Student.Active = 1 AND StudentRoster.BeginDate >= @BeginDate AND StudentRoster.EndDate IS null GROUP BY Organization.OrganizationName ORDER BY Organization.OrganizationName  " + // "\n" +
                           "ELSE SELECT Organization.OrganizationName, Count(Distinct StudentRoster.Student_ID) as \"Number of Members\" FROM Student INNER JOIN StudentRoster ON Student.Student_ID = StudentRoster.Student_ID INNER JOIN Organization ON StudentRoster.Organization_ID = Organization.Organization_ID WHERE Student.Active = 1 AND StudentRoster.BeginDate >= @BeginDate AND StudentRoster.EndDate <= @EndDate GROUP BY Organization.OrganizationName ORDER BY Organization.OrganizationName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query1, con);
                    cmd.Parameters.AddWithValue("@BeginDate", start);
                    cmd.Parameters.AddWithValue("@EndDate", end);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetActiveMembersInfoForAllOrganization(string dom)
        {
            string CS = dom;
            string query = "SELECT Organization.OrganizationName,Student.FirstName,Student.LastName,Student.Email,Student.Carroll_ID,Student.GPAOrg,Student.GPABoard,MemberType.MemberName,Status.StatusName FROM Student INNER JOIN StudentRoster ON Student.Student_ID = StudentRoster.Student_ID INNER JOIN MemberType ON StudentRoster.Member_ID = MemberType.Member_ID " +
                + " INNER JOIN Status ON StudentRoster.Status_ID = Status.Status_ID INNER JOIN Organization ON StudentRoster.Organization_ID = Organization.Organization_ID WHERE Student.Active = 1 AND StudentRoster.EndDate IS NULL ORDER BY Organization.OrganizationName, Student.LastName, Student.FirstName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetActiveOrgList(string dom)
        {
            string CS = dom;
            string query = "SELECT OrgStatus.Organization_ID, OrganizationName FROM Organization INNER JOIN OrgStatus ON Organization.Organization_ID = OrgStatus.Organization_ID WHERE OrgStatus.EndDate is null AND OrgStatus.Status_ID in (1,2,4) --Proposed, Active, Probation ORDER BY OrganizationName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetAdvisor(string lastname,string firstname,int advisorID,string dom)
        {
            string CS = dom;
            string query = "SELECT LastName, FirstName, Email, PhoneNumber, OffCampus FROM Advisor WHERE Advisor_ID=@Advisor_ID OR LastName=@LastName OR FirstName=@FirstName ORDER BY LastName, FirstName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Advisor_ID", advisorID);
                    cmd.Parameters.AddWithValue("@LastName", lastname);
                    cmd.Parameters.AddWithValue("@FirstName", firstname);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetAdvisorByAdvID(int advisorID, string dom)
        {
            string CS = dom;
            string query = "SELECT LastName,FirstName, offCampus ,Advisor_ID FROM Advisor WHERE Advisor_ID = @Advisor_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Advisor_ID", advisorID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetAdvisorByID(int advisorID, string dom)
        {
            string CS = dom;
            string query = "SELECT LastName,FirstName, offCampus ,Advisor_ID FROM Advisor WHERE Advisor_ID = @Advisor_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Advisor_ID", advisorID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetAdvisorList(string dom)
        {
            string CS = dom;
            string query = "SELECT LastName, FirstName, Advisor_ID FROM Advisor ORDER BY LastName, FirstName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetAdvisorListForOrgID(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT LastName,FirstName, offCampus ,Advisor.Advisor_ID FROM OrgAdvisor INNER JOIN Advisor ON OrgAdvisor.Advisor_ID = Advisor.Advisor_ID WHERE OrgAdvisor.Organization_ID = @OrganizationID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@OrganizationID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetCommentsByEventID(int eventID, string dom)
        {
            string CS = dom;
            //Single Quote question
            string query = "SELECT Date, Text, UserID, COALESCE(CONVERT(VARCHAR(10), EventComments.Date, 101), '') ShortDate, EventComment_ID FROM EventComments WHERE Event_ID = @EventID ORDER BY Date DESC";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EventID", eventID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetCommentsByIncidentID(int incidentID, string dom)
        {
            string CS = dom;
            string query = "SELECT Date, Text,UserID FROM IncidentComments WHERE Incident_ID=@IncidentID ORDER BY Date DESC";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IncidentID", incidentID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetCommentsByOrgID(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT Date, Text,UserID FROM OrgComments WHERE Organization_ID=@OrganizationID ORDER BY DATE DESC";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@OrganizationID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetCommentsByServiceProjectID(int projID, string dom)
        {
            string CS = dom;
            string query = "SELECT Date, Text, UserID, COALESCE(CONVERT(VARCHAR(10), ServiceProjectComments.Date, 101), '') ShortDate, ServiceComment_ID FROM ServiceProjectComments WHERE ServiceProject_ID = @ServiceProject_ID ORDER BY Date DESC";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ServiceProject_ID", projID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetDeActivatedOrganizationList(string dom)
        {
            string CS = dom;
            string query = "SELECT Organization.OrganizationName,Convert(varchar(11),Convert(Date,OrgStatus.StartDate)) As 'Deactivated' FROM Organization INNER JOIN OrgStatus ON Organization.Organization_ID = OrgStatus.Organization_ID " +   
                           "INNER JOIN Status ON Status.Status_ID = OrgStatus.Status_ID WHERE OrgStatus.Status_ID = 5 ORDER BY OrganizationName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetDriverByStuID(int stuID,string dom)
        {
            string CS = dom;
            string query = "SELECT LastName,FirstName, Student_ID, CertifiedDriver,ApprovedDriver FROM Student WHERE Student_ID = @Student_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Student_ID", stuID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetDrivers(string dom)
        {
            string CS = dom;
            string query = "SELECT LastName,FirstName,Student_ID,CertifiedDriver,ApprovedDriver,FirstName +  ' ' + LastName as Name FROM Student WHERE (CertifiedDriver = 1 OR ApprovedDriver = 1) ORDER BY LastName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetDriversForEvent(int stuID1, int stuID2,string dom)
        {
            string CS = dom;
            string query = "SELECT LastName,FirstName, Student_ID, CertifiedDriver,ApprovedDriver FROM Student WHERE Student_ID = @Driver1 OR Student_ID = @Driver2";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Driver1", stuID1);
                    cmd.Parameters.AddWithValue("@Driver2", stuID2);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetEventByOrgIDDate(int orgID, string date, string dom)
        {
            string CS = dom;
            DateTime Date = Convert.ToDateTime(date);  
            string query = "SELECT * FROM OrgEvent WHERE Organization_ID=@Organization_ID AND Date=@Date";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Date", Date);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetEventDatesByOrgID(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT Date FROM OrgEvent WHERE Organization_ID=@Organization_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_GetEventDrivers_byEvent(int eventID, string dom)
        {
            string CS = dom;
            string query =  "SELECT LastName, FirstName, Student_ID, case \n when CertifiedDriver = 1 AND ApprovedDriver = 1 then  'Certified & Approved' \n when CertifiedDriver = 1 then 'Certified' \n when ApprovedDriver = 1 then 'Approved' \n else '' end DriverType FROM OrgEvent " +
	                        "INNER JOIN Student firststu on OrgEvent.Driver1 = firststu.Student_ID WHERE Event_ID = @EventID AND OrgEvent.Driver1 is not null \n" +  
	                        "UNION \n SELECT LastName, FirstName, Student_ID, case \n when CertifiedDriver = 1 AND ApprovedDriver = 1 then 'Certified & Approved' \n when CertifiedDriver = 1 then 'Certified' \n when ApprovedDriver = 1 then 'Approved' \n else '' end DriverType FROM OrgEvent " +  
	                        "INNER JOIN Student secondstu on OrgEvent.Driver2 = secondstu.Student_ID WHERE Event_ID = @EventID AND OrgEvent.Driver2 is not null";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EventID", eventID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetExecutiveMembersForAllOrganization(string dom)
        {
            string CS = dom;
            string query = "SELECT Organization.OrganizationName,Student.FirstName,Student.LastName, Student.Carroll_ID, MemberType.MemberName FROM Student " +  
                           "INNER JOIN StudentRoster ON Student.Student_ID = StudentRoster.Student_ID " +  
                           "INNER JOIN Organization ON StudentRoster.Organization_ID  = Organization.Organization_ID " +  
                           "INNER JOIN MemberType ON MemberType.Member_ID = StudentRoster.Member_ID " +  
                           "WHERE Student.Active = 1 AND MemberType.BoardMember = 1 AND StudentRoster.EndDate IS null " +  
                           "ORDER BY Organization.OrganizationName, Student.LastName, Student.FirstName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetExecutiveMembersForOrganizationID(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT Organization.OrganizationName,Student.FirstName,Student.LastName,Student.Carroll_ID,MemberType.MemberName FROM Student " +  
                           "INNER JOIN StudentRoster ON Student.Student_ID = StudentRoster.Student_ID " +  
                           "INNER JOIN Organization	ON StudentRoster.Organization_ID  = Organization.Organization_ID  " +  
                           "INNER JOIN MemberType ON MemberType.Member_ID = StudentRoster.Member_ID " +  
                           "WHERE Student.Active = 1 AND MemberType.BoardMember = 1 AND StudentRoster.EndDate IS NULL AND Organization.Organization_ID = @Organization_ID " +    
                           "ORDER BY Student.LastName, Student.FirstName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetExeMembersByOrgID(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT Student.LastName,Student.FirstName,Student.Student_ID ,MemberType.MemberName,MemberType.Member_ID,Student.Carroll_ID,StudentRoster.BeginDate,StudentRoster.PrimaryContact FROM Student " +  
                           "INNER JOIN StudentRoster ON Student.Student_ID = StudentRoster.Student_ID  " +  
                           "INNER JOIN MemberType ON MemberType.Member_ID = StudentRoster.Member_ID " +  
                           "INNER JOIN Status ON StudentRoster.Status_ID = Status.Status_ID " +  
                           "WHERE StudentRoster.Organization_ID = @Organization_ID AND StudentRoster.EndDate IS NULL AND MemberType.BoardMember = 1 AND Student.Active = 1";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_GetHistoricalOrgsByStudentID(int studentID, string dom)
        {
            string CS = dom;
            string query = "DECLARE @results table(OrganizationName varchar(100), Session char(2), Year int, MemberName varchar(20), BeginDate smalldatetime, Sort varchar(17)); " +  
                           "INSERT INTO @results (OrganizationName, Session, Year, MemberName, BeginDate, Sort) " +  
	                       "SELECT Organization.OrganizationName, StudentRoster.Session, StudentRoster.Year, MemberType.MemberName, StudentRoster.BeginDate, rtrim(ltrim(Session))+convert(char(4),Year)+'1' FROM dbo.Student " +  
                           "INNER JOIN dbo.StudentRoster on dbo.Student.Student_ID = dbo.StudentRoster.Student_ID " +  
                           "INNER JOIN dbo.Organization on dbo.Organization.Organization_ID = dbo.StudentRoster.Organization_ID " +  
                           "INNER JOIN dbo.MemberType on dbo.MemberType.Member_ID = dbo.StudentRoster.Member_ID " +  
                           "WHERE Carroll_ID = @Carroll_ID AND BoardMember = 1; " +  
                           "INSERT INTO @results (OrganizationName, Session, Year, MemberName, BeginDate, Sort) " +  
                           "SELECT Organization.OrganizationName, StudentRoster.Session, StudentRoster.Year, MemberType.MemberName, StudentRoster.BeginDate, rtrim(ltrim(Session))+convert(char(4),Year)+'2' FROM dbo.Student " +  
                           "INNER JOIN dbo.StudentRoster on dbo.Student.Student_ID = dbo.StudentRoster.Student_ID " +  
                           "INNER JOIN dbo.Organization on dbo.Organization.Organization_ID = dbo.StudentRoster.Organization_ID " +  
                           "INNER JOIN dbo.MemberType on dbo.MemberType.Member_ID = dbo.StudentRoster.Member_ID " +  
                           "WHERE Carroll_ID = @Carroll_ID AND BoardMember = 0 " +  
                           "AND NOT EXISTS(SELECT * FROM @results results WHERE Organization.OrganizationName = results.OrganizationName AND StudentRoster.Session = results.Session AND StudentRoster.Year = results.Year); " +  
                           "SELECT OrganizationName as Organization, MemberName as [Member Type], Session, Year FROM @results " +  
	                       "ORDER BY Sort, BeginDate DESC, MemberName, OrganizationName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Carroll_ID", studentID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetHistoryOfMembers(string dom)
        {
            string CS = dom;
            string query = "SELECT Organization.CX_Code,Student.Carroll_ID,MemberType.MemberName,StudentRoster.Session,StudentRoster.Year FROM Student " +  
                           "INNER JOIN StudentRoster ON Student.Student_ID = StudentRoster.Student_ID " +  
                           "INNER JOIN MemberType ON StudentRoster.Member_ID = MemberType.Member_ID " +  
                           "INNER JOIN Organization ON StudentRoster.Organization_ID  = Organization.Organization_ID " +  
                           "WHERE Student.Active = 1 " +  
	                       "Order By Organization.CX_Code";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetIncident(int orgID,int incidentID,string dom)
        {
            string CS = dom;
            string query = "SELECT Incident_ID, date, Time, Location, Incident, Result, Notes, Organization_ID, ReportedBy, Organization FROM Incident " +  
	                       "WHERE Organization_ID=@Organization_ID OR Incident=@Incident_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Incident_ID", incidentID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetIncidentByOrgIDDate(int orgID, DateTime date, string dom)
        {
            string CS = dom;
            /*
             *  if parameter is a string.
             *  String date;
             *  DateTime dt = Convert.ToDateTime(date);  
             */
            string query = "SELECT * FROM Incident WHERE Organization_ID=@Organization_ID AND Datetime=@Date";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@orgID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetIncidentComments(int IncidentID, string dom)
        {
            string CS = dom;
            string query = "SELECT Date, Text FROM IncidentComments WHERE Incident_ID=@Incident_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Incident_ID", IncidentID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetIncidentDatesByOrgID(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT Date FROM Incident WHERE Organization_ID=@Organization_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetMeetingForOrgSignedUP(int orgID, int eventID, string dom)
        {
            string CS = dom;
            string query = "SELECT * FROM RequiredEventMeetings " +  
	                       "INNER JOIN OrgRequiredEvents ON RequiredEventMeetings.MeetingID = OrgRequiredEvents.MeetingID " +  
	                       "WHERE RequiredEventID = @Event_ID AND OrganizationID = @Organization_ID AND Attended = 0";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetMeetingForOrgSignedUPForDate(int orgID,int eventID,DateTime date,string dom)
        {
            string CS = dom;
            /*
             *  if parameter is a string.
             *  String date;
             *  DateTime dt = Convert.ToDateTime(date);  
             */
            string query = "SELECT * FROM RequiredEventMeetings " +  
	                       "INNER JOIN OrgRequiredEvents ON RequiredEventMeetings.MeetingID = OrgRequiredEvents.MeetingID " +  
	                       "WHERE RequiredEventID=@Event_ID AND Organization_ID=@OrganizationID AND Attended = 0 AND MeetingDate=@Date";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Date", date);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetMeetingForOrgSignedUPForDate(int orgID, int eventID, string dom)
        {
            string CS = dom;
            string query = "SELECT RequiredEventMeetings.MeetingID,RequiredEventMeetings.RequiredEventID,RequiredEventMeetings.Session,RequiredEventMeetings.Year,RequiredEventMeetings.MeetingRoomNumber,RequiredEventMeetings.MeetingTime,RequiredEventMeetings.Notes,RequiredEventMeetings.MeetingBuilding,RequiredEventMeetings.MeetingDate	FROM RequiredEventMeetings " +  
	                       "LEFT OUTER JOIN (SELECT * FROM OrgRequiredEvents WHERE Organization_ID = @Organization_ID) ResultSet ON RequiredEventMeetings.MeetingID = ResultSet.MeetingID " +  
	                       "WHERE ResultSet.MeetingID IS NULL AND RequiredEventID = @Event_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetMeetingForReqEventID(int eventID, string dom)
        {
            string CS = dom;
            string query = "SELECT * FROM RequiredEventMeetings WHERE RequiredEventID = @Event_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetMembersByOrgID(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT Student.LastName, Student.FirstName, Student.Student_ID ,MemberType.MemberName,MemberType.Member_ID,Student.Carroll_ID,StudentRoster.BeginDate,StudentRoster.PrimaryContact FROM Student " +  
                           "INNER JOIN StudentRoster ON Student.Student_ID = StudentRoster.Student_ID " +  
                           "INNER JOIN MemberType ON MemberType.Member_ID = StudentRoster.Member_ID " +  
                           "INNER JOIN Status ON StudentRoster.Status_ID  = Status.Status_ID " +  
	                       "WHERE StudentRoster.Organization_ID = @Organization_ID AND StudentRoster.EndDate IS NULL AND MemberType.BoardMember!=1 AND Student.Active=1";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetMembersByStuID(int studentID, string dom)
        {
            string CS = dom;
            string query = "SELECT Student.LastName, Student.FirstName, Student.Student_ID ,MemberType.MemberName,MemberType.Member_ID,Student.Carroll_ID,StudentRoster.BeginDate,StudentRoster.PrimaryContact FROM Student " +  
                           "INNER JOIN StudentRoster ON Student.Student_ID = StudentRoster.Student_ID " +  
                           "INNER JOIN MemberType ON MemberType.Member_ID = StudentRoster.Member_ID " +  
                           "INNER JOIN Status ON StudentRoster.Status_ID  = Status.Status_ID " +  
                           "WHERE Student.Student_ID = @Student_ID 	AND StudentRoster.EndDate IS NULL";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Student_ID", studentID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetMemberTypeByID(int memID, string dom)
        {
            string CS = dom;
            string query = "SELECT MemberName, Description, AddDate, EndDate, BoardMember FROM MemberType " +
                           "WHERE Member_ID=@Member_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Member_ID", memID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetMemberTypeList(string dom)
        {
            string CS = dom;
            string query = "SELECT MemberName, Member_ID FROM MemberType";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetMyOrgs_OrgByID(int orgID,string dom)
        {
            string CS = dom;
            string query1 = "IF (SELECT count(*) FROM OrgAdvisor INNER JOIN Advisor ON OrgAdvisor.Advisor_ID = Advisor.Advisor_ID " +
			                "WHERE OrgAdvisor.Organization_ID = @Organization_ID AND OrgAdvisor.EndDate is NULL) > 1 " +  
                            "SELECT @Advisor = (SELECT min(Advisor.FirstName + ' ' + Advisor.LastName) FROM OrgAdvisor " +  
                            "INNER JOIN Advisor ON OrgAdvisor.Advisor_ID = Advisor.Advisor_ID " +  
                            "WHERE OrgAdvisor.Organization_ID = @Organization_ID AND OrgAdvisor.EndDate is NULL) " +  
                            "SELECT @Advisor = (SELECT Advisor.FirstName + ' ' + Advisor.LastName FROM OrgAdvisor " +  
                            "INNER JOIN Advisor ON OrgAdvisor.Advisor_ID = Advisor.Advisor_ID " +  
                            "WHERE OrgAdvisor.Organization_ID = @Organization_ID AND OrgAdvisor.EndDate is NULL) " +  
                            "SELECT  OrganizationName, Organization.Description, MeetingTime, MeetingDay, MeetingBuilding, MeetingRoom, MeetingFrequency, Organization.Category_ID, OAccount, ProjectCode, @Advisor AdvisorName, GreekOrg FROM Organization " +  
                            "INNER JOIN OrgCategory on Organization.Category_ID = OrgCategory.Category_ID " +   
	                        "WHERE Organization.Organization_ID = @Organization_ID";
            
            DataTable dt = new DataTable();
           try{
               using (SqlConnection con = new SqlConnection(CS))
               {
                   SqlCommand cmd = new SqlCommand(query, con);
                   cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                   SqlDataReader rdr = cmd.ExecuteReader();
                   dt.Load(rdr);
               }
               return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_GetMyOrgsEventList(string orgID, string dom)
        {
            string CS = dom;
            /*
             *  int organizationID;
             *  Int32.TryParse(orgID, out organizationID);
             */
            string query = "IF @Incl_Old IS NULL \n SELECT @Incl_Old = 'N' \n SELECT OrgEvent.Event_ID,OrgEvent.Organization_ID,COALESCE(CONVERT(VARCHAR(10), OrgEvent.Date, 101), '') EventDate,OrgEvent.Time EventTime,OrgEvent.Title,OrgEvent.OrganizationWorkedWith FROM OrgEvent " +
                            "WHERE OrgEvent.Organization_ID = @OrgID AND (COALESCE(OrgEvent.Date,getdate()) >= convert(varchar(2),month(getdate())) + '-' + convert(varchar(2),day(getdate())) + '-' + convert(varchar(4),year(getdate())) OR @Incl_Old = 'Y') " +  
	                        "ORDER BY OrgEvent.Organization_ID, EventDate, EventTime";
                       
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query2, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_GetMyOrgsReportRoster(string orgID, string filter, string dom)
        {
            string CS = dom;
            string query = "SELECT StudentRoster.Organization_ID,Student.Student_ID,(select Session + convert(char(40),Year) from Term) Term,Organization.OrganizationName,MemberType.MemberName,MemberType.Member_ID,Student.Carroll_ID,Student.FirstName,Student.LastName, " +
			               "CASE WHEN MemberName = 'President' THEN '1' \n WHEN MemberName = 'Vice President' THEN '2' \n WHEN MemberName = 'Treasurer' THEN '3' \n WHEN MemberName = 'Secretary' THEN '4' \n WHEN MemberName <> 'Member' THEN '5' \n ELSE '999' \n END sort_order, " +  
			               "CASE WHEN Student.ClassOf = 'SR' THEN 'Senior' \n WHEN Student.ClassOf = 'JR' THEN 'Junior' \n WHEN Student.ClassOf = 'SO' THEN 'Sophomore' \n WHEN Student.ClassOf IN ('FR','FF') THEN 'Freshman' \n WHEN Student.ClassOf = 'UN' THEN 'Transfer' \n WHEN Student.ClassOf = 'S' THEN 'Special' \n WHEN Student.ClassOf = 'GR' THEN 'Graduate' \n ELSE Student.ClassOf " +  "END classification " +
                           "FROM Student " +
	                       "INNER JOIN StudentRoster on Student.Student_ID = StudentRoster.Student_ID " +
	                       "INNER JOIN Organization on StudentRoster.Organization_ID = Organization.Organization_ID " +
	                       "INNER JOIN MemberType on StudentRoster.Member_ID = MemberType.Member_ID " +
	                       "WHERE StudentRoster.Organization_ID = @OrgID AND StudentRoster.EndDate is null AND (Student.ClassOf = @cl_filter OR (@cl_filter = 'FR' AND Student.ClassOf in ('FF','FR')) OR @cl_filter = '**') " +
	                       "ORDER BY Organization_ID, sort_order, MemberType.MemberName, LastName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@OrgID", orgID);
                    cmd.Parameters.AddWithValue("@cl_filter", filter);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_GetMyOrgsRoster(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT  StudentRoster.Organization_ID,Student.Student_ID,Student.Carroll_ID,Student.FirstName,Student.LastName,MemberType.MemberName,MemberType.Member_ID, " +
                           "CASE WHEN MemberName = 'President' THEN '1' \n WHEN MemberName = 'Vice President' THEN '2' \n  WHEN MemberName = 'Treasurer' THEN '3' \n WHEN MemberName = 'Secretary' THEN '4' \n WHEN MemberName = 'Member' THEN '5' \n ELSE '999' \n END sort_order " +  
                           "FROM Student " +
                           "INNER JOIN StudentRoster on Student.Student_ID = StudentRoster.Student_ID " +  
                           "INNER JOIN MemberType on StudentRoster.Member_ID = MemberType.Member_ID " +  
                           "WHERE Organization_ID = @OrgID AND StudentRoster.EndDate is null " +  
	                       "ORDER BY Organization_ID, sort_order, MemberType.MemberName, LastName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@OrgID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_GetMyOrgsServiceList(string orgID, string dom)
        {
            string CS = dom;
            char incl = 'N';
            /*
             *  int organizationID;
             *  Int32.TryParse(orgID, out organizationID);
             */
            string query = "IF @Incl_Old IS NULL \n SELECT @Incl_Old = 'N' \n SELECT ServiceProject.ServiceProject_ID,ServiceProject.Organization_ID,COALESCE(CONVERT(VARCHAR(10),ServiceProject.StartDate, 101), '') StartDate,ServiceProject.OnGoing,ServiceProject.Location,ServiceProject.Title,ServiceProject.HoursVolunteered,ServiceProject.CommunityPartner,ServiceProject.Planned_HoursVolunteered FROM ServiceProject " +  
                            "WHERE ServiceProject.Organization_ID = @OrgID AND (COALESCE(ServiceProject.StartDate,getdate()) >= convert(varchar(2),month(getdate())) + '-' + convert(varchar(2),day(getdate())) + '-' + convert(varchar(4),year(getdate()))OR @Incl_Old = 'Y') " + 
	                        "ORDER BY ServiceProject.Organization_ID, StartDate, TimeFrom";

            DataTable dt = new DataTable();
           try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@OrgID", orgID);
                    cmd.Parameters.AddWithValue("@Incl_Old", incl);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetNonExecutiveMembersForOrgID(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT Student.Student_ID,Student.LastName,Student.FirstName FROM Student " +  
                           "LEFT OUTER JOIN (" +  
		                   "SELECT StudentRoster.Student_ID FROM StudentRoster " +  
		                   "INNER JOIN MemberType ON StudentRoster.Member_ID = MemberType.Member_ID " +  
		                   "WHERE StudentRoster.Organization_ID = @Organization_ID AND MemberType.BoardMember = 1 AND StudentRoster.EndDate IS NULL) " +  
                           "RESULTSET ON Student.Student_ID = RESULTSET.Student_ID WHERE RESULTSET.Student_ID IS NULL AND Student.Active != 0 " +  
	                       "Order By Student.LastName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetNonMembersForOrgID(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT Student.Student_ID,Student.LastName,Student.FirstName FROM Student " +  
                           "LEFT OUTER JOIN ( SELECT StudentRoster.Student_ID FROM StudentRoster " +  
                           "INNER JOIN MemberType ON StudentRoster.Member_ID = MemberType.Member_ID " +  
                           "WHERE StudentRoster.Organization_ID = @Organization_ID AND StudentRoster.EndDate IS NULL" +  
                           ") RESULTSET ON Student.Student_ID = RESULTSET.Student_ID " +  
                           "WHERE RESULTSET.Student_ID IS NULL AND Student.Active != 0 " +  
	                       "Order By Student.LastName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetNonMembersInvoluvedForIncident(int incident_ID, string dom)
        {
            string CS = dom;
            string query = "SELECT NonMemberName,ID FROM NonMembersInvolved	WHERE Incident_ID = @IncidentID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IncidentID", incident_ID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrgAdvisor(int orgID,int advisorID,string dom)
        {
            string CS = dom;
            string query = "SELECT StartDate, EndDate, ProfessionalTitle, AdvisorTitle FROM OrgAdvisor WHERE Advisor_ID=@Advisor_ID AND Organization_ID=@Organization_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Advisor_ID", advisorID);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrgAndStudentDump(string dom)
        {
            string CS = dom;
            string query = "SELECT Organization.OrganizationName,Organization.CX_Code,Organization.MeetingDay,Organization.MeetingFrequency,Organization.MeetingTime,Organization.MeetingBuilding,Organization.MeetingRoom,Organization.DateProposed,Organization.ProposalAccepted," +
			               "Organization.ProposalDenied,Organization.ConstitutionSubmitted,Organization.ConstitutionAccepted,Organization.RegistrarApproval,Organization.RequirementsMet,Organization.RequirementsMetDate,Organization.ConstitutionDenied,Organization.ConstitutionUpdated," +			
			               "OrgCategory.CategoryName,Status.StatusName,OrgStatus.StartDate,OrgStatus.EndDateStudent.Carroll_ID,Student.LastName,Student.FirstName,MemberType.MemberName,StudentRoster.BeginDate,StudentRoster.EndDate,StudentRoster.Session,StudentRoster.Year," +
			               "Student.ClassOF,Student.Address,Student.Email,Student.PreferredPhone,Student.Active,Student.Gender,Student.GPAOrg,Student.Major,Student.Major2,Student.CertifiedDriver,Student.ApprovedDriver,Student.GPABoard,Student.GreekGPA FROM Student " +  
                           "INNER JOIN StudentRoster ON Student.Student_ID = StudentRoster.Student_ID INNER JOIN MemberType ON StudentRoster.Member_ID	= MemberType.Member_ID INNER JOIN Organization ON StudentRoster.Organization_ID  = Organization.Organization_ID " +  
                           "INNER JOIN OrgCategory ON OrgCategory.Category_ID = Organization.Category_ID " +  
                           "INNER JOIN OrgStatus ON OrgStatus.Organization_ID  = Organization.Organization_ID INNER JOIN Status ON Status.Status_ID = OrgStatus.Status_ID"; 
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrganization(int orgID, string orgName, string dom)
        {
            string CS = dom;
            string query = "SELECT Organization_ID,OrganizationName,Description,MeetingTime,MeetingDay,MeetingBuilding,MeetingRoom,DateProposed,ProposalAccepted,ProposalDenied,ProposalNotes,ConstitutionSubmitted,ConstitutionAccepted, ConstitutionNotes, RegistrarApproval," +
			               "CX_Code,Category_ID,RequirementsMet,RequirementsMetDate,OAccount,ProjectCode FROM Organization " +  
	                       "Where (Organization_ID = @Organization_ID OR OrganizationName = @OrganizationName) " +  
	                       "ORDER BY OrganizationName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@OrganizationName", orgName);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrganizationByID(int orgID,string dom)
        {
            string CS = dom;
            string query = "SELECT  OrganizationName,Organization.Description,MeetingTime,MeetingDay,MeetingBuilding,MeetingRoom,DateProposed,ProposalAccepted,ProposalDenied,ProposalNotes,ConstitutionSubmitted,ConstitutionAccepted,ConstitutionNotes,RegistrarApproval,CX_Code," +
			               "Category_ID,RequirementsMet,Status_ID,ConstitutionDenied,ConstitutionUpdated,MeetingFrequency,RequirementsMetDate, OAccount, ProjectCode FROM Organization " +  
	                       "INNER JOIN OrgStatus ON Organization.Organization_ID = OrgStatus.Organization_ID WHERE Organization.Organization_ID=@Organization_ID AND OrgStatus.EndDate IS NULL";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrganizationList(string dom)
        {
            string CS = dom;
            string query = "SELECT Organization_ID, OrganizationName FROM Organization ORDER BY OrganizationName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrganizationRequirementMeetingByReqIDMeetID(int meetingID,int eventID,string dom)
        {
            string CS = dom;
            string query = "SELECT * FROM RequiredEventMeetings WHERE RequiredEventID=@RequiredEventID AND MeetingID =@MeetingID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@RequiredEventID", eventID);
                    cmd.Parameters.AddWithValue("@MeetingID", meetingID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrgCategory(string dom)
        {
            string CS = dom;
            string query = "SELECT CategoryName, Category_ID FROM OrgCategory ORDER BY CategoryName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrgCategoryByID(int categoryID, string dom)
        {
            string CS = dom;
            string query = "SELECT CategoryName, Description, AddDate, EndDate, GreekOrg FROM OrgCategory WHERE Category_ID=@Category_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Category_ID", categoryID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrgCategoryList(string dom)
        {
            string CS = dom;
            string query = "SELECT CategoryName, Category_ID FROM OrgCategory ORDER BY CategoryName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrgComments(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT Date, Text FROM OrgComments WHERE Organization_ID=@Organization_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_GetOrgEvent(int orgID,int eventID,string dom)
        {
            string CS = dom;
            string query = "SELECT Event_ID,Organization_ID,Date,Time,Location,Description,OrganizationWorkedWith,ContactPhone,ContactEmail,Attendance,HoursVolunteered,DollarsSpentByOrg,Title,EventType,OnOffCampus,VanRental,BusRental,ContactName," +
            "Driver1,Driver2,DollarsSpentByPartner,0 NumberOfVolunteers,CreatedByUser,RoomReserved,Marketing,Catering,SignupForm,ActivityWaiver,CUNight " +  
            "(select firstname + ' ' + lastname from Student WHERE Driver1 = Student_ID) as Driver1Name,  (select firstname + ' ' + lastname from Student WHERE Driver2 = Student_ID) as Driver2Name FROM OrgEvent " +
	        "WHERE Organization_ID=@Organization_ID OR Event_ID=@Event_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrgComments(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT Date, Text FROM OrgComments WHERE Organization_ID=@Organization_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrgFiles(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT  OrgFiles.file_number,name_of_file,description_of_file,upload_date,uploaded_by FROM OrgFiles " +  
	                       "INNER JOIN Files ON OrgFiles.file_number = Files.file_number WHERE OrgFiles.Organization_ID = @org_ID " +
	                       "ORDER BY upload_date, name_of_file";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrgForAdvisor(int advisorID, string dom)
        {
            string CS = dom;
            string query = "SELECT OrganizationName,StartDate, EndDate FROM OrgAdvisor " +  
                           "INNER JOIN Organization ON OrgAdvisor.Organization_ID = Organization.Organization_ID " +  
	                       "WHERE Advisor_ID=@Advisor_ID ORDER BY StartDate DESC";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Advisor_ID", advisorID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetOrgRequiredEvent(int eventID, string dom)
        {
            string CS = dom;
            string query = "SELECT RequiredEvent_ID, Notes, Organization_ID, Student1, Student2, Date FROM OrgRequiredEvents " +  
	                       "WHERE RequiredEvent_ID=@RequiredEvent_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@RequiredEvent_ID", eventID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetPeopleInvoluvedForIncident(int incidentID, string dom)
        {
            string CS = dom;
            string query = "DECLARE @Org_ID int " +  
                           "SET @Org_ID = (Select Organization_ID From Incident Where Incident_ID = @IncidentID) " +  
                           "SELECT Student.LastName, Student.FirstName,MemberType.MemberName FROM Student " +  
                           "INNER JOIN StudentRoster ON Student.Student_ID = StudentRoster.Student_ID " +  
                           "INNER JOIN MemberType ON MemberType.Member_ID = StudentRoster.Member_ID " +  
                           "INNER JOIN PeopleInvolved ON Student.Student_ID = PeopleInvolved.Student_ID " +  
	                       "WHERE PeopleInvolved.Incident_ID = @IncidentID 	AND StudentRoster.EndDate IS NULL AND StudentRoster.Organization_ID = @Org_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IncidentID", incidentID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetRequiredEventByID(int eventID, string dom)
        {
            string CS = dom;
            string query = "SELECT * FROM RequiredEvents WHERE RequiredEvent_ID = @Event_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetRequiredEventList(string dom)
        {
            string CS = dom;
            string query = "SELECT * FROM RequiredEvents";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetRoleOfUser(string uname,string password, string dom)
        {
            string CS = dom;
            string query = "SELECT @RoleName = Roles.Role_Name,@isActive = Is_Active FROM Roles " +  
	                       "INNER JOIN Users ON Roles.Role_ID = Users.Role_ID " +  
	                       "WHERE Users.User_ID = @User_ID AND Users.Password = @Password";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@User_ID", uname);
                    cmd.Parameters.AddWithValue("@Password", password);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetServiceProject(int projectID,string dom)
        {
            string CS = dom;
            string query = "SELECT ServiceProject_ID,Organization_ID,CommunityPartner,ContactName,ContactPhone,ContactEmail,NoOfVolunteers,HoursVolunteered,Description,OnGoing,Location,StartDate,EndDate,TimeFrom,TimeTo,MeetDay,MeetFrequency," +
                           "Title, Funds, Planned_NoOfVolunteers, Planned_HoursVolunteered,Planned_Funds FROM ServiceProject WHERE ServiceProject_ID = @ServiceProject_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ServiceProject_ID", projectID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetServiceProjectByOrgIDDate(int orgID, string date, string dom)
        {
            string CS = dom;
            DateTime datetime = Convert.ToDateTime(date);  
            string query = "SELECT * FROM ServiceProject WHERE Organization_ID=@Organization_ID AND StartDate=@Date";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Date", datetime);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetServiceProjectDatesByOrgID(int orgID, string dom)
        {
            string CS = dom;
            string query = "SELECT StartDate FROM ServiceProject WHERE Organization_ID=@Organization_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetStatus(string dom)
        {
            string CS = dom;
            string query = "SELECT StatusName,Status_ID FROM Status ORDER BY StatusName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetStudent(int studID,int carrollID,string dom)
        {
            string CS = dom;
            string query = "SELECT Student_ID,LastName,FirstName,ClassOf,Address,Email,PreferredPhone,Active,Gender,Carroll_ID,GPAOrg,Major,Major2,CertifiedDriver,ApprovedDriver,GPABoard,Ferpa,GreekGPA,OnCampus,Race,International FROM Student " +  
	                       "WHERE (Student_ID = @Student_ID OR Carroll_ID = @Carroll_ID)";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Student_ID", studID);
                    cmd.Parameters.AddWithValue("@Carroll_ID", carrollID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetStudentByCarrollID(int studID, string dom)
        {
            string CS = dom;
            string query = "SELECT * FROM Student WHERE Student.Carroll_ID = @Student_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Student_ID", studID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetStudentByID(int studID, string dom)
        {
            string CS = dom;
            string query = "SELECT Student.LastName,Student.FirstName,Student.Student_ID,Student.Carroll_ID,StudentRoster.BeginDate FROM Student " +  
                           "INNER JOIN StudentRoster ON Student.Student_ID = StudentRoster.Student_ID " +  
                           "INNER JOIN Status ON StudentRoster.Status_ID  = Status.Status_ID " +  
	                       "WHERE Student.Student_ID = @Student_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Student_ID", studID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetStudentByLastName(string lname, string dom)
        {
            string CS = dom;
            string query = "SELECT LastName,FirstName,Carroll_ID FROM Student WHERE Student.LastName = @LastName";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@LastName", lname);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Doubt [should begin and end be included in if stmt?]
        public static DataTable sp_GetStudentByName(string lname,string fname,string dom)
        {
            string CS = dom;
            string query = "DECLARE @lname varchar(21), @fname varchar(21); \n select @lname = rtrim(ltrim(lower(@LastName))); \n select @fname = rtrim(ltrim(lower(@FirstName))); " +
                           "IF @lname = '' AND @fname =  ''  \n BEGIN SELECT LastName,FirstName,Email,Carroll_ID,Student_ID FROM Student WHERE Student_ID = -1 END \n" +  
                           "IF @lname != '' AND @fname != '' \n BEGIN SELECT LastName,FirstName,Email,Carroll_ID,Student_ID FROM Student	WHERE (charindex(@lname,Student.LastName) > 0) AND (charindex(@fname,Student.FirstName) > 0) ORDER BY LastName, FirstName END \n" +  
                           "IF @lname = '' AND @fname != ''  \n BEGIN SELECT LastName,FirstName,Email,Carroll_ID,Student_ID FROM Student WHERE (charindex(@fname,Student.FirstName) > 0) ORDER BY LastName, FirstName END \n" +  
                           "IF @lname != '' AND @fname = ''  \n BEGIN SELECT LastName,FirstName,Email,Carroll_ID,Student_ID FROM Student WHERE (charindex(@lname,Student.LastName) > 0) ORDER BY LastName, FirstName END";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@LastName", lname);
                    cmd.Parameters.AddWithValue("@FirstName", fname);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetStudentByStudID(int studID,string dom)
        {
            string CS = dom;
            string query = "SELECT * FROM Student WHERE Student.Student_ID = @Student_ID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Student_ID", studID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetStudentList(string dom)
        {
            string CS = dom;
            string query = "SELECT Student_ID,FirstName,LastName,Carroll_ID FROM Student";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_GetUserList(string mode, string dom)
        {
            string CS = dom;
            //int UID = Convert.ToInt32(userID)
            string query = "IF @select_mode is null \n SELECT @select_mode = 'ALL' \n" +
                           "IF @select_mode =  'ACTIVE' \n SELECT User_ID FROM Users WHERE Is_Active = 1 \n" +  
                           "if @@ERROR<>0 \n raiserror('100',16,1) return -100; \n" +  
                           "IF @select_mode = 'ALL' \n SELECT User_ID FROM Users \n" +  
                           "if @@ERROR<>0 \n raiserror('101',16,1) return -101; \n" +  
                           "else return 0;";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@select_mode", mode);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetUserByID(string userID,string dom)
        {
            string CS = dom;
            /*
            *  int user_ID;
            *  Int32.TryParse(userID, out user_ID);
            */
            string query = "SELECT * FROM Users WHERE User_ID = @UserID";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetUserOrgsByID(string userID, string dom)
        {
            string CS = dom;
            /*
            *  int user_ID;
            *  Int32.TryParse(userID, out user_ID);
            */
            string query = "SELECT UserOrgs.*,Organization.OrganizationName FROM UserOrgs " +  
                           "INNER JOIN Users on Users.User_ID = UserOrgs.User_ID " +  
                           "INNER JOIN Organization on Organization.Organization_ID = UserOrgs.Organization_ID " +  
                           "WHERE UserOrgs.User_ID = @UserID " + 
	                       "AND inactive_date is NULL";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_GetUserRoles(string dom)
        {
            string CS = dom;
            string query = "SELECT * FROM Roles";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }
       
        //Correct
        public static DataTable sp_InsertOrganizationRequirementMeeting(int eventID,string session,string year,string date,string time,string building, string room, string notes,string dom)
        {
            string CS = dom;
            string query = "INSERT INTO [StudentOrgs].[dbo].[RequiredEventMeetings]([RequiredEventID],[Session],[Year],[MeetingDate],[MeetingTime],[MeetingBuilding],[MeetingRoomNumber],[Notes]) " +  
                           "VALUES (@RequiredEventID,@Session,@Year,Convert(Datetime,@MeetingDate),@MeetingTime,@MeetingBuilding,@MeetingRoomNumber,@Notes)";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@RequiredEventID", eventID);
                    cmd.Parameters.AddWithValue("@Session", session);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@MeetingDate", date);
                    cmd.Parameters.AddWithValue("@MeetingTime", time);
                    cmd.Parameters.AddWithValue("@MeetingBuilding", building);
                    cmd.Parameters.AddWithValue("@MeetingRoomNumber", room);
                    cmd.Parameters.AddWithValue("@Notes", notes);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewAdvisor(string lname, string fname, string email, string phone, char offcampus, string advTitle, string profTitle, string dom)
        {
            string CS = dom;
            string query = "INSERT INTO Advisor (LastName, FirstName, Email, PhoneNumber, OffCampus,AdvisorTitle,ProfessionalTitle) " +
	                       "VALUES (@LastName, @FirstName, @Email, @PhoneNumber, @OffCampus,@AdvisorTitle,@ProfessionalTitle)";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@LastName", lname);
                    cmd.Parameters.AddWithValue("@FirstName", fname);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phone);
                    cmd.Parameters.AddWithValue("@OffCampus", offcampus);
                    cmd.Parameters.AddWithValue("@AdvisorTitle", advTitle);
                    cmd.Parameters.AddWithValue("@ProfessionalTitle", profTitle);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_NewEventComments(int eventID, string userID, string text, string date, string dom)
        {
            string CS = dom;
            string query = "declare @today datetime; IF @Text is null return 0; IF @Date is null OR rtrim(ltrim(@Date)) = '' select @today = getdate() ELSE select @today = CONVERT(datetime,@Date) " +  
                           "INSERT INTO EventComments (Event_ID, Date, text,UserID) VALUES (@Event_ID, Convert(Datetime,@today), @Text, @UserID)";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);
                    cmd.Parameters.AddWithValue("@today", date);
                    cmd.Parameters.AddWithValue("@Text", text);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_NewFile(string upload, string nameFile, byte[] file, string dom)
        {
            string CS = dom;
            string query = "DECLARE @out_filenumber INT; INSERT INTO Files (name_of_file, description_of_file, upload_date, uploaded_by, file_blob) VALUES (@name_of_file,'',getdate(), @uploaded_by, @file_contents) " +  
                           "SELECT @out_filenumber = SCOPE_IDENTITY() \n if @@ERROR<>0 BEGIN RaisError('100', 16, 1) RETURN -100; else RETURN @out_filenumber;";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name_of_file", file);
                    cmd.Parameters.AddWithValue("@uploaded_by", upload);
                    cmd.Parameters.AddWithValue("@file_contents", nameFile);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewIncident(string date, string rdate, string time, string loc, string incident, string result, string notes, string followup, int orgID, string reported, string commText, string comDate, string username, string dom)
        {
            //incident_ID
            string CS = dom;
            DateTime Date = Convert.ToDateTime(date);
            DateTime repdate = Convert.ToDateTime(rdate);
            DateTime commdate = Convert.ToDateTime(comDate);

            string query = "INSERT INTO Incident (Incident.Date,DateReported, Incident.Time, Location, Incident, Result, Notes, Organization_ID, ReportedBy,FollowUp) VALUES @Date,@DateReported, @Time, @Location, @Incident, @Result, @Notes, @Organization_ID, @ReportedBy, @FollowUp) " +  
                           "SELECT @Incident_ID  = SCOPE_IDENTITY() INSERT INTO IncidentComments (Date,Text,Incident_ID,UserID) VALUES (CONVERT @CommentDate,@CommentText,@Incident_ID,@UserName)";
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Date", Date);
                    cmd.Parameters.AddWithValue("@DateReported", repdate);
                    cmd.Parameters.AddWithValue("@Time", time);
                    cmd.Parameters.AddWithValue("@Location", loc);
                    cmd.Parameters.AddWithValue("@Incident", incident);
                    cmd.Parameters.AddWithValue("@Result", result);
                    cmd.Parameters.AddWithValue("@Notes", notes);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@ReportedBy", reported);
                    cmd.Parameters.AddWithValue("@FollowUp", followup);
                    cmd.Parameters.AddWithValue("@CommentDate", commdate);
                    cmd.Parameters.AddWithValue("@CommentText", commText);
                    //cmd.Parameters.AddWithValue("@Incident_ID", nameFile);
                    cmd.Parameters.AddWithValue("@UserName", username);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch(exception ex){
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewIncidentComments(int incID, string uid, string date, string text, string dom)
        {
            string CS = dom;
            DateTime Date = Convert.ToDateTime(date);

            try
            {
                string query = "INSERT INTO IncidentComments (Incident_ID, Date, text, UserID) VALUES (@Incident_ID, @Date, @Text, @UserID)";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Date", Date);
                    cmd.Parameters.AddWithValue("@Incident_ID", incID);
                    cmd.Parameters.AddWithValue("@Text", text);
                    cmd.Parameters.AddWithValue("@UserID", uid);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }catch(exception ex) {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewMeetingForOrganizationRequirement(int meetID,int orgID,string dom)
        {
            string CS = dom;
            DateTime Date = Convert.ToDateTime(date);

            try
            {
                string query = "INSERT INTO [StudentOrgs].[dbo].[OrgRequiredEvents] ([MeetingID],[Notes],[Organization_ID],[Student1],[Student2],[Attended]) VALUES (@MeetingID,NULL,@OrganizationID,NULL,NULL,0)";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MeetingID", meetID);
                    cmd.Parameters.AddWithValue("@OrganizationID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewMemberType(string membName,string desc,string addDate,string endDate,char boardMemb,string dom)
        {
            string CS = dom;
            DateTime AddDate = Convert.ToDateTime(addDate);
            DateTime EndDate = Convert.ToDateTime(endDate);
            try
            {
                string query = "INSERT INTO MemberType (MemberName, Description, AddDate, EndDate, BoardMember) VALUES (@MemberName,@Description,@AddDate,@EndDate,@BoardMember)";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MemberName", membName);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@AddDate", AddDate);
                    cmd.Parameters.AddWithValue("@EndDate", EndDate);
                    cmd.Parameters.AddWithValue("@BoardMember", boardMemb);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewMyOrgsEvent(int orgID,string userID,string dom)
        {
            string CS = dom;
         
            try
            {
                string query = "DECLARE @Event_ID int; " +
	                           "INSERT INTO OrgEvent (Organization_ID,OrgEvent.Date,Time,Location,Description,OrganizationWorkedWith,ContactPhone,ContactEmail,Attendance,DollarsSpentByOrg,DollarsSpentByPartner, " +
                               "Title,EventType,OnOffCampus,VanRental,BusRental,ContactName,Driver1,Driver2,CreatedByUser,RoomReserved,Marketing,Catering,SignupForm,ActivityWaiver,CUNight) " +
                               "VALUES (@Organization_ID, getdate(),'','','', null,null, null, null, null, null,'Event Title', 1, 0, null, null, null, null, null, @UserID, 0, 0, 0, 0, 0, 0) SELECT @Event_ID = SCOPE_IDENTITY()";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@orgID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewMyOrgsServiceProject(int orgID, string dom)
        {
            string CS = dom;
          
            try
            {
                string query = "DECLARE @Service_ID int; " +  
                               "INSERT INTO ServiceProject (Organization_ID,CommunityPartner,ContactName,ContactPhone,ContactEmail,NoOfVolunteers,HoursVolunteered,Description, " +
                               "OnGoing,Location,StartDate,EndDate,TimeFrom,TimeTo,MeetDay,MeetFrequency,Title,Funds) " +  
                               "VALUES (@Organization_ID,NULL,NULL,NULL,NULL,0, 0,'',0,null,getdate(),null,null,null,null,null,'Service Project Title',null) " +
	                           "SELECT @Service_ID = SCOPE_IDENTITY()";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewNonMembersInvoluved(int ID, int incID, string name, string dom)
        {
            string CS = dom;
          
            try
            {
                string query = "INSERT INTO NonMembersInvolved (NonMemberName,ID, Incident_ID) VALUES (@Name,@ID, @Incident_ID)";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Incident_ID", incID);
                    cmd.Parameters.AddWithValue("@ID", ID);
                    cmd.Parameters.AddWithValue("@Name", name);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewOrgAdvisor(int orgID, int advisorID, DateTime sdate, DateTime edate, string profTitle, string advTitle, string dom)
        {
            string CS = dom;
            try
            {
                string query = "INSERT INTO OrgAdvisor (StartDate, EndDate, ProfessionalTitle, AdvisorTitle,Organization_ID) VALUES (@StartDate, @EndDate, @ProfessionalTitle, @AdvisorTitle,@Organization_ID)";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@StartDate", sdate);
                    cmd.Parameters.AddWithValue("@EndDate", edate);
                    cmd.Parameters.AddWithValue("@ProfessionalTitle", profTitle);
                    cmd.Parameters.AddWithValue("@AdvisorTitle", advTitle);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewOrgAdvisorRow(int orgID, int advisorID, string dom)
        {
            string CS = dom;
            try
            {
                string query = "IF NOT EXISTS ( SELECT 1 FROM OrgAdvisor WHERE Organization_ID = @Organization_ID AND Advisor_ID = @Advisor_ID ) " +  
                               "INSERT INTO OrgAdvisor (Advisor_ID,StartDate, EndDate,Organization_ID) VALUES (@Advisor_ID,CURRENT_TIMESTAMP,NULL,@Organization_ID)";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Advisor_ID", advisorID);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

         //Doubt
        public static DataTable sp_NewOrganization(string orgName,string desc,string meetTime,string meetDay,string meetBldg,string meetRoom,string dateProp,string propAccepted,string propDenied,string propNotes,
            string constSubmitted,string constAccepted,string contNotes,string regApproval,char codeCX,int catID,char reqtMet,string reqtMetDate,string commText,int statusID,string commDate,string statusStDate,
            string StatusEDate,string uname,string meetFrq,string oAcct,string projCode,string dom)
        {
            string CS = dom;
            try
            {
                string query = "DECLARE @Organization_ID INT " +  
                                "INSERT INTO Organization (OrganizationName,Description,MeetingTime,MeetingDay,MeetingBuilding,MeetingRoom,DateProposed,ProposalAccepted,ProposalDenied,ProposalNotes,ConstitutionSubmitted,ConstitutionAccepted,ConstitutionNotes,RegistrarApproval, " +
                               "CX_Code,Category_ID,RequirementsMet,RequirementsMetDate,MeetingFrequency,OAccount,ProjectCode) VALUES (@OrganizationName,@Description,@MeetingTime,@MeetingDay,@MeetingBuilding,@MeetingRoom,@DateProposed,@ProposalAccepted,@ProposalDenied,@ProposalNotes, " +
                               "@ConstitutionSubmitted,@ConstitutionAccepted,@ConstitutionNotes,@RegistrarApproval,@CX_Code,@Category_ID,@RequirementsMet,@RequirementsMetDate,@MeetingFrequency,coalesce(@OAccount,''),coalesce(@ProjectCode,'')) " +  
	                           "SELECT @Organization_ID = SCOPE_IDENTITY() IF (@CommentText IS NOT NULL AND @CommentText!= '') INSERT INTO OrgComments (Date,Text,Organization_ID,UserID) VALUES (@CommentDate,@CommentText,@Organization_ID,@UserName) " + 
                               "INSERT INTO OrgStatus (Organization_ID,Status_ID,StartDate, " + "EndDate) VALUES (@Organization_ID,1,@StatusStartDate,@StatusEndDate)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@OrganizationName", orgName);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@MeetingTime", meetTime);
                    cmd.Parameters.AddWithValue("@MeetingDay", meetDay);
                    cmd.Parameters.AddWithValue("@MeetingBuilding", meetBldg);
                    cmd.Parameters.AddWithValue("@MeetingRoom", meetRoom);
                    cmd.Parameters.AddWithValue("@DateProposed", dateProp);
                    cmd.Parameters.AddWithValue("@ProposalAccepted", propAccepted);
                    cmd.Parameters.AddWithValue("@ProposalDenied", propDenied);
                    cmd.Parameters.AddWithValue("@ProposalNotes", propNotes);
                    cmd.Parameters.AddWithValue("@ConstitutionSubmitted", constSubmitted);
                    cmd.Parameters.AddWithValue("@ConstitutionAccepted", constAccepted);
                    cmd.Parameters.AddWithValue("@ConstitutionNotes", contNotes);
                    cmd.Parameters.AddWithValue("@RegistrarApproval", regApproval);
                    cmd.Parameters.AddWithValue("@CX_Code", codeCX);
                    cmd.Parameters.AddWithValue("@Category_ID", catID);
                    cmd.Parameters.AddWithValue("@RequirementsMet", reqtMet);
                    cmd.Parameters.AddWithValue("@RequirementsMetDate", reqtMetDate);
                    cmd.Parameters.AddWithValue("@MeetingFrequency", meetFrq);
                    cmd.Parameters.AddWithValue("@OAccount", oAcct);
                    cmd.Parameters.AddWithValue("@ProjectCode", projCode);               
                    cmd.Parameters.AddWithValue("@CommentDate", commDate);
                    cmd.Parameters.AddWithValue("@CommentText", commText);
                    cmd.Parameters.AddWithValue("@UserName", uname);
                    cmd.Parameters.AddWithValue("@StatusStartDate", desc);
                    cmd.Parameters.AddWithValue("@StatusEndDate", desc);
                    SqlDataReader rdr = cmd3.ExecuteReader();

                    dt.Load(rdr);

                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewOrganizationComments(int orgID, string date, string text, string userID, string dom)
        {
            string CS = dom;
            DateTime Date = Convert.ToDateTime(date);

            try
            {
                string query = "INSERT INTO OrgComments (Organization_ID, Date, text,UserID) VALUES (@Organization_ID,@Date,@Text,@UserID)";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Date", Date);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Text", text);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewOrganizationFromExcel(string orgName, string desc, string dom)
        {
            string CS = dom;

            try
            {
                string query = "DECLARE @Organization_ID INT INSERT INTO Organization (OrganizationName, Description,Category_ID) Values (@OrganizationName, @Description,3) SELECT @Organization_ID = SCOPE_IDENTITY()";
	
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@OrganizationName", orgName);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewOrgCategory(string catName,string desc,string adddate,string edate,char geek,string dom)
        {
            string CS = dom;
            DateTime startDate = Convert.ToDateTime(adddate);
            DateTime endDate = Convert.ToDateTime(edate);

            try
            {
                string query = "INSERT INTO OrgCategory (CategoryName, Description,AddDate, EndDate, GreekOrg) VALUES (@CategoryName, @Description, CONVERT(datetime,@AddDate), CONVERT(datetime,@EndDate), @GreekOrg)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@CategoryName", orgName);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@AddDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Parameters.AddWithValue("@GreekOrg", geek);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewOrgComments(int orgID, DateTime date, string text, string dom)
        {
            string CS = dom;
            //DateTime Date = Convert.ToDateTime(date);

            try
            {
                string query = "Insert INTO OrgComments (Date,Text,Organization_ID) VALUES (@Date,@Text,@Organization_ID)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@Text", text);
                
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_NewOrgEvent(int orgID, DateTime date, string time, string loc, string desc, string orgWorkwWith, string phone, string email, int attendance, int hrsVolunteered, float dollarSpent, float dollarSptPartner, string title, char eventype, 
            char onoffcampus, int vanRental, int busRental, string contPerson, int driver1, int driver2, int userID, char roomRes, char marketing, char catering, char signUp, char waiver, string cun, DateTime commDate, string commText, string dom)
        {
            string CS = dom;
            //DateTime Date = Convert.ToDateTime(date);

            try
            {
                string query =  "DECLARE @Event_ID INT IF @RoomReserved IS NULL SELECT @RoomReserved = 0 " +
		                        "IF @Marketing IS NULL SELECT @RoomReserved = 0 " +
                                "IF @Catering IS NULL SELECT @RoomReserved = 0 " +
                                "IF @SignupForm IS NULL SELECT @RoomReserved = 0 " +
                                "IF @ActivityWaiver IS NULL SELECT @RoomReserved = 0 " +
                                "IF @CUNight IS NULL SELECT @RoomReserved = 0 " +   
	                            "INSERT INTO OrgEvent (Organization_ID,OrgEvent.Date,Time,Location,Description,OrganizationWorkedWith,ContactPhone,ContactEmail,Attendance,HoursVolunteered,DollarsSpentByOrg,DollarsSpentByPartner,Title,EventType, " +
                                "OnOffCampus,VanRental,BusRental,ContactName,Driver1,Driver2,CreatedByUser,RoomReserved,Marketing,Catering,SignupForm,ActivityWaiver,CUNight) " +
	                            "VALUES (@Organization_ID,@Date,@Time,@Location,@Description,@OrganizationWorkedWith,@ContactPhone,@ContactEmail,@Attendance,@HoursVolunteered,@DollarsSpent,@DollarsSpentByPartner,@Title,@EventType,@OnOffCampus, " +
                                "@VanRental,@BusRental,@ContactPerson,@Driver1,@Driver2,@UserID,@RoomReserved,@Marketing,@Catering,@SignupForm,@ActivityWaiver,@CUNight) " +  
                                "SELECT @Event_ID = SCOPE_IDENTITY() " +   
	                            "INSERT INTO EventComments (Date,Text,Event_ID,UserID) VALUES (@CommentDate,@CommentText,@Event_ID,@UserID)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@Time", time);
                    cmd.Parameters.AddWithValue("@Location", loc);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@OrganizationWorkedWith", orgWorkwWith);
                    cmd.Parameters.AddWithValue("@ContactPhone", phone);
                    cmd.Parameters.AddWithValue("@ContactEmail", email);
                    cmd.Parameters.AddWithValue("@Attendance", attendance);
                    cmd.Parameters.AddWithValue("@HoursVolunteered", hrsVolunteered);
                    cmd.Parameters.AddWithValue("@DollarsSpent", dollarSpent);
                    cmd.Parameters.AddWithValue("@DollarsSpentByPartner", dollarSptPartner);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@EventType", eventype);
                    cmd.Parameters.AddWithValue("@OnOffCampus", onoffcampus);
                    cmd.Parameters.AddWithValue("@VanRental", vanRental);
                    cmd.Parameters.AddWithValue("@BusRental", busRental);
                    cmd.Parameters.AddWithValue("@ContactPerson", contPerson);
                    cmd.Parameters.AddWithValue("@Driver1", driver1);
                    cmd.Parameters.AddWithValue("@Driver2", driver2);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@RoomReserved", roomRes);
                    cmd.Parameters.AddWithValue("@Marketing", marketing);
                    cmd.Parameters.AddWithValue("@Catering", catering);
                    cmd.Parameters.AddWithValue("@SignupForm", signUp);
                    cmd.Parameters.AddWithValue("@ActivityWaiver", waiver);
                    cmd.Parameters.AddWithValue("@CUNight", cun);
                    cmd.Parameters.AddWithValue("@CommentDate", commDate);
                    cmd.Parameters.AddWithValue("@CommentText", commText);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewOrgFile(int orgID, DateTime date, string nameFile, string uploaded, string fileContent, string dom)
        {
            string CS = dom;
            //DateTime Date = Convert.ToDateTime(date);

            try
            {
                string query = "DECLARE @out_filenumber INT; " +   
                               "exec @out_filenumber = dbo.sp_NewFile @uploaded_by, @file_contents " +  
                               "INSERT INTO OrgFiles (Organization_ID, file_number) VALUES (@org_ID, @out_filenumber)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@org_ID", orgID);
                    cmd.Parameters.AddWithValue("@uploaded_by", uploaded);
                    cmd.Parameters.AddWithValue("@file_contents", fileContent);
                    cmd.Parameters.AddWithValue("@name_of_file", nameFile );

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewOrgFile(int orgID, DateTime date, string nameFile, string uploaded, string fileContent, string dom)
        {
            string CS = dom;
            //DateTime Date = Convert.ToDateTime(date);

            try
            {
                string query = "DECLARE @out_filenumber INT; " +   
                               "exec @out_filenumber = dbo.sp_NewFile @uploaded_by, @file_contents " +  
                               "INSERT INTO OrgFiles (Organization_ID, file_number) VALUES (@org_ID, @out_filenumber)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@org_ID", orgID);
                    cmd.Parameters.AddWithValue("@uploaded_by", uploaded);
                    cmd.Parameters.AddWithValue("@file_contents", fileContent);
                    cmd.Parameters.AddWithValue("@name_of_file", nameFile );

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewOrgStatus(int orgID, DateTime strDate, DateTime endDate, int statusDate, string dom)
        {
            string CS = dom;
            //DateTime Date = Convert.ToDateTime(date);

            try
            {
                string query = "INSERT INTO OrgStatus (Organization_ID,Status_ID,StartDate,EndDate) VALUES (@Organization_ID,@Status_ID,@StartDate,@EndDate)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Status_ID", statusDate);
                    cmd.Parameters.AddWithValue("@StartDate", strDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewPeopleInvoluved(int studID, int incID, string dom)
        {
            string CS = dom;
            //DateTime Date = Convert.ToDateTime(date);

            try
            {
                string query = "INSERT INTO PeopleInvolved (Student_ID, incident_ID) VALUES (@StudentID, @IncidentID)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@StudentID", studID);
                    cmd.Parameters.AddWithValue("@IncidentID", incID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewRequiredEvent(string reqdEvent, string eventDesc, string adddate, string dom)
        {
            string CS = dom;
            DateTime addDate = Convert.ToDateTime(adddate);

            try
            {
                string query = "INSERT INTO RequiredEvents (RequiredEvent, EventDescription,AddDate) VALUES (@RequiredEvent, @EventDescription,@AddDate)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@RequiredEvent", reqdEvent);
                    cmd.Parameters.AddWithValue("@EventDescription", eventDesc);
                    cmd.Parameters.AddWithValue("@AddDate", adddate);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

      //Correct
        public static DataTable sp_NewServiceProject(int orgID, string commPartner, string contName, string contPhone, string contEmail, int numbVolunteers, int hrsVollunteered, string desc, char onGoing, string location, string strDate, string endDate,
            string timeFrom, string timeTo, string commText, string commDate, string userID, string meetDay, string meetFrq, string title, string funds, int pl_NumVolunteers, int pl_HrsVolunteered, string pl_Funds, string dom)
        {
            string CS = dom;
            DateTime StartDate = Convert.ToDateTime(strDate);
            DateTime EndDate = Convert.ToDateTime(endDate);
            DateTime CommDate = Convert.ToDateTime(commDate);


            try
            {
                string query = "DECLARE @ServiceProject_ID INT INSERT INTO ServiceProject (Organization_ID,CommunityPartner,ContactName,ContactPhone,ContactEmail,NoOfVolunteers,HoursVolunteered,Description," +
	                           "OnGoing,Location,StartDate,EndDate,TimeFrom,TimeTo,MeetDay,MeetFrequency,Title,Funds,Planned_NoOfVolunteers, Planned_HoursVolunteered, Planned_Funds) " +
	                           "VALUES (@Organization_ID,@CommunityPartner,@ContactName,@ContactPhone,@ContactEmail,@NoOfVolunteers,@HoursVolunteered,@Description,@OnGoing,@Location,@StartDate,@EndDate,@TimeFrom,@TimeTo," +
                               "@MeetDay,@MeetFrequency,@Title,@Funds,@Planned_NoOfVolunteers,@Planned_HoursVolunteered,@Planned_Funds) " +  
	                           "SELECT @ServiceProject_ID = SCOPE_IDENTITY() INSERT INTO ServiceProjectComments (Date,Text,ServiceProject_ID,UserID) VALUES (@CommentDate,@CommentText,@ServiceProject_ID,@UserID)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@CommunityPartner", commPartner);
                    cmd.Parameters.AddWithValue("@ContactName", contName);
                    cmd.Parameters.AddWithValue("@ContactPhone", contPhone);
                    cmd.Parameters.AddWithValue("@ContactEmail", contEmail);
                    cmd.Parameters.AddWithValue("@NoOfVolunteers", numbVolunteers);
                    cmd.Parameters.AddWithValue("@HoursVolunteered", hrsVollunteered);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@OnGoing", onGoing);
                    cmd.Parameters.AddWithValue("@Location", location);
                    cmd.Parameters.AddWithValue("@StartDate", StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", EndDate);
                    cmd.Parameters.AddWithValue("@TimeFrom", timeFrom);
                    cmd.Parameters.AddWithValue("@TimeTo", timeTo);
                    cmd.Parameters.AddWithValue("@MeetDay", meetDay);
                    cmd.Parameters.AddWithValue("@MeetFrequency", meetFrq);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Funds", funds);
                    cmd.Parameters.AddWithValue("@Planned_NoOfVolunteers", pl_NumVolunteers);
                    cmd.Parameters.AddWithValue("@Planned_HoursVolunteered", pl_HrsVolunteered);
                    cmd.Parameters.AddWithValue("@Planned_Funds", pl_Funds);
                    cmd.Parameters.AddWithValue("@CommentDate", CommDate);
                    cmd.Parameters.AddWithValue("@CommentText", commText);
                    cmd.Parameters.AddWithValue("@UserID", userID);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Doubt if- else same line - fomratting ??
        public static DataTable sp_NewServiceProjectComments(int projID, string date, string text, string userID, string dom)
        {
            string CS = dom;
            DateTime Date = Convert.ToDateTime(date);

            try
            {
                string query = "DECLARE @today datetime; IF @Text is null return 0; " + " IF @Date is null OR rtrim(ltrim(@Date)) =  '' select @today = getdate() " +  
                               "ELSE select @today = @Date " + "\n INSERT INTO ServiceProjectComments (ServiceProject_ID, Date, text,UserID) VALUES (@ServiceProject_ID, @today, @Text,@UserID)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ServiceProject_ID", projID);
                    cmd.Parameters.AddWithValue("@Text", text);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@Date", date);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }
        //Doubt
        public static DataTable sp_NewStudentRosterRow(int studID, int orgID, string dom)
        {
            string CS = dom;

            try
            {
                string query = "DECLARE @Session varchar(4),@Year varchar(4) " + " Select @Session = Session From Term Select @Year = Year From Term IF NOT EXISTS " +  "( " +  
			    "SELECT 1 FROM StudentRoster WHERE Organization_ID = @Organization_ID AND Student_ID = @Student_ID AND ENDDATE IS NULL AND Member_ID = 22) " +  
                "BEGIN INSERT INTO StudentRoster (Student_ID,BeginDate, EndDate,Organization_ID,Member_ID,Session,Year) VALUES (@Student_ID,CURRENT_TIMESTAMP,NULL,@Organization_ID,22,@Session,@Year)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Student_ID", studID);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_NewUser(int roleID, char isactive, string lname, string fname, string pw, string uname, string phone, string email, string dom)
        {
            string CS = dom;

            try
            {
                string query = "IF @RoleId is null OR NOT EXISTS(select * from dbo.Roles where role_id = @RoleID) select @RoleID = 2 " +    
	                           "select  @LastName = ltrim(rtrim(@LastName)), @FirstName = ltrim(rtrim(@FirstName)), @Password = ltrim(rtrim(@Password)), @UserId = ltrim(rtrim(@UserId)), " +   
			                   "@Phone = ltrim(rtrim(@Phone)), @Email = ltrim(rtrim(@Email)) " + " IF @Phone = '' select @Phone = null " + " IF @Email = '' select @Email = null " +    
                               "INSERT INTO Users (LastName, FirstName, Password, User_ID, Role_ID, Is_Active, Phone, Email) VALUES (@LastName, @FirstName,@Password, @UserId, @RoleID, @IsActive, @Phone, @Email)";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@LastName", lname);
                    cmd.Parameters.AddWithValue("@FirstName", fname);
                    cmd.Parameters.AddWithValue("@Password", pw);
                    cmd.Parameters.AddWithValue("@UserId", uname);
                    cmd.Parameters.AddWithValue("@RoleID", roleID);
                    cmd.Parameters.AddWithValue("@IsActive", isactive);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Email", email);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_Report_ActiveUsers(char inactive, string dom)
        {
            string CS = dom;

            try
            {
                string query = "select User_ID as UserName,Firstname,Lastname,coalesce(Email,'') as EmailAddress,coalesce(Phone,'') as PhoneNumber,Role_Name, case when Is_Active = 1 then 'Y' else 'N' end Active " +   
			                   ",case when exists(select * from UserOrgs where UserOrgs.User_ID = Users.User_ID and UserOrgs.inactive_date is null) " +   
				               "then (select convert(varchar(2), count(*)) from UserOrgs where UserOrgs.User_ID = Users.User_ID *and UserOrgs.inactive_date is null) " +    
		                       "else '' end LimitedOrgCount from Users " +  
	                           "INNER JOIN Roles ON Users.Role_ID = Roles.Role_ID " +   
	                           "WHERE ((@Include_Inactive = 'N' AND Is_Active = 1) OR @Include_Inactive = 'Y') " + 
	                           "ORDER BY Role_Name, User_ID";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Include_Inactive", inactive);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_Report_AllMembers(string classof, char oncampus, string race, char international, string dom)
        {
            string CS = dom;
     
            try
            {
                string query = "--DECLARE @ClassOf varchar(2), @OnCampus char(1), @Race varchar(50), @International char(1) " +  
	                           "--SELECT @ClassOf =  '**', @OnCampus =  'B', @Race = '*ALL*', @International = 'B' --1373 " +   
	                           "--SELECT @ClassOf =  '**', @OnCampus =  'B', @Race =  '*ALL*', @International =  'Y' --31 " +  
	                           "--SELECT @ClassOf =  '**', @OnCampus =  'B', @Race =  '*ALL*', @International =  'N' --1342 " +  
	                           "--SELECT @ClassOf =  '**', @OnCampus =  'Y', @Race =  '*ALL*', @International =  'B' --1131 " +  
	                           "--SELECT @ClassOf =  '**', @OnCampus =  'N', @Race =  '*ALL*', @International =  'B' --242 " +  
	                           "--SELECT @ClassOf =  'FR', @OnCampus =  'B', @Race =  '*ALL*', @International =  'B' --11 " +  
	                           "--SELECT @ClassOf =  'FF', @OnCampus =  'B', @Race =  '*ALL*', @International =  'B' --4 " +  
	                           "--SELECT @ClassOf =  'S', @OnCampus =  'B', @Race =  '*ALL*', @International =   'B' --9 " +  
	                           "--SELECT @ClassOf =  'GR', @OnCampus =  'B', @Race =  '*ALL*', @International =  'B' --12 " +  
	                           "--SELECT @ClassOf =  'UN', @OnCampus =  'B', @Race =  '*ALL*', @International =  'B' --2 " +  
	                           "--SELECT @ClassOf =  'SO', @OnCampus =  'B', @Race =  '*ALL*', @International =  'B' --377 " +  
	                           "--SELECT @ClassOf =  'JR', @OnCampus =  'B', @Race =  '*ALL*', @International =  'B' --446 " +  
	                           "--SELECT @ClassOf =  'SR', @OnCampus =  'B', @Race =  '*ALL*', @International =  'B' --512 " +  
                               "SELECT (SELECT ltrim(rtrim(Session)) + Year FROM Term) as Term,Organization.OrganizationName,MemberType.MemberName,Student.Carroll_ID,Student.FirstName,Student.LastName,Student.Email " +  
                               ", CASE WHEN Student.ClassOf in ('FF') THEN 'First-Time Freshman' \n" +
                               "WHEN Student.ClassOf in ( 'FR') THEN 'Freshman' \n" + 
                               "WHEN Student.ClassOf in ('SO') THEN  'Sophomore' \n" +
                               "WHEN Student.ClassOf in ('JR') THEN  'Junior' \n" + 
                               "WHEN Student.ClassOf in ('SR') THEN  'Senior' \n" + 
                               "WHEN Student.ClassOf in ('GN','GR') THEN 'Graduate Student' \n" +  
                               "WHEN Student.ClassOf in ('S') THEN 'Special' \n" +   
                               "WHEN Student.ClassOf in ( 'UN') THEN  'Unknown' \n END ClassOf \n" +   
				               ", CASE WHEN Student.OnCampus = 'N' THEN 'No' \n ELSE 'Yes' \n END LivesOnCampus, Student.Race " +   
		                       ", CASE WHEN Student.International =  'N' THEN 'No' \n ELSE 'Yes' \n END IntlStudent FROM Student " +  
	                           "INNER JOIN StudentRoster ON StudentRoster.Student_ID = Student.Student_ID INNER JOIN MemberType ON MemberType.Member_ID = StudentRoster.Member_ID " +   
	                           "INNER JOIN Organization ON StudentRoster.Organization_ID = Organization.Organization_ID INNER JOIN OrgStatus ON OrgStatus.Organization_ID = Organization.Organization_ID " +  
	                           "WHERE StudentRoster.EndDate IS NULL --Active Members AND OrgStatus.EndDate IS NULL AND OrgStatus.Status_ID IN (1, 2, 4) --active Orgs AND " + '\u000A'+ 
                               "((Student.ClassOf = @ClassOf OR (@ClassOf = 'GR' AND Student.ClassOf in ('GR','GN')) OR @ClassOf = '**') " +
		                         "AND (Student.OnCampus = @OnCampus OR @OnCampus =  'B') AND (Student.Race = @Race OR @Race = '*ALL*') AND (Student.International = @International OR @International = 'B') ) " +  
	                           "ORDER BY OrganizationName, Student.LastName, Student.FirstName";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ClassOf", classof);
                    cmd.Parameters.AddWithValue("@OnCampus", oncampus);
                    cmd.Parameters.AddWithValue("@Race", race);
                    cmd.Parameters.AddWithValue("@International", international);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_Report_GPAFail(string dom)
        {
            string CS = dom;

            try
            {
                string query = "SELECT (SELECT ltrim(rtrim(Session))+Year FROM Term) as Term,Organization.OrganizationName,MemberType.MemberName,Student.Carroll_ID,Student.FirstName,Student.LastName,Student.Email " +  
		                       ", case when MemberType.BoardMember = 1 and Student.GPABoard = 0 then 'Fail' else null end GPABoard " +  
		                       ", case when Student.GPAOrg = 0 then 'Fail' else 'Pass' end GPAOrg FROM StudentRoster " +  
	                           "INNER JOIN Organization on StudentRoster.Organization_ID = Organization.Organization_ID INNER JOIN OrgStatus on Organization.Organization_ID = OrgStatus.Organization_ID " +  
	                           "INNER JOIN Student on Student.Student_ID = StudentRoster.Student_ID INNER JOIN MemberType on StudentRoster.Member_ID = MemberType.Member_ID " +  
	                           "WHERE StudentRoster.EndDate IS NULL AND ((Student.GPABoard = 0 AND MemberType.BoardMember = 1) OR Student.GPAOrg = 0) " +   
	                           "AND OrgStatus.EndDate IS NULL AND OrgStatus.Status_ID IN (1,2,4) --Active Org ORDER BY StudentRoster.Organization_ID, Student.LastName";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_Report_UpcomingEvents(DateTime startDate, DateTime endDate, string dom)
        {
            string CS = dom;

            try
            {
                string query = "SELECT OrganizationName,OrgEvent.Title,COALESCE(CONVERT(VARCHAR(10), OrgEvent.Date, 101),'') as ShortDate,OrgEvent.Location,OrgEvent.ContactName,OrgEvent.ContactPhone,OrgEvent.ContactEmail FROM OrgEvent " +  
	                           "INNER JOIN Organization ON OrgEvent.Organization_ID = Organization.Organization_ID WHERE Date BETWEEN @StartDate AND coalesce(@EndDate,'01/01/2999') ORDER BY Organization.OrganizationName, OrgEvent.Date";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_ResetStudentRoster(string dom)
        {
            string CS = dom;

            try
            {
                string query = "UPDATE StudentRoster SET EndDate = GETDATE() FROM StudentRoster, Term WHERE StudentRoster.Session = Term.Session AND StudentRoster.Year = Term.Year AND EndDate is null; " +  
                               "DECLARE @session varchar(10); SELECT @session = Session FROM Term;";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateAdvisor(int advID, string lName, string fName, string email, string phone, char offcampus, string advTitle, string profTitle, string dom)
        {
            string CS = dom;

            try
            {
                string query = "UPDATE Advisor SET LastName=@LastName,FirstName=@FirstName,Email=@Email,PhoneNumber=@PhoneNumber, OffCampus=@OffCampus,AdvisorTitle = @AdvisorTitle,ProfessionalTitle=@ProfessionalTitle FROM Advisor WHERE Advisor_ID=@Advisor_ID";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Advisor_ID", advID);
                    cmd.Parameters.AddWithValue("@LastName", lName);
                    cmd.Parameters.AddWithValue("@FirstName", fName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phone);
                    cmd.Parameters.AddWithValue("@OffCampus", offcampus);
                    cmd.Parameters.AddWithValue("@AdvisorTitle", advTitle);
                    cmd.Parameters.AddWithValue("@ProfessionalTitle", profTitle);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateEventComment(int eventID, string text, string dom)
        {
            string CS = dom;

            try
            {
                string query = "UPDATE EventComments SET Text=@Text WHERE EventComment_ID = @EventComment_ID";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Text", text);
                    cmd.Parameters.AddWithValue("@EventComment_ID", eventID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateIncident(int incID, string date, string dateRep, string location, string incident, string result, string notes, string followUp, string reportedBy, string time, int orgID, string dom)
        {
            string CS = dom;
            DateTime Date = Convert.ToDateTime(date);
            DateTime DateReported = Convert.ToDateTime(dateRep);  

            try
            {
                string query = "UPDATE Incident SET [Date] = @Date,[DateReported] = @DateReported,[Location] = @Location,[Incident] = @Incident,[Result] = @Result ,[Notes] = @Notes,[Organization_ID] = @Organization_ID,[ReportedBy] = @ReportedBy,[FollowUp] = @FollowUp,[Time] = @Time " +  
                               "WHERE Incident_ID = @Incident_ID";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Date", Date);
                    cmd.Parameters.AddWithValue("@DateReported", DateReported);
                    cmd.Parameters.AddWithValue("@Location", location);
                    cmd.Parameters.AddWithValue("@Incident", incident);
                    cmd.Parameters.AddWithValue("@Result", result);
                    cmd.Parameters.AddWithValue("@Notes", notes);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@ReportedBy", reportedBy);
                    cmd.Parameters.AddWithValue("@FollowUp", followUp);
                    cmd.Parameters.AddWithValue("@Time", time);
                    cmd.Parameters.AddWithValue("@Incident_ID", incID);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateIncidentComments(int incID, DateTime date, string text, string dom)
        {
            string CS = dom;

            try
            {
                string query = "UPDATE IncidentComments SET Date=@Date, Text=@Text FROM OrgComments WHERE Incident_ID=@Incident_ID";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Incident_ID", incID);
                    cmd.Parameters.AddWithValue("@Text", text);
                    cmd.Parameters.AddWithValue("@Date", date);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateMeetingForOrganizationRequirement(int meetID, int orgID, int stud1, int stud2, string dom)
        {
            string CS = dom;

            try
            {
                string query = "UPDATE [StudentOrgs].[dbo].[OrgRequiredEvents] SET [Student1] = @Student1,[Student2] = @Student2,[Attended] = 1 WHERE Organization_ID = @OrganizationID AND MeetingID = @MeetingID";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Student1", stud1);
                    cmd.Parameters.AddWithValue("@Student2", stud2);
                    cmd.Parameters.AddWithValue("@MeetingID", meetID);
                    cmd.Parameters.AddWithValue("@OrganizationID", orgID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateMemberType(int memID, string membName, string desc, string adddate, string enddate, char board, string dom)
        {
            string CS = dom;
            DateTime AddDate = Convert.ToDateTime(adddate);
            DateTime EndDate = Convert.ToDateTime(enddate);  

            try
            {
                string query = "UPDATE MemberType SET MemberName=@MemberName, Description=@Description, AddDate=@AddDate), EndDate=@EndDate, BoardMember=@BoardMember FROM MemberType WHERE Member_ID=@Member_ID";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Member_ID", memID);
                    cmd.Parameters.AddWithValue("@MemberName", membName);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@AddDate", AddDate);
                    cmd.Parameters.AddWithValue("@EndDate", EndDate);
                    cmd.Parameters.AddWithValue("@BoardMember", board);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateMyOrgEventChecklist(int eventID, string location, char marketing, char catering, char signup, char waiver, char cun, string dom)
        {
            string CS = dom;
           
            try
            {
                string query = "UPDATE [StudentOrgs].[dbo].[OrgEvent] SET  RoomReserved = @location,Marketing = @marketing,Catering = @catering,SignupForm = @event_signup,ActivityWaiver = @activity_waiver,CUNight = @cu_night " +  
	                           "WHERE Event_ID = @Event_ID";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);
                    cmd.Parameters.AddWithValue("@location", location);
                    cmd.Parameters.AddWithValue("@marketing", marketing);
                    cmd.Parameters.AddWithValue("@catering", catering);
                    cmd.Parameters.AddWithValue("@event_signup", signup);
                    cmd.Parameters.AddWithValue("@activity_waiver", waiver);
                    cmd.Parameters.AddWithValue("@cu_night", cun);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateMyOrgEventCosts(int eventID, float dollar, float partnerCost, float van, float bus, string dom)
        {
            string CS = dom;
           //datatypes??
            try
            {
                string query = "UPDATE [StudentOrgs].[dbo].[OrgEvent] SET [DollarsSpentByOrg]=@Dollars, [DollarsSpentByPartner]=@DollarsByPartner, [VanRental]=@VanRental, [BusRental]=@BusRental WHERE Event_ID = @Event_ID";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Dollars", dollar);
                    cmd.Parameters.AddWithValue("@DollarsByPartner", partnerCost);
                    cmd.Parameters.AddWithValue("@BusRental", bus);
                    cmd.Parameters.AddWithValue("@VanRental", van);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateMyOrgEventDetails(int eventID, string title, string desc, char campus, string location, string date, string time, int attendance, string dom)
        {
            string CS = dom;
            DateTime Date = Convert.ToDateTime(date);

            try
            {
                string query = "UPDATE [StudentOrgs].[dbo].[OrgEvent] SET [Title] =@Title,[Description] = @Description,[OnOffCampus] = @OnOffCampus,[Location] = @Location,[Date] = @Date,[Time] = @Time,[Attendance] = @Attendence WHERE Event_ID = @Event_ID";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@OnOffCampus", campus);
                    cmd.Parameters.AddWithValue("@Location", location);
                    cmd.Parameters.AddWithValue("@Date", Date);
                    cmd.Parameters.AddWithValue("@Time", time);
                    cmd.Parameters.AddWithValue("@Attendence", attendance);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_UpdateMyOrgEventDriver(int eventID, int studID, string mode, string dom)
        {
            string CS = dom;
            DateTime Date = Convert.ToDateTime(date);

            try
            {
                string query = "DECLARE @driver1 int,@driver2 int SELECT  @driver1 = Driver1, @driver2 = Driver2 FROM OrgEvent WHERE Event_ID = @Event_ID " +  
	                           "IF @mode = 'D' BEGIN IF @driver1 = @Student_ID BEGIN UPDATE [StudentOrgs].[dbo].[OrgEvent] SET Driver1 = null WHERE Event_ID = @Event_ID " +  
                               "if @@ERROR<>0 BEGIN raiserror() return -100 END BEGIN raiserror('100',16,1) return -100; END END " +  
		                       "IF @driver2 = @Student_ID BEGIN UPDATE [StudentOrgs].[dbo].[OrgEvent] SET Driver2 = null WHERE Event_ID = @Event_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);
                    cmd.Parameters.AddWithValue("@Student_ID", studID);
                    cmd.Parameters.AddWithValue("@mode", mode);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateMyOrgEventPartner(int eventID, string orgWorkedWith, string ContactName, string ContactEmail, string ContactPhone, int hrsVol, string dom)
        {
            string CS = dom;

            try
            {
                string query = "UPDATE [StudentOrgs].[dbo].[OrgEvent] SET [OrganizationWorkedWith] = @OrganizationWorkedWith, [ContactPhone] = @ContactPhone, [ContactEmail] = @ContactEmail, [ContactName] = @ContactName ,[HoursVolunteered] = @HoursVolunteered WHERE Event_ID = @Event_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@OrganizationWorkedWith", orgWorkedWith);
                    cmd.Parameters.AddWithValue("@ContactPhone", ContactPhone);
                    cmd.Parameters.AddWithValue("@ContactEmail", ContactEmail);
                    cmd.Parameters.AddWithValue("@ContactName", ContactName);
                    cmd.Parameters.AddWithValue("@HoursVolunteered", hrsVol);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Doubt
        public static DataTable sp_UpdateMyOrgs_Organization(int orgID, char updateMode, string desc, string meetDay, string meetTime, string meetFrq, string meetBldg, string meetRoom, string dom)
        {
            string CS = dom;
           
            try
            {
                string query = "IF @UpdateMode = '1' BEGIN UPDATE Organization SET Description=@Description FROM Organization WHERE Organization_ID=@Organization_ID " +  
		                       "if @@ERROR<>0  BEGIN raiserror('100',16,1) return -100; END END " +   
		                       "IF @UpdateMode = '2'  BEGIN UPDATE Organization SET MeetingDay=@MeetingDay FROM Organization WHERE Organization_ID=@Organization_ID " +  
		                       "if @@ERROR<>0  BEGIN raiserror('100',16,1) return -100; END END " +   
                               "IF @UpdateMode = '3' BEGIN UPDATE Organization SET MeetingTime=@@MeetingTime FROM Organization WHERE Organization_ID=@Organization_ID " +  
		                       "if @@ERROR<>0  BEGIN raiserror('100',16,1) return -100; END END " +   
                               "IF @UpdateMode = '4' BEGIN UPDATE Organization SET @MeetingFrequency=@@MeetingFrequency FROM Organization WHERE Organization_ID=@Organization_ID " +  
		                       "if @@ERROR<>0  BEGIN raiserror('100',16,1) return -100; END END " +   
                               "IF @UpdateMode = '5' BEGIN UPDATE Organization SET @MeetingBuilding=@@MeetingBuilding FROM Organization WHERE Organization_ID=@Organization_ID " +  
		                       "if @@ERROR<>0  BEGIN raiserror('100',16,1) return -100; END END " +   
                               "IF @UpdateMode = '6' BEGIN UPDATE Organization SET @MeetingRoom=@@MeetingRoom FROM Organization WHERE Organization_ID=@Organization_ID " +  
		                       "if @@ERROR<>0  BEGIN raiserror('100',16,1) return -100; END END return 0; END";
                              
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@UpdateMode", updateMode);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@MeetingDay", meetDay);
                    cmd.Parameters.AddWithValue("@MeetingTime", meetTime);
                    cmd.Parameters.AddWithValue("@MeetingFrequency", meetFrq);
                    cmd.Parameters.AddWithValue("@MeetingBuilding", meetBldg);
                    cmd.Parameters.AddWithValue("@MeetingRoom", meetRoom);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateMyOrgServiceDetails(int projID, string title, string desc, char ongoing, string location, string strDate, string timeFrom, string timeTo, string dom)
        {
            string CS = dom;
            DateTime StartDate = Convert.ToDateTime(strDate);


            try
            {
                string query = "UPDATE [StudentOrgs].[dbo].[ServiceProject] SET [Title] = @Title,[Description] = @Description,[OnGoing] = @OnGoing,[Location] = @Location,[StartDate] = @StartDate,[TimeFrom] = @TimeFrom,[TimeTo] = @TimeTo " +   
	                           "WHERE ServiceProject_ID = @ServiceProject_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@OnGoing", ongoing);
                    cmd.Parameters.AddWithValue("@Location", location);
                    cmd.Parameters.AddWithValue("@StartDate", StartDate);
                    cmd.Parameters.AddWithValue("@TimeFrom", timeFrom);
                    cmd.Parameters.AddWithValue("@TimeTo", timeTo);
                    cmd.Parameters.AddWithValue("@ServiceProject_ID", projID);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateMyOrgServicePartner(int servID, string commPartner, string contName, string contEmail, string contPhone, string dom)
        {
            string CS = dom;
            
            try
            {
                string query = "UPDATE dbo.ServiceProject SET CommunityPartner = @CommunityPartner, ContactPhone = @ContactPhone, ContactEmail = @ContactEmail, ContactName = @ContactName WHERE ServiceProject_ID = @Service_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Service_ID", servID);
                    cmd.Parameters.AddWithValue("@CommunityPartner", commPartner);
                    cmd.Parameters.AddWithValue("@ContactName", contName);
                    cmd.Parameters.AddWithValue("@ContactEmail", contEmail);
                    cmd.Parameters.AddWithValue("@ContactPhone", contPhone);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateMyOrgServiceResults(int servID, int planVolunteers, int planHrs, string planItems, int volunteers, int hrs, string items, string dom)
        {
            string CS = dom;
           
            try
            {
                string query = "UPDATE dbo.ServiceProject SET NoOfVolunteers = @Actual_Volunteers,HoursVolunteered = @Actual_Hours,Funds = @Actual_Items,Planned_NoOfVolunteers = @Planned_Volunteers,Planned_HoursVolunteered = @Planned_Hours,Planned_Funds = @Planned_Items WHERE ServiceProject_ID = @Service_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Service_ID", servID);
                    cmd.Parameters.AddWithValue("@Planned_Volunteers", planVolunteers);
                    cmd.Parameters.AddWithValue("@Planned_Hours", planHrs);
                    cmd.Parameters.AddWithValue("@Planned_Items", planItems);
                    cmd.Parameters.AddWithValue("@Actual_Volunteers", volunteers);
                    cmd.Parameters.AddWithValue("@Actual_Hours", hrs);
                    cmd.Parameters.AddWithValue("@Actual_Items", items);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateOrgAdvisor(int orgID, int advID, DateTime startDate, DateTime endDate, string profTitle, string advTitle, string dom)
        {
            string CS = dom;
            
            try
            {
                string query = "UPDATE OrgAdvisor SET StartDate=@StartDate, EndDate=@EndDate, ProfessionalTitle=@ProfessionalTitle, AdvisorTitle=@AdvisorTitle FROM OrgAdvisor WHERE Organization_ID=@Organization_ID AND Advisor_ID=@Advisor_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Advisor_ID", advID);
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Parameters.AddWithValue("@ProfessionalTitle", profTitle);
                    cmd.Parameters.AddWithValue("@AdvisorTitle", advTitle);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateOrgAdvisorRow(int orgID, int advID, string dom)
        {
            string CS = dom;

            try
            {
                string query = "UPDATE OrgAdvisor SET EndDate = CURRENT_TIMESTAMP WHERE Advisor_ID = @Advisor_ID And Organization_ID = @Organization_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Advisor_ID", advID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateOrganization(int orgID, string orgName, string desc, DateTime meetTime, string meetDay, string meetBldg, string meetRoom, DateTime dateProp, DateTime propAccept, DateTime propDenied, string propNotes,
            DateTime contSubmitted, DateTime contApproved, string contNotes, DateTime regApproval, char CXcode, int catID, char reqMet, DateTime reqMetDate, string oAcct, string projCode, string dom)
        {
  
            string CS = dom;

            try
            {
                string query = "UPDATE Organization SET OrganizationName=@OrganizationName, Description=@Description, MeetingTime=@MeetingTime, MeetingDay=@MeetingDay,MeetingBuilding=@MeetingBuilding, MeetingRoom=@MeetingRoom, DateProposed=@DateProposed," +
		                       "ProposalAccepted=@ProposalAccepted, ProposalDenied=@ProposalDenied, ProposalNotes=@ProposalNotes, ConstitutionSubmitted=@ConstitutionSubmitted, ConstitutionAccepted=@ConstitutionApproved, ConstitutionNotes=@ConstitutionNotes," +
                               "RegistrarApproval=@RegistrarApproval, Category_ID=@Category_ID, RequirementsMet=@RequirementsMet, RequirementsMetDate=@RequirementsMetDate, OAccount=@OAccount, ProjectCode=@ProjectCode FROM Organization  " +  
	                           "WHERE Organization_ID=@Organization_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@OrganizationName", orgName);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@MeetingTime", meetTime);
                    cmd.Parameters.AddWithValue("@MeetingDay", meetDay);
                    cmd.Parameters.AddWithValue("@MeetingBuilding", meetBldg);
                    cmd.Parameters.AddWithValue("@MeetingRoom", meetRoom);
                    cmd.Parameters.AddWithValue("@DateProposed", dateProp);
                    cmd.Parameters.AddWithValue("@ProposalAccepted", propAccept);
                    cmd.Parameters.AddWithValue("@ProposalDenied", propDenied);
                    cmd.Parameters.AddWithValue("@ProposalNotes", propNotes);
                    cmd.Parameters.AddWithValue("@ConstitutionSubmitted", contSubmitted);
                    cmd.Parameters.AddWithValue("@ConstitutionApproved", contApproved);
                    cmd.Parameters.AddWithValue("@ConstitutionNotes", contNotes);
                    cmd.Parameters.AddWithValue("@RegistrarApproval", regApproval);
                    cmd.Parameters.AddWithValue("@CX_Code", CXcode);
                    cmd.Parameters.AddWithValue("@Category_ID", catID);
                    cmd.Parameters.AddWithValue("@RequirementsMet", reqMet);
                    cmd.Parameters.AddWithValue("@RequirementsMetDate", reqMetDate);
                    cmd.Parameters.AddWithValue("@OAccount", oAcct);
                    cmd.Parameters.AddWithValue("@ProjectCode", projCode);


                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateOrganizationDates(int orgID, string dateProp, string propAccepted, string propDenied, string propNotes, string constSubmitted, string constApproved, string constDenied, string constUpdated, string constNotes, string dom)
        {
           
            string CS = dom;
        //DateTime???
            /*
             * DateTime DatePropsed = Convert.ToDateTime(dateProp);
             * DateTime PropAccepted = Convert.ToDateTime(propAccepted);
             * DateTime PropDenied = Convert.ToDateTime(propDenied);
             * DateTime ConstSubmitted = Convert.ToDateTime(constSubmitted);
             * DateTime ConstApproved = Convert.ToDateTime(constApproved);
             * DateTime ConstDenied = Convert.ToDateTime(constDenied);
             * DateTime ConstUpdated = Convert.ToDateTime(constUpdated);

             */
            try
            {
                string query = "UPDATE Organization SET DateProposed=@DateProposed,ProposalAccepted=@ProposalAccepted,ProposalDenied=@ProposalDenied,,ProposalNotes=@ProposalNotes,ConstitutionSubmitted=@ConstitutionSubmitted,ConstitutionAccepted=@ConstitutionApproved,ConstitutionDenied=@ConstitutionDenied,ConstitutionUpdated=@ConstitutionUpdated,ConstitutionNotes=@ConstitutionNotes " +  
	                           "FROM Organization WHERE Organization_ID=@Organization_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@DateProposed", dateProp);
                    cmd.Parameters.AddWithValue("@ProposalAccepted", propAccepted);
                    cmd.Parameters.AddWithValue("@ProposalDenied", propDenied);
                    cmd.Parameters.AddWithValue("@ProposalNotes", propNotes);
                    cmd.Parameters.AddWithValue("@ConstitutionSubmitted", constSubmitted);
                    cmd.Parameters.AddWithValue("@ConstitutionApproved", constApproved);
                    cmd.Parameters.AddWithValue("@ConstitutionDenied", constDenied);
                    cmd.Parameters.AddWithValue("@ConstitutionUpdated", constUpdated);
                    cmd.Parameters.AddWithValue("@ConstitutionNotes", constNotes);
                   

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateOrganizationRequirementMeetingByReqIDMeetID(int reqEventID, int meetID, string session, string year, string meetDate, string meetTime, string meetBldg, string meetRoom, string notes, string dom)
        {
            string CS = dom;
            DateTime MeetDate = Convert.ToDateTime(meetDate);
            try
            {
                string query = "UPDATE [StudentOrgs].[dbo].[RequiredEventMeetings] SET [Session] = @Session,[Year] = @Year,[MeetingDate] = Convert(Datetime,@MeetingDate),[MeetingTime] = @MeetingTime,[MeetingBuilding] = @MeetingBuilding,[MeetingRoomNumber] = @MeetingRoomNumber,[Notes] = @Notes " +  
	                           "WHERE RequiredEventID=@RequiredEventID AND MeetingID =@MeetingID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MeetingID", meetID);
                    cmd.Parameters.AddWithValue("@Session", session);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@MeetingDate", MeetDate);
                    cmd.Parameters.AddWithValue("@MeetingTime", meetTime);
                    cmd.Parameters.AddWithValue("@MeetingBuilding", meetBldg);
                    cmd.Parameters.AddWithValue("@MeetingRoomNumber", meetRoom);
                    cmd.Parameters.AddWithValue("@Notes", notes);
                    cmd.Parameters.AddWithValue("@RequiredEventID", reqEventID);


                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateOrgCategory(int catID, string catName, string desc, string adddate, string enddate, char greek, string dom)
        {
            string CS = dom;
            DateTime EndDate = Convert.ToDateTime(enddate);
            DateTime AddDate = Convert.ToDateTime(adddate);

            try
            {
                string query = "UPDATE OrgCategory SET CategoryName=@CategoryName, Description=@Description, AddDate=CONVERT(datetime,@AddDate), EndDate=CONVERT(datetime,@EndDate), GreekOrg=@GreekOrg FROM MemberType WHERE Category_ID=@Category_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Category_ID", catID);
                    cmd.Parameters.AddWithValue("@CategoryName", catName);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@AddDate", AddDate);
                    cmd.Parameters.AddWithValue("@EndDate", EndDate);
                    cmd.Parameters.AddWithValue("@GreekOrg", greek);
               
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateOrgComments(int orgID, string text, DateTime date, string dom)
        {
            string CS = dom;
            
            try
            {
                string query = "UPDATE OrgComments SET Date=@Date, Text=@Text FROM OrgComments WHERE Organization_ID=@Organization_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@Text", text);
                    
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateOrgEvent(int eventID, string date, string time, string location, string title, string desc, string orgWorked, string phone, int vanRental, int busRental, string contactName, int driver1, int driver2, char onoffcampus, 
            char roomRes, char marketing, char catering, char signup, char activityWaiver, char cun, string email, int hoursVol, int orgID, string dollars, string dollarsPartner, string dom)
        {
            string CS = dom;
            DateTime Date = Convert.ToDateTime(date);
            roomRes = '0';
            marketing = '0';
            catering = '0';
            signup = '0';
            activityWaiver = '0';
            cun = '0';


            try
            {
                string query = "UPDATE [StudentOrgs].[dbo].[OrgEvent] SET [Organization_ID] = @Organization_ID,[Date] = @Date,[Time] = @Time,[Location] = @Location,[Description] = @Description,[OrganizationWorkedWith] = @OrganizationWorkedWith,[ContactPhone] = @ContactPhone,[ContactEmail] = @ContactEmail" + 
	                           ",[Attendance] = @Attendence,[HoursVolunteered] = @,[DollarsSpentByOrg] = Convert(Decimal(18,0),@Dollars),[DollarsSpentByPartner]=Convert(Decimal(18,0),@DollarsByPartner),[Title] = @Title,[OnOffCampus] = @OnOffCampus,[VanRental] = @VanRental" +
	                           ",[BusRental] = @,[ContactName] = @ContactName,[Driver1] = @Driver1,[Driver2] = @Driver2,[RoomReserved] = @RoomReserved,[Marketing] = @Marketing,[Catering] = @Catering,[SignupForm] = @SignupForm,[ActivityWaiver] = @ActivityWaiver,[CUNight] = @CUNight " +   	  
	                           "WHERE Event_ID = @Event_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Event_ID", eventID);
                    cmd.Parameters.AddWithValue("@Date", Date);
                    cmd.Parameters.AddWithValue("@Time", time);
                    cmd.Parameters.AddWithValue("@Location", location);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@OrganizationWorkedWith", orgWorked);
                    cmd.Parameters.AddWithValue("@ContactPhone", phone);
                    cmd.Parameters.AddWithValue("@ContactEmail", email);
                    cmd.Parameters.AddWithValue("@Attendence", attendance);
                    cmd.Parameters.AddWithValue("@HoursVolunteered", hoursVol);
                    cmd.Parameters.AddWithValue("@Dollars", dollars);
                    cmd.Parameters.AddWithValue("@DollarsByPartner", dollarsPartner);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@OnOffCampus", onoffcampus);
                    cmd.Parameters.AddWithValue("@VanRental", vanRental);
                    cmd.Parameters.AddWithValue("@BusRental", busRental);
                    cmd.Parameters.AddWithValue("@ContactName", contactName);
                    cmd.Parameters.AddWithValue("@Driver1", driver1);
                    cmd.Parameters.AddWithValue("@Driver2", driver2);
                    cmd.Parameters.AddWithValue("@RoomReserved", roomRes);
                    cmd.Parameters.AddWithValue("@Marketing", marketing);
                    cmd.Parameters.AddWithValue("@Catering", catering);
                    cmd.Parameters.AddWithValue("@SignupForm", signup);
                    cmd.Parameters.AddWithValue("@ActivityWaiver", activityWaiver);
                    cmd.Parameters.AddWithValue("@CUNight", cun);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateRequiredEvent(int reqEventID, string eventDesc, string date, string reqEvent, string dom)
        {
            string CS = dom;
            DateTime addDate = Convert.ToDateTime(date);
            try
            {
                string query = "UPDATE RequiredEvents SET RequiredEvent=@RequiredEvent, EventDescription=@EventDescription, AddDate=@AddDate) FROM RequiredEvents WHERE RequiredEvent_ID=@RequiredEvent_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@RequiredEvent", reqEvent);
                    cmd.Parameters.AddWithValue("@EventDescription", eventDesc);
                    cmd.Parameters.AddWithValue("@AddDate", addDate);
                    cmd.Parameters.AddWithValue("@RequiredEvent_ID", reqEventID);

                    
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateServiceComment(int commentID, string text, string dom)
        {
            string CS = dom;
            try
            {
                string query = "UPDATE ServiceProjectComments SET Text=@Text WHERE ServiceComment_ID = @ServiceComment_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ServiceComment_ID", commentID);
                    cmd.Parameters.AddWithValue("@Text", text);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateServiceComment(int projID, int orgID, string communityPartner, string contactName, string phone, string email, int numVol, int hoursVol, string desc, char ongoing, string location, 
            string startDate, string endDate, string timeFrom, string timeTo, string meetday, string meetingFreq, string title, string funds, int planNumVol, int planHoursVol, string planFunds, string dom)
        {
            string CS = dom;
            DateTime sDate = Convert.ToDateTime(startDate);
            DateTime eDate = Convert.ToDateTime(endDate);
            
            try
            {
                string query = "UPDATE [StudentOrgs].[dbo].[ServiceProject] SET [Organization_ID] = @Organization_ID,[CommunityPartner] = @CommunityPartner,[ContactName] = @ContactName,[ContactPhone] = @ContactPhone,[ContactEmail] = @ContactEmail,[NoOfVolunteers] = @NoOfVolunteers,[HoursVolunteered] = @HoursVolunteered" +
                               ",[Description] = @Description,[OnGoing] = @OnGoing,[Location] = @Location,[StartDate] = @StartDate,[EndDate] = @EndDate,[TimeFrom] = @TimeFrom,[TimeTo] = @TimeTo,[MeetDay] = @MeetDay,[MeetFrequency] = @MeetFrequency,[Title] = @Title,[Funds] = @Funds,[Planned_NoOfVolunteers] = @Planned_NoOfVolunteers,[Planned_HoursVolunteered] = @Planned_HoursVolunteered,[Planned_Funds] = @Planned_Funds " +   	  
	                           "WHERE ServiceProject_ID = @ServiceProject_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@CommunityPartner", communityPartner);
                    cmd.Parameters.AddWithValue("@ContactName", contactName);
                    cmd.Parameters.AddWithValue("@ContactPhone", phone);
                    cmd.Parameters.AddWithValue("@ContactEmail", email);
                    cmd.Parameters.AddWithValue("@NoOfVolunteers", numVol);
                    cmd.Parameters.AddWithValue("@HoursVolunteered", hoursVol);
                    cmd.Parameters.AddWithValue("@Description", desc);
                    cmd.Parameters.AddWithValue("@OnGoing", ongoing);
                    cmd.Parameters.AddWithValue("@Location", location);
                    cmd.Parameters.AddWithValue("@StartDate", sDate);
                    cmd.Parameters.AddWithValue("@EndDate", eDate);
                    cmd.Parameters.AddWithValue("@TimeFrom", timeFrom);
                    cmd.Parameters.AddWithValue("@TimeTo", timeTo);
                    cmd.Parameters.AddWithValue("@MeetDay", meetday);
                    cmd.Parameters.AddWithValue("@MeetFrequency", meetingFreq);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Funds", funds);
                    cmd.Parameters.AddWithValue("@Planned_NoOfVolunteers", planNumVol);
                    cmd.Parameters.AddWithValue("@Planned_HoursVolunteered", planHoursVol);
                    cmd.Parameters.AddWithValue("@Planned_Funds", planFunds);
                    cmd.Parameters.AddWithValue("@ServiceProject_ID", projID);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateStudent(int studID, char certifiedDriver, char approvedDriver, char greekGPA, string dom)
        {
            string CS = dom;
            try
            {
                string query = "UPDATE Student  SET CertifiedDriver=@CertifiedDriver, ApprovedDriver=@ApprovedDriver,GreekGPA=@GreekGPA FROM Student WHERE Student_ID=@Student_ID";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@CertifiedDriver", certifiedDriver);
                    cmd.Parameters.AddWithValue("@ApprovedDriver", approvedDriver);
                    cmd.Parameters.AddWithValue("@GreekGPA", greekGPA);
                    cmd.Parameters.AddWithValue("@Student_ID", studID);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }

        //Correct
        public static DataTable sp_UpdateStudentRoster(int studID, int orgID, int membID, String strbDate, char primaryCont, string dom)
        {
            DateTime beginDate = Convert.ToDateTime(strbDate);
            string CS = dom;
            try
            {
                string query = "DECLARE @Member int,@Session varchar(4),@Year varchar(4) " + " Select @Session = Session From Term Select @Year = Year From Term " +     
	                           "IF EXISTS(SELECT Student_ID FROM StudentRoster WHERE Student_ID = @Student_ID AND Organization_ID = @Organization_ID AND ENDDATE IS NULL) BEGIN " +  
		                       "SELECT @Member =  Member_ID FROM StudentRoster WHERE Student_ID = @Student_ID AND Organization_ID = @Organization_ID " +  
		                       "IF (@Member != @Member_ID) UPDATE StudentRoster SET ENDDATE = CURRENT_TIMESTAMP, PrimaryContact = @PrimaryContact, Session = @Session, Year = @Year " +  
			                   "WHERE Student_ID = @Student_ID AND Organization_ID = @Organization_ID AND ENDDATE IS NULL " +   
			                     "INSERT INTO [StudentOrgs].[dbo].[StudentRoster] ([Organization_ID],[Student_ID],[BeginDate],[EndDate],[Member_ID],[Session],[Year],[Status_ID],[PrimaryContact]) " + 
                                 "VALUES (@Organization_ID,@Student_ID,CURRENT_TIMESTAMP,NULL,@Member_ID,NULL,NULL,1,@PrimaryContact) " +   
		                       "ELSE UPDATE StudentRoster SET PrimaryContact = @PrimaryContact, Session = @Session, Year = @Year " +  
			                   "WHERE Student_ID = @Student_ID AND Organization_ID = @Organization_ID AND ENDDATE IS NULL " +  
                               "ELSE INSERT INTO [StudentOrgs].[dbo].[StudentRoster] ([Organization_ID],[Student_ID],[BeginDate],[EndDate],[Member_ID],[Session],[Year],[Status_ID],[PrimaryContact]) " +  
                               "VALUES (@Organization_ID,@Student_ID,CURRENT_TIMESTAMP,NULL,@Member_ID,@Session,@Year,1,@PrimaryContact)";
                
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Student_ID", studID);
                    cmd.Parameters.AddWithValue("@PrimaryContact", primaryCont);
                    cmd.Parameters.AddWithValue("@Member_ID", membID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }
        //Correct
        public static DataTable sp_UpdateStudentRosterRow(int studID, int orgID, string dom)
        {
            string CS = dom;
            try
            {
                string query = "DECLARE @Session varchar(4),@Year varchar(4) Select @Session = Session From Term " +  
	                           "Select @Year = Year From Term UPDATE StudentRoster " +   
                               "SET EndDate = CURRENT_TIMESTAMP, Session = @Session, Year = @Year " +   
                               "WHERE Student_ID = @Student_ID AND Organization_ID = @Organization_ID AND ENDDATE IS NULL AND Member_ID = 22";
                
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                    cmd.Parameters.AddWithValue("@Student_ID", studID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    dt.Load(rdr);
                }
                return dt;
            }
            catch (exception ex)
            {
                throw ex;
            }
        }
    //Correct
    public static DataTable sp_UpdateStudentRosterToDeleteMember(int studID, int orgID, String strbDate, char primaryCont, string dom)
    {
        DateTime beginDate = Convert.ToDateTime(strbDate);
        string CS = dom;
        try
        {
            string query = "IF EXISTS(SELECT Student_ID FROM StudentRoster WHERE Student_ID = @Student_ID AND Organization_ID = @Organization_ID AND ENDDATE IS NULL) " +   
		                   "UPDATE StudentRoster SET ENDDATE = CURRENT_TIMESTAMP, PrimaryContact = @PrimaryContact " +   
		                   "WHERE Student_ID = @Student_ID AND Organization_ID = @Organization_ID AND ENDDATE IS NULL";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                cmd.Parameters.AddWithValue("@Student_ID", studID);
                cmd.Parameters.AddWithValue("@PrimaryContact", primaryCont);

                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;
        }
        catch (exception ex)
        {
            throw ex;
        }
    }
    //Doubt
    public static DataTable sp_UpdateUser(String lname, String fname, String pass, String user, char isActive, int role, String phone, string email, string dom)
    {
      
        string CS = dom;
        try
        {
            string query = "SELECT  @LastName = ltrim(rtrim(@LastName)),@FirstName = ltrim(rtrim(@FirstName)),@Password = ltrim(rtrim(@Password)),@UserId = ltrim(rtrim(@UserId)),@Phone = ltrim(rtrim(@Phone)),@Email = ltrim(rtrim(@Email)) " +   
                           "IF (@LastName is null OR @LastName = '') OR (@FirstName is null OR @FirstName = '') " + "BEGIN \n raiserror( '100',16,1) \n return -100; " +   
	                       "IF (@Password is null OR @Password = '') " +  "BEGIN \n raiserror('101',16,1) \n return -101; " +   
	                       "IF @Phone = '' " + "select @Phone = null" + " \n IF @Email = '' " + " select @Email = null " +  
	                       "UPDATE Users SET LastName = coalesce(@LastName,LastName),FirstName = coalesce(@FirstName,FirstName),Password = coalesce(@Password,Password),Is_Active = coalesce(@IsActive,Is_Active),Role_ID = coalesce(@RoleID,Role_ID),Phone = @Phone,Email = @Email " +   
	                       "FROM Users WHERE User_ID = @UserId";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@LastName", lname);
                cmd.Parameters.AddWithValue("@FirstName", fname);
                cmd.Parameters.AddWithValue("@Password", pass);
                cmd.Parameters.AddWithValue("@IsActive", isActive);
                cmd.Parameters.AddWithValue("@RoleID", role);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@UserId", user);
                
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;
        }
        catch (exception ex)
        {
            throw ex;
        }
    }

    //Doubt
    public static DataTable sp_UpdateUserOrgs(String user, String orgID, String active, String inactive, string dom)
    {
        DateTime activeDate = Convert.ToDateTime(active);
        DateTime inactiveDate = Convert.ToDateTime(inactive);
        string CS = dom;
        try
        {
            string query = "IF @OrganizationID is null OR NOT EXISTS(select * from dbo.Organization where Organization_ID = @OrganizationID) " +   
		                   "RaisError('100', 16, 1) RETURN -100; " +  	
	                       "IF @UserID is null OR NOT EXISTS(select * from dbo.Users where User_ID = @UserID) " +  
		                   "RaisError('101', 16, 1) RETURN -101; " +  
                           "UPDATE UserOrgs SET inactive_date = @InActiveDate " +   
	                       "FROM UserOrgs WHERE User_ID = @UserId AND Organization_ID = @OrganizationID";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", user);
                cmd.Parameters.AddWithValue("@OrganizationID", orgID);
                cmd.Parameters.AddWithValue("@InActiveDate", inactiveDate);

                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;
        }
        catch (exception ex)
        {
            throw ex;
        }
    }
    //Doubt - Errors
    public static DataTable sp_CX_Import(string dom)
    {
        string CS = dom;
        try
        {
            string query = "create table #temp_student_import(lastname varchar(50),firstname varchar(50),carroll_id int,classof varchar(2),email varchar(50),preferredphone varchar(20),active bit, " +
                           "gender bit,gpaorg bit,gpaboard bit,ferpa bit,major varchar(50),major2 varchar(50),oncampus varchar(1),race varchar(50),international varchar(1)); " +  
                           "bulk insert #temp_student_import from 'C:\My Files\Desktop\StudentOrgsStudent.csv' " + 
                           "with ( fieldterminator =  ',', rowterminator = '\n'); " +  
                           "update #temp_student_import set lastname = REPLACE(lastname,'"',''), firstname = REPLACE(firstname,'"','')," +
                           "classof = REPLACE(classof,'"',''), email = REPLACE(email,'"',''), preferredphone = REPLACE(preferredphone,'"',''), major = REPLACE(major,'"',''), major2 = REPLACE(major2,'"',''), race = REPLACE(race,'"','');" +
                           "update Student set LastName = temp.lastname,FirstName = temp.firstname,ClassOf = temp.classof,Email = temp.email,PreferredPhone = temp.preferredphone,Active = temp.active,Gender = temp.gender, " +
                           "GPAOrg = temp.gpaorg,Major = temp.major,Major2 = temp.major2,GPABoard = temp.gpaboard,Ferpa = temp.ferpa,OnCampus = temp.oncampus,Race = temp.race,International = temp.international from #temp_student_import temp where Student.Carroll_ID = temp.carroll_id; " +  
                           "delete from #temp_student_import where #temp_student_import.carroll_id in (select carroll_id from Student); " +   
                           "Insert into Student (LastName,FirstName,ClassOf,Address,Email,PreferredPhone,Active,Gender,Carroll_ID,GPAOrg,Major,Major2,CertifiedDriver,ApprovedDriver,GPABoard,Ferpa,GreekGPA,OnCampus,Race,International) " +   
                           "select lastname,firstname,classof,null,email,preferredphone,active,gender,carroll_id,gpaorg,major,major2,null,null,gpaboard,ferpa,null,oncampus,race,international from #temp_student_import; " +  
                           "drop table #temp_student_import;";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;
        }
        catch (exception ex)
        {
            throw ex;
        }
    }
    //Correct
    public static DataTable sp_DeleteEvent(int eventID, string dom)
    {
        string CS = dom;
        try
        {
            string query = "DELETE FROM OrgEventFiles WHERE Event_ID = @Event_ID";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Event_ID", eventID);
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;
        }
        catch (exception ex)
        {
            throw ex;
        }
    }

    //Correct
    public static DataTable sp_DeleteEventComment(int eventCommentID, string dom)
    {
        string CS = dom;
        try
        {
            string query = "DELETE FROM EventComments WHERE EventComment_ID = @EventComment_ID";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EventComment_ID", eventCommentID);
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;
        }
        catch (exception ex)
        {
            throw ex;
        }
    }

    //Correct
    public static DataTable sp_DeleteFile(int fileNum, string dom)
    {
        string CS = dom;
        try
        {
            string query = "DELETE FROM Files WHERE file_number = @file_number";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@file_number", fileNum);
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;
        }
        catch (exception ex)
        {
            throw ex;
        }
    }

    //Correct
    public static DataTable sp_DeleteNonMembersInvoluvedForIncident(int incidentID, string dom)
    {
        string CS = dom;
        try
        {
            string query = "DELETE FROM NonMembersInvolved WHERE Incident_ID=@Incident_ID";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Incident_ID", incidentID);
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;
        }
        catch (exception ex)
        {
            throw ex;
        }
    }
    
    //Correct
    public static DataTable sp_DeleteOrgFile(int fileNum, int orgID, string dom)
    {
        string CS = dom;
        try
        {
            string query = "DELETE FROM OrgFiles WHERE file_number = @file_number AND Organization_ID = @Organization_ID exec dbo.sp_DeleteFile @file_number";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@file_number", fileNum);
                cmd.Parameters.AddWithValue("@Organization_ID", orgID);
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;
        }
        catch (exception ex)
        {
            throw ex;
        }
    }

    //Correct
    public static DataTable sp_DeletepeopleInvoluvedForIncident(int incidentID, string dom)
    {
        string CS = dom;
        try
        {
            string query = "DELETE FROM PeopleInvolved WHERE Incident_ID=@Incident_ID";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Incident_ID", incidentID);
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;
        }
        catch (exception ex)
        {
            throw ex;
        }
    }
    
    //Correct
    public static DataTable sp_DeleteServiceComment(int servCommentID, string dom)
    {
        string CS = dom;
        try
        {
            string query = "DELETE FROM ServiceProjectComments WHERE ServiceComment_ID = @ServiceComment_ID";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ServiceComment_ID", servCommentID);
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;
        }
        catch (exception ex)
        {
            throw ex;
        }
    }
    
    //Correct
    public static DataTable sp_DeleteServiceProject(int servID, string dom)
    {
        string CS = dom;
        try
        {
            string query = "DELETE FROM ServiceProjectFiles WHERE ServiceProject_ID = @Service_ID";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Service_ID", servID);
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            return dt;
        }
        catch (exception ex)
        {
            throw ex;
        }
    }

    }
}
