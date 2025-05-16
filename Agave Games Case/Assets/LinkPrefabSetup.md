# Link Object Prefab Setup Guide

This guide explains how to create and configure the link object prefab for visual connections between matched tiles.

## 1. Create the Link Prefab

1. In the Unity editor, right-click in the Project window and select **Create > UI > Image**.
2. Name the new GameObject "LinkObjectPrefab".
3. Set its RectTransform settings:
   - Anchors: Middle-Center
   - Pivot: (0.5, 0.5)
   - Width: 50
   - Height: 10
   - Scale: (1, 1, 1)

## 2. Add the LinkObject Component

1. Select the LinkObjectPrefab in the Project window.
2. Click **Add Component** in the Inspector.
3. Search for and add the **LinkObject** script.
4. Configure the LinkObject component:
   - **Rect Transform**: (Assign automatically)
   - **Image**: (Assign automatically)
   - **Link Color**: Choose a color for the link (white or any color that stands out)
   - **Horizontal Width**: 50
   - **Horizontal Height**: 10
   - **Vertical Width**: 10
   - **Vertical Height**: 50
   - **Animation Duration**: 0.3
   - **Appear Ease**: OutBack
   - **Disappear Ease**: InBack

## 3. Configure the Image Component

1. Set the **Color** property to white (or match the Link Color above).
2. For better visuals, you can use a sprite with soft edges or create a rounded rectangle.

## 4. Make it a Prefab

1. Drag the GameObject from the Hierarchy to the Project window to create a prefab.
2. Delete the instance from the Hierarchy (it's now saved as a prefab).

## 5. Assign to Board Script

1. Select the GameObject with the **Board** script in the Hierarchy.
2. In the Inspector, find the **Link Object Prefab** field.
3. Drag the LinkObjectPrefab from the Project window into this field.

## Additional Tips

- Make sure the LinkObject has a larger sorting layer than the tiles to appear above them.
- You can adjust the animation duration and easing functions to match your game's feel.
- The link will automatically rotate to connect tiles horizontally or vertically.
- If you want to change the appearance, modify the Image component's sprite or color.
