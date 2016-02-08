using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class Record
{
	public string m_word;
	public DateTime m_nextTime;
	private ArrayList m_recordHistory;

	public Record(string word)
	{
		m_word = word;
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

		int hours = timegap * 23;
		m_nextTime = now.AddHours(hours);
		m_nextTime = m_nextTime.AddMinutes(1);
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
