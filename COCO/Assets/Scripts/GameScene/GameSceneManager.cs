using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

// GameSceneManager is the main handler of the game scene
// Inherits from MonoBehaviour and EventListener (for receiving data from server)
public class GameSceneManager : MonoBehaviour, EventListener
{

    Vector3 startPos;
    Vector3 endPos;
    Camera camera;
    Ray ray;
    RaycastHit hit;
    Vector3 cameraOffset = new Vector3(0, 0, 10);
    GameModel gameModel;
    List<string> consumedCharacters;
    List<char> boardCharacters;
    List<string> answersList;
    List<string> answeredList;
    List<GameObject> characterGameObj;
    List<GameObject> linesGameObj;
    List<List<GameObject>> fieldsGameObj;
    int selected_level;

    [SerializeField] Texture2D tex;
    [SerializeField] TMP_FontAsset font;
    [SerializeField] AnimationCurve ac;
    [SerializeField] Material lineMaterial;
    [SerializeField] AudioSource CorrectGuessSound;
    [SerializeField] GameObject notif;
    [SerializeField] TextMeshProUGUI coinCounter;

    // When the Start function is called
    void Start()
    {   
        // Finds out which level player has selected for extracting the words corresponding to the level
        selected_level = PlayerPrefs.GetInt("selected_level");

        // Game Model is stored in a .json file for 
        string json = File.ReadAllText(Application.dataPath + "/gameModel.json");
        gameModel = JsonUtility.FromJson<GameModel>(json);
        gameModel.InstantiateGameConfig(selected_level);
        camera = Camera.main;
        consumedCharacters = new List<string>();
        characterGameObj = new List<GameObject>();
        linesGameObj = new List<GameObject>();
        fieldsGameObj = new List<List<GameObject>>();
        answeredList = new List<string>();

        boardCharacters = gameModel.GetBoardCharacters();
        answersList = gameModel.GetAnswersList();
        SpawnWordsAroundCircle(boardCharacters);
        CreateAnswerFields(answersList);
        ServiceLocator.Instance.eventManager.Register(this);

        // load last game guessed words
        int word_index;
        for (int i = 0; i < gameModel.GetAnsweredList().Count; i++)
        {
            word_index = answersList.IndexOf(gameModel.answeredList[i]);
            for (int j = 0; j < answersList[word_index].Length; j++)
            {
                fieldsGameObj[word_index][j].transform.GetChild(0).gameObject.SetActive(true);
                fieldsGameObj[word_index][j].GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 230);
            }

            answeredList.Add(answersList[word_index]);
        }
        coinCounter.text = gameModel.GetCoin().ToString();
    }

    void Update()
    {
        // check if mouse button is down and its position collided
        // with a character begin the first point of the line and initialize the LineRenderer
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                GameObject nullGameObj = new GameObject((linesGameObj.Count + 1).ToString());
                nullGameObj.transform.SetParent(gameObject.transform);
                linesGameObj.Add(nullGameObj);
                startPos = hit.collider.gameObject.transform.position;
                startPos.z -= 1;
                linesGameObj[linesGameObj.Count - 1].AddComponent<LineRenderer>();
                linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().SetPosition(0, startPos);
                linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().positionCount = 2;
                linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().useWorldSpace = true;
                linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().widthCurve = ac;
                linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().numCapVertices = 10;
                linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().numCornerVertices = 10;
                linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().material = lineMaterial;

                consumedCharacters.Add(hit.collider.name);
            }
        }

        // check if mouse is still down and collides another character
        // set the next point of the line otherwise continue drawing the line
        if (Input.GetMouseButton(0))
        {
            if (linesGameObj.Count > 0)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (!consumedCharacters.Contains(hit.collider.name) && consumedCharacters.Count != boardCharacters.Count)
                    {
                        endPos = hit.collider.gameObject.transform.position;
                        endPos.z -= 1;
                        linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().SetPosition(1, endPos);

                        consumedCharacters.Add(hit.collider.name);
                        if (consumedCharacters.Count != boardCharacters.Count)
                        {
                            GameObject nullGameObj = new GameObject((linesGameObj.Count + 1).ToString());
                            nullGameObj.transform.SetParent(gameObject.transform);
                            linesGameObj.Add(nullGameObj);
                            linesGameObj[linesGameObj.Count - 1].AddComponent<LineRenderer>();
                            linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().SetPosition(0, endPos);
                            linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().positionCount = 2;
                            linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().useWorldSpace = true;
                            linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().widthCurve = ac;
                            linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().numCapVertices = 10;
                            linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().numCornerVertices = 10;
                            linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().material = lineMaterial;
                        }
                    }
                }
                if (consumedCharacters.Count != boardCharacters.Count)
                {
                    endPos = camera.ScreenToWorldPoint(Input.mousePosition) + cameraOffset;
                    linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().SetPosition(1, endPos);
                }
            }
        }

        // in case of ending the mouse click or touch screen hide the line
        // and calculate word that is shaped by user
        if (Input.GetMouseButtonUp(0) && linesGameObj.Count > 0)
        {
            string guessedWord = "";
            for (int i = 0; i < consumedCharacters.Count; i++)
            {
                guessedWord += consumedCharacters[i];
            }

            // if word exists show the word in word fields
            int result = gameModel.CheckWordCorrectness(guessedWord);
            if (result >= 0 && !answeredList.Contains(answersList[result]))
            {
                for (int i = 0; i < fieldsGameObj[result].Count; i++)
                {
                    fieldsGameObj[result][i].transform.GetChild(0).gameObject.SetActive(true);
                    fieldsGameObj[result][i].GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 230);

                }
                CorrectGuessSound.Play();
                answeredList.Add(answersList[result]);

                if (answeredList.Count == answersList.Count)
                {
                    UserWon();
                }
            }

            // destrpy lines and clear our word queues
            for (int i = 0; i < linesGameObj.Count; i++)
            {
                Destroy(linesGameObj[i]);
            }
            linesGameObj.Clear();
            consumedCharacters.Clear();
        }
    }

    private void OnGestureInput(int handNumber)
    {
        // in case of using hand gestures create associated lines
        if (handNumber == 0)
        {
            if (consumedCharacters.Count != 0)
            {
                string guessedWord = "";
                for (int i = 0; i < consumedCharacters.Count; i++)
                {
                    guessedWord += consumedCharacters[i];
                }

                // if word exists show the word in word fields
                int result = gameModel.CheckWordCorrectness(guessedWord);
                if (result >= 0 && !answeredList.Contains(answersList[result]))
                {
                    for (int i = fieldsGameObj[result].Count - 1; i >= 0; i--)
                    {
                        fieldsGameObj[result][i].transform.GetChild(0).gameObject.SetActive(true);
                        fieldsGameObj[result][i].GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 230);

                    }
                    CorrectGuessSound.Play();
                    answeredList.Add(answersList[result]);

                    if (answeredList.Count == answersList.Count)
                    {
                        UserWon();
                    }
                }

                // destrpy lines and clear our word queues
                for (int i = 0; i < linesGameObj.Count; i++)
                {
                    Destroy(linesGameObj[i]);
                }
                linesGameObj.Clear();
                consumedCharacters.Clear();
            }
        }
        else
        {
            if (!consumedCharacters.Contains(characterGameObj[handNumber - 1].name) && handNumber <= boardCharacters.Count)
            {
                if (consumedCharacters.Count != 0)
                {
                    endPos = characterGameObj[handNumber - 1].transform.position;
                    endPos.z -= 1;
                    linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().SetPosition(1, endPos);
                }

                if (consumedCharacters.Count != boardCharacters.Count)
                {
                    GameObject nullGameObj = new GameObject((linesGameObj.Count + 1).ToString());
                    nullGameObj.transform.SetParent(gameObject.transform);
                    linesGameObj.Add(nullGameObj);
                    startPos = characterGameObj[handNumber - 1].transform.position;
                    startPos.z -= 1;
                    linesGameObj[linesGameObj.Count - 1].AddComponent<LineRenderer>();
                    linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().SetPosition(0, startPos);
                    linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().positionCount = 2;
                    linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().useWorldSpace = true;
                    linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().widthCurve = ac;
                    linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().numCapVertices = 10;
                    linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().numCornerVertices = 10;
                    linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().material = lineMaterial;
                    linesGameObj[linesGameObj.Count - 1].GetComponent<LineRenderer>().SetPosition(1, startPos);

                    consumedCharacters.Add(characterGameObj[handNumber - 1].name);
                }
            }
        }
    }

    private void OnDisable()
    {
        // save logic manager infos at the end to our model
        if (selected_level == gameModel.GetLevel())
        {
            string json = JsonUtility.ToJson(gameModel);
            File.WriteAllText(Application.dataPath + "/gameModel.json", json);
        }
    }

    void SpawnWordsAroundCircle(List<char> chars)
    {
        float angle = 360.0f / chars.Count;
        float characterContainerRadius = gameObject.GetComponent<SpriteRenderer>().bounds.extents.x;
        Vector3 characterContainerPos = gameObject.GetComponent<SpriteRenderer>().bounds.center;

        for (int i = 0; i < chars.Count; i++)
        {
            Vector3 pos;
            pos.x = characterContainerPos.x + characterContainerRadius * 0.7f * Mathf.Sin(angle * i * Mathf.Deg2Rad);
            pos.y = characterContainerPos.y + characterContainerRadius * 0.7f * Mathf.Cos(angle * i * Mathf.Deg2Rad);
            pos.z = characterContainerPos.z - 1;

            characterGameObj.Add(CreateCharacter(chars[i].ToString(), i + 1, pos));

        }
    }

    GameObject CreateCharacter(string displayText, int i, Vector3 pos)
    {
        GameObject myGO;
        GameObject myText;
        Canvas myCanvas;
        TextMeshProUGUI text;
        ContentSizeFitter myTextContentSizeFitter;
        BoxCollider boxCollider;

        // create canvas
        myGO = new GameObject();
        myGO.name = i.ToString() + "Canvas";
        myGO.transform.SetParent(gameObject.transform);
        myGO.AddComponent<Canvas>();

        myCanvas = myGO.GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = camera;
        myGO.AddComponent<CanvasScaler>();
        myGO.AddComponent<GraphicRaycaster>();

        // create text and set its name
        myText = new GameObject();
        myText.transform.parent = myGO.transform;
        myText.name = displayText;

        text = myText.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.color = new Color32(255, 213, 0, 255);
        text.text = displayText;
        text.fontStyle = FontStyles.Bold;
        text.enableAutoSizing = true;

        // align text container to fit the text
        myTextContentSizeFitter = myText.AddComponent<ContentSizeFitter>();
        myTextContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        myTextContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // set text position
        myText.transform.position = pos;
        myText.transform.localScale = new Vector3(1f, 1f, 1f);

        // add box collider to check if user touches the character
        boxCollider = myText.AddComponent<BoxCollider>();
        boxCollider.center = pos;
        boxCollider.size = new Vector3(text.preferredWidth, text.preferredHeight, 1);

        return myText;
    }

    void CreateAnswerFields(List<string> answers)
    {
        GameObject myGO;
        Canvas myCanvas;
        GameObject mySpriteGO;
        Sprite mySprite;
        SpriteRenderer mySpriteRenderer;
        GameObject myText;
        TextMeshProUGUI text;
        ContentSizeFitter myTextContentSizeFitter;

        // create canvas
        myGO = new GameObject("Answer Field Canvas");
        myGO.AddComponent<Canvas>();

        myCanvas = myGO.GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.worldCamera = camera;
        myGO.AddComponent<CanvasScaler>();
        myGO.AddComponent<GraphicRaycaster>();
        myGO.transform.position = new Vector3(0, 0, 0);


        // create rows of answer fields
        float UPPER = 280f;
        float LOWER = -20f;
        float rowHeight = (UPPER - LOWER) / answers.Count;
        LOWER = LOWER + rowHeight / 2f;
        for (int i = 0; i < answers.Count; i++)
        {
            List<GameObject> rowFieldsGameObj = new List<GameObject>();
            // create each field (word column) for the row
            float padRight, spriteWidth;
            for (int j = 0; j < answers[i].Length; j++)
            {
                mySpriteGO = new GameObject("sprite");
                mySpriteGO.transform.SetParent(myGO.transform);
                mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                mySpriteRenderer = mySpriteGO.AddComponent<SpriteRenderer>();
                mySpriteRenderer.sprite = mySprite;
                mySpriteRenderer.color = new Color32(255, 255, 255, 200);
                mySpriteGO.transform.localScale = new Vector3(60f, 60f, 1f);

                spriteWidth = tex.width * mySpriteRenderer.bounds.size.x / mySpriteRenderer.sprite.bounds.size.x / 1.4f;
                padRight = spriteWidth * (answers[i].Length - 1) / 2;
                mySpriteGO.transform.localPosition = new Vector3(padRight - j * spriteWidth, i * rowHeight + LOWER, 0);

                // create text and set its name
                myText = new GameObject(i.ToString() + " " + j.ToString());
                myText.transform.parent = mySpriteGO.transform;

                text = myText.AddComponent<TextMeshProUGUI>();
                text.font = font;
                text.color = new Color32(255, 213, 0, 255);
                text.text = answers[i][j].ToString();
                text.fontStyle = FontStyles.Bold;
                text.fontSize = 1;

                // align text container to fit the text
                myTextContentSizeFitter = myText.AddComponent<ContentSizeFitter>();
                myTextContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                myTextContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                // set text position
                myText.transform.position = mySpriteGO.transform.position;
                myText.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                //disable text until user find the answer
                myText.SetActive(false);

                // add to our fields so later we can access it
                rowFieldsGameObj.Add(mySpriteGO);
            }
            fieldsGameObj.Add(rowFieldsGameObj);
        }

    }

    void UserWon()
    {
        // this function will call when user win the current level
        // and will switch to win menu
        Debug.Log("YOU WON!!!");
        if (selected_level == gameModel.GetLevel())
        {
            gameModel.IncrementCurrentLevel();
            string json = JsonUtility.ToJson(gameModel);
            File.WriteAllText(Application.dataPath + "/gameModel.json", json);
        }
        PlayerPrefs.SetInt("win_or_lose", 1);
        SceneManager.LoadScene("MenuScene");
        PlayerPrefs.Save();
    }

    public void GetHelp()
    {
        // ask logic manager to help us and if logic manager returns a non negative value
        // it means logic manager helps us and program will show the answer of index's answer
        int result = gameModel.GetHelp();
        if (result >= 0)
        {
            for (int i = 0; i < fieldsGameObj[result].Count; i++)
            {
                fieldsGameObj[result][i].transform.GetChild(0).gameObject.SetActive(true);
                fieldsGameObj[result][i].GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 230);
            }

            coinCounter.text = gameModel.GetCoin().ToString();

            // play the correct word guess sound
            CorrectGuessSound.Play();
            answeredList.Add(answersList[result]);

            // check if user won
            if (answeredList.Count == answersList.Count)
            {
                UserWon();
            }
        }
        else
        {
            StartCoroutine(showNotif());
        }
    }

    IEnumerator showNotif()
    {
        notif.SetActive(true);

        //Wait for 3 seconds
        yield return new WaitForSecondsRealtime(3);

        notif.SetActive(false);
    }

    public void returnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void OnEvent(GameEvent gameEvent)
    {
        if (gameEvent is ZeroDetected)
        {
            OnGestureInput(0);
        }
        else if (gameEvent is OneDetected)
        {
            OnGestureInput(1);
        }
        else if (gameEvent is TwoDetected)
        {
            OnGestureInput(2);
        }
        else if (gameEvent is ThreeDetected)
        {
            OnGestureInput(3);
        }
        else if (gameEvent is FourDetected)
        {
            OnGestureInput(4);
        }
        else if (gameEvent is FiveDetected)
        {
            OnGestureInput(5);
        }
    }
}
