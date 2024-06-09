using Core.Abstracts;
using UnityEngine;

namespace Views
{
    public class ChooseCharacterHandler: Handler<ChooseCharacterPanel>
    {
        protected override void Initialize()
        {
            Debug.Log("Choose initialized");

        }
    }
}