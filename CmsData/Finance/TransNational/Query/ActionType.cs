
namespace CmsData.Finance.TransNational.Query
{
    internal enum ActionType
    {
        Unknown,
        Sale,
        Refund,
        Credit,
        Auth,
        Capture,
        Void,
        Settle,
        CheckReturn,
        CheckLateReturn
    }
}
