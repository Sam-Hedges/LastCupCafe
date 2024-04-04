﻿using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GUIStreamline
{
    public static class DrawerUtils
    {
        public static Texture2D LoadSubAsset(string path, string name)
        {
            var assetsAtPath = AssetDatabase.LoadAllAssetsAtPath(path);
            Debug.Assert(assetsAtPath != null, $"[GUIStreamline] Failed to load assets at path {path}");
            var subAsset = assetsAtPath.FirstOrDefault(asset => asset != null && asset.name.StartsWith(name));
            return subAsset as Texture2D;
        }
    }
}