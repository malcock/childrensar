using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class TriggerEvent : UnityEvent<Collider, GameObject> { }

[System.Serializable]
public class CollisionEvent : UnityEvent<Collision, GameObject> { }