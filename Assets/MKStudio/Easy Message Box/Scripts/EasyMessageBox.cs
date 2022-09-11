using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using MKStudio.UI.MessageBox;

[RequireComponent(typeof(Canvas))]
public class EasyMessageBox : MonoBehaviour
{
    public const string VersionString = "v1.4";

    public static Queue<MessageBoxParams> BoxQueue;
    public static EasyMessageBox Instance;
    public List<BoxController> Templates;
    List<List<BoxController>> boxPool;
    int maxCanvasOrderInTheScene = int.MinValue;
    bool hasQueuedBox;

    static GlobalSettings settings;

    Canvas currentCanvas;

    //List<BoxController> activeBoxes;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (settings == null)
        {
            settings = Resources.Load<GlobalSettings>("Global Settings");
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<GlobalSettings>();
                Debug.LogWarning("Easy Message Box cannot load global settings. Using default settings.");
            }
        }
        if (settings.DefaultPrefab == null)
        {
            var loaded = Resources.Load<GameObject>("Default Message Box");
            if (loaded == null)
            {
                Debug.LogError("Cannot find the Default Message Box prefab.");
            }
            else
            {
                settings.DefaultPrefab = loaded;
            }
        }

        currentCanvas = gameObject.GetComponent<Canvas>();

        BoxQueue = new Queue<MessageBoxParams>();
        boxPool = new List<List<BoxController>>();
        //activeBoxes = new List<BoxController>();

        if (Templates == null)
        {
            Templates = new List<BoxController>();
        }
        else
        {
            Templates.RemoveAll(t => t == null);
        }

        for (int i = 0; i < Templates.Count; i++)
        {
            boxPool.Add(new List<BoxController>());
            Templates[i].gameObject.SetActive(false);
            boxPool[i].Add(Templates[i]);
        }

        if (FindObjectOfType<EventSystem>() == null)
        {
            var evt = new GameObject("EventSystem");
            evt.AddComponent<EventSystem>();
            evt.AddComponent<StandaloneInputModule>();
        }
        var canvasesInTheScene = FindObjectsOfType<Canvas>();

        for (int i = 0; i < canvasesInTheScene.Length; i++)
        {
            if (canvasesInTheScene[i].sortingOrder > maxCanvasOrderInTheScene)
            {
                maxCanvasOrderInTheScene = canvasesInTheScene[i].sortingOrder;
            }
        }

        currentCanvas.sortingOrder = maxCanvasOrderInTheScene + 1;
    }

    public void OnMessageBoxClosed(BoxController closedBox)
    {
        bool canDestroy = true;
        if (closedBox.currentParam.MultipleCallBehaviour == MultipleCallBehaviours.Queue)
        {
            hasQueuedBox = false;
            if (BoxQueue.Count > 0)
            {
                ActualShowDialogBox(BoxQueue.Dequeue());
                canDestroy = false;
            }
        }
        if (canDestroy)
        {
            if (closedBox != Templates[closedBox.currentParam.TemplateId])
            {
                boxPool[closedBox.currentParam.TemplateId].Remove(closedBox);
                Destroy(closedBox.gameObject);
            }
        }
    }

    void ActualShowDialogBox(MessageBoxParams param)
    {
        bool hasAvailableBox = false;
        for (int i = 0; i < boxPool[param.TemplateId].Count; i++)
        {
            if (!boxPool[param.TemplateId][i].gameObject.activeSelf)
            {
                hasAvailableBox = true;
                (boxPool[param.TemplateId][i].transform as RectTransform).anchoredPosition = Vector2.zero;
                (boxPool[param.TemplateId][i].transform as RectTransform).sizeDelta = Vector2.zero;
                boxPool[param.TemplateId][i].gameObject.SetActive(true);
                boxPool[param.TemplateId][i].gameObject.GetComponent<BoxController>().Initiate(param);
                //activeBoxes.Add(boxPool[param.TemplateId][i].gameObject.GetComponent<BoxController>());
                break;
            }
        }
        if (!hasAvailableBox)
        {
            //Instantiate new box based on template.
            var box = Instantiate(Templates[param.TemplateId]);
            box.transform.SetParent(transform);
            (box.transform as RectTransform).anchoredPosition = Vector2.zero;
            (box.transform as RectTransform).sizeDelta = Vector2.zero;
            var controller = box.GetComponent<BoxController>();
            boxPool[param.TemplateId].Add(controller);
            //activeBoxes.Add(controller);
            controller.Background.color = Color.clear;
            controller.Initiate(param);
        }

        if (param.MultipleCallBehaviour == MultipleCallBehaviours.Queue)
        {
            hasQueuedBox = true;
        }
    }

    /// <summary>
    /// Show the message box. The appearance and behaviour of the message box is defined by setting 
    /// a series of parameters. All parameters are optional except the 'message' string. The optional parameters
    ///  can also be set in any order using named arguments. For example: EasyMessageBox.Show("Level complete!", layout: ContentLayouts.Vertical, messageBoxTitle:"Congratulations!");
    /// </summary>
    /// <param name="message">The message text to show in the message box. The UI element itself will hide if set to null or an empty string.</param>
    /// <param name="messageIcon">The icon for the message. The UI element itself will hide if set to null. It will be to the left of the message in horizontal layout and above the message in vertical layout.</param>
    /// <param name="messageTitle">An additiontial title for the main message itself (not the title of the message box). It will be above the message text in horizontal layout and above the message icon in vertical layout. The UI element itself will hide if set to null or an empty string.</param>
    /// <param name="messageBoxTitle">The title for the message box. The title bar will hide if set to null or an empty string.</param>
    /// <param name="showCloseButtonInTitleBar">Indicates whether the title bar will show a close button if the title bar is displayed.</param>
    /// <param name="button1Icon">The icon for button 1. To hide this button, set both its icon and text to null(or an empty string).</param>
    /// <param name="button1Text">The text for button 1. To hide this button, set both its icon and text to null(or an empty string).</param>
    /// <param name="button1Action">The action to take when button 1 is pressed.</param>
    /// <param name="button2Icon">The icon for button 2. To hide this button, set both its icon and text to null(or an empty string).</param>
    /// <param name="button2Text">The text for button 2. To hide this button, set both its icon and text to null(or an empty string).</param>
    /// <param name="button2Action">The action to take when button 2 is pressed.</param>
    /// <param name="button3Icon">The icon for button 3. To hide this button, set both its icon and text to null(or an empty string).</param>
    /// <param name="button3Text">The text for button 3. To hide this button, set both its icon and text to null(or an empty string).</param>
    /// <param name="button3Action">The action to take when button 3 is pressed.</param>
    /// <param name="toggleText">The text for the toggle. Will hide the toggle if set to null or an empty string.</param>
    /// <param name="toggleAction">The action to take when the toggle's state is changed. Setting this to null will hide the toggle.</param>
    /// <param name="defaultToggleState">Decides whether the toggle is on when first displayed.</param>
    /// <param name="inputField1Label">The label text for the optional input field 1. Will hide the input field if set to null or an empty string.</param>
    /// <param name="inputField1Text">Optional text to show in input field 1 when it's first displayed</param>
    /// <param name="inputField2Label">The label text for the optional input field 2. Will hide the input field if set to null or an empty string.</param>
    /// <param name="inputField2Text">Optional text to show in input field 2 when it's first displayed</param>
    /// <param name="inputFieldAction">The action to take if any of the input field is displayed. The action requires two strings for each input field.</param>
    /// <param name="inputFieldActionsFireOnButton">Controls which button will fire the input field action. The default is button 1.</param>
    /// <param name="inAnimation">The animation type to use for animating in.</param>
    /// <param name="inAnimationStartSound">The audio clip to play when the message box begins to animate in.</param>
    /// <param name="inAnimationEndSound">The audio clip to play when the message box finishes its in-animation. This is useful when you want to emphasize when the message box is fully present, e.g. when telling the player that an achievement has be achieved.</param>
    /// <param name="outAnimation">The animation type to use for animating out.</param>
    /// <param name="outAnimationStartSound">The audio clip to play when the message box begins to animate out.</param>
    /// <param name="layout">Controls which layout to use for the message box.</param>
    /// <param name="multipleCallBehaviour">Controls the behaviour when a new call is made to show a message box while there is already a message box displaying on the screen. When set to Queue, the new calls is automatically queued and will show based on a first-in, first-out rule when an existing message box is closed. When set to ShowNewInstance, a new message box instance will show immediately.</param>
    /// <param name="dragType">Controls how the message box can be dragged.</param>
    /// <param name="templateId">Controls which template to use when displaying a message box. This is the index of the Templates list and can be found in the inspector of EasyMessageBox component.</param>
    public static void Show(string message,
                            Sprite messageIcon = null,
                            string messageTitle = null,
                            string messageBoxTitle = null,
                            bool showCloseButtonInTitleBar = false,
                            Sprite button1Icon = null,
                            string button1Text = "OK",
                            UnityAction button1Action = null,
                            Sprite button2Icon = null,
                            string button2Text = null,
                            UnityAction button2Action = null,
                            Sprite button3Icon = null,
                            string button3Text = null,
                            string toggleText = null,
                            UnityAction button3Action = null,
                            UnityAction<bool> toggleAction = null,
                            bool defaultToggleState = false,
                            string inputField1Label = null,
                            string inputField1Text = null,
                            string inputField2Label = null,
                            string inputField2Text = null,
                            UnityAction<string, string> inputFieldAction = null,
                            Buttons inputFieldActionsFireOnButton = Buttons.Button1,
                            InAnimationTypes inAnimation = InAnimationTypes.Popup,
                            AudioClip inAnimationStartSound = null,
                            AudioClip inAnimationEndSound = null,
                            OutAnimationTypes outAnimation = OutAnimationTypes.None,
                            AudioClip outAnimationStartSound = null,
                            MessageLayout layout = MessageLayout.Horizontal,
                            MultipleCallBehaviours multipleCallBehaviour = MultipleCallBehaviours.ShowNewInstance,
                            DragTypes dragType = DragTypes.TitleOnly,
                            int templateId = 0
                           )
    {
        var param = new MessageBoxParams()
        {
            Message = message,
            MessageIcon = messageIcon,
            MessageTitle = messageTitle,
            MessageBoxTitle = messageBoxTitle,
            ShowTitleBarCloseButton = showCloseButtonInTitleBar,
            Button1Icon = button1Icon,
            Button1Text = button1Text,
            Button1Action = button1Action,
            Button2Icon = button2Icon,
            Button2Text = button2Text,
            Button2Action = button2Action,
            Button3Icon = button3Icon,
            Button3Text = button3Text,
            Button3Action = button3Action,
            ToggleText = toggleText,
            ToggleValueChangedAction = toggleAction,
            InputField1Label = inputField1Label,
            InputField1Text = inputField1Text,
            InputField2Label = inputField2Label,
            InputField2Text = inputField2Text,
            InputFieldAction = inputFieldAction,
            InputFieldActionsFireOnButton = inputFieldActionsFireOnButton,
            DefaultToggleState = defaultToggleState,
            InAnimation = inAnimation,
            InAnimationStartSound = inAnimationStartSound,
            InAnimationEndSound = inAnimationEndSound,
            OutAnimation = outAnimation,
            OutAnimationStartSound = outAnimationStartSound,
            MessageLayout = layout,
            MultipleCallBehaviour = multipleCallBehaviour,
            DragType = dragType,
            TemplateId = templateId,
        };

        Show(param);
    }


    /// <summary>
    /// Show the message box using specified param.
    /// </summary>
    /// <returns>The show.</returns>
    /// <param name="param">Parameter.</param>
    public static void Show(MessageBoxParams param)
    {
        if (Instance == null)
        {
            GameObject canvasObject = new GameObject();
            canvasObject.name = "Easy Message Box Canvas";
            canvasObject.AddComponent<Canvas>();
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            canvasObject.AddComponent<EasyMessageBox>();
            Instance = canvasObject.GetComponent<EasyMessageBox>();
            var messageBoxObject = (GameObject)Instantiate(settings.DefaultPrefab);
            var messageBox = messageBoxObject.GetComponent<BoxController>();
            messageBoxObject.transform.SetParent(Instance.transform);
            Instance.Templates.Add(messageBox);
            Instance.boxPool.Add(new List<BoxController>());
            Instance.boxPool[0].Add(Instance.Templates[0]);
            (messageBoxObject.transform as RectTransform).anchoredPosition = Vector2.zero;
            (messageBoxObject.transform as RectTransform).sizeDelta = Vector2.zero;
            messageBoxObject.SetActive(false);
            Debug.Log("No template found in the scene, using default message box template.");
        }
        else if (Instance.Templates == null || /*Instance.Templates.Count - 1 < param.TemplateId*/ Instance.Templates.Count == 0)
        {
            Debug.LogWarning("Cannot find template id " + param.TemplateId.ToString() + ", using default message box. Make sure to add corresponding template in the Inspector of EasyMessageBox object before you call its ID.");
            var messageBoxObject = (GameObject)Instantiate(settings.DefaultPrefab);
            var messageBox = messageBoxObject.GetComponent<BoxController>();
            messageBoxObject.transform.SetParent(Instance.transform);
            Instance.Templates.Add(messageBox);
            param.TemplateId = 0;
            Instance.boxPool.Add(new List<BoxController>());
            Instance.boxPool[0].Add(Instance.Templates[0]);
            (messageBoxObject.transform as RectTransform).anchoredPosition = Vector2.zero;
            (messageBoxObject.transform as RectTransform).sizeDelta = Vector2.zero;
            messageBoxObject.SetActive(false);
        }

        if (param.TemplateId > Instance.Templates.Count - 1)
        {
            Debug.Log(string.Format("Cannot find template id {0}, using the first template 0.", param.TemplateId.ToString()));
            param.TemplateId = 0;
        }

        switch (param.MultipleCallBehaviour)
        {
            case MultipleCallBehaviours.ShowNewInstance:
                Instance.ActualShowDialogBox(param);
                break;
            case MultipleCallBehaviours.Queue:
                if (!Instance.hasQueuedBox)
                {
                    Instance.ActualShowDialogBox(param);
                }
                else
                {
                    BoxQueue.Enqueue(param);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Close all message boxes immediately. This will also clear the queue if automatic queuing is used.
    /// </summary>
    public static void ForceCloseAllMessageBoxes()
    {
        if (Instance == null)
        {
            return;
        }

        BoxQueue.Clear();

        List<BoxController> activeBoxes = new List<BoxController>();

        for (int i = 0; i < Instance.boxPool.Count; i++)
        {
            for (int j = 0; j < Instance.boxPool[i].Count; j++)
            {
                if (Instance.boxPool[i][j].gameObject.activeSelf)
                {
                    activeBoxes.Add(Instance.boxPool[i][j]);
                }
            }
        }

        for (int i = 0; i < activeBoxes.Count; i++)
        {
            activeBoxes[i].ForceClose();
        }
    }
}