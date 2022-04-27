using ExCSS;
using Hypercastle.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;
using Unity.Collections;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Hypercastle.Web
{
    public struct ColorClass
    {
        public Color32 Color;
        public char Class;
    }

    public struct AnimationInput
    {
        public bool IsDayDream => Mode == 1 || Mode == 3;
        public bool IsTerraformed => Mode == 3 || Mode == 4;
        public bool IsOrigin => Mode == 3 || Mode == 4;
        public int SpeedFactor => Seed > 6500 ? 30 : 1;

        public int Mode;
        public int Seed;
        public int Direction;
        public float Resource;
    }

    public class SvgRenderData
    {
        public AnimationInput Inputs;
        
        // -----------------------------------------------------------------------
        // Immutable
        // -----------------------------------------------------------------------
        /// <summary>
        /// Stores the initial set of colors for each unique class.
        /// </summary>
        public ColorStyle[] BaseColors;

        /// <summary>
        /// Stores the interpolated color at a point normalized between 0 and 100%.
        /// </summary>
        public Keyframe[] Keyframes;

        /// <summary>
        /// Stores the type of css animation that needs to be interpretted in Unity.
        /// </summary>
        public AnimationData[] Animations;

        /// <summary>
        /// The initial set of unique class Ids.
        /// </summary>
        public char[] ClassIds;
        public int[] MainCharSet;
        public int[] CharSet;
        public int[] Unicodes;
        public ContextInfo[] GlyphContexts;

        public Color32 BackgroundColor;

        // -----------------------------------------------------------------------
        // Mutable
        // -----------------------------------------------------------------------
        /// <summary>
        /// The associated class identifier with the <seealso cref="SvgRenderData.CurrentGlyphs"/>
        /// </summary>
        public char[] AssociatedClasses;

        /// <summary>
        /// The set of glyphs that are _currently_ rendered. To get the color with this glyph
        /// <seealso cref="SvgRenderData.AssociatedClasses"/> and 
        /// <seealso cref="SvgRenderData.ColorMap"/>
        /// </summary>
        public int[] CurrentGlyphs;

        /// <summary>
        /// Stores the current time of the animation. This allows us to transition to the next 
        /// color.
        /// </summary>
        public AnimationState[] AnimationStates;

        /// <summary>
        /// Our animation driver that continuously increments.
        /// </summary>
        public uint AirShip;

        // TODO: Maybe a fixed dictionary might be better.
        /// <summary>
        /// The Color Map contains next target color of the css animation. 
        /// <remarks>
        /// Not all colors are mapped per class, so when reading from the data you 
        /// _must_ check that the class identifier actually exists.
        /// </remarks>
        /// </summary>
        public Dictionary<char, Color32> ColorMap;

        public void Initialize()
        {
            GlyphContexts = new ContextInfo[1024];
            for (int i = 0; i < 1024; i++)
            {
                char associatedClass = AssociatedClasses[i];
                var idx = Array.IndexOf(ClassIds, associatedClass);
                GlyphContexts[i] = new ContextInfo
                {
                    h = (int)(ClassIds.Length - idx),
                    ActiveClass = associatedClass,
                    OriginalClass = associatedClass
                };
            }

            // Build the initial color map
            ColorMap = new Dictionary<char, Color32>(ClassIds.Length);
            for (int i = 0; i < BaseColors.Length; i++) {
                var associatedColor = BaseColors[i];
                ColorMap.TryAdd(associatedColor.Class, associatedColor.Color);
            }
        }

        public Color32 GetAssociatedColor(char associatedClass)
        {
            for (int i = 0; i < BaseColors.Length; i++)
            {
                var colorStyle = BaseColors[i];
                if (colorStyle.Class == associatedClass)
                {
                    return colorStyle.Color;
                }
            }
            return default;
        }

        public void UpdateColors(float deltaTime)
        {
            for (int i = 0; i < AnimationStates.Length; i++)
            {
                ref var animationState = ref AnimationStates[i];
                if (animationState.CurrentDelay <= 0)
                {
                    var animationData = Animations[i];
                    animationState.CurrentTime = 
                        (animationState.CurrentTime + animationData.AnimationDuration - deltaTime) 
                        % animationData.AnimationDuration;
                    var ratio = 
                        Mathf.FloorToInt(
                            (animationState.CurrentTime / animationData.AnimationDuration) 
                            * 100);
                    for (int j = 0; j < Keyframes.Length; j++)
                    {
                        if (Keyframes[j].Percentage == ratio)
                        {
                            ColorMap[animationData.AssociatedClass] = Keyframes[j].Color;
                            break;
                        }
                    }
                } else
                {
                    animationState.CurrentDelay -= deltaTime;
                }
            }
        }
    }

    public struct Keyframe
    {
        public byte Percentage;
        public Color32 Color;
    }

    public struct ColorStyle
    {
        public char Class;
        public Color32 Color;
    }

    public struct ContextInfo
    {
        // TODO: Add more context info per glyph.
        public int h;
        public char OriginalClass;
        public char ActiveClass;
    }

    public struct AnimationState
    {
        public char AssociatedClass;
        public float CurrentTime;
        public float CurrentDelay;

        public void DebugState()
        {
            Debug.Log($"{AssociatedClass}, CurrentTime: {CurrentTime}, Delay: {CurrentDelay}");
        }
    }

    public struct AnimationData
    {
        public char AssociatedClass;
        public float AnimationDuration;
        public float AnimationDelay;
        public string AnimationDirection;
        public string AnimationTimingFunction;
        public string AnimationFillMode;
        public string AnimationIterationCount;
        public string AnimationName;
        public string AnimationPlayState;

        public AnimationData(IStyleRule rule)
        {
            AssociatedClass = rule.Selector.Text[1];
            AnimationDuration = float.Parse(rule.Style.AnimationDuration.Substring(0, rule.Style.AnimationDuration.IndexOf('m'))) / 1000f;
            AnimationDelay = float.Parse(rule.Style.AnimationDelay.Substring(0, rule.Style.AnimationDelay.IndexOf('m'))) / 1000f;
            AnimationDirection = rule.Style.AnimationDirection;
            AnimationTimingFunction = rule.Style.AnimationTimingFunction;
            AnimationFillMode = rule.Style.AnimationFillMode;
            AnimationIterationCount = rule.Style.AnimationIterationCount;
            AnimationName = rule.Style.AnimationName;
            AnimationPlayState = rule.Style.AnimationPlayState;
        }

        public override string ToString()
        {
            return
                $"{AnimationDuration} {AnimationDelay} {AnimationDirection} {AnimationTimingFunction} {AnimationFillMode} {AnimationName} {AnimationPlayState}";
        }
    }

    public static class SvgUtils
    {
        static readonly Regex ColorHexPattern = new(@"#[a-zA-Z0-9]{6}");
        static readonly Regex ColorPattern = new(@"[0-9]{1,3}");
        static readonly Regex PercentPattern = new(@"[0-9]{1,3}(?=%)");
        static readonly Regex ColorRgbPattern = new(@"([0-9]{1,3}), ([0-9]{1,3}), ([0-9]{1,3})");
        static readonly Regex ConstPattern = new(@"(?<=let |const )[A-Z]*=[0-9]*(?=;)");
        static readonly Regex UnicodeSeriesPattern = new(@"(?<=uni=\[)(.)*(\d)(?=];)");
        static readonly Regex ClassIdsPattern = new(@"(?<=classIds=\[)(.)*(?=],charSet)");

        static readonly Regex NumberPattern = new(@"(\d)+");
        static readonly Regex CharacterPattern = new(@"(?<=')[a-z]{1}(?=')");
        static readonly char[] TrimmableRgbCharacters = { ',', ')' };

        internal static Color32 ToColor32(this string value)
        {
            var matches = ColorPattern.Matches(value);
            if (matches.Count == 3)
            {
                var r = byte.Parse(matches[0].Value.TrimEnd(TrimmableRgbCharacters));
                var g = byte.Parse(matches[1].Value.TrimEnd(TrimmableRgbCharacters));
                var b = byte.Parse(matches[2].Value.TrimEnd(TrimmableRgbCharacters));
                return new Color32(r, g, b, 255);
            }
            return default;
        }

        internal static Color32 ParseColor(string cssValue)
        {
            var matchedColorHex = ColorHexPattern.Match(cssValue);
            if (matchedColorHex.Success && matchedColorHex.Groups.Count == 1)
            {
                if (ColorUtility.TryParseHtmlString(matchedColorHex.Groups[0].Value, out Color color))
                {
                    return color;
                }
            }
            return Color.clear;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void MakeSet(int unicode, List<List<int>> result)
        {
            var list = new List<int>();
            for (int i = unicode; i < unicode + 10; i++)
            {
                list.Add(i);
            }
            result.Add(list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void MakeSet(int unicode, List<int> result)
        {
            for (int i = unicode; i < unicode + 10; i++)
            {
                result.Add(i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void MakeSet(int unicode, ref FixedList<int> result)
        {
            for (int i = unicode; i < unicode + 10; i++)
            {
                result.Add(i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void MakeSet(int unicode, ref NativeFixedList<int> result)
        {
            for (int i = unicode; i < unicode + 10; i++)
            {
                result.Add(i);
            }
        }

        internal static T[] ParseArray<T>(
            string text,
            Regex arrayPattern,
            Regex elementPattern,
            Func<string, T> perElementConversion)
        {
            var arrayMatch = arrayPattern.Match(text);
            var elements = elementPattern.Matches(arrayMatch.Value);
            var output = new T[elements.Count];
            for (int i = 0; i < elements.Count; i++)
            {
                output[i] = perElementConversion(elements[i].Value);
            }
            return output;
        }

        public static SvgRenderData Parse(string svgContents)
        {
            var svgRenderData = new SvgRenderData();
            var document = new XmlDocument();
            document.LoadXml(svgContents);

            for (int i = 0; i < document.FirstChild.ChildNodes.Count; i++)
            {
                var child = document.FirstChild.ChildNodes[i];
                switch (child.Name)
                {
                    case "style":
                        var parser = new StylesheetParser();
                        var stylesheet = parser.Parse(document.InnerText);

                        if (svgRenderData.Animations == null)
                        {
                            svgRenderData.Animations = stylesheet.StyleRules
                                .Where(rule => !string.IsNullOrEmpty(rule.Style.Animation))
                                .Select(rule => new AnimationData(rule)).ToArray();

                            var animationStates = new AnimationState[svgRenderData.Animations.Length];
                            for (int j = 0; j < svgRenderData.Animations.Length; j++)
                            {
                                var animation = svgRenderData.Animations[j];
                                animationStates[j] = new AnimationState
                                {
                                    AssociatedClass = animation.AssociatedClass,
                                    CurrentTime = animation.AnimationDuration,
                                    CurrentDelay = animation.AnimationDelay,
                                };
                            }
                            svgRenderData.AnimationStates = animationStates;
                        }

                        if (svgRenderData.Keyframes == null)
                        {
                            svgRenderData.Keyframes =
                                stylesheet.Children
                                .Where(rule => rule is IKeyframesRule)
                                .SelectMany(rule =>
                                {
                                    var keyframeRule = rule as IKeyframesRule;
                                    return keyframeRule.Rules;
                                }).Select(rule =>
                                {
                                    var percentMatch = PercentPattern.Match(rule.Text);
                                    var percentageValue = percentMatch.Success ?
                                        byte.Parse(percentMatch.Value) : (byte)0;
                                    var colorValue = ColorRgbPattern.Match(rule.Text).Value.ToColor32();
                                    return new Keyframe
                                    {
                                        Color = colorValue,
                                        Percentage = percentageValue,
                                    };
                                }).ToArray();
                        }

                        if (svgRenderData.BaseColors == null)
                        {
                            svgRenderData.BaseColors = stylesheet.StyleRules
                                .Where(rule => !string.IsNullOrEmpty(rule.Style.Color))
                                .Select(rule => new ColorStyle
                                {
                                    Class = rule.SelectorText[1],
                                    Color = rule.Style.Color.ToColor32()
                                }).ToArray();
                        }

                        svgRenderData.BackgroundColor =
                            stylesheet.StyleRules
                                .Where(rule => !string.IsNullOrEmpty(rule.Style.BackgroundColor))
                                .Select(rule => rule.Style.BackgroundColor.ToColor32())
                                .First();
                        break;
                    case "foreignObject":
                        var node = RecurseFindGlyphParent(child, "r");
                        if (node != null && node.HasChildNodes)
                        {
                            var glyphs = new int[32 * 32];
                            var classes = new char[32 * 32];
                            for (int j = 0; j < node.ChildNodes.Count; j++)
                            {
                                var current = node.ChildNodes[j];
                                var text = current.InnerText;
                                var associatedClass = current.Attributes[0].Value;
                                classes[j] = associatedClass[0];

                                // Sometimes special characters end up being read with no valid 
                                // characters...
                                switch (text.Length)
                                {
                                    case 0:
                                        glyphs[j] = ' ';
                                        break;
                                    case 1:
                                        glyphs[j] = text[0];
                                        break;
                                    case 2:
                                        glyphs[j] = char.ConvertToUtf32(text, 0);
                                        break;
                                }
                            }

                            svgRenderData.AssociatedClasses = classes;
                            svgRenderData.CurrentGlyphs = glyphs;
                        }
                        break;
                    case "script":
                        // We have to parse the script and extra the constants out of it.
                        var matches = ConstPattern.Matches(child.InnerText);
                        var inputs = new AnimationInput();
                        for (int j = 0; j < matches.Count; j++)
                        {
                            var value = matches[j].Value;
                            var variable = value.Substring(0, value.IndexOf('='));
                            var number = int.Parse(NumberPattern.Match(value).Value);

                            switch (variable)
                            {
                                case "MODE":
                                    inputs.Mode = number;
                                    break;
                                case "RESOURCE":
                                    inputs.Resource = number / (float)1e4;
                                    break;
                                case "DIRECTION":
                                    inputs.Direction = number;
                                    break;
                                case "SEED":
                                    inputs.Seed = number;
                                    break;
                            }
                        }
                        svgRenderData.Inputs = inputs;

                        // Grab the classIds for parsing
                        svgRenderData.ClassIds = ParseArray(
                            child.InnerText,
                            ClassIdsPattern,
                            CharacterPattern,
                            text => text[0]);

                        // Parse the unicode embedded chars...
                        svgRenderData.Unicodes = ParseArray(
                            child.InnerText,
                            UnicodeSeriesPattern,
                            NumberPattern,
                            int.Parse);

                        var originalChars = new NativeFixedList<int>(svgRenderData.ClassIds.Length, 
                            Allocator.Temp);
                        foreach (var classId in svgRenderData.ClassIds)
                        {
                            var index = svgRenderData.AssociatedClasses
                                .FirstIndex(cls => cls == classId);
                            if (index > -1)
                            {
                                originalChars.Add(svgRenderData.CurrentGlyphs[index]);
                            }
                        }

                        originalChars.TrimExcess();
                        var charSet = new NativeFixedList<int>(
                            svgRenderData.ClassIds.Length + svgRenderData.Unicodes.Length * 10, 
                            Allocator.Temp);
                        charSet.AddRange(originalChars.Collection);

                        if (svgRenderData.Inputs.IsOrigin)
                        {
                            if (svgRenderData.Inputs.Seed > 9e3)
                            {
                                foreach (var unicode in svgRenderData.Unicodes)
                                {
                                    MakeSet(unicode, ref charSet);
                                }
                            } else
                            {
                                foreach (var unicode in svgRenderData.Unicodes)
                                {
                                    MakeSet(
                                        svgRenderData
                                            .Unicodes[
                                                Mathf.FloorToInt(svgRenderData.Inputs.Seed) % 
                                                svgRenderData.Unicodes.Length], 
                                        ref charSet);
                                }
                            }
                        } else if (svgRenderData.Inputs.Seed > 9970)
                        {
                            foreach (var unicode in svgRenderData.Unicodes)
                            {

                                MakeSet(unicode, ref charSet);
                            }
                        } else if (svgRenderData.Inputs.Seed > 5e3)
                        {
                            MakeSet(
                                svgRenderData.Unicodes[
                                    Mathf.FloorToInt(svgRenderData.Inputs.Seed) % 3], 
                                ref charSet);
                        }

                        charSet.TrimExcess();
                        var charCollection = charSet.ToArray();
                        svgRenderData.CharSet = charCollection;

                        if (svgRenderData.Inputs.Seed > 9950)
                        {
                            svgRenderData.MainCharSet = charCollection;
                        } else
                        {
                            originalChars.Reverse();
                            svgRenderData.MainCharSet = originalChars.ToArray();
                        }
                        charSet.Dispose();
                        originalChars.Dispose();
                        break;
                }
            }

            svgRenderData.Initialize();

            return svgRenderData;
        }

        static XmlNode RecurseFindGlyphParent(XmlNode current, string classValue)
        {
            // First search that the current node has the attribute we're looking for
            for (int j = 0; current.Attributes != null && j < current.Attributes.Count; j++)
            {
                var attribute = current.Attributes[j];
                if (classValue.Equals(attribute.Value))
                {
                    return current;
                }
            }

            if (current.HasChildNodes)
            {
                for (int i = 0; i < current.ChildNodes.Count; i++)
                {
                    var child = current.ChildNodes[i];
                    var node = RecurseFindGlyphParent(child, classValue);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }

            return null;
        }
    }
}
