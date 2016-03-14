using System;

namespace CmsData
{
    public interface ITableWithExtraValues
    {
        void AddEditExtraValue(string field, string value);
        void AddEditExtraData(string field, string value, DateTime? dt = null);
        void AddEditExtraDate(string field, DateTime? value);
        void AddToExtraData(string field, string value);
        void AddEditExtraInt(string field, int value);
        void AddEditExtraBool(string field, bool tf);
        void RemoveExtraValue(CMSDataContext db, string field);
        void LogExtraValue(string op, string field);
    }
}
