using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData
{
    public interface ITableWithExtraValues
    {
        void AddEditExtraValue(string field, string value);
        void AddEditExtraData(string field, string value);
        void AddEditExtraDate(string field, DateTime? value);
        void AddToExtraData(string field, string value);
        void AddEditExtraInt(string field, int value);
        void AddEditExtraBool(string field, bool tf);
        void RemoveExtraValue(CMSDataContext Db, string field);
    }
}
