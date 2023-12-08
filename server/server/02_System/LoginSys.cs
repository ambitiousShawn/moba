

using GameProtocol;
using MySql.Data.MySqlClient;
using PEUtils;

namespace GameServer
{
    public class LoginSys : SystemRoot<LoginSys>
    {
        public override void Init()
        {
            base.Init();
            PELog.ColorLog(LogColor.Cyan, "[Login] 登录系统初始化完成！");
        }
        public override void Update()
        {
            base.Update();
        }

        public void ReqLogin(MsgPack msgPack)
        {
            ReqLogin data = msgPack.msg.reqLogin;
            GameMsg msg = new GameMsg
            {
                cmd = CMD.RspLogin
            };

            if (cacheSvc.IsAccountOnline(data.account))
            {
                // 返回错误信息：账号已上线
                msg.errorCode = ErrorCode.AccountIsOnLine;
            }
            else
            {
                // // 上线相关操作，增加缓存
                uint sid = msgPack.session.GetSessionID();
                UserData ud = new UserData
                {
                    id = sid,
                    name = "Shawn_" + sid,
                    level = 17,
                    exp = 10086,
                    coin = 999,
                    diamond = 666,
                    ticket = 0,
                    heroDatas = new List<HeroData>
                    {
                        new HeroData
                        {
                            heroId = 101,
                        },
                        new HeroData
                        {
                            heroId = 102,
                        }
                    }
                };
                msg.rspLogin = new RspLogin
                {
                    userData = ud,
                };

                MySqlDataReader reader = null;
                DBSvc.Instance.QueryCommandBySingleCondition("select * from account_table where account=\"admin\" and password=\"admin\"", out reader);
                if (reader.FieldCount > 0)
                {
                    // 登录成功
                    msg.error = 0;
                    cacheSvc.AccountOnline(data.account, msgPack.session, ud);
                    msgPack.session.SendMsg(msg);
                    PELog.ColorLog(LogColor.Cyan, "登录成功！");
                }
                else
                {
                    // 登录失败
                    msg.error = 1;
                    PELog.Warn("账户不存在或密码错误!");
                }
                reader.Close();
            }
        }
    }
}
