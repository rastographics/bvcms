CREATE Function [dbo].[Split](
   @InputText nvarchar(4000), -- The text to be split into rows
      @Delimiter nvarchar(10)) -- The delimiter that separates tokens   .
                           -- Can be multiple characters, or empty

RETURNS @Array TABLE (
   TokenID Int PRIMARY KEY IDENTITY(1   ,1), --Comment out this line if
                                             -- you don't want the
                                          -- identity column
   Value nvarchar(4000))

AS

-----------------------------------------------------------
-- Function Split                                        --
--    • Returns a nvarchar rowset from a delimited string --
-----------------------------------------------------------

BEGIN

   DECLARE
      @Pos Int,        -- Start of token or ch      aracter
      @End Int,        -- End of token
      @TextLength Int, -- Length of input text
      @DelimLength Int -- Length of delimiter

-- Len ignores trailing spaces, thus t   he use of DataLength.
-- Note: if you switch to NVarchar input and out   put, you'll need    to divide by 2.
   SET @TextLength = DataLength(@InputText) / 2

-- Exit function if no text is passed in   
   IF @TextLength = 0 RETURN

         SET @Pos = 1
   SET @DelimLength = DataLength(@Delimiter) / 2

            IF @DelimLength = 0          BEGIN -- Each character in its own row
      WHILE @Pos <= @TextLength BEGIN
         INSERT @Array (Value) VALUES (SubString(@InputText,@Pos,1))
               SET @Pos = @Pos + 1
      END
   END
         ELSE BEGIN
      -- Tack on delimiter to 'see' the last token
      SET @InputText = @InputText + @Delimiter
      -- Find the end character of the first token
      SET @End = CharIndex(@Delimiter, @InputText)
      WHILE @End > 0 BEGIN         
         -- End > 0, a delimiter was found: there is a(nother) token
         INSERT @Array (Value) VALUES (SubString(@InputText, @Pos, @End - @Pos))
         -- Set next search to start after the previous token
         SET @Pos = @End + @DelimLength
         -- Find the end character of the next token
         SET @End = CharIndex(@Delimiter, @InputText, @Pos)
      END
   END
   
   RETURN

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
