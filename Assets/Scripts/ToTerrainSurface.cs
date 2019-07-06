using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTerrainSurface : MonoBehaviour
{
    private Rigidbody m_Rigidbody;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        // Move character to the surface of the terrain
        Vector3 terrainHeight = transform.position;
        terrainHeight.y = Terrain.activeTerrain.SampleHeight(transform.position);
        transform.position = terrainHeight;
        m_Rigidbody.MovePosition(terrainHeight);
    }
}