using System.IO;
using System.Threading.Tasks;

namespace WoT.Core.Definitions
{
    public class Content
    {
        /// <summary>
        /// The content type of the body
        /// </summary>
        public string type;

        /// <summary>
        /// The body of the content as a stream
        /// </summary>
        public MemoryStream body;

        /// <summary>
        /// Create a new Content object
        /// </summary>
        /// <param name="type">content-type</param>
        /// <param name="body">content as stream</param>
        public Content(string type, MemoryStream body)
        {
            this.type = type;
            this.body = body;
        }

        /// <summary>
        /// Convert the content body to a byte array
        /// </summary>
        /// <returns>a byte array based on the content-type encoding</returns>
        public Task<byte[]> ToBuffer()
        {
            var tsc = new TaskCompletionSource<byte[]>();
            var bytes = body.ToArray();
            tsc.SetResult(bytes);
            return tsc.Task;
        }
    }

    /// <summary>
    /// Content object with the default content type (`application/json`).
    /// </summary>
    public class DefaultContent : Content
    {
        /// <summary>
        /// Create a new DefaultContent object
        /// </summary>
        /// <param name="body">content as stream</param>
        public DefaultContent(MemoryStream body) : base(ContentSerdes.DEFAULT, body)
        {

        }
    }
}
