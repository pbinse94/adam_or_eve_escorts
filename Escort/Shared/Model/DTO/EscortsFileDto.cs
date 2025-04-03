using Microsoft.AspNetCore.Http;
using Shared.Model.Entities;
using Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.DTO
{
    public class EscortsFileDto
    {
        public int EscortID { get; set; }
        public int UserId { get; set; }
        public IFormFile? ProfileFile { get; set; }
        public string? ProfileImage { get; set; }
        public IFormFileCollection? SelectedImages { get; set; }
        public List<string>? ListImages { get; set; }
        public string? ImageUrls { get; set; }
        public IFormFile? Videos { get; set; }
        public string? VideoUrls { get; set; }
        public string? VideoName { get; set; }
    }
}
