using System;
using System.Collections;
using UnityEngine;

public class BossAreaEntrance : MonoBehaviour
{
    [SerializeField] private SpriteRenderer wallSpriteRenderer;
    [SerializeField] private float          wallHeight             = 6f;
    [SerializeField] private float          wallCloseSpeedInSecond = 3f;

    [SerializeField] private bool bossCleared, locked;

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (!_other.CompareTag("Player")) return;

        StartCoroutine(CloseWall());

        // EventBus.Publish(new BossAreaEntranceEvent());
    }

    private IEnumerator CloseWall()
    {
        if (locked) yield break;
        
        while (wallSpriteRenderer.size.y < wallHeight)
        {
            wallSpriteRenderer.size += new Vector2(0, wallCloseSpeedInSecond * Time.deltaTime);
            yield return null;
        }
        
        wallSpriteRenderer.size = new Vector2(wallSpriteRenderer.size.x, wallHeight);
        
        locked = true;
    }

    private IEnumerator OpenWall()
    {
        if (!bossCleared) yield break;
        
        while (wallSpriteRenderer.size.y > 0)
        {
            wallSpriteRenderer.size -= new Vector2(0, wallCloseSpeedInSecond * Time.deltaTime);
            yield return null;
        }
        
        wallSpriteRenderer.size = new Vector2(wallSpriteRenderer.size.x, 0);
        
        locked = false;
    }
}