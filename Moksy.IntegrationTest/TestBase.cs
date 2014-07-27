using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moksy.Common;
using Moksy.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.IntegrationTest
{
    [TestClass]
    public class TestBase
    {
        public const int Port = 10011;

        public virtual string Root
        {
            get
            {
                return string.Format("http://localhost:{0}", Port);
            }
        }

        public virtual string GetSimulationResource()
        {
            return string.Format("/{0}", "__Simulation");
        }

        protected RestSharp.IRestResponse Get(string url)
        {
            return Execute(url, RestSharp.Method.GET);
        }

        protected RestSharp.IRestResponse Delete(string url)
        {
            return Execute(url, RestSharp.Method.DELETE);
        }

        protected RestSharp.IRestResponse Delete(string url, List<Header> headers)
        {
            return Execute(url, RestSharp.Method.DELETE, headers);
        }

        protected RestSharp.IRestResponse Execute(string url, RestSharp.Method method)
        {
            return Execute(url, method, new List<Header>());
        }

        protected RestSharp.IRestResponse Execute(string url, RestSharp.Method method, List<Header> headers)
        {
            if (headers == null) headers = new List<Header>();

            RestSharp.IRestClient client = new RestSharp.RestClient(Root);
            RestSharp.IRestRequest request = new RestSharp.RestRequest(url, method);
            foreach (var header in headers)
            {
                request.AddParameter(header.Name, header.Value, RestSharp.ParameterType.HttpHeader);
            }
            request.RequestFormat = RestSharp.DataFormat.Json;
            var response = client.Execute(request);
            return response;
        }

        protected RestSharp.IRestResponse Post(string url, object o)
        {
            var json = JsonConvert.SerializeObject(o);
            return Post(url, json);
        }

        protected RestSharp.IRestResponse Post(string url, string json)
        {
            RestSharp.IRestClient client = new RestSharp.RestClient(Root);
            RestSharp.IRestRequest request = new RestSharp.RestRequest(url, RestSharp.Method.POST);
            request.AddParameter("application/json", json, RestSharp.ParameterType.RequestBody);
            request.RequestFormat = RestSharp.DataFormat.Json;
            var response = client.Execute(request);
            return response;
        }

        protected RestSharp.IRestResponse Put(string url, object o)
        {
            var json = JsonConvert.SerializeObject(o);
            return Put(url, json);
        }

        protected RestSharp.IRestResponse Put(string url, string json)
        {
            RestSharp.IRestClient client = new RestSharp.RestClient(Root);
            RestSharp.IRestRequest request = new RestSharp.RestRequest(url, RestSharp.Method.PUT);
            request.AddParameter("application/json", json, RestSharp.ParameterType.RequestBody);
            request.RequestFormat = RestSharp.DataFormat.Json;
            var response = client.Execute(request);
            return response;
        }

        protected RestSharp.IRestResponse Get(string url, IEnumerable<Header> headers)
        {
            RestSharp.IRestClient client = new RestSharp.RestClient(Root);
            RestSharp.IRestRequest request = new RestSharp.RestRequest(url);
            foreach (var h in headers)
            {
                request.AddHeader(h.Name, h.Value);
            }
            var response = client.Execute(request);
            return response;
        }

        /*             RestSharp.IRestResponse response = null;
            for (int i = 0; i < 3; i++)
            {
                response = client.Execute(request);
                if (response.StatusCode == 0)
                {
                    System.Threading.Thread.Sleep(2000);
                    continue;
                }

                break;
            }
            return response; */
    }
}
