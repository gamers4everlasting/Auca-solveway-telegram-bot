using System;
using System.Collections.Generic;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Models.Commands;

namespace TelegramBot.BLL.Services
{
    public class CommandService : ICommandService
    {
        private readonly List<Command> _commands;

        public CommandService()
        {
            _commands = new List<Command>
            {
                //new HelloCommand(),
               // new ApplicantNameCommand(),
                //new ApplicantNumberCommand(),
                //new ApplicantPositionCommand(),
                new ClientNameCommand(),
                //new ProblemCommand(),
                //new DateTimeCommand(),
                //new ProblemPhotoCommand()
            };
        }

        public List<Command> Get() => _commands;
    }
}
