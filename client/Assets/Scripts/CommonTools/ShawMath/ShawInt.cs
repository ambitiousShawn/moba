using System;

namespace ShawnFramework.ShawMath
{

    public struct ShawInt
    {
        private long scaledValue;
        public long ScaledValue
        {
            get
            {
                return scaledValue;
            }
            set
            {
                scaledValue = value;
            }
        }
        //��λ����
        const int BIT_MOVE_COUNT = 10;
        const long MULTIPLIER_FACTOR = 1 << BIT_MOVE_COUNT; // 1024

        public static readonly ShawInt zero = new ShawInt(0);
        public static readonly ShawInt one = new ShawInt(1);


        #region ���캯��
        //�ڲ�ʹ�ã��Ѿ�������ɵ�����
        private ShawInt(long scaledValue)
        {
            this.scaledValue = scaledValue;
        }
        public ShawInt(int val)
        {
            scaledValue = val * MULTIPLIER_FACTOR;
        }
        public ShawInt(float val)
        {
            scaledValue = (long)Math.Round(val * MULTIPLIER_FACTOR); // ��������
        }
        //float��ʧ���ȣ�������ʽת��
        public static explicit operator ShawInt(float f)
        {
            return new ShawInt((long)Math.Round(f * MULTIPLIER_FACTOR));
        }
        //int����ʧ���ȣ�������ʽת��
        public static implicit operator ShawInt(int i)
        {
            return new ShawInt(i);
        }
        #endregion

        #region �����
        //�ӣ������ˣ�����ȡ��
        public static ShawInt operator +(ShawInt a, ShawInt b)
        {
            return new ShawInt(a.scaledValue + b.scaledValue);
        }
        public static ShawInt operator -(ShawInt a, ShawInt b)
        {
            return new ShawInt(a.scaledValue - b.scaledValue);
        }
        public static ShawInt operator *(ShawInt a, ShawInt b)
        {
            long value = a.scaledValue * b.scaledValue;
            if (value >= 0)
            {
                value >>= BIT_MOVE_COUNT;
            }
            else
            {
                value = -(-value >> BIT_MOVE_COUNT);
            }
            return new ShawInt(value);
        }
        public static ShawInt operator /(ShawInt a, ShawInt b)
        {
            if (b.scaledValue == 0)
            {
                throw new Exception();
            }
            return new ShawInt((a.scaledValue << BIT_MOVE_COUNT) / b.scaledValue);
        }
        public static ShawInt operator -(ShawInt value)
        {
            return new ShawInt(-value.scaledValue);
        }
        public static bool operator ==(ShawInt a, ShawInt b)
        {
            return a.scaledValue == b.scaledValue;
        }
        public static bool operator !=(ShawInt a, ShawInt b)
        {
            return a.scaledValue != b.scaledValue;
        }
        public static bool operator >(ShawInt a, ShawInt b)
        {
            return a.scaledValue > b.scaledValue;
        }
        public static bool operator <(ShawInt a, ShawInt b)
        {
            return a.scaledValue < b.scaledValue;
        }
        public static bool operator >=(ShawInt a, ShawInt b)
        {
            return a.scaledValue >= b.scaledValue;
        }
        public static bool operator <=(ShawInt a, ShawInt b)
        {
            return a.scaledValue <= b.scaledValue;
        }

        public static ShawInt operator >>(ShawInt value, int moveCount)
        {
            if (value.scaledValue >= 0)
            {
                return new ShawInt(value.scaledValue >> moveCount);
            }
            else
            {
                return new ShawInt(-(-value.scaledValue >> moveCount));
            }
        }
        public static ShawInt operator <<(ShawInt value, int moveCount)
        {
            return new ShawInt(value.scaledValue << moveCount);
        }
        #endregion

        /// �����ʵ�ʵ��ֵ
        public float RawFloat
        {
            get
            {
                return scaledValue * 1.0f / MULTIPLIER_FACTOR;
            }
        }

        public int RawInt
        {
            get
            {
                if (scaledValue >= 0)
                {
                    return (int)(scaledValue >> BIT_MOVE_COUNT);
                }
                else
                {
                    return -(int)(-scaledValue >> BIT_MOVE_COUNT);
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            ShawInt vInt = (ShawInt)obj;
            return scaledValue == vInt.scaledValue;
        }

        public override int GetHashCode()
        {
            return scaledValue.GetHashCode();
        }

        public override string ToString()
        {
            return RawFloat.ToString();
        }
    }

}