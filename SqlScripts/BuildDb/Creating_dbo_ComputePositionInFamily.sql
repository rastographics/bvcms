CREATE FUNCTION  [dbo].[ComputePositionInFamily] (@age INT, @married BIT, @fid INT)
RETURNS INT
AS
BEGIN
	DECLARE -- Constants
		@sing BIT = 0,
		@marr BIT = 1,

		@noage INT = -1,
		@adult INT = 1,
		@child INT = 0,

		@returnprimary INT = 10,
		@returnsecondary INT = 20,
		@returnchild int = 30
	DECLARE @pos INT -- return value
	DECLARE @t TABLE
	(
		married BIT,
		childadult INT,
		family VARCHAR(20),
		[pos] INT
	)
	INSERT @t (married, childadult, family, pos) VALUES (@sing, @noage, '2primary', @returnchild)
	INSERT @t (married, childadult, family, pos) VALUES (@sing, @child, '2primary', @returnchild)
	INSERT @t (married, childadult, family, pos) VALUES (@sing, @adult, '2primary', @returnsecondary)

	INSERT @t (married, childadult, family, pos) VALUES (@sing, @noage, '1married', @returnchild)
	INSERT @t (married, childadult, family, pos) VALUES (@sing, @child, '1married', @returnchild)
	INSERT @t (married, childadult, family, pos) VALUES (@sing, @adult, '1married', @returnsecondary)

	INSERT @t (married, childadult, family, pos) VALUES (@sing, @noage, '1single', @returnchild)
	INSERT @t (married, childadult, family, pos) VALUES (@sing, @child, '1single', @returnchild)
	INSERT @t (married, childadult, family, pos) VALUES (@sing, @adult, '1single', @returnsecondary)

	INSERT @t (married, childadult, family, pos) VALUES (@marr, @noage, '2primary', @returnsecondary)
	INSERT @t (married, childadult, family, pos) VALUES (@marr, @child, '2primary', @returnchild)
	INSERT @t (married, childadult, family, pos) VALUES (@marr, @adult, '2primary', @returnsecondary)

	INSERT @t (married, childadult, family, pos) VALUES (@marr, @noage, '1married', @returnprimary)
	INSERT @t (married, childadult, family, pos) VALUES (@marr, @child, '1married', @returnchild)
	INSERT @t (married, childadult, family, pos) VALUES (@marr, @adult, '1married', @returnprimary)

	INSERT @t (married, childadult, family, pos) VALUES (@marr, @noage, '1single', @returnprimary)
	INSERT @t (married, childadult, family, pos) VALUES (@marr, @child, '1single', @returnchild)
	INSERT @t (married, childadult, family, pos) VALUES (@marr, @adult, '1single', @returnprimary)

	INSERT @t (married, childadult, family, pos) VALUES (@sing, @noage, '0primary', @returnprimary)
	INSERT @t (married, childadult, family, pos) VALUES (@sing, @child, '0primary', @returnchild)
	INSERT @t (married, childadult, family, pos) VALUES (@sing, @adult, '0primary', @returnprimary)

	INSERT @t (married, childadult, family, pos) VALUES (@marr, @noage, '0primary', @returnprimary)
	INSERT @t (married, childadult, family, pos) VALUES (@marr, @child, '0primary', @returnchild)
	INSERT @t (married, childadult, family, pos) VALUES (@marr, @adult, '0primary', @returnprimary)

	SELECT @pos = pos 
	FROM @t 
	WHERE dbo.FamilyMakeup(@fid) LIKE family + '%'
	AND @married = married 
	AND (childadult = 
		 CASE 
			WHEN ISNULL(@age, @noage) = @noage THEN @noage 
			WHEN ISNULL(@age, @noage) > 18 THEN @adult 
			ELSE 0 
		 END 
		)

	RETURN @pos

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
