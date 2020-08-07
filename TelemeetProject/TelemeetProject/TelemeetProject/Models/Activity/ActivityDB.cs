using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace TelemeetProject.Models
{
    public class ActivityDB : IActivityDB
    {
        private string strcon = "";

        public ActivityDB(IConfiguration configuration)
        {
            strcon = configuration.GetConnectionString("DefaultConnection");
        }
        public void logActivity(Activity activity)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("INSERT INTO ActivityLogs" +
                    "(user_email, user_first_name, user_last_name, activity_type, activity_details) VALUES " +
                    "(@user_email, @user_first_name, @user_last_name, @activity_type, @activity_details)", con);

                cmd.Parameters.AddWithValue("@user_email", activity.user_email);
                cmd.Parameters.AddWithValue("@user_first_name", activity.user_first_name);
                cmd.Parameters.AddWithValue("@user_last_name", activity.user_last_name);
                cmd.Parameters.AddWithValue("@activity_type", activity.activity_type);
                cmd.Parameters.AddWithValue("@activity_details", activity.activity_details);
                cmd.ExecuteNonQuery();
                con.Close();
            } catch (Exception e)
            {

            }
        }

        public List<Activity> GetActivities()
        {
            List<Activity> result = new List<Activity>();
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.ActivityLogs;", con);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Activity temp = new Activity();
                    temp.user_email = reader["user_email"].ToString();
                    temp.user_first_name = reader["user_first_name"].ToString();
                    temp.user_last_name = reader["user_last_name"].ToString();
                    temp.activity_type = reader["activity_type"].ToString();
                    temp.activity_details = reader["activity_details"].ToString();
                    temp.last_activity = (DateTime)reader["last_activity"];
                    result.Add(temp);
                }
                con.Close();
            }
            catch (Exception e)
            {

            }
            return result;
        }

    }
}
