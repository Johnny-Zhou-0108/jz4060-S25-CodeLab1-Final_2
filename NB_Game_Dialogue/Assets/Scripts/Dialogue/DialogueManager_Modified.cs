using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManager_Modified : MonoBehaviour
{
    [Header("Spawn Prefabs")]
    public GameObject sushiPrefab;
    public GameObject swordPrefab;
    public Vector3 sushiSpawnPos;
    public Vector3 swordSpawnPos;

// 防止重复生成
    private bool hasSpawnedSushi = false;
    private bool hasSpawnedSword = false;
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Audio")]
    [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
    [SerializeField] private DialogueAudioInfoSO[] audioInfos;
    private DialogueAudioInfoSO currentAudioInfo;
    private Dictionary<string, DialogueAudioInfoSO> audioInfoDictionary;
    private AudioSource audioSource;


    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }

    private bool canContinueToNextLine = false;

    private Coroutine displayLineCoroutine;

    private static DialogueManager_Modified instance;

    private const string SPEAKER_TAG = "speaker";
    private const string AUDIO_TAG = "audio";

    private DialogueVariables dialogueVariables;

    private int currentChoiceIndex = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

        dialogueVariables = new DialogueVariables(loadGlobalsJSON);
        Debug.Log(dialogueVariables);

        audioSource = this.gameObject.AddComponent<AudioSource>();
        currentAudioInfo = defaultAudioInfo;
    }

    public static DialogueManager_Modified GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

        InitializeAudioInfoDictionary();
    }

    private void InitializeAudioInfoDictionary()
    {
        audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>();
        audioInfoDictionary.Add(defaultAudioInfo.id, defaultAudioInfo);
        foreach (DialogueAudioInfoSO audioInfo in audioInfos)
        {
            audioInfoDictionary.Add(audioInfo.id, audioInfo);
        }
    }

    private void SetCurrentAudioInfo(string id)
    {
        if (audioInfoDictionary.TryGetValue(id, out DialogueAudioInfoSO audioInfo))
        {
            currentAudioInfo = audioInfo;
        }
        else
        {
            Debug.LogWarning($"Failed to find audio info for id: {id}");
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if (currentStory.currentChoices.Count > 0)
        {
            HandleChoiceNavigation();
            if (InputManager.GetInstance().GetSubmitPressed())
            {
                MakeChoice(currentChoiceIndex);
            }
        }
        else if (canContinueToNextLine && InputManager.GetInstance().GetSubmitPressed())
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode_2(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory);

        displayNameText.text = "???";

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueVariables.StopListening(currentStory);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        SetCurrentAudioInfo(defaultAudioInfo.id);
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }

            string nextLine = currentStory.Continue();
            HandleTags(currentStory.currentTags);
            displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));

            // ✅ 检查变量并生成Prefab
            CheckAndSpawnPrefabs();
        }
        else
        {
            if (canContinueToNextLine)
            {
                StartCoroutine(ExitDialogueMode());
            }
            else
            {
                continueIcon.SetActive(true);
                canContinueToNextLine = true;
            }
        }
    }



    private IEnumerator DisplayLine(string line)
    {
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;

        foreach (char letter in line.ToCharArray())
        {
            if (InputManager.GetInstance().GetSubmitPressed())
            {
                dialogueText.maxVisibleCharacters = line.Length;
                break;
            }

            PlayDialogueSound(dialogueText.maxVisibleCharacters, letter);
            dialogueText.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);
        }

        continueIcon.SetActive(true);
        DisplayChoices();

        canContinueToNextLine = true;
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        AudioClip[] dialogueTypingSoundClips = currentAudioInfo.dialogueTypingSoundClips;
        int frequencyLevel = currentAudioInfo.frequencyLevel;
        float minPitch = currentAudioInfo.minPitch;
        float maxPitch = currentAudioInfo.maxPitch;
        bool stopAudioSource = currentAudioInfo.stopAudioSource;

        if (currentDisplayedCharacterCount % frequencyLevel == 0)
        {
            if (stopAudioSource)
            {
                audioSource.Stop();
            }
            AudioClip soundClip = dialogueTypingSoundClips[Random.Range(0, dialogueTypingSoundClips.Length)];
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(soundClip);
        }
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError($"Tag could not be parsed: {tag}");
                continue;
            }

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;
                case AUDIO_TAG:
                    SetCurrentAudioInfo(tagValue);
                    break;
                default:
                    Debug.LogWarning($"Unhandled tag: {tag}");
                    break;
            }
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError($"More choices given than UI can support: {currentChoices.Count}");
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        HighlightChoice(currentChoiceIndex);
    }

    private void HighlightChoice(int choiceIndex)
    {
        EventSystem.current.SetSelectedGameObject(choices[choiceIndex]);
    }

    private void HandleChoiceNavigation()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            NavigateChoices(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            NavigateChoices(1);
        }
    }

    private void NavigateChoices(int direction)
    {
        int choiceCount = currentStory.currentChoices.Count;
        currentChoiceIndex = (currentChoiceIndex + direction + choiceCount) % choiceCount;
        HighlightChoice(currentChoiceIndex);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }
    
    private void CheckAndSpawnPrefabs()
    {
        bool sushiVal = currentStory.variablesState["sushichange"] is bool b1 && b1;
        bool swordVal = currentStory.variablesState["swordchange"] is bool b2 && b2;

        Debug.Log($"[Ink DEBUG] sushichange = {sushiVal}, swordchange = {swordVal}");

        if (!hasSpawnedSushi && sushiVal)
        {
            Instantiate(sushiPrefab, sushiSpawnPos, Quaternion.identity);
            hasSpawnedSushi = true;
            Debug.Log("Spawned Sushi!");
        }

        if (!hasSpawnedSword && swordVal)
        {
            Instantiate(swordPrefab, swordSpawnPos, Quaternion.identity);
            hasSpawnedSword = true;
            Debug.Log("Spawned Sword!");
        }
    }


}
