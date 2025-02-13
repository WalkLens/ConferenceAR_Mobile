using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class ProfileCard : VisualElement
{
    private VisualElement _notification;
    private Label _nameLabel;
    private Label _jobLabel;
    private VisualElement _profilePhoto;
    private VisualElement _keywordsContainer;
    private VisualElement _interestsContainer;
    public VisualElement _wishButton;
    public VisualElement _wishButton_blank;
    private Button _meetButton;
    private int status; // 0: offline, 1: online, 2: away
    private VisualElement _card;
    public UserData profileData;
    private bool isSelected = false;
    private bool isInWish;
    private bool alwaysShowWish;

    public ProfileCard(UserData userData, bool isInWish=false, bool alwaysShowWish=false)
    {
        this.profileData = userData;
        this.isInWish = isInWish;
        this.alwaysShowWish = alwaysShowWish;
        // UXML 및 USS 불러오기
        var visualTree = Resources.Load<VisualTreeAsset>("UI/VisualTree/ProfileCard");
        visualTree.CloneTree(this);
        // styleSheets.Add(Resources.Load<StyleSheet>("Chip"));

        // 요소 참조 가져오기
        _notification = this.Q<VisualElement>("notification");
        _card = this.Q<VisualElement>("card");
        _card.RegisterCallback<ClickEvent>(evt => ToggleCard());
        // _card.RegisterCallback<FocusEvent>(evt => OnSelected());
        _card.RegisterCallback<BlurEvent>(evt => OnUnselected());


        _nameLabel = this.Q<Label>("name");
        _jobLabel = this.Q<Label>("job");
        _profilePhoto = this.Q<VisualElement>("photo");
        _meetButton = this.Q<Button>("meet-button");
        _meetButton.RegisterCallback<ClickEvent>(evt => {Meet(); evt.StopPropagation();});
        _wishButton = this.Q<VisualElement>("wish");
        _wishButton.RegisterCallback<ClickEvent>(evt => {RemoveWish(); evt.StopPropagation();});
        _wishButton_blank = this.Q<VisualElement>("wish-blank");
        _wishButton_blank.RegisterCallback<ClickEvent>(evt => {AddWish(); evt.StopPropagation();});

        if(alwaysShowWish)
        {
            if(isInWish)
            {
                _wishButton.style.visibility = Visibility.Visible;
            }
            else
            {
                _wishButton_blank.style.visibility = Visibility.Visible;
            }
        }
        
        _keywordsContainer = this.Q<VisualElement>("keywords-container");
        _interestsContainer = this.Q<VisualElement>("interests-container");

        // 텍스트 설정
        _nameLabel.text = userData.name;
        _jobLabel.text = userData.job;
        // Profilephoto URL 설정

        AddChip(userData.introduction_1, _keywordsContainer);
        AddChip(userData.introduction_2, _keywordsContainer);
        AddChip(userData.introduction_3, _keywordsContainer);

        AddChip(userData.interest_1, _interestsContainer);
        AddChip(userData.interest_2, _interestsContainer);
        AddChip(userData.interest_3, _interestsContainer);
    }

    public void SetStatus(int status) // 0: offline, 1: online, 2: away
    {
        this.status = status;

        // TODO status 이미지 변경
    }

    private void AddChip(string text, VisualElement container)
    {
        var chip = new SelectableChip(text);
        container.Add(chip);
    }

    private void ToggleCard()
    {
        if(isSelected)
        {
            _card.Blur();
        }
        else
        {
            OnSelected();
            isSelected = true;
        }
    }

    private void ShowHMD()
    {
        Debug.Log(this.profileData.pin); // AR에 프로필 띄우기
    }

    private void HideHMD()
    {
        Debug.Log(this.profileData.pin); // AR에서 프로필 숨기기기
    }

    private void Meet()
    {
        Debug.Log(this.profileData.pin); // 만나러 가기;
    }

    private void OnSelected()
    {
        _notification.style.display = DisplayStyle.Flex;
        _notification.style.opacity = 1;
        _meetButton.style.display = DisplayStyle.Flex;
        _meetButton.style.opacity = 1;
        _card.AddToClassList("card-active");
        ShowHMD();
        if(!alwaysShowWish)
        {
            ShowWishButton();
        }
    }

    private void OnUnselected()
    {
        _notification.style.opacity = 0;
        _notification.style.display = DisplayStyle.None;
        _meetButton.style.opacity = 0;
        _meetButton.style.display = DisplayStyle.None;
        _card.RemoveFromClassList("card-active");
        HideHMD();
        isSelected = false;
        if(!alwaysShowWish)
        {
            HideWishButton();
        }
    }

    private void AddWish()
    {
        DatabaseManager.Instance.addWish(this.profileData.pin);
        this.isInWish = true;
        ShowWishButton();
    }

    private void RemoveWish()
    {
        DatabaseManager.Instance.removeWish(this.profileData.pin);
        this.isInWish = false;
        ShowWishButton();
    }

    private void ShowWishButton()
    {
        if(isInWish)
        {
            _wishButton.style.visibility = Visibility.Visible;
            _wishButton_blank.style.visibility = Visibility.Hidden;
        }
        else
        {
            _wishButton.style.visibility = Visibility.Hidden;
            _wishButton_blank.style.visibility = Visibility.Visible;
        }
    }

    private void HideWishButton()
    {
        _wishButton.style.visibility = Visibility.Hidden;
        _wishButton_blank.style.visibility = Visibility.Hidden;
    }
}