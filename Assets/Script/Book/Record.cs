﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;

public class Record
{
	public string m_word;
	public string m_log;
	public DateTime m_nextTime;
	private BookItem m_item;
	private ArrayList m_recordHistory;

	public BookItem Item
	{
		get
		{
			return m_item;
		}
		set
		{
			m_item = value;
		}
	}

	public Record()
	{
		m_recordHistory = new ArrayList();
	}

	public Record(BookItem item)
	{
		m_word = item.Word;
		m_item = item;
		m_recordHistory = new ArrayList();
	}

	public int Count()
	{
		return m_recordHistory.Count;
	}

	public void UpdateRecord(bool knowresult)
	{
		RecordEntry entry = new RecordEntry();
		entry.m_result = knowresult;
		entry.m_time = DateTime.Now;
		m_recordHistory.Add(entry);
		CalcuateNextTime(entry.m_time);
	}

	void CalcuateNextTime(DateTime now)
	{
		float wrongFactor = 0.0f;
		float correctFactor = 0.0f;
		int numContinuousCorrect = 0;
		bool continous = true;

		int timegap = 0;
		for(int idx = m_recordHistory.Count - 1; idx >= 0; idx--)
		{
			RecordEntry entry = (RecordEntry)m_recordHistory[idx];
			if(continous)
			{
				if(entry.m_result)
				{
					numContinuousCorrect++;
					timegap += numContinuousCorrect;
				}
				else
				{
					continous = false;
				}
			}

			int past = (int)(now - entry.m_time).TotalDays;
			if(past < 1)
			{
				past = 1;
			}
			if(entry.m_result)
			{
				correctFactor += 1.0f / past;
			}
			else
			{
				wrongFactor += 1.0f / past;
			}
		}

		int hours = timegap * 20;
		m_nextTime = now.AddHours(hours);
		m_nextTime = m_nextTime.AddSeconds(15);
	}

	public void UpdateInfoText(GameObject infoObj)
	{
		Text wordText = infoObj.GetComponent<Text> ();
		string text = "已记忆次数: " + m_recordHistory.Count + "\r\n";
		text += "下一次: " + m_nextTime.ToString() + "\r\n";
		int correct = 0;
		int latestCorrect = 0;
		int wrong = 0;
		foreach(RecordEntry entry in m_recordHistory)
		{
            if (entry.m_result)
			{
				correct++;
				latestCorrect++;
			}
			else
			{
				wrong++;
				latestCorrect = 0;
			}
		}
        int outputnum = 0;
        if (m_recordHistory.Count > 0)
        {
            for (int i = m_recordHistory.Count - 1; i != 0; i--)
            {
                RecordEntry entry = (RecordEntry)m_recordHistory[i];
                text += entry.m_time.ToString();
                if (entry.m_result)
                {
                    text += " 正确";
                }
                else
                {
                    text += " 错误";
                }
                text += "\r\n";
                outputnum++;
                if (outputnum > 3)
                {
                    break;
                }
            }
        }
        text += "正确: " + correct + " 错误: " + wrong + " 连续正确: " + latestCorrect + "\r\n";
		text += m_log;
		wordText.text = text;
	}

    public delegate void ProcessRecordEntry(DateTime date, bool correct);

    public void ProcessRecord(ProcessRecordEntry processEntry)
    {
        foreach (RecordEntry entry in m_recordHistory)
        {
            processEntry(entry.m_time, entry.m_result);
        }
    }

    public void Save(BinaryWriter writer)
	{
		writer.Write(m_word);
		writer.Write(m_nextTime.ToBinary());
		writer.Write(m_recordHistory.Count);
		foreach(RecordEntry entry in m_recordHistory)
		{
			entry.Save(writer);
		}
	}

	public void Load(BinaryReader reader, int version)
	{
		m_word = reader.ReadString();
		long time = reader.ReadInt64();
		m_nextTime = DateTime.FromBinary(time);
		int count = reader.ReadInt32();
		for(int i = 0; i < count; i++)
		{
			RecordEntry entry = new RecordEntry();
			entry.Load(reader, version);
			m_recordHistory.Add(entry);
		}
	}
}
