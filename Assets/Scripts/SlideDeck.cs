using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class SlideDeck : MonoBehaviour
{
    public static SlideDeck instance;
    public List<GameObject> slides;
    public int slide = 0;
    private int shownSlide = -1;
    public TMPro.TMP_Text progress;
    void OnEnable()
    {
        instance = this;
        foreach (var go in slides) {
            go.SetActive(false);
        }
    }
    void Update()
    {
        if (shownSlide != slide) {
            if (shownSlide >= 0) { slides[shownSlide].SetActive(false); }
            slides[slide].SetActive(true);
            shownSlide = slide;
            var progressStr = "";
            for (int i = 0; i < slides.Count; i++) {
                progressStr += i == slide ? "|" : ".";
                if (i < slides.Count - 1) {
                    progressStr += "  ";
                }
            }
            progress.text = progressStr;
        }
    }
    public void Next() {
        if (slide < slides.Count - 1) {
            slide++;
        }
    }
    public void Prev() {
        if (slide > 0) {
            slide--;
        }
    }
}
