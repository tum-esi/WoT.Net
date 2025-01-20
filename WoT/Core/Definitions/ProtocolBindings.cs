using System.Threading;
using System.Threading.Tasks;
using WoT.Core.Definitions.TD;

namespace WoT.Core.Definitions
{
    /// <summary>
    /// Protocol Client Interface. Used to implement different protocol bindings for the WoT Consumer.
    /// </summary>
    public interface IProtocolClient
    {
        string Scheme { get; }
        Task<Content> ReadResource(Form form);
        Task<Content> ReadResource(Form form, CancellationToken cancellationToken);
        Task WriteResource(Form form, Content Content);
        Task WriteResource(Form form, Content Content, CancellationToken cancellationToken);
        Task<Content> InvokeResource(Form form);
        Task<Content> InvokeResource(Form form, CancellationToken cancellationToken);
        Task<Content> InvokeResource(Form form, Content content);
        Task<Content> InvokeResource(Form form, Content content, CancellationToken cancellationToken);

        Task<ThingDescription> RequestThingDescription(string url);

    }


}