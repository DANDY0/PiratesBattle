using System;
using System.Collections.Generic;
using Enums;

namespace Models
{
    [Serializable]
    public class LocalizationSettingsVo
    {
        public List<LocalizationLanguageVo> Languages;
        public ELanguage DefaultLanguage;
        public bool RefreshLocalizationAtStart = true;
    }
}