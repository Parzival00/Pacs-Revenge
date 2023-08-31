using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualization : MonoBehaviour
{
    [SerializeField] float size = 1f;
    [SerializeField] int height = 50;
    [SerializeField] int width = 50;
    [SerializeField] Vector3 centerOffset;
    [SerializeField] bool startGridFromOrigin;
    [SerializeField] bool on = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (on)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3 offset = Vector3.zero;
                    if (!startGridFromOrigin)
                        offset = -new Vector3(width, 0, height) / 2f * size;
                    Gizmos.DrawWireCube(transform.position + offset + new Vector3(x, 0, y) * size + centerOffset, Vector3.one * size);
                }
            }
        }
    }
}
