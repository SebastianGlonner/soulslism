namespace EfesusProfiler;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.Control;

    class ControlUtil
    {
        static public void ApplyDefaults(Control control)
        {
            control.ClipContents = true;

            //control.OffsetLeft = 0;
            //control.OffsetTop = 0;
            //control.OffsetRight = 0;
            //control.OffsetBottom = 0;
            control.AnchorLeft = 0;
            control.AnchorTop = 0;
            control.AnchorRight = 0;
            control.AnchorBottom = 0;
            control.SizeFlagsHorizontal = SizeFlags.Fill;
            control.SizeFlagsVertical = SizeFlags.Fill;
        }
        static public void ApplyAnchorFull(Control control)
        {
            ApplyDefaults(control);
            control.AnchorRight = 1;
            control.AnchorBottom = 1;
        }
        static public void ApplyFullExpands(Control control)
        {
            ApplyDefaults(control);
            control.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            control.SizeFlagsVertical = SizeFlags.ExpandFill;
        }

        static public T FindNode<T>(SceneTree tree) where T : Node
        {
            T node = tree.Root.FindChild(typeof(T).Name, true, false) as T;
            if (node == null)
            {
                throw new Exception("Could not find node with name: " + typeof(T).Name);
            }
            return node;
        }

        static public Label NewAttachedLabel(string name, Control parent)
        {
            Label newLabel = new Label();
            newLabel.Text = name;
            newLabel.SizeFlagsVertical = SizeFlags.ExpandFill;
            parent.AddChild(newLabel);
            return newLabel;
        }
    }
