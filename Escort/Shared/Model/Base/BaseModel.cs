using System.ComponentModel.DataAnnotations;

namespace Shared.Model.Base
{
    public class BaseModel
    {
        [Display(Name = "S.No.")]
        public int Id { get; set; }
        public DateTime AddedOnUTC { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOnUTC { get; set; } = DateTime.UtcNow;
        public bool? IsDeleted { get; set; }
    }
}
