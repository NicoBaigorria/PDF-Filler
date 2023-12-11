namespace PlanB_Service.Models
{
    public class WebhookBody
    {
        public long EventId { get; set; }
        public int SubscriptionId { get; set; }
        public int PortalId { get; set; }
        public int AppId { get; set; }
        public long OccurredAt { get; set; }
        public string SubscriptionType { get; set; }
        public int AttemptNumber { get; set; }
        public long ObjectId { get; set; }  // Change from int to long
        public string ChangeFlag { get; set; }
        public string ChangeSource { get; set; }
        public string SourceId { get; set; }
    }
}
