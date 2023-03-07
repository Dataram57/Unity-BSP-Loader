using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dataram57.BSPImporter
{
    [CreateAssetMenu(fileName = "config", menuName = "Dataram57/BSP Import Config", order = 1)]
    public class BSPImportConfig : ScriptableObject
    {
        public string targetBSPFilePath;
        public float meshScale = 0.1f;
        public string materialsAssetsFilePath;

        public bool VeryfiConfig()
        {

            return true;
        }
    }
}