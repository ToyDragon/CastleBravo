using UnityEngine;
using Unity.VisualScripting;
using Unity.Mathematics;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(WaveMatMgr))]
public class EnemyShootScreenEditor : Editor {
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
                var f = prefab.GetComponent<MeshFilter>();
                f.sharedMesh.bounds = new Bounds(Vector3.up * 10, Vector3.one*100);

                for (int x = -12; x < 12; x++) {
                    for (int z = -12; z < 12; z++) {
                        var s = GameObject.Instantiate(prefab);
                        s.transform.SetParent(transform);
                        s.transform.localPosition = new Vector3(x, 0, z) * 5f;
                        s.transform.localScale = new Vector3(.5f, 1f, .5f);
                    }
                }

                for (int x = -12; x < 12; x++) {
                    for (int z = -12; z < 12; z++) {
                        if (x >= -5 && x < 5 && z >= -5 && z < 5) { continue; }
                        var s = GameObject.Instantiate(prefab);
                        s.transform.SetParent(transform);
                        s.transform.localPosition = new Vector3(x + .25f, 0, z + .25f) * 10f;
                        s.transform.localScale = new Vector3(1f, 1f, 1f);
                        var r = s.GetComponent<MeshRenderer>();
                        r.bounds = new Bounds(r.bounds.center + Vector3.up * 10, new Vector3(40, 80, 40)*2);
                    }
                }

                for (int x = -12; x < 12; x++) {
                    for (int z = -12; z < 12; z++) {
                        if (x >= -5 && x < 5 && z >= -5 && z < 5) { continue; }
                        var s = GameObject.Instantiate(prefab);
                        s.transform.SetParent(transform);
                        s.transform.localPosition = new Vector3(x + .375f, 0, z + .375f) * 20f;
                        s.transform.localScale = new Vector3(2f, 1f, 2f);
                        var r = s.GetComponent<MeshRenderer>();
                        r.bounds = new Bounds(r.bounds.center + Vector3.up * 10, new Vector3(40, 80, 40)*2);
                    }
                }

                DestroyImmediate(prefab);
            }
        }
    }
}
#endif

[ExecuteAlways]
public class WaveMatMgr : MonoBehaviour
{
    public Vector4[] waves = new Vector4[128];
    void Update() {
        var mat = GetComponent<MeshRenderer>().sharedMaterial;
        mat.SetVectorArray("_Waves", waves);
        mat.SetInt("_WaveCount", 128);
        mat.SetVector("_MainLightDir", SunController.instance.transform.forward);
    }
}
