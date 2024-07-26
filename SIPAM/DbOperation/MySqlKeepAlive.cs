using MySqlConnector;
using SIPAM.GlobalVariable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SIPAM.DbOperation
{
    public class MySqlKeepAlive
    {
        private static Timer _timer;
       
        public static void Start()
        {
            _timer = new Timer(300000); // 300000 毫秒 = 5 分钟
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var connection = DbClass.ConnectionMysql(Variable.DbConnectInfo);


            try
            {
                connection.Open();
                using (var command = new MySqlCommand("SELECT 1", connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("KeepAlive query succeeded");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KeepAlive query failed: {ex.Message}");
            }

        }
    }
}
