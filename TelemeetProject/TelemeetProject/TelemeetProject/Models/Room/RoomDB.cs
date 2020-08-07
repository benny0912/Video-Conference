using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TelemeetProject.Models.Room;

namespace TelemeetProject.Models
{
    public class RoomDB : IRoomDB
    {
        private string strcon = "";
        public RoomDB(IConfiguration configuration)
        {
            strcon = configuration.GetConnectionString("DefaultConnection");
        }


        //CHECK ROOM EXIST-------------------------------------------------------------------
        public bool isRoomExists(string roomName)
        {
            bool result = false;
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM Rooms WHERE room_name='" + roomName + "';", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    result = true;
                }
                con.Close();
            }
            catch (Exception)
            {
                
            }
            return result;
        }
        //GET ROOM NAME----------------------------------------------------------------------   
        public string GetRoomId(string roomName)
        {
            string result = "";
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM Rooms WHERE room_name='" + roomName + "';", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    result = dt.Rows[0]["room_id"].ToString();
                }
                con.Close();
            }
            catch (Exception)
            {
            }
            return result;
        }


        //GET ROOM NAME----------------------------------------------------------------------   
        public string GetRoom(string roomId)
        {
            string result = "";
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM Rooms WHERE room_id='" + roomId + "';", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    result = dt.Rows[0]["room_name"].ToString();
                }
                con.Close();
            }
            catch (Exception)
            {
            }
            return result;
        }

        //CHECK ROOM PASSWORD-------------------------------------------------------------------
        public bool validateRoomWithPass(string roomName, string roomPass)
        {
            bool result = false;
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM Rooms WHERE room_name='" + roomName + "';", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    string pass = dt.Rows[0]["room_password"].ToString();
                    result = BCrypt.Net.BCrypt.Verify(roomPass, pass);
                }
                con.Close();
            }
            catch (Exception)
            {
            }
            return result;
        }

        //CREATE ROOM-------------------------------------------------------------------------
        public void createRoom(RoomModel room)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("INSERT INTO Rooms" +
                    "(room_id, room_name, room_password, user_email) VALUES " +
                    "(@createRoomId, @createRoomName, @createRoomPassword, @user_email)", con);

                cmd.Parameters.AddWithValue("@createRoomId", room.room_id);
                cmd.Parameters.AddWithValue("@createRoomName", room.room_name);
                cmd.Parameters.AddWithValue("@createRoomPassword", BCrypt.Net.BCrypt.HashPassword(room.room_password));
                cmd.Parameters.AddWithValue("@user_email", room.user_email);

                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {

            }
        }

        //DELETE Room --------------------------------------------------------------------------
        public void deleteRoom(string roomid)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("DELETE FROM Rooms WHERE room_id = '" + roomid + "'", con);

                cmd.ExecuteNonQuery();
                con.Close();

            } catch (Exception e)
            {

            }
        }

        public string getRoomPassword(string roomId)
        {
            string pass = "";
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM Rooms WHERE room_id='" + roomId + "';", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    pass = dt.Rows[0]["room_password"].ToString();
                    
                }
                con.Close();
            }
            catch (Exception)
            {
            }
            return pass;
        }

        //Check User Is Inside Room Function ---------------------------------------------------------------
        public bool isUserCreatedRoom(string userEmail)
        {
            bool result = false;
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM Rooms WHERE user_email='" + userEmail + "';", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    result = true;
                }
                con.Close();
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
