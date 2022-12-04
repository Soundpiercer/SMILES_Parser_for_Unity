using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace Api
{
    public static class HttpClient
    {
        private const double ApiTimeout = 10;

        public static async UniTask Get(RequestContext context)
        {
            context.Method = "GET";
            await Request(context);
        }
        
        public static async UniTask Post(RequestContext context)
        {
            context.Method = "POST";
           await Request(context);
        }

        public static async UniTask<T> Get<T>(RequestContext context)
        {
            context.Method = "Get";
            return await Request<T>(context);
        }
        
        public static async UniTask<T> Post<T>(RequestContext context)
        {
            context.Method = "POST";
            return await Request<T>(context);
        }

        private static async UniTask Request(RequestContext context)
        {
            await Request<string>(context);
        }

        private static async UniTask<T> Request<T>(RequestContext context)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(ApiTimeout));

            var request = new UnityWebRequest(context.Url, context.Method);

            request.downloadHandler = new DownloadHandlerBuffer();
            if (!string.IsNullOrEmpty(context.Body))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(context.Body);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.SetRequestHeader("Content-Type", "application/json");
            }

            if (context.Headers != null)
            {
                foreach (var header in context.Headers)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            try
            {
                var res = await request.SendWebRequest().WithCancellation(cts.Token);

                // TODO : no problem with JsonUtility.FromJson Deserialization, but causes infinite loop while returning
                // returning default does not make infinite loop
                return JsonUtility.FromJson<T>(res.downloadHandler.text);
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {
                    Debug.Log("Timeout");
                    return await Request<T>(context);
                }
            }

            return default;
        }
    }
    
    public class RequestContext
    {
        internal string Method;
        public string Url { get; private set; }
        public Headers Headers { get; private set; }
        public string Body { get; private set; }
        public RequestContext(string url, Headers headers = null, string body = null)
        {
            Url = url;
            Headers = headers;
            Body = body;
        }
    }
        
    public class Headers : Dictionary<string, string>
    {
        
    }
}