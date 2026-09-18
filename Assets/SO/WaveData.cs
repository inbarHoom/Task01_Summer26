using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    [SerializeField] private int weakCount;
    [SerializeField] private int strongCount;
    [SerializeField] private float interval;

    public int WeakCount => weakCount;
    public int StrongCount => strongCount;
    public float Interval => interval;
}
