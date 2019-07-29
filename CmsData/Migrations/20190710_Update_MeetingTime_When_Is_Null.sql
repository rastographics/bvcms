UPDATE [OrgSchedule]
SET [MeetingTime] = CASE
						WHEN [SchedDay] = 0 THEN '1900-01-07 ' + CONVERT(VARCHAR(8), [SchedTime], 108)
						WHEN [SchedDay] = 1 THEN '1900-01-01 ' + CONVERT(VARCHAR(8), [SchedTime], 108)
						WHEN [SchedDay] = 2 THEN '1900-01-02 ' + CONVERT(VARCHAR(8), [SchedTime], 108)
						WHEN [SchedDay] = 3 THEN '1900-01-03 ' + CONVERT(VARCHAR(8), [SchedTime], 108)
						WHEN [SchedDay] = 4 THEN '1900-01-04 ' + CONVERT(VARCHAR(8), [SchedTime], 108)
						WHEN [SchedDay] = 5 THEN '1900-01-05 ' + CONVERT(VARCHAR(8), [SchedTime], 108)
						WHEN [SchedDay] = 6 THEN '1900-01-06 ' + CONVERT(VARCHAR(8), [SchedTime], 108)
						WHEN [SchedDay] = 10 THEN '1900-01-10 ' + CONVERT(VARCHAR(8), [SchedTime], 108)
					END
WHERE [MeetingTime] IS NULL