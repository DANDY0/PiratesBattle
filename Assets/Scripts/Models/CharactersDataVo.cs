using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Models
{
    [Serializable]
    public class CharactersDataVo
    {
        public List<CharacterData> CharactersData;
    }

    [Serializable]
    public class CharacterData
    {
        public Enumerators.Character Character;
        public Sprite AvatarImage;
        public Sprite FullImage;
    }
}