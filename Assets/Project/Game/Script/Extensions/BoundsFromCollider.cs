using UnityEngine;

public class BoundsFromCollider : MonoBehaviour
{
    private Vector3 _preCenter;
    private Vector3 _preSize;
    
    
    private void Awake()
    {
        var mf = GetComponent<MeshFilter>();
        var box = GetComponent<BoxCollider>();
        if (mf && box)
        {
            var mesh = mf.sharedMesh;
            _preCenter = mesh.bounds.center;
            _preSize = mesh.bounds.size;
            
            mesh.bounds = new Bounds(box.center, box.size);
        }
    }

    private void OnDisable()
    {
        var mf = GetComponent<MeshFilter>();
        var box = GetComponent<BoxCollider>();
        if (mf && box)
        {
            var mesh = mf.sharedMesh;
            
            mesh.bounds = new Bounds(_preCenter, _preSize);
        }
    }
    
    void OnValidate()
    {
        var mf = GetComponent<MeshFilter>();
        var box = GetComponent<BoxCollider>();
        if (mf && box)
        {
            var mesh = mf.sharedMesh;
            _preCenter = mesh.bounds.center;
            _preSize = mesh.bounds.size;
            
            mesh.bounds = new Bounds(box.center, box.size);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (TryGetComponent(out Renderer renderer))
        {
            var bounds = renderer.bounds;

            Gizmos.color = new Color(1, 0, 0, 0.8f);
            Gizmos.DrawCube(bounds.center, bounds.size);
        }
    }
}
