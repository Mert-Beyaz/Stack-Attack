

/* 𝕄𝔼ℝ𝕋 𝔹𝔼𝕐𝔸ℤ */


using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Data/Sound Data")]
public class SoundData : ScriptableObject
{
    [SerializeField] SoundClip[] soundClips;

    public SoundClip[] SoundClips => soundClips;
}

