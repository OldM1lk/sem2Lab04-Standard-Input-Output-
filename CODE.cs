using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace StandartInputOutput {
  [Serializable]
  class TextFile : IOriginator {
    public string fileName;
    public string content;

    public void Print() {
      Console.WriteLine("File name : " + fileName + "\nContent : " + content);
    }

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

    object IOriginator.GetMemento() {
      return new Memento { fileName = this.fileName, content = this.content };
    }

    void IOriginator.SetMemento(object memento) {
      if (memento is Memento) {
        var tempMemento = memento as Memento;
        fileName = tempMemento.fileName;
        content = tempMemento.content;
      }
    }
  }

  class TextFileSearcher {
    public string[] SearchFile (string directoryPath, string keyword) {
      return Directory.GetFiles(directoryPath, keyword);
    }
  }

  class Memento {
    public string fileName;
    public string content;
  }

  public interface IOriginator {
    object GetMemento();
    void SetMemento(object memento);
  }

  class CareTaker {
    private object memento;

    public void SaveState(IOriginator originator) {
      memento = originator.GetMemento();
    }

    public void RestoreState(IOriginator originator) {
      originator.SetMemento(memento);
    }
  }

  class TextFileEditor {
    public TextFile textFile;
    public CareTaker careTaker;

    public TextFileEditor(string fileName) {
      textFile = new TextFile(fileName, textFile.content = "");
      careTaker = new CareTaker();
    }

    public void Write(string content) {
      textFile.content = content;
    }

    public void Save() {
      careTaker.SaveState(textFile);
    }

    public void Undo() {
      careTaker.RestoreState(textFile);
    }
  }

  class Program {
    static void Main(string[] args) {

    }
  }
}
