using Hypercastle.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using TMPro;

namespace Hypercastle.EditorTools
{
    public class AbiUtils : EditorWindow
    {
        public static string terraformFetchIds;
        private static int MAX_TERRAFORM_ID;
        private static HypercastleClient _client;
        
        [MenuItem("Tools/Hypercastle SDK/Abi Utils")]
        static void Open()
        {
            GetWindow<AbiUtils>("Abi Utils");
        }

        TMP_FontAsset fontAsset;
        List<int> chars;
        bool useUint;

        void OnEnable()
        {
            chars = new List<int>(); 
            _client = new HypercastleClient();
            EditorCoroutineUtility.StartCoroutineOwnerless(GetTokenCount());
        }

        private void OnDisable()
        {
            _client = null;
        }

        void OnGUI()
        {
            if (GUILayout.Button("Generate Terraforms ABI"))
            {
                var path = EditorUtility.SaveFilePanelInProject(
                "Save Terraforms Abi",
                "abi",
                "json",
                "Save the ABI locally to your project");
                EditorCoroutineUtility.StartCoroutineOwnerless(FetchAndSerializeAbi(path));
            }

            fontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField(fontAsset, typeof(TMP_FontAsset), false);

            GUI.enabled = fontAsset != null;
            if (GUILayout.Button("Collect Unicodes from Svgs"))
            {
                var path = EditorUtility.OpenFolderPanel("Svg Directory", string.Empty, string.Empty);
                if (path.Length > 0)
                {
                    var files = Directory.GetFiles(path);
                    files = files.Where(file => !file.Contains("meta")).ToArray();

                    foreach (var filePath in files)
                    {
                        try
                        {
                            var content = File.ReadAllText(filePath);
                            var renderData = SvgUtils.Parse(content);
                            var uniqueCharacters = renderData.MainCharSet
                                .Union(renderData.CharSet)
                                .Union(renderData.Unicodes)
                                .Union(renderData.CurrentGlyphs);
                            chars.AddRange(uniqueCharacters);
                        } catch (Exception err)
                        {
                            Debug.Log($"Skipping: {filePath} due to caught error");
                            Debug.LogException(err);
                        }
                    }
                    chars = chars.Distinct().ToList();
                }
            }
            GUI.enabled = true;
            GUI.enabled = false;
            EditorGUILayout.IntField("Distinct Character Count", chars.Count);
            GUI.enabled = true;

            useUint = EditorGUILayout.Toggle("Use Uint", useUint);

            if (GUILayout.Button("Save Distinct Characters"))
            {
                var targetPath = EditorUtility.SaveFilePanelInProject("Save Distinct Characters", "chars", "txt", "Save");
                var stringBuilder = new StringBuilder(chars.Count * 2);

                chars.Sort((x, y) =>
                {
                    return x.CompareTo(y);
                });

                foreach (var unknownChar in chars)
                {
                    var hexValue = useUint ? $"{unknownChar}" : Convert.ToString(unknownChar, 16);
                    stringBuilder.Append(hexValue).Append('\n');
                }

                File.WriteAllText(targetPath, stringBuilder.ToString());
            }

            var tooltip = "Valid input includes any single Terraform as \"1234\", runs of Terraforms as\"X=>XXXX\" where";
            tooltip += "\'=>\' denotes a series of values incremented by one, or any mixture of single values and runs,";
            tooltip += " all values are filtered to only include minted tokens";
            EditorGUILayout.LabelField(new GUIContent("Query Terraform Data By Ids", tooltip));
            terraformFetchIds = GUILayout.TextField(terraformFetchIds);
            
            if (GUILayout.Button("Save Terraform Data"))
            {
                var terraformList = GenerateTerraformList(terraformFetchIds);
                if (terraformList == null) return;
                EditorCoroutineUtility.StartCoroutineOwnerless(FetchTerraformData(terraformList));
            }
        }

        bool ValidTerraformRange(int value)
        {
            return value > 0 && value <= MAX_TERRAFORM_ID;
        }

        List<int> GenerateTerraformList(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                Debug.LogError($"String {ids} is an invalid query");
                return null;
            }
            
            var workingSet = new HashSet<int>();
            var array = ids.Split(",");
            for (var index = 0; index < array.Length; index++)
            {
                if (array[index].Contains("=>"))
                {
                    var subIndexString = array[index].Split("=>");
                    if (int.TryParse(subIndexString[0], out var firstIndex) 
                        && int.TryParse(subIndexString[1], out var secondIndex))
                    {
                        if (firstIndex < secondIndex)
                        {
                            for (; firstIndex <= secondIndex; firstIndex++)
                            {
                                if(ValidTerraformRange(firstIndex)) workingSet.Add(firstIndex);
                            }
                        }
                        else
                        {
                            for (; secondIndex <= firstIndex; secondIndex++)
                            {
                                if(ValidTerraformRange(firstIndex)) workingSet.Add(secondIndex);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"failure to parse strings {subIndexString[0]} and {subIndexString[1]} into integers!");
                    }
                    continue;
                }

                if(int.TryParse(array[index], out var parsedInt))
                {

                    if (ValidTerraformRange(parsedInt)) workingSet.Add(parsedInt);
                }
                else
                {
                    Debug.LogWarning($"Failure to parse string {array[index]} into integer!");
                }
            }

            var result = new List<int>();
            foreach(var item in workingSet)
                result.Add(item);
            return result;
        }

        IEnumerator GetTokenCount()
        {
            if (_client == null)
            {
                Debug.LogWarning("HyperCastleClient failed to initialize");
                yield break;
            }

            yield return EditorCoroutineUtility.StartCoroutineOwnerless(
                _client.LoadContract(ContractAddresses.Terraforms));
                
            var task = _client.GetTotalSupply();

            while (!task.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"<color=cyan>Total Terraforms Minted: </color>{task.Result.TotalSupply}");
            MAX_TERRAFORM_ID = (int) task.Result.TotalSupply;
        }

        IEnumerator FetchAndSerializeAbi(string path)
        {
            yield return _client.LoadContract(ContractAddresses.Terraforms);
            File.WriteAllText(path, _client.Abi);
            AssetDatabase.Refresh();
        }

        IEnumerator FetchTerraformData(List<int>ids)
        {
            if (_client == null)
            {
                Debug.LogWarning("HyperCastleClient failed to initialize");
                yield break;
            }
            
            var targetPath = Application.streamingAssetsPath + "/Terraforms";
            Debug.Log(targetPath);
             
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
             
            var processedIds = 0f;
            var currentWork = string.Empty;
            EditorUtility.DisplayProgressBar("Fetching Terraform Data", currentWork,processedIds/ids.Count);
            Debug.Log($"Starting download data for Terraforms");

            for(var index = 0; index < ids.Count; index++)
            {
                var path2 = $"{targetPath}\\{ids[index]}_height.txt";
                var task2 = _client.GetTokenTerrainValues(ids[index]);
                
                var path = $"{targetPath}\\{ids[index]}.svg";
                var task = _client.GetTokenSVG(ids[index]);
                
                yield return new WaitUntil(() => task2.IsCompleted);
                File.WriteAllText(path2, task2.Result.ToString());
                
                yield return new WaitUntil(() => task.IsCompleted);
                File.WriteAllText(path, task.Result.TokenSVG);
                 
                currentWork = $"Fetching Terraform Data for Token: {ids[index]}";
                processedIds += 1.0f;
                var cancel = EditorUtility.DisplayCancelableProgressBar("Fetching Terraform Data", currentWork,processedIds/ids.Count);
                Debug.Log($"Downloaded data for Terraform: {ids[index]}");

                if (cancel) break;
            }
            
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
}
