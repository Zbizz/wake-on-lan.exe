using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace WakeOnLan.exe
{
    public static class Machines
    {
        public static List<string> GetMacs(string table, out bool success)
        {
            List<string> macs = new List<string>();

            success = true;

            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT MAC FROM " + table);
                    cmd.Connection = conn;

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            macs.Add(reader["MAC"].ToString());
                        }
                    }
                }
            }
            catch
            {
                success = false;
            }

            return macs;
        }
    }
}
