﻿using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopDecoration : MonoBehaviour
    {
        public string id;
        public bool locked = true;

        public void Unlock()
        {
            if (!locked) return;
            locked = false;
            gameObject.SetActive(true);
        }
    }
}