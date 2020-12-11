using EntityFrameworkCore.CommonTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot.DAL.Entities
{
    public interface IEntityBase<TKey> : IEntityBase
    {
        TKey Id { get; set; }
    }
    public interface IEntityBase : IDeletionTrackable, IModificationTrackable, ICreationTrackable
    {
    }
}
