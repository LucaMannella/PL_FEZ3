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
    class Database
    {
        MySqlConnection myConn;
        bool connection_Opened = false;

        public bool OpenConnect()
        {
            string myConnectionString = "User Id=root;Host=localhost;Database=PL_FEZ03";

            try
            {
                myConn = new MySqlConnection(myConnectionString);
                myConn.Open();
            }
            catch (Exception ex)
            {
                connection_Opened = false;
                Console.WriteLine("Error: Impossible to open the database: " + ex.ToString());
                return false;
            }

            connection_Opened = true;
            return true;
        }

        public void CloseConnect()
        {
            myConn.Close();
            connection_Opened = false;
        }

        public bool ConnectionOpened
        {
            set { connection_Opened = value; }
            get { return connection_Opened; }
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
            if (!connection_Opened) {
                if (OpenConnect() == false)
                    return false;
            }
                
            String query = "INSERT INTO test(Name) VALUES('" + name + "');";
            int res = ExecuteNonQuery(query);

            if (res < 0)
                return false;
            else
                return true;   
        }

        /**
         * This method allows to insert a new connection with a client inside
         * the "clients" table of the PL_FEZ03 database.
         * 
         * @param name - The name that you want to insert
         * @returns True if the name was inserted, false otherwise.
         */
        public bool insertClient(String MACAddress, int port)
        {
            if (!connection_Opened) {
                if (OpenConnect() == false)
                    return false;
            }

            int res = -1;

            String query = "SELECT * FROM clients WHERE MAC = '"+MACAddress+"';";
            DataRowCollection records = GetRowsWhithQuery(query, "clients");
            if (records.Count == 0)
            {
                query = "INSERT INTO clients(MAC, Port) VALUES('" + MACAddress + "', " + port + ");";
                res = ExecuteNonQuery(query);
            }
            else
            {
                query = "UPDATE clients SET Port="+port+" WHERE MAC='"+MACAddress+"';";
                res = ExecuteNonQuery(query);
            }

            if (res <= 0)
                return false;
            else
                return true;
        }

        /**
         * This method execute a non-query on the database (INSERT, UPDATE, REMOVE).
         * It returns -1 in case of error.
         */
        public int ExecuteNonQuery(string query)
        {
            if (!connection_Opened) {
                if (OpenConnect() == false)
                    return -1;
            }

            int toRet = -1;

            try
            {
                MySqlCommand myCommand = new MySqlCommand(query, myConn);
                toRet = myCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: something wrong in query execution: " + ex.ToString());
            }

            return toRet;
        }

        /**
         * This method should be used to query the database.
         */
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

        /**
         * This method prevent some SQL injections
         */
        public string validate(string s)
        {
            return MySqlHelper.EscapeString(s);
        }

        public MySqlTransaction getTransaction()
        {
            MySqlTransaction t;
            t = myConn.BeginTransaction();
            return t;
        }


// actually not used methods
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
    }
}
