﻿using UnityEngine;
using System.Collections.Generic;
using Grid;

namespace Helper
{
    public class GridHelper : MonoBehaviour
    {
        public static readonly double SQUAREROOT3 = 1.73205080757;

        public static Vector3 PointToVector3(Point point)
        {
            return new Vector3((float)point.x, (float)point.y);
        }

        public static Vector2 PointToVector2(Point point)
        {
            return new Vector2((float)point.x, (float)point.y);
        }

        public static Point Vector3ToPoint(Vector3 vector)
        {
            return new Point(vector.x, vector.y);
        }

        public static Point Vector2ToPoint(Vector2 vector)
        {
            return new Point(vector.x, vector.y);
        }

        public static Hex Vector2ToHex(Vector2 pixel, Layout layout)
        {
            Point pixelPoint = GridHelper.Vector2ToPoint(pixel);
            Hex pixelHex = FractionalHex.HexRound(Layout.PixelToHex(layout, pixelPoint));

            return pixelHex;
        }

        public static Vector2 HexToVector2(Hex hex, Layout layout)
        {
             return PointToVector2(Layout.HexToPixel(layout, hex));
        }

        public static float AccurateDistance(Hex a, Hex b, Layout layout)
        {
            Vector2 pointA = HexToVector2(a, layout);
            Vector2 pointB = HexToVector2(b, layout);

            return Vector2.SqrMagnitude(pointB - pointA);
        }

        public static Vector2 CopyVector2(Vector2 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static List<Hex> GetHexOnVerticalLine(Vector2 top, Vector2 bottom, RectangleMap map)
        {
            List<Hex> hexList = new List<Hex>();
            
            Hex hexTop = FractionalHex.HexRound(Layout.PixelToHex(map.MapLayout, Vector2ToPoint(top)));
            Hex hexBottom = FractionalHex.HexRound(Layout.PixelToHex(map.MapLayout, Vector2ToPoint(bottom)));

            hexList.Add(hexTop);
            hexList.Add(hexBottom);

            //starting from the top and getting all the hexes till the bottom
            Hex currentHex = hexTop;
            Vector2 currentPoint = top;
            double nudge = 0.0001;
            while (!currentHex.Equals(hexBottom))
            {
                //get bottom point
                Point bottomCorner = Layout.HexCornerOffset(map.MapLayout, (int)Layout.PointyCorners.DOWN);
                Point center = Layout.HexToPixel(map.MapLayout, currentHex);
                currentPoint.y = (float)(bottomCorner.y + center.y - nudge);
                if (currentPoint.y < bottom.y)
                {
                    break;
                }
                currentHex = FractionalHex.HexRound(Layout.PixelToHex(map.MapLayout, Vector2ToPoint(currentPoint)));
                hexList.Add(currentHex);
            }

            return hexList;
        }

        public static List<Hex> GetHexOnHorizontalLine(Vector2 right, Vector2 left, RectangleMap map)
        {
            List<Hex> hexList = new List<Hex>();

            Hex hexLeft = FractionalHex.HexRound(Layout.PixelToHex(map.MapLayout, Vector2ToPoint(left)));
            Hex hexRight = FractionalHex.HexRound(Layout.PixelToHex(map.MapLayout, Vector2ToPoint(right)));

            hexList.Add(hexRight);
            hexList.Add(hexLeft);

            //starting from the top and getting all the hexes till the bottom
            Hex currentHex = hexRight;
            Vector2 currentPoint = right;
            double nudge = 0.0001;
            while (!currentHex.Equals(hexLeft))
            {
                //get bottom point
                Point leastXCorner = Layout.HexCornerOffset(map.MapLayout, (int)Layout.PointyCorners.LEFTUP);
                Point center = Layout.HexToPixel(map.MapLayout, currentHex);
                currentPoint.x = (float)(leastXCorner.x + center.x - nudge);
                if (currentPoint.x < left.x)
                {
                    break;
                }
                currentHex = FractionalHex.HexRound(Layout.PixelToHex(map.MapLayout, Vector2ToPoint(currentPoint)));
                hexList.Add(currentHex);
            }

            return hexList;
        }

        public static HashSet<Hex> BoxToHexList(Vector2 boxCenter, Vector2 boxDimensions, RectangleMap map)
        {
            HashSet<Hex> hexList = new HashSet<Hex>();

            Vector2 boxTopLeft = new Vector2((boxCenter.x - boxDimensions.x / 2), (boxCenter.y + boxDimensions.y / 2));
            Vector2 boxBottomLeft = CopyVector2(boxCenter - (boxDimensions / 2));
            Vector2 boxBottomRight = new Vector2((boxCenter.x + boxDimensions.x / 2), (boxCenter.y - boxDimensions.y / 2));
            Vector2 boxTopRight = CopyVector2(boxCenter + (boxDimensions / 2));

            hexList.UnionWith(GetHexOnVerticalLine(boxTopLeft, boxBottomLeft, map));
            hexList.UnionWith(GetHexOnVerticalLine(boxTopRight, boxBottomRight, map));

            hexList.UnionWith(GetHexOnHorizontalLine(boxTopRight, boxTopLeft, map));
            hexList.UnionWith(GetHexOnHorizontalLine(boxBottomRight, boxBottomLeft, map));

            return hexList;
        }
    }
}