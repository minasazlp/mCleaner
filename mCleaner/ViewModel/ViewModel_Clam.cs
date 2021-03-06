﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using mCleaner.Logics.Clam;
using mCleaner.Logics.Enumerations;
using mCleaner.Model;
using Microsoft.Practices.ServiceLocation;

namespace mCleaner.ViewModel
{
    public class ViewModel_Clam : ViewModelBase
    {
        #region properties

        public ViewModel_CleanerML CleanerML
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewModel_CleanerML>();
            }
        }

        private bool _showClamWinVirusUpdateWindow = false;
        public bool ShowClamWinVirusUpdateWindow
        {
            get { return _showClamWinVirusUpdateWindow; }
            set
            {
                if (_showClamWinVirusUpdateWindow != value)
                {
                    _showClamWinVirusUpdateWindow = value;
                    base.RaisePropertyChanged("ShowClamWinVirusUpdateWindow");
                }
            }
        }

        private bool _showClamWinVirusScanner = false;
        public bool ShowClamWinVirusScanner
        {
            get { return _showClamWinVirusScanner; }
            set
            {
                if (_showClamWinVirusScanner != value)
                {
                    _showClamWinVirusScanner = value;
                    base.RaisePropertyChanged("ShowClamWinVirusScanner");
                }
            }
        }

        private string _virusDefUpdateLog = string.Empty;
        public string VirusDefUpdateLog
        {
            get { return _virusDefUpdateLog; }
            set
            {
                if (_virusDefUpdateLog != value)
                {
                    _virusDefUpdateLog = value;
                    base.RaisePropertyChanged("VirusDefUpdateLog");
                }
            }
        }

        private string _virusScanUpdateLog = string.Empty;
        public string VirusScanUpdateLog
        {
            get { return _virusScanUpdateLog; }
            set
            {
                if (_virusScanUpdateLog != value)
                {
                    _virusScanUpdateLog = value;
                    base.RaisePropertyChanged("VirusScanUpdateLog");
                }
            }
        }

        private string _windowTitle = "Update Virus Definition Database";
        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                if (_windowTitle != value)
                {
                    _windowTitle = value;
                    base.RaisePropertyChanged("WindowTitle");
                }
            }
        }

        private string _progressText = string.Empty;
        public string ProgressText
        {
            get { return _progressText; }
            set
            {
                if (_progressText != value)
                {
                    _progressText = value;
                    base.RaisePropertyChanged("ProgressText");
                }
            }
        }

         private string _infoTipText = string.Empty;
        public string InfoTipText
        {
            get { return _infoTipText; }
            set
            {
                if (_infoTipText != value)
                {
                    _infoTipText = value;
                    base.RaisePropertyChanged("InfoTipText");
                }
            }
        }


        private int _infectedFilesCount = 0;
        public int InfectedFilesCount
        {
            get { return _infectedFilesCount; }
            set
            {
                if (_infectedFilesCount != value)
                {
                    _infectedFilesCount = value;
                    base.RaisePropertyChanged("InfectedFilesCount");
                }
            }
        }

        private bool _enableCleanNowButton = false;
        public bool EnableCleanNowButton
        {
            get { return _enableCleanNowButton; }
            set
            {
                if (_enableCleanNowButton != value)
                {
                    _enableCleanNowButton = value;
                    base.RaisePropertyChanged("EnableCleanNowButton");
                }
            }
        }

        private bool _enableCancelButton = false;
        public bool EnableCancelButton
        {
            get { return _enableCancelButton; }
            set
            {
                if (_enableCancelButton != value)
                {
                    _enableCancelButton = value;
                    base.RaisePropertyChanged("EnableCancelButton");
                }
            }
        }

        private bool _enableCloseButton = false;
        public bool EnableCloseButton
        {
            get { return _enableCloseButton; }
            set
            {
                if (_enableCloseButton != value)
                {
                    _enableCloseButton = value;
                    base.RaisePropertyChanged("EnableCloseButton");
                }
            }
        }

        private bool _progressIsIndeterminate = false;
        public bool ProgressIsIndeterminate
        {
            get { return _progressIsIndeterminate; }
            set
            {
                if (_progressIsIndeterminate != value)
                {
                    _progressIsIndeterminate = value;
                    base.RaisePropertyChanged("ProgressIsIndeterminate");
                }
            }
        }
        #endregion

        #region ctor
        public ViewModel_Clam()
        {
            if (base.IsInDesignMode)
            {
                this.ShowClamWinVirusUpdateWindow = false;
                this.VirusDefUpdateLog = "Log goes here";
                this.ProgressText = "Progress Text Here";
            }
            else
            {
                Command_UpdateVirusDefinition = new RelayCommand(Command_UpdateVirusDefinition_Click);
                Command_ScanForVirus = new RelayCommand(Command_ScanForVirus_Click);
                Command_CancelUpdate = new RelayCommand(Command_CancelUpdate_Click);
                Command_CloseWindow = new RelayCommand(Command_CloseWindow_Click);
                Command_ScanMemory = new RelayCommand(Command_ScanMemory_Click);
                Command_CleanNow = new RelayCommand(Command_CleanNow_Click);

                this.EnableCancelButton = true;
                this.EnableCleanNowButton = true;
                this.EnableCloseButton = true;
            }
        }
        #endregion

        #region commands
        public ICommand Command_UpdateVirusDefinition { get; internal set; }
        public ICommand Command_ScanForVirus { get; internal set; }
        public ICommand Command_CancelUpdate { get; internal set; }
        public ICommand Command_CloseWindow { get; internal set; }
        public ICommand Command_ScanMemory { get; internal set; }
        public ICommand Command_CleanNow { get; internal set; }
        #endregion

        #region command methods
        public void Command_UpdateVirusDefinition_Click()
        {
            this.ProgressIsIndeterminate = true;
            this.ShowClamWinVirusUpdateWindow = true;
            CommandLogic_Clam.I.isUpdate = true;
            CommandLogic_Clam.I.LaunchUpdater();
        }

        public void Command_ScanForVirus_Click()
        {
            //MessageBox.Show("Scheduled for Sprint 3");
            //this.ShowClamWinVirusUpdateWindow = true;;

            this.InfoTipText = "This will Scan PC for Virus.";
            this.ProgressIsIndeterminate = true;
            CommandLogic_Clam.I.isUpdate = false;
            CommandLogic_Clam.I.blnIsCancel = false;
            CommandLogic_Clam.I.LaunchCleaner();
        }

        public void Command_CancelUpdate_Click()
        {
            CommandLogic_Clam.I.CancelUpdate();
        }

        public void Command_CloseWindow_Click()
        {
            CommandLogic_Clam.I.CancelUpdate();
            CleanerML.Run = false;
            CleanerML.ShowCleanerDescription = false;
            CleanerML.ShowFrontPage = true;
            CleanerML.btnPreviewCleanEnable = CleanerML.btnCleanNowPreviousState;
            CleanerML.btnCleaningOptionsEnable = true;
            this.ShowClamWinVirusUpdateWindow = false;
            this.ShowClamWinVirusScanner = false;
        }

        public void Command_ScanMemory_Click()
        {
            //this.ShowClamWinVirusUpdateWindow = true;
            this.InfoTipText = "This will Scan Memory for Virus.";
            this.ProgressIsIndeterminate = true;
            CleanerML.Run = false;
            CleanerML.ShowCleanerDescription = false;
            CleanerML.btnCleanNowPreviousState = CleanerML.btnPreviewCleanEnable;
            CleanerML.btnPreviewCleanEnable = false;
            CleanerML.btnCleaningOptionsEnable = false;
            CleanerML.ShowFrontPage = false;
            this.ShowClamWinVirusScanner = true;
            CommandLogic_Clam.I.blnIsCancel = false;
            CommandLogic_Clam.I.isUpdate = false;
            CommandLogic_Clam.I.LaunchScanner(SEARCH.clamscan_memory, string.Empty, true);
        }

        public void Command_CleanNow_Click()
        {
            CommandLogic_Clam.I.LaunchCleaner(true);
        }
        #endregion
    }
}
