﻿using System.Collections.Generic;
using EA4S.Helpers;

namespace EA4S.Database.Management
{
    /// <summary>
    /// Custom JSON parser for WordData
    /// </summary>
    public class WordParser : DataParser<WordData, WordTable>
    {
        override protected WordData CreateData(Dictionary<string, object> dict, DatabaseObject db)
        {
            var data = new WordData();

            data.Id = ToString(dict["Id"]);
            data.Active = (ToInt(dict["Active"]) == 1);
            if (!data.Active) return null;  // Skip this data if inactive

            data.Kind = ParseEnum<WordDataKind>(data, dict["Kind"]);
            data.Category = ParseEnum<WordDataCategory>(data, dict["Category"]);
            data.Form = CustomParseForm(data, dict["Form"]);
            data.Article = ParseEnum<WordDataArticle>(data, dict["Article"]);
            data.LinkedWord = ToString(dict["LinkedWord"]);
            data.Arabic = ToString(dict["Arabic"]);
            data.Value = ToString(dict["Value"]);
            data.Letters = CustomParseLetters(data, db);
            data.Drawing = ToString(dict["Drawing"]);
            data.Intrinsic = ToFloat(dict["Intrinsic"]);

            return data;
        }

        private string[] CustomParseLetters(WordData wordData, DatabaseObject db)
        {
            var parts = ArabicAlphabetHelper.AnalyzeData(AppManager.I.DB, wordData, db);

            string[] letters = new string[parts.Count];

            for (int i = 0, count = letters.Length; i < count; ++i)
                letters[i] = parts[i].letter.Id;

            return letters;
        }

        private WordDataForm CustomParseForm(WordData data, object enum_object)
        {
            if (ToString(enum_object) == "")
            {
                return WordDataForm.Singular;
            }
            else
            {
                return ParseEnum<WordDataForm>(data, enum_object);
            }
        }

        protected override void RegenerateEnums(List<Dictionary<string, object>> rowdicts_list)
        {
            ExtractEnum(rowdicts_list, "Kind");
            ExtractEnum(rowdicts_list, "Category", addNoneValue: true);
            //ExtractEnum(rowdicts_list, "Form");   // @note: cannot auto-generate or Singular won't work
            ExtractEnum(rowdicts_list, "Article", addNoneValue: true);
        }

        protected override void FinalValidation(WordTable table, DatabaseObject db)
        {
            // Field 'LinkedWord' is validated with a final validation step, since it is based on this same table
            foreach (var data in table.GetValuesTyped())
            {
                if (data.LinkedWord != "" && table.GetValue(data.LinkedWord) == null)
                {
                    LogValidation(data, "Cannot find id of WordData for Linked value " + data.LinkedWord + " (found in word " + data.Id + ")");
                }
            }

        }
    }
}
