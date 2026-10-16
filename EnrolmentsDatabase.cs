using System.Data.SqlClient;
using RaceDay_poe_part2.Models;

namespace RaceDay_poe_part2.backend;

public class EnrolmentDatabase
{

    //filled when a read method has an error
    public string errorMessage = "";

    string connectionString = @"Data source=(localdb)\race_day;Database=race_day;";

    string selectEnrolment = "SELECT en.enrolment_id, en.enrolment_date, en.status, en.user_id, " +
                             "u.FirstName + ' ' + u.LastName AS participant_name, " +
                             "en.event_id, e.event_name, e.organiser_id, en.category_id, c.category_name " +
                             "FROM EventEnrolment en " +
                             "INNER JOIN users u ON en.user_id = u.UserId " +
                             "INNER JOIN [Event] e ON en.event_id = e.event_id " +
                             "INNER JOIN EventCategory c ON en.category_id = c.category_id ";

    public string CreateEnrolment(int userId, int eventId, int categoryId)
    {//start of CreateEnrolment
        string result = "Failed to enrol in the event";

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                string query = "INSERT INTO EventEnrolment (user_id, event_id, category_id) " +
                               "VALUES (@userId, @eventId, @categoryId)";

                SqlCommand run_query = new SqlCommand(query, connect);
                run_query.Parameters.AddWithValue("@userId", userId);
                run_query.Parameters.AddWithValue("@eventId", eventId);
                run_query.Parameters.AddWithValue("@categoryId", categoryId);

                run_query.ExecuteNonQuery();
                result = "Enrolled successfully, your enrolment is Pending...";

                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            result = error.Message;
        }//end of catch

        return result;
    }//end of CreateEnrolment


    public Enrolments GetEnrolmentById(int enrolmentId)
    {//start of GetEnrolmentById
        Enrolments enrolment = null;

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                SqlCommand run_query = new SqlCommand(selectEnrolment + "WHERE en.enrolment_id=@id", connect);
                run_query.Parameters.AddWithValue("@id", enrolmentId);

                SqlDataReader read = run_query.ExecuteReader();

                if (read.Read())
                {
                    enrolment = ReadEnrolment(read);
                }

                read.Close();
                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            errorMessage = error.Message;
        }//end of catch

        return enrolment;
    }//end of GetEnrolmentById


    public List<Enrolments> GetEnrolmentsByUser(int userId)
    {//start of GetEnrolmentsByUser
        List<Enrolments> enrolments = new List<Enrolments>();

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                SqlCommand run_query = new SqlCommand(selectEnrolment + "WHERE en.user_id=@userId ORDER BY en.enrolment_date DESC", connect);
                run_query.Parameters.AddWithValue("@userId", userId);

                SqlDataReader read = run_query.ExecuteReader();

                while (read.Read())
                {
                    enrolments.Add(ReadEnrolment(read));
                }

                read.Close();
                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            errorMessage = error.Message;
        }//end of catch

        return enrolments;
    }//end of GetEnrolmentsByUser


    public List<Enrolments> GetEnrolmentsByEvent(int eventId)
    {//start of GetEnrolmentsByEvent
        List<Enrolments> enrolments = new List<Enrolments>();

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                SqlCommand run_query = new SqlCommand(selectEnrolment + "WHERE en.event_id=@eventId ORDER BY en.enrolment_date", connect);
                run_query.Parameters.AddWithValue("@eventId", eventId);

                SqlDataReader read = run_query.ExecuteReader();

                while (read.Read())
                {
                    enrolments.Add(ReadEnrolment(read));
                }

                read.Close();
                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            errorMessage = error.Message;
        }//end of catch

        return enrolments;
    }//end of GetEnrolmentsByEvent


    public string UpdateEnrolmentStatus(int enrolmentId, string status)
    {//start of UpdateEnrolmentStatus
        string result = "Failed to update the enrolment";

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                SqlCommand run_query = new SqlCommand("UPDATE EventEnrolment SET status=@status WHERE enrolment_id=@id", connect);
                run_query.Parameters.AddWithValue("@status", status);
                run_query.Parameters.AddWithValue("@id", enrolmentId);

                run_query.ExecuteNonQuery();
                result = "Enrolment status updated successfully...";

                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            result = error.Message;
        }//end of catch

        return result;
    }//end of UpdateEnrolmentStatus


    //turns the current row of the reader into an Enrolments object
    private Enrolments ReadEnrolment(SqlDataReader read)
    {
        Enrolments enrolment = new Enrolments();
        enrolment.EnrolmentId = Convert.ToInt32(read["enrolment_id"]);
        enrolment.EnrolmentDate = Convert.ToDateTime(read["enrolment_date"]);
        enrolment.Status = read["status"].ToString();
        enrolment.UserId = Convert.ToInt32(read["user_id"]);
        enrolment.ParticipantName = read["participant_name"].ToString();
        enrolment.EventId = Convert.ToInt32(read["event_id"]);
        enrolment.EventName = read["event_name"].ToString();
        enrolment.OrganiserId = Convert.ToInt32(read["organiser_id"]);
        enrolment.CategoryId = Convert.ToInt32(read["category_id"]);
        enrolment.CategoryName = read["category_name"].ToString();
        return enrolment;
    }

}//end of class