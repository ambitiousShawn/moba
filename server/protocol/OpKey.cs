// 网络协议配置
namespace GameProtocol
{
    public enum EKeyType
    {
        None = 0,
        Move = 1,
        Skill = 2,
    }
    [Serializable]
    public class OpKey
    {
        public int opIndex;
        public EKeyType keyType;

        public SkillKey skillKey;
        public MoveKey moveKey;
    }

    [Serializable]
    public class MoveKey
    {
        public uint keyID;

        public long x;
        public long z;
    }

    [Serializable]
    public class SkillKey
    {
        public uint skillID;

        public long x;
        public long z;
    }
}