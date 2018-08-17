IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'FundManagerRoleId'
          AND Object_ID = Object_ID(N'dbo.ContributionFund'))
BEGIN
alter table dbo.ContributionFund
add FundManagerRoleId int not null default(0)
END
