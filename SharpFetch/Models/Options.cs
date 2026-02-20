namespace SharpFetch.Models
{
    public class Options
    {
        public required HttpMethod Method { get; set; } = HttpMethod.Get;

        public Headers? Headers { get; set; }

        public object? Body { get; set; }
    }
}
