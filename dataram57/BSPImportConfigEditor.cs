using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dataram57.BSPImporter
{
    [CustomEditor(typeof(BSPImportConfig))]
    public class BSPImportConfigEditor : Editor
    {
        private BSPImportConfig config;

        private void OnEnable()
        {
            config = (BSPImportConfig)target;
        }

        public override void OnInspectorGUI()
        {
            //tempvars
            string temp;
            float tempf;

            //targetBSPFilePath
            GUILayout.Label("Target BSP file:");
            config.targetBSPFilePath = GUILayout.TextField(config.targetBSPFilePath);
            if (GUILayout.Button("Select BSP file"))
            {
                temp = EditorUtility.OpenFilePanel("Select BSP", "", "bsp");
                if(temp.Length > 0)
                {
                    if (temp != config.targetBSPFilePath) EditorUtility.SetDirty(config);
                    config.targetBSPFilePath = temp;
                }
                    config.targetBSPFilePath = temp;
            }

            //meshScale
            GUILayout.Label("Mesh scale:");
            tempf = EditorGUILayout.FloatField(config.meshScale);
            if (tempf != config.meshScale)
            {
                EditorUtility.SetDirty(config);
                config.meshScale = tempf;
            }

            //materialsAssetsFilePath
            GUILayout.Label("Materials:");
            config.materialsAssetsFilePath = GUILayout.TextField(config.materialsAssetsFilePath);
            if (GUILayout.Button("Select Material Folder"))
            {
                temp = EditorUtility.OpenFolderPanel("Select Material Folder", "", "");
                if (temp.Length > 0) 
                { 
                    temp = ConvertToUnityPath(temp);
                    if (temp != config.materialsAssetsFilePath) EditorUtility.SetDirty(config);
                    config.materialsAssetsFilePath = temp;
                }
            }
        }

        private string ConvertToUnityPath(string path)
        {
            string appBasePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
            if (path.Length > appBasePath.Length)
                if (path.Substring(0, appBasePath.Length) == appBasePath)
                {
                    path = path.Substring(appBasePath.Length);
                    if (path.Substring(0, 1) == "/")
                        path = path.Substring(1);
                    return path;
                }
            if (path.Length == appBasePath.Length)
                return "";
            Debug.LogError("\"" + path + "\" is not inside this project folder.");
            return "";
        }

    }
}