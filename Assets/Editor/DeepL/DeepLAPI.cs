#if My_Utils

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace EditorExpansion
{
    public static class DeepLAPI
    {
        private const string Endpoint = "https://api-free.deepl.com/v2/translate?auth_key=";
        private static string _apiKey;
        public static void SetApiKey(string apiKey) => _apiKey = apiKey;
        
        /// <summary>
        /// 設定言語
        /// </summary>
        public enum Language
        {
            [InspectorName("Japanese")] Ja,
            [InspectorName("English")] En,
            [InspectorName("Korean")] Ko,
            [InspectorName("Chinese")] Zh
        }
        
        /// <summary>
        /// レスポンスを格納する構造体
        /// </summary>
        [Serializable]
        public struct TranslateData
        {
            public Translations[] translations;
            
            [Serializable]
            public struct Translations
            {
                public string detectedSourceLanguage;
                public string text;
            }
        }
        
        public static async UniTask<string> SendTranslationRequestAsync(string requestUrl)
        {
            using var request = UnityWebRequest.PostWwwForm(requestUrl, "Post");
            var result = await request.SendWebRequest();
    
            if (request.result == UnityWebRequest.Result.Success)
            {
                var json = result.downloadHandler.text;
                var data = JsonUtility.FromJson<TranslateData>(json);
                return data.translations[0].text;
            }
    
            return HandleRequestError(request);
        }
        
        public static string BuildRequestUrl(Language from, Language to, string text)
        {
            var requestInfo = Endpoint + _apiKey;
            requestInfo += $"&text={Uri.EscapeDataString(text)}&source_lang={from}&target_lang={to}";
            return requestInfo;
        }
        
        private static string HandleRequestError(UnityWebRequest request)
        {
            return request.responseCode switch
            {
                >= 500 => "Server Error",
                >= 400 => "Rate Limit Exceeded",
                _ => $"Error: {request.error}"
            };
        }
    }
}
#endif
