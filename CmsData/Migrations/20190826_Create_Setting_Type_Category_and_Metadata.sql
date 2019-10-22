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
      [SettingTypeId] [int] NULL,
      [SettingCategoryId] [int] NULL,
  CONSTRAINT [PK_SettingMetadata] PRIMARY KEY CLUSTERED 
  (
   [SettingId] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
  )
  ALTER TABLE [dbo].[SettingMetadata]  WITH CHECK ADD  CONSTRAINT [FK_SettingMetadata_SettingCategory] FOREIGN KEY([SettingCategoryId])
   REFERENCES [dbo].[SettingCategory] ([SettingCategoryId])
  ALTER TABLE [dbo].[SettingMetadata]  WITH CHECK ADD  CONSTRAINT [FK_SettingMetadata_SettingType] FOREIGN KEY([SettingTypeId])
   REFERENCES [dbo].[SettingType] ([SettingTypeId])
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
WITH st AS (
 SELECT 1 system, 2 features, 3 integrations
)
INSERT INTO dbo.SettingCategory
(SettingCategoryId, Name, SettingTypeId, DisplayOrder) VALUES
(1, 'Administration', (SELECT st.system FROM st), 10),
(2, 'Church Info',    (SELECT st.system FROM st), 20),
(3, 'Contributions',  (SELECT st.system FROM st), 30),
(4, 'Security',       (SELECT st.system FROM st), 40),
(5, 'Check-In',       (SELECT st.features FROM st), 10),
(6, 'Contacts',       (SELECT st.features FROM st), 20),
(7, 'Mobile App',     (SELECT st.features FROM st), 30),
(8, 'Registrations',  (SELECT st.features FROM st), 40),
(9, 'Resources',     (SELECT st.features FROM st), 50),
(10, 'Small Group Finder', (SELECT st.features FROM st), 60),
(11, 'Protect My Ministry',     (SELECT st.features FROM st), 10),
(12, 'Pushpay',  (SELECT st.features FROM st), 20),
(13, 'Rackspace',     (SELECT st.features FROM st), 30),
(14, 'Twilio', (SELECT st.features FROM st), 40);
SET IDENTITY_INSERT dbo.SettingCategory OFF;
GO

INSERT INTO dbo.SettingMetadata
(SettingId, Description, DataType, SettingTypeId, SettingCategoryId) VALUES
('BankDepositFormat', 'This is needed only for those churches using Vanco for online giving. Add this Setting ID and enter vanco as the Setting Value.', 3, 2, 3),
('CampusIds', 'If you are using ShowCampusOnRegistration, you must also add this Setting to indicate which Campuses you want to offer as options for a new record. This applies only to the drop down that displays when a registrant creates a new record during an online registration. Enter the Campus ID #s separated by a comma without any spaces. You can enter all the Campuses or only a select few. See Setting regarding ShowCampusOnRegistration.', 3, 2, 9),
('CheckImagesDisabledForUser', 'Set this to true to hide the check images on a person''s giving tab.', 1, 2, 3),
('ContributionStatementFundDisplayFieldName', 'If you want the Fund Descriptions rather than Fund Names to display on contribution statements, add this setting and give it the value of FundDescription. To revert to the default, remove this setting or change its value to FundName.', 3, 2, 3),
('CustomBundleReport', 'Add this Setting and enter the appropriate value from the Bundle Report or Bundle Report 2 SQL recipes. https://docs.touchpointsoftware.com/CustomProgramming/Python/Scripts/BundleReport.html. https://docs.touchpointsoftware.com/CustomProgramming/Python/Scripts/BundleReport2.html', 3, 2, 3),
('DebitCreditLabel', 'If your Merchant Provider allows you to accept only debit cards and bank accounts and not credit cards for all online transactions, you can add this Setting named DebitCreditLabel and enter what you want the user to see as the value. You might prefer the label to be Debit Card, instead of the default Debit/Credit Card. Note: Be sure to check with your Merchant Provider if you are interested in allowing the use of debit cards, but not credit cards for all online transactions. Some providers may not offer that option. Important: If you want to allow credit cards for all transactions except online giving, see the setting named NoCreditCardGiving.', 3, 2, 3),
('DebitCreditLabel-Giving', 'This setting, similar to DebitCreditLabel, will allow you to define the label displayed, but only on the payment pages for giving transactions.', 3, 2, 3),
('DebitCreditLabel-Registrations', 'This setting, similar to DebitCreditLabel, will allow you to define the label displayed, but only on the payment pages for registration payments.', 3, 2, 3),
('DefaultBundleTypeId', 'You can change the default Bundle Type (the one you start with when creating a new Bundle) by adding this setting code and entering the ID# for the Bundle Type you want as the default. You can find this ID in Administration > Setup > Lookup Codes. See aso Contribution Bundle. https://docs.touchpointsoftware.com/Finance/Bundle_Index.html', 3, 2, 3),
('DefaultCampusFunds-x', 'Use this setting to specify the funds to display on the managed giving page for the specified campus. In the setting, the -x at the end should be replaced with the campus ID. If, for example, your Main campus has ID 1, enter the setting DefaultCampusFunds-1 and, as its value, enter the IDs of the funds you wish to be displayed for that campus. If you enter multiple fund IDs, separate them with commas. Use DefaultCampusFunds-0 to specify funds to display if you do not have multiple campuses, or if a donor has no campus specified on their record.', 3, 2, 3),
('DefaultFundId', 'Add this setting if you want a fund other than Fund ID 1 to be the default fund. When creating a new Bundle, this is the fund that always appears at the top of the list of funds when setting the default for the bundle.', 3, 2, 3),
('DisallowInactiveCheckin', 'If you do not want Inactive Members to be able to check into the org in which they are Inactive, add this setting and set the value as true. This is referring to TouchPoint Self-Check-In. By default, Inactive Members of an organization that uses TouchPoint Check-In will display as enrolled in the organization when they enter their phone number at the check-in kiosk. In other words, they will not have to ‘find’ a class to attend.', 1, 2, 5),
('Feature-ContactExtra', 'Add this Setting with the value = true if you want to use the feature that allows you to add Extra Values to a Contact form. See also Contact Extra Values. https://docs.touchpointsoftware.com/ContactsAndTasks/AddExtraValuesToContact.html', 1, 2, 6),
('GoogleGeocodeAPIKey', 'Copy the value from the key named touchpoint-gmap and paste it here. See Small Group Finder - Embedding a Map. https://docs.touchpointsoftware.com/Organizations/SmallGroupFinderMap.html', 3, 2, 11),
('GoogleMapsAPIKey', 'Copy the value from the key named touchpoint-geocode. See Small Group Finder - Embedding a Map. https://docs.touchpointsoftware.com/Organizations/SmallGroupFinderMap.html', 3, 2, 11),
('LimitRegistrationHistoryToRole', 'Enter the role name required to view the registration history for others.', 3, 2, 9),
('LimitToRolesForContacts', 'This setting will let you determine which user roles will display in the Limit To Role drop down list on the Contact form when a user is recording a Contact. Add this setting and enter (in the Value field) the user roles (comma separated) that you want to display in the drop down. See also Add a Contact. https://docs.touchpointsoftware.com/ContactsAndTasks/AddContact.html', 3, 2, 6),
('MinContributionAmount', 'Set this to minimum dollar amount required for a contribution statement to generate. For example: 5 or 200 Churches that send statements every quarter may set this to a higher amount for the first 3 quarters in order not to send statements to children. Then change it to a lower amount for the yearly statement in order to include everyone.', 3, 2, 3),
('NoCreditCardGiving', 'By default, this is set to false, meaning that donors can use a credit card for online giving. Enter the value true if you want donors to only use their bank account for giving. Note: Even with this setting, online giving for Mission Trip Support and Ask Extra Donations will allow credit card use.', 1, 2, 3),
('NoEChecksAllowed', 'Add this setting and set the value to true if you only want people to use only credit cards for giving and for online registration payments.', 1, 2, 3),
('NoPrintDateOnStatement', 'The default is to display (in the top right corner of the statement) the date it was generated (print date). If you do not want this to display, add this setting and make the value true.', 1, 2, 3),
('NoTitlesOnStatements', 'Add this Setting Code and enter true as the Value if you want the names of joint statements to read as follows: first and last name of the head of household, followed by the first and last name of the spouse. Examples: John Smith and Mary Smith; or Frank Jones and Janet Williams-Jones.', 1, 2, 3),
('NotifyCheckinChanges', 'Add this Setting Code and enter true as the value in order for the New People Manager(s) to receive an email each time someone edits a person’s record during the checkin process. These are like those notifications received when a lay person edits their own record in the desktop application.', 1, 2, 5),
('OnlineContributionBundleDayTime', 'Add this Setting Code and enter a day and time in this format: Sunday 11:00 PM (This is Central TZ), if you want your online bundles to get created once a week instead of every 24 hours. With this setting, a new online bundle will get created after the day/time you specify. So, instead of having one online bundle per day, you would have one per week. See also Online Giving Bundles and Contribution Bundle. https://docs.touchpointsoftware.com/Finance/OnlineGivingBundles.html. https://docs.touchpointsoftware.com/Finance/Bundle_Index.html. Important: This is based on the Central Time Zone, where our TouchPoint servers are located.', 3, 2, 3),
('PrintEnvelopeNumberOnStatement', 'By adding this setting to your database, if you have envelope numbers stored as Extra Values, that envelope number will print on the contribution statement in place of the people ID #, which is the default. Set the value as true. See also Using Envelope Numbers. https://docs.touchpointsoftware.com/Finance/EnvelopeNumber.html', 1, 2, 3),
('RegTimeout', 'This setting determines how long a registration will stay active before timing out. The default is 180000 milliseconds, which is 180 seconds (3 minutes). The timer is reset on every keystroke (not mouse clicks), so in essence the 3 minutes does not start counting until you quit typing. So, if all you are doing it clicking, it could timeout. If you want to change it, enter a number that is bigger or smaller than the default of 180000. Do not use punctuation. Note: You can also use the Timeout setting on a specific organization, in order to set a different timeout just for that online registration. See also Registration Settings Tab. https://docs.touchpointsoftware.com/OnlineRegistration/RegistrationSettings.html', 3, 2, 9),
('RegisterSomeoneElseText', 'Add this Setting and enter whatever text you want to display in place of the default Register Someone Else button just before someone completes their registration. For example: Click to Register Another Person. This affects all online registrations in the database, so be sure to make the text appropriate.', 3, 2, 9),
('RelaxAppAddGuest', 'This setting with a value of true allows an OrgLeaderOnly user to access all records in the database, but only when using the iPhone/Android app to add a guest when taking attendance. See also iPhone / Android Attendance with App. https://docs.touchpointsoftware.com/Organizations/RecordingAttendanceiPhone.html', 1, 2, 8),
('RelaxedReqAdminOnly', 'Add this Setting and set it to true if you want only an Admin user to be able to relax requirements for an Online Registration. With this Setting, other users do not even see those options on the Registration tab.', 1, 2, 9),
('RequireAddressOnStatement', 'Enter false as the value if you want contribution statements to print (when printing All Statements), even if the record has no address. By default, if there is no address, a statement will not generate, except from the individual’s record.', 1, 2, 3),
('RequireCheckNoOnStatement', 'If you want check numbers to print on Contribution Statements, add this Setting Code and enter true as the Value. If you print check numbers, the statements will print only one column on the page, instead of wrapping to make two columns. You can choose to print both the check numbers and the notes (see below). See also Post Bundle - Record Individual Contributions', 1, 2, 3),
('RequireNotesOnStatement', 'If you want notes to print on Contribution Statements,  enter true as the Value. If you print notes, the statements will print only one column on the page, instead of wrapping to make two columns. You can choose to print both the check numbers and the notes (see above). See also Post Bundle - Record Individual Contributions', 1, 2, 3),
('Resources-Enabled', 'Enter a value of true to enable the Resources feature on your database. See also Resources', 1, 2, 10),
('SGF-LoadAllExtraValues', 'Loads all extra values, even if they lack the SGF prefix', 1, 2, 11),
('SGF-OrgTypes', 'To include only organizations of a specified Organization Type in the group finder, enter a value of the desire Organization Type. For example, to include only groups of the type Community Groups, give the setting the value Community Groups.', 3, 2, 11),
('SGF-UseEmbeddedMap', 'Set this to true to embed a map in your Small Group Finder. See Small Group Finder - Embedding a Map', 1, 2, 11),
('SendLinkExpireMinutes', 'Set this to the number of minutes after which a SendLink expires. The default is 30.', 3, 2, 9),
('SendRecurringGiftFailureNoticesToFinanceUsers', 'By default, this is set to false and these notices are sent only to the Online Notify Person(s) on the Online Giving organizations. If you prefer for all users with Finance role to receive these, change the value to true.', 1, 2, 3),
('ShowCampusOnRegistration', 'With this setting added and the value set to true, all new records that are added during an online registration will be presented with a drop down to select a Campus. The registrant will be required to make a selection, unless you check the option on the Registration > Registration tab of the org labeled NotReqCampus. This option only displays if you have ShowCampusOnRegistration in the Settings. when you add this Setting you must also add CampusIds in order to indicate which Campuses you want displayed. See Setting regarding CampusIds.', 1, 2, 9),
('ShowChildInExtraValueParentFamily', 'This setting with the value set to true is used in conjunction with a special Parent Extra Value. This will allow a child (who is not in the family with his non-custodial parent in TouchPoint) to display in the family list of his non-custodial parent when that parent enters his phone number at Check-In. This works only if the child has an Ad Hoc EV named Parent with that non-custodial parent’s PID# as the value. See also Extra Value - Parent', 1, 2, 5),
('ShowPledgeIfMet', 'If your church tracks Pledges, you have the option to display the pledge or not to display it once the pledge has been met. The default is to not show the Pledge if the person has fulfilled his Pledge and the Fund is still Open. Add the setting ShowPledgeIfMet and set the value to true if you want to show the Pledge. Once you close the Fund Pledges will no longer display on the Statement.', 1, 2, 3),
('SortContributionFundsByFieldName', 'Controls how funds are sorted in the drop-down list when you are posting contributions in Post Bundle/Edit mode. Set the value to FundId to sort by ID (number) and to allow fund selection by typing the ID. Set the value to FundName to sort by fund name and to allow fund selection by typing the name of the fund.', 3, 2, 3),
('SpecialGivingFundsHeader', 'Enter the text you want for the header above the special funds as the value, if you do not want to use the default. This is applicable only if your church is using the Special Online Giving funds (those with a sort order of 100 and greater). See also Online Giving', 3, 2, 3),
('StandardFundSetName', 'This applies only to churches that have set up CustomStatements for printing contribution statements for multiple 501c3 organizations. Enter a value to change the name of the standard set from Standard Statements to something else. See also Custom, Contribution, Statements', 3, 2, 3),
('UX-MapPinTextColor', 'Use to set the text color within the SGF map pin (hexadecimal RGB)', 3, 2, 11),
('UX-SGFSortBy', 'Use this setting to specify a particular extra value field to sort the groups in the finder. For example, if you give this setting the value SGF:Neighborhood, the groups will be sorted alphabetically by the values in the SGF:Neighborhood extra value.', 3, 2, 11),
('UseFourDigitCodeForCheckin', 'Add this Setting to your database with the value true if you prefer a 4-digit Security Code instead of the default which is 3 characters, at least 1 number and 1 letter. This refers to the Security Code that prints on the labels for Check-In.', 1, 2, 5),
('UseLabelNameForDonorDetails', 'This Setting with the value = true will display an alternative Donor Details export that has the address fields in separate columns and has a field for Label Name. This name will combine couples if they give jointly. See also Export Donor Details', 1, 2, 3),
('UseSavingAccounts', 'You only need this if the bank for any of your donors requires a flag inside the transaction to let them know it is using a Savings account. This also requires an Ad Hoc Extra value be added to the donor’s record. See also Online Giving - see Savings Accounts for Online Giving section', 1, 2, 3),
('SGF-ExtraValueHost', 'Set the value to the name of an org extra value added to groups in the Small Group Finder to contain the group''s host address.', 3, 2, 11),
('EnableBackgroundChecks', 'This setting must be in the database and set to true in order to use the Protect My Ministry integration.', 1, 3, 12),
('EnableBackgroundLabels', 'Add this setting and and enter true as the Value if you want to use labels from your Protect My Ministry account. You must also add these labels to the Lookups. See also Background Checks with Protect My Ministry. https://docs.touchpointsoftware.com/People/ProtectMyMinistry-BackgroundCh.html', 1, 3, 12),
('PMMPassword', 'By default, this is empty. If you use the integration with Protect My Ministry for background checks, enter your account password as the value. See also Background Check with Protect My Ministry. https://docs.touchpointsoftware.com/People/ProtectMyMinistry-BackgroundCh.html', 4, 3, 12),
('PMMUser', 'By default, this is empty. If you use the integration with Protect My Ministry for background checks, enter your account username as the value.', 3, 3, 12),
('PushPayEnableImport', 'When set to true, this setting enables the nightly uploads of Pushpay transactions.', 1, 3, 13),
('TwilioSID', 'If you are using the integration with Twilio for SMS (text) messaging, you will get this ID# from your Twilio account. Enter it as the value. See also Establish an Account with Twilio', 3, 3, 15),
('TwilioToken', 'If you are using the integration with Twilio for SMS (text) messaging, you will get this token from your Twilio account. Enter it as the value.', 4, 3, 15),
('RackspaceKey', 'API Key for the Rackspace account.', 4, 3, 14),
('RackspaceUrlCDN', 'URL for the Rackspace Container.', 3, 3, 14),
('RackspaceUser', 'Username for the Rackspace account.', 3, 3, 14),
('AdminCoupon', 'This will be in all new databases. You can change the Setting Value and use this as a coupon when testing online registrations.', 3, 1, 1),
('AdminMail', 'This is the email address that will be used for any admin notifications, and from whom standard automated notifications will be sent. See also First Steps with your TouchPoint Database. https://docs.touchpointsoftware.com/GettingStarted/FirstSteps.html', 3, 1, 1),
('AllowLimitToRoleForInvolvementExport', 'This is mainly for churches that have Organizations locked down so only users with a specific role can view them. By adding this setting set to true, staff can run the Involvement report and see these protected orgs in the Activity List even though they may not be able to actually access the org itself. It has a very limited use case.', 1, 1, 1),
('AllowNewGenders', 'Add this Setting with true as the value if you want to be able to add another Gender in the Lookup Codes. If you do add another Gender, if will function as Unknown. We do not recommend that you do this, as Male and Female are used to determine HOH, family labels, and more, but you can choose to do so if you wish.', 1, 1, 1),
('AttendanceUseMeetingCategory', 'Limits the Meeting description to the choices in the Meeting Categories Lookup Table.', 1, 1, 1),
('BlogAppUrl', 'This will be in your database and should not be changed. This points the TouchPoint News link to the TouchPoint News blog on the home page, which is where we notify users of changes and new features.', 3, 1, 1),
('BlogFeedUrl', 'This will be in your database and should not be changed. This presents the TouchPoint News blog articles on the home page, which is where we notify users of changes and new features.', 3, 1, 1),
('CampusLabel', 'Add this Setting and enter the name you want displayed as the label instead of Campus (example: Congregation or Location). Remember, if you add this label, that is what will display in the Lookup Code instead of Campus.', 3, 1, 1),
('CampusRequired', 'If you add this setting, every new record will require a Campus be selected. There is no option for leaving the Campus not specified. However, if you also have the Setting ShowCampusOnRegistration and, for some reason, do not want registrants (who are creating a new people record) to be required to select a Campus during the registration, you can check Not Req Campus on the organization’s Registration > Registration tab. When you select this for a registration the Campus option is displayed for the registrant, but with Optional displaying. See ShowCampusOnRegistration Setting. See also Registration Settings Tab. https://docs.touchpointsoftware.com/OnlineRegistration/RegistrationSettings.html', 1, 1, 1),
('CanOverrideHeadOfHousehold', 'Enter this setting with a value of true to enable an Admin’s ability to override the default Head of Household calculation for any family desired. Once this is enabled, an Admin can promote another family member to Head. See also Position in Family. https://docs.touchpointsoftware.com/People/PositionInFamily.html', 1, 1, 1),
('DbConvertedDate', 'This is automatically populated with the date of your data conversion.', 2, 1, 2),
('DefaultCampusId', 'Enter the ID# (not the name) of the campus to use as the default campus on new records and new organizations. Leave empty if your church does not have more than one campus in the database.', 3, 1, 2),
('DefaultHost', 'This is the URL of your database and will be set for you.', 3, 1, 1),
('DisplayNonTaxOnStatement', 'Add this setting and enter true as the Value if you want all non tax deductible contributions to print on contribution statements. These print in a separate section from the other contributions.', 1, 1, 3),
('ElectronicStatementDefault', 'Add this setting and enter true as the Value to have this flag set automatically for all new records added to the database. See also Electronic Only Statement Option. ElectronicStatementDefault', 1, 1, 3),
('EmailPromptBeforeSend', 'Add this setting with a value of true to add a prompt that will appear after you click Send, asking “Are You Sure?” and giving you an opportunity to cancel or continue.', 1, 1, 1),
('EnableContributionFundsOnStatementDisplay', 'To make custom statements available electronically, so that donors can download them from their profiles, add this setting and give it a value of true. See also Custom Contribution Statements. https://docs.touchpointsoftware.com/Finance/CustomStatements.html', 1, 1, 3),
('EnforceEditCampusRole', 'Add this Setting with the value equal true when you want only those with the user role EditCampus to be able to change/edit the Campus on a person’s record. This applies only to the Campus field on a person’s record, not to setting the Campus for an organization. After adding this Setting, add the User Role EditCampus in the Lookup Codes and assign that role to everyone that should be able to edit the Campus for an individual. Exception: If you have this Setting, a My Data user (and any other user) can edit his and his family’s Campus without having the EditCampus role if you have the Setting MyDataCanEditCampus.', 1, 1, 1),
('EvCommentFields', 'If you have Standard Extra Values that you want to display on the Inreach / Outreach Report, add this Setting Code and enter in the Setting Value the names of the Extra Values you want to display on the report, separated by a comma (no spaces).', 3, 1, 1),
('ExternalManageGivingUrl', 'If your donors give through an external giving service rather than through TouchPoint, add this setting and enter as its value the URL to your recurring giving site. This will configure the link for managed giving on a donor’s Giving tab to point to your external giving service.', 3, 1, 3),
('ExternalOneTimeGiftUrl', 'If your donors give through an external giving service rather than through TouchPoint, add this setting and enter as its value the URL to your one-time giving site. This will configure the link for one-time giving on a donor’s Giving tab to point to your external giving service. See also Online Giving for the section on External Giving Links. https://docs.touchpointsoftware.com/Finance/OnlineGiving.html. https://docs.touchpointsoftware.com/Finance/Onlinegiving.html#external-giving-links', 3, 1, 3),
('HideMemberStatusFromMyData', 'Add this setting with the value = true if you do not want My Data users to be able to view the Profile tab on their own people records or that of their family members. See also New Users and Roles. https://docs.touchpointsoftware.com/Administration/Roles_Index.html', 1, 1, 1),
('HideMyDataMemberBadge', 'Add this setting wih the value = true if you do not want My Data users to see the Member Badge on their people record or the Member Status for immediate family members in the family list. See also New Users and Roles. https://docs.touchpointsoftware.com/Administration/Roles_Index.html', 1, 1, 1),
('MaxExcelRows', 'By default, this is set to 10,000 for Excel exports.', 3, 1, 1),
('MeetingTypesReportIncludeEmpty', 'Set this to true to include empty types on the Meeting Types Report.', 1, 1, 1),
('MenuAddPeople', 'Without this setting, behind the scenes, the default is True. If you add the Setting Code to your database, you can enter false as the Value. This will disable the Add New Person option under People in the header. The button still displays, but the person clicking it will be taken to the help article explaining why it is disabled. See also Add Person in Context. https://docs.touchpointsoftware.com/SearchAdd/AddPeopleContext.html', 1, 1, 1),
('MinimumUserAge', 'This Setting is an age, such as 13. Without this setting, the default is 16. This applies to people trying to create a user account on the church database. For churches that do not capture dates of birth, they may want to add this Setting ID and make the value 0. This will allow a person without a date of birth to create a user account.', 3, 1, 4),
('MonthFirst', 'For dates, this will display the month, then the day. By default, it is set to true.', 1, 1, 1),
('MorningBatchRan', 'If you are using Morning Batch, this will get created automatically and display the last date / time that it ran. See also Morning Batch. https://docs.touchpointsoftware.com/Administration/MorningBatch.html', 2, 1, 2),
('MorningBatchUsername', 'If you need to have special roles (e.g., the Finance role) for any of the Python scripts in your morning batch, add this setting and enter as its value the name of an account with the needed roles. This can be an existing user account, or one you have created specifically for this purpose.', 3, 1, 1),
('MyDataCanEditCampus', 'If you would like for My Data users to be able to edit the Campus on their own people records and that of family members, add this Setting with the value equal true. This also allows any other user to edit his own Campus. See also New Users and Roles - specifically EditCampus role. https://docs.touchpointsoftware.com/Administration/Roles_Index.html', 1, 1, 1),
('NameOfChurch', 'Enter the name as you want it to appear on the bottom of your TouchPoint database.', 3, 1, 2),
('NewMemberClassLabel', 'Enter the text you want to display in place of the New Member Class on the Profile > Member tab.', 3, 1, 1),
('NewPeopleManagerIds', 'Enter the People ID # for the person serving as the New People Manager for your church. You can enter more than one ID#, just separate them with a comma. The first ID in the list, will be the user that owns the New Data Entry tasks. See also People ID Numbers and New People Manager. https://docs.touchpointsoftware.com/People/PeopleIdNumbers.html. https://docs.touchpointsoftware.com/People/NewPeopleManager.html', 3, 1, 1),
('NoBirthYearOverAge', 'This must be used in conjunction with the setting NoBirthYearRole. You can add this setting if you want these users to see the age and complete DOB for those under a specified age. Without this setting, if you use NoBirthYearRole, those aged 18 and under will have their age and complete DOB displayed. You only need this if you want to raise or lower the age for those whose ages/DOB can be seen.', 3, 1, 1),
('NoBirthYearRole', 'Adding this setting to your database will require that you specify a user role whose users should not see an age or a birth year for those records they can view in the database. One use case might be for OrgLeadersOnly. This will allow staff users but not lay leaders to view ages and complete dates of birth. This will mask that information on all on-screen places where a DOB or age appear, such as people lists and people records as well as normal exports and reports. It also masks this information on the mobile app. If the person has a DOB, this user will be able to see the Month and Day. With this Setting all users can see the complete DOB and Age for a child, i.e. anyone aged 18 and under. See next Setting (NoBirthYearOverAge) if you want to change the age.', 3, 1, 1),
('NoNewPersonTasks', 'If this setting is added to a database and set to true the New People Manager will not receive an email when a new people record is created and no data entry task will be created for the user creating the new record. The new records will still have the Member Status of Just Added. See also New People Manager. https://docs.touchpointsoftware.com/People/NewPeopleManager.html', 1, 1, 1),
('PasswordMinLength', 'Use a number, such as 7.', 3, 1, 4),
('PasswordRequireOneNumber', 'Set to true if you want to require at least one number in the password.', 1, 1, 4),
('PasswordRequireOneUpper', 'Set to true if you want to require at least one upper case letter in the password.', 1, 1, 4),
('PasswordRequireSpecialCharacter', 'Set to true if you want to require at least one special character in the password.', 1, 1, 4),
('PictureDirectorySelector', 'Enter the name of the Status Flag search you are using for the Online Picture Directory. See also Online Picture Directory. https://docs.touchpointsoftware.com/SummaryReports/OnlinePictureDirectory/QuickStart.html', 3, 1, 1),
('RegularMeetingHeadCount', 'By default, the value is true. This allows a user to add a head count to a regular meeting. If you want them to be required to change the meeting type from Regular to Headcount, change this to false.', 1, 1, 1),
('RequiredBirthYear', 'If you want to require that the year is included when birthdays are entered, enter the value of true. If its value is false, the birthday can be entered as month and day only (mm/dd).', 1, 1, 1),
('ResetNewVisitorDays', 'The default in the database is 180 days. If you add this setting code, you can specify how many days a person can go without attending the class before the system resets their Attend Type back to New Visitor. See also Reset New Visitor Days', 3, 1, 1),
('ResetPasswordExpiresHours', 'Controls the length of time a person has to use a link that is sent to them to either set their password for the first time or to reset a password for an existing account. The default is 24 hours, but you might want to set it to 48. We do not recommend setting it for much longer than that. Use an integer for the actual value. See also My Data Accounts, Forgot Password or Username', 3, 1, 4),
('RunMorningBatch', 'If you have either created or asked TouchPoint to create a Python script to run each morning, change the Value to true. See also Morning Batch', 1, 1, 1),
('ShowAltNameOnSearchResults', 'This setting will display the AltName in Basic Search, Quick Search, PostBundleEdit, as well as in Search Builder results. The AltName will display beside or under the person’s LastName in the results. This was added to aid Chinese churches that use Chinese characters in the AltName field, but also works for English as well.', 1, 1, 1),
('SortCampusByCode', 'If you want to change the default sort order for how the list of Campuses will display, set the value to true. You will then need to set the order in the Lookup Codes. Click in the Code field for each Campus and enter the sort as a number followed by a dash in front of the existing Campus Code. Example: 1-AP, 2-AR, 3-BV, and so on. See also Lookup Codes', 1, 1, 1),
('StartAddress', 'The value is the street address of your church in this format: 2000 Appling Rd, Cordova, TN 38016 This address is used for Driving Directions.', 3, 1, 2),
('StatusFlags', 'By default, the StatusFlags F01,F02,and F03 are in new databases, but each church can configure these to what they want. The order that you enter them in this setting is the order in which they will display on a person’s record. See also Status Flags', 3, 1, 1),
('SubjectAttendanceNotices', 'Add this setting code and enter as the value, the text you want as the subject line of the Email Attendance Notices, if you do not want to use the default subject. See also Email Attendance Notices. https://docs.touchpointsoftware.com/Organizations/EmailAttendanceNotices.html', 3, 1, 1),
('TZOffset', 'Enter the number of hours (plus or minus) offset from Central time zone. If you are in the Central Time Zone, you can leave this blank. If, for example, you are in the Eastern Time Zone, enter 1 for the value.', 3, 1, 2),
('UX-HeaderLogo', 'Enter the URL to the image you wish to use for the logo that displays at the top-left of TouchPoint.', 3, 1, 1),
('UX-SmallHeaderLogo', 'Enter the URL to the image you wish to use for the small logo that displays at the top-left of TouchPoint.', 3, 1, 1),
('UseAltnameContains', 'This is specifically to allow searches by one or more Chinese characters of the AltName when searching using Basic Search, Quick Search, and PostBundleEdit. You need this only if you use Chinese characters. By default, a search using English letters allows you to search by using the first few letters of a name. The value should be = true. See also Alt Name. https://docs.touchpointsoftware.com/People/AltName.html', 1, 1, 1),
('UseMemberProfileAutomation', 'Enter either true or false depending on whether or not your church wants the membership process to be automated. See also Church Membership Process', 1, 1, 1),
('UseRecaptcha', 'Enter true to use ReCAPTCHA for online giving.', 1, 1, 4),
('ImpersonatePassword', 'Used by Admins and TP Support to log in as another user in order to troubleshoot issues.', 4, 1, 4),
('ChurchBlogUrl', 'This points the Church News link to your own blog for your church database.', 3, 1, 2),
('ChurchFeedUrl', 'You can create your own blog for your church database and enter the URL as the value.', 3, 1, 2),
('ChurchPhone', 'This phone number will display at the bottom of your TouchPoint database along with the church name and the AdminMail email address. Enter without dashes.', 3, 1, 2),
('ChurchWebSite', 'Enter the URL for your church’s website, if applicable. This can be used in online registrations to return a person to your website.', 3, 1, 2)
GO
