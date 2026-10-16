using System.Data.SqlClient;
using RaceDay_poe_part2.Models;

namespace RaceDay_poe_part2.backend;

public class CategoryDatabase
{

    //filled when a read method has an error
    public string errorMessage = "";

    string connectionString = @"Data source=(localdb)\race_day;Database=race_day;";

    string selectCategory = "SELECT category_id, category_name, description, event_id FROM EventCategory ";

    public List<Category> GetCategoriesByEvent(int eventId)
    {//start of GetCategoriesByEvent
        List<Category> categories = new List<Category>();

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                SqlCommand run_query = new SqlCommand(selectCategory + "WHERE event_id=@eventId ORDER BY category_name", connect);
                run_query.Parameters.AddWithValue("@eventId", eventId);

                SqlDataReader read = run_query.ExecuteReader();

                while (read.Read())
                {
                    categories.Add(ReadCategory(read));
                }

                read.Close();
                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            errorMessage = error.Message;
        }//end of catch

        return categories;
    }//end of GetCategoriesByEvent


    public Category GetCategoryById(int categoryId)
    {//start of GetCategoryById
        Category category = null;

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                SqlCommand run_query = new SqlCommand(selectCategory + "WHERE category_id=@id", connect);
                run_query.Parameters.AddWithValue("@id", categoryId);

                SqlDataReader read = run_query.ExecuteReader();

                if (read.Read())
                {
                    category = ReadCategory(read);
                }

                read.Close();
                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            errorMessage = error.Message;
        }//end of catch

        return category;
    }//end of GetCategoryById


    public string CreateCategory(int eventId, string categoryName, string description)
    {//start of CreateCategory
        string result = "Failed to create the category";

        try
        {//start of try
            using (SqlConnection connect = new SqlConnection(connectionString))
            {//start of using
                connect.Open();

                string query = "INSERT INTO EventCategory (category_name, description, event_id) " +
                               "VALUES (@name, @description, @eventId)";

                SqlCommand run_query = new SqlCommand(query, connect);
                run_query.Parameters.AddWithValue("@name", categoryName);
                run_query.Parameters.AddWithValue("@description", description);
                run_query.Parameters.AddWithValue("@eventId", eventId);

                run_query.ExecuteNonQuery();
                result = "Category created successfully...";

                connect.Close();
            }//end of using
        }//end of try
        catch (Exception error)
        {//start of catch
            result = error.Message;
        }//end of catch

        return result;
    }//end of CreateCategory


    //turns the current row of the reader into a Category object
    private Category ReadCategory(SqlDataReader read)
    {
        Category category = new Category();
        category.CategoryId = Convert.ToInt32(read["category_id"]);
        category.CategoryName = read["category_name"].ToString();
        category.Description = read["description"].ToString();
        category.EventId = Convert.ToInt32(read["event_id"]);
        return category;
    }

}//end of class