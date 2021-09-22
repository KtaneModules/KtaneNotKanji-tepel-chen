using KeepCoding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NotKanjiLib
{
    class TPNotKanjiModule : TPScript<NotKanjiModule>
    {
        public override IEnumerator ForceSolve()
        {
            yield return null;
            var answers = Module.encoder.Answers;
            foreach (int i in answers)
            {
                Module.Keys[i-1].OnInteract();
                yield return new WaitForSeconds(0.10f);
            }
        }

        public override IEnumerator Process(string command)
        {
            yield return null;

            string[] splitted = command
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(str => str.Trim().ToLower())
                .ToArray();

            string digits;

            if (splitted.Length == 0) {
                yield return "sendtochaterror You must specify and argument.";
                yield break;
            }
            if (splitted.Length > 2)
            {
                yield return "sendtochaterror Too many argument.";
                yield break;
            }

            if(splitted.Length == 2)
            {
                if(!(splitted[0] == "press" || splitted[0] == "submit"))
                {
                    yield return "sendtochaterror First argument must be an answer, \"press\" or \"submit\"";
                    yield break;
                }
                digits = splitted[1];
            } else if(splitted.Length == 1)
            {
                digits = splitted[0];
            } else
            {
                yield return "sendtochaterror Invalid format.";
                yield break;
            }

            if(!rxDigits.IsMatch(digits) || digits.Distinct().Count() != digits.Length)
            {
                yield return "sendtochaterror The answer must be written in 4 unique digits.";
                yield break;
            }

            foreach(int i in digits.Select(c => c - '1'))
            {
                yield return new[] { Module.Keys[i] };
                yield return new WaitForSeconds(0.1f);
            }

        }

        static readonly Regex rxDigits = new Regex(@"^[1-4]{4}$");
    }
}
