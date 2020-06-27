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
        public SqlConnection OpenSQLConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ISBMAdapter"].ConnectionString;
            SqlConnection cnn = new SqlConnection(connectionString);
            cnn.Open();
            return cnn;
        }
        public DataSet Select(string sqlStatement)
        {
            SqlConnection cnn = OpenSQLConnection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            DataSet returnData = new DataSet();
            dataAdapter.SelectCommand = new SqlCommand(sqlStatement, cnn);
            dataAdapter.Fill(returnData, "Return Data");
            cnn.Close();

            return returnData;
        }
        
        public void Insert(string sqlStatement)
        {
            SqlConnection cnn = OpenSQLConnection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            dataAdapter.InsertCommand = new SqlCommand(sqlStatement, cnn);
            dataAdapter.InsertCommand.ExecuteNonQuery();
            cnn.Close();
        }
        public void Update(string sqlStatement)
        {
            SqlConnection cnn = OpenSQLConnection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            dataAdapter.UpdateCommand = new SqlCommand(sqlStatement, cnn);
            dataAdapter.UpdateCommand.ExecuteNonQuery();
            cnn.Close();
        }

        public void Delete(string sqlStatement)
        {
            SqlConnection cnn = OpenSQLConnection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            dataAdapter.DeleteCommand = new SqlCommand(sqlStatement, cnn);
            dataAdapter.DeleteCommand.ExecuteNonQuery();

        }

    }
    

}