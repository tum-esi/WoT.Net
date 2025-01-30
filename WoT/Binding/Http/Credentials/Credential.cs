using System.Net.Http;
using System.Threading.Tasks;

namespace WoT.Binding.Http.Credentials
{
    internal interface ICredential
    {
        void Sign(HttpRequestMessage request);
    }
}
