IF EXISTS (
	SELECT type_desc, type
	FROM SYS.PROCEDURES WITH(NOLOCK)
	WHERE NAME = 'GreatPlainsIncomeExport'
		AND type = 'P')
	BEGIN
		DROP PROCEDURE [dbo].[GreatPlainsIncomeExport]
	END
GO