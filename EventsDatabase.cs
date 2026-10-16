using System.Data.SqlClient;
using RaceDay_poe_part2.Models;

namespace RaceDay_poe_part2.backend;

public class EventDatabase
{

    //filled when a read method has an error
    public string errorMessage = "";

    string connectionString = @"Data source=(localdb)\race_day;Database=race_day;";

    string selectEvent = "SELECT e.event_id, e.event_name, e.description, e.event_date, e.location, e.distance_km, " +
                         "e.event_type, e.max_participants, e.status, e.organiser_id, " +
                         "u.FirstName + ' ' + u.LastName AS organiser_name " +
                         "FROM [Event] e INNER JOIN users u ON e.organiser_id = u.UserId ";

    public List<Events> GetAllEvents()
    {//start of GetAllEvents
        List<Events> events = new List<Events>();

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                SqlCommand run_query = new SqlCommand(selectEvent + "ORDER BY e.event_date", connect);

                SqlDataReader read = run_query.ExecuteReader();

                while (read.Read())
                {
                    events.Add(ReadEvent(read));
                }

                read.Close();
                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            errorMessage = error.Message;
        }//end of catch

        return events;
    }//end of GetAllEvents


    public Events GetEventById(int eventId)
    {//start of GetEventById
        Events ev = null;

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                SqlCommand run_query = new SqlCommand(selectEvent + "WHERE e.event_id=@id", connect);
                run_query.Parameters.AddWithValue("@id", eventId);

                SqlDataReader read = run_query.ExecuteReader();

                if (read.Read())
                {
                    ev = ReadEvent(read);
                }

                read.Close();
                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            errorMessage = error.Message;
        }//end of catch

        return ev;
    }//end of GetEventById


    public string CreateEvent(Events ev, int organiserId)
    {//start of CreateEvent
        string result = "Failed to create the event";

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                string query = "INSERT INTO [Event] (event_name, description, event_date, location, distance_km, event_type, max_participants, status, organiser_id) " +
                               "VALUES (@name, @description, @date, @location, @distance, @type, @max, @status, @organiser)";

                SqlCommand run_query = new SqlCommand(query, connect);
                run_query.Parameters.AddWithValue("@name", ev.EventName);
                run_query.Parameters.AddWithValue("@description", ev.Description);
                run_query.Parameters.AddWithValue("@date", ev.EventDate.Date);
                run_query.Parameters.AddWithValue("@location", ev.Location);
                run_query.Parameters.AddWithValue("@distance", ev.DistanceKm);
                run_query.Parameters.AddWithValue("@type", ev.EventType);
                run_query.Parameters.AddWithValue("@max", ev.MaxParticipants);
                run_query.Parameters.AddWithValue("@status", ev.Status);
                run_query.Parameters.AddWithValue("@organiser", organiserId);

                run_query.ExecuteNonQuery();
                result = "Event created successfully...";

                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            result = error.Message;
        }//end of catch

        return result;
    }//end of CreateEvent


    public string UpdateEvent(int eventId, Events ev)
    {//start of UpdateEvent
        string result = "Failed to update the event";

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                string query = "UPDATE [Event] SET event_name=@name, description=@description, event_date=@date, " +
                               "location=@location, distance_km=@distance, event_type=@type, " +
                               "max_participants=@max, status=@status WHERE event_id=@id";

                SqlCommand run_query = new SqlCommand(query, connect);
                run_query.Parameters.AddWithValue("@name", ev.EventName);
                run_query.Parameters.AddWithValue("@description", ev.Description);
                run_query.Parameters.AddWithValue("@date", ev.EventDate.Date);
                run_query.Parameters.AddWithValue("@location", ev.Location);
                run_query.Parameters.AddWithValue("@distance", ev.DistanceKm);
                run_query.Parameters.AddWithValue("@type", ev.EventType);
                run_query.Parameters.AddWithValue("@max", ev.MaxParticipants);
                run_query.Parameters.AddWithValue("@status", ev.Status);
                run_query.Parameters.AddWithValue("@id", eventId);

                run_query.ExecuteNonQuery();
                result = "Event updated successfully...";

                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            result = error.Message;
        }//end of catch

        return result;
    }//end of UpdateEvent


    public string DeleteEvent(int eventId)
    {//start of DeleteEvent
        string result = "Failed to delete the event";

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                //enrolments go first (their results are deleted automatically), then the event
                string query = "DELETE FROM EventEnrolment WHERE event_id=@id; " +
                               "DELETE FROM [Event] WHERE event_id=@id;";

                SqlCommand run_query = new SqlCommand(query, connect);
                run_query.Parameters.AddWithValue("@id", eventId);

                run_query.ExecuteNonQuery();
                result = "Event deleted successfully...";

                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            result = error.Message;
        }//end of catch

        return result;
    }//end of DeleteEvent


    //turns the current row of the reader into an Events object
    private Events ReadEvent(SqlDataReader read)
    {
        Events ev = new Events();
        ev.EventId = Convert.ToInt32(read["event_id"]);
        ev.EventName = read["event_name"].ToString();
        ev.Description = read["description"].ToString();
        ev.EventDate = Convert.ToDateTime(read["event_date"]);
        ev.Location = read["location"].ToString();
        ev.DistanceKm = Convert.ToDecimal(read["distance_km"]);
        ev.EventType = read["event_type"].ToString();
        ev.MaxParticipants = Convert.ToInt32(read["max_participants"]);
        ev.Status = read["status"].ToString();
        ev.OrganiserId = Convert.ToInt32(read["organiser_id"]);
        ev.OrganiserName = read["organiser_name"].ToString();
        return ev;
    }

}//end of class