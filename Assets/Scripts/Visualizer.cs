using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TextAsset modelJson;
    [SerializeField] private TextAsset spaceJson;
    [SerializeField] private TextAsset offsetsJson;

    [Header("Prefabs")]
    [SerializeField] private GameObject spacePrefab;
    [SerializeField] private GameObject modelPrefab;

    [Header("Visualization")]
    [SerializeField] private float stepDelay = 0.5f;

    private readonly List<GameObject> _modelObjects = new();
    private readonly List<GameObject> _spaceObjects = new();
    private Matrix4x4[] _model;
    private Matrix4x4[] _space;
    private Matrix4x4[] _offsets;


    private void Start()
    {
         _model = JsonConvert.DeserializeObject<MatrixDto[]>(modelJson.text)
            .Select(x => x.ToMatrix4X4())
            .ToArray();
         _space = JsonConvert.DeserializeObject<MatrixDto[]>(spaceJson.text)
            .Select(x => x.ToMatrix4X4())
            .ToArray();;
         _offsets = JsonConvert.DeserializeObject<MatrixDto[]>(offsetsJson.text)
            .Select(x => x.ToMatrix4X4())
            .ToArray();
        
        DrawMatrix(spacePrefab, _space, _spaceObjects);
        DrawMatrix(modelPrefab, _model, _modelObjects);
        VisualizeOffsets();
    }
    
    private void DrawMatrix(GameObject prefab, Matrix4x4[] space, List<GameObject> pull)
    {
        foreach (var m in space)
        {
            var go = Instantiate(
                prefab,
                ExtractPosition(m),
                ExtractRotation(m),
                transform
            );

            pull.Add(go);
        }
    }
    
    private void VisualizeOffsets()
    {
        StartCoroutine(AnimateOffsets());
    }

    private IEnumerator AnimateOffsets()
    {
        foreach (var offset in _offsets)
        {
            ApplyOffset(offset);
            yield return new WaitForSeconds(stepDelay);
            ResetModel();
        }
    }
    
    private void ApplyOffset(Matrix4x4 offset)
    {
        for (int i = 0; i < _modelObjects.Count; i++)
        {
            var world = offset * _model[i];

            _modelObjects[i]
                .transform.SetPositionAndRotation(
                    ExtractPosition(world),
                    ExtractRotation(world)
                );
        }
    }
    
    private void ResetModel()
    {
        for (int i = 0; i < _modelObjects.Count; i++)
        {
            _modelObjects[i]
                .transform.SetPositionAndRotation(
                    ExtractPosition(_model[i]),
                    ExtractRotation(_model[i])
                );
        }
    }

    private static Vector3 ExtractPosition(Matrix4x4 m)
    {
        return m.GetColumn(3);
    }

    private static Quaternion ExtractRotation(Matrix4x4 m)
    {
        return Quaternion.LookRotation(
            m.GetColumn(2),
            m.GetColumn(1)
        );
    }
}