﻿using UnityEngine;
using System.Collections;

namespace EA4S
{
    public enum Music {
        Silence = 0,
        MainTheme = 1,
        Relax = 2,
        Lullaby = 3,
        Theme3 = 4,
        Theme4 = 5
    }

    public enum Sfx {
        Hit,
        BaloonPop,
        DangerClock,
        GameTitle,
        Win,
        Lose,
        UIPopup,
        UIButtonClick,
        UIPauseIn,
        UIPauseOut,
        CameraMovement,
        AlarmClock,
        LetterAngry,
        LetterHappy,
        LetterSad,
        WalkieTalkie,
        WheelStart,
        WheelTick,
        DogBarking,
        DogSnoring,
        DogSnorting,
        OK,
        KO
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager I;
        static System.Action OnNotifyEndAudio;
        bool musicEnabled = true;
        Music currentMusic;

        void Awake()
        {
            I = this;
            musicEnabled = true;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            // app is pausing
            if (pauseStatus) {
                StopMusic();
            }
            //app is resuming
            if (!pauseStatus) {
                if (musicEnabled) {
                    PlayMusic(currentMusic);
                }
            }
        }

        public void NotifyEndAudio(Fabric.EventNotificationType type, string boh, object info, GameObject gameObject)
        {
            // Debug.Log ("OnNotify:" + type + "GameObject:" + gameObject.name);
            if (info != null) {
                if (type == Fabric.EventNotificationType.OnAudioComponentStopped) {
                    //Debug.Log("NotifyEndAudio OnAudioComponentStopped()");
                    if (OnNotifyEndAudio != null) {
                        OnNotifyEndAudio();
                    }
                }
            }
        }

        public void ToggleMusic()
        {
            musicEnabled = !musicEnabled;
            if (musicEnabled) {
                PlayMusic(currentMusic);
            } else {
                StopMusic();
            }
        }

        public void PlayMusic(Music music)
        {
            currentMusic = music;
            var eventName = "";
            switch (music) {
                case Music.Silence:
                    eventName = "";
                    break;
                case Music.MainTheme:
                    eventName = "Music1";
                    break;
                case Music.Relax:
                    eventName = "Music2";
                    break;
                case Music.Lullaby:
                    eventName = "Music5";
                    break;
                case Music.Theme3:
                    eventName = "Music3";
                    break;
                case Music.Theme4:
                    eventName = "Music4";
                    break;
            }

            if (eventName == "") {
                StopMusic();
            } else {
                if (musicEnabled) {
                    Fabric.EventManager.Instance.PostEvent("MusicTrigger", Fabric.EventAction.SetSwitch, eventName);
                    Fabric.EventManager.Instance.PostEvent("MusicTrigger");
                    //Fabric.EventManager.Instance.PostEvent("Music/" + eventName);
                }
            }
        }

        public void StopMusic()
        {
            Fabric.EventManager.Instance.PostEvent("MusicTrigger", Fabric.EventAction.StopAll);
        }

        public void PlaySfx(Sfx sfx)
        {
            PlaySound(GetEventName(sfx));
        }

        public void StopSfx(Sfx sfx)
        {
            StopSound(GetEventName(sfx));
        }

        void PlaySound(string eventName)
        {
            Fabric.EventManager.Instance.PostEvent(eventName);
        }

        void PlaySound(string eventName, System.Action callback)
        {
            OnNotifyEndAudio = callback;
            Fabric.EventManager.Instance.PostEventNotify(eventName, NotifyEndAudio);
        }

        void PlaySound(string eventName, GameObject GO)
        {
            Fabric.EventManager.Instance.PostEvent(eventName, GO);
        }

        public void PlayLetter(string letterId)
        {
            Fabric.EventManager.Instance.PostEvent("VOX/Letters/" + letterId);
        }

        public void PlayWord(string wordId)
        {
            Fabric.EventManager.Instance.PostEvent("VOX/Words/" + wordId);
        }

        void StopSound(string eventName)
        {
            if (Fabric.EventManager.Instance != null)
                Fabric.EventManager.Instance.PostEvent(eventName, Fabric.EventAction.StopAll);
        }

        void FadeOutMusic(string n)
        {
            Fabric.Component component = Fabric.FabricManager.Instance.GetComponentByName(n);
            if (component != null) {
                component.FadeOut(0.1f, 0.5f);
            }
        }

        public void PlayDialog(string string_id)
        {
            Fabric.EventManager.Instance.PostEvent("KeeperDialog", Fabric.EventAction.SetSwitch, string_id);
        }

        public void PlayDialog(string string_id, System.Action callback)
        {
            OnNotifyEndAudio = callback;
            Fabric.EventManager.Instance.PostEvent("KeeperDialog", Fabric.EventAction.SetSwitch, string_id);
            Fabric.EventManager.Instance.PostEventNotify("KeeperDialog", NotifyEndAudio);
        }


        string GetEventName(Sfx sfx)
        {
            var eventName = "";
            switch (sfx) {
                case Sfx.Hit:
                    eventName = "Sfx/Hit";
                    break;
                case Sfx.DangerClock:
                    eventName = "Sfx/DangerClock";
                    break;
                case Sfx.Win:
                    eventName = "Sfx/Win";
                    break;
                case Sfx.Lose:
                    eventName = "Sfx/Lose";
                    break;
                case Sfx.BaloonPop:
                    eventName = "Sfx/BalloonPop";
                    break;
                case Sfx.UIPopup:
                    eventName = "Sfx/UI/Popup";
                    break;
                case Sfx.UIButtonClick:
                    eventName = "Sfx/UI/Button";
                    break;
                case Sfx.UIPauseIn:
                    eventName = "Sfx/UI/PauseIn";
                    break;
                case Sfx.UIPauseOut:
                    eventName = "Sfx/UI/PauseOut";
                    break;
                case Sfx.CameraMovement:
                    eventName = "Sfx/CameraMovement";
                    break;
                case Sfx.AlarmClock:
                    eventName = "Sfx/AlarmClock";
                    break;
                case Sfx.LetterAngry:
                    eventName = "LivingLetter/Angry";
                    break;  
                case Sfx.LetterHappy:
                    eventName = "LivingLetter/Happy";
                    break;  
                case Sfx.LetterSad:
                    eventName = "LivingLetter/Sad";
                    break;  
                case Sfx.GameTitle:
                    eventName = "VOX/GameTitle";
                    break;
                case Sfx.WalkieTalkie:
                    eventName = "Sfx/WalkieTalkie";
                    break;
                case Sfx.WheelStart:
                    eventName = "Sfx/WheelStart";
                    break;  
                case Sfx.WheelTick:
                    eventName = "Sfx/WheelTick";
                    break;  
                case Sfx.DogBarking:
                    eventName = "Dog/Barking";
                    break;
                case Sfx.DogSnoring:
                    eventName = "Dog/Snoring";
                    break;  
                case Sfx.DogSnorting:
                    eventName = "Dog/Snorting";
                    break;
                case Sfx.OK:
                    eventName = "Sfx/OK";
                    break;
                case Sfx.KO:
                    eventName = "Sfx/KO";
                    break;

            }
            return eventName;
        }
           
    }
}