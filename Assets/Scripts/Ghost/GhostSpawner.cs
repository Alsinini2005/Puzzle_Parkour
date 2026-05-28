using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostSpawner : MonoBehaviour
{
    public GameObject ghostPrefab;
    private bool ghostSpawned = false;
    private bool isReadyToSpawn = false;

    public void PrepareForGhost()
{
    Debug.Log("👻 PrepareForGhost CALLED");

    StopAllCoroutines();
    StartCoroutine(TrySpawnGhostSequence());
}

IEnumerator SpawnRoutine()
{
    yield return new WaitForSeconds(0.2f); 
    
    int attempts = 0;
    while (!ghostSpawned && attempts < 5)
    {
        TrySpawnGhost();
        if (!ghostSpawned)
        {
            yield return new WaitForSeconds(0.5f);
            attempts++;
        }
    }
}

    IEnumerator TrySpawnGhostSequence()
    {
        yield return new WaitForSeconds(0.5f);
        TrySpawnGhost();

        if (!ghostSpawned)
        {
            yield return new WaitForSeconds(1f);
            TrySpawnGhost();
        }

        if (!ghostSpawned)
        {
            yield return new WaitForSeconds(1f);
            TrySpawnGhost();
        }

        if (!ghostSpawned)
        {
            if (GhostManager.instance != null)
            {
                GhostManager.instance.PrintStatus();
            }
        }
    }

    void TrySpawnGhost()
{
    Debug.Log("👻 Trying Spawn Ghost");

    if (GhostManager.instance == null)
    {
        Debug.Log("❌ GhostManager NULL");
        return;
    }

    Debug.Log($"Has Data: {GhostManager.instance.HasGhostData}");

    if (!GhostManager.instance.HasGhostData)
    {
        Debug.Log("❌ No Ghost Data");
        return;
    }

    List<GhostFrame> ghostData = GhostManager.instance.GetGhostData();

    Debug.Log($"Frames: {ghostData.Count}");

    SpawnGhostWithData(ghostData);
}

    void SpawnGhostWithData(List<GhostFrame> ghostData)
    {
        if (ghostPrefab == null)
        {
            return;
        }

        GameObject ghost = Instantiate(ghostPrefab);

        if (ghost == null)
        {
            return;
        }

        GhostPlayer player = ghost.GetComponent<GhostPlayer>();

        if (player == null)
        {
            Destroy(ghost);
            return;
        }

        player.framesToPlay = new List<GhostFrame>(ghostData);
        player.ResetPlayback();

        if (ghostData.Count > 0)
        {
            ghost.transform.position = ghostData[0].position + Vector3.up * 2f;
            ghost.transform.rotation = ghostData[0].rotation;
        }

        Renderer[] renderers = ghost.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }

        ghostSpawned = true;
    }
}