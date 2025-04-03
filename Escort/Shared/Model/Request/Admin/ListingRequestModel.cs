namespace Shared.Model.Request.Admin
{
    public class ListingRequestModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
    }
}
