using System.IO;
using System.Collections.Generic;
using Codice.Client.Commands.TransformerRule;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hypercastle.Render.Prototype
{
    public class TerraformBoxBuilder : MonoBehaviour
    {
        public GameObject instancePrimitive;
        public string FileName;
        public Vector3 Offset;
        public Vector3 Scale;


        private List<GameObject> _instances = new List<GameObject>();
        private Vector3 _offset;
        private Vector3 _scale;
        private Material mat;
        
        void Start()
        {
            if (string.IsNullOrEmpty(FileName)) return;
            var path = Path.Combine(Application.streamingAssetsPath, FileName);
            var text = File.ReadAllText(path);
            
            var indices = text.Split(' ');
            var heights = new float[indices.Length];
            for(var i = 0; i < indices.Length; i++)
            {
                if(int.TryParse(indices[i],out var height))
                {
                    heights[i] = height;
                }
            }
            
            Debug.Log($"heights count: {heights.Length}");
            for (var i = 0; i < heights.Length; i++)
            {
                var root = new GameObject();
                _instances.Add(root);
                root.transform.parent = transform;
                for (var j = 0; j < heights[i] + 1; j++)
                {
                    var go = Instantiate(instancePrimitive, root.transform, true);
                    go.name = $"cell{i}";
                    go.transform.position = new Vector3((i % 32) * Offset.x, i / 32f * -Offset.y, j * Offset.z);
                    go.transform.localScale = Scale;
                }
            }
            
            if (GetComponent<MeshFilter>() == null)
                gameObject.AddComponent<MeshFilter>();
            
            if (GetComponent<MeshRenderer>() == null)
                gameObject.AddComponent<MeshRenderer>();
            
            var meshFilters = GetComponentsInChildren<MeshFilter>();
            var combine = new CombineInstance[meshFilters.Length];

            var meshCount = 0;
            while (meshCount < meshFilters.Length)
            {
                if (meshCount == 0)
                    mat = meshFilters[meshCount].gameObject.GetComponent<MeshRenderer>().material;
                combine[meshCount].mesh = meshFilters[meshCount].sharedMesh;
                combine[meshCount].transform = meshFilters[meshCount].transform.localToWorldMatrix;
                meshFilters[meshCount].gameObject.SetActive(false);

                meshCount++;
            }
            
            var meshFilter = GetComponent<MeshFilter>().mesh = new Mesh();
            meshFilter.indexFormat = IndexFormat.UInt32;
            GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            GetComponent<MeshRenderer>().material = mat;
            transform.gameObject.SetActive(true);

            //_offset = Offset;
            //_scale = Scale;
            for (var index = 0; index < transform.childCount; index++)
            {
                var child = transform.GetChild(index);
                for (var j = 0; j < child.childCount; j++)
                {
                    var subChild = child.transform.GetChild(j);
                    Destroy(subChild.gameObject);
                }
                Destroy(child.gameObject);
            }
        }
        /*
        void UpdatePositions()
        {
            for (var i = 0; i < _instances.Count; i++)
            {
                for (var j = 0; j < _instances[i].transform.childCount; j++)
                {
                    _instances[i].transform.GetChild(j).localPosition =
                        new Vector3((i % 32f) * Offset.x, i / 32f * -Offset.y, j * Offset.z);
                }
            }
        }

        void UpdateScales()
        {
            for (var i = 0; i < _instances.Count; i++)
            {
                for (var j = 0; j < _instances[i].transform.childCount; j++)
                {
                    _instances[i].transform.GetChild(j).localPosition = Scale;
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
            
            if (Scale != _scale)
            {
                UpdateScales();
                _scale = Scale;
            }
        }
        */
    }
}