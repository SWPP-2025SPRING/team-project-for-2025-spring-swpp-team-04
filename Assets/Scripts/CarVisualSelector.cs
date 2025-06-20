using System.Collections.Generic;
using UnityEngine;

public class CarVisualSelector : MonoBehaviour
{
    [Header("List of Car Visuals")]
    public List<GameObject> carVisuals;

    [Header("Default Index to Use When No PlayerPrefs Key is Found")]
    public int defaultIndex = 0;

    void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCarIndex", defaultIndex);
        selectedIndex = Mathf.Clamp(selectedIndex, 0, carVisuals.Count - 1);

        for (int i = 0; i < carVisuals.Count; i++)
        {
            GameObject car = carVisuals[i];

            if (car != null)
            {
                bool isActive = (i == selectedIndex);
                car.SetActive(isActive);

                // Only reset position/rotation of the selected car
                if (isActive)
                {
                    car.transform.localPosition = Vector3.zero;
                    car.transform.localRotation = Quaternion.identity;
                }

                // Explicitly enable/disable all renderers and fix material issues
                Renderer[] renderers = car.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in renderers)
                {
                    if (renderer != null && renderer.sharedMaterial != null)
                    {
                        // Use shared material directly to avoid losing URP data
                        Material mat = renderer.sharedMaterial; // clone and assign

                        if (mat.HasProperty("_BaseMap") && mat.GetTexture("_BaseMap") == null)
                        {
                            // Try to manually assign the correct base map here (reference from Resources or manually assign)
                            Texture fallbackTexture = Resources.Load<Texture>("Textures/PCC_Default_Blue"); // 👈 use your real texture path here
                            if (fallbackTexture != null)
                            {
                                mat.SetTexture("_BaseMap", fallbackTexture);
                                Debug.Log($"[Material Fix] Assigned fallback BaseMap to: {renderer.gameObject.name}");
                            }
                            else
                            {
                                Debug.LogWarning($"[Material Warning] No BaseMap and fallback not found for: {renderer.gameObject.name}");
                            }
                        }

                        Texture baseMap = mat.GetTexture("_BaseMap");
                        Debug.Log($"[Material Debug] {renderer.gameObject.name}: Material = {mat.name}, BaseMap = {(baseMap ? baseMap.name : "NULL")}");

                        // Optional: Log to verify it's working
                        // Debug.Log($"[Renderer] Material: {renderer.material.name}, Shader: {renderer.material.shader.name}");
                    }
                }

                MeshRenderer[] meshRenderers = car.GetComponentsInChildren<MeshRenderer>(true);
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    meshRenderer.enabled = isActive;
                }

                Debug.Log($"[Visual] {(isActive ? "ON" : "OFF")} - {car.name}");
            }
            else
            {
                Debug.LogError($"carVisuals[{i}] is null!");
            }
        }
    }
}
