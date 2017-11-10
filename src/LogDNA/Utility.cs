using System;

namespace RedBear.LogDNA
{
    internal static class Utility
    {
        /// <summary>
        /// Converts the DateTime to a UNIX timestamp.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The UNIX timestamp repersentation of the DateTime.</returns>
        public static int ToUnixTimestamp(this DateTime time)
        {
            var t = time - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }

        /// <summary>
        /// Converts the DateTime to a Java timestamp in milliseconds.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The Java timestamp repersentation of the DateTime in milliseconds.</returns>
        public static long ToJavaTimestamp(this DateTime time)
        {
            return ((long) time.ToUnixTimestamp())*1000;
        }

    }
}
