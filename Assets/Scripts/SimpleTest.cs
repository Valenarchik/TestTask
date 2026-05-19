using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class SimpleTest : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TextAsset modelJson;
    [SerializeField] private TextAsset spaceJson;
    [SerializeField] private TextAsset offsetsJson;

    [Header("Settings")]
    [SerializeField] private float epsilon = 1e-3f;

    private void Start()
    {
        var model = JsonConvert.DeserializeObject<MatrixDto[]>(modelJson.text)
            .Select(x => x.ToMatrix4X4())
            .ToArray();

        var space = JsonConvert.DeserializeObject<MatrixDto[]>(spaceJson.text)
            .Select(x => x.ToMatrix4X4())
            .ToArray();

        var offsets = JsonConvert.DeserializeObject<MatrixDto[]>(offsetsJson.text)
            .Select(x => x.ToMatrix4X4())
            .ToArray();

        Debug.Log($"Model: {model.Length}");
        Debug.Log($"Space: {space.Length}");
        Debug.Log($"Offsets: {offsets.Length}");

        RunTest(model, space, offsets);
    }

    private void RunTest(Matrix4x4[] model, Matrix4x4[] space, Matrix4x4[] offsets)
    {
        for (var o = 0; o < offsets.Length; o++)
        {
            var offset = offsets[o];
            var valid = true;

            foreach (var m in model)
            {
                var transformed = offset * m;

                if (!transformed.ExistIn(space, epsilon))
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
                Debug.Log($"Offset #{o} is VALID");
            else
                Debug.LogError($"Offset #{o} is INVALID");
        }

        Debug.Log($"{nameof(SimpleTest)} finished");
    }
}