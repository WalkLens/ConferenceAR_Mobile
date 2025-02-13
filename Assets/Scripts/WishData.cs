using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WishData
{
    public int user_id;
    public int target_id;
}

[System.Serializable]
public class WishList
{
    public List<WishData> wishes;
}