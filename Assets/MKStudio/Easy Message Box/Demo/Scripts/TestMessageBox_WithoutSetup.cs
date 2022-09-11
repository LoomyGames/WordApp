using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKStudio.UI.MessageBox
{

    public class TestMessageBox_WithoutSetup : MonoBehaviour
    {
        public Sprite yesIcon, noIcon, mainIcon;


        public void OnButtonClick_ShowBasic()
        {
            EasyMessageBox.Show("This is a test message.");
        }

        public void OnButtonClick_ShowTitles()
        {
            EasyMessageBox.Show("This is a test message.",
                                messageTitle: "Dear developer:",
                                messageBoxTitle: "Easy Message Box");
        }

        public void OnButtonClick_ShowIcons()
        {
            EasyMessageBox.Show("Buy more ammo?",
                                button1Icon: yesIcon, button1Text: "$0.99",
                                button2Icon: noIcon, button2Text: "Cancel",
                                messageIcon: mainIcon);
        }

        public void OnButtonClick_ShowCallback1()
        {
            EasyMessageBox.Show("Want callbacks?",
                                button1Text: "Yes", button1Action: () => { EasyMessageBox.Show("Yes Clicked."); },
                                button2Text: "No", button2Action: () => { EasyMessageBox.Show("No Clicked."); });
        }

        public void OnButtonClick_ShowCallback2()
        {
            EasyMessageBox.Show("Want callbacks?",
                                button1Text: "Yes", button1Action: Yes,
                                button2Text: "No", button2Action: No);
        }

        public void OnButtonClick_Drag_Title()
        {
            EasyMessageBox.Show("I can only be dragged by my title.",
                                messageBoxTitle: "Drag Me",
                                dragType: DragTypes.TitleOnly);
        }
        public void OnButtonClick_Drag_TitleAndMessage()
        {
            EasyMessageBox.Show("I can be dragged by both the title and the message area. Drag me.",
                                messageBoxTitle: "Drag Me",
                                dragType: DragTypes.TitleAndMessage);
        }
        public void OnButtonClick_Drag_Disable()
        {
            EasyMessageBox.Show("I cannot be dragged. ",
                                messageBoxTitle: "Tough Like a Mountain.",
                                dragType: DragTypes.Disabled);
        }

        public void OnButtonClick_MultiCallNewInstance()
        {
            for (int i = 0; i < 3; i++)
            {
                EasyMessageBox.Show("This is message box #" + (i + 1).ToString(), dragType: DragTypes.TitleAndMessage);
            }
        }

        public void OnButtonClick_MultiCallQueue()
        {
            for (int i = 0; i < 3; i++)
            {
                EasyMessageBox.Show("This is message box #" + (i + 1).ToString(),
                                   multipleCallBehaviour: MultipleCallBehaviours.Queue);
            }
        }

        public void OnButtonClick_AnimationOff()
        {
            EasyMessageBox.Show("Animation is off.", inAnimation: InAnimationTypes.None, outAnimation: OutAnimationTypes.None);
        }

        public void OnButtonClick_Animation1()
        {
            EasyMessageBox.Show("In animation: Popup. Out animation: Fade.", inAnimation: InAnimationTypes.Popup, outAnimation: OutAnimationTypes.Fade);
        }

        public void OnButtonClick_Animation2()
        {
            EasyMessageBox.Show("In animation: Spin. Out animation: Spin.", inAnimation: InAnimationTypes.Spin, outAnimation: OutAnimationTypes.Spin);
        }

        public void OnButtonClick_Animation3()
        {
            EasyMessageBox.Show("In animation: Stamp. Out animation: Fade.", inAnimation: InAnimationTypes.Stamp, outAnimation: OutAnimationTypes.Fade);
        }

        public void OnButtonClick_Animation4()
        {
            EasyMessageBox.Show("In animation: Horizontal Expand. Out animation: Shrink.", inAnimation: InAnimationTypes.HorizontalExpand, outAnimation: OutAnimationTypes.Shrink);
        }

        public void OnButtonClick_Animation5()
        {
            EasyMessageBox.Show("In animation: Vertical Expand. Out animation: Shrink.", inAnimation: InAnimationTypes.VerticalExpand, outAnimation: OutAnimationTypes.Shrink);
        }

        public void OnButtonClick_Toggle()
        {
            EasyMessageBox.Show("Click the toggle.", toggleText: "Don't show next time", toggleAction: (x) =>
             {
                 if (x) { EasyMessageBox.Show("Checked."); }
                 else { EasyMessageBox.Show("Unchecked."); }
             });
        }

        public void OnButtonClick_Inputfield()
        {
            EasyMessageBox.Show("", button2Text: "Cancel", 
                                inputField1Label: "User Name", inputField2Label: "Password",
                                inputFieldAction: (x1, x2) => { EasyMessageBox.Show("User name: " + x1 + " Password: " + x2); },
                                inputFieldActionsFireOnButton: Buttons.Button1, 
                                toggleText: "Remember Me",
                                messageBoxTitle: "Login");
        }

        void Yes() { EasyMessageBox.Show("Yes Clicked."); }
        void No() { EasyMessageBox.Show("No Clicked."); }
    }
}