IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Object_ID = Object_ID(N'dbo.SettingType'))
    BEGIN
  CREATE TABLE [dbo].[SettingType](
         [SettingTypeId] [int] IDENTITY(1,1) NOT NULL,
         [Name] [varchar](50) NOT NULL,
         [DisplayOrder] [int] NOT NULL,
         CONSTRAINT [PK_SettingType] PRIMARY KEY CLUSTERED 
        (
         [SettingTypeId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
        )
    END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Object_ID = Object_ID(N'dbo.SettingCategory'))
    BEGIN
  CREATE TABLE [dbo].[SettingCategory](
         [SettingCategoryId] [int] IDENTITY(1,1) NOT NULL,
         [Name] [nvarchar](50) NOT NULL,
         [SettingTypeId] [int] NOT NULL,
         [DisplayOrder] [int] NOT NULL,
         CONSTRAINT [PK_SettingCategory] PRIMARY KEY CLUSTERED 
        (
         [SettingCategoryId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
        )
        ALTER TABLE [dbo].[SettingCategory]  WITH CHECK ADD  CONSTRAINT [FK_SettingCategory_SettingType] FOREIGN KEY([SettingTypeId])
        REFERENCES [dbo].[SettingType] ([SettingTypeId])
    END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Object_ID = Object_ID(N'dbo.SettingMetadata'))
    BEGIN
  CREATE TABLE [dbo].[SettingMetadata](
      [SettingId] [nvarchar](50) NOT NULL,
      [DisplayName] [varchar](max) NULL,
      [Description] [varchar](max) NULL,
      [DataType] [int] NULL,
      [SettingCategoryId] [int] NULL,
  CONSTRAINT [PK_SettingMetadata] PRIMARY KEY CLUSTERED 
  (
   [SettingId] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
  )
  ALTER TABLE [dbo].[SettingMetadata]  WITH CHECK ADD  CONSTRAINT [FK_SettingMetadata_SettingCategory] FOREIGN KEY([SettingCategoryId])
   REFERENCES [dbo].[SettingCategory] ([SettingCategoryId])
    END
GO

DELETE dbo.SettingMetadata
DELETE dbo.SettingCategory
DELETE dbo.SettingType
GO

SET IDENTITY_INSERT dbo.SettingType ON;
INSERT INTO dbo.SettingType
(SettingTypeId, Name, DisplayOrder) VALUES
(1, 'System', 0),
(2, 'Features', 10),
(3, 'Integrations', 20);
SET IDENTITY_INSERT dbo.SettingType OFF;
GO

SET IDENTITY_INSERT dbo.SettingCategory ON;

DECLARE @System INT = 1, @Features INT = 2, @Integrations INT = 3;
INSERT INTO dbo.SettingCategory
(SettingCategoryId, Name, SettingTypeId, DisplayOrder) VALUES
(1, 'Administration', @System, 10),
(2, 'Church Info',    @System, 20),
(3, 'Contributions',  @System, 30),
(4, 'Security',       @System, 40),
(5, 'Check-In',       @Features, 10),
(6, 'Contacts',       @Features, 20),
(7, 'Mobile App',     @Features, 30),
(8, 'Registrations',  @Features, 40),
(9, 'Resources',     @Features, 50),
(10, 'Small Group Finder', @Features, 60),
(11, 'Protect My Ministry',     @Integrations, 10),
(12, 'Pushpay',  @Integrations, 20),
(13, 'Rackspace',     @Integrations, 30),
(14, 'Twilio', @Integrations, 40);
SET IDENTITY_INSERT dbo.SettingCategory OFF;
GO

DECLARE @System INT = 1, @Features INT = 2, @Integrations INT = 3;
DECLARE @dataBool INT = 1, @dataDate INT = 2, @dataText INT = 3, @dataPassword INT = 4, @dataInt INT = 5;
DECLARE
@Administration INT = 1,
@ChurchInfo INT = 2,
@Contributions INT = 3,
@Security INT = 4,
@CheckIn INT = 5,
@Contacts INT = 6,
@MobileApp INT = 7,
@Registrations INT = 8,
@Resources INT = 9,
@SmallGroupFinder INT = 10,
@ProtectMyMinistry INT = 11,
@Pushpay INT = 12,
@Rackspace INT = 13,
@Twilio INT = 14;
INSERT INTO dbo.SettingMetadata
(SettingId, Description, DataType, SettingCategoryId) VALUES
('BankDepositFormat', 'This is needed only for those churches using Vanco for online giving. Enter <i>vanco</i> as the Setting value.', @dataText, @Contributions),
('CampusIds', 'If you are using the <b>ShowCampusOnRegistration</b> Setting, you must also use this Setting to indicate which Campuses you want to offer as options for a new record. This applies only to the drop down that displays when a registrant creates a new record during an online registration. Enter the Campus ID #s, separated by a comma, without any spaces. You can enter all the Campuses or only a select few. See the Setting regarding <b>ShowCampusOnRegistration</b> for more information.', @dataText, @Registrations),
('CheckImagesDisabledForUser', 'Set this to <i>true</i> to hide the check images on a person`s giving tab.', @dataBool, @Contributions),
('ContributionStatementFundDisplayFieldName', 'If you want the Fund Descriptions rather than Fund Names to display on contribution statements, enter the value of <i>FundDescription</i>. To revert to the default, enter the value of <i>FundName</i>.', @dataText, @Contributions),
('CustomBundleReport', 'Enter the appropriate value from the <a href="https://docs.touchpointsoftware.com/CustomProgramming/Python/Scripts/BundleReport.html" target="_blank">Bundle Report</a> or <a href="https://docs.touchpointsoftware.com/CustomProgramming/Python/Scripts/BundleReport2.html" target="_blank">Bundle Report 2</a> SQL recipes.', @dataText, @Contributions),
('DebitCreditLabel', 'If your Merchant Provider allows you to accept only debit cards and bank accounts and not credit cards for all online transactions, you can enter what you want the user to see as the label. You might prefer the label to be <i>Debit Card</i>, instead of the default <i>Debit/Credit Card</i>. <b>Note:</b> Be sure to check with your Merchant Provider if you are interested in allowing the use of debit cards, but not credit cards for all online transactions. Some providers may not offer that option. <b>Important:</b> If you want to allow credit cards for all transactions except online giving, see the setting named <b>NoCreditCardGiving</b>.', @dataText, @Contributions),
('DebitCreditLabel-Giving', 'This Setting, similar to <b>DebitCreditLabel</b>, will allow you to define the label displayed, but only on the payment pages for giving transactions.', @dataText, @Contributions),
('DebitCreditLabel-Registrations', 'This Setting, similar to <b>DebitCreditLabel</b>, will allow you to define the label displayed, but only on the payment pages for registration payments.', @dataText, @Contributions),
('DefaultBundleTypeId', 'You can change the default Bundle Type (the one you start with when creating a new Bundle) by entering the ID# for the Bundle Type you want as the default. You can find this ID in <i>Administration > Setup > Lookup Codes</i>. See aso <a href="https://docs.touchpointsoftware.com/Finance/Bundle_Index.html" target="_blank">Contribution Bundle</a>.', @dataInt, @Contributions),
('DefaultFundId', 'Use this Setting if you want a fund other than Fund ID <i>1</i> to be the default fund. When creating a new Bundle, this is the fund that always appears at the top of the list of funds when setting the default for the bundle.', @dataInt, @Contributions),
('DisallowInactiveCheckin', 'If you do not want Inactive Members to be able to check into the Org in which they are Inactive, set the value as <i>true</i>. This is referring to TouchPoint Self-Check-In. By default, Inactive Members of an organization that use TouchPoint Check-In will display as enrolled in the organization when they enter their phone number at the check-in kiosk. In other words, they will not have to <i>find</i> a class to attend.', @dataBool, @CheckIn),
('Feature-ContactExtra', 'Set this value to <i>true</i> if you want to use the feature that allows you to add Extra Values to a Contact form. See also <a href="https://docs.touchpointsoftware.com/ContactsAndTasks/AddExtraValuesToContact.html" target="_blank">Contact Extra Values</a>.', @dataBool, @Contacts),
('GoogleGeocodeAPIKey', 'Copy the value from the key named <i>touchpoint-gmap</i> and paste it here. See <a href="https://docs.touchpointsoftware.com/Organizations/SmallGroupFinderMap.html" target="_blank">Small Group Finder - Embedding a Map</a>.', @dataPassword, @SmallGroupFinder),
('GoogleMapsAPIKey', 'Copy the value from the key named <i>touchpoint-geocode</i>. See <a href="https://docs.touchpointsoftware.com/Organizations/SmallGroupFinderMap.html" target="_blank">Small Group Finder - Embedding a Map</a>.', @dataPassword, @SmallGroupFinder),
('LimitRegistrationHistoryToRole', 'Enter the role name required to view the registration history for others.', @dataText, @Registrations),
('LimitToRolesForContacts', 'This Setting will let you determine which user roles will display in the <i>Limit To Role</i> drop down list on the Contact form when a user is recording a Contact. Enter the user roles (comma separated) in the <i>Value</i> field that you want to display in the drop down. See also <a href="https://docs.touchpointsoftware.com/ContactsAndTasks/AddContact.html" target="_blank">Add a Contact</a>.', @dataText, @Contacts),
('MinContributionAmount', 'Enter the minimum dollar amount required for a contribution statement to generate. For example: 5 or 200. Churches that send statements every quarter may set this to a higher amount for the first 3 quarters in order not to send statements to children. Then, they may change it to a lower amount for the yearly statement in order to include everyone.', @dataInt, @Contributions),
('NoCreditCardGiving', 'By default, this is set to <i>false</i>, meaning that donors can use a credit card for online giving. Set the value to <i>true</i> if you want donors to only use their bank account for giving. <b>Note:</b> Even with this Setting, online giving for Mission Trip Support and Ask Extra Donations will allow credit card use.', @dataBool, @Contributions),
('NoEChecksAllowed', 'Set the value to <i>true</i> if you want people to only use credit cards for giving and for online registration payments.', @dataBool, @Contributions),
('NoPrintDateOnStatement', 'The default is to display (in the top-right corner of the statement) the date it was generated (print date). If you do not want this to display, set the value to <i>true</i>.', @dataBool, @Contributions),
('NoTitlesOnStatements', 'Set the value to <i>true</i> if you want the names of joint statements to read as follows: first and last name of the head of household, followed by the first and last name of the spouse. Examples: John Smith and Mary Smith; or Frank Jones and Janet Williams-Jones.', @dataBool, @Contributions),
('NotifyCheckinChanges', 'Set the value to <i>true</i> in order for the New People Manager(s) to receive an email each time someone edits a person`s record during the check-in process. These are like the notifications received when a lay person edits their own record.', @dataBool, @CheckIn),
('OnlineContributionBundleDayTime', 'Enter a day and time in this format: <i>Sunday 11:00 PM</i>, if you want your online bundles to get created once a week instead of every 24 hours. With this Setting, a new online bundle will get created after the day/time you specify. So, instead of having one online bundle per day, you will have one per week. See also <a href="https://docs.touchpointsoftware.com/Finance/OnlineGivingBundles.html" target="_blank">Online Giving Bundles</a> and <a href="https://docs.touchpointsoftware.com/Finance/Bundle_Index.html" target="_blank">Contribution Bundle</a>. <b>Important:</b> This is based on the Central Time Zone, where our TouchPoint servers are located.', @dataText, @Contributions),
('PrintEnvelopeNumberOnStatement', 'If you have envelope numbers stored as Extra Values, set the value to <i>true</i> and that envelope number will print on the contribution statement in place of the People ID #, which is the default. See also <a href="https://docs.touchpointsoftware.com/Finance/EnvelopeNumber.html" target="_blank">Using Envelope Numbers</a>.', @dataBool, @Contributions),
('RegTimeout', 'This setting determines how long a registration will stay active before timing out. The default is 180000 milliseconds, which is 180 seconds (3 minutes). The timer is reset on every keystroke (not mouse clicks), so in essence the 3 minutes does not start counting until you quit typing. So, if all you are doing is clicking, it could timeout. If you want to change it, enter a number that is bigger or smaller than the default of 180000. Do not use punctuation. <b>Note:</b> You can also use the <i>Timeout</i> setting on a specific Organization, in order to set a different timeout just for that online registration. See also <a href="https://docs.touchpointsoftware.com/OnlineRegistration/RegistrationSettings.html" target="_blank">Registration Settings Tab</a>.', @dataInt, @Registrations),
('RegisterSomeoneElseText', 'Enter whatever text you want to display in place of the default <i>Register Someone Else</i> button just before someone completes their registration. For example: Click to Register Another Person. This affects all online registrations in the database, so be sure to make the text appropriate.', @dataText, @Registrations),
('RelaxAppAddGuest', 'Setting the value to <i>true</i> allows an OrgLeaderOnly user to access all records in the database, but only when using the iPhone/Android app to add a guest when taking attendance. See also <a href="https://docs.touchpointsoftware.com/Organizations/RecordingAttendanceiPhone.html" target="_blank">iPhone / Android Attendance with App</a>.', @dataBool, @MobileApp),
('RelaxedReqAdminOnly', 'Set the value to <i>true</i> if you want only an Admin user to be able to relax requirements for an Online Registration. With this Setting, other users do not even see those options on the Registration tab.', @dataBool, @Registrations),
('RequireAddressOnStatement', 'Set the value to <i>false</i> if you want contribution statements to print (when printing All Statements), even if the record has no address. By default, if there is no address, a statement will not generate, except from the individualâ€™s record.', @dataBool, @Contributions),
('RequireCheckNoOnStatement', 'If you want check numbers to print on Contribution Statements, set the value to <i>true</i>. If you print check numbers, the statements will print only one column on the page, instead of wrapping to make two columns. You can choose to print both the check numbers and the notes (see below). See also <a href="https://docs.touchpointsoftware.com/Finance/PostBundle_Index.html" target="_blank">Post Bundle - Record Individual Contributions</a>.', @dataBool, @Contributions),
('RequireNotesOnStatement', 'If you want notes to print on Contribution Statements,  set the value to <i>true</i>. If you print notes, the statements will print only one column on the page, instead of wrapping to make two columns. You can choose to print both the check numbers and the notes (see above). See also <a href="https://docs.touchpointsoftware.com/Finance/PostBundle_Index.html" target="_blank">Post Bundle - Record Individual Contributions</a>.', @dataBool, @Contributions),
('Resources-Enabled', 'Set the value to <i>true</i> to enable the Resources feature on your database. See also <a href="https://docs.touchpointsoftware.com/People/Resources.html" target="_blank">Resources</a>.', @dataBool, @Resources),
('SGF-LoadAllExtraValues', 'Loads all extra values, even if they lack the SGF prefix', @dataBool, @SmallGroupFinder),
('SGF-OrgTypes', 'To include only Organizations of a specified Organization Type in the group finder, enter a value of the desire Organization Type. For example, to include only groups of the type <i>Community Groups</i>, give the Setting the value <i>Community Groups</i>.', @dataText, @SmallGroupFinder),
('SGF-UseEmbeddedMap', 'Set this to true to embed a map in your Small Group Finder. See <a href="https://docs.touchpointsoftware.com/Organizations/SmallGroupFinderMap.html" target="_blank">Small Group Finder - Embedding a Map</a>', @dataBool, @SmallGroupFinder),
('SendLinkExpireMinutes', 'Set this to the number of minutes after which a <a href="https://docs.touchpointsoftware.com/EmailTexting/ComposeSendLinks.html" target="_blank">SendLink</a> expires. The default is 30.', @dataText, @Registrations),
('SendRecurringGiftFailureNoticesToFinanceUsers', 'By default, this is set to <i>false</i> and these notices are sent only to the Online Notify Person(s) on the Online Giving Organizations. If you prefer for all users with <b>Finance</b> role to receive these, change the value to <i>true</i>.', @dataBool, @Contributions),
('ShowCampusOnRegistration', 'With this Setting added and the value set to <i>true</i>, all new records that are added during an online registration will be presented with a drop down to select a Campus. The registrant will be required to make a selection, unless you check the option on the <i>Registration > Registration</i> tab of the Org labeled <i>NotReqCampus</i>. This option only displays if you have <b>ShowCampusOnRegistration</b> enabled in the Settings. When you enable this Setting you must also set <b>CampusIds</b> in order to indicate which Campuses you want displayed. See Setting regarding <b>CampusIds</b>.', @dataBool, @Registrations),
('ShowChildInExtraValueParentFamily', 'This Setting with the value set to <i>true</i> is used in conjunction with a special Parent Extra Value. This will allow a child (who is not in the family with his non-custodial parent in TouchPoint) to display in the family list of his non-custodial parent when that parent enters his phone number at Check-In. This works only if the child has an Ad Hoc Extra Value named <i>Parent</i> with that non-custodial parentâ€™s People ID# as the value. See also <a href="https://docs.touchpointsoftware.com/ExtraValues/ExtraValueParent.html" target="_blank">Extra Value - Parent</a>.', @dataBool, @CheckIn),
('ShowPledgeIfMet', 'If your church tracks Pledges, you have the option to display the pledge or not to display it once the pledge has been met. The default is to not show the Pledge if the person has fulfilled his Pledge and the Fund is still Open. Set the <b>ShowPledgeIfMet</b> Setting value to <i>true</i> if you want to show the Pledge. Once you close the Fund, Pledges will no longer display on the Statement.', @dataBool, @Contributions),
('SortContributionFundsByFieldName', 'This Setting controls how funds are sorted in the drop-down list when you are posting contributions in Post Bundle/Edit mode. Set the value to <i>FundId</i> to sort by ID (number) and to allow fund selection by typing the ID. Set the value to <i>FundName</i> to sort by fund name and to allow fund selection by typing the name of the fund.', @dataText, @Contributions),
('SpecialGivingFundsHeader', 'Enter the text you want for the header above the special funds as the value, if you do not want to use the default. This is applicable only if your church is using the Special Online Giving funds (those with a sort order of 100 and greater). See also <a href="https://docs.touchpointsoftware.com/Finance/OnlineGiving.html" target="_blank">Online Giving</a>.', @dataText, @Contributions),
('StandardFundSetName', 'This applies only to churches that have set up Custom Statements for printing contribution statements for multiple 501c3 organizations. Enter a value to change the name of the standard set from Standard Statements to something else. See also <a href="https://docs.touchpointsoftware.com/Finance/CustomStatements.html" target="_blank">Custom Contribution Statements</a>.', @dataText, @Contributions),
('UX-MapPinTextColor', 'Use this Setting to set the text color within the SGF map pin (hexadecimal RGB)', @dataText, @SmallGroupFinder),
('UX-SGFSortBy', 'Use this Setting to specify a particular Extra Value field to sort the groups in the finder. For example, if you give this Setting the value <i>SGF:Neighborhood</i>, the groups will be sorted alphabetically by the values in the <i>SGF:Neighborhood</i> Extra Value.', @dataText, @SmallGroupFinder),
('UseFourDigitCodeForCheckin', 'Set this to <i>true</i> if you prefer a 4-digit Security Code instead of the default which is 3 characters, at least 1 number and 1 letter. This refers to the Security Code that prints on the labels for Check-In.', @dataBool, @CheckIn),
('UseLabelNameForDonorDetails', 'This Setting with a value of <i>true</i> will display an alternative Donor Details export that has the address fields in separate columns and has a field for Label Name. This name will combine couples if they give jointly. See also <a href="https://docs.touchpointsoftware.com/Finance/ExportContributions.html" target="_blank">Export Donor Details</a>.', @dataBool, @Contributions),
('UseSavingAccounts', 'You only need this if the bank for any of your donors requires a flag inside the transaction to let them know it is using a Savings account. This also requires an Ad Hoc Extra value be added to the donorâ€™s record. See also <a href="https://docs.touchpointsoftware.com/Finance/OnlineGiving.html#saving-accounts-for-online-giving" target="_blank">Saving Accounts for Online Giving</a>.', @dataBool, @Contributions),
('SGF-ExtraValueHost', 'Set the value to the name of an Org Extra Value added to groups in the Small Group Finder to contain the groupâ€™s host address.', @dataText, @SmallGroupFinder),
('EnableBackgroundChecks', 'Set this to <i>true</i> to use the Protect My Ministry integration.', @dataBool, @ProtectMyMinistry),
('EnableBackgroundLabels', 'Set this to <i>true</i> if you want to use labels from your Protect My Ministry account. You must also add these labels to the <a href="https://docs.touchpointsoftware.com/Administration/Lookup_Index.html" target="_blank">Lookup Codes</a>. See also <a href="https://docs.touchpointsoftware.com/People/ProtectMyMinistry-BackgroundCh.html" target="_blank">Background Checks with Protect My Ministry</a>.', @dataBool, @ProtectMyMinistry),
('PMMPassword', 'By default, this is empty. If you use the integration with Protect My Ministry for background checks, enter your account password as the value. See also <a href="https://docs.touchpointsoftware.com/People/ProtectMyMinistry-BackgroundCh.html" target="_blank">Background Checks with Protect My Ministry</a>.', @dataPassword, @ProtectMyMinistry),
('PMMUser', 'By default, this is empty. If you use the integration with Protect My Ministry for background checks, enter your account username as the value.', @dataText, @ProtectMyMinistry),
('PushPayEnableImport', 'When set to true, this Setting enables the nightly uploads of Pushpay transactions.', @dataBool, @Pushpay),
('TwilioSID', 'If you are using the integration with Twilio for SMS (text) messaging, you will get this ID# from your Twilio account. Enter it as the value. See also <a href="https://docs.touchpointsoftware.com/EmailTexting/TextingTwilio.html" target="_blank">Establish an Account with Twilio</a>.', @dataText, @Twilio),
('TwilioToken', 'If you are using the integration with Twilio for SMS (text) messaging, you will get this token from your Twilio account. Enter it as the value.', @dataPassword, @Twilio),
('RackspaceKey', 'API Key for the Rackspace account.', @dataPassword, @Rackspace),
('RackspaceUrlCDN', 'URL for the Rackspace Container.', @dataText, @Rackspace),
('RackspaceUser', 'Username for the Rackspace account.', @dataText, @Rackspace),
('AdminCoupon', 'You can change the Setting value and use this as a coupon when testing online registrations.', @dataPassword, @Administration),
('AdminMail', 'This is the email address that will be used for any Admin notifications, and from whom standard automated notifications will be sent. See also <a href="https://docs.touchpointsoftware.com/GettingStarted/FirstSteps.html" target="_blank">First Steps with your TouchPoint Database</a>.', @dataText, @Administration),
('AllowLimitToRoleForInvolvementExport', 'This is mainly for churches that have Organizations locked down so only users with a specific role can view them. By setting this to <i>true</i>, staff can run the Involvement report and see these protected Orgs in the Activity List even though they may not be able to actually access the Org itself. It has a very limited use case.', @dataBool, @Administration),
('AllowNewGenders', 'Set this to <i>true</i> if you want to be able to add another Gender in the Lookup Codes. If you do add another Gender, if will function as Unknown. We do not recommend that you do this, as Male and Female are used to determine HOH, family labels, and more, but you can choose to do so if you wish.', @dataBool, @Administration),
('AttendanceUseMeetingCategory', 'Limits the Meeting description to the choices in the Meeting Categories Lookup Table.', @dataBool, @Administration),
('BlogAppUrl', 'This should not be changed. This points the TouchPoint News link to the TouchPoint News blog on the home page, which is where we notify users of changes and new features.', @dataText, @Administration),
('BlogFeedUrl', 'This should not be changed. This presents the TouchPoint News blog articles on the home page, which is where we notify users of changes and new features.', @dataText, @Administration),
('CampusLabel', 'Enter the name you want displayed as the label instead of Campus (example: Congregation or Location). Remember, if you add this label, that is what will display in the Lookup Codes instead of Campus.', @dataText, @Administration),
('CampusRequired', 'If you set this to <i>true</i>, every new record will require a Campus be selected. There is no option for leaving the Campus not specified. However, if you also have the Setting <b>ShowCampusOnRegistration</b> and, for some reason, do not want registrants (who are creating a new people record) to be required to select a Campus during the registration, you can check <i>Not Req Campus</i> on the Organizationâ€™s <i>Registration > Registration</i> tab. When you select this for a registration, the Campus option is displayed for the registrant, but with <i>Optional</i> displaying. See the <b>ShowCampusOnRegistration</b> Setting. See also <a href="https://docs.touchpointsoftware.com/OnlineRegistration/RegistrationSettings.html" target="_blank">Registration Settings Tab</a>.', @dataBool, @Administration),
('CanOverrideHeadOfHousehold', 'Enter this setting with a value of true to enable an Adminâ€™s ability to override the default Head of Household calculation for any family desired. Once this is enabled, an Admin can promote another family member to Head. See also <a href="https://docs.touchpointsoftware.com/People/PositionInFamily.html" target="_blank">Position in Family</a>.', @dataBool, @Administration),
('DbConvertedDate', 'This is automatically populated with the date of your data conversion.', @dataDate, @ChurchInfo),
('DefaultCampusId', 'Enter the ID# (not the name) of the Campus to use as the default campus on new records and new organizations. Leave empty if your church does not have more than one campus in the database.', @dataInt, @ChurchInfo),
('DefaultHost', 'This is the URL of your database and will be set for you.', @dataText, @Administration),
('DisplayNonTaxOnStatement', 'Set this to <i>true</i> if you want all non tax deductible contributions to print on contribution statements. These print in a separate section from the other contributions.', @dataBool, @Contributions),
('ElectronicStatementDefault', 'Set this to <i>true</i> to have this flag set automatically for all new records added to the database. See also <a href="https://docs.touchpointsoftware.com/Finance/ElectronicOnlyStatements.html" target="_blank">Electronic Only Statement Option</a>.', @dataBool, @Contributions),
('EmailPromptBeforeSend', 'Set this to <i>true</i> to add a prompt that will appear after you click <i>Send</i>, asking â€œAre You Sure?â€ and giving you an opportunity to cancel or continue.', @dataBool, @Administration),
('EnableContributionFundsOnStatementDisplay', 'To make custom statements available electronically, so that donors can download them from their profiles, set this to <i>true</i>. See also <a href="https://docs.touchpointsoftware.com/Finance/CustomStatements.html" target="_blank">Custom Contribution Statements</a>.', @dataBool, @Contributions),
('EnforceEditCampusRole', 'Set this to <i>true</i> when you want only those with the user role <b>EditCampus</b> to be able to change/edit the Campus on a personâ€™s record. This applies only to the Campus field on a personâ€™s record, not to setting the Campus for an organization. After adding this Setting, add the User Role <b>EditCampus</b> in the Lookup Codes and assign that role to everyone that should be able to edit the Campus for an individual. Exception: If you have this Setting, a My Data user (and any other user) can edit his and his familyâ€™s Campus without having the <b>EditCampus</b> role if you have the Setting <b>MyDataCanEditCampus</b>.', @dataBool, @Administration),
('EvCommentFields', 'If you have Standard Extra Values that you want to display on the Inreach / Outreach Report, enter the names of the Extra Values you want to display on the report, separated by a comma (no spaces).', @dataText, @Administration),
('ExternalManageGivingUrl', 'If your donors give through an external giving service rather than through TouchPoint, enter the URL to your recurring giving site. This will configure the link for managed giving on a donorâ€™s <i>Giving</i> tab to point to your external giving service.', @dataText, @Contributions),
('ExternalOneTimeGiftUrl', 'If your donors give through an external giving service rather than through TouchPoint, enter the URL to your one-time giving site. This will configure the link for one-time giving on a donorâ€™s Giving tab to point to your external giving service. See also <a href="https://docs.touchpointsoftware.com/Finance/OnlineGiving.html" target="_blank">Online Giving</a> for the section on <a href="https://docs.touchpointsoftware.com/Finance/Onlinegiving.html#external-giving-links" target="_blank">External Giving Links</a>. ', @dataText, @Contributions),
('HideMemberStatusFromMyData', 'Set this to <i>true</i> if you do not want My Data users to be able to view the <i>Profile</i> tab on their own people record or that of their family members. See also <a href="https://docs.touchpointsoftware.com/Administration/Roles_Index.html" target="_blank">New Users and Roles</a>.', @dataBool, @Administration),
('HideMyDataMemberBadge', 'Set this to <i>true</i> if you do not want My Data users to see the Member Badge on their people record or the Member Status for immediate family members in the family list. See also <a href="https://docs.touchpointsoftware.com/Administration/Roles_Index.html" target="_blank">New Users and Roles</a>.', @dataBool, @Administration),
('MaxExcelRows', 'By default, this is set to 10,000 for Excel exports.', @dataInt, @Administration),
('MeetingTypesReportIncludeEmpty', 'Set this to <i>true</i> to include empty types on the Meeting Types Report.', @dataBool, @Administration),
('MenuAddPeople', 'The default is <i>false</i>. You can set this to <i>true</i> to enable the <i>Add New Person</i> option under <i>People</i> in the menu header. The option always displays, but the person clicking it will be taken to the help article explaining why it is disabled. See also <a href="https://docs.touchpointsoftware.com/SearchAdd/AddPeopleContext.html" target="_blank">Add Person in Context</a>.', @dataBool, @Administration),
('MinimumUserAge', 'Set the value to the age that a person must be to create a user account, such as 13. The default is 16. This applies to people trying to create a user account on the church database. For churches that do not capture dates of birth, they may want to set this to a value of 0. This will allow a person without a date of birth to create a user account.', @dataInt, @Administration),
('MonthFirst', 'For dates, this will display the month, then the day. By default, it is set to <i>true</i>.', @dataBool, @Administration),
('MorningBatchRan', 'If you are using Morning Batch, this will get created automatically and display the last date/time that it ran. See also <a href="https://docs.touchpointsoftware.com/Administration/MorningBatch.html" target="_blank">Morning Batch</a>.', @dataDate, @Administration),
('MorningBatchUsername', 'If you need to have special roles (e.g., the <b>Finance</b> role) for any of the Python scripts in your morning batch, enter the name of an account with the needed roles. This can be an existing user account, or one you have created specifically for this purpose.', @dataText, @Administration),
('MyDataCanEditCampus', 'If you would like for My Data users to be able to edit the Campus on their own people records and that of family members, set this to <i>true</i>. This also allows any other user to edit his own Campus. See also <a href="https://docs.touchpointsoftware.com/Administration/Roles_Index.html" target="_blank">New Users and Roles - specifically <b>EditCampus</b> role</a>.', @dataBool, @Administration),
('NameOfChurch', 'Enter the name as you want it to appear on the bottom of your TouchPoint database.', @dataText, @ChurchInfo),
('NewMemberClassLabel', 'Enter the text you want to display in place of the New Member Class on the <i>Profile > Member</i> tab.', @dataText, @Administration),
('NewPeopleManagerIds', 'Enter the People ID # for the person serving as the New People Manager for your church. You can enter more than one ID #, just separate them with a comma. The first ID in the list, will be the user that owns the New Data Entry tasks. See also <a href="https://docs.touchpointsoftware.com/People/PeopleIdNumbers.html" target="_blank">People ID Numbers</a> and <a href="https://docs.touchpointsoftware.com/People/NewPeopleManager.html" target="_blank">New People Manager</a>.', @dataText, @Administration),
('NoBirthYearOverAge', 'This must be used in conjunction with the Setting <b>NoBirthYearRole</b>. You can use this setting if you want these users to see the age and complete DOB for those under a specified age. Without this Setting, if you use <b>NoBirthYearRole</b>, those aged 18 and under will have their age and complete DOB displayed. You only need this if you want to raise or lower the age for those whose ages/DOB can be seen.', @dataText, @Administration),
('NoBirthYearRole', 'This setting will require that you specify a user role whose users should not see an age or a birth year for those records they can view in the database. One use case might be for <b>OrgLeadersOnly</b>. This will allow staff users but not lay leaders to view ages and complete dates of birth. This will mask that information on all on-screen places where a DOB or age appear, such as people lists and people records as well as normal exports and reports. It also masks this information on the mobile app. If the person has a DOB, this user will be able to see the Month and Day. With this Setting, all users can see the complete DOB and Age for a child, i.e. anyone aged 18 and under. See the <b>NoBirthYearOverAge</b> Setting if you want to change the age.', @dataText, @Administration),
('NoNewPersonTasks', 'If this is set to <i>true</i>, the New People Manager will not receive an email when a new people record is created and no data entry task will be created for the user creating the new record. The new records will still have the Member Status of <i>Just Added</i>. See also <a href="https://docs.touchpointsoftware.com/People/NewPeopleManager.html" target="_blank">New People Manager</a>.', @dataBool, @Administration),
('PasswordMinLength', 'Use a number, such as <i>7</i>.', @dataInt, @Security),
('PasswordRequireOneNumber', 'Set to <i>true</i> if you want to require at least one number in the password.', @dataBool, @Security),
('PasswordRequireOneUpper', 'Set to <i>true</i> if you want to require at least one upper case letter in the password.', @dataBool, @Security),
('PasswordRequireSpecialCharacter', 'Set to <i>true</i> if you want to require at least one special character in the password.', @dataBool, @Security),
('PictureDirectorySelector', 'Enter the name of the Status Flag search you are using for the Online Picture Directory. See also <a href="https://docs.touchpointsoftware.com/SummaryReports/OnlinePictureDirectory/QuickStart.html" target="_blank">Online Picture Directory</a>.', @dataText, @Administration),
('RegularMeetingHeadCount', 'By default, the value is <i>true</i>. This allows a user to add a head count to a regular meeting. If you want them to be required to change the meeting type from Regular to Headcount, change this to <i>false</i>.', @dataBool, @Administration),
('RequiredBirthYear', 'If you want to require that the year is included when birthdays are entered, set the value to <i>true</i>. If the value is <i>false</i>, the birthday can be entered as month and day only (mm/dd).', @dataBool, @Administration),
('ResetNewVisitorDays', 'The default is <i>180</i> days. You can specify how many days a person can go without attending the class before the system resets their Attend Type back to New Visitor. See also <a href="https://docs.touchpointsoftware.com/Organizations/ResetNewVisitorDays.html" target="_blank">Reset New Visitor Days</a>', @dataInt, @Administration),
('ResetPasswordExpiresHours', 'Controls the length of time a person has to use a link that is sent to them to either set their password for the first time or to reset a password for an existing account. The default is <i>24</i> hours, but you might want to set it to <i>48</i>. We do not recommend setting it for much longer than that. Use an integer for the actual value. See also <a href="https://docs.touchpointsoftware.com/Administration/ForgotUsernamePassword.html" target="_blank">Forgot Password or Username</a>.', @dataInt, @Security),
('RunMorningBatch', 'If you have either created or asked TouchPoint to create a Python script to run each morning, set this to <i>true</i>. See also <a href="https://docs.touchpointsoftware.com/Administration/MorningBatch.html" target="_blank">Morning Batch</a>.', @dataBool, @Administration),
('ShowAltNameOnSearchResults', 'This Setting will display the <i>AltName</i> in Basic Search, Quick Search, PostBundleEdit, as well as in Search Builder results. The <i>AltName</i> will display beside or under the personâ€™s Last Name in the results. This was added to aid Chinese churches that use Chinese characters in the <i>AltName</i> field, but also works for English as well.', @dataBool, @Administration),
('SortCampusByCode', 'If you want to change the default sort order for how the list of Campuses will display, set the value to <i>true</i>. You will then need to set the order in the Lookup Codes. Click in the Code field for each Campus and enter the sort as a number followed by a dash in front of the existing Campus Code. Example: 1-AP, 2-AR, 3-BV, and so on. See also <a href="https://docs.touchpointsoftware.com/Administration/Lookup_Index.html" target="_blank">Lookup Codes</a>.', @dataBool, @Administration),
('StartAddress', 'The value is the street address of your church in this format: 2000 Appling Rd, Cordova, TN 38016. This address is used for Driving Directions.', @dataText, @ChurchInfo),
('StatusFlags', 'By default, the StatusFlags F01,F02,and F03 are in new databases, but each church can configure these to whatever they want. The order that you enter them in this Setting is the order in which they will display on a personâ€™s record. See also <a href="https://docs.touchpointsoftware.com/People/StatusFlags.html" target="_blank">Status Flags</a>.', @dataText, @Administration),
('SubjectAttendanceNotices', 'Enter the text you want as the subject line of the Email Attendance Notices, if you do not want to use the default subject. See also <a href="https://docs.touchpointsoftware.com/Organizations/EmailAttendanceNotices.html" target="_blank">Email Attendance Notices</a>.', @dataText, @Administration),
('TZOffset', 'Enter the number of hours (plus or minus) offset from the Central Time zone. If you are in the Central Time zone, you can leave this blank. If, for example, you are in the Eastern Time zone, enter <i>1</i> for the value.', @dataText, @CheckIn),
('UX-HeaderLogo', 'Enter the URL to the image you wish to use for the logo that displays at the top-left of TouchPoint.', @dataText, @Administration),
('UX-SmallHeaderLogo', 'Enter the URL to the image you wish to use for the small logo that displays at the top-left of TouchPoint.', @dataText, @Administration),
('UseAltnameContains', 'This is specifically to allow searches by one or more Chinese characters of the <i>AltName</i> when searching using Basic Search, Quick Search, and PostBundleEdit. You need this only if you use Chinese characters. By default, a search using English letters allows you to search by using the first few letters of a name. The value should be set to <i>true</i>. See also <a href="https://docs.touchpointsoftware.com/People/AltName.html" target="_blank">Alt Name</a>.', @dataBool, @Administration),
('UseMemberProfileAutomation', 'Set to either <i>true</i> or <i>false</i> depending on whether or not your church wants the membership process to be automated. See also <a href="https://docs.touchpointsoftware.com/People/MembershipProcess.html" target="_blank">Church Membership Process</a>.', @dataBool, @Administration),
('UseRecaptcha', 'Set to <i>true</i> to use ReCAPTCHA for online giving.', @dataBool, @Security),
('ImpersonatePassword', 'Used by Admins and TouchPoint Support to log in as another user in order to troubleshoot issues.', @dataPassword, @Security),
('ChurchBlogUrl', 'This points the Church News link to your own blog for your church database.', @dataText, @ChurchInfo),
('ChurchFeedUrl', 'You can create your own blog for your church database and enter the URL as the value.', @dataText, @ChurchInfo),
('ChurchPhone', 'This phone number will display at the bottom of your TouchPoint database along with the church name and the <b>AdminMail</b> email address. Enter without dashes.', @dataText, @ChurchInfo),
('ChurchWebSite', 'Enter the URL for your churchâ€™s website, if applicable. This can be used in online registrations to return a person to your website.', @dataText, @ChurchInfo),
('TwoFactorAuthEnabled', 'Set to <i>true</i> to enable two-factor authentication (2FA) for login.', @dataBool, @Security),
('TwoFactorAuthRequiredRoles', 'This Setting will let you determine which user roles must have two-factor authentication (2FA) set up in order to log in. Enter the user roles (comma separated) in the <i>Value</i> field for which 2FA is required. See also <a href="https://docs.touchpointsoftware.com/Administration/TwoFactorAuthentication.html" target="_blank">Two-factor Authentication</a>.', @dataText, @Security),
('TwoFactorAuthExpirationDays', 'Determines how many days a two-factor authentication will remain valid, set the <i>Value</i> to the number of days you wish an authentication to last. The default is 30 days. See also <a href="https://docs.touchpointsoftware.com/Administration/TwoFactorAuthentication.html" target="_blank">Two-factor Authentication</a>', @dataInt, @Security),
('EnableSecureSearchFaith', 'Secure Search Faith is a third-party vendor associated with Protect My Ministry that uses the same integration. Set the value to true if you have an account with Secure Search Faith instead of Protect My Ministry.', @dataBool, @ProtectMyMinistry),
('PushPayImportStartDate', 'This sets the earliest date/time when TouchPoint will check for new Pushpay transactions to sync. The batch service automatically sets this, but this can be set to requery Pushpay for older data.', @dataDate, @Pushpay),
('HideGivingTabMyDatausers', 'Set this value to true to hide the Giving tab from MyData users on their record and the records of family members.', @dataBool, @Contributions),
('ShowAllOrgsByDefaultInSearchBuilder', 'Set the value to false to offer Organization options only if Program and Division have been selected in Search Builder conditions that allow for adding Program, Division, and Organization as parameters. With the value set to true, all Organizations will appear in the drop down list of Organizations.', @dataBool, @Administration),
('AttendanceReminder', 'Set the value to true to activate attendance reminders on your database.', @dataBool, @Administration),
('AttendanceReminderSMSOptin', 'Set the value to false if you wish to ignore the SMS Opt-in flag and send attendance reminders as texts to all leaders who have a cell phone number on their record.', @dataBool, @Administration),
('AttendanceShowDescription', 'Set the value to true to display the meeting description on the screen when recording attendance from the attendance reminder link.', @dataBool, @Administration),
('PostContributionPledgeFunds', 'Enter the Fund IDs that you want to show pledge summary for when posting contributions. If you enter multiple Fund IDs, separate them with commas, but no spaces. Example: 301,412.', @dataText, @Contributions),
('SecureProfilePictures', 'For security reasons, viewing profile pictures requires you to log in. Set this value to false to override this security feature and allow leader photos to be displayed on the Small Group Finder (without being logged in).', @dataBool, @SmallGroupFinder),
('EnableWebCheckin', 'Set the value to true to enable the new Web Check-In feature.', @dataBool, @CheckIn),
('Contacts-DefaultRole', 'Contacts will automatically set the LimitToRole option to the specified role.', @dataText, @Contacts)
GO
