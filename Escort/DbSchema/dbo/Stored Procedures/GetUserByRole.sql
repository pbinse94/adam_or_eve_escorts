-- ========================================================
-- Author:	Khushi Saini
-- Create date: 25-Sep.-2023
-- Description:	Get Customer list for Admin
-- ========================================================
CREATE   PROCEDURE [dbo].[GetUserByRole] (
@PageNo int,
@PageSize int,
@SearchKeyword varchar(100),
@SortColumn varchar(100)='',
@SortOrder varchar(100)='',
@Role tinyint
)
AS  
/*  
	EXEC GetUserByRole 1, 10, '', '', 'ASC',2
	SELECT * FROM UserDetail
*/  
BEGIN  
	SET NOCOUNT ON;

	SELECT u.UserId Id, u.[FirstName] +' '+ u.LastName AS [Name], u.Email, u.PhoneNumber, u.ProfileImage, u.IsActive, u.IsDeleted, 
			u.AddedOnUTC CreatedOn, Count(1) Over() AS TotalRecord  
	FROM UserDetail u
	WITH(NOLOCK)
	WHERE u.UserType = @Role AND IsDeleted = 0
	AND (([FirstName] LIKE CASE WHEN LEN(@SearchKeyword)>0 AND @SearchKeyword!='%'  THEN '%'+  @SearchKeyword +'%'   
		                WHEN @SearchKeyword='%' THEN ''  
		        ELSE '%%' END)   
		OR (Email LIKE CASE WHEN LEN(@SearchKeyword)>0 AND @SearchKeyword!='%'  THEN '%'+  @SearchKeyword +'%'   
		                WHEN @SearchKeyword='%' THEN ''  
		        ELSE '%%' END)
		OR (PhoneNumber LIKE CASE WHEN LEN(@SearchKeyword)>0 AND @SearchKeyword!='%'  THEN '%'+  @SearchKeyword +'%'   
						WHEN @SearchKeyword='%' THEN ''  
						ELSE '%%' END)
	)  
	ORDER BY
			CASE WHEN @SortColumn='' AND @SortOrder = 'ASC' THEN u.AddedOnUTC END ASC,
			CASE WHEN @SortColumn='Email' AND @SortOrder = 'ASC' THEN u.Email END ASC,
			CASE WHEN @SortColumn='FirstName' AND @SortOrder = 'ASC' THEN u.[FirstName] END ASC,
			CASE WHEN @SortColumn='LastName' AND @SortOrder = 'ASC' THEN u.[LastName] END ASC,
			CASE WHEN @SortColumn='UserName' AND @SortOrder = 'ASC' THEN u.[FirstName] + ' ' +u.[LastName] END ASC,
			CASE WHEN @SortColumn='PhoneNumber' AND @SortOrder = 'ASC' THEN u.PhoneNumber END ASC,
			CASE WHEN @SortColumn='IsActive' AND @SortOrder = 'ASC' THEN u.IsActive END ASC,
			CASE WHEN @SortColumn='IsDeleted' AND @SortOrder = 'ASC' THEN u.IsDeleted END ASC,
			CASE WHEN @SortColumn='RegisterDate' AND @SortOrder = 'ASC' THEN u.AddedOnUTC END ASC,
			
			CASE WHEN @SortColumn='' AND @SortOrder = 'DESC' THEN u.AddedOnUTC END DESC,
			CASE WHEN @SortColumn='Email' AND @SortOrder = 'DESC' THEN u.Email END DESC,
			CASE WHEN @SortColumn='FirstName' AND @SortOrder = 'DESC' THEN u.[FirstName] END DESC,
			CASE WHEN @SortColumn='LastName' AND @SortOrder = 'DESC' THEN u.[LastName] END DESC,
			CASE WHEN @SortColumn='UserName' AND @SortOrder = 'DESC' THEN u.[FirstName] + ' ' +u.[LastName] END DESC,
			CASE WHEN @SortColumn='PhoneNumber' AND @SortOrder = 'DESC' THEN u.PhoneNumber END DESC,
			CASE WHEN @SortColumn='IsActive' AND @SortOrder = 'DESC' THEN u.IsActive END DESC,
			CASE WHEN @SortColumn='IsDeleted' AND @SortOrder = 'DESC' THEN u.IsDeleted END DESC,
			CASE WHEN @SortColumn='RegisterDate' AND @SortOrder = 'DESC' THEN u.AddedOnUTC END DESC
	

	OFFSET (@PageNo) ROWS
	FETCH NEXT @PageSize ROWS ONLY
END  
