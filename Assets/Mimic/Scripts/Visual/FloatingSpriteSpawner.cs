using System.Collections.Generic;
using UnityEngine;

public class FloatingSpriteSpawner : MonoBehaviour
{
    [SerializeField] private Sprite firstSprite;
    [SerializeField] private Sprite secondSprite;
    [SerializeField] private Sprite thirdSprite;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float randomIntervalOffset = 0.2f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private Vector2 randomHorizontalOffsetRange = new Vector2(-0.2f, 0.2f);

    private readonly List<SpawnedSprite> _spawnedSprites = new List<SpawnedSprite>();
    private float _spawnTimer;
    private float _nextSpawnDelay;

    private void OnEnable()
    {
        ResetSpawnTimer();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        UpdateSpawner(deltaTime);
        UpdateSpawnedSprites(deltaTime);
    }

    private void UpdateSpawner(float deltaTime)
    {
        _spawnTimer += deltaTime;

        if (_spawnTimer < _nextSpawnDelay)
        {
            return;
        }

        _spawnTimer = 0f;
        SpawnSprite();
        ResetSpawnTimer();
    }

    private void UpdateSpawnedSprites(float deltaTime)
    {
        for (int i = _spawnedSprites.Count - 1; i >= 0; i--)
        {
            SpawnedSprite spawnedSprite = _spawnedSprites[i];

            if (spawnedSprite.Transform == null)
            {
                _spawnedSprites.RemoveAt(i);
                continue;
            }

            spawnedSprite.Transform.position += Vector3.up * moveSpeed * deltaTime;
            spawnedSprite.RemainingLifetime -= deltaTime;

            if (spawnedSprite.RemainingLifetime <= 0f)
            {
                Destroy(spawnedSprite.Transform.gameObject);
                _spawnedSprites.RemoveAt(i);
                continue;
            }

            _spawnedSprites[i] = spawnedSprite;
        }
    }

    private void SpawnSprite()
    {
        Sprite sprite = GetRandomSprite();

        if (sprite == null)
        {
            return;
        }

        GameObject spawnedObject = new GameObject("Floating Sprite");
        spawnedObject.transform.position = GetSpawnPosition();

        SpriteRenderer spriteRenderer = spawnedObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;

        SpawnedSprite spawnedSprite = new SpawnedSprite(spawnedObject.transform, lifetime);
        _spawnedSprites.Add(spawnedSprite);
    }

    private Vector3 GetSpawnPosition()
    {
        float randomX = Random.Range(randomHorizontalOffsetRange.x, randomHorizontalOffsetRange.y);
        Vector3 randomOffset = new Vector3(randomX, 0f, 0f);
        return transform.position + spawnOffset + randomOffset;
    }

    private Sprite GetRandomSprite()
    {
        List<Sprite> availableSprites = new List<Sprite>(3);

        if (firstSprite != null)
        {
            availableSprites.Add(firstSprite);
        }

        if (secondSprite != null)
        {
            availableSprites.Add(secondSprite);
        }

        if (thirdSprite != null)
        {
            availableSprites.Add(thirdSprite);
        }

        if (availableSprites.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, availableSprites.Count);
        return availableSprites[randomIndex];
    }

    private void ResetSpawnTimer()
    {
        float minDelay = Mathf.Max(0f, spawnInterval - randomIntervalOffset);
        float maxDelay = Mathf.Max(minDelay, spawnInterval + randomIntervalOffset);
        _nextSpawnDelay = Random.Range(minDelay, maxDelay);
    }

    private struct SpawnedSprite
    {
        public Transform Transform;
        public float RemainingLifetime;

        public SpawnedSprite(Transform transform, float remainingLifetime)
        {
            Transform = transform;
            RemainingLifetime = remainingLifetime;
        }
    }
}
