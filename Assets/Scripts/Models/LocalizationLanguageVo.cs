using System;
using System.Collections.Generic;
using Enums;

namespace Models
{
    [Serializable]
    public class LocalizationLanguageVo
    {
        public ELanguage Language;
        public List<FieldInfoVo> LocalizedTexts;
    }
}