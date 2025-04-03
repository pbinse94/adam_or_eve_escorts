CREATE   PROCEDURE [dbo].[GetCity] ( @CityId int = null, @StateId int = null)           
AS          
/*          
 EXEC GetCity null, 5
*/          
BEGIN          
 SET NOCOUNT ON;    
      
  SELECT * FROM [City] 
  WHERE StateID = (CASE WHEN @StateId IS NOT NULL THEN @StateId ELSE StateID END) AND
  CityID = (CASE WHEN @CityId IS NOT NULL THEN @CityId ELSE CityID END)   

END 
