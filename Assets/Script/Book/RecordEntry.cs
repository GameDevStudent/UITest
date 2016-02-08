using UnityEngine;
using System;
using System.IO;

public class RecordEntry
{
	public bool m_result;
	public DateTime m_time;

	public void Save(BinaryWriter writer)
	{
		writer.Write(m_result);
		writer.Write(m_time.ToBinary());
	}

	public void Load(BinaryReader reader, int version)
	{
		version = 0;
		m_result = reader.ReadBoolean();
		long time = reader.ReadInt64();
		m_time = DateTime.FromBinary(time);
	}
}
