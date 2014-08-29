using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using QA_System.Models;

namespace QA_System.DataProvider
{
    public class UserProvider : DataBase
    {
        public List<UserModels> GetAllUser()
        {
            List<UserModels> userList = new List<UserModels>();
            using (SqlConnection connection = new SqlConnection(base.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetAllUserInfo";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        UserModels user = new UserModels();
                        user.UserID = reader[0].ToString();
                        user.UserName = reader[1].ToString();
                        userList.Add(user);
                    }
                }
            }
            return userList;
        }

        public bool GetUserInfo(string userID, out string userName)
        {
            userName = string.Empty;
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetUserInfo";

                    cmd.Parameters.AddWithValue("@userID", userID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        userName = reader.GetString(1);
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
        }
    }
}