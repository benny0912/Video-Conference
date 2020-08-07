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
    public class UserDB : IUserDB
    {
        private string strcon = "";
        public UserDB(IConfiguration configuration)
        {
            strcon = configuration.GetConnectionString("DefaultConnection");
        }
        public User.Respond loginUser(User user)
        {
            bool err = true;
            string msg = "";
            if (checkUserExist(user))
            {
                if (checkUserWithPassword(user))
                {
                    msg = "Successfully logged in.";
                    err = false;
                }
                else
                {
                    msg = "Incorrect password.";
                }
            }
            else
            {
                msg = "There is no user register with corresponding email.";
            }

            return new User.Respond
            {
                error = err,
                message = msg
            };
        }

        public User getUser(User user)
        {
            if (checkUserExist(user))
            {
                return getUser(user.user_email);
            }
            else
            {
                return null;
            }
        }

        public User.Respond createUser(User user)
        {
            bool err = true;
            string msg = "";

            if (checkUserExist(user))
            {
                msg = "There is existing user with this email. Please use another email to register.";
            }
            else
            {
                if (signUpNewUser(user))
                {
                    err = false;
                    msg = "Successfully registered.";

                }
                else
                {
                    msg = "Failed to register.";
                }
            }

            return new User.Respond
            {
                error = err,
                message = msg
            };
        }



        private bool signUpNewUser(User user)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("INSERT INTO Users" +
                    "(user_email, user_password, user_first_name, user_last_name, user_role) VALUES " +
                    "(@user_email, @user_password, @user_first_name, @user_last_name, @user_role)", con);

                cmd.Parameters.AddWithValue("@user_email", user.user_email);
                cmd.Parameters.AddWithValue("@user_password", BCrypt.Net.BCrypt.HashPassword(user.user_password));
                cmd.Parameters.AddWithValue("@user_first_name", user.user_first_name);
                cmd.Parameters.AddWithValue("@user_last_name", user.user_last_name);
                cmd.Parameters.AddWithValue("@user_role", "user");
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public User getUser(string userEmail)
        {
            User result = null;
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE user_email='" + userEmail + "';", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    result = new User();
                    result.user_email = dt.Rows[0]["user_email"].ToString();
                    result.user_password = dt.Rows[0]["user_password"].ToString();
                    result.user_first_name = dt.Rows[0]["user_first_name"].ToString();
                    result.user_last_name = dt.Rows[0]["user_last_name"].ToString();
                    result.user_role = dt.Rows[0]["user_role"].ToString();
                    result.last_room = dt.Rows[0]["last_room"].ToString();
                    result.signed_in = (DateTime)dt.Rows[0]["signed_in"];
                    result.user_image = dt.Rows[0]["user_image"].ToString();
                    result.image_created = (DateTime)dt.Rows[0]["image_created"];
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool checkUserWithPassword(User user)
        {
            bool result = false;
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE user_email='" + user.user_email + "';", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    string pass = dt.Rows[0]["user_password"].ToString();
                    if (BCrypt.Net.BCrypt.Verify(user.user_password, pass))
                    {
                        result = true;
                    }
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool checkUserExist(User user)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE user_email='" + user.user_email + "';", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void updateUser(User user)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("UPDATE dbo.Users SET user_first_name=@user_first_name, user_last_name=@user_last_name, user_password=@user_password" +
                    " WHERE user_email=@user_email_old;", con);
                cmd.Parameters.AddWithValue("@user_password", BCrypt.Net.BCrypt.HashPassword(user.user_password));
                cmd.Parameters.AddWithValue("@user_first_name", user.user_first_name);
                cmd.Parameters.AddWithValue("@user_last_name", user.user_last_name);
                cmd.Parameters.AddWithValue("@user_email_old", user.user_email);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception)
            {

            }
        }

        public void updateUserWithEmail(User user, string oldEmail)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("UPDATE dbo.Users SET user_email=@user_email, user_first_name=@user_first_name, user_last_name=@user_last_name, user_password=@user_password" +
                    " WHERE user_email=@user_email_old;", con);
                cmd.Parameters.AddWithValue("@user_email", user.user_email);
                cmd.Parameters.AddWithValue("@user_password", BCrypt.Net.BCrypt.HashPassword(user.user_password));
                cmd.Parameters.AddWithValue("@user_first_name", user.user_first_name);
                cmd.Parameters.AddWithValue("@user_last_name", user.user_last_name);
                cmd.Parameters.AddWithValue("@user_email_old", oldEmail);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception)
            {

            }
        }

        public void updateUserSignIn(string userEmail)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("UPDATE dbo.Users SET signed_in=@signed_in WHERE user_email=@user_email;", con);
                cmd.Parameters.AddWithValue("@user_email", userEmail);
                cmd.Parameters.AddWithValue("@signed_in", DateTime.Now);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception)
            {

            }
        }

        public void updateUserLastRoom(string userEmail, string roomName)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("UPDATE dbo.Users SET last_room=@last_room WHERE user_email=@user_email;", con);
                cmd.Parameters.AddWithValue("@user_email", userEmail);
                cmd.Parameters.AddWithValue("@last_room", roomName);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception)
            {

            }
        }

        public void updateUserImg(string userEmail, string imageUrl)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("UPDATE dbo.Users SET user_image=@user_image, image_created=@image_created WHERE user_email=@user_email;", con);
                cmd.Parameters.AddWithValue("@user_email", userEmail);
                cmd.Parameters.AddWithValue("@user_image", imageUrl);
                cmd.Parameters.AddWithValue("@image_created", DateTime.Now);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception)
            {

            }
        }

        public List<User> getUsers()
        {
            List<User> result = new List<User>();
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Users;", con);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    User temp = new User();
                    temp.user_email = reader["user_email"].ToString();
                    temp.user_password = reader["user_password"].ToString();
                    temp.user_first_name = reader["user_first_name"].ToString();
                    temp.user_last_name = reader["user_last_name"].ToString();
                    temp.date_created = (DateTime) reader["date_created"];
                    temp.signed_in = (DateTime) reader["signed_in"];
                    temp.last_room = reader["last_room"].ToString();
                    temp.user_role = reader["user_role"].ToString();
                    temp.user_image = reader["user_image"].ToString();
                    temp.image_created = (DateTime) reader["image_created"];
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
