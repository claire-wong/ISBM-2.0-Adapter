/* Purpose: SQL Server Client class to handle basic CRUD on ISBM adapter
 *          database. This project is using SQL Express 2017 at the  
 *          time of development.
 *          
 * Author: Claire Wong
 * Date Created:  2020/04/29
 * 
 * (c) 2020
 * This code is licensed under MIT license
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ISBM_Adapter.Processes
{
    public class DatabaseHandler
    {
        //Open database connection
        public SqlConnection OpenSQLConnection()
        {
            //Get SQL Server connection string from Web.Config
            string connectionString = ConfigurationManager.ConnectionStrings["ISBMAdapter"].ConnectionString;
            SqlConnection cnn = new SqlConnection(connectionString);
            cnn.Open();
            return cnn;
        }

        //Read requested data records 
        public DataSet Select(string sqlStatement)
        {
            //Create SqlConnection with open connection to database
            SqlConnection cnn = OpenSQLConnection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            DataSet returnData = new DataSet();
            dataAdapter.SelectCommand = new SqlCommand(sqlStatement, cnn);
            dataAdapter.Fill(returnData, "Return Data");
            //Done with the database connection
            cnn.Close();                         

            return returnData;
        }
        
        //Create data record
        public void Insert(string sqlStatement)
        {
            //Create SqlConnection with open connection to database
            SqlConnection cnn = OpenSQLConnection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            dataAdapter.InsertCommand = new SqlCommand(sqlStatement, cnn);
            dataAdapter.InsertCommand.ExecuteNonQuery();
            //Done with the database connection
            cnn.Close();
        }

        //Update data records
        public void Update(string sqlStatement)
        {
            //Create SqlConnection with open connection to database
            SqlConnection cnn = OpenSQLConnection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            dataAdapter.UpdateCommand = new SqlCommand(sqlStatement, cnn);
            dataAdapter.UpdateCommand.ExecuteNonQuery();
            //Done with the database connection
            cnn.Close();
        }

        //Delete data records
        public void Delete(string sqlStatement)
        {
            //Create SqlConnection with open connection to database
            SqlConnection cnn = OpenSQLConnection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            dataAdapter.DeleteCommand = new SqlCommand(sqlStatement, cnn);
            dataAdapter.DeleteCommand.ExecuteNonQuery();
            //Done with the database connection
            cnn.Close();
        }
    }
}