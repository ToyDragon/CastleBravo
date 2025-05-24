using UnityEngine;
using Unity.VisualScripting;
using Unity.Mathematics;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(WaveMatMgr))]
public class WaveMatMgrEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var t = (WaveMatMgr)target;
        if (GUILayout.Button("waves")) { 
            var mat = target.GetComponent<Renderer>().sharedMaterial;
            for (int i = 0; i < 128; i++) {
                Vector2 r = UnityEngine.Random.insideUnitCircle.normalized;
                float phase = UnityEngine.Random.Range(0, Mathf.PI);
                float freq = i/8 + 1;
                t.waves[i] = new Vector4(r.x, r.y, phase, freq);
            }
        }


        if (GUILayout.Button("planes")) {
            var transform = t.transform;
            if (transform.childCount > 0) {
                for (int i = transform.childCount - 1; i >= 0; i--) {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            } else {
                var prefab = GameObject.Instantiate(t.gameObject);
                DestroyImmediate(prefab.GetComponent<WaveMatMgr>());
                int planeCount = 0;

                for (int x = -11; x < 11; x++) {
                    for (int z = -11; z < 11; z++) {
                        planeCount++;
                        var s = GameObject.Instantiate(prefab);
                        s.transform.SetParent(transform);
                        s.transform.localPosition = new Vector3(x, 0, z) * 5f;
                        s.transform.localScale = new Vector3(.5f, 1f, .5f);
                        var r = s.GetComponent<MeshRenderer>();
                        r.localBounds = new Bounds(Vector3.zero, new Vector3(20, 20, 20));
                    }
                }

                for (int x = -11; x < 11; x++) {
                    for (int z = -11; z < 11; z++) {
                        if (x >= -5 && x < 5 && z >= -5 && z < 5) { continue; }
                        planeCount++;
                        var s = GameObject.Instantiate(prefab);
                        s.transform.SetParent(transform);
                        float offset = .25f;
                        s.transform.localPosition = new Vector3(x + offset, 0, z + offset) * 10f;
                        s.transform.localScale = new Vector3(1f, 1f, 1f);
                        var r = s.GetComponent<MeshRenderer>();
                        r.localBounds = new Bounds(r.bounds.center + Vector3.up * 20, new Vector3(40, 80, 40)*3);
                    }
                }

                for (int x = -9; x < 9; x++) {
                    for (int z = -9; z < 9; z++) {
                        if (x >= -5 && x < 5 && z >= -5 && z < 5) { continue; }
                        planeCount++;
                        var s = GameObject.Instantiate(prefab);
                        s.transform.SetParent(transform);
                        float offset = .25f + .125f;
                        s.transform.localPosition = new Vector3(x + offset, 0, z + offset) * 20f;
                        s.transform.localScale = new Vector3(2f, 1f, 2f);
                        var r = s.GetComponent<MeshRenderer>();
                        r.localBounds = new Bounds(r.bounds.center + Vector3.up * 20, new Vector3(40, 80, 40)*3);
                    }
                }

                for (int x = -5; x < 5; x++) {
                    for (int z = -5; z < 5; z++) {
                        if (x >= -1 && x < 1 && z >= -1 && z < 1) { continue; }
                        planeCount++;
                        var s = GameObject.Instantiate(prefab);
                        s.transform.SetParent(transform);
                        float offset = .25f + .125f + .0625f + .03125f + .03125f*.5f;
                        s.transform.localPosition = new Vector3(x + offset, 0, z + offset) * 160f;
                        s.transform.localScale = new Vector3(16f, 1f, 16f);
                        var r = s.GetComponent<MeshRenderer>();
                        r.localBounds = new Bounds(r.bounds.center + Vector3.up * 20, new Vector3(40, 80, 40)*3);
                    }
                }

                for (int x = -5; x < 5; x++) {
                    for (int z = -5; z < 5; z++) {
                        if (x >= -1 && x < 1 && z >= -1 && z < 1) { continue; }
                        if (Mathf.Abs(x*2 + 1) + Mathf.Abs(z*2 + 1) > 8) { continue; }
                        planeCount++;
                        var s = GameObject.Instantiate(prefab);
                        s.transform.SetParent(transform);
                        float offset = .25f + .125f + .0625f + .03125f + .03125f*.5f + .03125f*.25f + .03125f*.125f;
                        s.transform.localPosition = new Vector3(x + offset, 0, z + offset) * 640f;
                        s.transform.localScale = new Vector3(64f, 1f, 64f);
                        var r = s.GetComponent<MeshRenderer>();
                        r.localBounds = new Bounds(r.bounds.center + Vector3.up * 20, new Vector3(40, 80, 40)*3);
                    }
                }

                Debug.Log($"Tri count: {planeCount * 200}");

                DestroyImmediate(prefab);
            }
        }
    }
}
#endif

[ExecuteAlways]
public class WaveMatMgr : MonoBehaviour
{
    public static WaveMatMgr instance;
    public Vector4[] waves = new Vector4[128];
    public int targetWaveCount = 128;
    public float time;
    void OnEnable() {
        instance = this;
        time = 0;
    }
    void Update() {
        var mat = GetComponent<MeshRenderer>().sharedMaterial;
        time += Time.deltaTime;
        mat.SetVectorArray("_Waves", waves);
        mat.SetInt("_WaveCount", targetWaveCount);
        mat.SetVector("_MainLightDir", SunController.instance.transform.forward);
        mat.SetFloat("_WaveTime", time);
    }

    public float GetWaveT(Vector2 pos, Vector4 wave) {
        return Vector2.Dot(pos*Mathf.Pow(wave.w + 1, 1.5f)/16.0f, wave.XY().normalized) + wave.z + (1 + wave.w/3)*time*.25f;
    }

    public float HeightAt(Vector2 pos) {
        float h = 0;
        for (int waveI = 0; waveI < targetWaveCount; waveI++) {
            float4 wave = waves[waveI];
            float t = GetWaveT(pos, wave);
            h += .25f * Mathf.Sin(t) / wave.w;
        }
        return h;
    }
}
