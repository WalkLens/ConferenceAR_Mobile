using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class ChipsTab : VisualElement
{
    [SerializeField] private string[] researchChipString = {"게임 기획", "서비스 기획", "콘텐츠 기획", "데이터 분석", "마케팅", "UX 기획", "비즈니스 분석", "PM"};
    [SerializeField] private string[] designChipString = {"3D 디자인", "브랜딩 디자인", "UX/UI", "모션 그래픽", "캐릭터/레벨", "VFX", "컨셉 아티스트/그래픽", "사운드"};
    [SerializeField] private string[] devChipString = {"프론트엔드", "백엔드/서버", "게임 개발", "그래픽 개발", "임베디드", "데이터/AI", "데브옵스/지원", "XR"};
    
    private int _activeTab = 0; // 0: Research, 1: Design, 2: Dev
    private Button[] _tabButtons = new Button[3];
    private VisualElement _bar;
    private VisualElement _chipsContainer;
    private VisualElement _researchContainer;
    private VisualElement _designContainer;
    private VisualElement _devContainer;

    private int _maxLength;
    private int _researchChipCount = 0;
    private int _designChipCount = 0;
    private int _devChipCount = 0;

    private List<string> _selectedKeywords = new List<string>();
    private Label _nameLabel;

    public ChipsTab(int maxLength, int style) 
    {
        this._maxLength = maxLength;
        // UXML 및 USS 불러오기
        var visualTree = Resources.Load<VisualTreeAsset>("UI/VisualTree/ChipsTab");
        visualTree.CloneTree(this);
        // styleSheets.Add(Resources.Load<StyleSheet>("Chip"));

        // 요소 참조 가져오기
        _tabButtons[0] = this.Q<Button>("tab-button-research");
        _tabButtons[1] = this.Q<Button>("tab-button-design");
        _tabButtons[2] = this.Q<Button>("tab-button-dev");

        _tabButtons[0].RegisterCallback<ClickEvent>(evt => ToggleKeywordTabButton(0));
        _tabButtons[1].RegisterCallback<ClickEvent>(evt => ToggleKeywordTabButton(1));
        _tabButtons[2].RegisterCallback<ClickEvent>(evt => ToggleKeywordTabButton(2));

        _bar = this.Q<VisualElement>("tab-bar");

        _tabButtons[0].text = "기획";
        
        _chipsContainer = this.Q<VisualElement>("chips-container");
        _researchContainer = this.Q<VisualElement>("research-chip-container");
        _designContainer = this.Q<VisualElement>("design-chip-container");
        _devContainer = this.Q<VisualElement>("dev-chip-container");

        foreach(var item in researchChipString)
        {
            AddChip(item, _researchContainer);
        }
        foreach(var item in designChipString)
        {
            AddChip(item, _designContainer);
        }
        foreach(var item in devChipString)
        {
            AddChip(item, _devContainer);
        }
    }

    private void ToggleKeywordTabButton(int category)
    {
        if(_activeTab != category)
        {
            // Debug.Log($"Category Changed From {activeTab} To {category}");
            _tabButtons[_activeTab].RemoveFromClassList("tab-button-selected");
            _tabButtons[category].AddToClassList("tab-button-selected");
            _activeTab = category;
            _bar.style.translate = new Translate(Length.Percent(132*(_activeTab-1)), 0);
            _chipsContainer.style.translate = new Translate(Length.Percent(-33*(_activeTab-1)), 0);
        }
    }

    private void UpdateTabButtonText()
    {
        if(_researchChipCount > 0)
        {
            _tabButtons[0].text = $"기획 <color=white>{_researchChipCount}</color>";
        }
        else
        {
            _tabButtons[0].text = "기획";
        }

        if(_designChipCount > 0)
        {
            _tabButtons[1].text = $"디자인 <color=white>{_designChipCount}</color>";
        }
        else
        {
            _tabButtons[1].text = "디자인";
        }

        if(_devChipCount > 0)
        {
            _tabButtons[2].text = $"데브 <color=white>{_devChipCount}</color>";
        }
        else
        {
            _tabButtons[2].text = "데브";
        }
    }

    private void AddChip(string text, VisualElement container)
    {
        var chip = new SelectableChip(text);
        container.Add(chip);
    }
}