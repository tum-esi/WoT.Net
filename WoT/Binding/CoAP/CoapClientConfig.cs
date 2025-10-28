using System;

namespace WoT.Binding.CoAP
{
    /// <summary>
    /// Configuration options for the CoAP client
    /// </summary>
    public class CoapClientConfig
    {
        /// <summary>
        /// Timeout for CoAP requests in milliseconds
        /// </summary>
        public int Timeout { get; set; } = 30000;

        /// <summary>
        /// Maximum number of retransmissions for confirmable messages
        /// </summary>
        public int MaxRetransmit { get; set; } = 4;

        /// <summary>
        /// Acknowledgement timeout in milliseconds
        /// </summary>
        public int AckTimeout { get; set; } = 2000;

        /// <summary>
        /// Default block size for block-wise transfers (in bytes)
        /// </summary>
        public int DefaultBlockSize { get; set; } = 512;
    }
}
