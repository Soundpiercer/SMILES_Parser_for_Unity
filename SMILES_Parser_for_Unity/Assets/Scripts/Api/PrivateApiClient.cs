using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Api
{
    public static class PrivateApiClient
    {
        private const string ApiEndPoint = "localhost:8080";
        
        public static async UniTask<PostTargetDataResponse> PostTargetData(PostTargetDataRequest request)
        {
            const string requestUrl = ApiEndPoint + "sample";
            var requestContext = new RequestContext(requestUrl, body: request.ToJson());
            return await HttpClient.Post<PostTargetDataResponse>(requestContext);
        }
    }
    
    [Serializable]
    public class PostTargetDataRequest : IJsonExtensions
    {
    }
        
    [Serializable]
    public class PostTargetDataResponse : IJsonExtensions
    {
        public string data;
    }
    
    public interface IJsonExtensions {}
    
    public static class JsonExtensions
    {
        public static string ToJson(this IJsonExtensions param)
        {
            return JsonUtility.ToJson(param);
        }
    }
}
