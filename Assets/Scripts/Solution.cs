using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class Solution : MonoBehaviour
{
    [SerializeField] private TextAsset modelJson;
    [SerializeField] private TextAsset spaceJson;
    [SerializeField] private string offsetsJsonAssetPath = "result_offsets.json";

    [Header("Settings")]
    [SerializeField] private float matrixEpsilon = 1e-3f;

    private void Start()
    {
        var model = JsonConvert.DeserializeObject<MatrixDto[]>(modelJson.text)
            .Select(x => x.ToMatrix4X4())
            .ToArray();
        var space = JsonConvert.DeserializeObject<MatrixDto[]>(spaceJson.text)
            .Select(x => x.ToMatrix4X4())
            .ToArray();;

        Debug.Log($"Model: {model.Length}");
        Debug.Log($"Space: {space.Length}");
        
        var offsets = FindAllOffsets(model, space);

        Debug.Log($"Offsets: {offsets.Count}");
        
        ExportOffsets(offsets);
    }
    
    private List<Matrix4x4> FindAllOffsets(Matrix4x4[] model, Matrix4x4[] space)
    {
        List<Matrix4x4> results = new();
        var modelAnchorInverse = model[0].inverse;

        foreach (var spaceMatrix in space)
        {
            // находим смещение для первой матрицы через формулу
            var offset = spaceMatrix * modelAnchorInverse;

            var valid = true;

            // теперь надо проверить, что все матрицы модели*смещение тоже есть в space
            for (var i = 1; i < model.Length; i++)
            {
                var transformed = offset * model[i];

                if (!ExistsInSpace(transformed, space))
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
                results.Add(offset);
        }

        return results;
    }

    private bool ExistsInSpace(Matrix4x4 target, Matrix4x4[] space)
    {
        foreach (var s in space)
        {
            if (MatrixApproximately(s, target, matrixEpsilon))
                return true;
        }
        return false;
    }

    private bool MatrixApproximately(Matrix4x4 a, Matrix4x4 b, float eps = 1e-3f)
    {
        for (var i = 0; i < 16; i++)
            if (Mathf.Abs(a[i] - b[i]) > eps)
                return false;
        return true;
    }
    
    private void ExportOffsets(List<Matrix4x4> offsets)
    {
        var json = JsonConvert.SerializeObject(offsets.Select(x => new MatrixDto(x)).ToArray(), Formatting.Indented);
        var path = Path.Combine(Application.dataPath, offsetsJsonAssetPath);
        File.WriteAllText(path, json);

        Debug.Log($"Offsets exported to {path}");
    }
}