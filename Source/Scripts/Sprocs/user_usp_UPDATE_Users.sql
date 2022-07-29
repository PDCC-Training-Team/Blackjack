CREATE PROCEDURE [user].[usp_UPDATE_User_ByID]
	@UserID int,
	@Username varchar(50),
	@Balance int
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
		UPDATE Users
		SET Username = @Username, Balance = @Balance
		WHERE UserID = @UserID

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0
			BEGIN 
				ROLLBACK TRANSACTION
				PRINT('An error has occurred. The update transaction has been rolled back.')
			END;
		THROW
	END CATCH

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