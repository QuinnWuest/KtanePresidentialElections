using System;

namespace Assets
{
    internal enum VotingMethod
    {
        FirstPastThePost,
        LastPastThePost,
        InstantRunoff,
        CoombMethod,
        BordaCount,
        ApprovalVoting,
        STV,
        CondorcetMethod
    }

    internal static class VotingMethodExtensions
    {
        internal static string ToLogString(this VotingMethod vm)
        {
            switch (vm)
            {
                case VotingMethod.FirstPastThePost:
                    return "first-past-the-post";
                case VotingMethod.LastPastThePost:
                    return "last-past-the-post";
                case VotingMethod.InstantRunoff:
                    return "instant runoff";
                case VotingMethod.CoombMethod:
                    return "Coomb's method";
                case VotingMethod.BordaCount:
                    return "Borda count";
                case VotingMethod.ApprovalVoting:
                    return "approval voting";
                case VotingMethod.STV:
                    return "STV";
                case VotingMethod.CondorcetMethod:
                    return "Condorcet method";
                default:
                    throw new ArgumentOutOfRangeException("vm", vm, null);
            }
        }
    }
}
