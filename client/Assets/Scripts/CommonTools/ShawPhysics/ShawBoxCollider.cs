using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;

namespace ShawnFramework.ShawnPhysics
{
    public class ShawBoxCollider : ShawColliderBase
    {
        public ShawVector3 mSize;
        public ShawVector3[] mDir; // ÖáÏò

        public ShawBoxCollider(ColliderConfig config)
        {
            name = config.mName;
            mPos = config.mPos;
            mSize = config.mSize;
            mDir = new ShawVector3[3];
            mDir[0] = config.mAxis[0];
            mDir[1] = config.mAxis[1];
            mDir[2] = config.mAxis[2];
        }


        public override bool BoxCollisionDetect(ShawBoxCollider collider, ref ShawVector3 normal, ref ShawVector3 borderAdjust)
        {
            return false;
        }

        public override bool SphereCollisionDetect(ShawCylinderCollider collider, ref ShawVector3 normal, ref ShawVector3 borderAdjust)
        {
            ShawVector3 tmpNormal = ShawVector3.zero;
            ShawVector3 tmpAdjust = ShawVector3.zero;
            bool result = collider.BoxCollisionDetect(this, ref tmpNormal, ref tmpAdjust);
            normal = -tmpNormal;
            borderAdjust = -tmpAdjust;
            return result;
        }
    }
}