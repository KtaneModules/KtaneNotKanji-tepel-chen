using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NotKanji
{
    class Step2Data
    {
        public string Kanji;
        public string Meaning;
        public int Strokes;
        private List<char> _lst;
        private bool? _isAppended;

        public Step2Data(string kanji, string meaning, int strokes)
        {
            this.Kanji = kanji;
            this.Meaning = meaning.ToUpper();
            this.Strokes = strokes;
        }
        
        public char Matrix(bool isAppended, int row, int column, int layer)
        {
            return getAlphabet(isAppended)[9 * layer + 3 * row + column];
        }

        // [row, column, layer]
        public int[] MatrixIdx(bool isAppended, char query)
        {
            int idx = getAlphabet(isAppended).IndexOf(query);
            return new int[]
            {
                (idx / 3) % 3,
                idx % 3,
                idx / 9
            };
        }

        private List<char> getAlphabet(bool isAppended)
        {
            if (_lst != null || isAppended == _isAppended) return _lst;
            _lst = new List<char>();
            _isAppended = isAppended;
            foreach (char c in this.Meaning.ToCharArray().ToList())
            {
                if (c != ' ' && !_lst.Contains(c)) _lst.Add(c);
            }

            if (isAppended)
            {
                _lst.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ#"
                    .ToCharArray()
                    .Where(c => !Meaning.Contains(c))
                );
            }
            else
            {
                _lst.InsertRange(0, "ABCDEFGHIJKLMNOPQRSTUVWXYZ#"
                    .ToCharArray()
                    .Where(c => !Meaning.Contains(c))
                );
            }
            return _lst;
        }

        public override string ToString()
        {
            if (_isAppended == null) return $"Not sure is appended: {Kanji} {Meaning}";

            return string.Join("\n", Enumerable.Range(0, 3).Select(
                i => string.Join(" ", Enumerable.Range(0, 3).Select(
                    j => new string(Enumerable.Range(0, 3).Select(
                        k => Matrix((bool)_isAppended, i, k, j)
                    ).ToArray())
                ).ToArray())
            ).ToArray());
        }

        public static Step2Data[] Data = {
            new Step2Data("左", "Left", 5),
            new Step2Data("右", "Right", 5 ),
            new Step2Data("北", "North", 5),
            new Step2Data("東", "East", 8),
            new Step2Data("西", "West", 6),
            new Step2Data("南", "South", 9),
            new Step2Data("外", "Outside", 5),
            new Step2Data("小", "Small", 3),
            new Step2Data("中", "Inside", 4),
            new Step2Data("長", "Long", 8),
            new Step2Data("金", "Money", 8),
            new Step2Data("白", "White", 5),
            new Step2Data("雨", "Rain", 8),
            new Step2Data("秋", "Autumn", 9),
            new Step2Data("朝", "Morning", 12),
            new Step2Data("侍", "Samurai", 8),
            new Step2Data("心", "Heart", 4),
            new Step2Data("愛", "Love", 13),
            new Step2Data("馬", "Horse", 10),
            new Step2Data("祭", "Festival", 11),
            new Step2Data("家", "House", 10),
            new Step2Data("何", "What", 7),
            new Step2Data("花", "Flower", 10),
            new Step2Data("川", "River", 3),
            new Step2Data("龍", "Dragon", 16),
            new Step2Data("山", "Mountain", 3),
            new Step2Data("次", "Next", 6),
            new Step2Data("火", "Fire", 4),
            new Step2Data("人", "Person", 2),
            new Step2Data("水", "Water", 4)
        };

        public static List<Step2Data> Get4()
        {
            var idx = new List<int>();
            while(idx.Count < 4)
            {
                var i = UnityEngine.Random.Range(0, Data.Count());
                if (!idx.Contains(i)) idx.Add(i);
            }
            return idx.Select(i => Data.ElementAt(i)).ToList();
        }

    }
}
