CREATE VIEW [export].[XpPeopleExtra] AS 
SELECT
	PeopleId ,
    Field ,
    CodeValue = StrValue ,
    DateValue ,
    TextValue = Data ,
    IntValue ,
    BitValue ,
    Type
FROM dbo.PeopleExtra
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
