using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Threading;

namespace Server
{
    public class Database
    {
        MySqlConnection myConn = null;
        bool connection_Opened = false;
        private static Database myInstance = null;

        private Database() {    /* empty default private constructor */    }

        public static Database getInstance()
        {
            if (myInstance == null)
                myInstance = new Database();

            return myInstance;
        }


        public bool OpenConnect()
        {
            string myConnectionString = "User Id=root;Host=localhost;Database=PL_FEZ03";

            try {
                myConn = new MySqlConnection(myConnectionString);
                myConn.Open();
            }
            catch (Exception ex) {
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
         * of the database.
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
         * the database. A MAC address and a Port number are required.
         * 
         * @param MACAddress - The MAC address of the client.
         * @param port - The port that is used by the server for that connection.
         * @returns True if the entry was succesfully inserted, false otherwise.
         */
        public bool insertClient(String MACAddress, int port)
        {
            int res = -1;

            if (!connection_Opened) {
                if (OpenConnect() == false)
                    return false;
            }

            String query = "SELECT * FROM clients WHERE MAC = '"+MACAddress+"';";
            DataRowCollection records = GetRowsWhithQuery(query, "clients");
            if (records.Count == 0) {
                query = "INSERT INTO clients(MAC, Port) VALUES('" + MACAddress + "', " + port + ");";
                res = ExecuteNonQuery(query);
            }
            else {
                query = "UPDATE clients SET Port="+port+" WHERE MAC='"+MACAddress+"';";
                res = ExecuteNonQuery(query);
            }

            if (res <= 0)
                return false;
            else
                return true;
        }

        /**
         * This method should be used to store the information about a 
         * suspicious picture snapped by the client inside the database.
         * 
         * @param MACAddress - The MAC address of the client.
         * @param timestamp - A long representation of the moment of the snap.
         * @param path - The path in which the picture was stored on the server.
         * @returns True if the entry was succesfully inserted, false otherwise.
         */
        public bool insertSuspiciousPicturePath(String MACAddress, long timestamp, String path)
        {
            if (!connection_Opened) {
                if (OpenConnect() == false)
                    return false;
            }

            String query = "INSERT INTO suspicious_pictures(MAC, Timestamp, Path) VALUES('"+MACAddress+"', "+timestamp+", '"+path+"');";
            int res = ExecuteNonQuery(query);
           
            if (res <= 0)
                return false;
            else
                return true;
        }

        /**
         * This method checks if exists a specified client inside the database
         * with the specified MACAddress.
         * 
         * @param MACAddress - The MAC address of the client.
         * @returns True if the client exists, false otherwise.
         */
        public bool existClient(String MACAddress) 
        {
            if (!connection_Opened) {
                if (OpenConnect() == false)
                    return false;
            }

            String query = "SELECT * FROM clients WHERE MAC = '"+MACAddress+"';";
            DataRowCollection records = GetRowsWhithQuery(query, "clients");
            if (records.Count <= 0)
                return false;
            else
                return true;
        }

        /**
         * This method checks if exists a specified client inside the database
         * with the specified MACAddress and the specified PIN.
         * 
         * @param MACAddress - The MAC address of the client.
         * @parama PIN - The PIN related to this client.
         * @returns True if the login is valid, false otherwise.
         */
        public bool isValidClient(String MACAddress, String PIN)
        {
            String hashedPIN = String.Empty;

            if (!connection_Opened) {
                if (OpenConnect() == false)
                    return false;
            }

            Byte[] bytePIN = Encoding.UTF8.GetBytes(PIN+"lms_fez03");
            Byte[] hashedArray = new SHA256Managed().ComputeHash(bytePIN);

            foreach (byte x in hashedArray)
                hashedPIN += String.Format("{0:x2}", x);
            
            String query = "SELECT * FROM customers WHERE MAC = '"+MACAddress+"' AND pin = '"+hashedPIN+"';";
            DataRowCollection records = GetRowsWhithQuery(query, "clients");
            if (records.Count <= 0)
                return false;
            else
                return true;
        }

        /**
         * This method returns the email address related to given MAC address.
         * 
         * @param MACAddress - The MAC address of the client.
         * @returns A string containing the MAC address.
         */
        public String getClientEmail(String MACAddress)
        {
            if (!connection_Opened) {
                if (OpenConnect() == false)
                    return null;
            }

            String query = "SELECT email FROM customers WHERE MAC = '"+MACAddress+"';";
            DataRowCollection records = GetRowsWhithQuery(query, "clients");
            if (records.Count <= 0)
                return null;
            else
                return records[0]["email"].ToString();
        }

        /**
         * This method allows to remove a connection with a client
         * from the database. It should be called when a device
         * will go correctly down.
         * 
         * @param MACAddress - The MAC address of the client.
         * @returns True if the entry was succesfully removed, false otherwise.
         */
        public bool removeClient(String MACAddress)
        {
            if (!connection_Opened) {
                if (OpenConnect() == false)
                    return false;
            }

            String query = "DELETE FROM clients WHERE MAC = '" + MACAddress + "';";
            int res = ExecuteNonQuery(query);

            if (res <= 0)
                return false;
            else
                return true;
        }

        /**
         * This method removes all the connection with the clients 
         * from the database.
         * 
         * @returns True if the entries were succesfully removed, false otherwise.
         */
        public bool removeAllClient()
        {
            if (!connection_Opened) {
                if (OpenConnect() == false)
                    return false;
            }

            String query = "DELETE FROM clients;";
            int res = ExecuteNonQuery(query);

            if (res <= 0)
                return false;
            else
                return true;
        }

        /**
         * This method allows to remove all the picture of a specified client
         * from the database.
         * 
         * @param MACAddress - The MAC address of the client.
         * @returns True if the entries were succesfully removed, false otherwise.
         */
        public bool removePictures(String MACAddress)
        {
            if (!connection_Opened) {
                if (OpenConnect() == false)
                    return false;
            }

            String query = "DELETE FROM suspicious_pictures WHERE MAC = '" + MACAddress + "';";
            int res = ExecuteNonQuery(query);

            if (res <= 0)
                return false;
            else
                return true;
        }

        /**
         * This method execute a non-query on the database (INSERT, UPDATE, DELETE).
         * It returns -1 in case of error.
         */
        public int ExecuteNonQuery(string query)
        {
            if (!connection_Opened) {
                if (OpenConnect() == false)
                    return -1;
            }

            int toRet;

            try {
                MySqlCommand myCommand = new MySqlCommand(query, myConn);
                toRet = myCommand.ExecuteNonQuery();
            }
            catch (Exception ex) {
                Console.WriteLine("Error: something wrong in query execution: " + ex.ToString());
                toRet = -1;
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


// from here methods actually not used
        /**
         * This method prevent some SQL injections
         */
        public string validate(string s)
        {
            return MySqlHelper.EscapeString(s);
        }

        /**
         * This method should be used to manage a transaction.
         */ 
        public MySqlTransaction getTransaction()
        {
            MySqlTransaction t;
            t = myConn.BeginTransaction();
            return t;
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
    }
}