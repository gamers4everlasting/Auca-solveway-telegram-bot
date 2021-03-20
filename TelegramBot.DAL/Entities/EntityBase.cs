using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot.DAL.Entities
{
    public class EntityBase<TKey> : IEntityBase<TKey>
    {
        public TKey Id { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
