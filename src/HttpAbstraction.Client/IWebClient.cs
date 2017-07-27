using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttpAbstraction.Client
{
    public interface IWebClient : IDisposable
    {
        Task<TResult> Get<TResult>(string uri, Dictionary<string, string> queryParams = null);

        Task<TResult> Put<TResult>(string uri, object item, Dictionary<string, string> queryParams = null);

        Task<TResult> Patch<TResult>(string uri, object item, Dictionary<string, string> queryParams = null);

        Task<TResult> Post<TResult>(string uri, object item, Dictionary<string, string> queryParams = null);

        Task<TResult> Delete<TResult>(string uri, Dictionary<string, string> queryParams = null);
    }
}
