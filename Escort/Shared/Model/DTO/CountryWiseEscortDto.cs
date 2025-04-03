namespace Shared.Model.DTO
{
    public class CountryWiseEscortDto
    {
        public string CountryName { get; set; } = string.Empty;
        public string CountryAbbreviation { get; set; } = string.Empty;
        public int CountryEscortCount { get; set; }
    }
}
