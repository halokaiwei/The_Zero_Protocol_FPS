using UnityEngine;

public class PuzzleNode : MonoBehaviour, IDamageable
{
    public int nodeIndex;
    public bool isOn = false;
    private bool isLocked = false;
    private MeshFilter meshFilter;
    private PuzzleTwoManager manager;

    [Header("Mesh References")]
    public Mesh offMesh; 
    public Mesh onMesh; 

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        manager = GetComponentInParent<PuzzleTwoManager>();
        UpdateMesh();
    }

    public void TakeDamage(int damage)
    {
        if (isLocked) return;
        manager.OnNodeHit(nodeIndex);
    }

    public void Toggle()
    {
        if (isLocked) return;
        isOn = !isOn;
        UpdateMesh();
    }

    void UpdateMesh()
    {
        if (meshFilter != null)
        {
            meshFilter.mesh = isOn ? onMesh : offMesh;
        }
    }

    public void LockNode()
    {
        isLocked = true;
    }
}