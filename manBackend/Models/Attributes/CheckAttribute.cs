using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace manBackend.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckAttribute : Attribute
    {
        public CheckAttribute()
        {

        }
        /// <summary>
        /// Use for data types with <see cref="CheckAttribute"/> properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Check<T>(T left, T right)
        {
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (!prop.HasAttribute<CheckAttribute>()) 
                    continue;
                //At this moment "prop" has CheckAttribute

                var leftVal = prop.GetValue(left);
                var rightVal = prop.GetValue(right);

                if (leftVal == null && rightVal == null) continue;

                if ((leftVal == null && rightVal != null) || !leftVal.Equals(rightVal)) 
                    return false;

            }

            return true;
        }
        /// <summary>
        /// Use for data types with <see cref="CheckAttribute"/> properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static int GetHashCode<T>()
        {
            int hashCode = 0;
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (prop.HasAttribute<CheckAttribute>())
                {
                    int propHashCode = prop.PropertyType.GetHashCode() % 1_000_000;

                    hashCode += propHashCode;

                }
            }
            return hashCode | 0b0000_0101_1101_0010_1100_0001_0000_0011;
        }
    }
}
