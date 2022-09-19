using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace AmazingAssets.BeastEditor
{
    public static class EditorUtilities
    {
        public static string version = "2021.1";
        public static string assetStorePath = "content/82066";
        public static string assetStorePathShortLink = "http://u3d.as/JxL";
        public static string assetForumPath = "https://forum.unity.com/threads/beast.454483/";
        public static string assetSupportMail = "support@amazingassets.world";

        static string wireframeShaderEditorFolderPath;


        static public string GetWireframeShaderEditorFolderPath()
        {
            if (string.IsNullOrEmpty(wireframeShaderEditorFolderPath))
            {
                wireframeShaderEditorFolderPath = Path.Combine(AmazingAssets.EditorUtility.GetAmazingAssetsFolderPath(), "Beast");
            }

            return wireframeShaderEditorFolderPath;
        }

        static public bool IsPathProjectRelative(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;


            if (path.IndexOf("Assets") == 0)
                return true;


            path = path.Replace("\\", "/").Replace("\"", "/");
            return path.StartsWith(Application.dataPath);
        }

        static public bool ConvertFullPathToProjectRelative(string path, out string newPath)
        {
            if (string.IsNullOrEmpty(path))
            {
                newPath = string.Empty;
                return false;
            }


            if (path.IndexOf("Assets") == 0)
            {
                newPath = path;
                return true;
            }


            path = path.Replace("\\", "/").Replace("\"", "/");
            if (path.StartsWith(Application.dataPath))
            {
                newPath = "Assets" + path.Substring(Application.dataPath.Length);
                return true;
            }
            else
            {
                newPath = path;
                return false;
            }
        }
    }
}
