using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fadable : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FadeOut(float fadeTime)
    {
        StartCoroutine(DoFadeOut(fadeTime));
    }

    public void FadeIn(float fadeTime)
    {
        StartCoroutine(DoFadeIn(fadeTime));
    }


    IEnumerator DoFadeIn(float fadeTime)
    {

        float timeout = fadeTime;

        Material[] materials = GetMaterials();
        while (timeout > 0)
        {
            foreach (Material m in materials)
            {

                Color c = m.color;
                c.a = 1 - (timeout / fadeTime);
                m.color = c;
                timeout -= Time.deltaTime;
            }
            yield return null;
        }
    }

    IEnumerator DoFadeOut(float fadeTime)
    {

        float timeout = fadeTime;

        Material[] materials = GetMaterials();
        Debug.Log(name + " materials: " + materials.Length);
        while (timeout > 0)
        {
            foreach (Material m in materials)
            {

                Color c = m.color;
                c.a = timeout / fadeTime;
                m.color = c;
                timeout -= Time.deltaTime;
            }
            yield return null;
        }
    }

    Material[] GetMaterials()
    {
        List<Material> materials = new List<Material>();
        Renderer[] renders = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renders)
        {
            materials.AddRange(r.materials);
        }

        return materials.ToArray();
    }


}
