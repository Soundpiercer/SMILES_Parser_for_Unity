﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Api
{
    public static class ApiManagement
    {
        public static async UniTask<List<string>> GetTargetGenerateFormulas(bool isRemoteServer)
        {
            List<string> formulaList;
            if (isRemoteServer)
            {
                formulaList = await GetTargetDataFromServer();
            }
            else
            {
                formulaList = RawTextDataToFormulasList(GetTargetDataFromLocalData());
            }

            return RandomlyPickFormulas(formulaList, 8);
        }

        private static async UniTask<List<string>> GetTargetDataFromServer()
        {
            var response = await PrivateApiClient.TestMolecularRequest(new TestPostMolecularRequest("aspirin"));
            return response.data.ToList();
        }

        private static string GetTargetDataFromLocalData()
        {
            return Resources.Load<TextAsset>("40smiles").text;
        }

        private static List<string> RawTextDataToFormulasList(string rawText)
        {
            var result = new List<string>();
            var sr = new StringReader(rawText);
            while (sr.Peek() >= 0)
            {
                result.Add(sr.ReadLine());
            }

            return result;
        }

        // TODO : Random하게 일정 숫자만 보여주는 기능은 추후에 빠질 수도 있을 것 같아 따로 함수로 빼둠.
        private static List<string> RandomlyPickFormulas(IReadOnlyList<string> data, int pickNum)
        {
            var randomIndexes = new List<int>();
            while (randomIndexes.Count < pickNum)
            {
                var index = Random.Range(0, data.Count - 1);
                if (randomIndexes.Contains(index)) continue;
                randomIndexes.Add(index);
            }

            var result = new List<string>();
            foreach (var i in randomIndexes)
            {
                // TODO : 소문자와 대문자, 괄호의 의미가 있어서 추후에 유지하는 지에 대한 의문
                string formula = Regex.Replace(data[i], @"[()23\[\]]", string.Empty).ToUpper()
                    .Replace('=', 'D')
                    .Replace('#', 'T')
                    ;

                result.Add(formula);
            }

            return result;
        }
    }
}