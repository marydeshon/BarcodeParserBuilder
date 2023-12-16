﻿namespace BarcodeParserBuilder.Exceptions.CODE39
{
    internal class Code39ParseException : ParseException
    {
        public Code39ParseException(string message, Exception? e = null) : base(message, e) { }
    }
}
