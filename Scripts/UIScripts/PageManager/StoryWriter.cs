using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryWriter : MonoBehaviour
{
    [Header("Color settings")]
    [SerializeField] private Color highlightedColor;
    private Color standartColor;

    [Header("Interface objects")]
    [SerializeField] private GameObject windowObject;
    [SerializeField] private GameObject textObject;
    [SerializeField] private GameObject titleTextObject;
    [SerializeField] private GameObject darkeningObject;

    [System.Serializable] public struct Paragraph {
        public string text;
        public string reaction;
        [HideInInspector] public bool IsOpened;
    }
    
    [System.Serializable] public struct Page {
        public Paragraph paragraph;
        public GameObject buttonObject;
        public int pageIndex;

        public Page(Paragraph paragraph, GameObject buttonObject, int pageIndex) {
            this.paragraph = paragraph;
            this.buttonObject = buttonObject;
            this.pageIndex = pageIndex;
        }
    }

    [Header("Paragraphs settings")]
    [SerializeField] private Paragraph[] randomParagraphs;
    [SerializeField] private Paragraph[] storyParagraphs;

    #region Page controller 
    private List<Page> openedPages = new List<Page>();
    private int currentPage;

    private int currentStoryParagraph = 0;
    private int totalPageNumber = 0;
    #endregion

    [Header("Button settings")]
    [SerializeField] private GameObject panelObject;
    [SerializeField] private GameObject buttonPrefab;
    private Paragraph lastAddedParagraph = new Paragraph();

    [Header("Other settings")]
    [SerializeField] private AudioClip openingClip;
    [SerializeField] private AudioClip closingClip;
    [SerializeField] private AudioClip chaningPageClip;

    public void Start() => standartColor = buttonPrefab.GetComponent<Image>().color;

    public void SetActiveMenu (bool mode) {
        windowObject.SetActive(mode);
        darkeningObject.SetActive(mode);

        if (mode && openingClip != null)
            AudioManager.PlaySound(openingClip);
        else if (!mode && openingClip != null)
            AudioManager.PlaySound(closingClip);

        if (!mode) {
            if (lastAddedParagraph.reaction != "" && lastAddedParagraph.reaction != null)
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CharacterDialogue>().AddLine(lastAddedParagraph.reaction);
        }

        lastAddedParagraph = new Paragraph();
    }

    public void AddParagraph(Paragraph paragraph) {
        GameObject createdButtonObject = Instantiate(buttonPrefab, panelObject.transform);
        Page createdPage = new Page(paragraph, createdButtonObject, totalPageNumber);
        createdButtonObject.GetComponent<Button>().onClick.AddListener(() => OpenParagraph(createdPage.pageIndex));
        createdButtonObject.GetComponent<Button>().onClick.AddListener(() => AudioManager.PlaySound(chaningPageClip));
        createdButtonObject.GetComponentInChildren<Text>().text = $"Page {totalPageNumber + 1}";
        openedPages.Add(createdPage);
        OpenParagraph(totalPageNumber);
        SetActiveMenu(true);
        lastAddedParagraph = paragraph;
        totalPageNumber++;
    }

    private bool OpenStoryParagraph() {
        if (currentStoryParagraph >= storyParagraphs.Length) {
            Debug.Log("All story paragraphs are unlocked");
            return false;
        }

        AddParagraph(storyParagraphs[currentStoryParagraph++]);
        return true;
    }

    private bool IsOpenedParagraph(Paragraph paragraph) {
        for (int i = 0; i < openedPages.Count; ++i) {
            if (openedPages[i].paragraph.text == paragraph.text)
                return true;
        }

        return false;
    }

    private bool OpenRandomP1aragraph() {
        int indexToChoose = Random.Range(0, randomParagraphs.Length);
        bool allOpened = true;

        for (int x = 0; x < randomParagraphs.Length; ++x) {
            if (!IsOpenedParagraph(randomParagraphs[x]))
                allOpened = false;
        }

        if (allOpened && openedPages.Count != 0) {
            Debug.Log("All random paragraphs are unlocked");
            return false;
        }

        while (IsOpenedParagraph(randomParagraphs[indexToChoose])) 
            indexToChoose = Random.Range(0, randomParagraphs.Length);

        AddParagraph(randomParagraphs[indexToChoose]);
        return true;
    }

    public void OpenNewParagraph () {
        int mode = Random.Range(0, 2);
        if (mode == 0) {
            if (!OpenRandomP1aragraph())
                OpenStoryParagraph();
        } else if (!OpenStoryParagraph())
            OpenRandomP1aragraph();
    }

    public void OpenParagraph (int id) {
        currentPage = id;
        UpdateText();
    }

    public void OpenBookAndPage(int id) {
        SetActiveMenu(true);
        OpenParagraph(id);
    }

    public void UpdateText() {
        titleTextObject.GetComponent<Text>().text = "Page " + (currentPage + 1);
        textObject.GetComponent<Text>().text = openedPages[currentPage].paragraph.text;
    }

    private void Update() {
        for (int i = 0; i < openedPages.Count; ++i)
            openedPages[i].buttonObject.GetComponent<Image>().color = (i == currentPage) ? highlightedColor : standartColor;
    }
}
