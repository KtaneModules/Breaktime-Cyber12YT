using System;
using System.Collections.Generic;
using UnityEngine;

public class Breaktime : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombModule Module;
    public KMSelectable[] Spheres;
    public TextMesh Hours;

    private bool _isSolved;
    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;
    private float _break;

    private static readonly Dictionary<string, int> hoursToButton = new Dictionary<string, int>(10)
    {
        { "1", 0 },
        { "2", 0 },
        { "3", 0 },
        { "4", 1 },
        { "5", 2 },
        { "6", 3 },
        { "7", 4 },
        { "8", 5 },
        { "9", 6 },
        { "10", 7 }
    };

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
        Hours.text = UnityEngine.Random.Range(1, 11).ToString();

        // The amount of breaks used is linear, except for when there are 3 hours, which it uses 0 instead of 0.5.
        _break = Math.Max((int.Parse(Hours.text) / 2) - 1, 0) == 0.5f ? 0 : Math.Max((int.Parse(Hours.text) / 2) - 1, 0);
        Debug.LogFormat("[Breaktime #{0}]: The hour display shows {1}, meaning the button to push is {2} break(s).", _moduleId, Hours.text, _break);
    }

    private void PressedButton(int sphere)
    {
        Audio.PlaySoundAtTransform("buttonPress", Module.transform);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Spheres[sphere].transform);
        Spheres[sphere].AddInteractionPunch();

        if (_isSolved)
            return;

        int solution;
        hoursToButton.TryGetValue(Hours.text, out solution);

        // If the button pushed matches the solution, solve the module.
        if (sphere == solution)
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, Spheres[sphere].transform);

            Debug.LogFormat("[Breaktime #{0}]: The correct button was pushed, module solved!", _moduleId);
            _isSolved = true;
            Module.HandlePass();
        }

        else
        {
            // Calculate the amount of hours corresponding to the button that was pressed, then log and strike.
            float floatSphere = sphere + 1, buttonPressed = Math.Max(floatSphere / 2, 0) == 0.5 ? 0 : Math.Max(floatSphere / 2, 0);
            Debug.LogFormat("[Breaktime #{0}]: The button \"{1} breaks\" was pushed which was incorrect, module strike!", _moduleId, buttonPressed);
            Module.HandleStrike();
        }
    }
}
