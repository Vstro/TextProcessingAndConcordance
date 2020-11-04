using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessing
{
    class TextStream
    {
        public Sentence CurrentSentence { get; set; }
        public String TextFileName { get; set; }

        public TextStream(String textFileName = "text.txt")
        {
            TextFileName = textFileName;
        }

        public void WriteSortedByWordsAmountSentences(String newTextFileName = "processedtext.txt")
        {
            int maxWordsAmount = 0;
            using (StreamReader file = new StreamReader(TextFileName, Encoding.UTF8))
            {
                //Finding max words amount in sentence
                StringBuilder sentence = new StringBuilder();
                bool IsPreviousCharPunctuation = false;
                while (!file.EndOfStream)
                {
                    if (!Symbol.IsPunctuation((char)file.Peek()) && IsPreviousCharPunctuation)
                    {
                        CurrentSentence = new Sentence(sentence.ToString());
                        sentence.Clear();
                        IsPreviousCharPunctuation = false;

                        if (CurrentSentence.Words.Count > maxWordsAmount)
                        {
                            maxWordsAmount = CurrentSentence.Words.Count;
                        }
                    }
                    else
                    {
                        char ch = (char)file.Read();
                        IsPreviousCharPunctuation = Sentence.IsSentenceEnding(ch);
                        sentence.Append(ch);
                    }
                }
            }

            //Write sentences with 1, 2 and so on words up to maxWordsAmount
            using (StreamWriter newFile = new StreamWriter(newTextFileName, false, Encoding.UTF8))
            {
                for (int i = 1; i <= maxWordsAmount; i++)
                {
                    using (StreamReader file = new StreamReader(TextFileName, Encoding.UTF8))
                    {
                        StringBuilder sentence = new StringBuilder();
                        bool IsPreviousCharPunctuation = false;
                        while (!file.EndOfStream)
                        {
                            if (!Symbol.IsPunctuation((char)file.Peek()) && IsPreviousCharPunctuation)
                            {
                                CurrentSentence = new Sentence(sentence.ToString());
                                sentence.Clear();
                                IsPreviousCharPunctuation = false;

                                if (CurrentSentence.Words.Count == i)
                                {
                                    newFile.WriteLine(CurrentSentence);
                                }
                            }
                            else
                            {
                                char ch = (char)file.Read();
                                IsPreviousCharPunctuation = Sentence.IsSentenceEnding(ch);
                                sentence.Append(ch);
                            }
                        }
                    }
                }
            }
        }

        public void WriteFixedLengthWordsFromQuestions(int fixedLength, String newTextFileName = "processedtext.txt")
        {
            String tempFileName = "tmp";
            using (StreamWriter tempFile = new StreamWriter(tempFileName, false, Encoding.UTF32))
            {
                using (StreamReader file = new StreamReader(TextFileName, Encoding.UTF8))
                {
                    //Finding ALL fixed length words from questions
                    StringBuilder sentence = new StringBuilder();
                    bool IsPreviousCharPunctuation = false;
                    while (!file.EndOfStream)
                    {
                        if (!Symbol.IsPunctuation((char)file.Peek()) && IsPreviousCharPunctuation)
                        {
                            CurrentSentence = new Sentence(sentence.ToString());
                            sentence.Clear();
                            IsPreviousCharPunctuation = false;

                            if (CurrentSentence.IsQuestion())
                            {
                                foreach (Word w in CurrentSentence.Words)
                                {
                                    if (w.Length() == fixedLength)
                                    {
                                        tempFile.Write(w.ToStringWithoutPunctuators() + " ");
                                    }
                                }
                            }
                        }
                        else
                        {
                            char ch = (char)file.Read();
                            IsPreviousCharPunctuation = Sentence.IsSentenceEnding(ch);
                            sentence.Append(ch);
                        }
                    }
                }
            }
            using (StreamWriter newFile = new StreamWriter(newTextFileName, false, Encoding.UTF8))
            {
                byte[] buffer = new byte[4];
                char currentChar;
                int readBytes;
                bool notEmpty = true;
                StringBuilder uniqueWord = new StringBuilder();
                StringBuilder currentWord = new StringBuilder();
                using (FileStream tempfile = new FileStream(tempFileName, FileMode.Open))
                {
                    while (notEmpty)
                    {
                        notEmpty = false;
                        readBytes = tempfile.Read(buffer, 0, buffer.Length);
                        while (readBytes == buffer.Length)
                        {
                            currentChar = Encoding.UTF32.GetChars(buffer)[0];
                            if (currentChar != ' ' && currentChar != '\uFEFF')
                            {
                                notEmpty = true;
                                uniqueWord.Append(currentChar);
                            }
                            else if (uniqueWord.Length > 0)
                            {
                                tempfile.Seek(0, SeekOrigin.Begin);
                                break;
                            }
                            readBytes = tempfile.Read(buffer, 0, buffer.Length);
                        }
                        if (uniqueWord.Length > 0)
                        {
                            newFile.Write(uniqueWord.ToString().ToLower() + " ");
                            readBytes = tempfile.Read(buffer, 0, buffer.Length);
                        }
                        while (readBytes == buffer.Length)
                        {
                            currentChar = Encoding.UTF32.GetChars(buffer)[0];
                            if (currentChar != ' ' && currentChar != '\uFEFF')
                            {
                                currentWord.Append(currentChar);
                            }
                            else
                            if (currentWord.Length > 0)
                            {
                                if (currentWord.ToString().ToLower().Equals(uniqueWord.ToString().ToLower()))
                                {
                                    tempfile.Seek(-(currentWord.Length + 1) * 4, SeekOrigin.Current);
                                    for (int i = 0; i < currentWord.Length; i++)
                                    {
                                        tempfile.Write(Encoding.UTF32.GetBytes(new char[] { ' ' }), 0, 4);
                                    }
                                }
                                currentWord.Clear();
                            }
                            readBytes = tempfile.Read(buffer, 0, buffer.Length);
                        }
                        uniqueWord.Clear();
                        tempfile.Seek(0, SeekOrigin.Begin);
                    }
                }
            }
            File.Delete(tempFileName);
        }

        public void DeleteFixedLengthWordsWithFirstConsonant(int fixedLength, String newTextFileName = "processedtext.txt")
        {
            using (StreamWriter newfile = new StreamWriter(newTextFileName, false, Encoding.UTF8))
            {
                using (StreamReader file = new StreamReader(TextFileName, Encoding.UTF8))
                {
                    StringBuilder sentence = new StringBuilder();
                    bool IsPreviousCharPunctuation = false;
                    while (!file.EndOfStream)
                    {
                        if (!Symbol.IsPunctuation((char)file.Peek()) && IsPreviousCharPunctuation)
                        {
                            CurrentSentence = new Sentence(sentence.ToString());
                            sentence.Clear();
                            IsPreviousCharPunctuation = false;

                            CurrentSentence.ReplaceMatchingWords(w =>
                            (w.Length() == fixedLength && w.Symbols[0].IsConsonant()));
                            newfile.Write(CurrentSentence);
                        }
                        else
                        {
                            char ch = (char)file.Read();
                            IsPreviousCharPunctuation = Sentence.IsSentenceEnding(ch);
                            sentence.Append(ch);
                        }
                    }
                }
            }
        }

        public void ReplaceFixedLengthWords(long targetSentenceNum, int fixedLength, String replacer, String newTextFileName = "processedtext.txt")
        {
            using (StreamWriter newfile = new StreamWriter(newTextFileName, false, Encoding.UTF8))
            {
                using (StreamReader file = new StreamReader(TextFileName, Encoding.UTF8))
                {
                    StringBuilder sentence = new StringBuilder();
                    bool IsPreviousCharPunctuation = false;
                    long currentSentenceNum = 0;
                    while (!file.EndOfStream)
                    {
                        if (!Symbol.IsPunctuation((char)file.Peek()) && IsPreviousCharPunctuation)
                        {
                            CurrentSentence = new Sentence(sentence.ToString());
                            sentence.Clear();
                            IsPreviousCharPunctuation = false;

                            if (currentSentenceNum == targetSentenceNum)
                            {
                                CurrentSentence.ReplaceMatchingWords(w => w.Length() == fixedLength, replacer);
                            }
                            newfile.Write(CurrentSentence);
                            currentSentenceNum++;
                        }
                        else
                        {
                            char ch = (char)file.Read();
                            IsPreviousCharPunctuation = Sentence.IsSentenceEnding(ch);
                            sentence.Append(ch);
                        }
                    }
                }
            }
        }
    }
}
