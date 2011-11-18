using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckEngine.Interfaces
{
    /// <summary>
    /// An interface for that which should react to
    /// mouse clicks and mouse hovering.
    /// </summary>
    interface IMouseEvent3D
    {
        void OnMouseOver();
        void OnMouseDown();
        void OnMouseUp();
        void OnMouseOut();
    }
}
