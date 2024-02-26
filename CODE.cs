using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace StandartInputOutput {
  [Serializable]
  class TextFile {
    public string fileName;
    public string content;

    public TextFile(string fileName, string content) {
      this.fileName = fileName;
      this.content = content;
    }

    public void BinarySerialize(FileStream fileStream) {
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      binaryFormatter.Serialize(fileStream, this);
      fileStream.Flush();
      fileStream.Close();
    }

    public void BinaryDeserialize(FileStream fileStream) {
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      TextFile deserialized = (TextFile)binaryFormatter.Deserialize(fileStream);
      fileName = deserialized.fileName;
      content = deserialized.content;
      fileStream.Close();
    }

    public void XMLSerialize(FileStream fileStream) {
      XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
      xmlSerializer.Serialize(fileStream, this);
      fileStream.Flush();
      fileStream.Close();
    }

    public void XMLDeserialize(FileStream fileStream) {
      XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
      TextFile deserialized = (TextFile)xmlSerializer.Deserialize(fileStream);
      fileName = deserialized.fileName;
      content = deserialized.content;
      fileStream.Close();
    }
  }

  class TextFileSearcher {
    public string[] SearchFile (string directoryPath, string keyword) {
      return Directory.GetFiles(directoryPath, keyword);
    }
  }

  class Program {
    static void Main(string[] args) {

    }
  }
}
