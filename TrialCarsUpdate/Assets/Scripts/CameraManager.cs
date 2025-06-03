using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;          // Main Camera = third person
    public Camera firstPersonCam;
    public Camera rearViewCam;

    public RenderTexture rearViewTexture;
    public GameObject rearViewUI;
    public GameObject rearViewBorderPanel;

    private int cameraView = 0;

    void Start()
    {
        UpdateCameraView();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            cameraView = (cameraView + 1) % 3;
            UpdateCameraView();
        }
    }

    void UpdateCameraView()
    {
        CameraController mainCamController = mainCamera.GetComponent<CameraController>();
        CameraController fpCamController = firstPersonCam.GetComponent<CameraController>();

        switch (cameraView)
        {
            case 0: // Third-person
                mainCamera.tag = "MainCamera";
                firstPersonCam.tag = "Untagged";

                mainCamera.gameObject.SetActive(true);
                firstPersonCam.gameObject.SetActive(false);
                rearViewCam.gameObject.SetActive(false);
                rearViewUI.SetActive(false);

                if (rearViewBorderPanel != null) rearViewBorderPanel.SetActive(false);
                if (mainCamController) mainCamController.enabled = true;
                if (fpCamController) fpCamController.enabled = false;
                break;

            case 1: // First-person + rear view
                mainCamera.tag = "Untagged";
                firstPersonCam.tag = "MainCamera";

                mainCamera.gameObject.SetActive(false);
                firstPersonCam.gameObject.SetActive(true);
                rearViewCam.gameObject.SetActive(true);
                rearViewUI.SetActive(true);

                if (rearViewBorderPanel != null) rearViewBorderPanel.SetActive(true);
                if (mainCamController) mainCamController.enabled = false;
                if (fpCamController) fpCamController.enabled = false;  // Make sure this one stays off
                break;

            case 2: // Third-person + rear view
                mainCamera.tag = "MainCamera";
                firstPersonCam.tag = "Untagged";

                mainCamera.gameObject.SetActive(true);
                firstPersonCam.gameObject.SetActive(false);
                rearViewCam.gameObject.SetActive(true);
                rearViewUI.SetActive(true);

                if (rearViewBorderPanel != null) rearViewBorderPanel.SetActive(true);
                if (mainCamController) mainCamController.enabled = true;
                if (fpCamController) fpCamController.enabled = false;
                break;
        }
    }
}
