using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Request.WebUser
{
    public class EscortSearchRequest
    { 
        public int StartAge { get; set; }
        public int EndAge { get; set; }
        public string? Gender { get; set; }
        public int StartRate { get; set; }
        public int? EndRate { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public int  LoginUserId { get; set; }
        public int EscortType { get; set; }
        public List<string> Services { get; set; } = [];
        public List<string> EsortCategories { get; set; } = [];
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int CallType { get; set; }
        public int ProfileType { get; set; }
    }

    public class PopularEscortRequest
    {
        public int LoginUserId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Country { get; set; }
        public string SearchText { get; set; } = string.Empty;
    }
}
