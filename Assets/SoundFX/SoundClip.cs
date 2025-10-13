

/* 𝕄𝔼ℝ𝕋 𝔹𝔼𝕐𝔸ℤ */


using UnityEngine;


[System.Serializable]
public class SoundClip
{
    [SerializeField] string _name;
    [SerializeField] AudioClip _clip;
    [SerializeField] [Range(0, 1)] float _volume;

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public AudioClip Clip
    {
        get => _clip;
        set => _clip = value;
    }

    public float Volume
    {
        get => _volume;
        set => _volume = Mathf.Clamp(value, 0, 1);
    }
}
