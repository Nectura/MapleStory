namespace LoginServer.Handlers.Packets.Enums;

public enum ELoginResult : byte
{
    Success = 0,
    Blocked = 3,
    IncorrectPassword = 4,
    Unregistered = 5,
    LoggedIn = 7,
    Error = 8,
    TooManyConnections = 10,
    AdultChannel = 11,
    NotWhitelistedGM = 13,
    AccountVerification = 17,
    TemporaryRegionBlock = 19,
    LicenseAgreement = 23,
    IPBlocked = 28,
    BadGateway = 29,
    PermanentRegionBlock = 32,
    ServerOverburdened = 40,
    ChargebackBlock = 43,
    PasswordChangeRequired = 91
}