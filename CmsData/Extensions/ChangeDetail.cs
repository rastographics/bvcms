namespace CmsData
{
    public partial class ChangeDetail
    {
        public ChangeDetail(string field, object before, object after)
        {
            Field = field;
            Before = before == null 
                ? "(null)" 
                : before.ToString();
            After = after == null 
                ? "(null)" 
                : after.ToString();
        }
    }
}
