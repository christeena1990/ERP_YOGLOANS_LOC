using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Drawing;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class DB
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        //SqlConnection con_yog = new SqlConnection(ConfigurationManager.ConnectionStrings["con_yog"].ConnectionString);


        //  private SqlConnection oSqlCon;
        private SqlCommand oSqlCommand;

        private SqlConnection oSqlCon;
       // private SqlCommand oSqlCommand;
        public string ServerName { get; set; }
        public string dbName { get; set; }
        public string UserName { get; set; }
        public string Userpassword { get; set; }
        public int MaxpoolSize { get; set; }
        public int MinPoolSize { get; set; }
        public bool Pooling { get; set; }
        public int TimeOut { get; set; }

        public DB()
        {

            this.ServerName = "10.0.21.8";

            this.dbName = "main_db";

            this.UserName = "yoga";
            this.Userpassword = "yog@#1234";



            this.MaxpoolSize = 75;
            this.MinPoolSize = 5;
            this.Pooling = false;
            this.TimeOut = 500;
        }
        public DataSet ExecuteDataset(String ProcedureName, SqlParameter[] Parameters)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                //if (!this.isOpened()) this.Open();
                SqlDataAdapter sDA = new SqlDataAdapter();
                oSqlCommand = new SqlCommand();
                oSqlCommand.CommandType = CommandType.StoredProcedure;
                oSqlCommand.CommandText = ProcedureName;
                oSqlCommand.Connection = con;
                Int32 TransNo = 0;
                foreach (SqlParameter Parameter in Parameters)
                {
                    oSqlCommand.Parameters.Add(Parameter);
                    if (Parameter.ParameterName.Equals("@TrNo"))
                        TransNo = Int32.Parse(Parameter.Value.ToString());
                }
                if (TransNo > 0 && TransNo < 10)
                { oSqlCommand.ExecuteNonQuery(); return null; }
                else
                    sDA.SelectCommand = oSqlCommand;
                DataSet ds = new DataSet();
                sDA.Fill(ds);
                return ds;
            }
            catch (Exception ex) { String Err = ex.Message; return null; }
        }

        public SqlDataReader ExecuteStoredProcedure(String ProcedureName, SqlParameter[] Parameters)
        {
            if (con.State != ConnectionState.Closed)
                con.Close();

            if (con.State != ConnectionState.Open)
                con.Open();




            oSqlCommand = new SqlCommand();
            oSqlCommand.CommandType = CommandType.StoredProcedure;
            oSqlCommand.CommandText = ProcedureName;
            oSqlCommand.Connection = con;
            Int32 TransNo = 0;
            // oSqlCon.Open();
            foreach (SqlParameter Parameter in Parameters)
            {
                oSqlCommand.Parameters.Add(Parameter);
                if (Parameter.ParameterName.Equals("@TrNo"))
                    TransNo = Int32.Parse(Parameter.Value.ToString());
            }
            if (TransNo > 0 && TransNo < 10)
            { oSqlCommand.ExecuteNonQuery(); return null; }
            else
            {
                return oSqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }





        }

        public DataSet ExecuteQuery(String Query)
        {
            try
            {
                if (!this.isOpened()) con.Open();
                oSqlCommand = new SqlCommand();
                oSqlCommand.CommandText = Query;
                oSqlCommand.Connection = con;
                SqlDataAdapter sDA = new SqlDataAdapter();
                sDA.SelectCommand = oSqlCommand;
                DataSet ds = new DataSet();
                sDA.Fill(ds);
                return ds;
                //return oSqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                String Err = ex.Message;
                return null;
            }
        }

        private bool isOpened()
        {
            try
            {
                return ((con.State == ConnectionState.Open) ? true : false);
            }
            catch (Exception ex) { String Err = ex.Message; return false; }
        }

        internal void ExecuteNonQuery(string v, SqlParameter[] pr)
        {
            throw new NotImplementedException();
        }
      
        public void Close()
        {
            try
            { con.Close(); }
            catch (Exception ex)
            { string str = ex.Message; }
        }
        public void Open()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = this.ServerName;
                builder.Pooling = this.Pooling;
                builder.MaxPoolSize = this.MaxpoolSize;
                builder.MinPoolSize = this.MinPoolSize;
                builder.ConnectTimeout = this.TimeOut;
                builder.InitialCatalog = this.dbName;
                builder.UserID = this.UserName;
                builder.Password = this.Userpassword;
                oSqlCon = new SqlConnection(builder.ConnectionString);
                if (oSqlCon.State == ConnectionState.Open || oSqlCon.State == ConnectionState.Broken) oSqlCon.Close();
                oSqlCon.Open();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }
    }
}