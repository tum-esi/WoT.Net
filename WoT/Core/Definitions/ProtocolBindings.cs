using System;
using System.Collections.Generic;
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
        /// <summary>
        /// The protocol's Scheme
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Perform a "read" on the resource with the given URI
        /// </summary>
        /// <param name="form">The form used for the request</param>
        /// <returns>A task that resolves to the <see cref="Content"/> of the request's body</returns>
        Task<Content> ReadResource(Form form);

        /// <summary>
        /// Request a "read" on the resource with the given URI
        /// </summary>
        /// <param name="form">The form used for the request</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the request (timeout, user-defined, etc...)</param>
        /// <returns>A task that resolves to the <see cref="Content"/> of the request's body</returns>
        Task<Content> ReadResource(Form form, CancellationToken cancellationToken);

        /// <summary>
        /// Perform a "write" on the resource with the given URI
        /// </summary>
        /// <param name="form">The form used for the request</param>
        /// <param name="content">The content to be written</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        Task WriteResource(Form form, Content content);

        /// <summary>
        /// Perform a "write" on the resource with the given URI
        /// </summary>
        /// <param name="form">>The form used for the request</param>
        /// <param name="content">The content to be written</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the request (timeout, user-defined, etc...)</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        Task WriteResource(Form form, Content content, CancellationToken cancellationToken);

        /// <summary>
        /// Perform an "invoke" on the resource with the given URI
        /// </summary>
        /// <param name="form">The form used for the request</param>
        /// <returns>A task that resolves to the <see cref="Content"/> of the request's body</returns>
        Task<Content> InvokeResource(Form form);

        /// <summary>
        /// Perform an "invoke" on the resource with the given URI
        /// </summary>
        /// <param name="form">The form used for the request</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the request (timeout, user-defined, etc...)</param>
        /// <returns>A task that resolves to the <see cref="Content"/> of the request's body</returns>
        Task<Content> InvokeResource(Form form, CancellationToken cancellationToken);

        /// <summary>
        /// Perform an "invoke" on the resource with the given URI
        /// </summary>
        /// <param name="form">The form used for the request</param>
        /// <param name="content">The content to be written</param>
        /// <returns>A task that resolves to the <see cref="Content"/> of the request's body</returns>
        Task<Content> InvokeResource(Form form, Content content);

        /// <summary>
        /// Perform an "invoke" on the resource with the given URI
        /// </summary>
        /// <param name="form">The form used for the request</param>
        /// <param name="content">The content to be written</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the request (timeout, user-defined, etc...)</param>
        /// <returns>A task that resolves to the <see cref="Content"/> of the request's body</returns>
        Task<Content> InvokeResource(Form form, Content content, CancellationToken cancellationToken);

        /// <summary>
        /// Subscribe to a resource with the given URI
        /// </summary>
        /// <param name="form">The form used for the request</param>
        /// <param name="nextHandler"></param>
        /// <param name="errorHandler"></param>
        /// <param name="complete"></param>
        /// <returns></returns>
        Task<IProtocolSubscription> SubscribeResource(Form form, Action<Content> nextHandler, Action<Exception> errorHandler = null, Action complete = null);
        Task UnlinkResource(Form form);

        /// <summary>
        /// Requests a single Thing Description from a string representing a Uri
        /// </summary>
        /// <param name="uri">Uri string</param>
        /// <returns>A task which resolves to a <see cref="Content"/> , which has to be deserialized and validated by the upper layers of the implementation.</returns>
        Task<Content> RequestThingDescription(string uri);

        /// <summary>
        /// Requests a single Thing Description from a <see cref="Uri"/>
        /// </summary>
        /// <param name="uri">The URI</param>
        /// <returns>A task which resolves to a <see cref="Content"/> , which has to be deserialized and validated by the upper layers of the implementation.</returns>
        Task<Content> RequestThingDescription(Uri uri);

        /// <summary>
        /// Start the client (ensure it is ready to send requests)
        /// </summary>
        /// <returns>an awaitable Task</returns>
        Task Start();

        /// <summary>
        /// Stop the client
        /// </summary>
        /// <returns>an awaitable Task</returns>
        Task Stop();

        /// <summary>
        /// Apply Security metadata
        /// </summary>
        /// <param name="metadata">Security metadata</param>
        /// <param name="credentials">Credentials for the security scheme</param>
        /// <returns><see langword="true"/> if applying metadata was successful, otherwise <see langword="false"/></returns>
        bool SetSecurity(SecurityScheme[] metadata, Dictionary<CredentialScheme, object> credentials);
    }

    /// <summary>
    /// A factory that creates a corresponding protocol client
    /// </summary>
    public interface IProtocolClientFactory
    {
        string Scheme { get; }
        /// <summary>
        /// Creates a protocol client
        /// </summary>
        /// <returns>a protocol client</returns>
        IProtocolClient GetClient();

        /// <summary>
        /// Runs initialization code for the client factory
        /// </summary>
        /// <returns><see langword="true"/> if initialization was successful, otherwise <see langword="false"/></returns>
        bool Init();

        /// <summary>
        /// Destroys the client factory instance
        /// </summary>
        /// <returns><see langword="true"/> if the instance was destroyed successfully, otherwise <see langword="false"/></returns>
        bool Destroy();
    }

    public interface IProtocolSubscription
    {
        Task Open(Action<Content> next, Action<Exception> onerror = null, Action complete = null);
        void Close();

        bool Closed { get; }
    }

}