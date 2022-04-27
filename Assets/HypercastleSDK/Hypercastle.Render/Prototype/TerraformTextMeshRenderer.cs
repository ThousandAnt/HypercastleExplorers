using System;
using System.IO;
using Hypercastle.Web;
using TMPro;
using UnityEngine;

namespace Hypercastle.Render.Prototype
{

    public class TerraformTextMeshRenderer : MonoBehaviour
    {
        readonly float TOLERANCE = 0.01f;
        public string FileName;
        public string HeightFileName;
        public float[] heights;
        public Vector2 minMaxScalar;
        public TextMeshPro[] TextMesh = new TextMeshPro[1024];
        public Vector2 Offset;
        public Vector3 EulerAngles;
        public TMP_FontAsset FontAsset;
        public float FontSize = 50f;
        public Camera Camera;

        SvgRenderData renderData;
        Vector2 _offset;
        Vector3 _eulers;
        Vector2 _heights;
        Vector2 _minMaxHeights;
        float _fontSize;

        public float Timer = 0.5f;
        float _timer;

        void Start()
        {
            if (string.IsNullOrEmpty(FileName)) return;
            var path = Path.Combine(Application.streamingAssetsPath, FileName);
            var svgContents = File.ReadAllText(path);

            renderData = SvgUtils.Parse(svgContents);
            Camera.backgroundColor = renderData.BackgroundColor;

            for (var i = 0; i < renderData.CurrentGlyphs.Length; i++)
            {
                var go = new GameObject();
                var txtMesh = go.AddComponent<TextMeshPro>();
                go.transform.SetParent(transform, true);
                go.name = $"cell{i}";

                txtMesh.font = FontAsset;
                var glyph = renderData.CurrentGlyphs[i];
                txtMesh.text = glyph.ToString();
                txtMesh.color = renderData.GetAssociatedColor(renderData.AssociatedClasses[i]);

                txtMesh.transform.position = new Vector3((i % 32) * Offset.x, i / 32 * -Offset.y);
                TextMesh[i] = txtMesh;
            }

            if (string.IsNullOrEmpty(HeightFileName)) return;
            path = Path.Combine(Application.streamingAssetsPath, HeightFileName);
            var heightContents = File.ReadAllText(path);

            var heightStrings = heightContents.Split(' ');
            Debug.Log($"heightStrings count: {heightStrings.Length}");
            heights = new float[heightStrings.Length];
            _minMaxHeights = Vector2.zero;
            for (var i = 0; i < heights.Length; i++)
            {
                if (float.TryParse(heightStrings[i], out var height))
                {
                    if (height < _minMaxHeights.x) _minMaxHeights.x = height;
                    if (height > _minMaxHeights.y) _minMaxHeights.y = height;
                    heights[i] = height;
                }
            }
            _timer = Timer;
        }

        void UpdateFontSize()
        {
            for (int i = 0; i < renderData.CurrentGlyphs.Length; i++)
            {
                var txtMesh = TextMesh[i];
                txtMesh.fontSize = FontSize;
            }
        }

        void UpdatePositions()
        {
            for (int i = 0; i < renderData.CurrentGlyphs.Length; i++)
            {
                var txtMesh = TextMesh[i];
                txtMesh.transform.localPosition = new Vector3((i % 32) * Offset.x, i / 32 * -Offset.y);
            }
        }

        void UpdateEulerAngles()
        {
            for (int i = 0; i < renderData.CurrentGlyphs.Length; i++)
            {
                var txtMesh = TextMesh[i];
                txtMesh.transform.localRotation = Quaternion.Euler(EulerAngles);
            }
        }

        float map(float input, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (input - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }

        void UpdateHeightOffsets()
        {
            if (heights.Length < TextMesh.Length) return;
            for (var i = 0; i < renderData.CurrentGlyphs.Length; i++)
            {
                var height = map(heights[i], _minMaxHeights.x, _minMaxHeights.y,
                    minMaxScalar.x, minMaxScalar.y);
                var pos = TextMesh[i].transform.localPosition;
                TextMesh[i].transform.localPosition = new Vector3(pos.x, pos.y, height);
            }
        }

        void ApplyColors(float t)
        {
            for (int i = 0; i < TextMesh.Length; i++)
            {
                var mesh = TextMesh[i];
                var associatedClass = renderData.AssociatedClasses[i];
                if (renderData.ColorMap.TryGetValue(associatedClass, out var targetColor))
                {
                    mesh.color = Color.Lerp(mesh.color, targetColor, 1 - t);
                }
            }
        }

        void Update()
        {
            if (Offset != _offset)
            {
                UpdatePositions();
                _offset = Offset;
            }

            if (Math.Abs(FontSize - _fontSize) > TOLERANCE)
            {
                UpdateFontSize();
                _fontSize = FontSize;
            }

            if (EulerAngles != _eulers)
            {
                UpdateEulerAngles();
                _eulers = EulerAngles;
            }

            if (minMaxScalar != _heights)
            {
                UpdateHeightOffsets();
                _heights = minMaxScalar;
            }
            renderData.UpdateColors(Time.deltaTime);
            _timer -= Time.deltaTime;
            ApplyColors(Time.deltaTime * 5f);
            if (_timer < 0)
            {
                Animation.Update(
                    in renderData.Inputs,
                    in renderData.MainCharSet,
                    in renderData.CharSet,
                    renderData.GlyphContexts,
                    ref renderData.AirShip,
                    (index, unicode) => 
                    { 
                        TextMesh[index].text = char.ConvertFromUtf32(unicode); 
                    },
                    (index, size) => { TextMesh[index].fontSize = size; });

                _timer = Timer;
            }
        }
    }
}
