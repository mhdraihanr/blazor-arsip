ALTER DATABASE CHARACTER SET utf8mb4;


CREATE TABLE `FileCategories` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
    `ColorCode` varchar(7) CHARACTER SET utf8mb4 NULL DEFAULT '#007bff',
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    `CreatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_FileCategories` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `FileRecords` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `FileName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `OriginalFileName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `FilePath` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `FileExtension` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `MimeType` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `FileSize` bigint NOT NULL,
    `Description` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `Tags` varchar(500) CHARACTER SET utf8mb4 NULL,
    `Category` varchar(100) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'General',
    `UploadedBy` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `UploadedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `ModifiedAt` datetime(6) NULL,
    `ModifiedBy` varchar(100) CHARACTER SET utf8mb4 NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    `IsPublic` tinyint(1) NOT NULL DEFAULT FALSE,
    `FileHash` varchar(32) CHARACTER SET utf8mb4 NULL,
    `LastAccessedAt` datetime(6) NULL,
    CONSTRAINT `PK_FileRecords` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `Users` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Email` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `PasswordHash` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `PhotoUrl` varchar(500) CHARACTER SET utf8mb4 NULL,
    `IsActive` tinyint(1) NOT NULL DEFAULT TRUE,
    `CreatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `LastLoginAt` datetime(6) NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `FileActivities` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `FileRecordId` int NOT NULL,
    `ActivityType` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
    `PerformedBy` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `PerformedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `IpAddress` varchar(45) CHARACTER SET utf8mb4 NULL,
    `UserAgent` varchar(500) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_FileActivities` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_FileActivities_FileRecords_FileRecordId` FOREIGN KEY (`FileRecordId`) REFERENCES `FileRecords` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `FileVersions` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `FileRecordId` int NOT NULL,
    `VersionFileName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `VersionFilePath` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `VersionNumber` int NOT NULL,
    `FileSize` bigint NOT NULL,
    `ChangeDescription` varchar(500) CHARACTER SET utf8mb4 NULL,
    `CreatedBy` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `CreatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `FileHash` varchar(32) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_FileVersions` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_FileVersions_FileRecords_FileRecordId` FOREIGN KEY (`FileRecordId`) REFERENCES `FileRecords` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


INSERT INTO `FileCategories` (`Id`, `ColorCode`, `CreatedAt`, `CreatedBy`, `Description`, `IsActive`, `Name`)
VALUES (1, '#007bff', TIMESTAMP '2025-09-07 09:30:27.520397', 'System', 'General documents and text files', TRUE, 'Documents'),
(2, '#28a745', TIMESTAMP '2025-09-07 09:30:27.520506', 'System', 'Image files and graphics', TRUE, 'Images'),
(3, '#dc3545', TIMESTAMP '2025-09-07 09:30:27.520506', 'System', 'Video files and multimedia', TRUE, 'Videos'),
(4, '#ffc107', TIMESTAMP '2025-09-07 09:30:27.520506', 'System', 'Audio files and music', TRUE, 'Audio'),
(5, '#6f42c1', TIMESTAMP '2025-09-07 09:30:27.520506', 'System', 'Compressed files and archives', TRUE, 'Archives'),
(6, '#20c997', TIMESTAMP '2025-09-07 09:30:27.520506', 'System', 'Excel and spreadsheet files', TRUE, 'Spreadsheets'),
(7, '#fd7e14', TIMESTAMP '2025-09-07 09:30:27.520506', 'System', 'PowerPoint and presentation files', TRUE, 'Presentations'),
(8, '#6c757d', TIMESTAMP '2025-09-07 09:30:27.520506', 'System', 'Other file types', TRUE, 'Other');


INSERT INTO `Users` (`Id`, `CreatedAt`, `Email`, `IsActive`, `LastLoginAt`, `Name`, `PasswordHash`, `PhotoUrl`)
VALUES (1, TIMESTAMP '2024-01-01 00:00:00', 'admin@company.com', TRUE, NULL, 'Administrator', '$2a$11$8K1p/a0dRTlNqo/x3/Yd4.WdRuBdHdXRf5mGvFlvzeH4p5rEeIXJG', NULL),
(2, TIMESTAMP '2024-01-01 00:00:00', 'user@company.com', TRUE, NULL, 'Regular User', '$2a$11$8K1p/a0dRTlNqo/x3/Yd4.WdRuBdHdXRf5mGvFlvzeH4p5rEeIXJG', NULL),
(3, TIMESTAMP '2024-01-01 00:00:00', 'test@company.com', TRUE, NULL, 'Test User', '$2a$11$8K1p/a0dRTlNqo/x3/Yd4.WdRuBdHdXRf5mGvFlvzeH4p5rEeIXJG', NULL);


CREATE INDEX `IX_FileActivities_ActivityType` ON `FileActivities` (`ActivityType`);


CREATE INDEX `IX_FileActivities_FileRecordId` ON `FileActivities` (`FileRecordId`);


CREATE INDEX `IX_FileActivities_PerformedAt` ON `FileActivities` (`PerformedAt`);


CREATE UNIQUE INDEX `IX_FileCategories_Name` ON `FileCategories` (`Name`);


CREATE INDEX `IX_FileRecords_Category` ON `FileRecords` (`Category`);


CREATE INDEX `IX_FileRecords_FileName` ON `FileRecords` (`FileName`);


CREATE INDEX `IX_FileRecords_IsActive` ON `FileRecords` (`IsActive`);


CREATE INDEX `IX_FileRecords_UploadedAt` ON `FileRecords` (`UploadedAt`);


CREATE INDEX `IX_FileVersions_FileRecordId` ON `FileVersions` (`FileRecordId`);


CREATE INDEX `IX_FileVersions_VersionNumber` ON `FileVersions` (`VersionNumber`);


CREATE UNIQUE INDEX `IX_Users_Email` ON `Users` (`Email`);


