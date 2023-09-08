using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameDifficulty
{
    //각 값은 난이도 상승 주기(Seconds)  (상승속도 => 상승주기)
    Easy   = 480, //8분                      50%   =>  +100%
    Normal = 240, //4분                     100%   =>  +  0%
    Hard   = 180  //3분                     125%   =>  - 25%
}

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [Header("Set in inspector")]
    [SerializeField] private Slider slider;

    [SerializeField] private TMP_Text index;

    [Space]
    private static int Difficulty = 1;

    [SerializeField] private float parsedTimeToDifficulty;

    public static int NowDifficulty => Difficulty; //외부 접근용

    //TODO: 난이도 설정은 시작시 받아와서 넣어줘야함
    [SerializeField] private GameDifficulty gameDifficulty = GameDifficulty.Easy;

    private int DifficultyInInt => (int)gameDifficulty;

    private void Awake() => Instance ??= this;

    private void Update()
    {
        parsedTimeToDifficulty = (int)(PlayTimer.playTime / DifficultyInInt)             //정수 부분
                               + PlayTimer.playTime % DifficultyInInt / DifficultyInInt; //소수 부분

        slider.value = parsedTimeToDifficulty - (int)parsedTimeToDifficulty;

        if (Difficulty == (int)parsedTimeToDifficulty + 1) return;

        //난이도가 바뀌여야 할때 바꿔준다. (매 프레임에 text를 수정하는것은 안좋지 않을까)
        Difficulty = (int)parsedTimeToDifficulty + 1;
        index.text = $"{Difficulty}";

        //이벤트 발생
        OnDifficultyChange?.Invoke(this, EventArgs.Empty);
    }

    public static event EventHandler OnDifficultyChange;
}