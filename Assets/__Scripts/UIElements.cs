using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIElements : MonoBehaviour
{
    public static UIElements Instance;

    public Dictionary<SkillTypes, SkillBlock> skillBlocks = new Dictionary<SkillTypes, SkillBlock>();

    private void Awake()
    {
        Instance ??= this;

        var _playerUI = transform.Find("PlayerUI");
        foreach (Transform _block in _playerUI.Find("Blocks"))
        {
            SkillBlock _skillBlock = _block.GetComponent<SkillBlock>();
            skillBlocks.Add(_skillBlock.skillType, _skillBlock);
        }
    }
}

