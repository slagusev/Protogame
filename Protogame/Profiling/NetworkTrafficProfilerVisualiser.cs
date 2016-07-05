﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System;

namespace Protogame
{
    public class NetworkTrafficProfilerVisualiser : INetworkTrafficProfilerVisualiser
    {
        private readonly FontAsset _defaultFont;
        private readonly INetworkEngine _networkEngine;
        private readonly I2DRenderUtilities _renderUtilities;

        private NetworkSampler _sentSampler;
        private NetworkSampler _receivedSampler;

        public NetworkTrafficProfilerVisualiser(
            IAssetManagerProvider assetManagerProvider,
            INetworkEngine networkEngine,
            I2DRenderUtilities renderUtilities)
        {
            _defaultFont = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
            _networkEngine = networkEngine;
            _renderUtilities = renderUtilities;

            _sentSampler = new NetworkSampler(_renderUtilities, _defaultFont, "SENT");
            _receivedSampler = new NetworkSampler(_renderUtilities, _defaultFont, "RECV");
        }

        public int GetHeight(int backBufferHeight)
        {
            return _sentSampler.Height + _receivedSampler.Height;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, Rectangle rectangle)
        {
            _sentSampler.Sample(
                _networkEngine.GetSizeOfMessagesSentLastFrame(),
                _networkEngine.GetCountOfMessagesSentLastFrame(),
                rectangle.Width);

            _receivedSampler.Sample(
                _networkEngine.GetSizeOfMessagesReceivedLastFrame(),
                _networkEngine.GetCountOfMessagesReceivedLastFrame(),
                rectangle.Width);
            
            _sentSampler.Render(
                renderContext,
                new Rectangle(
                    rectangle.X,
                    rectangle.Y,
                    rectangle.Width,
                    _sentSampler.Height));

            _receivedSampler.Render(
                renderContext,
                new Rectangle(
                    rectangle.X,
                    rectangle.Y + _sentSampler.Height,
                    rectangle.Width,
                    _receivedSampler.Height));
        }

        private class NetworkSampler
        {
            private readonly I2DRenderUtilities _renderUtilities;
            private readonly FontAsset _defaultFont;

            private readonly List<int> _bytesOverTime;
            private readonly List<int> _countsOverTime;

            public string Type { get; set; }

            public int Height
            {
                get { return 100; }
            }

            public NetworkSampler(I2DRenderUtilities renderUtilities, FontAsset defaultFont, string type)
            {
                _renderUtilities = renderUtilities;
                _defaultFont = defaultFont;
                Type = type;

                _bytesOverTime = new List<int>();
                _countsOverTime = new List<int>();
            }

            public void Sample(Dictionary<Type, int> bytes, Dictionary<Type, int> counts, int maxHistory)
            {
                _bytesOverTime.Add(bytes.Select(x => x.Value).DefaultIfEmpty(0).Sum());
                _countsOverTime.Add(counts.Select(x => x.Value).DefaultIfEmpty(0).Sum());
                
                while (_bytesOverTime.Count > maxHistory)
                {
                    _bytesOverTime.RemoveAt(0);
                }
                while (_countsOverTime.Count > maxHistory)
                {
                    _countsOverTime.RemoveAt(0);
                }
            }

            public void Render(IRenderContext renderContext, Rectangle rectangle)
            {
                var maxBytes = _bytesOverTime.DefaultIfEmpty(0).Max();
                var maxCounts = _countsOverTime.DefaultIfEmpty(0).Max();
                
                RenderHeader(renderContext, rectangle, Type, 0, _bytesOverTime.DefaultIfEmpty(0).Last(), maxBytes, "b");
                RenderHeader(renderContext, rectangle, string.Empty, 20, _countsOverTime.DefaultIfEmpty(0).Last(), maxCounts, "#");

                if (maxBytes > 0)
                {
                    var a = 0;
                    for (var i = _bytesOverTime.Count - 1; i >= 0; i--)
                    {
                        _renderUtilities.RenderLine(
                            renderContext,
                            new Vector2(rectangle.X + rectangle.Width - a - 1, rectangle.Y + 100),
                            new Vector2(rectangle.X + rectangle.Width - a - 1,
                                rectangle.Y + 100 - (int)((_bytesOverTime[i] / (float)maxBytes) * 60)),
                            Color.Cyan);
                        a++;
                    }
                }
            }

            private void RenderHeader(IRenderContext renderContext, Rectangle rectangle, string title, int n, int lastBytes, int maxBytes, string s)
            {
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(rectangle.X + 2, rectangle.Y + 2 + n),
                    title,
                    _defaultFont);

                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(rectangle.X + 2 + 60, rectangle.Y + 2 + n),
                    "last",
                    _defaultFont);

                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(rectangle.X + 2 + 160, rectangle.Y + 2 + n),
                    lastBytes + s,
                    _defaultFont,
                    HorizontalAlignment.Right,
                    textColor: lastBytes >= 512 ? Color.Red : Color.White);

                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(rectangle.X + 2 + 180, rectangle.Y + 2 + n),
                    "max",
                    _defaultFont);

                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(rectangle.X + 2 + 280, rectangle.Y + 2 + n),
                    maxBytes + s,
                    _defaultFont,
                    HorizontalAlignment.Right,
                    textColor: maxBytes >= 512 ? Color.Red : Color.White);
            }
        }
    }
}