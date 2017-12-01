using Antura.LivingLetters;
using Antura.Teacher;

namespace Antura.Minigames.DancingDots
{

    public enum DancingDotsVariation
    {
        Letter = MiniGameCode.DancingDots_letter,
        LetterForm = MiniGameCode.DancingDots_letterform
    }

    public class DancingDotsConfiguration : IGameConfiguration
    {
        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }

        #region Game configurations
        public float Difficulty { get; set; }
        public bool TutorialEnabled { get; set; }
        public DancingDotsVariation Variation { get; set; }

        public void SetMiniGameCode(MiniGameCode code)
        {
            Variation = (DancingDotsVariation)code;
        }

        #endregion

        /////////////////
        // Singleton Pattern
        static DancingDotsConfiguration instance;
        public static DancingDotsConfiguration Instance
        {
            get {
                if (instance == null) {
                    instance = new DancingDotsConfiguration();
                }
                return instance;
            }
        }
        /////////////////

        private DancingDotsConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Context = new MinigamesGameContext(MiniGameCode.DancingDots_letter, System.DateTime.Now.Ticks.ToString());

            Variation = DancingDotsVariation.Letter;
            Questions = new DancingDotsQuestionProvider();
            TutorialEnabled = true;
        }

        #region external configuration call
        public static void SetConfiguration(float _difficulty, int _variation)
        {
            instance = new DancingDotsConfiguration()
            {
                Difficulty = _difficulty,
                Variation = (DancingDotsVariation)_variation,
            };
        }
        #endregion

        public IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = TutorialEnabled ? 7 : 6; // extra one for the tutorial
            int nCorrect = 1;
            int nWrong = 0;

            var builderParams = new Teacher.QuestionBuilderParameters();

            switch (Variation) {
                case DancingDotsVariation.Letter:
                    builderParams.letterFilters.excludeDiacritics = LetterFilters.ExcludeDiacritics.AllButMain;
                    builderParams.letterFilters.excludeLetterVariations = LetterFilters.ExcludeLetterVariations.All;
                    builderParams.wordFilters.excludeDiacritics = false;
                    builderParams.wordFilters.excludeLetterVariations = true;
                    builderParams.letterFilters.excludeDiphthongs = true;
                    builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);
                    break;
                case DancingDotsVariation.LetterForm:
                    // TODO CHECK NEW MINIGAME VARIATION
                    builderParams.letterFilters.excludeDiacritics = LetterFilters.ExcludeDiacritics.AllButMain;
                    builderParams.letterFilters.excludeLetterVariations = LetterFilters.ExcludeLetterVariations.All;
                    builderParams.wordFilters.excludeDiacritics = false;
                    builderParams.wordFilters.excludeLetterVariations = true;
                    builderParams.letterFilters.excludeDiphthongs = true;
                    builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);
                    break;
            }
            return builder;
        }

        public MiniGameLearnRules SetupLearnRules()
        {
            var rules = new MiniGameLearnRules();
            // example: a.minigameVoteSkewOffset = 1f;
            return rules;
        }

    }

}
