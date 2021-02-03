using System;

namespace Dispatch.Service.License
{
    public interface ILicense
    {
        // TODO: Remote identifier
        DateTime ExpiresAt { get; }
        bool IsExpired { get; }
    }
}
