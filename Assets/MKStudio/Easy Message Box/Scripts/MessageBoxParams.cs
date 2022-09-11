using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using MKStudio.UI.MessageBox;

public class MessageBoxParams
{
    /// <summary>
    /// The message text to show in the message box. The UI 
    /// element itself will hide if set to null or an empty string.
    /// </summary>
    public string Message;
    /// <summary>
    /// An additional title for the main message itself (not the title of the message box). It will be above the 
    /// message text in horizontal layout and above the message icon in vertical layout. The UI element itself will
    ///  hide if set to null or an empty string.
    /// </summary>
    public string MessageTitle;
    /// <summary>
    /// The title for the message box. The title bar will hide if set to null or an empty string.
    /// </summary>
    public string MessageBoxTitle;
    /// <summary>
    /// The icon for the message. The UI element itself will hide if set to null. It will be to the left of 
    /// the message in horizontal layout and above the message in vertical layout.
    /// </summary>
    public Sprite MessageIcon;
    /// <summary>
    /// The icon for button 1. The button will hide if its icon is set to null AND its text is set to null or an empty string, 
    /// otherwise it will show.
    /// </summary>
    public Sprite Button1Icon;
    /// <summary>
    /// The text for button 1. The button will hide if its icon is set to null AND its text is set to null or an empty string, 
    /// otherwise it will show.
    /// </summary>
    public string Button1Text = "OK";
    /// <summary>
    /// The icon for button 2. The button will hide if its icon is set to null AND its text is set to null or an empty string, 
    /// otherwise it will show.
    /// </summary>
    public Sprite Button2Icon;
    /// <summary>
    /// The text for button 2. The button will hide if its icon is set to null AND its text is set to null or an empty string, 
    /// otherwise it will show.
    /// </summary>
    public string Button2Text;
    /// <summary>
    /// The icon for button 3. The button will hide if its icon is set to null AND its text is set to null or an empty string, 
    /// otherwise it will show.
    /// </summary>
    public Sprite Button3Icon;
    /// <summary>
    /// The text for button 3. The button will hide if its icon is set to null AND its text is set to null or an empty string, 
    /// otherwise it will show.
    /// </summary>
    public string Button3Text;
    /// <summary>
    /// The text for the toggle. Will hide the toggle if set to null or an empty string.
    /// </summary>
    public string ToggleText;
    /// <summary>
    /// The action to take when the toggle's state is changed. 
    /// </summary>
    public UnityAction<bool> ToggleValueChangedAction;
    /// <summary>
    /// Decides whether the toggle is on when first displayed.
    /// </summary>
    public bool DefaultToggleState;
    /// <summary>
    /// The action to take when button 1 is pressed.
    /// </summary>
    public UnityAction Button1Action;
    /// <summary>
    /// The action to take when button 2 is pressed.
    /// </summary>
    public UnityAction Button2Action;
    /// <summary>
    /// The action to take when button 3 is pressed.
    /// </summary>
    public UnityAction Button3Action;
    /// <summary>
    /// The label text for the optional input field 1. Will hide the input field if set to null or an empty string.
    /// </summary>
    public string InputField1Label;
    /// <summary>
    /// The label text for the optional input field 2. Will hide the input field if set to null or an empty string.
    /// </summary>
    public string InputField2Label;
    /// <summary>
    /// Optional text to show in input field 1 when it's first displayed.
    /// </summary>
    public string InputField1Text;
    /// <summary>
    /// Optional text to show in input field 2 when it's first displayed
    /// </summary>
    public string InputField2Text;
    /// <summary>
    /// The action to take if any of the input field is displayed. The action requires two strings for each input field.
    /// </summary>
    public UnityAction<string, string> InputFieldAction;
    /// <summary>
    /// Controls which button will fire the input field action.
    /// </summary>
    public Buttons InputFieldActionsFireOnButton;
    /// <summary>
    /// The audio clip to play when the message box begins to animate in.
    /// </summary>
    public AudioClip InAnimationStartSound;
    /// <summary>
    /// The audio clip to play when the message box finishes its in-animation. This is useful when you want to emphasize when the message box
    /// is fully present, e.g. when telling the player that an achievement has be achieved.
    /// </summary>
    public AudioClip InAnimationEndSound;
    /// <summary>
    /// The audio clip to play when the message box begins to animate out.
    /// </summary>
    public AudioClip OutAnimationStartSound;
    /// <summary>
    /// The animation type to use for animating in.
    /// </summary>
    public InAnimationTypes InAnimation;
    /// <summary>
    /// The animation type to use for animating out.
    /// </summary>
    public OutAnimationTypes OutAnimation;
    /// <summary>
    /// Controls which layout to use for the message content. 
    /// </summary>
    public MessageLayout MessageLayout;
    /// <summary>
    /// Indicates whether the title bar will show a close button if the title bar is displayed.
    /// </summary>
    public bool ShowTitleBarCloseButton;
    /// <summary>
    /// Controls the behaviour when a new call is made to show a message box while there is already a message box displaying on the screen. When
    ///  set to Queue, the new calls is automatically queued and will show based on a first-in, first-out rule when existing message box is closed. 
    /// When set to ShowNewInstance, a new message box instance will show immediately.
    /// </summary>
    public MultipleCallBehaviours MultipleCallBehaviour;
    /// <summary>
    /// Controls how the message box can be dragged.
    /// </summary>
    public DragTypes DragType;
    /// <summary>
    /// Controls which template to use when displaying a message box. This is the index of the Templates list and 
    /// can be found in the inspector of EasyMessageBox component.
    /// </summary>
    public int TemplateId;
}


