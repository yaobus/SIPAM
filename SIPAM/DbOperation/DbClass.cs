using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIPAM.GlobalVariable;
using SIPAM.ManagePage;
using System.Collections;

namespace SIPAM.DbOperation
{
    internal class DbClass
    {
        public static MySqlConnector.MySqlConnection MySqlConnection;

        //连接MYSQL数据库
        public static MySqlConnector.MySqlConnection ConnectionMysql(string dbConnectInfo)
        {

            MySqlConnection = new MySqlConnection(dbConnectInfo);

            return MySqlConnection;
        }



        /// <summary>
        /// 在指定的mysql连接上执行sql语句,返回MySqlDataReader 
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static MySqlDataReader CarrySqlCmd(string sql)
        {
            string query = AddressManage.ReplaceChineseComma(sql);

            try
            {
                if (MySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    MySqlConnection.Open();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }


            MySqlCommand cmdCommand = new MySqlCommand(query, MySqlConnection);
            MySqlDataReader reader = cmdCommand.ExecuteReader();

            return reader;

            
        }


        /// <summary>
        /// 执行mysql修改,插入数据
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回受影响的行数,为int值</returns>
        public static int ModifySql(string sql)
        {

            string query = AddressManage.ReplaceChineseComma(sql);

            try
            {
                MySqlConnection = DbClass.ConnectionMysql(Variable.DbConnectInfo);
                MySqlConnection.Open(); //连接数据库
                MySqlCommand SqlCommand = new MySqlCommand(query, MySqlConnection);
                return SqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 0;
            }


        }


        /// <summary>
        /// 查询表中的数据数量，返回数据条数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteScalarTableNum(string sql)
        {
            string query = AddressManage.ReplaceChineseComma(sql);

            using (MySqlConnection connection = new MySqlConnection(Variable.DbConnectInfo))
            {
                connection.Open();

                
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // 使用 ExecuteScalar 获取总行数
                    int rowCount = Convert.ToInt32(command.ExecuteScalar());

                    return rowCount;

                    //MessageBox.Show( GlobalFunction.IpAddressConvert.IpToDecimal(IpTextBox.Text).ToString());

                }


            }

        }




        public static string ExecuteSql(string sql)
        {

            if (MySqlConnection.State != System.Data.ConnectionState.Open)
            {
                MySqlConnection.Open();
            }


            MySqlCommand command = new MySqlCommand(sql, MySqlConnection);

            command.ExecuteNonQuery();



            return sql;
        }




        /// <summary>
        /// 异步执行sql语句
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <returns></returns>
        public static async Task ExecuteSqlAsync(string sql)
        {
            string query = AddressManage.ReplaceChineseComma(sql);

            // 创建 MySqlConnection 对象
            using (MySqlConnection connection = new MySqlConnection(Variable.DbConnectInfo))
            {
                try
                {
                    // 异步打开连接
                    await connection.OpenAsync();


                    // 执行异步 SQL 查询
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                
            }



        }


        /// <summary>
        /// 获取查询返回的数据
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetReaderString(MySqlDataReader reader, string name)
        {


            string value = (reader[name] != DBNull.Value) ? reader[name].ToString() : string.Empty;

            

            return value;


        }


        /// <summary>
        /// 异步执行无返回值的sql语句，并返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
       public static async Task<int> ExecuteNonQueryAsync(string sql)
        {
            int rowsAffected = 0;
            using (MySqlConnection connection = new MySqlConnection(Variable.DbConnectInfo))
            {

                

                await connection.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    //// 设置参数值
                    //command.Parameters.AddWithValue("@value1", "some_value1");
                    //command.Parameters.AddWithValue("@value2", "some_value2");

                    // 执行 INSERT 语句
                    rowsAffected = await command.ExecuteNonQueryAsync();
                }
            }

            return rowsAffected;
        }


    }
}
