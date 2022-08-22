using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static int Count;

    [Header("References")]
    [SerializeField] private GameObject enemy;
    [Space]
    [SerializeField] private Transform[] positions;
    [Header("Settings")]
    [SerializeField] private float cooldown = 2;
    
    private float _timer;

    private void Awake()
    {
        Count = 2;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        
        if (Count >= 2)
            return;

        if (_timer < cooldown)
            return;
        
        _timer = 0;

        Instantiate(enemy, positions[Random.Range(0, positions.Length)].position,
            Quaternion.identity);
        Count++;
    }
}