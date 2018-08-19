using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns killerbird's in a regular intervall.
/// </summary>
public class BirdManager : MonoBehaviour
{
    public float spawnSpeed = 0.2f;
    public Transform target, spawnPos;

    public GameObject prefab;


    private void Start()
    {
        StartCoroutine(Spawn());
    }


    private IEnumerator Spawn ()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnSpeed);

            var bird = Instantiate(prefab, SpawnPos(), Quaternion.identity);
            bird.GetComponent<Bird>().player = target;
        }
    }


    private Vector3 SpawnPos ()
    {
        var x = Random.Range(-15, 15);
        var result = new Vector3(spawnPos.position.x + x, spawnPos.position.y, spawnPos.position.z);
        return result;
    }
}
