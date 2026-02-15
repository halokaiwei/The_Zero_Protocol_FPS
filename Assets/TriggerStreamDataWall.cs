using UnityEngine;

public class TriggerDataWall : DestructibleObject
{
    public StreamData targetStream;
    public MatrixWorldTrigger worldTrigger;
    protected override void Die()
    {

        if (targetStream != null)
        {
            worldTrigger.RevealTruth();
            targetStream.StartDataStream();
            Debug.Log("D Sector Truth Reveal.");
        }

        base.Die();
    }
}