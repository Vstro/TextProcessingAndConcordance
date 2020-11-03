using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessing
{
    class Concordance
    {
        private class Element
        {
            public String Word { get; set; }
            public UInt16 ThePageFrequency { get; set; }
        }

        private List<Element> PageConcordance { get; set; } = new List<Element>();
        private UInt32 PageNum { get; set; } = 0;
        public String TextFileName { get; set; }
        public int LinesInPage { get; set; }


        public Concordance(int linesInPage = 10, String textFileName = "text.txt")
        {
            TextFileName = textFileName;
            LinesInPage = linesInPage;
        }

        public void WriteConcordance(String newTextFileName)
        {

            using (StreamReader file = new StreamReader(TextFileName, Encoding.UTF8))
            {
                using (StreamWriter tempFile = new StreamWriter("0.concordance", false, Encoding.UTF8))
                {
                    ReadCurrentPageConcordance(file);
                    foreach (Element e in PageConcordance)
                    {
                        tempFile.WriteLine(e.Word + " " + e.ThePageFrequency + " " + PageNum);
                    }
                }

                while (!file.EndOfStream)
                {
                    using (StreamWriter newTotalConcordance = new StreamWriter(
                        PageNum % 2 + ".concordance", false, Encoding.UTF8))
                    {
                        using (StreamReader oldTotalConcordance = new StreamReader(
                            (PageNum + 1) % 2 + ".concordance", Encoding.UTF8))
                        {
                            ReadCurrentPageConcordance(file);
                            UniteTotalAndCurrentPageConcordances(newTotalConcordance, oldTotalConcordance);
                        }
                    }
                }
            }
            using (StreamWriter newFile = new StreamWriter(newTextFileName, false, Encoding.UTF8))
            {
                using (StreamReader tempTotalConcordance = new StreamReader((PageNum + 1) % 2 + ".concordance", Encoding.UTF8))
                {
                    WriteFormattedConcordance(newFile, tempTotalConcordance);
                }
            }
            File.Delete("0.concordance");
            File.Delete("1.concordance");
        }

        private void UnitePageConcordanceDuplicates()
        {
            for (int i = 0; i < PageConcordance.Count; i++)
            {
                for (int j = i + 1; j < PageConcordance.Count; j++)
                {
                    if (PageConcordance[i].Word.Equals(PageConcordance[j].Word))
                    {
                        PageConcordance[i].ThePageFrequency++;
                        PageConcordance.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        private void SortPageConcordance()
        {
            PageConcordance.Sort((e1, e2) => e1.Word.CompareTo(e2.Word));
        }

        private void ReadCurrentPageConcordance(StreamReader file)
        {
            StringBuilder currentPage = new StringBuilder();
            for (int i = 0; i < LinesInPage; i++)
            {
                currentPage.Append(file.ReadLine() + " ");
                if (file.EndOfStream) break;
            }
            PageNum++;
            PageConcordance.Clear();
            foreach (Word w in new Sentence(currentPage.ToString()).Words)
            {
                if (!w.ToStringWithoutPunctuators().Equals(""))
                {
                    PageConcordance.Add(new Element()
                    {
                        Word = w.ToStringWithoutPunctuators().ToLower(),
                        ThePageFrequency = 1
                    });
                }
            }
            UnitePageConcordanceDuplicates();
            SortPageConcordance();
        }

        private void UniteTotalAndCurrentPageConcordances(StreamWriter newTotalConcordance, StreamReader oldTotalConcordance)
        {
            int elementIndex = 0;
            int compareResult = -1;
            bool endOfOldTotalConcordance = false;
            String[] totalConcordanceElement = oldTotalConcordance.ReadLine().Split(' ');
            while (!endOfOldTotalConcordance || elementIndex < PageConcordance.Count)
            {
                if (elementIndex < PageConcordance.Count)
                {
                    compareResult = totalConcordanceElement[0]
                        .CompareTo(PageConcordance[elementIndex].Word);
                }
                if (endOfOldTotalConcordance || compareResult > 0)
                {
                    newTotalConcordance.WriteLine(
                        PageConcordance[elementIndex].Word + " " +
                        PageConcordance[elementIndex].ThePageFrequency + " " +
                        PageNum);
                    elementIndex++;
                }
                else if (elementIndex >= PageConcordance.Count || compareResult < 0)
                {
                    newTotalConcordance.WriteLine(
                        totalConcordanceElement.Aggregate((s1, s2) => s1 + " " + s2));
                    endOfOldTotalConcordance = oldTotalConcordance.EndOfStream;
                    if (!endOfOldTotalConcordance)
                        totalConcordanceElement = oldTotalConcordance.ReadLine().Split(' ');
                }
                else
                {
                    int oldTotalFrequency;
                    int.TryParse(totalConcordanceElement[1], out oldTotalFrequency);
                    newTotalConcordance.WriteLine(
                        PageConcordance[elementIndex].Word + " " +
                        (PageConcordance[elementIndex].ThePageFrequency + oldTotalFrequency) + " " +
                        totalConcordanceElement.Skip(2).Aggregate((s1, s2) => s1 + " " + s2) + " " +
                        PageNum);
                    elementIndex++;
                    endOfOldTotalConcordance = oldTotalConcordance.EndOfStream;
                    if (!endOfOldTotalConcordance)
                        totalConcordanceElement = oldTotalConcordance.ReadLine().Split(' ');
                }
            }
        }

        private void WriteFormattedConcordance(StreamWriter newFile, StreamReader tempTotalConcordance)
        {
            String[] totalConcordanceElement = tempTotalConcordance.ReadLine().Split(' ');
            char currentGroupLetter = '\uFEFF';
            while (!tempTotalConcordance.EndOfStream)
            {
                if (currentGroupLetter != totalConcordanceElement[0][0])
                {
                    newFile.WriteLine(totalConcordanceElement[0][0].ToString().ToUpper());
                    currentGroupLetter = totalConcordanceElement[0][0];
                }
                newFile.Write(totalConcordanceElement[0]);
                for (int i = 0; i < (75 - totalConcordanceElement[0].Length * 2); i++)
                {
                    newFile.Write(".");
                }
                newFile.WriteLine(totalConcordanceElement[1] + ": " +
                    totalConcordanceElement.Skip(2).Aggregate((s1, s2) => s1 + " " + s2));
                totalConcordanceElement = tempTotalConcordance.ReadLine().Split(' ');
            }
        }
    }
}
