BEGIN TRY
    IF NOT EXISTS (
        SELECT name  
        FROM master.sys.server_principals
        WHERE name = N'sfc'
    )
    BEGIN
        CREATE LOGIN sfc WITH PASSWORD = N'Test1234!';
        EXEC sp_addsrvrolemember @loginame = N'sfc', @rolename = N'sysadmin';
        PRINT 'Login "sfc" created and added to sysadmin role.';
    END
    ELSE
    BEGIN
        PRINT 'Login "sfc" already exists.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error occurred while creating login or assigning role.';
    PRINT ERROR_MESSAGE();
    PRINT ERROR_NUMBER();
    PRINT ERROR_SEVERITY();
    PRINT ERROR_STATE();
    PRINT ERROR_LINE();
END CATCH