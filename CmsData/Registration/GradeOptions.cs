﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmsData.Registration
{
	public class GradeOptions : Ask
	{
		public string Label { get; set; }
		public List<GradeOption> list { get; set; }

		public GradeOptions()
			: base("GradeOptions")
		{
			list = new List<GradeOption>();
		}

		public override void Output(StringBuilder sb)
		{
			if (list.Count == 0)
				return;
			Settings.AddValueNoCk(0, sb, "GradeOptions", Label);
			foreach (var g in list)
				g.Output(sb);
			sb.AppendLine();
		}
		public static GradeOptions Parse(Parser parser)
		{
			var go = new GradeOptions();
			go.Label = parser.GetString("GradeOptions");
			if (parser.curr.indent == 0)
				throw parser.GetException("expected indented Options");
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				var option = GradeOption.Parse(parser, startindent);
				go.list.Add(option);
			}
			var q2 = go.list.GroupBy(mi => mi.Code).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
			if (q2.Any())
				throw parser.GetException("Duplicate Code in GradeOptions: " + string.Join(",", q2));
			return go;
		}
		public class GradeOption
		{
			public string Name { get; set; }
			public string Description { get; set; }
			public int Code { get; set; }
			public void Output(StringBuilder sb)
			{
				Settings.AddValueCk(1, sb, Description);
				Settings.AddValueCk(2, sb, "Code", Code);
			}
			public static GradeOption Parse(Parser parser, int startindent)
			{
				if (parser.curr.kw != Parser.RegKeywords.None)
					throw parser.GetException("expected description only");
				var option = new GradeOption();
				option.Description = parser.GetLine();
				if (parser.curr.indent <= startindent)
					throw parser.GetException("expected greater indent");
				if (parser.curr.kw != Parser.RegKeywords.Code)
					throw parser.GetException("expected Code");
				var code = parser.GetNullInt();
				if (!code.HasValue)
				{
					parser.lineno--;
					throw parser.GetException("expected integer code");
				}
				option.Code = code.Value;
				return option;
			}
		}
	}
}
