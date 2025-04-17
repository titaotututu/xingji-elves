using System;

namespace TravelApi.Models
{
    public class Feedback
    {
        public long FeedbackId { get; set; }
        public long UserId { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public string Status { get; set; }
    }
} 