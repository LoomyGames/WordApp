using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKStudio.UI.MessageBox
{
    public class TestMessageBox_WithSetup : MonoBehaviour
    {
        public Sprite shareIcon;
        public Sprite restartIcon;
        public Sprite homeIcon;

        public Sprite flowerIcon;
        public Sprite lanternIcon;
        public Sprite cashIcon;
        public Sprite dashboardIcon;

        public void OnButton_MessageBox()
        {
            EasyMessageBox.Show("This is a regular message box. My template ID is 0",
                                toggleText: "Do not show again.",
                                toggleAction: OnToggleSet);
        }

        void OnToggleSet(bool isOn)
        {
            if (isOn)
                EasyMessageBox.Show("Checked!");
            else
                EasyMessageBox.Show("Unchecked!");
        }

        public void OnButton_LevelComplete()
        {
            MessageBoxParams param = new MessageBoxParams()
            {
                TemplateId = 1,
                MessageLayout = MessageLayout.Vertical,
                MessageTitle = "Level Complete!",
                MessageIcon = dashboardIcon,
                Button1Text = "Restart",
                Button1Icon = restartIcon,
                Button2Text = "Home",
                Button2Icon = homeIcon,
                Button3Text = "Share",
                Button3Icon = shareIcon,
                InAnimation = InAnimationTypes.VerticalExpand,
                OutAnimation = OutAnimationTypes.Shrink
            };
            EasyMessageBox.Show(param);
        }

        public void OnButton_Achievement()
        {
            EasyMessageBox.Show("You have reached level 5! Please claim your reward.", messageTitle: "Novice", messageIcon: flowerIcon,
                                button1Icon: cashIcon, button1Text: "10", templateId: 2, inAnimation: InAnimationTypes.Stamp,
                                button1Action: () => { Debug.Log("You got 10 diamonds."); }, multipleCallBehaviour: MultipleCallBehaviours.Queue);
            EasyMessageBox.Show("You have reached level 100! Please claim your reward.", messageTitle: "Expert", messageIcon: lanternIcon,
                                button1Icon: cashIcon, button1Text: "1000", templateId: 2, inAnimation: InAnimationTypes.Stamp,
                                button1Action: () => { Debug.Log("You got 1000 diamonds."); }, multipleCallBehaviour: MultipleCallBehaviours.Queue);
        }

        public void OnButton_GetItem()
        {
            EasyMessageBox.Show("A magical lantern.", messageTitle: "Lantern", layout: MessageLayout.Vertical,
                                messageBoxTitle: "You got a new item", messageIcon: lanternIcon,
                                button1Text: "Keep", button2Text: "Discard", templateId: 3, multipleCallBehaviour: MultipleCallBehaviours.Queue);
            EasyMessageBox.Show("A lovely flower.", messageTitle: "Flower", layout: MessageLayout.Vertical,
                                        messageBoxTitle: "You got a new item", messageIcon: flowerIcon,
                                        button1Text: "Keep", button2Text: "Discard", templateId: 3, multipleCallBehaviour: MultipleCallBehaviours.Queue);
        }

        public void OnButton_RateUs()
        {
            EasyMessageBox.Show("Like the game? Please consider rating us!",
                                button1Text: "Rate Now", button2Text: "Maybe Later", button3Text: "Neve Show Again",
                                inAnimation: InAnimationTypes.Spin,
                                templateId: 4);
        }
    }
}