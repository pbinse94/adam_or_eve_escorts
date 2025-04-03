CREATE   PROCEDURE [dbo].[GetCountry] ( @Id int = null )           
AS          
/*          
 EXEC GetCountry
*/          
BEGIN          
 SET NOCOUNT ON;    
      
  SELECT * FROM Country WHERE CountryID = (CASE WHEN @Id IS NOT NULL THEN @Id ELSE CountryID END)
END 
