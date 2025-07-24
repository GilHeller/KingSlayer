using UnityEngine;

public class ReplaceWithOther : MonoBehaviour
{
    [Header("Replacement Prefab")]
    public GameObject replacementPrefab;

    public void ReplaceNow()
    {

        if (replacementPrefab == null)
        {
            Debug.LogWarning("[ReplaceWithOther] No replacement prefab assigned!");
            return;
        }

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Transform parent = transform.parent;

        Debug.Log($"[ReplaceWithOther] Replacing {gameObject.name} with {replacementPrefab.name} at {pos}");

        Destroy(gameObject);
        Debug.Log($"[ReplaceWithOther] Destroyed original object: {gameObject.name}");

        GameObject newObj = Instantiate(replacementPrefab, pos, rot, parent);
        newObj.name = replacementPrefab.name;

        Debug.Log($"[ReplaceWithOther] Successfully instantiated: {newObj.name}");

    }
}