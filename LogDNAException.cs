using System;

namespace RedBear.LogDNA
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// An exception related to the RedBear.LogDNA library.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class LogDNAException : Exception
    {
        public LogDNAException(string message) : base(message) { }
    }
}
