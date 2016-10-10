namespace WDAdmin.Domain.Entities
{
    /// <summary>
    /// Enum helper for logging system
    /// </summary>
    public enum LogType
    {
        Ok = 0,

        LoginOk = 101,
        LoginError = 102,

        PasswordResetOk = 201,
        PasswordResetError = 202,
        PasswordChangeOk = 203,
        PasswordChangeError = 204,

        DbCreateOk = 301,
        DbCreateError = 302,
        DbUpdateOk = 303,
        DbUpdateError = 304,
        DbDeleteOk = 305,
        DbDeleteError = 306,
        DbQueryError = 307,

        ServiceTrainingCheckOk = 401,
        ServiceTrainingCheckError = 402,
        ServiceBillingOk = 403,
        ServiceBillingError = 404,

        CustomerBillingOk = 501,
        CustomerBillingError = 502,

        PostTrainingReportOk = 601,
        PostTrainingReportError = 602,
        UpdateStatusLogOk = 603,
        UpdateStatusLogError = 604,
        LogEntryCreateError = 605,

        MailSendOk = 701,
        MailSendError = 702,

        UnauthorizedAccess = 801,
        UnauthenticatedAccess = 802,

        UsergroupFromUgroup = 901,
        UsergroupFromAgroup = 902,
        UsergroupChange = 903,

        FileCreateError = 1001,
        FileReadError = 1002,
        FileCreateOk = 1003,

        JsonStringReceived = 1101,
        JsonError = 1102,

        SessionExpired = 1201,

        InvalidCulture = 1301,

        TEST = 9999,

        UncaughtException = -1
    }
}