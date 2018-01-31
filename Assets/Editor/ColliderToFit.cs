using UnityEngine;
using UnityEditor;
using System.Collections;

public class ColliderToFit : MonoBehaviour
{


    [MenuItem("My Tools/Collider/Fit to Children")]
    static void FitToChildren()
    {
        foreach(GameObject rootGameObject in Selection.gameObjects){

            BoxCollider[] exisingColliders = rootGameObject.GetComponentsInChildren<BoxCollider>();
            foreach(BoxCollider b in exisingColliders){
                DestroyImmediate(b);
            }

            Transform[] children = rootGameObject.GetComponentsInChildren<Transform>();

            foreach(Transform child in children){
                if (child.GetComponent<SkinnedMeshRenderer>() || child.childCount!=1) continue;



                BoxCollider box = child.gameObject.AddComponent<BoxCollider>();

                float mag = Vector3.Magnitude(child.GetChild(0).position - child.position);

                box.size = new Vector3(mag, 0.05f, 0.05f);
                box.center = new Vector3(-mag/2, 0, 0);

            }
          

        }

    }





    [MenuItem("My Tools/Collider/Destroy Colliders")]
    static void DestroyColliders()
    {
        foreach (GameObject rootGameObject in Selection.gameObjects)
        {

            BoxCollider[] exisingColliders = rootGameObject.GetComponentsInChildren<BoxCollider>();
            foreach (BoxCollider b in exisingColliders)
            {
                DestroyImmediate(b);
            }




        }

    }

}