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
                string section, object pk1, object pk2 = null, object pk3 = null, object pk4 = null)
        {
            var dd = DynamicDataFromJson(json);
            AddUpdateJsonRecord(dd, section, pk1, pk2, pk3, pk4);
        }
        public void AddUpdateJsonRecord(DynamicData data,
                string section, object pk1, object pk2 = null, object pk3 = null, object pk4 = null)
        {
            var id1 = pk1.ToString();
            var id2 = pk2?.ToString() ?? "";
            var id3 = pk3?.ToString() ?? "";
            var id4 = pk4?.ToString() ?? "";
            var json = "";
            if (data != null)
            {
                var emptyvalues = data.dict.Where(vv => vv.Value?.ToString() == "").Select(vv => vv.Key).ToList();
                foreach (var k in emptyvalues)
                    data.Remove(k);
                json = data.ToString();
            }
            db.Connection.Execute("custom.AddUpdateJsonRecord",
                new { json, section, id1, id2, id3, id4 },
                commandType: System.Data.CommandType.StoredProcedure);
        }
        public void DeleteJsonRecord(string section, object pk1, object pk2 = null, object pk3 = null, object pk4 = null)
        {
            var id1 = pk1.ToString();
            var id2 = pk2?.ToString() ?? "";
            var id3 = pk3?.ToString() ?? "";
            var id4 = pk4?.ToString() ?? "";
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
