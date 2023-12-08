using MySql.Data.MySqlClient;
using PEUtils;

namespace GameServer
{
    // 数据库操作管理类
    public class DBSvc : Singleton<DBSvc>
    {
        private const string UserID = "root";
        private const string Password = "root";
        private const string IP = "localhost";
        private const uint Port = 3306;
        private const string Database = "mysql";

        public MySqlConnection Connetion = null;

        public override void Init()
        {
            base.Init();
            try
            {
                // 初始化数据库
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
                // 用户名
                builder.UserID = UserID;
                // 密码
                builder.Password = Password;
                // IP地址
                builder.Server = IP;
                // 端口
                builder.Port = Port;
                // 数据库名
                builder.Database = Database;

                Connetion = new MySqlConnection(builder.ConnectionString);
                // 开启数据库
                Connetion.Open();
                PELog.ColorLog(LogColor.Cyan, "[Database] 数据库服务初始化完成！");
            }
            catch (Exception ex)
            {
                PELog.Warn($"IP:{IP},Port:{Port},DB:{Database},error:{ex.Message}");
            }
        }

        /// <summary>
        /// 数据库查询API
        /// </summary>
        /// <param name="queryInfo">查询字段</param>
        /// <param name="tableName">表名</param>
        /// <param name="result">查询结果</param>
        public void QueryCommandBySingleCondition(string sql, out MySqlDataReader reader)
        {
            MySqlCommand cmd = new MySqlCommand(sql, Connetion);
            reader = cmd.ExecuteReader();
        }
    }
}