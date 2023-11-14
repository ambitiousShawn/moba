using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShawnFramework.ShawMath
{

    public class ShawMathLibrary
    {
        /// <summary>
        /// 算数平方根(牛顿迭代法)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="interatorCount"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ShawInt Sqrt(ShawInt value, int interatorCount = 8)
        {
            if (value == ShawInt.zero)
            {
                return 0;
            }
            if (value < ShawInt.zero)
            {
                throw new Exception();
            }

            ShawInt result = value;
            ShawInt history;
            int count = 0;
            do
            {
                history = result;
                result = (result + value / result) >> 1;
                ++count;
            } while (result != history && count < interatorCount);
            return result;
        }

        public static ShawArgs Acos(ShawInt value)
        {
            ShawInt rate = (value * AcosTable.HalfIndexCount) + AcosTable.HalfIndexCount;
            rate = Clamp(rate, ShawInt.zero, AcosTable.IndexCount);
            return new ShawArgs(AcosTable.table[rate.RawInt], AcosTable.Multipler);
        }


        public static ShawInt Clamp(ShawInt input, ShawInt min, ShawInt max)
        {
            if (input < min)
            {
                return min;
            }
            if (input > max)
            {
                return max;
            }
            return input;
        }
    }

}