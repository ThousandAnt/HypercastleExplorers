using System.IO;
using System.Linq;
using Hypercastle.Web;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Hypercastle.Tests
{
    public class SvgUtilsTests
    {
        struct Results
        {
            public int[] MainSet;
            public int[] CharSet;
        }

        internal static string GetSvgFileContents(string file)
        {
            var path = Path.Combine(Application.dataPath, "HypercastleSDK/Hypercastle.Tests/TestingResources/Svgs", file);
            return File.ReadAllText(path);
        }

        internal static SvgRenderData Parse(string file)
        {
            var svgFileContents = GetSvgFileContents(file);
            return SvgUtils.Parse(svgFileContents);
        }

        static Results FetchResultsTxt(string filter)
        {
            var guids = AssetDatabase.FindAssets(filter);
            Assert.AreEqual(1, guids.Length);
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

            var lines = textAsset.text.Split('\n');
            Assert.AreEqual(2, lines.Length);

            var mainSet = lines[0].Split(',').Select(int.Parse).ToArray();
            var charSet = lines[1].Split(',').Select(int.Parse).ToArray();

            return new Results
            {
                MainSet = mainSet,
                CharSet = charSet
            };
        }

        [Test]
        public void ExtractsDataFromTheSvg_556()
        {
            var svgRenderData = Parse("556.svg");

            char[] classIds = { 'i', 'h', 'g', 'f', 'e', 'd', 'c', 'b', 'a' };
            uint[] mainSet = { 9053, 46, 9617, 9617, 9617, 46, 46, 10007, 9053 };
            uint[] charSet = { 9053, 10007, 46, 46, 9617, 9617, 9617, 46, 9053 };

            Assert.IsNotNull(svgRenderData.Animations);
            Assert.IsNotNull(svgRenderData.BaseColors);
            Assert.IsNotNull(svgRenderData.Keyframes);
            Assert.AreNotEqual(svgRenderData.BackgroundColor, default);
            Assert.IsNotNull(svgRenderData.CurrentGlyphs);
            Assert.IsNotNull(svgRenderData.AssociatedClasses);

            Assert.AreEqual(svgRenderData.ClassIds.Length, classIds.Length);
            for (int i = 0; i < svgRenderData.ClassIds.Length; i++)
            {
                Assert.AreEqual(classIds[i], svgRenderData.ClassIds[i], "Classes do not match");
            }
            Assert.AreEqual(svgRenderData.MainCharSet.Length, mainSet.Length);
            for (int i = 0; i < svgRenderData.MainCharSet.Length; i++)
            {
                Assert.AreEqual(mainSet[i], svgRenderData.MainCharSet[i], "Main char set didn't match");
            }
            Assert.AreEqual(charSet.Length, svgRenderData.CharSet.Length);

            for (int i = 0; i < svgRenderData.CharSet.Length; i++)
            {
                Assert.AreEqual(charSet[i], (int)svgRenderData.CharSet[i]);
            }
        }

        [Test]
        public void ExtractsDataFromTheSvg_2752()
        {
            var svgRenderData = Parse("2752.svg");
            Assert.IsNotNull(svgRenderData.Animations);
            Assert.IsNotNull(svgRenderData.BaseColors);
            Assert.IsNotNull(svgRenderData.Keyframes);
            Assert.AreNotEqual(svgRenderData.BackgroundColor, default);
            Assert.IsNotNull(svgRenderData.CurrentGlyphs);
            Assert.IsNotNull(svgRenderData.AssociatedClasses);

            var results = FetchResultsTxt("t: TextAsset 2752_results");

            Assert.AreEqual(results.MainSet.Length, svgRenderData.MainCharSet.Length);

            for (int i = 0; i < results.MainSet.Length; i++)
            {
                Assert.AreEqual(results.MainSet[i], svgRenderData.MainCharSet[i], $"Failed at index: {i}");
            }

            Assert.AreEqual(results.CharSet.Length, svgRenderData.CharSet.Length);
            for (int i = 0; i < results.CharSet.Length; i++)
            {
                Assert.AreEqual((char)results.CharSet[i], (char)svgRenderData.CharSet[i]);
            }
        }

        [Test]
        public void ExtractsDataFromTheSvg_1122()
        {
            var charSet = new uint[]
            {
                127956,9552,9552,9552,9552,9552,9552,9552,127956,9620,9621,9622,9623,9624,9625,9626,9627,9628,9629
            };

            var mainSet = new uint[]
            {
                127956,9552,9552,9552,9552,9552,9552,9552,127956
            };

            var svgRenderData = Parse("1122.svg");
            Assert.IsNotNull(svgRenderData.Animations);
            Assert.IsNotNull(svgRenderData.BaseColors);
            Assert.IsNotNull(svgRenderData.Keyframes);
            Assert.AreNotEqual(svgRenderData.BackgroundColor, default);
            Assert.IsNotNull(svgRenderData.CurrentGlyphs);
            Assert.IsNotNull(svgRenderData.AssociatedClasses);

            Assert.AreEqual(charSet.Length, svgRenderData.CharSet.Length);
            for (int i = 0; i < charSet.Length; i++)
            {
                Assert.AreEqual((char)charSet[i], (char)svgRenderData.CharSet[i]);
            }

            Assert.AreEqual(mainSet.Length, svgRenderData.MainCharSet.Length);
            for (int i = 0; i < mainSet.Length; i++)
            {
                Assert.AreEqual((char)mainSet[i], (char)svgRenderData.MainCharSet[i]);
            }
        }

        [Test]
        public void ExtractsDataFromTheSvg_7034()
        {
            var charSet = new uint[] {
                9820,9814,9814,9814,32,32,32,9816,9820
            };

            var mainSet = new uint[] {
                9820,9816,32,32,32,9814,9814,9814,9820
            };
            var svgRenderData = Parse("7034.svg");

            Assert.IsNotNull(svgRenderData.Animations);
            Assert.IsNotNull(svgRenderData.BaseColors);
            Assert.IsNotNull(svgRenderData.Keyframes);
            Assert.AreNotEqual(svgRenderData.BackgroundColor, default);
            Assert.IsNotNull(svgRenderData.CurrentGlyphs);
            Assert.IsNotNull(svgRenderData.AssociatedClasses);

            Assert.AreEqual(charSet.Length, svgRenderData.CharSet.Length);
            Assert.AreEqual(mainSet.Length, svgRenderData.MainCharSet.Length);

            Assert.AreEqual(charSet.Length, svgRenderData.CharSet.Length);
            for (int i = 0; i < charSet.Length; i++)
            {
                Assert.AreEqual((char)charSet[i], (char)svgRenderData.CharSet[i]);
            }

            Assert.AreEqual(mainSet.Length, svgRenderData.MainCharSet.Length);
            for (int i = 0; i < mainSet.Length; i++)
            {
                Assert.AreEqual((char)mainSet[i], (char)svgRenderData.MainCharSet[i]);
            }
        }

        [Test]
        public void ExtractsDataFromTheSvg_8857()
        {
            var charSet = new uint[] {
                9617,9618,9618,9618,9619,9619,9618,9617,9610,9611,9612,9613,9614,9615,9616,9617,
                9618,9619
            };

            var mainSet = new uint[] {
                9617,9618,9619,9619,9618,9618,9618,9617
            };
            var svgRenderData = Parse("8857.svg");
            Assert.IsNotNull(svgRenderData.Animations);
            Assert.IsNotNull(svgRenderData.BaseColors);
            Assert.IsNotNull(svgRenderData.Keyframes);
            Assert.AreNotEqual(svgRenderData.BackgroundColor, default);
            Assert.IsNotNull(svgRenderData.CurrentGlyphs);
            Assert.IsNotNull(svgRenderData.AssociatedClasses);

            for (int i = 0; i < svgRenderData.CharSet.Length; i++)
            {
                Debug.Log($"{i} {(char)svgRenderData.CharSet[i]} {(char)charSet[i]}");
            }

            Assert.AreEqual(charSet.Length, svgRenderData.CharSet.Length);
            for (int i = 0; i < charSet.Length; i++)
            {
                Assert.AreEqual((char)charSet[i], (char)svgRenderData.CharSet[i]);
            }

            Assert.AreEqual(mainSet.Length, svgRenderData.MainCharSet.Length);
            for (int i = 0; i < mainSet.Length; i++)
            {
                Assert.AreEqual((char)mainSet[i], (char)svgRenderData.MainCharSet[i]);
            }
        }

        [Test]
        public void ExtractsDataFromTheSvg_7702()
        {
            var charSet = new uint[] {
                9604,9617,9600,9619,9618,9617,9617,9604,9608
            };
            var mainSet = new uint[] {
                9608,9604,9617,9617,9618,9619,9600,9617,9604
            };
            var svgRenderData = Parse("7702.svg");

            Assert.IsNotNull(svgRenderData.Animations);
            Assert.IsNotNull(svgRenderData.BaseColors);
            Assert.IsNotNull(svgRenderData.Keyframes);
            Assert.AreNotEqual(svgRenderData.BackgroundColor, default);
            Assert.IsNotNull(svgRenderData.CurrentGlyphs);
            Assert.IsNotNull(svgRenderData.AssociatedClasses);

            Assert.AreEqual(charSet.Length, svgRenderData.CharSet.Length);
            for (int i = 0; i < charSet.Length; i++)
            {
                Assert.AreEqual((char)charSet[i], (char)svgRenderData.CharSet[i]);
            }

            Assert.AreEqual(mainSet.Length, svgRenderData.MainCharSet.Length);
            for (int i = 0; i < mainSet.Length; i++)
            {
                Assert.AreEqual((char)mainSet[i], (char)svgRenderData.MainCharSet[i]);
            }
        }
    }
}
