using System.Data.SqlClient;

namespace RaceDay_poe_part2.backend;

public class AuthDatabase
{

    string message = "Failed to register the participant";

    string connectionString = @"Data source=(localdb)\race_day;Database=race_day;";

    public string RegisterUserInfo(string FirstName, string LastName, int age, string Email, string passwordHashed, string UserRole)
    {//start of RegisterInfo

        try
        {//start of try

            //using the using method
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using 

                string createdAt = DateTime.Now.ToString();

                //creating a query to insert user info into the database
                string query = "INSERT INTO users VALUES('" + FirstName + "','" + LastName + "','" + age + "','" + Email + "','" + passwordHashed + "','" + createdAt + "','" + UserRole + "')";

                //open connection
                connect.Open();

                //sqlcommand to run the query
                SqlCommand run_query = new SqlCommand(query, connect);


                int result = run_query.ExecuteNonQuery();

                if (result > 0)
                {
                    //messager to the user
                    message = " Thank you " + FirstName + " you, have been registered successfully";
                }
                else
                {
                    if (UserRole == null)
                    {
                        return message;
                    }
                }

                if (UserRole == "Organizer")
                {
                    string query2 = "INSERT INTO Organizers (User_id) VALUES((SELECT UserId FROM users WHERE Email='" + Email + "'))";

                    SqlCommand storeId = new SqlCommand(query2, connect);

                    storeId.ExecuteNonQuery();


                }
                else if (UserRole == "Participant")
                {

                    string query3 = "INSERT INTO Participants (User_id) VALUES((SELECT UserId FROM users WHERE Email='" + Email + "'))";

                    SqlCommand store_Id = new SqlCommand(query3, connect);

                    store_Id.ExecuteNonQuery();


                }

                //close connection
                connect.Close();

            }//end of using

        }//end if try
        catch (Exception error)
        { //start of catch

            //assign error message
            message = error.Message;

        }//end of catch


        return message;

    }//end of ResgisterInfo





    public string LogInInfo(string Email, string passwordHashed)
    {

        string message = string.Empty;

        try
        {
            //sqlconnection to connect to the database
            using (SqlConnection connector = new SqlConnection(connectionString))
            {
                int countUser = 0;

                //open connection
                connector.Open();

                //Sql query to select details from the database
                string query = "SELECT  Email,passwordHashed FROM users WHERE Email='" + Email + "';";

                SqlCommand run_query = new SqlCommand(query, connector);


                SqlDataReader read = run_query.ExecuteReader();



                if (read != null)
                {

                    while (read.Read())
                    {

                        countUser = 1;
                        string storedHash = read["passwordHashed"].ToString();

                        if (BCrypt.Net.BCrypt.Verify(passwordHashed, storedHash))
                        {
                            message = "Logged in successfully...";

                        }
                        else
                        {
                            message = "Login Unsucessful...";
                        }

                    }
                    //verify the typed password against the stored hash

                }
                //close connection
                connector.Close();
            }
        }
        catch (Exception error)
        {
            message = error.Message;
        }
        return message;

    }//end of login method


}//end of class
//end of method

