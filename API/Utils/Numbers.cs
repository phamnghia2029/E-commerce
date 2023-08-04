using System;

namespace API.Utils
{
    public static class Numbers
    {
        public static int? IntegerOf(object? value, int? defaultValue = null)
        {
            if (value == null)
            {
                return defaultValue;
            }
            try
            {
                return Int32.Parse(value?.ToString());
            }
            catch (Exception e)
            {
                return defaultValue;
            }
        }

        public static float? FloatOf(object? value)
        {
            if (value == null)
            {
                return null;
            }
            try
            {
                return float.Parse(value?.ToString());
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static decimal? DecimalOf(object? value)
        {
            if (value == null)
            {
                return null;
            }
            try
            {
                return decimal.Parse(value?.ToString());
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static int GetCeilOnDivide(int large, int small)
        {
            int multiply = large / small;

            return multiply * small == large ? multiply : multiply + 1;
        }
    }
}
