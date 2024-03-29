using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Api
{
    public class PrivateApiClient : MonoBehaviour
    {
        // warning : Requests towards Django should end with slash!
        private const string ApiEndPoint = "http://15.165.247.54:80/api/";

        public static async UniTask<RegisterResponse> RegisterRequest(RegisterRequest request)
        {
            const string requestUrl = ApiEndPoint + "account/register/";
            request.username = request.username.ToLower();
            request.password = request.password.ToLower();
            request.password2 = request.password2.ToLower();
            var requestContext = new RequestContext(requestUrl, body: request.ToJson());
            return await HttpClient.Post<RegisterResponse>(requestContext);
        }
        
        public static async UniTask<LoginResponse> LoginRequest(LoginRequest request)
        {
            const string requestUrl = ApiEndPoint + "account/login/";
            request.username = request.username.ToLower();
            request.password = request.password.ToLower();
            Debug.Log(request);
            var requestContext = new RequestContext(requestUrl, body: request.ToJson());
            return await HttpClient.Post<LoginResponse>(requestContext);
        }
        
        public static async UniTask<PostMolecularResponse> MolecularRequest(string accessToken)
        {
            const string requestUrl = ApiEndPoint + "molecular/";
            var requestContext = new RequestContext(requestUrl);
            requestContext.Headers.Add("Authoriation",  "Bearer " + accessToken);
            return await HttpClient.Post<PostMolecularResponse>(requestContext);
        }

        public static async UniTask<TestPostMolecularResponse> TestMolecularRequest(TestPostMolecularRequest request)
        {
            const string requestUrl = ApiEndPoint;
            var requestContext = new RequestContext(requestUrl, body: request.ToJson());

            return await HttpClient.Post<TestPostMolecularResponse>(requestContext);
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

    [Serializable]
    public class TestPostMolecularRequest : IJsonExtensions
    {
        public string keyword;

        private TestPostMolecularRequest() { }

        public TestPostMolecularRequest(string keyword)
        {
            this.keyword = keyword;
        }
    }

    [Serializable]
    public class TestPostMolecularResponse : IJsonExtensions
    {
        public string keyword;
        public string[] data;
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
