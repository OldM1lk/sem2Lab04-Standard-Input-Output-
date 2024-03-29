using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace StandartInputOutput {
  [Serializable]
  class TextFile : IOriginator {
    public string Path;
    public string Content;

    public TextFile(string Path, string Content) {
      this.Path = Path;
      this.Content = Content;
    }

    public void PrintContent() {
      Console.WriteLine("Содержимое файла:\n" + Content);
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
      Path = deserialized.Path;
      Content = deserialized.Content;
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
      Path = deserialized.Path;
      Content = deserialized.Content;
      fileStream.Close();
    }

    object IOriginator.GetMemento() {
      return new Memento { Path = this.Path, Content = this.Content };
    }

    void IOriginator.SetMemento(object memento) {
      if (memento is Memento) {
        var temporaryMemento = memento as Memento;
        Path = temporaryMemento.Path;
        Content = temporaryMemento.Content;
      }
    }
  }

  class TextFileSearcher {
    public Dictionary<string, List<string>> keywordFilesPairs = new Dictionary<string, List<string>>();

    public void SearchFile(string directoryPath, string keyword) {
      string[] files = Directory.GetFiles(directoryPath, keyword + ".txt", SearchOption.AllDirectories);

      foreach (string file in files) {
        if (!keywordFilesPairs.ContainsKey(keyword)) {
          keywordFilesPairs[keyword] = new List<string>();
        }
        keywordFilesPairs[keyword].Add(file);
      }
    }

    public void DisplayIndex() {
      Console.Clear();
      foreach (var pair in keywordFilesPairs) {
        Console.WriteLine("Ключевое слово: " + pair.Key);
        Console.WriteLine("\nРезультат Поиска: ");

        foreach (string file in pair.Value) {
          Console.WriteLine(" - " + file);
        }
      }
      Console.WriteLine();
    }
  }

  class Memento {
    public string Path;
    public string Content;
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
      textFile.Content = newContent;
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
            FileStream fileStream = new FileStream(pathToFile, FileMode.Open, FileAccess.ReadWrite);

            if (typeOfSerialization > 0) {
              textFile.BinarySerialize(fileStream);
            } else {
              textFile.XMLSerialize(fileStream);
            }

            break;
          case 6:
            FileStream fileStream1 = new FileStream(pathToFile, FileMode.Open, FileAccess.Read);

            if (typeOfSerialization > 0) {
              textFile.BinaryDeserialize(fileStream1);
            } else {
              textFile.XMLDeserialize(fileStream1);
            }

            break;
          case 7:
            typeOfSerialization = -typeOfSerialization;

            Console.WriteLine("Тип сериализации изменен");

            break;
          case 8:
            Console.Write("\nВведите директорию поиска : ");
            string directoryOfSearching = Console.ReadLine();
            Console.Write("\nВведите ключевое слово: ");
            string keywordForSearching = Console.ReadLine();

            TextFileSearcher textFileSearcher = new TextFileSearcher();
            textFileSearcher.SearchFile(directoryOfSearching, keywordForSearching);
            textFileSearcher.DisplayIndex();

            break;
          default:
            Console.WriteLine("Несуществующая опция");

            break;
        }
      }

      Console.ReadKey();
    }
  }
}
