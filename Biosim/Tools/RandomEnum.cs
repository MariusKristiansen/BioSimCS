using System;
using System.Collections.Generic;
using System.Text;

namespace Biosim.Tools
{
    public class RandomEnum
    {
        private static Random _R = new Random();
        public static T RandomEnumValue<T> ()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_R.Next(v.Length));
        }
    }
}
