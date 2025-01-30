using System;

namespace WoT.Core.Definitions
{
    /// <summary>
    /// Enumerates the different types of credentials that can be used in a request
    /// </summary>
    [Flags]
    public enum CredentialScheme : byte
    {
        NoSec = 0b0000_0000,
        Basic = 0b0000_0001,
        Digest = 0b0000_0010,
        Apikey = 0b0000_0100,
        Bearer = 0b0000_1000,
        Psk = 0b0001_0000,
        OAuth2 = 0b0010_0000,
    }
    /// <summary>
    /// Describes a credential that can be used in a request
    /// </summary>
    public class CredentialDescription
    {
        public CredentialScheme Scheme { get; set; }
        public object Credential { get; set; }
    }
}
