using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WoT.Core.Definitions
{
    public class Content
    {
        public string type;
        public Stream body;

        public Content(string type, Stream body)
        {
            this.type = type;
            this.body = body;
        }

        public async Task<Byte[]> ToBuffer()
        {
            MemoryStream ms = new MemoryStream();
            await body.CopyToAsync(ms);
            return ms.ToArray();
        }
    }

    /// <summary>
    /// Content object with the default content type (`application/json`).
    /// </summary>
    public class DefaultContent: Content
    {
        public DefaultContent(Stream body): base(ContentSerdes.DEFAULT, body)
        {

        }
    }
}
