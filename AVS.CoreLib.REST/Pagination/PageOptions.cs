using System;
using System.ComponentModel;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.ComponentModel;

namespace AVS.CoreLib.REST.Pagination
{
    /// <summary>
    /// struct encompasses common paging parameters: offset, limit and sort order
    /// 5 10 ASK means offset:5, limit:10, sort order: ASK
    /// 10 means offset:0, limit:10, sort order: DESC
    /// </summary>
    [TypeConverter(typeof(PageOptionsTypeConverter))]
    public class PageOptions : IQueryStringFormattable
    {
        public PageOptions(int limit = 0, int offset = 0, string sort = "DESC")
        {
            Limit = limit;
            Offset = offset;
            Sort = GetSort(sort);
        }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public string Sort { get; set; }

        public int GetLimit(int count)
        {
            return Limit > 0 ? Limit : count;
        }

        public override string ToString()
        {
            if (Offset > 0)
                return $"{Offset} {Limit} {Sort}";
            return $"{Limit} {Sort}";
        }

        public string ToQueryString(string format = "offset={0}&limit={1}&sort={2}")
        {
            return string.Format(format, Offset, Limit, Sort);
        }

        public static implicit operator PageOptions(string value)
        {
            if (TryParse(value, out PageOptions range))
                return range;

            throw new Exception($"String '{value}' is not valid PageOptions value");
        }

        public static bool TryParse(string str, out PageOptions options)
        {
            options = new PageOptions();
            if (string.IsNullOrEmpty(str))
                return false;

            int limit;
            string[] parts = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 3 && int.TryParse(parts[0], out int offset) &&
                int.TryParse(parts[1], out limit))
            {
                //15 100 DESC
                options = new PageOptions(limit, offset, parts[2]);
                return true;
            }
            else if (parts.Length == 2 && int.TryParse(parts[0], out limit))
            {
                //25 ASK
                options = new PageOptions(limit, 0, parts[1]);
                return true;
            }
            else if (parts.Length == 1 && int.TryParse(parts[0], out limit))
            {
                options = new PageOptions(limit);
                return true;
            }
            return false;
        }

        private string GetSort(string arg)
        {
            return arg == "ASK" ? "ASK" : "DESC";
        }
    }

    public class PageOptionsTypeConverter : TypeConverter<PageOptions>
    {
        public override bool Parse(string str, out PageOptions options)
        {
            return PageOptions.TryParse(str, out options);
        }
    }
}