using Milky.Settings;
using MilkyNet;
using System;
using System.Text;

namespace Milky.Utils
{
    public class RequestUtils
    {
        private ListUtils _listUtils;
        private RunSettings _runSettings;

        public MilkyRequest SetProxy(MilkyRequest request, string proxy = null, string protocol = null, int timeout = -1)
        {
            _listUtils = ListUtils.GetOrNewInstance();
            _runSettings = RunSettings.GetOrNewInstance();

            int _timeout = timeout != -1 ? timeout : _runSettings.proxyTimeout;

            request.Proxy = proxy ?? _listUtils.GetRandomProxy();

            request.Type = protocol ?? _runSettings.proxyProtocol;

            request.ConnectTimeout = _timeout;
            request.KeepAliveTimeout = _timeout;
            request.ReadWriteTimeout = _timeout;

            return request;
        }

        public MilkyResponse Execute(MilkyRequest request, HttpMethod method, string url, string payload = null)
        {
            return request.Start(method, new Uri(url), new BytesContent(payload != null ? Encoding.UTF8.GetBytes(payload) : new byte[0]));
        }

        private static RequestUtils _classInstance;
        public static RequestUtils GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new RequestUtils());
        }
    }
}