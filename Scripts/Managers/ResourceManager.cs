using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
   [SerializeField] private Emoji[] _emoji;
   private Dictionary<EmojiType, Sprite> _emojiDictionary;
   
   [SerializeField] private SFX[] _sfx;
   private Dictionary<SFXType, AudioClip> _sfxDictionary;

   public void Init()
   {
      _emojiDictionary = new Dictionary<EmojiType, Sprite>(_emoji.Length - 1);
      foreach (var emoji in _emoji)
      {
         _emojiDictionary.Add(emoji.Type, emoji.Emoij);
      }
      
      _sfxDictionary = new Dictionary<SFXType, AudioClip>(_sfx.Length - 1);
      foreach (var clip in _sfx)
      {
         _sfxDictionary.Add(clip.Type, clip.AudioClip);
      }
   }

   public Sprite GetEmojiByType(EmojiType type)
   {
      if (_emojiDictionary.TryGetValue(type, out var emoji))
      {
         return emoji;
      }
      
      DebugLog.CustomLog($"{nameof(type)} 이모티콘이 없습니다!", Color.yellow);
      return null;
   }

   public AudioClip GetAudioClipByType(SFXType type)
   {
      if (_sfxDictionary.TryGetValue(type, out var clip))
      {
         return clip;
      }
      
      DebugLog.CustomLog($"{nameof(type)} SFX 효과음이 없습니다!", Color.yellow);
      return null;
   }
}

[Serializable]
public struct Emoji
{
   public EmojiType Type;
   public Sprite Emoij;
}

[Serializable]
public struct SFX
{
   public SFXType Type;
   public AudioClip AudioClip;
}
