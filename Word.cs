using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessing
{
    class Word
    {
        public List<Symbol> Symbols { get; set; } = new List<Symbol>();

        public Word(String word)
        {
            foreach (char value in word)
            {
                Symbols.Add(new Symbol(value));
            }
        }

        public override String ToString()
        {
            StringBuilder word = new StringBuilder();
            foreach (Symbol s in Symbols)
            {
                word.Append(s.Value);
            }
            return word.ToString();
        }

        public String ToStringWithoutPunctuators()
        {
            StringBuilder word = new StringBuilder();
            foreach (Symbol s in Symbols)
            {
                if (!s.IsPunctuative || (s.Value == '-' && word.Length > 0 && word.Length < Symbols.Count - 1))
                {
                    word.Append(s.Value);
                }
            }
            return word.ToString();
        }

        public int Length()
        {
            int length = 0;
            foreach (Symbol symbol in Symbols)
            {
                if (!symbol.IsPunctuative)
                {
                    length++;
                }
            }
            return length;
        }

        public String LeadingPunctuations()
        {
            StringBuilder leadingPunctuations = new StringBuilder();
            foreach (Symbol s in Symbols)
            {
                if (s.IsPunctuative)
                {
                    leadingPunctuations.Append(s);
                }
                else
                {
                    break;
                }
            }
            return leadingPunctuations.ToString();
        }

        public String EndingPunctuations()
        {
            StringBuilder endingPunctuations = new StringBuilder();
            Symbols.Reverse();
            foreach (Symbol s in Symbols)
            {
                if (Sentence.IsSentenceEnding(s.Value))
                {
                    endingPunctuations.Append(s);
                }
                else
                {
                    break;
                }
            }
            Symbols.Reverse();
            return endingPunctuations.ToString();
        }

        public bool UpFirstLetterCase()
        {
            for (int i = 0; i < Symbols.Count; i++)
            {
                if (!Symbols[i].IsPunctuative)
                {
                    Symbols[i] = new Symbol(Symbols[i].ToString().ToUpper()[0]);
                    return true;
                }
            }
            return false;
        }
    }
}
