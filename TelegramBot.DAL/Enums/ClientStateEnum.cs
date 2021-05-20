namespace TelegramBot.DAL.Enums
{
    public enum ClientStateEnum
    {
        LanguageCodeSet = 1, //asking to enter language for system
        SolvewayCodeSet, //asking to enter code from solveway.club
        ProblemSet, //Problem is set to solve (next message is submission)
        None
    }
}
