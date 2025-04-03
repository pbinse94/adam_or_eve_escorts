namespace Shared.Common
{
    public class DataTableParameters
    {
        public int Draw { get; set; }
        public DTColumn[]? Columns { get; set; }
        public DTOrder[]? Order { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DTSearch? Search { get; set; }
        public string? SortOrder
        {
            get
            {
                return Order != null ? Order?.FirstOrDefault()?.Dir.ToString() : null;
            }
        }
        public string? SortColumn
        {
            get
            {
                return Order != null ? Columns?[Order?.FirstOrDefault()?.Column ?? 0].Name : null;
            }
        }
    }

    public class DTColumn
    {
        public string? Data { get; set; }
        public string? Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DTSearch? Search { get; set; }
    }

    public class DTOrder
    {
        public int Column { get; set; }
        public DTOrderDir Dir { get; set; }
    }

    public enum DTOrderDir
    {
        ASC,
        DESC
    }

    public class DTSearch
    {
        public string? Value { get; set; }
        public bool Regex { get; set; }
    }
}
