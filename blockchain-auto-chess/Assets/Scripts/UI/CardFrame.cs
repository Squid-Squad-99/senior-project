using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [ExecuteAlways]
    public class CardFrame : MonoBehaviour
    {
        enum CardColor
        {
            Blue,
            Orange
        }

        [SerializeField] private CardColor _cardColor;
        [SerializeField] private Sprite _characterSprite;
        [SerializeField] private string _characterName;

        [Serializable]
        struct ColorSpecificConfig
        {
            public CardColor _cardColor;
            public Color _color;
            public Sprite _frame;
        }

        [SerializeField] private ColorSpecificConfig[] _colorSpecificConfigs;

        private readonly Dictionary<CardColor, ColorSpecificConfig> _colorSpecificConfigDict =
            new Dictionary<CardColor, ColorSpecificConfig>();

        [Serializable]
        struct InternalReference
        {
            public Image _cardFramePlaceHolder;
            public Image _glowImage;
            public Image _characterPlaceHolder;
            public TMP_Text _characterNamePlaceHolder;
        }

        [SerializeField] private InternalReference _internalReference;
        private bool IsInit => _colorSpecificConfigDict.Count == _colorSpecificConfigs.Length;

        private void Init()
        {
            // set up dictionary
            foreach (var colorSpecific in _colorSpecificConfigs)
            {
                _colorSpecificConfigDict.Add(colorSpecific._cardColor, colorSpecific);
            }
        }

        private void OnValidate()
        {
            if (!IsInit) Init();
            UpdateCard();
        }

        private void UpdateCard()
        {
            // set up card
            _internalReference._cardFramePlaceHolder.sprite = _colorSpecificConfigDict[_cardColor]._frame;
            _internalReference._glowImage.color = _colorSpecificConfigDict[_cardColor]._color;
            _internalReference._characterPlaceHolder.sprite = _characterSprite;
            _internalReference._characterNamePlaceHolder.text = _characterName;
        }

        public void FocusCard(bool focus)
        {
            _internalReference._glowImage.gameObject.SetActive(focus);
        }
    }
}