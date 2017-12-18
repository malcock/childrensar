using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class RingColliderPrompt : EditorWindow
{
    enum ColliderType { Capsule, Sphere };
    enum Axis { x, y, z };

    float centerRadius = 0.3f;
    float colliderWidth = 0.4f;
    float height = 0.5f;
    int accuracy=16;
    Axis axis = Axis.x;
    ColliderType colliderType = ColliderType.Sphere;




    [MenuItem("My Tools/Collider/Ring Collider")]
    static void Init()
    {
        RingColliderPrompt window = (RingColliderPrompt)EditorWindow.GetWindow(typeof(RingColliderPrompt));
        window.Show();
    }


    private void OnGUI()
    {
        axis = (Axis)GUILayout.SelectionGrid((int)axis, Enum.GetNames(typeof(Axis)), 3);
        colliderType = (ColliderType)GUILayout.SelectionGrid((int)colliderType, Enum.GetNames(typeof(ColliderType)), 2);
        centerRadius = EditorGUILayout.FloatField("Center Radius",centerRadius);
        colliderWidth = EditorGUILayout.FloatField("Collider Width",colliderWidth);
        if(colliderType != ColliderType.Sphere){
            height = EditorGUILayout.FloatField("Height",height);
        }
        GUILayout.Label("accuracy:" + accuracy.ToString());
        accuracy = Mathf.RoundToInt(GUILayout.HorizontalSlider(accuracy, 8, 32));

        if(GUILayout.Button("Build colliders")){
            OnClickBuild();
        }

    }

    void OnClickBuild(){
        foreach (GameObject rootGameObject in Selection.gameObjects)
        {
            //destroy existing colliders
            Collider[] exisingColliders = rootGameObject.GetComponentsInChildren<Collider>();
            foreach (Collider b in exisingColliders)
            {
                DestroyImmediate(b);
            }

            Vector3 rotationAxis = Vector3.zero;
            Bounds bounds = rootGameObject.GetComponent<Renderer>().bounds;
            Vector3 center = bounds.size/2;
            Debug.Log(bounds.size);
            Debug.Log(center);
            switch(axis){
                case Axis.x:
                    rotationAxis = Vector3.left;

                    break;
                case Axis.y:
                    rotationAxis = Vector3.up;
                    break;
                case Axis.z:
                    rotationAxis = Vector3.forward;
                    break;

            }


            float angleStep = 360 / accuracy;
            for (int i = 0; i < accuracy;i++){
                float angle = (angleStep * i) * Mathf.Deg2Rad;

                Vector3 colliderCenter = new Vector3(Mathf.Sin(angle) * centerRadius, Mathf.Cos(angle) * centerRadius, 0);

                colliderCenter = RotatePointAroundPivot(colliderCenter, Vector3.zero, rotationAxis*90);

                switch(colliderType){
                    case ColliderType.Capsule:
                        CapsuleCollider newCol = rootGameObject.AddComponent<CapsuleCollider>();
                        newCol.radius = colliderWidth;
                        newCol.height = height;

                        switch(axis){
                            case Axis.x:
                                newCol.direction = 1;
                                break;
                            case Axis.y:
                                newCol.direction = 0;
                                break;
                            case Axis.z:
                                newCol.direction = 2;
                                colliderCenter.z = center.z;
                                break;
                                
                        }
                        newCol.center = colliderCenter;
                        break;
                    case ColliderType.Sphere:
                        SphereCollider sphereCol = rootGameObject.AddComponent<SphereCollider>();
                        sphereCol.radius = colliderWidth;
                        sphereCol.center = colliderCenter;
                        break;


                }
            }

        }
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
}
