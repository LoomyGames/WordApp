using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MKStudio.UI.MessageBox
{
    public class BoxController : MonoBehaviour
    {
        public RectTransform messageBox;

        public Button button1;
        public Button button2;
        public Button button3;
        
        public Text button1Text;
        public Text button2Text;
        public Text button3Text;

        public Transform horizontalGroup;
        public Transform verticalGroup;

        public Text messageTextHorizontal;
        public Image messageIconHorizontal;
        public Text messageTitleHorizontal;
        public Text messageTextVertical;
        public Image messageIconVertical;
        public Text messageTitleVertical;

        public Image button1Image;
        public Image button2Image;
        public Image button3Image;

        public Toggle toggle;
        public Text toggleText;

        public InputField InputField1;
        public InputField InputField2;
        public Text inputField1Label;
        public Text inputField2Label;

        AudioSource aSource;

        Animator animator;

        OutAnimationTypes currentOutAnimation;

        public GameObject titleBar;
        public Text messageBoxTitle;
        public Button titleBarCloseButton;

        public MessageBoxParams currentParam;

        public Image Background;

        public DragHandler titleDrag;
        public DragHandler messageDrag;

        Color backgroundLerpStartColor;
        [HideInInspector]public Color backgroundLerpEndColor;
        [HideInInspector]public bool lerpEndColorSet;
        bool lerpState;
        float lerpValue;

        public float backgroundFadeTime = 0.3f;

        [HideInInspector] public AudioClip defaultSound_InAnimationStart;
        [HideInInspector] public AudioClip defaultSound_InAnimationEnd;
        [HideInInspector] public AudioClip defaultSound_OutAnimationStart;
        [HideInInspector] public bool disableSoundEffects;
        [HideInInspector] public float dragBoundLimit = 50f;
        [HideInInspector] public Vector3 initialScaleSelf;
        [HideInInspector] public Vector3 initialScaleBackground;
        [HideInInspector] public bool initialScaleSet;
        public DragTypes CurrentDragType { get; set; }

        bool closeInvokedFlag;

        string inputField1Content;
        string inputField2Content;

        
        void Awake()
        {
            aSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            backgroundLerpStartColor = new Color(Background.color.r, Background.color.g, Background.color.b, 0f);
            if (!lerpEndColorSet)
            {
                backgroundLerpEndColor = Background.color;
                lerpEndColorSet = true;
            }
            if (!initialScaleSet)
            {
                initialScaleSelf = transform.localScale;
                initialScaleBackground = Background.transform.localScale;
                initialScaleSet = true;
            }
        }

        /// <summary>
        /// Initiate the message box with a param set.
        /// </summary>
        /// <param name="param">The param set used to initiate.</param>
        public void Initiate(MessageBoxParams param)
        {
            this.currentParam = param;
            currentOutAnimation = param.OutAnimation;

            //reset transform
            messageBox.anchoredPosition = Vector2.zero;
            messageBox.localRotation = Quaternion.identity;
            messageBox.localScale = Vector3.one;
            transform.localScale = initialScaleSelf;
            Background.transform.localScale = initialScaleBackground;

            //layout
            if (param.MessageLayout == MessageLayout.Horizontal)
            {
                horizontalGroup.gameObject.SetActive(true);
                verticalGroup.gameObject.SetActive(false);
            } else if (param.MessageLayout == MessageLayout.Vertical)
            {
                horizontalGroup.gameObject.SetActive(false);
                verticalGroup.gameObject.SetActive(true);
            }
            //background
            Background.color = Color.clear;
            lerpState = true;
            lerpValue = 0f;

            //message
            if (string.IsNullOrEmpty(param.Message))
            {
                messageTextHorizontal.gameObject.SetActive(false);
                messageTextVertical.gameObject.SetActive(false);
            }
            else
            {
                messageTextHorizontal.gameObject.SetActive(true);
                messageTextVertical.gameObject.SetActive(true);
                messageTextHorizontal.text = param.Message;
                messageTextVertical.text = param.Message;
            }
            //message title
            if (string.IsNullOrEmpty(param.MessageTitle))
            {
                messageTitleHorizontal.gameObject.SetActive(false);
                messageTitleVertical.gameObject.SetActive(false);
            }
            else
            {
                messageTitleHorizontal.gameObject.SetActive(true);
                messageTitleVertical.gameObject.SetActive(true);
                messageTitleHorizontal.text = param.MessageTitle;
                messageTitleVertical.text = param.MessageTitle;
            }
            //message box title
            if (string.IsNullOrEmpty(param.MessageBoxTitle))
            {
                titleBar.gameObject.SetActive(false);
            }
            else
            {
                titleBar.gameObject.SetActive(true);
                messageBoxTitle.text = param.MessageBoxTitle;
            }
            //title bar close button
            titleBarCloseButton.gameObject.SetActive(param.ShowTitleBarCloseButton);
            if (param.ShowTitleBarCloseButton)
            {
                titleBarCloseButton.onClick.RemoveAllListeners();
                titleBarCloseButton.onClick.AddListener(OnTitleBarCloseButtonClicked);
            }
            //message icon
            if (param.MessageIcon == null)
            {
                messageIconHorizontal.transform.parent.gameObject.SetActive(false);
                messageIconVertical.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                messageIconHorizontal.transform.parent.gameObject.SetActive(true);
                messageIconVertical.transform.parent.gameObject.SetActive(true);
                messageIconHorizontal.sprite = param.MessageIcon;
                messageIconVertical.sprite = param.MessageIcon;
            }
            //button 1
            if (string.IsNullOrEmpty(param.Button1Text) && param.Button1Icon == null)
            {
                button1.gameObject.SetActive(false);
            }
            else
            {
                button1.gameObject.SetActive(true);

                button1.onClick.RemoveAllListeners();
                if (param.Button1Action != null)
                {
                    button1.onClick.AddListener(param.Button1Action);
                }
                button1.onClick.AddListener(Close);

                if (string.IsNullOrEmpty(param.Button1Text))
                {
                    button1Text.gameObject.SetActive(false);
                }
                else
                {
                    button1Text.gameObject.SetActive(true);
                    button1Text.text = param.Button1Text;
                }
                if (param.Button1Icon == null)
                {
                    button1Image.gameObject.SetActive(false);
                }
                else
                {
                    button1Image.gameObject.SetActive(true);
                    button1Image.sprite = param.Button1Icon;
                }

                if (param.InputFieldActionsFireOnButton == Buttons.Button1 || param.InputFieldActionsFireOnButton == Buttons.All)
                {
                    button1.onClick.AddListener(FireInputFieldActions);
                }
            }
            //button 2
            if (string.IsNullOrEmpty(param.Button2Text) && param.Button2Icon == null)
            {
                button2.gameObject.SetActive(false);
            }
            else
            {
                button2.gameObject.SetActive(true);

                button2.onClick.RemoveAllListeners();
                if (param.Button2Action != null)
                {
                    button2.onClick.AddListener(param.Button2Action);
                }
                button2.onClick.AddListener(Close);

                if (string.IsNullOrEmpty(param.Button2Text))
                {
                    button2Text.gameObject.SetActive(false);
                }
                else
                {
                    button2Text.gameObject.SetActive(true);
                    button2Text.text = param.Button2Text;
                }
                if (param.Button2Icon == null)
                {
                    button2Image.gameObject.SetActive(false);
                }
                else
                {
                    button2Image.gameObject.SetActive(true);
                    button2Image.sprite = param.Button2Icon;
                }
                if (param.InputFieldActionsFireOnButton == Buttons.Button2 || param.InputFieldActionsFireOnButton == Buttons.All)
                {
                    button2.onClick.AddListener(FireInputFieldActions);
                }
            }
            //button 3
            if (string.IsNullOrEmpty(param.Button3Text) && param.Button3Icon == null)
            {
                button3.gameObject.SetActive(false);
            }
            else
            {
                button3.gameObject.SetActive(true);

                button3.onClick.RemoveAllListeners();
                if (param.Button3Action != null)
                {
                    button3.onClick.AddListener(param.Button3Action);
                }
                button3.onClick.AddListener(Close);

                if (string.IsNullOrEmpty(param.Button3Text))
                {
                    button3Text.gameObject.SetActive(false);
                }
                else
                {
                    button3Text.gameObject.SetActive(true);
                    button3Text.text = param.Button3Text;
                }
                if (param.Button3Icon == null)
                {
                    button3Image.gameObject.SetActive(false);
                }
                else
                {
                    button3Image.gameObject.SetActive(true);
                    button3Image.sprite = param.Button3Icon;
                }
                if (param.InputFieldActionsFireOnButton == Buttons.Button3 || param.InputFieldActionsFireOnButton == Buttons.All)
                {
                    button3.onClick.AddListener(FireInputFieldActions);
                }
            }
            //toggle
            bool toggleEnabled = !string.IsNullOrEmpty(param.ToggleText);
            toggle.transform.parent.gameObject.SetActive(toggleEnabled);
            if (toggleEnabled)
            {
                toggleText.text = param.ToggleText;
                toggle.onValueChanged.RemoveAllListeners();
                toggle.isOn = param.DefaultToggleState;
                if (param.ToggleValueChangedAction != null)
                {
                    toggle.onValueChanged.AddListener(param.ToggleValueChangedAction);
                }
            }
            //Input fields
            if (InputField1 != null)
            {
                if (!string.IsNullOrEmpty(param.InputField1Label))
                {
                    InputField1.text = string.IsNullOrEmpty(param.InputField1Text) ? "" : param.InputField1Text;
                    InputField1.transform.parent.gameObject.SetActive(true);
                    inputField1Label.text = param.InputField1Label;
                }
                else
                {
                    InputField1.transform.parent.gameObject.SetActive(false);
                }
            }
            if (InputField2 != null)
            {
                if (!string.IsNullOrEmpty(param.InputField2Label))
                {
                    InputField2.text = string.IsNullOrEmpty(param.InputField2Text) ? "" : param.InputField2Text;
                    InputField2.transform.parent.gameObject.SetActive(true);
                    inputField2Label.text = param.InputField2Label;
                }
                else
                {
                    InputField2.transform.parent.gameObject.SetActive(false);
                }
            }

            //animations
            switch (param.InAnimation)
            {
                case InAnimationTypes.None:
                    animator.SetBool("In_None", true);
                    animator.SetBool("In_Popup", false);
                    animator.SetBool("In_Fade", false);
                    animator.SetBool("In_Stamp", false);
                    animator.SetBool("In_Spin", false);
                    animator.SetBool("In_HorizontalExpand", false);
                    animator.SetBool("In_VerticalExpand", false);
                    animator.SetBool("Out_None", false);
                    animator.SetBool("Out_Fade", false);
                    animator.SetBool("Out_Shrink", false);
                    animator.SetBool("Out_Spin", false);
                    break;
                case InAnimationTypes.Popup:
                    animator.SetBool("In_None", false);
                    animator.SetBool("In_Popup", true);
                    animator.SetBool("In_Fade", false);
                    animator.SetBool("In_Stamp", false);
                    animator.SetBool("In_Spin", false);
                    animator.SetBool("In_HorizontalExpand", false);
                    animator.SetBool("In_VerticalExpand", false);
                    animator.SetBool("Out_None", false);
                    animator.SetBool("Out_Fade", false);
                    animator.SetBool("Out_Shrink", false);
                    animator.SetBool("Out_Spin", false);
                    break;
                case InAnimationTypes.Stamp:
                    animator.SetBool("In_None", false);
                    animator.SetBool("In_Popup", false);
                    animator.SetBool("In_Fade", false);
                    animator.SetBool("In_Stamp", true);
                    animator.SetBool("In_Spin", false);
                    animator.SetBool("In_HorizontalExpand", false);
                    animator.SetBool("In_VerticalExpand", false);
                    animator.SetBool("Out_None", false);
                    animator.SetBool("Out_Fade", false);
                    animator.SetBool("Out_Shrink", false);
                    animator.SetBool("Out_Spin", false);
                    break;
                case InAnimationTypes.Spin:
                    animator.SetBool("In_None", false);
                    animator.SetBool("In_Popup", false);
                    animator.SetBool("In_Fade", false);
                    animator.SetBool("In_Stamp", false);
                    animator.SetBool("In_Spin", true);
                    animator.SetBool("In_HorizontalExpand", false);
                    animator.SetBool("In_VerticalExpand", false);
                    animator.SetBool("Out_None", false);
                    animator.SetBool("Out_Fade", false);
                    animator.SetBool("Out_Shrink", false);
                    animator.SetBool("Out_Spin", false);
                    break;
                case InAnimationTypes.HorizontalExpand:
                    animator.SetBool("In_None", false);
                    animator.SetBool("In_Popup", false);
                    animator.SetBool("In_Fade", false);
                    animator.SetBool("In_Stamp", false);
                    animator.SetBool("In_Spin", false);
                    animator.SetBool("In_HorizontalExpand", true);
                    animator.SetBool("In_VerticalExpand", false);
                    animator.SetBool("Out_None", false);
                    animator.SetBool("Out_Fade", false);
                    animator.SetBool("Out_Shrink", false);
                    animator.SetBool("Out_Spin", false);
                    break;
                case InAnimationTypes.VerticalExpand:
                    animator.SetBool("In_None", false);
                    animator.SetBool("In_Popup", false);
                    animator.SetBool("In_Fade", false);
                    animator.SetBool("In_Stamp", false);
                    animator.SetBool("In_Spin", false);
                    animator.SetBool("In_HorizontalExpand", false);
                    animator.SetBool("In_VerticalExpand", true);
                    animator.SetBool("Out_None", false);
                    animator.SetBool("Out_Fade", false);
                    animator.SetBool("Out_Shrink", false);
                    animator.SetBool("Out_Spin", false);
                    break;
                default:
                    animator.SetBool("In_None", true);
                    animator.SetBool("In_Popup", false);
                    animator.SetBool("In_Fade", false);
                    animator.SetBool("In_Stamp", false);
                    animator.SetBool("In_Spin", false);
                    animator.SetBool("In_HorizontalExpand", false);
                    animator.SetBool("In_VerticalExpand", false);
                    animator.SetBool("Out_None", false);
                    animator.SetBool("Out_Fade", false);
                    animator.SetBool("Out_Shrink", false);
                    animator.SetBool("Out_Spin", false);
                    break;
            }

            //drag
            CurrentDragType = param.DragType;

            //sound effects
            if (!disableSoundEffects)
            {
                if (param.InAnimationStartSound != null)
                {
                    aSource.PlayOneShot(param.InAnimationStartSound);
                }
                else if (defaultSound_InAnimationStart != null)
                {
                    aSource.PlayOneShot(defaultSound_InAnimationStart);
                }
            }
            //close flag
            closeInvokedFlag = false;
        }

        void OnMessageBoxFullyShowed()
        {
            if (!disableSoundEffects)
            {
                if (currentParam != null && currentParam.InAnimationEndSound != null)
                {
                    aSource.PlayOneShot(currentParam.InAnimationEndSound);
                }
                else if (defaultSound_InAnimationEnd != null)
                {
                    aSource.PlayOneShot(defaultSound_InAnimationEnd);
                }
            }
        }

        public void OnTitleBarCloseButtonClicked()
        {
            Close();
        }
        
        void Close()
        {
            switch (currentOutAnimation)
            {
                case OutAnimationTypes.None:
                    animator.SetBool("In_None", false);
                    animator.SetBool("In_Popup", false);
                    animator.SetBool("In_Fade", false);
                    animator.SetBool("In_Stamp", false);
                    animator.SetBool("In_Spin", false);
                    animator.SetBool("In_HorizontalExpand", false);
                    animator.SetBool("In_VerticalExpand", false);
                    animator.SetBool("Out_None", true);
                    animator.SetBool("Out_Fade", false);
                    animator.SetBool("Out_Shrink", false);
                    animator.SetBool("Out_Spin", false);
                    break;
                case OutAnimationTypes.Shrink:
                    animator.SetBool("In_None", false);
                    animator.SetBool("In_Popup", false);
                    animator.SetBool("In_Fade", false);
                    animator.SetBool("In_Stamp", false);
                    animator.SetBool("In_Spin", false);
                    animator.SetBool("In_HorizontalExpand", false);
                    animator.SetBool("In_VerticalExpand", false);
                    animator.SetBool("Out_None", false);
                    animator.SetBool("Out_Fade", false);
                    animator.SetBool("Out_Shrink", true);
                    animator.SetBool("Out_Spin", false);
                    break;
                case OutAnimationTypes.Spin:
                    animator.SetBool("In_None", false);
                    animator.SetBool("In_Popup", false);
                    animator.SetBool("In_Fade", false);
                    animator.SetBool("In_Stamp", false);
                    animator.SetBool("In_Spin", false);
                    animator.SetBool("In_HorizontalExpand", false);
                    animator.SetBool("In_VerticalExpand", false);
                    animator.SetBool("Out_None", false);
                    animator.SetBool("Out_Fade", false);
                    animator.SetBool("Out_Shrink", false);
                    animator.SetBool("Out_Spin", true);
                    break;
                case OutAnimationTypes.Fade:
                    animator.SetBool("In_None", false);
                    animator.SetBool("In_Popup", false);
                    animator.SetBool("In_Fade", false);
                    animator.SetBool("In_Stamp", false);
                    animator.SetBool("In_Spin", false);
                    animator.SetBool("In_HorizontalExpand", false);
                    animator.SetBool("In_VerticalExpand", false);
                    animator.SetBool("Out_None", false);
                    animator.SetBool("Out_Fade", true);
                    animator.SetBool("Out_Shrink", false);
                    animator.SetBool("Out_Spin", false);
                    break;
                default:
                    animator.SetBool("In_None", false);
                    animator.SetBool("In_Popup", false);
                    animator.SetBool("In_Fade", false);
                    animator.SetBool("In_Stamp", false);
                    animator.SetBool("In_Spin", false);
                    animator.SetBool("In_HorizontalExpand", false);
                    animator.SetBool("In_VerticalExpand", false);
                    animator.SetBool("Out_None", true);
                    animator.SetBool("Out_Fade", false);
                    animator.SetBool("Out_Shrink", false);
                    animator.SetBool("Out_Spin", false);
                    break;
            }

            if (!disableSoundEffects)
            {
                if (currentParam != null && currentParam.OutAnimationStartSound != null)
                {
                    aSource.PlayOneShot(currentParam.OutAnimationStartSound);
                }
                else if (defaultSound_OutAnimationStart != null)
                {
                    aSource.PlayOneShot(defaultSound_OutAnimationStart);
                }
            }

            lerpState = false;
            closeInvokedFlag = true;
        }

        void OnResetAnimationComplete()
        {
            EasyMessageBox.Instance.OnMessageBoxClosed(this);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Manually force the message box to close immediately. Will not take effect if close() is already invoked.
        /// </summary>
        public void ForceClose()
        {
            if (closeInvokedFlag)
            {
                return;
            }
            OnResetAnimationComplete();
        }

        void Update()
        {
            if (lerpState)
            {
                if (lerpValue < 1f)
                {
                    if (backgroundFadeTime < 0.02f)
                    {
                        lerpValue += 1f;
                        Background.color = backgroundLerpEndColor;
                    }
                    else
                    {
                        lerpValue += (Time.unscaledDeltaTime / backgroundFadeTime);
                        Background.color = Color.Lerp(backgroundLerpStartColor, backgroundLerpEndColor, lerpValue);
                    }
                }
            }
            else
            {
                if (lerpValue > 0)
                {
                    if (backgroundFadeTime < 0.02f)
                    {
                        lerpValue -= 1f;
                        Background.color = backgroundLerpStartColor;
                    }
                    else
                    {
                        lerpValue -= (Time.unscaledDeltaTime / backgroundFadeTime);
                        Background.color = Color.Lerp(backgroundLerpStartColor, backgroundLerpEndColor, lerpValue);
                    }
                }
            }
        }

        public void OnInputfield1Input(string text)
        {
            inputField1Content = text;
        }

        public void OnInputfield2Input(string text)
        {
            inputField2Content = text;
        }

        void FireInputFieldActions()
        {
            if (currentParam.InputFieldAction != null)
            {
                currentParam.InputFieldAction.Invoke(inputField1Content, inputField2Content);
            }
        }
    }
}