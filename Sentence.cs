using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TextProcessing
{
    class Sentence
    {
        public List<Word> Words { get; set; } = new List<Word>();
        public static char[] sentenceEndings = { '.', '?', '!' };

        public Sentence(String sentence)
        {
            String[] words = new Regex(@"[\s+\t+]").Split(sentence);
            foreach (String word in words)
            {
                if (word.Length > 0)
                {
                    Words.Add(new Word(word));
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sentence = new StringBuilder();
            foreach (Word w in Words)
            {
                sentence.Append(w + " ");
            }
            return sentence.ToString();
        }

        public static bool IsSentenceEnding(char s)
        {
            foreach (char punct in sentenceEndings)
            {
                if (punct.Equals(s))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsQuestion()
        {
            return (Words.Last().Symbols.Last().Value == '?');
        }

        public void ReplaceMatchingWords(Func<Word, bool> matcher, String replacer = "")
        {
            for (int i = 0; i < Words.Count; i++)
            {
                if (matcher(Words[i]))
                {
                    if (i == Words.Count - 1)
                    {
                        Words[i] = new Word(replacer + Words[i].EndingPunctuations());
                    }
                    else
                    {
                        Words[i] = new Word(replacer);
                    }
                }
            }
            for (int i = Words.Count - 1; i >= 0; i--)
            {
                if (Words[i].ToStringWithoutPunctuators().Equals(""))
                {
                    if (i > 0)
                    {
                        Words[i - 1] = new Word(Words[i - 1].LeadingPunctuations() +
                        Words[i - 1].ToStringWithoutPunctuators() + Words[i].ToString());
                    }
                    Words[i] = new Word("");
                }
                else
                {
                    break;
                }
            }
            Words.RemoveAll(w => w.ToString().Equals(""));
            for (int i = 0; !Words[i].UpFirstLetterCase() && i < Words.Count; i++) ;
        }
    }
}
