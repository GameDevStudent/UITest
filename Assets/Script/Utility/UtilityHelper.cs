using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Utility
{
    class UtilityHelper
    {
        public static void CreateText(GameObject parent)
        {
            GameObject child = new GameObject();
            Text text = child.AddComponent<Text>();
            text.text = "aaasad";
        }
    }
}
