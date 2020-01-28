ALTER FUNCTION [dbo].[PledgeBalances] ( @fundid INT )
RETURNS TABLE 
AS
RETURN 
(
    WITH contribs AS (
        SELECT
            c.CreditGiverId
            ,c.CreditGiverId2 SpouseId
            ,SUM(ISNULL(c.PledgeAmount, 0)) PledgeAmt
            ,SUM(ISNULL(c.Amount, 0)) GivenAmt
        FROM dbo.Contributions2('1900-01-01', '3000-01-01', 0, NULL, NULL, 1, NULL) c
        WHERE c.FundId = @fundid
        GROUP BY c.CreditGiverId, c.CreditGiverId2
    )
    SELECT CreditGiverId, SpouseId, PledgeAmt, GivenAmt,
        CASE WHEN PledgeAmt > 0 
            THEN
                CASE WHEN GivenAmt < PledgeAmt OR EXISTS(SELECT 1 FROM dbo.Setting s WHERE s.Id = 'ShowNegativePledgeBalances' AND s.Setting = 'true')
                THEN PledgeAmt - GivenAmt
                ELSE 0
                END
            ELSE 0
        END Balance
    FROM contribs
)
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE name = 'DefaultValue' AND object_id = OBJECT_ID('dbo.SettingMetaData'))
ALTER TABLE dbo.SettingMetadata ADD	[DefaultValue] varchar(MAX) NULL
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SettingMetadata m WHERE m.SettingId = 'ShowNegativePledgeBalances')
INSERT INTO dbo.SettingMetadata (SettingId, Description, DataType, SettingCategoryId)
VALUES ('ShowNegativePledgeBalances', 'If you set this to <i>true</i>, in places where a pledge balance is shown, the balance will be displayed as a negative number if someone gave more than was pledged. Otherwise, the balance will show as 0.00', 1, 3)
GO

UPDATE dbo.SettingMetadata SET DefaultValue = 'true' WHERE SettingId IN (
    'AttendanceReminderSMSOptin',
    'ElectronicStatementDefault',
    'NotifyCheckinChanges',
    'PasswordRequireSpecialCharacter',
    'RegularMeetingHeadCount',
    'RequireAddressOnStatement',
    'SecureProfilePictures',
    'ShowAllOrgsByDefaultInSearchBuilder',
    'UseMemberProfileAutomation')
GO
