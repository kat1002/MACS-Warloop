using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "SpaceShooter/WaveData")]
public class WaveData : ScriptableObject
{
    public enum EnemyType { Small, Mid, Large }

    [System.Serializable]
    public struct EnemyEntry
    {
        public Vector2   position;
        public EnemyType type;
    }

    public WaveManager.Difficulty difficulty;
    public int                   fullWidth;
    public EnemyEntry[]          entries;
}
