﻿using UnityEngine;
using System.Collections;
using System;
using ModularFramework.Core;
using ModularFramework.Modules;

namespace EA4S {

    public class SceneModule : SceneModuleDefault {

        public override void LoadSceneWithTransition(string _sceneToLoad, SceneTransition _transitionSettings) {
            SceneTransitioner.Show(!SceneTransitioner.IsShown, delegate { sceneTransitionDone(_sceneToLoad); });
        }

        void sceneTransitionDone(string _sceneToLoad) { 
            UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoad);
        }

        public override void SceneLoadedBehaviour() {
            if(SceneTransitioner.IsShown)
                SceneTransitioner.Show(false);
        }


    }

    /// <summary>
    /// Settings for this implementation of DataModule.
    /// </summary>
    [Serializable]
    public class SceneModuleSettings : IModuleSettings { }

}