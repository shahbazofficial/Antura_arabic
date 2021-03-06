﻿using Antura.Audio;
using Antura.Core;
using Antura.Database;
using Antura.Dog;
using Antura.LivingLetters;
using Antura.UI;
using UnityEngine;

namespace Antura.Scenes
{
    /// <summary>
    /// Controls the _Start scene, providing an entry point for all users prior to having selected a player profile. 
    /// </summary>
    public class HomeScene : SceneBase
    {
        [Header("Setup")]
        public AnturaAnimationStates AnturaAnimation = AnturaAnimationStates.sitting;

        public LLAnimationStates LLAnimation = LLAnimationStates.LL_dancing;

        [Header("References")]
        public AnturaAnimationController AnturaAnimController;

        public LivingLetterController LLAnimController;
        public GameObject DialogReservedArea;
        public GameObject ProfileSelectorUI;

        public GameObject PanelAppUpdate;

        protected override void Start()
        {
            base.Start();
            GlobalUI.ShowPauseMenu(true, PauseMenuType.StartScreen);
            AudioManager.I.PlaySound(Sfx.GameTitle);

            AnturaAnimController.State = AnturaAnimation;
            LLAnimController.State = LLAnimation;

            // after 2 seconds (after the game title audio) invite palyer to create a profile
            Invoke("TutorCreateProfile", 2.3f);

            if (AppManager.I.AppSettingsManager.IsAppJustUpdated) {
                AppManager.I.AppSettingsManager.AppUpdateCheckDone();
                OpenAppUpdatePanel();
            }
        }

        void TutorCreateProfile()
        {
            if (AppManager.I.PlayerProfileManager.GetPlayersIconData().Count < 1) {
                AudioManager.I.PlayDialogue(LocalizationDataId.Action_Createprofile);
            }
        }

        /// <summary>
        /// Start the game using the currently selected player.
        /// </summary>
        public void Play()
        {
            Debug.Log("Play with Player: " + AppManager.I.Player);

            GlobalUI.ShowPauseMenu(true);

            // TODO refactor: move this initialisation logic to the AppManager
            LogManager.I.InitNewSession();
            LogManager.I.LogInfo(InfoEvent.AppPlay, JsonUtility.ToJson(new DeviceInfo()));

            AppManager.I.NavigationManager.GoToNextScene();
        }

        #region Reserved Area

        private bool reservedAreaIsOpen = false;

        public void OnBtnReservedArea()
        {
            if (reservedAreaIsOpen) {
                CloseReservedAreaPanel();
            } else {
                OpenReservedAreaPanel();
            }
        }

        public void OnBtnQuit()
        {
            AppManager.I.QuitApplication();
        }

        public void OpenReservedAreaPanel()
        {
            AudioManager.I.PlaySound(Sfx.UIButtonClick);
            DialogReservedArea.SetActive(true);
            ProfileSelectorUI.SetActive(false);
            GlobalUI.ShowPauseMenu(false);
            reservedAreaIsOpen = true;
        }

        public void CloseReservedAreaPanel()
        {
            AudioManager.I.PlaySound(Sfx.UIButtonClick);
            DialogReservedArea.SetActive(false);
            ProfileSelectorUI.SetActive(true);
            GlobalUI.ShowPauseMenu(true, PauseMenuType.StartScreen);
            reservedAreaIsOpen = false;
        }
        #endregion

        #region FirstCheckPanel
        public void OpenAppUpdatePanel()
        {
            ProfileSelectorUI.SetActive(false);
            GlobalUI.ShowPauseMenu(false);

            PanelAppUpdate.SetActive(true);
        }

        public void CloseAppUpdatePanel()
        {
            ProfileSelectorUI.SetActive(true);
            GlobalUI.ShowPauseMenu(true, PauseMenuType.StartScreen);

            PanelAppUpdate.SetActive(false);
        }
        #endregion
    }
}