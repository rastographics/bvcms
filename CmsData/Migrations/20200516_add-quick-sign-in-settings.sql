IF NOT EXISTS(SELECT 1 FROM dbo.SettingMetadata WHERE SettingId = 'UseMobileQuickSignInCodes')
BEGIN

DECLARE @Security INT = 4, @dataBool INT = 1, @dataText INT = 3;

    INSERT INTO dbo.SettingMetadata
    (SettingId, Description, DataType, SettingCategoryId) VALUES
    ('UseMobileQuickSignInCodes', 'Set this to <i>true</i> to enable quick mobile app sign in without a password using a 6-digit code. See <a href="https://docs.touchpointsoftware.com/MobileApp/QuickSignIn.html" target="_blank">this help article</a> for more information.', @dataBool, @Security),
    ('MobileQuickSignInCodeSMS', 'This is the SMS message template that will be sent to users when they sign in to the mobile app using quick sign in using their cell phone number. See <a href="https://docs.touchpointsoftware.com/MobileApp/QuickSignIn.html" target="_blank">this help article</a> for more information.', @dataText, @Security),
    ('MobileQuickSignInSubject', 'Set this to customize the subject line of the email for mobile quick sign in codes. See <a href="https://docs.touchpointsoftware.com/MobileApp/QuickSignIn.html" target="_blank">this help article</a> for more information.', @dataText, @Security)
END
GO

IF NOT EXISTS(SELECT 1 FROM dbo.Content WHERE Name = 'MobileQuickSignInCodeEmailBody' AND TypeID=0)
BEGIN
INSERT INTO dbo.Content (Name, Title, TypeID, RoleID, Body) 
VALUES ('MobileQuickSignInCodeEmailBody', 'MobileQuickSignInCodeEmailBody', 0, 0,
'<html><head><title></title></head>
<body>
<h3>Here&#39;s your one-time mobile sign in code for {church}:</h3>
<h4 style="text-align:center;font-family:monospace">{code}</h4>
</body>
</html>')
END
GO