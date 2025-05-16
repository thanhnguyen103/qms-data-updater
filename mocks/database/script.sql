-- ==========================================================================================
-- Create Table Script for UserCertificates
-- Description: Stores information about certificates issued to users.
-- ==========================================================================================

-- Drop the table if it already exists (optional, use with caution in production)
-- IF OBJECT_ID('dbo.UserCertificates', 'U') IS NOT NULL
-- BEGIN
--     DROP TABLE dbo.UserCertificates;
-- END
-- GO

CREATE TABLE dbo.UserCertificates (
    -- Primary Key for the UserCertificate record
    UserCertificateGUID UNIQUEIDENTIFIER NOT NULL,

    -- Foreign key referencing a specific certificate definition
    CertificateGUID UNIQUEIDENTIFIER NOT NULL,

    -- Foreign key referencing the user to whom the certificate is issued
    UserGUID UNIQUEIDENTIFIER NOT NULL,

    -- License number associated with the certificate, if applicable
    licenseNumber VARCHAR(100) NULL,

    -- Country that issued the certificate
    issuingCountry VARCHAR(100) NULL,

    -- Grade or level achieved for the certificate
    grade VARCHAR(50) NULL,

    -- Name or identifier of the trainer or training institution
    trainer VARCHAR(255) NULL,

    -- Number of hours completed for the certificate (stored as text)
    hours VARCHAR(20) NULL,

    -- Flag indicating if this certificate requires approval
    requireApproval BIT NOT NULL DEFAULT 0, -- 0 = No, 1 = Yes

    -- Flag indicating if this is an official certificate
    isOfficial BIT NOT NULL DEFAULT 0, -- 0 = No, 1 = Yes

    -- Additional notes or comments about the certificate
    notes NVARCHAR(1000) NULL,

    -- Timezone relevant to the certificate issuance or completion
    timeZone NVARCHAR(100) NULL,

    -- Type of suspension, if the certificate is suspended
    suspensionType VARCHAR(100) NULL,

    -- Date when the certificate was completed
    completedDate DATETIME2(3) NULL,

    -- Date when the certificate expires
    expiryDate DATETIME2(3) NULL,

    -- Flag indicating if this is the latest certificate of its type for the user
    isLatest BIT NOT NULL DEFAULT 0, -- 0 = No, 1 = Yes

    -- Date and time when this record was created
    createdDate DATETIME2(3) NOT NULL DEFAULT GETUTCDATE(),

    -- Date and time when this record was soft-deleted (if applicable)
    deletedDate DATETIME2(3) NULL,

    -- Date and time when this record was last updated
    lastUpdatedDate DATETIME2(3) NOT NULL DEFAULT GETUTCDATE(),

    -- Define the primary key constraint
    CONSTRAINT PK_UserCertificates PRIMARY KEY CLUSTERED (UserCertificateGUID ASC)
);
GO

-- ==========================================================================================
-- Add Indexes (Optional but Recommended for Performance)
-- ==========================================================================================

-- Index on UserGUID for faster lookups by user
CREATE NONCLUSTERED INDEX IX_UserCertificates_UserGUID
ON dbo.UserCertificates (UserGUID);
GO

-- Index on CertificateGUID for faster lookups by certificate type
CREATE NONCLUSTERED INDEX IX_UserCertificates_CertificateGUID
ON dbo.UserCertificates (CertificateGUID);
GO

-- Index on completedDate for querying certificates by completion date
CREATE NONCLUSTERED INDEX IX_UserCertificates_completedDate
ON dbo.UserCertificates (completedDate);
GO

-- Index on expiryDate for querying certificates by expiry date
CREATE NONCLUSTERED INDEX IX_UserCertificates_expiryDate
ON dbo.UserCertificates (expiryDate);
GO

-- ==========================================================================================
-- Add Comments to Table and Columns (Optional but good for documentation)
-- ==========================================================================================
/*
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Stores information about certificates issued to users, linking users to specific certificates they have obtained.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Primary Key for the UserCertificate record (GUID).' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'UserCertificateGUID';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Foreign key referencing a specific certificate definition (e.g., from a Certificates table).' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'CertificateGUID';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Foreign key referencing the user to whom the certificate is issued (e.g., from a Users table).' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'UserGUID';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'License number associated with the certificate, if applicable.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'licenseNumber';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Country that issued the certificate.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'issuingCountry';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Grade or level achieved for the certificate.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'grade';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Name or identifier of the trainer or training institution.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'trainer';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Number of hours completed for the certificate (stored as text).' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'hours';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Flag indicating if this certificate requires approval (0 = No, 1 = Yes).' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'requireApproval';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Flag indicating if this is an official certificate (0 = No, 1 = Yes).' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'isOfficial';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Additional notes or comments about the certificate.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'notes';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Timezone relevant to the certificate issuance or completion.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'timeZone';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Type of suspension, if the certificate is suspended.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'suspensionType';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Date when the certificate was completed.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'completedDate';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Date when the certificate expires.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'expiryDate';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Flag indicating if this is the latest certificate of its type for the user (0 = No, 1 = Yes).' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'isLatest';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Date and time when this record was created (UTC).' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'createdDate';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Date and time when this record was soft-deleted (if applicable, UTC).' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2nameN'deletedDate';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Date and time when this record was last updated (UTC).' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserCertificates', @level2type=N'COLUMN',@level2name=N'lastUpdatedDate';
GO
*/
