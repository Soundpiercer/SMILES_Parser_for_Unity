using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Api
{
    public class PrivateApiClient : MonoBehaviour
    {
        private const string ApiEndPoint = "http://15.165.247.54:80/api";

        public static async UniTask<RegisterResponse> RegisterRequest(RegisterRequest request)
        {
            const string requestUrl = ApiEndPoint + "/account/register/";
            var requestContext = new RequestContext(requestUrl, body: request.ToJson());
            return await HttpClient.Post<RegisterResponse>(requestContext);
        }
        
        public static async UniTask<LoginResponse> LoginRequest(LoginRequest request)
        {
            const string requestUrl = ApiEndPoint + "/account/login/";
            var requestContext = new RequestContext(requestUrl, body: request.ToJson());
            return await HttpClient.Post<LoginResponse>(requestContext);
        }
        
        public static async UniTask<PostMolecularResponse> MolecularRequest(string accessToken)
        {
            const string requestUrl = ApiEndPoint + "/molecular/";
            var requestContext = new RequestContext(requestUrl);
            requestContext.Headers.Add("Authoriation",  "Bearer " + accessToken);
            return await HttpClient.Post<PostMolecularResponse>(requestContext);
        }
    }

    [Serializable]
    public class RegisterRequest : IJsonExtensions
    {
        private RegisterRequest(){}

        public RegisterRequest(
            string username,
            string password,
            string password2,
            string email,
            string first_name,
            string last_name
        )
        {
            this.username = username;
            this.password = password; 
            this.password2 = password2;
            this.email = email; 
            this.first_name = first_name;
            this.last_name = last_name;
        }
        
        public string username;
        public string password;
        public string password2;
        public string email;
        public string first_name;
        public string last_name;
    }
    
    [Serializable]
    public class RegisterResponse : IJsonExtensions
    {
        public string username;
        public string email;
        public string first_name;
        public string last_name;
    }
    
    [Serializable]
    public class LoginRequest : IJsonExtensions
    {
        public string username;
        public string password;
        
        private LoginRequest(){}

        public LoginRequest(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }

    [Serializable]
    public class LoginResponse : IJsonExtensions
    {
        public string access;
        public string refresh;
    }
    
    [Serializable]
    public class PostMolecularResponse : IJsonExtensions
    {
        public string owner;
        public string id;
        public string formula;
        public string effect;
        public string others;
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
