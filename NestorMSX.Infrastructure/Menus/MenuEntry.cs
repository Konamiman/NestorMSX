using System;
using System.Collections.Generic;
using System.Linq;

namespace Konamiman.NestorMSX.Menus
{
    /// <summary>
    /// Represents a menu entry to be displayed under the "Plugins" menu in the NestorMSX main window.
    /// </summary>
    /// <remarks>Use <see cref="PluginContext.SetMenuEntry"/> in the plugin constructor
    /// to register a menu entry.</remarks>
    public class MenuEntry
    {
        public event EventHandler EnableChanged;
        public event EventHandler CheckedChanged;
        public event EventHandler VisibleChanged;
        public event EventHandler TitleChanged;

        /// <summary>
        /// Creates a new menu entry that has an associated action
        /// </summary>
        /// <param name="title">Title to display in the menu entry</param>
        /// <param name="callback">Callback to execute when the menu entry is selected</param>
        public MenuEntry(string title, Action callback) : this(title, callback, null)
        {
        }

        /// <summary>
        /// Creates a new menu entry that has set of child entries
        /// </summary>
        /// <param name="title">Title to display in the menu entry</param>
        /// <param name="childEntries">Menu entries to show when the menu entry is selected</param>
        public MenuEntry(string title, IEnumerable<MenuEntry> childEntries) : this(title, null, childEntries)
        {
        }

        private MenuEntry(string title, Action callback, IEnumerable<MenuEntry> childEntries)
        {
            this.Title = title;

            if (callback == null && childEntries == null)
                throw new ArgumentException("Menu entries must have either an associated action or a collection of child entries");

            this.Callback = callback;
            this.ChildEntries = childEntries?.ToArray();

            this.IsEnabled = true;
            this.IsVisible = true;
        }

        /// <summary>
        /// Gets the callback to execute when the menu entry is selected,
        /// or null if the menu entry has child entries
        /// </summary>
        public Action Callback { get; }

        /// <summary>
        /// Gets the child entries to show when the menu entry is selected,
        /// or null if there is a registered callback
        /// </summary>
        public MenuEntry[] ChildEntries { get; }

        private string _Title;
        /// <summary>
        /// Gets or sets the title to display in the menu entry
        /// </summary>
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Menu entry titles can't be null or empty");

                if (value == _Title)
                    return;

                _Title = value;
                TitleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _IsEnabled;
        /// <summary>
        /// Gets or sets a value indicating whether the menu entry can be selected
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if(value == _IsEnabled)
                    return;

                _IsEnabled = value;
                EnableChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _IsChecked;
        /// <summary>
        /// Gets or sets a value indicating whether a check mark is displayed next to the menu entry
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                if (value == _IsChecked)
                    return;

                _IsChecked = value;
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _IsVisible;
        /// <summary>
        /// Gets or sets a value indicating whether the menu entry is currently visible
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }
            set
            {
                if (value == _IsVisible)
                    return;

                _IsVisible = value;
                VisibleChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
