using TelegramBot.Dto.Helper;

namespace TelegramBot.Common.Helper.Enums
{
    public enum SubmissionsStatusesEnum
    {
        [EnumDescription("Send")]
        Send,
        [EnumDescription("Correct")]
        Correct,
        [EnumDescription("Mistake")]
        Mistake,
        [EnumDescription("InternalError")]
        InternalError,
        [EnumDescription("Received")]
        Received,
        [EnumDescription("Compiling")]
        Compiling,
        [EnumDescription("Running")]
        Running,
        [EnumDescription("Compile Error")]
        CompileError,
        [EnumDescription("Run-Time Error")]
        RunTimeError,
        [EnumDescription("Time Limit Exceeded")]
        TimeLimitExceeded,
        [EnumDescription("Memory Limit Exceeded")]
        MemoryLimitExceeded,
        [EnumDescription("Output Limit Exceeded")]
        OutputLimitExceeded,
        [EnumDescription("Security Violation")]
        SecurityViolation,
        [EnumDescription("Wrong Answer")]
        WrongAnswer,
        [EnumDescription("Accepted")]
        Accepted,
        [EnumDescription("Waiting For Compile")]
        WaitingForCompile,
        [EnumDescription("Waiting For Run")]
        WaitingForRun,
        [EnumDescription("Presentation Error")]
        PresentationError,
        [EnumDescription("Partial Solution")]
        PartialSolution,
        [EnumDescription("Rejected")]
        Rejected,
        [EnumDescription("Disqualified")]
        Disqualified
    }
}
