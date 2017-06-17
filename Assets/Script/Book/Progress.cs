using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Progress
{
    private const int m_version = 1;
    private string m_Id = "Default";

    public string ProgressID
    {
        get
        {
            return m_Id;
        }
        set
        {
            m_Id = value;
        }
    }

    private string m_rootPath;
    private List<string> m_books;

    public void AddBook(string book)
    {
        m_books.Add(book);
    }

    // Begin 按日统计
    private struct RecordDay
    {
        public int year, month, day;
    }

    public struct RecordDayReport
    {
        public int total;
        public int correct;
    }
    private Dictionary<RecordDay, RecordDayReport> m_report;

    public void CountOneEntry(DateTime date, bool correct)
    {
        RecordDay day;
        day.year = date.Year;
        day.month = date.Month;
        day.day = date.Day;

        RecordDayReport report;
        if (m_report.TryGetValue(day, out report))
        {
            report.total++;
            if (correct)
            {
                report.correct++;
            }
            // yay, value exists!
            m_report[day] = report;
        }
        else
        {
            report.total = 1;
            report.correct = 0;
            if (correct)
            {
                report.correct++;
            }
            // darn, lets add the value
            m_report.Add(day, report);
        }
    }

    // End

    private Dictionary<string, Record> m_wordSet;
    private List<Record> m_freshWords;
    private List<Record> m_usedWords;
    private List<Record> m_pendingWords;

    public Progress(string rootpath)
    {
        m_rootPath = rootpath;
        m_books = new List<string>();

        m_wordSet = new Dictionary<string, Record>();
        m_freshWords = new List<Record>();
        m_usedWords = new List<Record>();
        m_pendingWords = new List<Record>();

        m_report = new Dictionary<RecordDay, RecordDayReport>();
    }

    public void AddWord(BookItem item)
    {
        if (!m_wordSet.ContainsKey(item.Word))
        {
            Record rec = new Record(item);
            m_wordSet.Add(item.Word, rec);
        }
        else
        {
            Record rec = m_wordSet[item.Word];
            rec.Item = item;
        }
    }

    public void ReOrganize()
    {
        m_freshWords.Clear();
        m_usedWords.Clear();
        DateTime currentTime = DateTime.Now;

        foreach (KeyValuePair<string, Record> entry in m_wordSet)
        {
            Record record = entry.Value;
            if (record.Count() == 0)
            {
                m_freshWords.Add(record);
            }
            else
            {
                if (record.m_nextTime < currentTime)
                {
                    m_pendingWords.Add(record);
                }
                else
                {
                    m_usedWords.Add(record);
                }
            }
        }
    }

    public Record GetCurrentWord()
    {
        Record record = null;
        if (m_pendingWords.Count > 0)
        {
            record = m_pendingWords[0];
            record.m_log = "pending 0";
        }
        else if (m_freshWords.Count > 0)
        {
            System.Random random = new System.Random();
            int index = random.Next(0, m_freshWords.Count);
            if (index < m_freshWords.Count)
            {
                record = m_freshWords[index];
                record.m_log = "fresh random " + index + ", 0 to " + m_freshWords.Count;
            }
            else
            {
                record = m_freshWords[0];
                record.m_log = "fresh 0";
            }
        }
        return record;
    }

    public void ReBelance()
    {
        DateTime now = DateTime.Now;
        if (m_pendingWords.Count > 0)
        {
            Record record = m_pendingWords[0];
            if (record.m_nextTime > now)
            {
                m_pendingWords.RemoveAt(0);
                m_usedWords.Add(record);
            }
        }
        RecordComparer comparer = new RecordComparer();
        m_freshWords.Sort(comparer);
        m_freshWords.Reverse();
        if (m_freshWords.Count > 0)
        {
            Record record = m_freshWords[0];
            if (record.Count() > 0)
            {
                m_freshWords.RemoveAt(0);
                if (record.m_nextTime > now)
                {
                    m_usedWords.Add(record);
                }
                else
                {
                    m_pendingWords.Add(record);
                }
            }
        }
        m_usedWords.Sort(comparer);
        while (m_usedWords.Count > 0)
        {
            Record record = m_usedWords[0];
            if (record.m_nextTime < now)
            {
                m_pendingWords.Add(record);
                m_usedWords.RemoveAt(0);
            }
            else
            {
                break;
            }
        }
        m_pendingWords.Sort(comparer);
    }

    public void UpdateInfoText(GameObject infoObj)
    {
        Text wordText = infoObj.GetComponent<Text>();
        string text = "已认识字: " + m_usedWords.Count + "\r\n";
        text += "待背字: " + m_pendingWords.Count + "\r\n";
        text += "未认识字: " + m_freshWords.Count + "\r\n";
        wordText.text = text;
    }

    public void Save(string rootpath)
    {
        string dir = ProgressContainer.ProgressDirectory(rootpath);
        Directory.CreateDirectory(dir);
        string fullpath = ProgressContainer.FullPath(rootpath, m_Id);
        using (BinaryWriter file = new BinaryWriter(File.Open(fullpath, FileMode.OpenOrCreate)))
        {
            file.Write(m_version);
            file.Write(m_books.Count);
            foreach (string book in m_books)
            {
                file.Write(book);
            }

            file.Write(m_wordSet.Count);
            foreach (KeyValuePair<string, Record> entry in m_wordSet)
            {
                Record record = entry.Value;
                record.Save(file);
            }
        }
    }

    public void Load(string rootpath)
    {
        string dir = ProgressContainer.ProgressDirectory(rootpath);
        Directory.CreateDirectory(dir);
        string fullpath = ProgressContainer.FullPath(rootpath, m_Id);
        if (File.Exists(fullpath))
        {
            using (BinaryReader file = new BinaryReader(File.Open(fullpath, FileMode.Open)))
            {
                int version = file.ReadInt32();
                if (version >= 1)
                {
                    int booknum = file.ReadInt32();
                    for (int i = 0; i < booknum; i++)
                    {
                        string book = file.ReadString();
                        if (!m_books.Exists(x => x == book))
                        {
                            m_books.Add(book);
                        }
                    }
                }
                int count = file.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    Record record = new Record();
                    record.Load(file, version);
                    record.ProcessRecord(new Record.ProcessRecordEntry(CountOneEntry));
                    m_wordSet.Add(record.m_word, record);
                }
            }
        }

        foreach (string book in m_books)
        {
            BookDict dict = new BookDict(rootpath, book);
            dict.ReadBook(this);
            dict.SaveBook();
        }
        ReOrganize();
    }

    public RecordDayReport GetDayReport(DateTime date)
    {
        RecordDay day;
        day.year = date.Year;
        day.month = date.Month;
        day.day = date.Day;

        RecordDayReport report;
        m_report.TryGetValue(day, out report);
        return report;
    }
}