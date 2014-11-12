using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System;

[Serializable()]
public class GameInfo
{
    public ArrayList Tags = new ArrayList();
    public String Name = "";
    public int Rating = 1;
    public bool Done = false;

    public String TagString()
    {
        String Out = "";

        for (int i = 0; i < Tags.Count; i++)
        {
            Out += (i > 0 ? ", " : "") + (String)Tags[i];
        }

        if(Out.Length == 0)
            Out = "(None)";

        return Out;
    }
}

[XmlInclude(typeof(GameInfo))]
[Serializable()]
public class GameData
{
    public ArrayList Games = new ArrayList();

    [NonSerialized()]
    public static GameData Instance = new GameData();

    public static bool Load(String FileName)
    {
        try
        {
            FileStream ReadFileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer Serializer = new XmlSerializer(typeof(GameData));
            Instance = (GameData)Serializer.Deserialize(ReadFileStream);
            ReadFileStream.Close();
        }
        catch (Exception e)
        {
            Instance = new GameData();

            return false;
        }

        return true;
    }

    public static bool Save(String FileName)
    {
        try
        {
            TextWriter WriteFileStream = new StreamWriter(FileName);
            XmlSerializer Serializer = new XmlSerializer(typeof(GameData));
            Serializer.Serialize(WriteFileStream, Instance);
            WriteFileStream.Close();
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    public ArrayList Find(String Name, String Tags)
    {
        ArrayList ActualTags = new ArrayList();
        ArrayList Out = new ArrayList();

        String[] TagFragments = Tags.Split(",".ToCharArray());

        for (int i = 0; i < TagFragments.Length; i++)
        {
            String FinalTag = TagFragments[i].Trim();

            if (FinalTag.Length > 0)
                ActualTags.Add(FinalTag);
        }

        for (int i = 0; i < Games.Count; i++)
        {
            GameInfo Info = (GameInfo)Games[i];

            if (Info.Name.ToUpper().Contains(Name.ToUpper()))
            {
                bool AllGood = true;

                if (Info.Tags.Count != 0 && ActualTags.Count != 0)
                {
                    for (int j = 0; j < ActualTags.Count; j++)
                    {
                        bool Found = false;

                        for (int k = 0; k < Info.Tags.Count; k++)
                        {
                            if (((String)Info.Tags[k]).ToUpper().Contains(((String)ActualTags[j]).ToUpper()))
                            {
                                Found = true;

                                break;
                            }
                        }

                        if (!Found)
                        {
                            AllGood = false;

                            break;
                        }
                    }
                }

                if (AllGood)
                {
                    Out.Add(Info);
                }
            }
        }

        return Out;
    }

    public void AddGame(String Name, String Tags, int Rating, bool Done)
    {
        GameInfo Info = new GameInfo();
        Info.Name = Name;
        Info.Rating = Rating;
        Info.Done = Done;

        String[] TagFragments = Tags.Split(",".ToCharArray());

        for (int i = 0; i < TagFragments.Length; i++)
        {
            String FinalTag = TagFragments[i].Trim();

            if (FinalTag.Length > 0)
                Info.Tags.Add(FinalTag);
        }

        Games.Add(Info);
    }
}
