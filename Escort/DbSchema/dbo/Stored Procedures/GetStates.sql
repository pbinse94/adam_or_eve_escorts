CREATE   PROCEDURE [dbo].[GetStates] ( @StateId int = null, @CountryId int =null )           
AS          
/*          
 EXEC GetStates null, 1
*/          
BEGIN          
 SET NOCOUNT ON;    
      
  SELECT * FROM [State] 
  WHERE CountryID = (CASE WHEN @CountryId IS NOT NULL THEN @CountryId ELSE CountryID END) AND
  StateID = (CASE WHEN @StateId IS NOT NULL THEN @StateId ELSE StateID END) 
  
END 
