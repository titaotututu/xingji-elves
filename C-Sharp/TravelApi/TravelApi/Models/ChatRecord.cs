using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApi.Models
{
    public class ChatRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long UserId { get; set; }

        [ForeignKey("UserId")]

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<ChatMessage>? Messages { get; set; } = new List<ChatMessage>();
    }
} 