﻿#pragma checksum "C:\Users\Multiply\Desktop\SilverlightLab\RealApplication01\RealApplication01\Views\BlacklistPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "577AADC2A2B10F3D3ECC135E0AAC996D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace RealApplication01.Views {
    
    
    public partial class BlacklistPage : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.DomainDataSource blackEventDomainDataSource;
        
        internal System.Windows.Controls.DataGrid blackEventDataGrid;
        
        internal System.Windows.Controls.DataGridTemplateColumn blackEventIDColumn;
        
        internal System.Windows.Controls.DataGridTemplateColumn dateColumn;
        
        internal System.Windows.Controls.DataGridTextColumn detailColumn;
        
        internal System.Windows.Controls.DataGridTemplateColumn outsiderIDColumn;
        
        internal System.Windows.Controls.DataGridTextColumn typeColumn;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/RealApplication01;component/Views/BlacklistPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.blackEventDomainDataSource = ((System.Windows.Controls.DomainDataSource)(this.FindName("blackEventDomainDataSource")));
            this.blackEventDataGrid = ((System.Windows.Controls.DataGrid)(this.FindName("blackEventDataGrid")));
            this.blackEventIDColumn = ((System.Windows.Controls.DataGridTemplateColumn)(this.FindName("blackEventIDColumn")));
            this.dateColumn = ((System.Windows.Controls.DataGridTemplateColumn)(this.FindName("dateColumn")));
            this.detailColumn = ((System.Windows.Controls.DataGridTextColumn)(this.FindName("detailColumn")));
            this.outsiderIDColumn = ((System.Windows.Controls.DataGridTemplateColumn)(this.FindName("outsiderIDColumn")));
            this.typeColumn = ((System.Windows.Controls.DataGridTextColumn)(this.FindName("typeColumn")));
        }
    }
}
