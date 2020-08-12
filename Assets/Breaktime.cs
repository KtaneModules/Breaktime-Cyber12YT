using System;
using UnityEngine;

public class Breaktime : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombModule Module;
    public KMSelectable[] Spheres;
    public TextMesh Hours;

    private bool _isSolved;
    private static int _moduleIdCounter = 1;
    private int _moduleId = 0, _solution;
    private float _break;

    private void Awake()
    {
        _moduleId = _moduleIdCounter++;

        for (int i = 0; i < Spheres.Length; i++)
        {
            int j = i;
            Spheres[i].OnInteract += delegate ()
            {
                PressedButton(j);
                return false;
            };
        }
    }

    private void Start()
    {
        // Module generation.
        _moduleId = _moduleIdCounter++;
        Hours.text = UnityEngine.Random.Range(1, 11).ToString();
        _solution = 7 - Math.Max(int.Parse(Hours.text) - 2, 0);

        // The amount of breaks used are linear, except for when there are 3 hours, which it uses 0 instead of 0.5.
        _break = Math.Max(int.Parse(Hours.text) / 2 - 1, 0) == 0.5f ? 0 : Math.Max(int.Parse(Hours.text) / 2 - 1, 0);
        Debug.LogFormat("[Breaktime #{0}]: The hour display shows {1}, meaning the button to push is {2} breaks.", _moduleId, Hours.text, _break);
    }

    private void PressedButton(int sphere)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Spheres[sphere].transform);
        Spheres[sphere].AddInteractionPunch();

        if (_isSolved)
            return;

        if (_solution == int.Parse(Hours.text))
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, Spheres[sphere].transform);
            Debug.LogFormat("[Breaktime #{0}]: The correct button was pushed, module solved!", _moduleId);
            _isSolved = true;
            Module.HandlePass();
        }

        else
        {
            Debug.LogFormat("[Breaktime #{0}]: The button \"{1} breaks\" was pushed which was incorrect, module strike!", _moduleId, );
            Module.HandleStrike();
        }
    }
}









