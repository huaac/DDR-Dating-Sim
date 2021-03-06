using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceScreen : MonoBehaviour
{
    public static ChoiceScreen instance;

    public GameObject root;

    public ChoiceButton choicePrefab;

    static List<ChoiceButton> choices = new List<ChoiceButton>();

    public VerticalLayoutGroup LayoutGroup;

    void Awake()
    {
        instance = this;
    }

    public static void Show(params string[] choices)
    {
        instance.root.SetActive(true);
        if (isShowingChoices)
        {
            instance.StopCoroutine(showingChoices);
        }
        ClearAllCurrentChoices();
        showingChoices = instance.StartCoroutine(ShowingChoices(choices));
    }

    public static void Hide()
    {
        if (isShowingChoices)
        {
            instance.StopCoroutine(showingChoices);
        }
        showingChoices = null;
        ClearAllCurrentChoices();
        instance.root.SetActive(false);
    }

    static void ClearAllCurrentChoices()
    {
        foreach(ChoiceButton b in choices)
        {
            DestroyImmediate(b.gameObject);
        }
        choices.Clear();
    }

    public static bool isWaitingForChoiceToBeMade { get { return isShowingChoices && !lastChoiceMade.hasBeenMade; } }
    public static bool isShowingChoices { get { return showingChoices != null; } }
    static Coroutine showingChoices = null;
    public static IEnumerator ShowingChoices(string[] choices)
    {
        yield return new WaitForEndOfFrame();
        lastChoiceMade.Reset();

        for (int i = 0; i < choices.Length; i++)
        {
            CreateChoice(choices[i]);
        }

        while(isWaitingForChoiceToBeMade)
        {
            yield return new WaitForEndOfFrame();
        }
        Hide();
    }

    static void CreateChoice(string choice)
    {
        GameObject ob = Instantiate(instance.choicePrefab.gameObject, instance.choicePrefab.transform.parent);
        ob.SetActive(true);
        ChoiceButton b = ob.GetComponent<ChoiceButton>();

        b.text = choice;
        b.choiceIndex = choices.Count;

        choices.Add(b);
    }

    [System.Serializable]
    public class CHOICE
    {
        public bool hasBeenMade { get { return index != -1; } }

        public int index = -1;

        public void Reset()
        {
            index = -1;
        }
    }
    public CHOICE choice = new CHOICE();
    public static CHOICE lastChoiceMade { get { return instance.choice; } }

    public void MakeChoice(ChoiceButton button)
    {
        choice.index = button.choiceIndex;
    }
}
