namespace CmsData
{
    public partial class EmailReplacements
    {
        private string StatementTypeReplacement()
        {
            var stmtcode = person.ContributionOptionsId;
            switch (stmtcode)
            {
                case Codes.StatementOptionCode.Individual:
                    return "Individual";

                case Codes.StatementOptionCode.Joint:
                    return "Joint";

                case Codes.StatementOptionCode.None:
                default:
                    return "None";
            }
        }
    }
}
