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
        public byte[] body;

        public Content(string type, byte[] body)
        {
            this.type = type;
            this.body = body;
        }
    }

    /// <summary>
    /// Content object with the default content type (`application/json`).
    /// </summary>
    public class DefaultContent: Content
    {
        public DefaultContent(byte[] body): base(ContentSerdes.DEFAULT, body)
        {

        }
    }
}
