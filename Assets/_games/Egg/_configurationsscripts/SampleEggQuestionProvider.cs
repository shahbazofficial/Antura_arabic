﻿using System.Collections.Generic;

namespace EA4S.Egg
{
    public class SampleEggQuestionProvider : IQuestionProvider
    {
        float difficulty;

        ILivingLetterDataProvider letterProvider;

        public SampleEggQuestionProvider(float difficulty)
        {
            this.difficulty = difficulty;

            letterProvider = new SampleLetterProvider();
        }

        public IQuestionPack GetNextQuestion()
        {
            ILivingLetterData questionSentence = null;

            List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
            List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

            if (difficulty < 0.5f)
            {
                correctAnswers.Add(AppManager.Instance.Teacher.GimmeARandomLetter());

                while (wrongAnswers.Count < 8)
                {
                    var letter = AppManager.Instance.Teacher.GimmeARandomLetter();

                    if (!CheckIfContains(correctAnswers, letter) && !CheckIfContains(wrongAnswers, letter))
                    {
                        wrongAnswers.Add(letter);
                    }
                }
            }
            else
            {
                while (correctAnswers.Count < 8)
                {
                    var letter = AppManager.Instance.Teacher.GimmeARandomLetter();

                    if (!CheckIfContains(correctAnswers, letter))
                    {
                        correctAnswers.Add(letter);
                    }
                }
            }

            return new SampleQuestionPack(questionSentence, wrongAnswers, correctAnswers);
        }

        static bool CheckIfContains(List<ILivingLetterData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Key == letter.Key)
                    return true;
            return false;
        }

        public string GetDescription()
        {
            return "Question Description";
        }
    }
}
