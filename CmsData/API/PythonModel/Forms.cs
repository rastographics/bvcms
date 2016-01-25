namespace CmsData
{
    public partial class PythonModel
    {
        public string Script { get; set; } // set this in the python code for javascript on the output page
        public string Header { get; set; } // set this in the python code for output page
        public string Output { get; set; } // this is set automatically from the print statements for the output page
        public string Form { get; set; }
        public string HttpMethod { get; set; }
    }
}