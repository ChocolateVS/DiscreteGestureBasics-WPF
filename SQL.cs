using System.Collections.Generic;
using System.Data.SqlClient;
using System;


namespace compx241GroupProject
{
    class SQL
    {
        //generates the connection to the database       
        //Make sure that in the Database connection you put your Database connection here:
        public static SqlConnection con = new SqlConnection(@"Server=.\SQLEXPRESS; Database=timetable; Trusted_Connection = True;");
        public static SqlCommand cmd = new SqlCommand();
        public static SqlDataReader read;

        /// <summary>
        /// This excecutres the query, used mainly for 
        /// insert/delete/update statements etc. where we don't need
        /// to read from what we are doing.
        /// </summary>
        /// <param name="query"></param>
        public static void executeQuery(string query)
        {
            //try catch to catch any unforseen errors gracefully
            try
            {
                con.Close();
                cmd.Connection = con;
                con.Open();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
            catch
            {
                //put a message box in here if you are recieving errors and see if you can find out why?
                return;
            }
        }

    /// <summary>
    /// Generates an SQL query based on the input
    /// query e.g. "SELECT * FROM staff"
    /// </summary>
    /// <param name="query"></param>
    public static void selectQuery(string query)
        {
            con.Close();
            cmd.Connection = con;
            con.Open();
            cmd.CommandText = query;
            read = cmd.ExecuteReader();
        }
    }
}
