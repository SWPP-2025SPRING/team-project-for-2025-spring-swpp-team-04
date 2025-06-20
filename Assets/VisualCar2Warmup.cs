using UnityEngine;

public class VisualCar2Warmup : MonoBehaviour
{
    public Material car2Material;

    void Awake()
    {
        if (car2Material != null)
        {
            if (car2Material.HasProperty("_BaseMap"))
            {
                Texture tex = car2Material.GetTexture("_BaseMap");
                Debug.Log($"[Warmup] visualcar2 BaseMap = {(tex != null ? tex.name : "null")}");
            }

            GameObject tempObj = new GameObject("VisualCar2WarmupDummy");
            tempObj.hideFlags = HideFlags.HideAndDontSave;

            Renderer renderer = tempObj.AddComponent<MeshRenderer>();
            renderer.material = new Material(car2Material);

            MeshFilter mf = tempObj.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            Destroy(tempObj, 1f);
        }
        else
        {
            Debug.LogWarning("[Warmup] car2Material is null");
        }
    }
}
