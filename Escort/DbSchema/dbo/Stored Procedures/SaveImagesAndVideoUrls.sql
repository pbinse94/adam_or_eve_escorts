ALTER PROCEDURE SaveImagesAndVideoUrls    
    @EscortId INT,
    @FileName VARCHAR(200),    
    @MediaType TINYINT
AS
BEGIN
    
        -- Insert new record
        INSERT INTO EscortGallery (EscortId, [FileName], MediaType)
        VALUES (@EscortId, @FileName, @MediaType);

	--IF NOT EXISTS(SELECT 1 FROM EscortGallery WHERE EscortID = @EscortId)
 --   BEGIN
 --   END
 --   ELSE
 --   BEGIN
 --       -- Update existing record
 --       UPDATE EscortGallery
 --       SET EscortId = @EscortId,
 --           [FileName] = @FileName,                        
 --           MediaType = @MediaType
 --       WHERE EscortID = @EscortId;
 --   END;
END;
