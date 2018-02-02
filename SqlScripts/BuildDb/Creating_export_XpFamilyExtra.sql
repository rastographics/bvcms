CREATE VIEW [export].[XpFamilyExtra] AS 
SELECT
	FamilyId ,
    Field ,
    CodeValue = StrValue ,
    DateValue ,
    TextValue = Data ,
    IntValue ,
    BitValue ,
    Type
FROM dbo.FamilyExtra



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
