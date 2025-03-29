using UnityEngine;

[ExecuteAlways]
public class SkyboxController : MonoBehaviour
{
    public Transform dirLight;
    public Transform moon;
    private Material mat;
    void OnEnable() {
        mat = GetComponent<MeshRenderer>().sharedMaterial;
    }
    void Update() {
        mat.SetVector("_MainLightDir", -dirLight.forward);
        mat.SetVector("_MoonDir", -moon.forward);
    }
}
