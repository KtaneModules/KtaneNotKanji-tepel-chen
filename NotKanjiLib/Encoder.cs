using System;
using System.Collections.Generic;
using System.Linq;
using KeepCoding;

namespace NotKanji
{
    class Encoder
    {
        private char[] message;
        public int[] Answers;
        public List<Step2Data> Kanjis;
        private readonly ILog logger;
        private Step2Data keyA;
        private Step2Data keyB;
        private bool isKeyAAppended;
        private bool isKeyBAppended;

        public Encoder(string serial, ILog logger)
        {
            this.logger = logger;
            var element = messageToOrder.ElementAtWrap(UnityEngine.Random.Range(0, messageToOrder.Count()));
            message = element.Key.ToUpper().ToCharArray();
            Answers = element.Value;

            logger.Log("Selected message: {0}", message);
            logger.Log("Expected answer: {0}", Answers);

            Kanjis = Step2Data.Get4();

            logger.Log("Top left kanji: {0}, meaning: {1}", Kanjis[0].Kanji, Kanjis[0].Meaning);
            logger.Log("Top right kanji: {0}, meaning: {1}", Kanjis[1].Kanji, Kanjis[1].Meaning);
            logger.Log("Bottom left kanji: {0}, meaning: {1}", Kanjis[2].Kanji, Kanjis[2].Meaning);
            logger.Log("Bottom right kanji: {0}, meaning: {1}", Kanjis[3].Kanji, Kanjis[3].Meaning);


            var digitSum = Enumerable.Sum(serial
                .Where(c => c.IsBetween('0', '9'))
                .Select(c => c - '0')
            );
            var letterSum = Enumerable.Sum(serial
                .Where(c => c.IsBetween('A', 'Z'))
                .Select(c => c - 'A' + 1)
            );


            if(digitSum % 2 == 1)
            {
                keyA = Kanjis[0];
                isKeyAAppended = Kanjis[1].Strokes > 7;
                logger.Log("Top left kanji chosen as Key A. Counted {0} strokes for top right kanji. {1} alphabets.", Kanjis[1].Strokes, isKeyAAppended ? "Appending" : "Prepending");
            } else
            {
                keyA = Kanjis[1];
                isKeyAAppended = Kanjis[0].Strokes > 7;
                logger.Log("Top right kanji chosen as Key A. Counted {0} strokes for top left kanji. {1} alphabets.", Kanjis[0].Strokes, isKeyAAppended ? "Appending" : "Prepending");
            }

            if (letterSum % 2 == 0)
            {
                keyB = Kanjis[2];
                isKeyBAppended = Kanjis[3].Strokes > 7;
                logger.Log("Bottom left kanji chosen as Key B. Counted {0} strokes for bottom right kanji. {1} alphabets.", Kanjis[3].Strokes, isKeyBAppended ? "Appending" : "Prepending");
            }
            else
            {
                keyB = Kanjis[3];
                isKeyBAppended = Kanjis[2].Strokes > 7;
                logger.Log("Bottom right kanji chosen as Key B. Counted {0} strokes for bottom left kanji. {1} alphabets.", Kanjis[2].Strokes, isKeyBAppended ? "Appending" : "Prepending");
            }
        }

        public string Encode()
        {
            keyA.Matrix(isKeyAAppended, 0, 0, 0);
            logger.Log("Cube A: ");
            foreach (string line in keyA.ToString().Split("\n")) logger.Log(line);
            keyB.Matrix(isKeyBAppended, 0, 0, 0);
            logger.Log("Cube B: ");
            foreach (string line in keyB.ToString().Split("\n")) logger.Log(line);

            var lst = encodeStep2(message[0], message[1], message[2], true);
            lst.AddRange(encodeStep2(message[3], message[4], message[5], false));

            var result = new string(lst.Select(c => hiraganaToAlphabet[c]).ToArray());
            logger.Log("Encrypted message is {0}", result);
            return result;
        }

        private List<char> encodeStep2(char c1, char c2, char c3, bool isFirst)
        {
            var c1Idx = keyB.MatrixIdx(isKeyBAppended, c1);
            var c2Idx = keyB.MatrixIdx(isKeyBAppended, c2);
            var c3Idx = keyB.MatrixIdx(isKeyBAppended, c3);

            logger.Log("{0} was found in row {1}, {2} was found in column {3}, {4} was found in layer {5}", c1, c1Idx[0] + 1, c3, c3Idx[1] + 1, c2, c2Idx[2] + 1);
            var e1 = keyA.Matrix(isKeyAAppended, c1Idx[0], c3Idx[1], c2Idx[2]);
            logger.Log("Letter {0} was encoded to {1}", isFirst ? 1 : 4, e1);
            logger.Log("{0} was found in row {1}, {2} was found in column {3}, {4} was found in layer {5}", c2, c2Idx[0] + 1, c1, c1Idx[1] + 1, c3, c1Idx[2] + 1);
            var e2 = keyA.Matrix(isKeyAAppended, c2Idx[0], c1Idx[1], c3Idx[2]);
            logger.Log("Letter {0} was encoded to {1}", isFirst ? 2 : 5, e2);
            logger.Log("{0} was found in row {1}, {2} was found in column {3}, {4} was found in layer {5}", c3, c3Idx[0] + 1, c2, c2Idx[1] + 1, c1, c3Idx[2] + 1);
            var e3 = keyA.Matrix(isKeyAAppended, c3Idx[0], c2Idx[1], c1Idx[2]);
            logger.Log("Letter {0} was encoded to {1}", isFirst ? 3 : 6, e3);
            return new List<char>() { e1, e2, e3 };
        }

        public bool Check(List<int> input)
        {
            for(int i = 0; i < 4; i++)
            {
                if (input[i] != Answers[i]-1) return false;
            }
            return true;
        }

        static readonly Dictionary<string, int[]> messageToOrder = new Dictionary<string, int[]>()
        {
            { "Hizasi", new int[] {1,3,2,4} },
            { "Hiziri", new int[] {1,4,2,3} },
            { "Karate", new int[] {3,4,1,2} },
            { "Karasi", new int[] {4,1,2,3} },
            { "Karuta", new int[] {3,4,1,2} },
            { "Katate", new int[] {1,2,4,3} },
            { "Katana", new int[] {4,2,3,1} },
            { "Katura", new int[] {2,3,1,4} },
            { "Negiri", new int[] {4,3,1,2} },
            { "Negoto", new int[] {4,3,2,1} },
            { "Negura", new int[] {2,4,1,3} },
            { "Sakana", new int[] {3,2,4,1} },
            { "Sakura", new int[] {2,1,4,3} },
            { "Yagate", new int[] {3,1,4,2} },
            { "Yagura", new int[] {1,2,3,4} },
        };

        static readonly Dictionary<char, char> hiraganaToAlphabet = new Dictionary<char, char>()
        {
            { 'W', 'わ' },
            { 'R', 'ら' },
            { 'Y', 'や' },
            { 'M', 'ま' },
            { 'P', 'は' },
            { 'T', 'た' },
            { 'G', 'か' },
            { 'A', 'あ' },
            { 'L', 'り' },
            { 'B', 'ひ' },
            { 'C', 'し' },
            { 'Q', 'き' },
            { 'I', 'い' },
            { 'F', 'ふ' },
            { 'N', 'ぬ' },
            { 'S', 'す' },
            { 'V', 'く' },
            { 'U', 'う' },
            { 'H', 'へ' },
            { 'D', 'て' },
            { 'J', 'せ' },
            { 'K', 'け' },
            { 'E', 'え' },
            { '#', 'ん' },
            { 'Z', 'そ' },
            { 'X', 'こ' },
            { 'O', 'お' }
        };
    }
}
