using System;

namespace CmsData
{
    public interface ITableWithExtraValues
    {
        void AddEditExtraCode(string field, string value);
        void AddEditExtraText(string field, string value, DateTime? dt = null);
        void AddEditExtraDate(string field, DateTime? value);
        void AddToExtraText(string field, string value);
        void AddEditExtraInt(string field, int value);
        void AddEditExtraBool(string field, bool tf);
        void AddEditExtraValue(string field, string code, DateTime? date, string text, bool? bit, int? intn, DateTime? dt = null);
        void RemoveExtraValue(CMSDataContext db, string field);
        void LogExtraValue(string op, string field);
    }
}
