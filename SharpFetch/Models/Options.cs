using System;
using System.Collections.Generic;
using System.Text;

namespace SharpFetch.Models
{
    public class Options
    {
        public HttpMethod Method { get; set; }

        public Headers Headers { get; set; }

        public object? Body { get; set; }
    }
}
