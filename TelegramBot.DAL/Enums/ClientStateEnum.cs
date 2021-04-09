using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot.DAL.Enums
{
    public enum ClientStateEnum
    {
        SolvewayCodeSet = 1, //asking to enter code from solveway.club
        ProblemSet, //Problem is set to solve (next message is submission)
        None
    }
}
