using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsData.API;
using UtilityExtensions;
using Dapper;

namespace CmsData
{
    public partial class PythonModel
    {
        public void AddUpdateJsonRecord(string json,
                string section, object i1, object i2 = null, object i3 = null, object i4 = null)
        {
            var id1 = i1.ToString();
            var id2 = i2?.ToString() ?? "";
            var id3 = i3?.ToString() ?? "";
            var id4 = i4?.ToString() ?? "";
            var dd = DynamicDataFromJson(json);
            if (dd != null)
            {
                var emptyvalues = dd.dict.Where(vv => vv.Value?.ToString() == "").Select(vv => vv.Key).ToList();
                foreach (var k in emptyvalues)
                    dd.Remove(k);
                json = dd.ToString();
            }
            db.Connection.Execute("custom.AddUpdateJsonRecord",
                new { json, section, id1, id2, id3, id4 },
                commandType: System.Data.CommandType.StoredProcedure);
        }
        public void AddUpdateJsonRecord(DynamicData data,
                string section, object i1, object i2 = null, object i3 = null, object i4 = null)
        {
            AddUpdateJsonRecord(data.ToString(), section, i1, i2, i3, i4);
        }
        public void DeleteJsonRecord(string section, object i1, object i2 = null, object i3 = null, object i4 = null)
        {
            var id1 = i1.ToString();
            var id2 = i2?.ToString() ?? "";
            var id3 = i3?.ToString() ?? "";
            var id4 = i4?.ToString() ?? "";
            db.Connection.Execute("custom.DeleteJsonRecord",
                new { section, id1, id2, id3, id4 },
                commandType: System.Data.CommandType.StoredProcedure);
        }
        public void DeleteJsonRecordSection(string section)
        {
            db.Connection.Execute("delete custom.JsonDocumentRecords where Section = @section", new {section});
        }
        public List<DynamicData> SqlListDynamicData(string sql, DynamicData metadata = null)
        {
            var q = QueryFunctions();
            var rows = q.QuerySql(sql).ToList();
            var list = new List<DynamicData>();
            foreach (var r in rows)
            {
                var dd = ConvertDapperRow(r, metadata);
                list.Add(dd);
            }
            return list;
        }
        public DynamicData SqlTop1DynamicData(string sql, DynamicData metadata = null)
        {
            var q = QueryFunctions();
            var r = q.QuerySqlTop1(sql);
            if(r == null)
                return null;
            return ConvertDapperRow(r, metadata);
        }

        private DynamicData ConvertDapperRow(dynamic r, DynamicData metadata)
        {
            var dd = new DynamicData(r);
            var jkey = dd.Keys().FirstOrDefault(k => k.Contains("Json", ignoreCase: true));
            if (jkey.HasValue())
                dd.AddValue(jkey, DynamicDataFromJson(dd[jkey].ToString(), metadata));
            return dd;
        }

        public List<dynamic> SqlList(string sql)
        {
            var q = QueryFunctions();
            return q.QuerySql(sql).ToList();
        }

        private DynamicData DynamicDataFromJson(object json, DynamicData metadata = null)
        {
            var dd = DynamicDataFromJson(json.ToString());
            if(metadata == null)
                return dd;
            foreach (var k in dd.Keys())
            {
                var meta = (metadata[k])?.ToString();
                if(meta == null)
                    continue;
                var val = dd[k];
                switch (meta)
                {
                    case "money":
                        dd.AddValue(k, val.ToNullableDecimal());
                        break;
                    case "int":
                        dd.AddValue(k, val.ToInt2());
                        break;
                }
            }
            return dd;
        }
    }
}
