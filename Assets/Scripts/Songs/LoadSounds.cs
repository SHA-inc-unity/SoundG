using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static JsonDataSaver;

public class LoadSounds
{
    public async Task<List<SoundData>> LoadAllSounds(int reanalize, bool isGetNetSong)
    {
        List<SoundData> soundList = new List<SoundData>();

        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("Sound");

        Dictionary<string, (int, List<TimeValuePair>)> bits = JsonDataSaver.LoadBitList();

        List<(OwnerData, string)> owners = JsonDataSaver.LoadOwnerData();
        

        foreach (var clip in audioClips)
        {
            string soundName = clip.name;
            Sprite image = LoadImageForSound(soundName);

            // Проверка наличия анализа битов
            if (reanalize == 1 && !bits.ContainsKey(soundName))
            {
                bits[soundName] = (60, BitGenerator.AnalyzeMusic(clip));
            }
            else if (reanalize == 2)
            {
                bits[soundName] = (bits[soundName].Item1, BitGenerator.AnalyzeMusic(clip));
            }

#if !UNITY_EDITOR
            OwnerData owner = owners.FirstOrDefault(o => o.Item2 == soundName).Item1 ?? new OwnerData(null, 0);
#endif
#if UNITY_EDITOR
            OwnerData owner = owners.FirstOrDefault(o => o.Item2 == soundName).Item1 ?? new OwnerData(null, OwnerType.standart);
#endif

            soundList.Add(new SoundData(soundName, image, clip, bits[soundName].Item2, owner, bits[soundName].Item1));
        }

        List<MuzPackPreview> muzPackPreviews = MuzPackSaver.GetMuzPackPreviews();

        foreach (var item in muzPackPreviews)
        {
            if (item.OwnerType.ownedType == (OwnerType.owner | OwnerType.standart | OwnerType.buyed))
            {
                soundList.Add(MuzPackSaver.LoadMuzPack(item.Name));
            }
            else
            {
                soundList.Add(new SoundData(item.Name, item.Image, null, null, 
                    new OwnerData(item.OwnerType.owner, item.OwnerType.ownedType), 0));
            }
        }

        if (LoginData.IsLogin)
        {
            List<SoundData> loadSounds;

            if (isGetNetSong)
            {
                loadSounds = await NetServerController.Instance.LoadSongs(
                    LoginData.UserData.name, LoginData.UserData.password);
            }
            else
            {
                loadSounds = new List<SoundData>();
            }

            Dictionary<string, OwnerType> loadSoundsDict = loadSounds.ToDictionary(
                sound => sound.Name,
                sound => sound.Owner.ownedType);

            // Найдем отсутствующие песни
            List<SoundData> missingSongs = loadSounds
                .Where(sound => !soundList.Any(s => s.Name == sound.Name))
                .ToList();

            soundList = soundList
                .Where(sound =>
                    loadSoundsDict.ContainsKey(sound.Name) || // Если песня есть в loadSounds, оставляем
                    sound.Owner.ownedType == OwnerType.owner ||
                    sound.Owner.ownedType == OwnerType.standart)
                .ToList();

            owners = owners
                .Where(ownerTuple =>
                    loadSoundsDict.ContainsKey(ownerTuple.Item2) || // Проверяем, есть ли трек в loadSounds
                    ownerTuple.Item1.ownedType == OwnerType.owner ||
                    ownerTuple.Item1.ownedType == OwnerType.standart)
                .ToList();

            soundList.AddRange(missingSongs);

            foreach (var sound in soundList)
            {
                if (loadSoundsDict.TryGetValue(sound.Name, out OwnerType newOwnerType))
                {
                    sound.ResetOwner(new OwnerData(sound.Owner.owner, newOwnerType));
                }
            }

            for (int i = 0; i < owners.Count; i++)
            {
                if (loadSoundsDict.TryGetValue(owners[i].Item2, out OwnerType newOwnerType))
                {
                    owners[i] = (new OwnerData(owners[i].Item1.owner, newOwnerType), owners[i].Item2);
                }
            }
        }

        JsonDataSaver.SaveOwnerData(owners);
        JsonDataSaver.SaveBitList(bits);

        return soundList;
    }

    public Dictionary<string, (int, List<TimeValuePair>)> LoadBits()
    {
        Dictionary<string, (int, List<TimeValuePair>)> bits = JsonDataSaver.LoadBitList();

        return bits;
    }

    private Sprite LoadImageForSound(string soundName)
    {
        Sprite image = Resources.Load<Sprite>("Sound/" + soundName);
        return image;
    }
}