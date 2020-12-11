using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot.DAL.Enums
{
    public enum ClientStateEnum
    {
        Initial = 1,
        SetCompanyName = 2,
        SetClientName = 3,
        SetClientPosition = 4,
        SetClientPhoneNumber = 5,
        SetProblemDateTime = 6,
        SetPromlem = 7,
        SetProblemPhoto = 8
    }
}
