using System;

namespace ShawnFramework.ShawMath
{

    public struct ShawArgs
    {
        public int value;
        public uint multipler;

        public ShawArgs(int value, uint multipler)
        {
            this.value = value;
            this.multipler = multipler;
        }

        public static ShawArgs Zero = new ShawArgs(0, 10000);
        public static ShawArgs HALFPI = new ShawArgs(15708, 10000);
        public static ShawArgs PI = new ShawArgs(31416, 10000);
        public static ShawArgs TWOPI = new ShawArgs(62832, 10000);

        public static bool operator >(ShawArgs a, ShawArgs b)
        {
            if (a.multipler == b.multipler)
            {
                return a.value > b.value;
            }
            else
            {
                throw new Exception("multipler is unequal.");
            }
        }
        public static bool operator <(ShawArgs a, ShawArgs b)
        {
            if (a.multipler == b.multipler)
            {
                return a.value < b.value;
            }
            else
            {
                throw new Exception("multipler is unequal.");
            }
        }
        public static bool operator >=(ShawArgs a, ShawArgs b)
        {
            if (a.multipler == b.multipler)
            {
                return a.value >= b.value;
            }
            else
            {
                throw new Exception("multipler is unequal.");
            }
        }
        public static bool operator <=(ShawArgs a, ShawArgs b)
        {
            if (a.multipler == b.multipler)
            {
                return a.value <= b.value;
            }
            else
            {
                throw new Exception("multipler is unequal.");
            }
        }
        public static bool operator ==(ShawArgs a, ShawArgs b)
        {
            if (a.multipler == b.multipler)
            {
                return a.value == b.value;
            }
            else
            {
                throw new Exception("multipler is unequal.");
            }
        }
        public static bool operator !=(ShawArgs a, ShawArgs b)
        {
            if (a.multipler == b.multipler)
            {
                return a.value != b.value;
            }
            else
            {
                throw new Exception("multipler is unequal.");
            }
        }


        /// <summary>
        /// 转化为视图角度，不可再用于逻辑运算
        /// </summary>
        /// <returns></returns>
        public int ConvertViewAngle()
        {
            float radians = ConvertToFloat();
            return (int)Math.Round(radians / Math.PI * 180);
        }

        /// <summary>
        /// 转化为视图弧度，不可再用于逻辑运算
        /// </summary>
        public float ConvertToFloat()
        {
            return value * 1.0f / multipler;
        }

        public override bool Equals(object obj)
        {
            return obj is ShawArgs args &&
                value == args.value &&
                multipler == args.multipler;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return $"value:{value} multipler:{multipler}";
        }
    }

}