using SharpFetch.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpFetch.Models
{
    public class Options
    {
        public MethodEnum Method { get; set; }

        public Dictionary<string, string>? Headers { get; set; }

        public object? Body { get; set; }
    }
}
