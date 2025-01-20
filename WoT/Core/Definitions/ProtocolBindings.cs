using System.IO;
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
        Task<Stream> ReadResource(Form form);
        Task<Stream> ReadResource(Form form, CancellationToken cancellationToken);
        Task WriteResource<T>(Form form, T value);
        Task<Stream> InvokeResource(Form form);
        Task<Stream> InvokeResource<U>(Form form, U parameters);

        Task<ThingDescription> RequestThingDescription(string url);

    }


}