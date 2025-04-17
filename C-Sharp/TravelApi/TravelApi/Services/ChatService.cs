using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TravelApi.Models;
using TravelApi.Repository;

namespace TravelApi.Services
{
    public interface IChatService
    {
        IEnumerable<ChatRecord> GetUserChatRecords(long userId);
        ChatRecord GetChatRecord(int recordId);
        IEnumerable<ChatMessage> GetChatMessages(int recordId);
        void AddChatRecord(ChatRecord chatRecord);
        void AddChatMessage(int recordId, ChatMessage message);
        void DeleteChatRecord(int recordId);
    }

    public class ChatService : IChatService
    {
        private readonly TravelContext _context;

        public ChatService(TravelContext context)
        {
            _context = context;
        }

        public IEnumerable<ChatRecord> GetUserChatRecords(long userId)
        {
            return _context.ChatRecords
                .Include(c => c.Messages)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }

        public ChatRecord GetChatRecord(int recordId)
        {
            return _context.ChatRecords
                .Include(c => c.Messages)
                .FirstOrDefault(c => c.Id == recordId);
        }

        public IEnumerable<ChatMessage> GetChatMessages(int recordId)
        {
            return _context.ChatMessages
                .Where(m => m.ChatRecordId == recordId)
                .OrderBy(m => m.CreatedAt)
                .ToList();
        }

        public void AddChatRecord(ChatRecord chatRecord)
        {
            _context.ChatRecords.Add(chatRecord);
            _context.SaveChanges();
        }

        public void AddChatMessage(int recordId, ChatMessage message)
        {
            var chatRecord = _context.ChatRecords.Find(recordId);
            if (chatRecord != null)
            {
                message.ChatRecordId = recordId;
                _context.ChatMessages.Add(message);
                _context.SaveChanges();
            }
        }

        public void DeleteChatRecord(int recordId)
        {
            var chatRecord = _context.ChatRecords
                .Include(c => c.Messages)
                .FirstOrDefault(c => c.Id == recordId);

            if (chatRecord != null)
            {
                _context.ChatMessages.RemoveRange(chatRecord.Messages);
                _context.ChatRecords.Remove(chatRecord);
                _context.SaveChanges();
            }
        }
    }
} 