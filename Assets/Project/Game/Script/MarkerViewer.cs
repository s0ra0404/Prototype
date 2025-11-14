using RaycastHitExtensions;
using UnityEngine;

public class MarkerViewer : MonoBehaviour
{
    [SerializeField] private GameObject _markerPrefab;
    [SerializeField] private LayerMask _playerLayerMask;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.IsInLayerMask(_playerLayerMask))
        {
            _markerPrefab.SetActive(true);
        }
    }
}
