namespace CmsData.Finance.Acceptiva.Core
{
    internal class CanadaState
    {
        public CanadaState(string name, string code)
        {
            this.Name = name;
            this.Code = code;
        }

        public string Name { get; private set; }

        public string Code { get; private set; }
    }
}
