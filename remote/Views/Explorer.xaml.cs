﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using IoC;
using remote.Annotations;
using Path = System.IO.Path;
using Timer = System.Windows.Forms.Timer;

namespace remote
{
    /// <summary>
    /// Interaction logic for Explorer.xaml
    /// </summary>
    public partial class Explorer : INotifyPropertyChanged, IExplorer
    {
        private string _currentPath;
        private ObservableCollection<string> _files = new ObservableCollection<string>();
        private int _selectedIndex;
        private string _currentTime;
        public IDispatcher MyDispatcher { get { return IocKernel.GetInstance<IDispatcher>(); } }
        private Timer timer;

        public Explorer()
        {
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();
            
            InitializeComponent();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToLongTimeString();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            timer.Tick -= Timer_Tick;
            base.OnClosing(e);
        }


        private IActions Actions
        { get { return IoC.IocKernel.GetInstance<IActions>(); } }

        public ObservableCollection<string> Files
        {
            get { return _files; }
            set
            {
                if (Equals(value, _files)) return;
                _files = value;
                OnPropertyChanged();
            }
        }

        public int SelectedIndex
        {
       get {
            return _selectedIndex;
        }
            set
            {
                if (value == _selectedIndex) return;
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        

        public string CurrentTime
        {
            get { return _currentTime; }
            set
            {
                if (value == _currentTime) return;
                _currentTime = value;
                OnPropertyChanged();
            }
        }

        public string CurrentPath
        {
            get { return _currentPath; }
            set
            {
                if (value == _currentPath) return;
                _currentPath = value;
                OnPropertyChanged();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        

        private void ScrollIntoView(object sender, SelectionChangedEventArgs e)
        {
            if (Files.Count > _selectedIndex && _selectedIndex > -1)
                ListView.ScrollIntoView(Files[_selectedIndex]);
        }

        private void ListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Actions.OkButton();
        }
    }
}