using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessing
{
    class Symbol
    {
        public char Value { set; get; } = '\0';
        public bool IsPunctuative { set; get; } = false;
        private static char[] punctuators = { ',', '.', '?', '!', ':', ';', '-', '(', ')', '"' };

        public Symbol() { }

        public Symbol(char value)
        {
            Value = value;
            IsPunctuative = IsPunctuation(value);
        }

        public bool IsVowel()
        {
            return "aeiouAEIOUёуеыаоэяиюЁУЕЫАОЭЯИЮ".Contains(this.Value);
        }

        public bool IsConsonant()
        {
            return "qwrtypsdfghjklzxcvbnmQWRTYPSDFGHJKLZXCVBNMйцкнгшщзхъфвпрлджчсмтьбЙЦКНГШЩЗХЪФВПРЛДЖЧСМТЬБ"
                .Contains(this.Value);
        }

        public static bool IsPunctuation(char symbol)
        {
            foreach (char punct in punctuators)
            {
                if (punct == symbol)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
