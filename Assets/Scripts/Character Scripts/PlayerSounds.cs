using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] AudioClip[] footstepSounds = default;
    void FootStep()
    {
        AudioManager.instance.source.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
    }   
    
}
