-- Installed only by explicit database maintenance, never by workstation startup.
IF SCHEMA_ID('Security') IS NULL EXEC('CREATE SCHEMA [Security] AUTHORIZATION dbo');
IF DATABASE_PRINCIPAL_ID('EGcareClient') IS NULL CREATE ROLE [EGcareClient] AUTHORIZATION dbo;
IF OBJECT_ID('Security.DesktopSchema') IS NULL
    CREATE TABLE [Security].[DesktopSchema](Id int PRIMARY KEY CHECK(Id=1), Version int NOT NULL, LoginPrefix sysname NOT NULL,ContractHash nvarchar(64) NOT NULL DEFAULT '');
IF COL_LENGTH('Security.DesktopSchema','ContractHash') IS NULL ALTER TABLE [Security].[DesktopSchema] ADD ContractHash nvarchar(64) NOT NULL DEFAULT '';
IF OBJECT_ID('Security.DesktopAccounts') IS NULL
    CREATE TABLE [Security].[DesktopAccounts](
        UserId uniqueidentifier PRIMARY KEY, PrincipalName sysname NOT NULL UNIQUE,
        UserName nvarchar(256) NOT NULL DEFAULT '', FirstName nvarchar(100) NOT NULL DEFAULT '',
        LastName nvarchar(100) NOT NULL DEFAULT '', Email nvarchar(256) NULL,
        TimeZoneId nvarchar(100) NOT NULL DEFAULT 'Africa/Cairo',
        IsActive bit NOT NULL DEFAULT 0, IsAdministrator bit NOT NULL DEFAULT 0,
        MustChangePassword bit NOT NULL DEFAULT 1, LockoutEnd datetimeoffset NULL
    );
IF OBJECT_ID('Security.DesktopPermissions') IS NULL
    CREATE TABLE [Security].[DesktopPermissions](UserId uniqueidentifier NOT NULL,Code nvarchar(200) NOT NULL, PRIMARY KEY(UserId,Code));
IF COL_LENGTH('Security.DesktopAccounts','CredentialVersion') IS NULL
    ALTER TABLE [Security].[DesktopAccounts] ADD CredentialVersion nvarchar(256) NOT NULL DEFAULT '';
GO
CREATE OR ALTER PROCEDURE [Security].[RebuildAuthority] WITH EXECUTE AS OWNER AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [Security].[DesktopAccounts] SET IsActive=0;
    UPDATE a SET UserName=u.UserName,FirstName=u.FirstName,LastName=u.LastName,Email=u.Email,
        TimeZoneId=u.TimeZoneId, IsActive=CASE WHEN u.IsActive=1 THEN 1 ELSE 0 END,
        LockoutEnd=u.LockoutEnd,MustChangePassword=u.MustChangePassword,CredentialVersion=COALESCE(u.SecurityStamp,''),
        IsAdministrator=CASE WHEN EXISTS(SELECT 1 FROM [Identity].[AspNetUserRoles] ur
          JOIN [Identity].[AspNetRoles] r ON r.Id=ur.AssignedRolesId
          WHERE ur.UserId=u.Id AND r.Name='Administrator' AND r.IsSystem=1 AND r.IsActive=1) THEN 1 ELSE 0 END
    FROM [Security].[DesktopAccounts] a JOIN [Identity].[AspNetUsers] u ON a.UserId=u.Id;
    DELETE FROM [Security].[DesktopPermissions];
    INSERT [Security].[DesktopPermissions](UserId,Code)
        SELECT DISTINCT a.UserId,p.Code FROM [Security].[DesktopAccounts] a
        JOIN [Identity].[AspNetUserRoles] ur ON ur.UserId=a.UserId
        JOIN [Identity].[AspNetRoles] r ON r.Id=ur.AssignedRolesId AND r.IsActive=1
        JOIN [Identity].[RolePermissions] rp ON rp.RoleId=r.Id
        JOIN [Identity].[AspNetPermissions] p ON p.Id=rp.PermissionsId;
END
GO
CREATE OR ALTER FUNCTION [Security].[CanAccess](@codes nvarchar(max),@ownerOnly bit)
RETURNS TABLE WITH SCHEMABINDING AS RETURN
    SELECT 1 AS Allowed WHERE IS_MEMBER('db_owner')=1 OR EXISTS(
        SELECT 1 FROM [Security].[DesktopAccounts] a WHERE a.PrincipalName=ORIGINAL_LOGIN()
        AND a.IsActive=1 AND (a.LockoutEnd IS NULL OR a.LockoutEnd<=SYSUTCDATETIME())
        AND a.MustChangePassword=0 AND (a.IsAdministrator=1 OR (@ownerOnly=0 AND EXISTS(
            SELECT 1 FROM [Security].[DesktopPermissions] p WHERE p.UserId=a.UserId
            AND CHARINDEX(','+p.Code+',',','+@codes+',')>0))));
GO
CREATE OR ALTER FUNCTION [Security].[CanChange](@update nvarchar(max),@delete nvarchar(max),@isDeleted bit,@ownerOnly bit)
RETURNS TABLE WITH SCHEMABINDING AS RETURN
    SELECT Allowed FROM [Security].[CanAccess](CASE WHEN @isDeleted=1 THEN @delete ELSE @update END,@ownerOnly);
GO
CREATE OR ALTER FUNCTION [Security].[CanWriteCashSession](@codes nvarchar(max),@userId uniqueidentifier,@allowApproval bit)
RETURNS TABLE WITH SCHEMABINDING AS RETURN
    SELECT Allowed FROM [Security].[CanAccess](@codes,0)
    WHERE IS_MEMBER('db_owner')=1 OR EXISTS(SELECT 1 FROM [Security].[DesktopAccounts] a
        WHERE a.PrincipalName=ORIGINAL_LOGIN() AND (a.IsAdministrator=1 OR a.UserId=@userId
            OR (@allowApproval=1 AND EXISTS(SELECT 1 FROM [Security].[DesktopPermissions] p
                WHERE p.UserId=a.UserId AND p.Code='cash.handovers.approve'))));
GO
CREATE OR ALTER FUNCTION [Security].[CanWriteCashEntry](@codes nvarchar(max),@sessionId uniqueidentifier,@allowApproval bit)
RETURNS TABLE WITH SCHEMABINDING AS RETURN
    SELECT Allowed FROM [Security].[CanAccess](@codes,0)
    WHERE IS_MEMBER('db_owner')=1 OR EXISTS(SELECT 1 FROM [Cash].[CashSessions] s
        CROSS APPLY [Security].[CanWriteCashSession](@codes,s.UserId,@allowApproval) scope WHERE s.Id=@sessionId);
GO
CREATE OR ALTER PROCEDURE [Identity].[ReadDesktopPrincipal] WITH EXECUTE AS OWNER AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @id uniqueidentifier=(SELECT UserId FROM [Security].[DesktopAccounts]
        WHERE PrincipalName=ORIGINAL_LOGIN() AND IsActive=1 AND (LockoutEnd IS NULL OR LockoutEnd<=SYSUTCDATETIME()));
    SELECT UserId,UserName,FirstName,LastName,TimeZoneId,Email,IsAdministrator,MustChangePassword,CredentialVersion
        FROM [Security].[DesktopAccounts] WHERE UserId=@id;
    SELECT Code FROM [Security].[DesktopPermissions] WHERE UserId=@id;
    SELECT r.Name FROM [Identity].[AspNetUserRoles] ur JOIN [Identity].[AspNetRoles] r ON r.Id=ur.AssignedRolesId
        WHERE ur.UserId=@id AND r.IsActive=1;
END
GO
CREATE OR ALTER PROCEDURE [Security].[ProvisionDesktopUser] @UserId uniqueidentifier,@Password nvarchar(max)
WITH EXECUTE AS OWNER AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    IF ISNULL(IS_SRVROLEMEMBER('sysadmin',ORIGINAL_LOGIN()),0)<>1 AND NOT EXISTS(
        SELECT 1 FROM [Security].[DesktopAccounts] a WHERE a.PrincipalName=ORIGINAL_LOGIN()
        AND a.IsActive=1 AND a.IsAdministrator=1 AND a.MustChangePassword=0
        AND (a.LockoutEnd IS NULL OR a.LockoutEnd<=SYSUTCDATETIME()))
        THROW 51003,'Only a clinic administrator can provision database identities.',1;
    DECLARE @userName nvarchar(256)=(SELECT UserName FROM [Identity].[AspNetUsers] WHERE Id=@UserId);
    IF @userName IS NULL OR LEN(@userName)>80 OR @userName COLLATE Latin1_General_100_BIN2 LIKE '%[^a-zA-Z0-9_.-]%'
        THROW 51004,'The clinic username cannot be used as a database identity.',1;
    IF (@Password IS NULL OR DATALENGTH(@Password)/2 NOT BETWEEN 12 AND 128 OR @Password COLLATE Latin1_General_100_BIN2 NOT LIKE '%[A-Z]%'
        OR @Password COLLATE Latin1_General_100_BIN2 NOT LIKE '%[a-z]%' OR @Password NOT LIKE '%[0-9]%'
        OR @Password COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^a-zA-Z0-9]%')
        THROW 51004,'A strong password of at least 12 characters is required.',1;
    DECLARE @name sysname=(SELECT LoginPrefix FROM [Security].[DesktopSchema] WHERE Id=1)+@userName;
    IF EXISTS(SELECT 1 FROM [Security].[DesktopAccounts] WHERE PrincipalName=@name AND UserId<>@UserId)
        THROW 51004,'That database identity belongs to another clinic user.',1;
    IF DATABASE_PRINCIPAL_ID(@name) IS NOT NULL AND NOT EXISTS(SELECT 1 FROM [Security].[DesktopAccounts] WHERE UserId=@UserId AND PrincipalName=@name)
        THROW 51004,'An existing database principal cannot be adopted by the application.',1;
    BEGIN TRANSACTION;
    DECLARE @sql nvarchar(max);
    SET @sql=CASE WHEN DATABASE_PRINCIPAL_ID(@name) IS NULL THEN 'CREATE USER ' ELSE 'ALTER USER ' END
            +QUOTENAME(@name)+' WITH PASSWORD=N'+QUOTENAME(@Password,'''')+';';
    IF @sql IS NOT NULL EXEC(@sql);
    IF NOT EXISTS(SELECT 1 FROM sys.database_role_members WHERE role_principal_id=DATABASE_PRINCIPAL_ID('EGcareClient') AND member_principal_id=DATABASE_PRINCIPAL_ID(@name))
    BEGIN SET @sql='ALTER ROLE [EGcareClient] ADD MEMBER '+QUOTENAME(@name); EXEC(@sql); END;
    IF NOT EXISTS(SELECT 1 FROM [Security].[DesktopAccounts] WHERE UserId=@UserId)
        INSERT [Security].[DesktopAccounts](UserId,PrincipalName) VALUES (@UserId,@name);
    EXEC [Security].[RebuildAuthority];
    COMMIT;
END
GO
CREATE OR ALTER PROCEDURE [Security].[ChangeDesktopPassword] @OldPassword nvarchar(max),@NewPassword nvarchar(max),@PasswordHash nvarchar(max)=NULL
WITH EXECUTE AS OWNER AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @id uniqueidentifier,@name sysname;
    SELECT @id=UserId,@name=PrincipalName FROM [Security].[DesktopAccounts]
        WHERE PrincipalName=ORIGINAL_LOGIN() AND IsActive=1 AND (LockoutEnd IS NULL OR LockoutEnd<=SYSUTCDATETIME());
    IF @id IS NULL THROW 51003,'An active clinic identity is required.',1;
    IF @OldPassword IS NULL OR DATALENGTH(@OldPassword)/2 NOT BETWEEN 1 AND 128
        THROW 51004,'The current password is required.',1;
    IF @NewPassword IS NULL OR DATALENGTH(@NewPassword)/2 NOT BETWEEN 12 AND 128 OR @NewPassword COLLATE Latin1_General_100_BIN2 NOT LIKE '%[A-Z]%'
        OR @NewPassword COLLATE Latin1_General_100_BIN2 NOT LIKE '%[a-z]%' OR @NewPassword NOT LIKE '%[0-9]%'
        OR @NewPassword COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^a-zA-Z0-9]%'
        THROW 51004,'A strong password of at least 12 characters is required.',1;
    BEGIN TRANSACTION;
    -- Run ALTER USER as the unprivileged account so OLD_PASSWORD is checked by SQL Server.
    -- The procedure's owner context is restored before updating protected account metadata.
    DECLARE @sql nvarchar(max)='EXECUTE AS USER=N'+QUOTENAME(@name,'''')+'; BEGIN TRY ALTER USER '+QUOTENAME(@name)+' WITH PASSWORD=N'+QUOTENAME(@NewPassword,'''')+' OLD_PASSWORD=N'+QUOTENAME(@OldPassword,'''')+'; REVERT; END TRY BEGIN CATCH REVERT; THROW; END CATCH;';
    EXEC(@sql);
    UPDATE [Identity].[AspNetUsers] SET MustChangePassword=0,SecurityStamp=CONVERT(nvarchar(36),NEWID()),
        PasswordHash=COALESCE(@PasswordHash,PasswordHash) WHERE Id=@id;
    EXEC [Security].[RebuildAuthority];
    COMMIT;
END
GO
GRANT EXECUTE ON [Identity].[ReadDesktopPrincipal] TO [EGcareClient];
GRANT EXECUTE ON [Security].[ProvisionDesktopUser] TO [EGcareClient];
GRANT EXECUTE ON [Security].[ChangeDesktopPassword] TO [EGcareClient];
GRANT SELECT ON [dbo].[__EFMigrationsHistory] TO [EGcareClient];
GRANT SELECT ON [Security].[DesktopSchema] TO [EGcareClient];
GO
CREATE OR ALTER PROCEDURE [Identity].[ReadUserDirectory] WITH EXECUTE AS OWNER AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS(SELECT 1 FROM [Security].[DesktopAccounts] a WHERE a.PrincipalName=ORIGINAL_LOGIN()
        AND a.IsActive=1 AND a.MustChangePassword=0 AND (a.LockoutEnd IS NULL OR a.LockoutEnd<=SYSUTCDATETIME())
        AND (a.IsAdministrator=1 OR EXISTS(SELECT 1 FROM [Security].[DesktopPermissions] p WHERE p.UserId=a.UserId AND p.Code='users.read')))
        THROW 51003,'Permission to read clinic users is required.',1;
    SELECT (SELECT Id,UserName,Email,FirstName,LastName,PhoneNumber,TimeZoneId,IsActive,
        EmailConfirmed,TwoFactorEnabled,LockoutEnabled,LockoutEnd,LastLoginAt,CreatedAt FROM [Identity].[AspNetUsers] FOR JSON PATH) AS Json;
END
GO
CREATE OR ALTER PROCEDURE [Identity].[ReadRoleDirectory] WITH EXECUTE AS OWNER AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS(SELECT 1 FROM [Security].[DesktopAccounts] a WHERE a.PrincipalName=ORIGINAL_LOGIN()
        AND a.IsActive=1 AND a.MustChangePassword=0 AND (a.LockoutEnd IS NULL OR a.LockoutEnd<=SYSUTCDATETIME())
        AND (a.IsAdministrator=1 OR EXISTS(SELECT 1 FROM [Security].[DesktopPermissions] p WHERE p.UserId=a.UserId AND p.Code='roles.read')))
        THROW 51003,'Permission to read clinic roles is required.',1;
    SELECT (SELECT r.Id,r.Name,r.Description,r.IsSystem,r.IsActive,r.CreatedAt,
        JSON_QUERY(COALESCE((SELECT '['+STRING_AGG('"'+STRING_ESCAPE(p.Code,'json')+'"',',')+']'
            FROM [Identity].[RolePermissions] rp JOIN [Identity].[AspNetPermissions] p ON p.Id=rp.PermissionsId WHERE rp.RoleId=r.Id),'[]')) AS Permissions
        FROM [Identity].[AspNetRoles] r FOR JSON PATH) AS Json;
END
GO
CREATE OR ALTER VIEW [Security].[CashUsers] AS
    SELECT u.UserId AS Id,u.FirstName,u.LastName FROM [Security].[DesktopAccounts] u
    WHERE EXISTS(SELECT 1 FROM [Security].[DesktopAccounts] a WHERE a.PrincipalName=ORIGINAL_LOGIN()
        AND a.IsActive=1 AND a.MustChangePassword=0 AND (a.LockoutEnd IS NULL OR a.LockoutEnd<=SYSUTCDATETIME())
        AND (a.UserId=u.UserId OR a.IsAdministrator=1 OR EXISTS(SELECT 1 FROM [Security].[DesktopPermissions] p
            WHERE p.UserId=a.UserId AND p.Code IN ('users.read','cash.sessions.read','cash.handovers.read','cash.handovers.approve'))));
GO
GRANT SELECT ON [Security].[CashUsers] TO [EGcareClient];
GRANT EXECUTE ON [Identity].[ReadUserDirectory] TO [EGcareClient];
GRANT EXECUTE ON [Identity].[ReadRoleDirectory] TO [EGcareClient];
