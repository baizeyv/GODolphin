using System;
using System.Collections.Generic;
using Godot;

namespace GODolphin
{
    public static class GodotExtensions
    {
        #region CanvasItem

        public static void SetZIndex(this CanvasItem self, int zIndex)
        {
            self.ZIndex = zIndex;
        }

        public static void SetVisible(this CanvasItem self, bool visible)
        {
            self.Visible = visible;
        }

        public static void SetClip(
            this CanvasItem self,
            CanvasItem.ClipChildrenMode clipChildrenMode
        )
        {
            self.ClipChildren = clipChildrenMode;
        }

        public static void SetTop(this CanvasItem self, bool top)
        {
            self.TopLevel = top;
        }

        public static void SetBehindParent(this CanvasItem self, bool behindParent)
        {
            self.ShowBehindParent = behindParent;
        }

        public static void SetGroupColor(this CanvasItem self, Color groupColor)
        {
            self.Modulate = groupColor;
        }

        public static void SetGroupAlpha(this CanvasItem self, float alpha)
        {
            self.Modulate = self.Modulate with { A = alpha };
        }

        public static void SetSelfColor(this CanvasItem self, Color selfColor)
        {
            self.SelfModulate = selfColor;
        }

        public static void SetSelfAlpha(this CanvasItem self, float alpha)
        {
            self.SelfModulate = self.SelfModulate with { A = alpha };
        }

        #endregion

        #region Node2D

        /// <summary>
        /// * 设置2D位置
        /// </summary>
        /// <param name="self"></param>
        /// <param name="position"></param>
        public static void SetPosition(this Node2D self, Vector2 position)
        {
            self.Position = position;
        }

        /// <summary>
        /// * 设置2D位置
        /// </summary>
        /// <param name="self"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SetPosition(this Node2D self, float x, float y)
        {
            self.Position = new Vector2(x, y);
        }

        public static void ScaleBy(this Node2D self, float x, float y)
        {
            self.Scale = new Vector2(self.Scale.X + x, self.Scale.Y + y);
        }

        public static void ScaleByGlobal(this Node2D self, float x, float y)
        {
            self.GlobalScale = new Vector2(self.GlobalScale.X + x, self.GlobalScale.Y + y);
        }

        public static void ScaleBy(this Control self, float x, float y)
        {
            self.Scale = new Vector2(self.Scale.X + x, self.Scale.Y + y);
        }

        public static void RotateBy(this Node2D self, float amountInDegrees)
        {
            if (amountInDegrees != 0)
            {
                self.RotationDegrees += amountInDegrees;
            }
        }

        public static void RotateByGlobal(this Node2D self, float amountInDegrees)
        {
            if (amountInDegrees != 0)
            {
                self.GlobalRotationDegrees += amountInDegrees;
            }
        }

        public static void RotateBy(this Control self, float amountInDegrees)
        {
            if (amountInDegrees != 0)
            {
                self.RotationDegrees += amountInDegrees;
            }
        }

        public static void MoveBy(this Node2D self, float x, float y)
        {
            if (x != 0 || y != 0)
            {
                self.Position = new Vector2(self.Position.X + x, self.Position.Y + y);
            }
        }

        public static void MoveByGlobal(this Node2D self, float x, float y)
        {
            if (x != 0 || y != 0)
            {
                self.GlobalPosition = new Vector2(
                    self.GlobalPosition.X + x,
                    self.GlobalPosition.Y + y
                );
            }
        }

        public static void MoveBy(this Control self, float x, float y)
        {
            if (x != 0 || y != 0)
            {
                self.Position = new Vector2(self.Position.X + x, self.Position.Y + y);
            }
        }

        public static void MoveByGlobal(this Control self, float x, float y)
        {
            if (x != 0 || y != 0)
            {
                self.GlobalPosition = new Vector2(
                    self.GlobalPosition.X + x,
                    self.GlobalPosition.Y + y
                );
            }
        }

        /// <summary>
        /// * 设置2D位置
        /// </summary>
        /// <param name="self"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SetGlobalPosition(this Node2D self, float x, float y)
        {
            self.GlobalPosition = new Vector2(x, y);
        }

        /// <summary>
        /// * 设置2D位置的X坐标
        /// </summary>
        /// <param name="self"></param>
        /// <param name="x"></param>
        public static void SetX(this Node2D self, float x)
        {
            self.Position = self.Position with { X = x };
        }

        /// <summary>
        /// * 设置2D位置的Y坐标
        /// </summary>
        /// <param name="self"></param>
        /// <param name="y"></param>
        public static void SetY(this Node2D self, float y)
        {
            self.Position = self.Position with { Y = y };
        }

        /// <summary>
        /// * 设置旋转角度
        /// </summary>
        /// <param name="self"></param>
        /// <param name="angle"></param>
        public static void SetRotation(this Node2D self, float angle)
        {
            self.RotationDegrees = angle;
        }

        public static void SetGlobalScale(this Node2D self, float x, float y)
        {
            self.GlobalScale = self.GlobalPosition with { X = x, Y = y };
        }

        public static void SetScale(this Node2D self, float x, float y)
        {
            self.Scale = self.Scale with { X = x, Y = y };
        }

        public static void SetScale(this Control self, float x, float y)
        {
            self.Scale = self.Scale with { X = x, Y = y };
        }

        /// <summary>
        /// * 设置缩放比例
        /// </summary>
        /// <param name="self"></param>
        /// <param name="scale"></param>
        public static void SetScale(this Node2D self, float scale)
        {
            self.Scale = new Vector2(scale, scale);
        }

        /// <summary>
        /// * 设置X缩放比例
        /// </summary>
        /// <param name="self"></param>
        /// <param name="x"></param>
        public static void SetScaleX(this Node2D self, float x)
        {
            self.Scale = self.Scale with { X = x };
        }

        /// <summary>
        /// * 设置Y缩放比例
        /// </summary>
        /// <param name="self"></param>
        /// <param name="y"></param>
        public static void SetScaleY(this Node2D self, float y)
        {
            self.Scale = self.Scale with { Y = y };
        }

        /// <summary>
        /// * 设置斜切角度
        /// </summary>
        /// <param name="self"></param>
        /// <param name="angle"></param>
        public static void SetSkew(this Node2D self, float angle)
        {
            self.Skew = angle;
        }

        #endregion

        #region Control

        public static void SetSize(this Control self, float x, float y)
        {
            self.Size = new Vector2(x, y);
        }

        public static void SizeBy(this Control self, float width, float height)
        {
            if (width != 0 || height != 0)
            {
                self.Size = new Vector2(width + self.Size.X, height + self.Size.Y);
            }
        }

        public static void SetPosition(this Control self, Vector2 position)
        {
            self.Position = position;
        }

        public static void SetPosition(this Control self, float x, float y)
        {
            self.Position = self.Position with { X = x, Y = y };
        }

        public static void SetGlobalPosition(this Control self, float x, float y)
        {
            self.GlobalPosition = self.GlobalPosition with { X = x, Y = y };
        }

        public static void SetX(this Control self, float x)
        {
            self.Position = self.Position with { X = x };
        }

        public static void SetY(this Control self, float y)
        {
            self.Position = self.Position with { Y = y };
        }

        public static void SetRotation(this Control self, float angle)
        {
            self.RotationDegrees = angle;
        }

        public static void SetScale(this Control self, float scale)
        {
            self.Scale = new Vector2(scale, scale);
        }

        public static void SetScaleX(this Control self, float x)
        {
            self.Scale = self.Scale with { X = x };
        }

        public static void SetScaleY(this Control self, float y)
        {
            self.Scale = self.Scale with { Y = y };
        }

        public static void SetPivot(this Control self, Vector2 position)
        {
            self.PivotOffset = position;
        }

        public static void SetPivot(this Control self, float x, float y)
        {
            self.PivotOffset = self.Position with { X = x, Y = y };
        }

        public static void SetPivotX(this Control self, float x)
        {
            self.PivotOffset = self.Position with { X = x };
        }

        public static void SetPivotY(this Control self, float y)
        {
            self.PivotOffset = self.Position with { Y = y };
        }

        #endregion
    }
}
