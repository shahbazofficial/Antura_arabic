﻿using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EA4S.ColorTickle
{
    public class ColorTickleGame : MiniGame
    {
        #region PUBLIC MEMBERS
        
        public GameObject m_LetterPrefab;

        [SerializeField]
        private Canvas m_ColorsCanvas;
        [SerializeField]
        private ColorTickle_AnturaController m_AnturaController;
        [SerializeField]
        private Canvas m_EndCanvas;
        [SerializeField]
        private StarFlowers m_StarsFlowers;
        [SerializeField]
        private int m_Rounds = 3;
        [SerializeField]
        private int m_MaxLives = 3;
        [SerializeField]
        [Range(0f,1f)]
        private float m_fDifficulty = 0;
        [SerializeField]
        private Music m_oBackgroundMusic;

        [HideInInspector]
        public int m_Stars = 0;


        // GAME STATES
        public IntroductionGameState IntroductionState { get; private set; }
        public QuestionGameState QuestionState { get; private set; }
        public TutorialGameState TutorialState { get; private set; }
        public PlayGameState PlayState { get; private set; }
        public ResultGameState ResultState { get; private set; }

        #endregion

        #region PRIVATE MEMBERS

        GameObject[] m_MyLetters;
        GameObject m_TutorialLetter;
        IOverlayWidget m_GameUI;

        #endregion

        #region GETTER/SETTER

        public GameObject[] myLetters
        {
            get { return m_MyLetters; }
            set { m_MyLetters = value; }
        }

        public GameObject tutorialLetter
        {
            get { return m_TutorialLetter; }
            set { m_TutorialLetter = value; }
        }

        public Canvas colorsCanvas
        {
            get { return m_ColorsCanvas; }
        }

        public ColorTickle_AnturaController anturaController
        {
            get { return m_AnturaController; }
        }

        public Canvas endCanvas
        {
            get { return m_EndCanvas; }
        }

        public StarFlowers starFlowers
        {
            get { return m_StarsFlowers; }
        }

        public int lives
        {
            get { return m_MaxLives; }
        }

        public int rounds
        {
            get { return m_Rounds; }
            set { m_Rounds = value; }
        }

        public float difficulty
        {
            get { return m_fDifficulty; }
            set { m_fDifficulty = value; }
        }

        public IOverlayWidget gameUI
        {
            get { return m_GameUI; }
            set { m_GameUI = value; }
        }

        public Music backgroundMusic
        {
            get { return m_oBackgroundMusic; }
            set { m_oBackgroundMusic = value; }
        }

        #endregion

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new IntroductionGameState(this);
            QuestionState = new QuestionGameState(this);
            TutorialState = new TutorialGameState(this);
            PlayState = new PlayGameState(this);
            ResultState = new ResultGameState(this);

            //Difficulty is decided like this: 0 <= easy <= 0.333 < medium <= 0.666 < hard <=1
            #if UNITY_EDITOR
                //while running from editor use the difficult setted from the inspector
            #else
                m_fDifficulty = GetConfiguration().Difficulty;
            #endif


            //Adjust parameters accordingly:
            //- max lives: 1 on hard, full lives on easy, mean of the two on medium
            //- Antura's probability to scare LL: linear with the difficulty, the base value is the minimum
            if (m_fDifficulty>0.666f) //hard
            {
                m_MaxLives = 1;
            }
            else if(m_fDifficulty > 0.333f) //medium
            {
                m_MaxLives = (m_MaxLives + 1) / 2;
            }
            else //easy
            {
                //on easy use the base value of max lives
            }
            m_AnturaController.probabilityToScareLL = Mathf.Max(m_AnturaController.probabilityToScareLL, m_fDifficulty * 100); //at least the base value

        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return ColorTickleConfiguration.Instance;
        }

    }
}
