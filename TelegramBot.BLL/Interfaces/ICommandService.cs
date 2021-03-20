using System.Collections.Generic;
using TelegramBot.BLL.Models.Commands;

namespace TelegramBot.BLL.Interfaces
{
    public interface ICommandService
    {
        List<Command> Get();
    }
}
