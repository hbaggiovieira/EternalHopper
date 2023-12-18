using UnityEngine;
using System.Collections.Generic;

public class PlatformGenerator : MonoBehaviour
{
    public List<GameObject> platformPrefabs;
    private float stageWidth;

    private Vector3 nextSpawnPosition;
    private int platformCount = 0;
    private int currentPrefabIndex = 0;
    private int currentLevel = 1;

    public Transform playerTransform;
    public Transform cameraTransform;
    public int initialPlatformBuffer = 20;
    public float generationThreshold = 15.0f;

    private int levelUpPlatformConstant = 50;
    private float initialPlatformWidthPercent = 40;
    private const float minWidthPercent = 25f;
    private const float decrementPercent = 2f;

    private float verticalDistance = 3f;
    private const float verticalIncrement = 0.1f;

    void Start()
    {
        nextSpawnPosition = transform.position;
        CalculateStageWidth();
        GenerateInitialPlatforms();
    }

    void Update()
    {
        if (playerTransform.position.y > nextSpawnPosition.y - generationThreshold)
        {
            GeneratePlatform();
        }

        DestroyOffScreenPlatforms();
    }

    private void CalculateStageWidth()
    {
        float left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float right = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        stageWidth = right - left;
    }

    private void GenerateInitialPlatforms()
    {
        for (int i = 0; i < initialPlatformBuffer; i++)
        {
            GeneratePlatform();
        }
    }

    private void GeneratePlatform()
    {
        bool isLevelUpPlatform = platformCount % levelUpPlatformConstant == 0 && platformCount != 0;
        GameObject platformPrefab = platformPrefabs[currentPrefabIndex];

        // Calcular a nova largura com base no nível atual
        float platformWidthPercent = Mathf.Max(initialPlatformWidthPercent - (decrementPercent * (currentLevel - 1)), minWidthPercent);
        float newWidth = isLevelUpPlatform ? stageWidth : stageWidth * platformWidthPercent / 100f;

        // Instanciar a nova plataforma
        Vector3 spawnPosition = CalculateSpawnPosition(newWidth);

        if (platformCount == 0)
        {
            spawnPosition = new Vector3(0f, -0.5f, 0f);
        }

        GameObject newPlatform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);

        // Ajustar o BoxCollider2D para a nova largura
        BoxCollider2D platformCollider = newPlatform.GetComponent<BoxCollider2D>();
        platformCollider.size = new Vector2(newWidth, platformCollider.size.y);

        // Ajustar o tamanho do SpriteRenderer
        AdjustSpriteRendererSize(newPlatform, newWidth);

        nextSpawnPosition.y += verticalDistance;
        platformCount++;

        // Incrementa o nível e ajusta a distância vertical se for uma plataforma de subida de nível
        if (isLevelUpPlatform)
        {
            currentLevel++;
            verticalDistance += verticalIncrement;
            currentPrefabIndex = (platformPrefabs.Count > currentPrefabIndex + 1) ? currentPrefabIndex + 1 : currentPrefabIndex;
        }
    }

    private void AdjustSpriteRendererSize(GameObject platform, float newWidth)
    {
        SpriteRenderer spriteRenderer = platform.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.size = new Vector2(newWidth, spriteRenderer.size.y);
        }
    }

    private Vector3 CalculateSpawnPosition(float width)
    {
        float halfWidth = width / 2;
        float xPosition = Random.Range(-stageWidth / 2 + halfWidth, stageWidth / 2 - halfWidth);
        return new Vector3(xPosition, nextSpawnPosition.y, 0);
    }

    private void DestroyOffScreenPlatforms()
    {
        foreach (GameObject platform in GameObject.FindGameObjectsWithTag("Platform"))
        {
            if (platform.transform.position.y < cameraTransform.position.y - 20)
            {
                Destroy(platform);
            }
        }
    }
}
