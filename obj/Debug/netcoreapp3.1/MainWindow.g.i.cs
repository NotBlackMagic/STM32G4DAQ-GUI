﻿#pragma checksum "..\..\..\MainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "932DAC1C0B5C1544FAB12C4718BB251384EA4D93"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LiveCharts.Wpf;
using STM32G4DAQ;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace STM32G4DAQ {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 29 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox portNamesComboBox;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox baudRatesComboBox;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button serialButton;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox currentSourceAComboBox;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox currentOutAComboBox;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox currentSourceBComboBox;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox currentOutBComboBox;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAMode;
        
        #line default
        #line hidden
        
        
        #line 131 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInASamplerate;
        
        #line default
        #line hidden
        
        
        #line 148 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAScale;
        
        #line default
        #line hidden
        
        
        #line 160 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel1Enabled;
        
        #line default
        #line hidden
        
        
        #line 167 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel1RateDiv;
        
        #line default
        #line hidden
        
        
        #line 180 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel1Resolution;
        
        #line default
        #line hidden
        
        
        #line 194 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel1Range;
        
        #line default
        #line hidden
        
        
        #line 215 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel2Enabled;
        
        #line default
        #line hidden
        
        
        #line 222 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel2RateDiv;
        
        #line default
        #line hidden
        
        
        #line 235 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel2Resolution;
        
        #line default
        #line hidden
        
        
        #line 249 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel2Gain;
        
        #line default
        #line hidden
        
        
        #line 270 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel3Enabled;
        
        #line default
        #line hidden
        
        
        #line 277 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel3RateDiv;
        
        #line default
        #line hidden
        
        
        #line 290 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel3Resolution;
        
        #line default
        #line hidden
        
        
        #line 304 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel3Gain;
        
        #line default
        #line hidden
        
        
        #line 325 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel4Enabled;
        
        #line default
        #line hidden
        
        
        #line 332 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel4RateDiv;
        
        #line default
        #line hidden
        
        
        #line 345 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel4Resolution;
        
        #line default
        #line hidden
        
        
        #line 359 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogInAChannel4Gain;
        
        #line default
        #line hidden
        
        
        #line 391 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox analogOutAChannel1Mode;
        
        #line default
        #line hidden
        
        
        #line 402 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox analogOutAChannel1Offset;
        
        #line default
        #line hidden
        
        
        #line 406 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox analogOutAChannel1Freq;
        
        #line default
        #line hidden
        
        
        #line 410 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox analogOutAChannel1Amp;
        
        #line default
        #line hidden
        
        
        #line 414 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox analogOutAChannel1DC;
        
        #line default
        #line hidden
        
        
        #line 425 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label rxUSBDatarateLabel;
        
        #line default
        #line hidden
        
        
        #line 429 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label txUSBDatarateLabel;
        
        #line default
        #line hidden
        
        
        #line 433 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label rxUSBDroppedLabel;
        
        #line default
        #line hidden
        
        
        #line 438 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label analogInACH1kspsLabel;
        
        #line default
        #line hidden
        
        
        #line 442 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label analogInACH2kspsLabel;
        
        #line default
        #line hidden
        
        
        #line 446 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label analogInACH3kspsLabel;
        
        #line default
        #line hidden
        
        
        #line 450 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label analogInACH4kspsLabel;
        
        #line default
        #line hidden
        
        
        #line 455 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LiveCharts.Wpf.CartesianChart analogInChart;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.1.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/STM32G4DAQ;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.1.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.portNamesComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.baudRatesComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.serialButton = ((System.Windows.Controls.Button)(target));
            
            #line 39 "..\..\..\MainWindow.xaml"
            this.serialButton.Click += new System.Windows.RoutedEventHandler(this.SerialButtonClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.currentSourceAComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 48 "..\..\..\MainWindow.xaml"
            this.currentSourceAComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.CurrentOutputAChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.currentOutAComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 56 "..\..\..\MainWindow.xaml"
            this.currentOutAComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.CurrentOutputAChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.currentSourceBComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 78 "..\..\..\MainWindow.xaml"
            this.currentSourceBComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.CurrentOutputBChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.currentOutBComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 86 "..\..\..\MainWindow.xaml"
            this.currentOutBComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.CurrentOutputBChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.analogInAMode = ((System.Windows.Controls.ComboBox)(target));
            
            #line 122 "..\..\..\MainWindow.xaml"
            this.analogInAMode.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.analogInASamplerate = ((System.Windows.Controls.ComboBox)(target));
            
            #line 131 "..\..\..\MainWindow.xaml"
            this.analogInASamplerate.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            this.analogInAScale = ((System.Windows.Controls.ComboBox)(target));
            
            #line 148 "..\..\..\MainWindow.xaml"
            this.analogInAScale.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChanged);
            
            #line default
            #line hidden
            return;
            case 11:
            this.analogInAChannel1Enabled = ((System.Windows.Controls.ComboBox)(target));
            
            #line 160 "..\..\..\MainWindow.xaml"
            this.analogInAChannel1Enabled.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel1Changed);
            
            #line default
            #line hidden
            return;
            case 12:
            this.analogInAChannel1RateDiv = ((System.Windows.Controls.ComboBox)(target));
            
            #line 167 "..\..\..\MainWindow.xaml"
            this.analogInAChannel1RateDiv.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel1Changed);
            
            #line default
            #line hidden
            return;
            case 13:
            this.analogInAChannel1Resolution = ((System.Windows.Controls.ComboBox)(target));
            
            #line 180 "..\..\..\MainWindow.xaml"
            this.analogInAChannel1Resolution.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel1Changed);
            
            #line default
            #line hidden
            return;
            case 14:
            this.analogInAChannel1Range = ((System.Windows.Controls.ComboBox)(target));
            
            #line 194 "..\..\..\MainWindow.xaml"
            this.analogInAChannel1Range.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel1Changed);
            
            #line default
            #line hidden
            return;
            case 15:
            this.analogInAChannel2Enabled = ((System.Windows.Controls.ComboBox)(target));
            
            #line 215 "..\..\..\MainWindow.xaml"
            this.analogInAChannel2Enabled.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel2Changed);
            
            #line default
            #line hidden
            return;
            case 16:
            this.analogInAChannel2RateDiv = ((System.Windows.Controls.ComboBox)(target));
            
            #line 222 "..\..\..\MainWindow.xaml"
            this.analogInAChannel2RateDiv.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel2Changed);
            
            #line default
            #line hidden
            return;
            case 17:
            this.analogInAChannel2Resolution = ((System.Windows.Controls.ComboBox)(target));
            
            #line 235 "..\..\..\MainWindow.xaml"
            this.analogInAChannel2Resolution.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel2Changed);
            
            #line default
            #line hidden
            return;
            case 18:
            this.analogInAChannel2Gain = ((System.Windows.Controls.ComboBox)(target));
            
            #line 249 "..\..\..\MainWindow.xaml"
            this.analogInAChannel2Gain.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel2Changed);
            
            #line default
            #line hidden
            return;
            case 19:
            this.analogInAChannel3Enabled = ((System.Windows.Controls.ComboBox)(target));
            
            #line 270 "..\..\..\MainWindow.xaml"
            this.analogInAChannel3Enabled.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel3Changed);
            
            #line default
            #line hidden
            return;
            case 20:
            this.analogInAChannel3RateDiv = ((System.Windows.Controls.ComboBox)(target));
            
            #line 277 "..\..\..\MainWindow.xaml"
            this.analogInAChannel3RateDiv.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel3Changed);
            
            #line default
            #line hidden
            return;
            case 21:
            this.analogInAChannel3Resolution = ((System.Windows.Controls.ComboBox)(target));
            
            #line 290 "..\..\..\MainWindow.xaml"
            this.analogInAChannel3Resolution.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel3Changed);
            
            #line default
            #line hidden
            return;
            case 22:
            this.analogInAChannel3Gain = ((System.Windows.Controls.ComboBox)(target));
            
            #line 304 "..\..\..\MainWindow.xaml"
            this.analogInAChannel3Gain.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel3Changed);
            
            #line default
            #line hidden
            return;
            case 23:
            this.analogInAChannel4Enabled = ((System.Windows.Controls.ComboBox)(target));
            
            #line 325 "..\..\..\MainWindow.xaml"
            this.analogInAChannel4Enabled.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel4Changed);
            
            #line default
            #line hidden
            return;
            case 24:
            this.analogInAChannel4RateDiv = ((System.Windows.Controls.ComboBox)(target));
            
            #line 332 "..\..\..\MainWindow.xaml"
            this.analogInAChannel4RateDiv.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel4Changed);
            
            #line default
            #line hidden
            return;
            case 25:
            this.analogInAChannel4Resolution = ((System.Windows.Controls.ComboBox)(target));
            
            #line 345 "..\..\..\MainWindow.xaml"
            this.analogInAChannel4Resolution.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel4Changed);
            
            #line default
            #line hidden
            return;
            case 26:
            this.analogInAChannel4Gain = ((System.Windows.Controls.ComboBox)(target));
            
            #line 359 "..\..\..\MainWindow.xaml"
            this.analogInAChannel4Gain.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogInAChannel4Changed);
            
            #line default
            #line hidden
            return;
            case 27:
            this.analogOutAChannel1Mode = ((System.Windows.Controls.ComboBox)(target));
            
            #line 391 "..\..\..\MainWindow.xaml"
            this.analogOutAChannel1Mode.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AnalogOutAChannel1Changed);
            
            #line default
            #line hidden
            return;
            case 28:
            this.analogOutAChannel1Offset = ((System.Windows.Controls.TextBox)(target));
            
            #line 402 "..\..\..\MainWindow.xaml"
            this.analogOutAChannel1Offset.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.AnalogOutAChannel1Changed);
            
            #line default
            #line hidden
            
            #line 402 "..\..\..\MainWindow.xaml"
            this.analogOutAChannel1Offset.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.AnalogOutOffsetValidation);
            
            #line default
            #line hidden
            return;
            case 29:
            this.analogOutAChannel1Freq = ((System.Windows.Controls.TextBox)(target));
            
            #line 406 "..\..\..\MainWindow.xaml"
            this.analogOutAChannel1Freq.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.AnalogOutAChannel1Changed);
            
            #line default
            #line hidden
            
            #line 406 "..\..\..\MainWindow.xaml"
            this.analogOutAChannel1Freq.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.AnalogOutFrequencyValidation);
            
            #line default
            #line hidden
            return;
            case 30:
            this.analogOutAChannel1Amp = ((System.Windows.Controls.TextBox)(target));
            
            #line 410 "..\..\..\MainWindow.xaml"
            this.analogOutAChannel1Amp.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.AnalogOutAChannel1Changed);
            
            #line default
            #line hidden
            
            #line 410 "..\..\..\MainWindow.xaml"
            this.analogOutAChannel1Amp.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.AnalogOutAmplitudeValidation);
            
            #line default
            #line hidden
            return;
            case 31:
            this.analogOutAChannel1DC = ((System.Windows.Controls.TextBox)(target));
            
            #line 414 "..\..\..\MainWindow.xaml"
            this.analogOutAChannel1DC.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.AnalogOutAChannel1Changed);
            
            #line default
            #line hidden
            
            #line 414 "..\..\..\MainWindow.xaml"
            this.analogOutAChannel1DC.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.AnalogOutDCValidation);
            
            #line default
            #line hidden
            return;
            case 32:
            this.rxUSBDatarateLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 33:
            this.txUSBDatarateLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 34:
            this.rxUSBDroppedLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 35:
            this.analogInACH1kspsLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 36:
            this.analogInACH2kspsLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 37:
            this.analogInACH3kspsLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 38:
            this.analogInACH4kspsLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 39:
            this.analogInChart = ((LiveCharts.Wpf.CartesianChart)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

