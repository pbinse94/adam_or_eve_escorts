

using Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.Request.Profile
{
    public class EmailRequestModel
    {
#nullable disable
       
        public string Name { get; set; }

        
        public string Email { get; set; }

   
        
        public string PhoneNumber { get; set; }

        
        public string Day { get; set; }
        public string EscortMail { get; set; }
        public string DisplayName { get; set; }

        
        public TimeSpan Time { get; set; }

        
        public int Duration { get; set; }

        
        public string Service { get; set; }

        
        public string MeetLocation { get; set; }

      
        public bool IsFirstTime { get; set; }

       
        public string ContactMethod { get; set; }
    }
}
