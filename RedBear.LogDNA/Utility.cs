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
    }
}
