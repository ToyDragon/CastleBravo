using System.Collections.Generic;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SextantController : MonoBehaviour
{
    public Camera forwardCamera;
    public Camera upwardCamera;
    public Camera pipCamera;
    public Transform armPivot;
    private RenderTexture pipRenderTexture;
    private RenderTexture sextantRenderTextureLeft;
    public MeshRenderer sextantRendererLeft;
    private RenderTexture sextantRenderTextureRight;
    public MeshRenderer sextantRendererRight;
    public bool focusing = false;
    public Transform targetLocation;
    private Vector2Int lastScreenSize;
    public float sextantAngle = 0;
    public TMP_Text angleText;
    public RawImage pipImage;
    void OnEnable() {
        sextantRendererLeft.enabled = sextantRendererRight.enabled = false;
        sextantRendererLeft.material = new Material(sextantRendererLeft.material);
        sextantRendererRight.material = new Material(sextantRendererRight.material);
        pipRenderTexture = new RenderTexture(400, 300, 32);
        pipCamera.targetTexture = pipRenderTexture;
        pipImage.texture = pipRenderTexture;

        var filter = sextantRendererLeft.GetComponent<MeshFilter>();
        filter.sharedMesh = Mesh.Instantiate(filter.sharedMesh);
        var uvs = new List<Vector2>();
        filter.sharedMesh.GetUVs(0, uvs);
        for (int i = 0; i < uvs.Count; i++) {
            if (uvs[i].x > .5f) {
                uvs[i] = new Vector2(.5f, uvs[i].y);
            }
        }
        filter.sharedMesh.SetUVs(0, uvs);
        filter.sharedMesh.UploadMeshData(true);

        filter = sextantRendererRight.GetComponent<MeshFilter>();
        filter.sharedMesh = Mesh.Instantiate(filter.sharedMesh);
        uvs.Clear();
        filter.sharedMesh.GetUVs(0, uvs);
        for (int i = 0; i < uvs.Count; i++) {
            if (uvs[i].x < .5f) {
                uvs[i] = new Vector2(.5f, uvs[i].y);
            }
        }
        filter.sharedMesh.SetUVs(0, uvs);
        filter.sharedMesh.UploadMeshData(true);
    }
    void Update() {
        if (focusing) {
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            if (screenSize != lastScreenSize) {
                lastScreenSize = screenSize;
                if (sextantRenderTextureLeft != null) { sextantRenderTextureLeft.Release(); }
                sextantRenderTextureLeft = new RenderTexture(screenSize.x, screenSize.y, 32);
                sextantRendererLeft.material.mainTexture = sextantRenderTextureLeft;
                forwardCamera.targetTexture = sextantRenderTextureLeft;
                if (sextantRenderTextureRight != null) { sextantRenderTextureRight.Release(); }
                sextantRenderTextureRight = new RenderTexture(screenSize.x, screenSize.y, 32);
                sextantRendererRight.material.mainTexture = sextantRenderTextureRight;
                upwardCamera.targetTexture = sextantRenderTextureRight;
            }
        }
        sextantRendererLeft.enabled = sextantRendererRight.enabled = focusing;
        upwardCamera.enabled = focusing;
        forwardCamera.enabled = focusing;
        pipCamera.enabled = focusing;
        pipImage.enabled = focusing;

        armPivot.localRotation = Quaternion.Euler(
            armPivot.localRotation.eulerAngles.x,
            armPivot.localRotation.eulerAngles.y,
            -25 + sextantAngle
        );

        upwardCamera.transform.localRotation = Quaternion.Euler(
            -sextantAngle*1.5F,
            upwardCamera.transform.localRotation.eulerAngles.y,
            upwardCamera.transform.localRotation.eulerAngles.z
        );
        angleText.text = Mathf.RoundToInt(sextantAngle*1.5f).ToString();

        var delta = targetLocation.position - transform.position;
        float step = Time.deltaTime * 3f;
        transform.position = delta.magnitude < step ? targetLocation.position : transform.position + delta.normalized*step;
    }

    void LateUpdate() {
        transform.rotation = targetLocation.rotation;
        var angs = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(
            angs.x,
            angs.y,
            (angs.z > 180 ? angs.z - 360 : angs.z) * .25f
        );
        pipCamera.transform.rotation = Quaternion.Euler(0, pipCamera.transform.rotation.eulerAngles.y, 0);
    }
}
