using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("“Ù¿÷ ˝æ›ø‚")]
    public SoundDetailsList_SO soundDetailsData;
    public SceneSoundList_SO sceneSoundData;
    [Header("Audio Source")]
    public AudioSource ambientSource;
    public AudioSource gameSource;

    private Coroutine soundRoutine;
    public float MusicStartSecond => Random.Range(5f, 15f);
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;

    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneSoundItem sceneSound = sceneSoundData.GetSceneSoundItem(currentScene);
        if (sceneSound == null) return;

        SoundDetails ambient = soundDetailsData.GetSoundDetails(sceneSound.ambient);
        SoundDetails music = soundDetailsData.GetSoundDetails(sceneSound.music);
        if (soundRoutine != null) StopCoroutine(soundRoutine);
        else
        {
            soundRoutine = StartCoroutine(PlaySoundRoutine(music, ambient));
        }
    }
    private IEnumerator PlaySoundRoutine(SoundDetails music,SoundDetails ambient)
    {
        if(music != null && ambient != null)
        {
            PlayAmbientClip(ambient);
            yield return new WaitForSeconds(MusicStartSecond);
            PlayMusicClip(music);
        }
    }
    /// <summary>
    /// ≤•∑≈±≥æ∞“Ù¿÷
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayMusicClip(SoundDetails soundDetails)
    {
        gameSource.clip = soundDetails.soundClip;
        if (gameSource.isActiveAndEnabled) gameSource.Play();

    }
    /// <summary>
    /// ≤•∑≈ª∑æ≥“Ù¿÷
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayAmbientClip(SoundDetails soundDetails)
    {
        ambientSource.clip = soundDetails.soundClip;
        if (ambientSource.isActiveAndEnabled) ambientSource.Play();

    }
}
