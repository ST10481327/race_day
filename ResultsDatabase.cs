using System.Data.SqlClient;
using RaceDay_poe_part2.Models;

namespace RaceDay_poe_part2.backend;

public class ResultDatabase
{

    //filled when a read method has an error
    public string errorMessage = "";

    string connectionString = @"Data source=(localdb)\race_day;Database=race_day;";

    string selectResult = "SELECT r.result_id, r.enrolment_id, en.event_id, e.event_name, " +
                          "u.FirstName + ' ' + u.LastName AS participant_name, " +
                          "c.category_name, r.finish_time, r.position, r.notes " +
                          "FROM Result r " +
                          "INNER JOIN EventEnrolment en ON r.enrolment_id = en.enrolment_id " +
                          "INNER JOIN [Event] e ON en.event_id = e.event_id " +
                          "INNER JOIN users u ON en.user_id = u.UserId " +
                          "INNER JOIN EventCategory c ON en.category_id = c.category_id ";

    //finishTime is text like 00:48:32
    public string CreateResult(int enrolmentId, string finishTime, int position, string notes)
    {//start of CreateResult
        string result = "Failed to save the result";

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                string query = "INSERT INTO Result (finish_time, position, notes, enrolment_id) " +
                               "VALUES (@time, @position, @notes, @enrolmentId)";

                SqlCommand run_query = new SqlCommand(query, connect);
                run_query.Parameters.AddWithValue("@time", finishTime);
                run_query.Parameters.AddWithValue("@position", position);
                run_query.Parameters.AddWithValue("@notes", notes);
                run_query.Parameters.AddWithValue("@enrolmentId", enrolmentId);

                run_query.ExecuteNonQuery();
                result = "Result saved successfully...";

                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            result = error.Message;
        }//end of catch

        return result;
    }//end of CreateResult


    public List<Results> GetResultsByUser(int userId)
    {//start of GetResultsByUser
        List<Results> results = new List<Results>();

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                SqlCommand run_query = new SqlCommand(selectResult + "WHERE en.user_id=@userId ORDER BY e.event_date", connect);
                run_query.Parameters.AddWithValue("@userId", userId);

                SqlDataReader read = run_query.ExecuteReader();

                while (read.Read())
                {
                    results.Add(ReadResult(read));
                }

                read.Close();
                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            errorMessage = error.Message;
        }//end of catch

        return results;
    }//end of GetResultsByUser


    public List<Results> GetResultsByEvent(int eventId)
    {//start of GetResultsByEvent
        List<Results> results = new List<Results>();

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                SqlCommand run_query = new SqlCommand(selectResult + "WHERE en.event_id=@eventId ORDER BY r.finish_time", connect);
                run_query.Parameters.AddWithValue("@eventId", eventId);

                SqlDataReader read = run_query.ExecuteReader();

                while (read.Read())
                {
                    results.Add(ReadResult(read));
                }

                read.Close();
                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            errorMessage = error.Message;
        }//end of catch

        return results;
    }//end of GetResultsByEvent


    //turns the current row of the reader into a Results object
    private Results ReadResult(SqlDataReader read)
    {
        Results result = new Results();
        result.ResultId = Convert.ToInt32(read["result_id"]);
        result.EnrolmentId = Convert.ToInt32(read["enrolment_id"]);
        result.EventId = Convert.ToInt32(read["event_id"]);
        result.EventName = read["event_name"].ToString();
        result.ParticipantName = read["participant_name"].ToString();
        result.CategoryName = read["category_name"].ToString();
        result.FinishTime = read["finish_time"].ToString();
        result.Position = Convert.ToInt32(read["position"]);
        result.Notes = read["notes"].ToString();
        return result;
    }

}//end of class