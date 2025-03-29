using UnityEngine;

public class SextantController : MonoBehaviour
{
    public Camera forwardCamera;
    public Camera upwardCamera;
    public Transform armPivot;
    private RenderTexture forwardRenderTexture;
    private RenderTexture upwardRenderTexture;
    public MeshRenderer leftRenderer;
    public MeshRenderer rightRenderer;
    public bool focusing = false;
    public Transform targetLocation;
    private Vector2Int lastScreenSize;
    public float sextantAngle = 0;
    void OnEnable() {
        leftRenderer.enabled = false;
        rightRenderer.enabled = false;
        leftRenderer.material = new Material(leftRenderer.material);
        rightRenderer.material = new Material(rightRenderer.material);
    }
    void Update() {
        if (focusing) {
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            if (screenSize != lastScreenSize) {
                lastScreenSize = screenSize;
                if (forwardRenderTexture != null) { forwardRenderTexture.Release(); }
                if (upwardRenderTexture != null) { upwardRenderTexture.Release(); }
                forwardRenderTexture = new RenderTexture(screenSize.x/2, screenSize.y, 32);
                leftRenderer.material.mainTexture = forwardRenderTexture;
                forwardCamera.targetTexture = forwardRenderTexture;

                upwardRenderTexture = new RenderTexture(screenSize.x - screenSize.x/2, screenSize.y, 32);
                rightRenderer.material.mainTexture = upwardRenderTexture;
                upwardCamera.targetTexture = upwardRenderTexture;
            }
        }
        leftRenderer.enabled = focusing;
        rightRenderer.enabled = focusing;
        upwardCamera.enabled = focusing;
        forwardCamera.enabled = focusing;

        armPivot.localRotation = Quaternion.Euler(
            armPivot.localRotation.eulerAngles.x,
            armPivot.localRotation.eulerAngles.y,
            -25 + sextantAngle
        );

        upwardCamera.transform.localRotation = Quaternion.Euler(
            -sextantAngle,
            upwardCamera.transform.localRotation.eulerAngles.y,
            upwardCamera.transform.localRotation.eulerAngles.z
        );
    }

    void LateUpdate() {
        transform.position = targetLocation.position;
        transform.rotation = targetLocation.rotation;
        var angs = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(
            angs.x,
            angs.y,
            (angs.z > 180 ? angs.z - 360 : angs.z) * .25f
        );
    }
}
