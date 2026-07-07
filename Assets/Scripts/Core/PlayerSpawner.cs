using UnityEngine;

/// <summary>
/// Umísti do HERNÍ scény na prázdný GameObject "PlayerSpawner".
/// Podle volby v GameManageru spawne buď Human, nebo Mosquito prefab
/// na daném spawn pointu a nastaví na hlavní kameru CameraFollow cíl.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject humanPrefab;
    [SerializeField] private GameObject mosquitoPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CameraFollow cameraFollow; // volitelné

    private void Start()
    {
        GameManager.CharacterType choice = GameManager.CharacterType.Human;

        if (GameManager.Instance != null)
        {
            choice = GameManager.Instance.SelectedCharacter;
        }
        else
        {
            Debug.LogWarning("GameManager nenalezen (spouštíš scénu přímo?). Použije se výchozí volba: Člověk.");
        }

        GameObject prefabToSpawn = choice == GameManager.CharacterType.Human ? humanPrefab : mosquitoPrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogError("PlayerSpawner: prefab pro zvolenou postavu není přiřazen v Inspectoru!");
            return;
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        GameObject player = Instantiate(prefabToSpawn, pos, rot);

        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(player.transform);
        }
    }
}
