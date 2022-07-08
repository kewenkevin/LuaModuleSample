namespace ND.UI.Core
{
    public enum GearTypeState : int
    {
        // UIExpansion
        Controller = 0,
        Transition = 1,

        // GameObject
        Active = 2,

        // RectTransform
        Position = 3,
        SizeDelta = 4,
        Rotation = 5,
        Scale = 6,

        // Text
        TextStr = 7,
        TextColor = 8,


        // Image
        ImageSprite = 9,
        ImageColor = 10,
        
        RawImageColor = 11,
        ImageMaterial = 12,
        
        //new Feature
        OverallAlpha = 13,
        TextFontStyle = 14,
        TextColorStyle = 15,
        PercentPosition = 16,
        
        
        LocalizationKey = 9999,
        //rating
        RatingCurrent = 17,
        RatingTotal = 18,
    }
}