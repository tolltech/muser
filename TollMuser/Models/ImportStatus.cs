namespace Tolltech.Muser.Models
{
    public enum ImportStatus
    {
        Ok = 1,
        AlreadyExists = 2,
        NotFound = 3,
        Error = 100, // 0x00000064
    }
}