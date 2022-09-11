using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MKStudio.UI.MessageBox
{
    public class TipDisplayer : MonoBehaviour
    {
        public static TipDisplayer Instance;
        public Text tipText;

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
        }

        public void SetTip(string text)
        {
            tipText.text = text;
        }
    }
}