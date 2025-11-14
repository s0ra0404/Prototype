using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorExpansion
{
    public class AssetInporter : EditorWindow
    {
        [SerializeField] private VisualTreeAsset assetInporterWindow;
        [SerializeField] private VisualTreeAsset dataBoxUI;
    
        private AddRequest addRequest;
        private VisualElement _root;
        private const string FilePath = "Assets/Editor/AssetImporter/AssetData.json";
        
        [MenuItem("Utils/AssetInporter")]
        public static void ShowExample()
        {
            AssetInporter wnd = GetWindow<AssetInporter>();
            wnd.titleContent = new GUIContent("AssetInporter");
        }
        
        [Serializable]
        private class AssetDatas
        {
            public List<AssetData> datas;
        }

        [Serializable]
        private class AssetData
        {
            public string title;
            public string url;

            public AssetData(string title, string url)
            {
                this.title = title;
                this.url = url;
            }
        }

        private void Awake()
        {
            _root = rootVisualElement;
            
            // URLWindow の UI をルートに追加
            _root.Add(assetInporterWindow.Instantiate());
            
            if (!File.Exists(FilePath))
            {
                JsonManager.SaveData(new AssetDatas(), FilePath);
            }
            else
            {
                ViewAssetDatas();
            }
        }

        public void CreateGUI()
        {
            if(_root == null) return;
            
            _root.Q<Button>("SaveButton").clicked += () =>
            {
                SaveAssetData();
                ViewAssetDatas();
            };
            _root.Q<Button>("ImportButton").clicked += () =>
            {
                foreach (var url in GetSelectedUrls())
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        ImportPackage(url); // ← ここで非同期チェック付きの呼び出しを使う
                    }
                    else
                    {
                        Debug.LogWarning("Please enter a valid Git URL.");
                    }
                }
            };
        }
        
        private void ImportPackage(string url)
        {
            addRequest = Client.Add(url);
            EditorApplication.update += Progress;
        }

        private void Progress()
        {
            if (addRequest == null) return;

            if (addRequest.IsCompleted)
            {
                if (addRequest.Status == StatusCode.Success)
                {
                    Debug.Log($"Success: Imported package {addRequest.Result.packageId}");
                }
                else if (addRequest.Status >= StatusCode.Failure)
                {
                    Debug.LogError($"Error: {addRequest.Error.message}");
                }

                addRequest = null;
                EditorApplication.update -= Progress;
            }

            ViewAssetDatas();
        }
        
        private void SaveAssetData()
        {
            var data = JsonManager.LoadData<AssetDatas>(FilePath);
            
            var titleText = _root.Q<TextField>("TitleText").value;
            var urlText = _root.Q<TextField>("URLText").value;
            
            data.datas.Add(new AssetData(titleText, urlText));
            
            JsonManager.SaveData(data, FilePath);
        }
        
        private void ViewAssetDatas()
        {
            var scrollView = _root.Q<ScrollView>("ScrollView");
            
            scrollView.Clear();
            
            var data = JsonManager.LoadData<AssetDatas>(FilePath);
            
            foreach (var assetData in data.datas)
            {
                var dataBox = dataBoxUI.Instantiate();
                dataBox.Q<Toggle>("Toggle").value = false;
                dataBox.Q<Label>("Title").text = assetData.title;
                dataBox.Q<Label>("URL").text = assetData.url;
                
                scrollView.Add(dataBox);
            }
        }
        
        private List<string> GetSelectedUrls()
        {
            var scrollView = _root.Q<ScrollView>("ScrollView");
            var selectedUrls = new List<string>();

            foreach (var child in scrollView.Children())
            {
                var toggle = child.Q<Toggle>("Toggle");
                if (toggle != null && toggle.value)
                {
                    var urlLabel = child.Q<Label>("URL");
                    if (urlLabel != null)
                    {
                        selectedUrls.Add(urlLabel.text);
                    }
                }
            }

            return selectedUrls;
        }
    }
}

    
    



