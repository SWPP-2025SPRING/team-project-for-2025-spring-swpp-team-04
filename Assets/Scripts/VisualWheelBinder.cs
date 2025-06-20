using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualWheelBinder : MonoBehaviour
{
    public PlayerController_updated controller;
    public GameObject visualRoot; // Player/Visual 오브젝트

    void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCarIndex", 0);
        string carName = visualRoot.transform.GetChild(selectedIndex).name;

        Transform selectedCar = visualRoot.transform.Find(carName);
        if (selectedCar == null)
        {
            Debug.LogError($"🚨 {carName} not found under VisualRoot");
            return;
        }

        Transform wheelsRoot = selectedCar.Find("Wheels");
        if (wheelsRoot == null)
        {
            Debug.LogError("🚨 Cannot find Wheels inside selected car!");
            return;
        }

        Transform meshes = wheelsRoot.Find("Meshes");
        Transform colliders = wheelsRoot.Find("Colliders");

        if (meshes == null || colliders == null)
        {
            Debug.LogError("🚨 Cannot find Meshes or Colliders inside Wheels!");
            return;
        }

        // 🔧 Mesh 연결
        controller.wheelMeshes = new Transform[4];
        controller.wheelMeshes[0] = meshes.Find("FrontLeftWheel");
        controller.wheelMeshes[1] = meshes.Find("FrontRightWheel");
        controller.wheelMeshes[2] = meshes.Find("RearLeftWheel");
        controller.wheelMeshes[3] = meshes.Find("RearRightWheel");

        // 🔧 Collider 연결
        controller.wheelColliders = new WheelCollider[4];
        controller.wheelColliders[0] = colliders.Find("FrontLeftWheel").GetComponent<WheelCollider>();
        controller.wheelColliders[1] = colliders.Find("FrontRightWheel").GetComponent<WheelCollider>();
        controller.wheelColliders[2] = colliders.Find("RearLeftWheel").GetComponent<WheelCollider>();
        controller.wheelColliders[3] = colliders.Find("RearRightWheel").GetComponent<WheelCollider>();

        Debug.Log("✅ Wheel meshes and colliders successfully assigned.");
    }
}
