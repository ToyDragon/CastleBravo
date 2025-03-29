using UnityEngine;

[ExecuteAlways]
public class SkyboxController : MonoBehaviour
{
    public Transform dirLight;
    public Transform moon;
    private Material mat;
    private Vector3 sunDir;
    public SunController sunController;
    void OnEnable() {
        mat = GetComponent<MeshRenderer>().sharedMaterial;
        sunDir = dirLight.forward;
    }
    void LateUpdate() {
        if (!Application.isPlaying) {
            sunDir = dirLight.forward;
        }
        mat.SetVector("_RotationQuaternion", new Vector4(
            sunController.skyRotationQuaternion.x,
            sunController.skyRotationQuaternion.y,
            sunController.skyRotationQuaternion.z,
            sunController.skyRotationQuaternion.w
        ));
        mat.SetVector("_InvRotationQuaternion", new Vector4(
            sunController.skyInvRotationQuaternion.x,
            sunController.skyInvRotationQuaternion.y,
            sunController.skyInvRotationQuaternion.z,
            sunController.skyInvRotationQuaternion.w
        ));
        mat.SetVector("_MainLightDir", sunDir);
        mat.SetVector("_RotationAxis", sunController.rotationAxis.up);
        mat.SetVector("_RotationForward", sunController.rotationAxis.forward);
        mat.SetVector("_MoonDir", -moon.forward);
    }
}
