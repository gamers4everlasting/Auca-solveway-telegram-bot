using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot.DAL.Enums
{
    public enum ClientStateEnum
    {
        
        LanguageSet = 1, //getting language from user
        SolvewayCodeSet, //asking to enter code from solveway.club
        ProblemSet, //Problem is set to solve (next message is submission)
        None
    }
}
