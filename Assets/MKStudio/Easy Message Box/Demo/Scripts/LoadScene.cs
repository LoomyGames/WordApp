using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKStudio.UI.MessageBox
{
    public class LoadScene : MonoBehaviour 
    {
        public void Load(string name)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(name);
        }
    }
}