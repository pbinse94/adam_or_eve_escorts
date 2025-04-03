namespace Shared.Model.Entities
{
    public class ActivityLog
    {
        public long LogID { get; set; }
        public DateTime ActionDate { get; set; }
        public int AdminUserID { get; set; }
        public int TargetID { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string ActionDescription { get; set; } = string.Empty;
        public string DbEntityType { get; set; } = string.Empty;
    }
}
