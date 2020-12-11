using System.Collections.Generic;
using TelegramBot.BLL.Models.Commands;

namespace TelegramBot.BLL.Services
{
    public interface ICommandService
    {
        List<Command> Get();
    }
}
