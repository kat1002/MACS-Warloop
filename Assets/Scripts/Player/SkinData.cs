using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "SpaceShooter/SkinData")]
public class SkinData : ScriptableObject
{
    public string skinName;
    public Sprite level0;
    public Sprite level1;
    public Sprite level2;

    public Sprite PreviewSprite => level0;

    public Sprite GetSprite(int level)
    {
        switch (level)
        {
            case 2:  return level2 != null ? level2 : (level1 != null ? level1 : level0);
            case 1:  return level1 != null ? level1 : level0;
            default: return level0;
        }
    }
}
