using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayData
{
    public int        characterIndex = -1;
    public PlayerData characterData  = Addressables.LoadAssetAsync<PlayerData>("DataSets/Character/Astro").WaitForCompletion();
}