IF((SELECT COUNT(*) 
	FROM [dbo].[PaymentProcess] 
	WHERE [ProcessName] = 'Temporary Recurring Giving') <= 0)
BEGIN
	INSERT INTO [dbo].[PaymentProcess]
        ([ProcessName])
        VALUES
        ('Temporary Recurring Giving');
END