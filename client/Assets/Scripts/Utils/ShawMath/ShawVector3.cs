namespace ShawnFramework.ShawMath
{
    public struct ShawVector3
    {
        public ShawInt x;
        public ShawInt y;
        public ShawInt z;
        public ShawVector3(ShawInt x, ShawInt y, ShawInt z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public ShawInt this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                }
            }
        }

        #region 定义常用向量
        public static ShawVector3 zero
        {
            get
            {
                return new ShawVector3(0, 0, 0);
            }
        }
        public static ShawVector3 one
        {
            get
            {
                return new ShawVector3(1, 1, 1);
            }
        }
        public static ShawVector3 forward
        {
            get
            {
                return new ShawVector3(0, 0, 1);
            }
        }
        public static ShawVector3 back
        {
            get
            {
                return new ShawVector3(0, 0, -1);
            }
        }
        public static ShawVector3 left
        {
            get
            {
                return new ShawVector3(-1, 0, 0);
            }
        }
        public static ShawVector3 right
        {
            get
            {
                return new ShawVector3(1, 0, 0);
            }
        }
        public static ShawVector3 up
        {
            get
            {
                return new ShawVector3(0, 1, 0);
            }
        }
        public static ShawVector3 down
        {
            get
            {
                return new ShawVector3(0, -1, 0);
            }
        }
        #endregion

        #region 运算符
        public static ShawVector3 operator +(ShawVector3 v1, ShawVector3 v2)
        {
            ShawInt x = v1.x + v2.x;
            ShawInt y = v1.y + v2.y;
            ShawInt z = v1.z + v2.z;
            return new ShawVector3(x, y, z);
        }
        public static ShawVector3 operator -(ShawVector3 v1, ShawVector3 v2)
        {
            ShawInt x = v1.x - v2.x;
            ShawInt y = v1.y - v2.y;
            ShawInt z = v1.z - v2.z;
            return new ShawVector3(x, y, z);
        }
        public static ShawVector3 operator *(ShawVector3 v, ShawInt value)
        {
            ShawInt x = v.x * value;
            ShawInt y = v.y * value;
            ShawInt z = v.z * value;
            return new ShawVector3(x, y, z);
        }
        public static ShawVector3 operator *(ShawInt value, ShawVector3 v)
        {
            ShawInt x = v.x * value;
            ShawInt y = v.y * value;
            ShawInt z = v.z * value;
            return new ShawVector3(x, y, z);
        }
        public static ShawVector3 operator /(ShawVector3 v, ShawInt value)
        {
            ShawInt x = v.x / value;
            ShawInt y = v.y / value;
            ShawInt z = v.z / value;
            return new ShawVector3(x, y, z);
        }
        public static ShawVector3 operator -(ShawVector3 v)
        {
            ShawInt x = -v.x;
            ShawInt y = -v.y;
            ShawInt z = -v.z;
            return new ShawVector3(x, y, z);
        }

        public static bool operator ==(ShawVector3 v1, ShawVector3 v2)
        {
            return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        }
        public static bool operator !=(ShawVector3 v1, ShawVector3 v2)
        {
            return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        }
        #endregion

        public ShawInt sqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
            }
        }

        public static ShawInt SqrMagnitude(ShawVector3 v)
        {
            return v.x * v.x + v.y * v.y + v.z * v.z;
        }

        public ShawInt magnitude
        {
            get
            {
                return ShawMathLibrary.Sqrt(this.sqrMagnitude);
            }
        }

        /// <summary>
        /// 返回当前定点向量的单位向量
        /// </summary>
        public ShawVector3 normalized
        {
            get
            {
                if (magnitude > 0)
                {
                    ShawInt rate = ShawInt.one / magnitude;
                    return new ShawVector3(x * rate, y * rate, z * rate);
                }
                else
                {
                    return zero;
                }
            }
        }

        /// <summary>
        /// 返回传入参数向量的单位向量
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static ShawVector3 Normalize(ShawVector3 v)
        {
            if (v.magnitude > 0)
            {
                ShawInt rate = ShawInt.one / v.magnitude;
                return new ShawVector3(v.x * rate, v.y * rate, v.z * rate);
            }
            else
            {
                return zero;
            }
        }

        /// <summary>
        /// 规格化当前PE向量为单位向量
        /// </summary>
        public void Normalize()
        {
            ShawInt rate = ShawInt.one / magnitude;
            x *= rate;
            y *= rate;
            z *= rate;
        }

        /// <summary>
        /// 点乘
        /// </summary>
        public static ShawInt Dot(ShawVector3 a, ShawVector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        /// <summary>
        /// 叉乘
        /// </summary>
        public static ShawVector3 Cross(ShawVector3 a, ShawVector3 b)
        {
            return new ShawVector3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }

        /// <summary>
        /// 向量夹角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static ShawArgs Angle(ShawVector3 from, ShawVector3 to)
        {
            ShawInt dot = Dot(from, to);
            ShawInt mod = from.magnitude * to.magnitude;
            if (mod == 0)
            {
                return ShawArgs.Zero;
            }
            ShawInt value = dot / mod;
            //反余弦函数计算
            return ShawMathLibrary.Acos(value);
        }

        public long[] CovertLongArray()
        {
            return new long[] { x.ScaledValue, y.ScaledValue, z.ScaledValue };
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            ShawVector3 v = (ShawVector3)obj;
            return v.x == x && v.y == y && v.z == z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1} z:{2}", x, y, z);
        }
    }
}