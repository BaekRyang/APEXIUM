using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Since Tiled animated tiles can be used in a tile layer OR in an object, this class is used to animate an object using an animated tile
/// </summary>
public class AnimatedTileObject : MonoBehaviour
{
    public AnimatedTMXTile m_tileAsset;

    SpriteRenderer m_renderer;
    float m_timer;
    float m_animationSpeed;

    void Start()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_timer = m_tileAsset.m_AnimationStartTime;
        m_animationSpeed = Random.Range(m_tileAsset.m_MinSpeed, m_tileAsset.m_MaxSpeed);
    }

    void Update()
    {
        m_timer += Time.deltaTime * m_animationSpeed;
        int frameNumber = ((int)m_timer) % m_tileAsset.m_AnimatedSprites.Length;
        m_renderer.sprite = m_tileAsset.m_AnimatedSprites[frameNumber];
    }
}
