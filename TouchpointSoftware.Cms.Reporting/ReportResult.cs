using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TouchpointSoftware.Cms.Reporting
{
    public class ReportResult<TOutput>
        where TOutput : class, new()
    {
        public IEnumerable<TOutput> Data { get; protected set; }
        public long TotalItems { get; set; }

        public ReportResult(IEnumerable<TOutput> data)
        {
            Data = data;
            TotalItems = Enumerable.LongCount(data);
        }

        public Stream AsPdf()
        {
            return null;
        }

        public Stream AsExcel()
        {
            return null;
        }

        public Stream AsText()
        {
            return null;
        }

        public Stream AsJson()
        {
            return null;
        }

        public Stream AsXml()
        {
            return null;
        }

    }
}
