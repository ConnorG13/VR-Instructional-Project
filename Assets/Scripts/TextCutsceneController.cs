using System;
using TMPro;
using UnityEngine;

public class TextCutsceneController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textComponent;

    [Header("Typing Settings")]
    public TypewriterSettings settings = new TypewriterSettings();
    private float originDelayBetweenChars;
    private bool lastCharPunctuation = false;
	private char charNewLine;
    private char charComma;
    private char charPeriod;
	
	[HideInInspector]
    public string story;

    [Header("Audio Settings")]
    [Tooltip("When true requires AudioSource on this object.")]
    public bool useAudio = true;
    [Range(0f, 2f)]
    public float volume = .3f;
    [Tooltip("GameObject with AudioSource component.")]
    public GameObject AudioTyping;
    private AudioSource TypingFX;

    [Header("Extra Settings")]
    [SerializeField] bool _clearOnStart = true;
    [SerializeField] bool _clearOnFinish = true;

    Coroutine _typingCoroutine;

    public UnityEngine.Events.UnityEvent OnTypingFinished;

    void Awake()
    {
        if (useAudio)
        {
            TypingFX = GetComponent<AudioSource>();
            TypingFX.clip = AudioTyping.GetComponent<AudioSource>().clip;
        }

        if (_textComponent == null) _textComponent = GetComponent<TextMeshProUGUI>();
        originDelayBetweenChars = settings.delayBetweenChars;

		charNewLine = Convert.ToChar(10);
        charComma = Convert.ToChar(44);
        charPeriod = Convert.ToChar(46);
    }

    void Start()
    {
        if (_clearOnStart && _textComponent) _textComponent.text = "";

        //StartTyping(story);
    }

    public void StartTyping(string text)
    {
		if (_clearOnStart && _textComponent) _textComponent.text = "";
		story = text;
        _typingCoroutine = StartCoroutine(TypeText());
    }

    public void StopTyping()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }
        
        if (useAudio) TypingFX.Stop();
        _textComponent.text = story;

        OnTypingFinished.Invoke();
    }

    private System.Collections.IEnumerator TypeText()
    {
        yield return new WaitForSeconds(settings.delayToStart);
        foreach (char c in story)
        {
            settings.delayBetweenChars = originDelayBetweenChars;

            if (lastCharPunctuation)  //If previous character was a comma/period, pause typing
            {
                if (useAudio) TypingFX.Pause();
                yield return new WaitForSeconds(settings.delayBetweenChars = settings.delayAfterPunctuation);
                lastCharPunctuation = false;
            }

            if (c == charComma || c == charPeriod || c == charNewLine)
            {
                if (useAudio) TypingFX.Pause();
                lastCharPunctuation = true;
            }

            if (useAudio) TypingFX.PlayOneShot(TypingFX.clip, volume);
            _textComponent.text += c;
            yield return new WaitForSeconds(settings.delayBetweenChars);
        }

        if (useAudio) TypingFX.Stop();

        yield return new WaitForSeconds(settings.delayAfterTyping);
        if (_clearOnFinish) _textComponent.text = "";

        OnTypingFinished.Invoke();
		yield break;
    }

}

[Serializable]
public class TypewriterSettings
{
    public float delayToStart;
    public float delayBetweenChars;
    public float delayAfterPunctuation;
    public float delayAfterTyping;

    public TypewriterSettings()
    {
        delayToStart = 0.5f;
        delayBetweenChars = 0.05f;
        delayAfterPunctuation = 0.1f;
        delayAfterTyping = 1f;
    }
}

