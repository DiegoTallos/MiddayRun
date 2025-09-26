using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveObjectMaterial : MonoBehaviour
{   
    void Start()
    {
        #if UNITY_EDITOR
        Material mat = gameObject.GetComponent<MeshRenderer>().material;
        UnityEditor.AssetDatabase.CreateAsset(mat, "Assets/Resources/Materials/" + mat.name + ".mat");
        #endif
    }
}
