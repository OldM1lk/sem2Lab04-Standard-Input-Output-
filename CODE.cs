using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace StandartInputOutput {
  [Serializable]
  class TextFile : IOriginator {
    public string filePath;
    public string content;

    public TextFile(string filePath, string content) {
      this.filePath = filePath;
      this.content = content;
    }

    public void PrintContent() {
      Console.WriteLine("Содержимое файла: " + content);
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
      filePath = deserialized.filePath;
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
      filePath = deserialized.filePath;
      content = deserialized.content;
      fileStream.Close();
    }

    object IOriginator.GetMemento() {
      return new Memento { filePath = this.filePath, content = this.content };
    }

    void IOriginator.SetMemento(object memento) {
      if (memento is Memento) {
        var temporaryMemento = memento as Memento;
        filePath = temporaryMemento.filePath;
        content = temporaryMemento.content;
      }
    }
  }

  class TextFileSearcher {
    public Dictionary<string, List<string>> index = new Dictionary<string, List<string>>();

    public void SearchFile(string directoryPath, string keyword) {
      string[] files = Directory.GetFiles(directoryPath, keyword + ".txt", SearchOption.AllDirectories);

      foreach (string file in files) {
        if (!index.ContainsKey(keyword)) {
          index[keyword] = new List<string>();
        }
        index[keyword].Add(file);
      }
    }

    public void DisplayIndex() {
      Console.Clear();
      foreach (var entry in index) {
        Console.WriteLine("Ключевое слово: " + entry.Key);
        Console.WriteLine("\nРезультат Поиска: ");

        foreach (string file in entry.Value) {
          Console.WriteLine(" - " + file);
        }
      }
      Console.WriteLine();
    }
  }

  class Memento {
    public string filePath;
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

    public TextFileEditor(TextFile textFile) {
      this.textFile = textFile;
      careTaker = new CareTaker();
      Save();
    }

    public void Edit(string newContent) {
      textFile.content = newContent;
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
      int userChoice;
      bool isProgramRunning = true;
      int typeOfSerialization = 1;

      Console.Write("Введите путь, где хранится файл: ");
      string pathToFile = Console.ReadLine();
      string fileContent = File.ReadAllText(pathToFile);
      Console.Clear();
      Console.WriteLine("Файл успешно открыт!\n");

      FileStream fileStream = new FileStream(pathToFile, FileMode.Open, FileAccess.Read);
      TextFile textFile = new TextFile(pathToFile, fileContent);
      CareTaker careTaker = new CareTaker();
      careTaker.SaveState(textFile);

      while (isProgramRunning) {
        Console.WriteLine("Меню:");
        Console.WriteLine("1. Вывести содержимое файла");
        Console.WriteLine("2. Редактировать содержимое файла");
        Console.WriteLine("3. Сохранить изменения");
        Console.WriteLine("4. Откатить изменения");
        Console.WriteLine("5. Сериализовать файл");
        Console.WriteLine("6. Десериализовать файл");
        Console.WriteLine("7. Изменить тип сериализации");
        Console.WriteLine("8. Выполнить поиск текстовых файлов по ключевому слову");
        Console.WriteLine("0. Выйти из программы");
        Console.Write("\nВаш выбор: ");
        userChoice = Convert.ToInt32(Console.ReadLine());

        switch (userChoice) {
          case 0:
            isProgramRunning = false;

            break;
          case 1:
            textFile.PrintContent();

            break;
          case 2:
            TextFileEditor editableFile = new TextFileEditor(textFile);

            Console.Write("\nВведите новое содержимое файла: ");
            string newFileContent = Console.ReadLine();
            editableFile.Edit(newFileContent);

            Console.WriteLine();

            break;
          case 3:
            careTaker.SaveState(textFile);

            break;
          case 4:
            careTaker.RestoreState(textFile);

            break;
          case 5:
            if (typeOfSerialization > 0) {
              textFile.BinarySerialize(fileStream);
            } else {
              textFile.XMLSerialize(fileStream);
            }

            break;
          case 6:
            if (typeOfSerialization > 0) {
              textFile.BinaryDeserialize(fileStream);
            } else {
              textFile.XMLDeserialize(fileStream);
            }

            break;
          case 7:
            typeOfSerialization *= (-1);

            Console.WriteLine("Тип сериализации изменен");

            break;
          case 8:
            Console.Write("\nВведите директорию поиска : ");
            string pathOfSearching = Console.ReadLine();
            Console.Write("\nВведите ключевое слово: ");
            string keywordForSearching = Console.ReadLine();

            TextFileSearcher textFileSearcher = new TextFileSearcher();
            textFileSearcher.SearchFile(pathOfSearching, keywordForSearching);
            textFileSearcher.DisplayIndex();

            break;
        }
      }
      
      Console.ReadKey();
    }
  }
}
