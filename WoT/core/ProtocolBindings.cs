using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WoT.Definitions;
namespace WoT.ProtocolBindings
{

    public interface IProtocolClient
    {
        string Scheme { get; }
        Task<Stream> SendGetRequest(Form form);
        Task<Stream> SendGetRequest(Form form, CancellationToken cancellationToken);
        Task<Stream> SendPostRequest(Form form);
        Task<Stream> SendPostRequest<U>(Form form, U parameters);
        Task SendPutRequest<T>(Form form, T value);

        Task<ThingDescription> RequestThingDescription(string url);

    }


}