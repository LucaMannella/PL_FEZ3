using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Threading;

namespace Server
{
    // ciao!
    class Database
    {
        MySqlConnection myConn;
        bool connection_Opened = false;

        public void OpenConnect()
        {
            string myConnectionString = "User Id=root;Host=localhost;Database=PL_FEZ03";
            myConn = new MySqlConnection(myConnectionString);
            myConn.Open();
            connection_Opened = true;
        }

        public void CloseConnect()
        {
            myConn.Close();
            connection_Opened = false;
        }

        public int ExecuteNonQuery(string query)
        {
            MySqlCommand myCommand = new MySqlCommand(query, myConn);
            return myCommand.ExecuteNonQuery();
        }

        public int ExecuteNonQuery(string query, byte[] bytes)
        {
            //query = query.Replace("\\", "\\\\");
            MySqlCommand myCommand = new MySqlCommand(query, myConn);
            myCommand.Parameters.Add("@hash_code", bytes);
            return myCommand.ExecuteNonQuery();
        }

        public int ExecuteNonQuery(string query, DateTime d)
        {
            //query = query.Replace("\\", "\\\\");
            MySqlCommand myCommand = new MySqlCommand(query, myConn);
            myCommand.Parameters.Add("@data", d);
            return myCommand.ExecuteNonQuery();
        }

        public int ExecuteNonQuery(string query, DateTime d, MySqlTransaction tr)
        {
            //query = query.Replace("\\", "\\\\");
            MySqlCommand myCommand = new MySqlCommand(query, myConn);
            myCommand.Parameters.Add("@data", d);
            myCommand.Transaction = tr;
            return myCommand.ExecuteNonQuery();
        }

        public int ExecuteNonQuery(string query, MySqlTransaction tr)
        {
            //query = query.Replace("\\", "\\\\");
            MySqlCommand myCommand = new MySqlCommand(query, myConn);
            myCommand.Transaction = tr;
            return myCommand.ExecuteNonQuery();
        }
        public int ExecuteNonQuery(string query, byte[] bytes, MySqlTransaction tr)
        {
            //query = query.Replace("\\", "\\\\");
            MySqlCommand myCommand = new MySqlCommand(query, myConn);
            myCommand.Parameters.Add("@hash_code", bytes);
            myCommand.Transaction = tr;
            return myCommand.ExecuteNonQuery();
        }

        public bool ConnectionOpened
        {
            set { connection_Opened = value; }
            get { return connection_Opened; }
        }


        public MySqlTransaction getTransaction()
        {
            MySqlTransaction t;
            t = myConn.BeginTransaction();
            return t;
        }

        public DataRowCollection GetRowsWhithQuery(string query, string table)
        {
            lock (this)
            {
                DataTable dt;
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, myConn))
                {
                    using (DataSet ds = new DataSet())
                    {
                        da.Fill(ds, table);
                        dt = ds.Tables[table];
                    }
                }
                return dt.Rows;
            }
        }

        public string validate(string s)
        {
            return MySqlHelper.EscapeString(s);
        }


        /**
         * This method allows to insert a new name inside the test table
         * of the PL_FEZ03 database.
         * 
         * @param name - The name that you want to insert
         * @returns True if the name was inserted, false otherwise.
         */
        public bool insertName(String name)
        {
            if (connection_Opened)
            {
                String query = "INSERT INTO test(Name) VALUES('" + name + "');";
                int res = ExecuteNonQuery(query);

                if (res < 0)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
    }
}
