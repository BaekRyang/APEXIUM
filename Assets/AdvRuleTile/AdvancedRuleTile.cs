using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Advanced Rule Tile")]
public class AdvancedRuleTile : RuleTile<AdvancedRuleTile.Neighbor> {
    [Header("Advanced Tile")]
    [Tooltip("활성화시 이 타일은 \"This\" 모드일때 이 타일도 연결됩니다.")]
    public bool alwaysConnect;
    [Tooltip("모드가 \"any\"일때 자신을 체크합니다.")]
    public bool checkSelfInAny = true;
    [Space]
    [Tooltip("모드가 \"Specified\"일때 연결될 타일들")]
    public TileBase[] tilesToConnect;
    
    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Any = 3;
        public const int Specified = 4;
        public const int Nothing = 5;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.This: return Check_This(tile);
            case Neighbor.NotThis: return Check_NotThis(tile);
            case Neighbor.Any: return Check_Any(tile);
            case Neighbor.Specified: return Check_Specified(tile);
            case Neighbor.Nothing: return Check_Nothing(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }
    
    bool Check_This(TileBase tile)
    {
        if (!alwaysConnect) return tile == this;
        else return tilesToConnect.Contains(tile) || tile == this;
    }
    
    bool Check_NotThis(TileBase tile)
    {
        if (!alwaysConnect) return tile != this;
        else return !tilesToConnect.Contains(tile) && tile != this;
    }
    
    bool Check_Any(TileBase tile)
    {
        if (checkSelfInAny) return tile != null;
        else return tile != null && tile != this;
    }
    
    bool Check_Specified(TileBase tile)
    {
        return tilesToConnect.Contains(tile);
    }
    bool Check_Nothing(TileBase tile)
    {
        return tile == null;
    }
}