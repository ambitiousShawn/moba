using ShawnFramework.ShawMath;

namespace ShawnFramework.ShawnPhysics
{
    public enum ColliderType
    {
        Box,
        Cylinder,
    }

    public class ColliderConfig
    {
        public string mName;
        public ColliderType mType;
        public ShawVector3 mPos;

        //box
        public ShawVector3 mSize;
        public ShawVector3[] mAxis;//÷·œÚ

        //cylinder
        public ShawInt mRadius;//∞Îæ∂
    }
}