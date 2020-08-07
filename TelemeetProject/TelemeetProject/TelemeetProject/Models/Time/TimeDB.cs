using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace TelemeetProject.Models
{
    public class TimeDB : ITimeDB
    {
        private string strcon = "";
        
        public TimeDB(IConfiguration configuration)
        {
            strcon = configuration.GetConnectionString("DefaultConnection");
        }

        public BigInteger getTime()
        {
            BigInteger result = 10;
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.CameraTime;", con);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Time temp = new Time();
                    temp.capture_time = (BigInteger)reader["capture_time"];         
                }
                con.Close();
            }
            catch (Exception e)
            {

            }
            return result;
        }
        public void updateTime(string time)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("UPDATE dbo.CameraTIme SET capture_time=@capture_time WHERE time_id=1;", con);               
                cmd.Parameters.AddWithValue("@capture_time", Int32.Parse(time));                
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception)
            {

            }
        }
    }
}
