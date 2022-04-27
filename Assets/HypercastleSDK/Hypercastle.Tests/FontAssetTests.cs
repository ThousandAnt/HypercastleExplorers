using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using System.Text;

namespace Hypercastle.Tests
{
    public class FontAssetTests
    {
        TMP_FontAsset _fontAsset;
        List<uint> unicodes;

        [SetUp]
        public void SetUp()
        {
            var guids = AssetDatabase.FindAssets("t: TMP_FontAsset MathcastlesCompleteRegularSDF");
            Assert.Greater(guids.Length, 0);
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            Assert.IsNotNull(_fontAsset);

            unicodes = new List<uint>();

            path = Path.Combine(Application.streamingAssetsPath, "CharacterData/sorted-uints.txt");
            var allChars = File.ReadAllText(path);

            var lines = allChars.Split('\n');
            foreach (var line in lines)
            {
                if (line.Length == 0)
                {
                    continue;
                }

                var value = Convert.ToUInt32(line.Trim());
                unicodes.Add(value);
            }

            Assert.AreEqual(unicodes.Count, lines.Length - 1);
        }

        bool LookUp(uint unicode, TMP_FontAsset fontAsset)
        {
            return fontAsset.characterLookupTable.ContainsKey(unicode);
        }

        [Test]
        public void FontAssetContainsCharacters()
        {
            var unknownGlyphs = new List<uint>();
            foreach (var unicode in unicodes)
            {
                var result = LookUp(unicode, _fontAsset);
                if (!result) {
                    unknownGlyphs.Add(unicode);
                }
            }
            unknownGlyphs.Sort();

            if (unknownGlyphs.Count > 0) 
            {
                var sb = new StringBuilder(unknownGlyphs.Count * 4);
                sb.Append("Failed to find the following glyphs: ");
                foreach (var unknown in unknownGlyphs) 
                {
                    sb.Append(Convert.ToString(unknown, 16)).Append(',');
                }
                Debug.LogWarning(sb.ToString());
                sb.Clear();

                foreach (var unknown in unknownGlyphs) {
                    sb.Append(Convert.ToString(unknown, 16)).Append('\n');
                }
                File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "CharacterData", "FontAssetTest_missing.txt"), sb.ToString());
            }

            Assert.AreEqual(0, unknownGlyphs.Count);
        }
    }
}
