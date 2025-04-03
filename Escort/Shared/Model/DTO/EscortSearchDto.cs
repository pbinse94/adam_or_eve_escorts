namespace Shared.Model.DTO
{
    public class EscortSearchDto
    {
#nullable disable
        public int UserId { get; set; }
        public int EscortId { get; set; }
        public string DisplayName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public int Age { get; set; }
        public int UserType { get; set; }
        public short Height { get; set; }
        public string HeightInFeet { get; set; }
        public string BodyType { get; set; }
        public string ProfileImage { get; set; }
        public string Dress { get; set; }
        public int TotalViews { get; set; }

        public bool IsFavorite { get; set; }
        public bool IsPhotoVerified { get; set; }
    }
}
