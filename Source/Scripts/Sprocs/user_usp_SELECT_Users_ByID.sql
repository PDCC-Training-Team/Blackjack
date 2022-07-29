CREATE PROCEDURE [user].[usp_SELECT_Users_ByID]
	@UserID int
AS
BEGIN

	SELECT 
		UserID,
		Username,
		UserSoundex,
		Balance
	FROM
		Users
	WHERE
		UserID = @UserID
END