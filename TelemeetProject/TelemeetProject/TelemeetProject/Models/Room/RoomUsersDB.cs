using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TelemeetProject.Hubs;
using TelemeetProject.Models.Room;

namespace TelemeetProject.Models
{
    public class RoomUsersDB : IRoomUserDB
    {
        private string strcon = "";
        private readonly IHubContext<RoomHub> context;

        public RoomUsersDB(IConfiguration configuration, IHubContext<RoomHub> hubContext)
        {
            strcon = configuration.GetConnectionString("DefaultConnection");
            context = hubContext;
            SqlDependency.Start(strcon);
        }

        public List<string> getUsersInRoom(string roomid)
        {
            List<string> result = new List<string>();
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT user_email FROM dbo.RoomsUsers WHERE room_id='" + roomid + "';", con);
                SqlDependency dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(userChangeNotification);

                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    result.Add(reader["user_email"].ToString());
                }
                
                con.Close();
            }
            catch (Exception e)
            {

            }
            return result;
        }

        private void userChangeNotification(object sender, SqlNotificationEventArgs e)
        {
            context.Clients.All.SendAsync("refreshUsers");
        }

        //Join ROOM-------------------------------------------------------------------------
        public void insertRoomUsers(string roomid, string userEmail)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("INSERT INTO RoomsUsers" +
                    "(room_id, user_email) VALUES " +
                    "(@createRoomId, @user_email)", con);

                cmd.Parameters.AddWithValue("@createRoomId", roomid);
                cmd.Parameters.AddWithValue("@user_email", userEmail);

                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {

            }
        }

        //Leave Room--------------------------------------------------------------------------
        public void deleteRoomUser(string userEmail)
        {
            SqlConnection con = new SqlConnection(strcon);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            SqlCommand cmd = new SqlCommand("DELETE FROM RoomsUsers WHERE user_email = '" + userEmail + "'", con);

            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
