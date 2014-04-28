using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;

namespace CmsWeb.MobileAPI
{
	/*
	+ [Id] [int] IDENTITY(1,1) NOT NULL,
	# [OwnerId] [int] NOT NULL,
	# [ListId] [int] NOT NULL,
	+ [CoOwnerId] [int] NULL,
	# [CoListId] [int] NULL,
	+ [StatusId] [int] NULL,
	# [CreatedOn] [datetime] NOT NULL,
	# [SourceContactId] [int] NULL,
	# [CompletedContactId] [int] NULL,
	+ [Notes] [varchar](max) NULL,
	# [ModifiedBy] [int] NULL,
	# [ModifiedOn] [datetime] NULL,
	# [Project] [varchar](50) NULL,
	# [Archive] [bit] NOT NULL,
	+ [Priority] [int] NULL,
	+ [WhoId] [int] NULL,
	+ [Due] [datetime] NULL,
	# [Location] [varchar](50) NULL,
	+ [Description] [varchar](100) NULL,
	# [CompletedOn] [datetime] NULL,
	# [ForceCompleteWContact] [bit] NULL,
	# [OrginatorId] [int] NULL,
	*/

	public class MobileTaskFull : MobileTask
	{
		public int coowner = 0;
		public int coownerlist = 0;

		public int originator = 0;

		public DateTime created = DateTime.Now;

		public int completewithcontact = 0;
		public int sourcecontact = 0;
		public int completedcontact = 0;

		public DateTime? completed = null;
		public int archive = 0;

		public int modifiedby = 0;
		public DateTime? modifieddate = null;

		public string project = "";
		public string location = "";

		public new MobileTaskFull populate(CmsData.Task t)
		{
			base.populate(t);

			// ToDo: Finish population

			// End ToDo

			return this;
		}
	}
}