using UnityEngine;

[ExecuteAlways]
public class SunController : MonoBehaviour
{
    public float dayLength = 120;
    public float maxYRot = 80;
    public float maxXRot = 80;
    public Renderer oceanRenderer;
    private Material oceanMaterial;
    private Color initialShallowColor;
    private Color initialHorizonColor;
    private Color initialFoamColor;
    private Color initialCaveColor;
    public Transform rotationAxis;
    private Quaternion initialRotation;
    public Quaternion skyRotationQuaternion;
    public Quaternion skyInvRotationQuaternion;
    public float time;
    public int timeSpeed = 1;
    void OnEnable() {
        if (Application.isPlaying) {
            oceanMaterial = oceanRenderer.sharedMaterial = new Material(oceanRenderer.sharedMaterial);
            initialShallowColor = oceanMaterial.GetColor("_ShallowColor");
            initialHorizonColor = oceanMaterial.GetColor("_HorizonColor");
            initialFoamColor = oceanMaterial.GetColor("_SurfaceFoamColor");
            initialCaveColor = oceanMaterial.GetColor("_CaveColor");
        }
        initialRotation = transform.rotation;
        foreach (var r in rotationAxis.GetComponentsInChildren<Renderer>()) {
            r.enabled = false;
        }
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.RightArrow)) { timeSpeed += 1; }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { timeSpeed -= 1; }
        time += Time.deltaTime * timeSpeed;

        float t = (time / dayLength) % 1;
        if (!Application.isPlaying) { t = 0; }

        skyRotationQuaternion = Quaternion.AngleAxis(t * 360, rotationAxis.up);
        skyInvRotationQuaternion = Quaternion.AngleAxis(t * 360, -rotationAxis.up);

        if (Application.isPlaying) {
            transform.rotation = skyInvRotationQuaternion * initialRotation;
            
            float dayT = (Mathf.Clamp(-transform.forward.y, -.2f, .2f) + .2f) / .4f;
            oceanMaterial.SetColor("_ShallowColor",     Color.Lerp(Color.black, initialShallowColor, dayT));
            oceanMaterial.SetColor("_HorizonColor",     Color.Lerp(Color.black, initialHorizonColor, dayT));
            oceanMaterial.SetColor("_SurfaceFoamColor", Color.Lerp(Color.black, initialFoamColor, dayT));
            oceanMaterial.SetColor("_CaveColor",        Color.Lerp(Color.black, initialCaveColor, dayT));
        }
    }
}
