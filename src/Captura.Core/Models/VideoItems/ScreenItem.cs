﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Screna;
using System;

namespace Captura.Models
{
    public class ScreenItem : NotifyPropertyChanged, IVideoItem
    {
        public Screen Screen { get; }

        readonly int _index;

        ScreenItem(int i)
        {
            Screen = Screen.AllScreens[i];

            _index = i;
        }

        public static int Count => Screen.AllScreens.Length;

        public Bitmap Capture(bool Cursor)
        {
            var rectangle = Screen.Bounds;

            var bmp = new Bitmap(rectangle.Width, rectangle.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rectangle.Location, Point.Empty, rectangle.Size, CopyPixelOperation.SourceCopy);

                if (Cursor)
                    MouseCursor.Draw(g, P => new Point(P.X - rectangle.X, P.Y - rectangle.Y));

                g.Flush();
            }

            return bmp;
        }

        public static IEnumerable<ScreenItem> Enumerate()
        {
            var n = Count;

            for (var i = 0; i < n; ++i)
                yield return new ScreenItem(i);
        }

        public string Name => Screen.DeviceName;

        public override string ToString() => Name;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = P => new Point(P.X - Screen.Bounds.X, P.Y - Screen.Bounds.Y);
            
            if (Settings.Instance.UseDeskDupl)
                return new DeskDuplImageProvider(_index, IncludeCursor);

            return new ScreenProvider(Screen, IncludeCursor);
        }
    }
}