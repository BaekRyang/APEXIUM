using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private bool play;
    
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private float    _frameRate = .05f;

    private SpriteRenderer _spriteRenderer;
    private int            _currentFrame;
    private float          _timer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (play)
        {
            play = false;
            Play();
        }
    }

    public async void Play()
    {
        while (true)
        {
            _timer += Time.deltaTime;

            if (_timer >= _frameRate)
            {
                _timer = 0;
                _currentFrame++;

                if (_currentFrame >= _sprites.Length)
                {
                    _currentFrame = 0;
                    break;
                }

                _spriteRenderer.sprite = _sprites[_currentFrame];
            }

            await UniTask.Yield();
        }
    }
}