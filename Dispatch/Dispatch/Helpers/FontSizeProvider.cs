using Dispatch.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dispatch.Helpers
{
    public class FontSizeProvider
    {
        private static FontSizeProvider instance;
        public static FontSizeProvider Instance
        {
            get
            {
                AssertIsInitialized();
                return instance;
            }
        }

        private static bool isInitialized = false;

        public static void Initialize(AppFontSize fontSize)
        {
            AssertIsNotInitialized();
            instance = new FontSizeProvider(fontSize);
            isInitialized = true;
        }

        private AppFontSize fontSize;
        public AppFontSize FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
                OnFontSizeChanged();
            }
        }

        private readonly List<WeakReference> listeners = new List<WeakReference>();

        private FontSizeProvider(AppFontSize fontSize)
        {
            this.fontSize = fontSize;
        }

        private void OnFontSizeChanged()
        {
            foreach (var lref in listeners.ToArray())
            {
                var listener = (FontSizeListener)lref.Target;
                if (listener == null)
                {
                    listeners.Remove(lref);
                }
                else
                {
                    listener.InformFontSizeChanged();
                }
            }
        }

        public void AddListener(FontSizeListener listener)
        {
            WeakReference lref = new WeakReference(listener, true);
            listeners.Add(lref);
        }

        public void RemoveListener(FontSizeListener listener)
        {
            foreach (var lref in listeners.ToArray())
            {
                if (lref.Target == listener)
                {
                    listeners.Remove(lref);
                }
            }
        }

        public double GetValue()
        {
            AssertIsInitialized();

            switch (fontSize)
            {
                case AppFontSize.Large:
                    return 1.3;
                case AppFontSize.Small:
                    return 0.85;
                case AppFontSize.Normal:
                default:
                    return 1.0;
            }
        }

        private static void AssertIsInitialized()
        {
            if (!isInitialized)
            {
                throw new Exception("FontSizeProvider is not initialized. Initialize it first.");
            }
        }

        private static void AssertIsNotInitialized()
        {
            if (isInitialized)
            {
                throw new Exception("FontSizeProvider is already initialized.");
            }
        }
    }
}
